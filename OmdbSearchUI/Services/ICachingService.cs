using OmdbSearchUI.Models;

namespace OmdbSearchUI.Services
{
    public interface ICachingService
    {
        SearchResult Get(SearchRequest searchRequest);

        void Put(SearchRequest searchRequest, SearchResult searchResult);
    }
}
