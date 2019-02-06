using System.Collections.Generic;
using OmdbSearchUI.Models;

namespace OmdbSearchUI.Services
{
    /// <summary>
    /// Simple in-memory cache which stores last five search results
    /// </summary>
    public class CachingService : ICachingService
    {
        private static readonly int MAX_SIZE = 5;
        private readonly IDictionary<SearchRequest, SearchResult> _cache = new Dictionary<SearchRequest, SearchResult>();
        private readonly LinkedList<SearchRequest> _cacheKeys = new LinkedList<SearchRequest>();
        private readonly object _lock = new object();

        public SearchResult Get(SearchRequest searchRequest)
        {
            lock (_lock)
            {
                if (_cache.ContainsKey(searchRequest))
                {
                    return _cache[searchRequest];
                }
            }

            return null;
        }

        public void Put(SearchRequest searchRequest, SearchResult searchResult)
        {
            lock (_lock)
            {
                if (IsFull())
                {
                    // Remove oldest if cache is full
                    _cache.Remove(_cacheKeys.First.Value);
                    _cacheKeys.RemoveFirst();
                }
                _cache.Add(searchRequest, searchResult);
                _cacheKeys.AddLast(searchRequest);
            }
        }

        private bool IsFull()
        {
            return _cache.Count == MAX_SIZE;
        }
    }
}
