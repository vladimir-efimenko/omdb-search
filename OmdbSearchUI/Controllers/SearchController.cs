using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OmdbSearchUI.Models;
using OmdbSearchUI.Services;

namespace OmdbSearchUI.Controllers
{
    [Route("api")]
    public class SearchController : ControllerBase
    {
        private readonly IOmdbApiGateway _omdbApiGateway;
        private readonly ICachingService _cachingService;

        public SearchController(IOmdbApiGateway omdbApiGateway, ICachingService cachingService)
        {
            _omdbApiGateway = omdbApiGateway;
            _cachingService = cachingService;
        }

        [HttpGet("search")]
        public async Task<SearchResult> Search(SearchRequest searchRequest)
        {
            if (IsSearchRequestValid(searchRequest))
            {
                var searchResult = _cachingService.Get(searchRequest);
                if (searchResult != null)
                {
                    return searchResult;
                }
                var content = await _omdbApiGateway.Search(searchRequest.Title, searchRequest.Page);

                searchResult = JsonConvert.DeserializeObject<SearchResult>(content);
                _cachingService.Put(searchRequest, searchResult);

                return searchResult;
            }

            return SearchResult.Empty;
        }

        private bool IsSearchRequestValid(SearchRequest searchRequest)
        {
            return !string.IsNullOrWhiteSpace(searchRequest?.Title);
        }

        [HttpGet("movie/{id}")]
        public async Task<Movie> SearchById(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var content = await _omdbApiGateway.SearchById(id);

                return JsonConvert.DeserializeObject<Movie>(content);
            }

            return null;
        }
    }
}
