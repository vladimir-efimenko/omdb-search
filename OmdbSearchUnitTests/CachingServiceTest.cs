using OmdbSearchUI.Models;
using OmdbSearchUI.Services;
using Xunit;

namespace OmdbSearchUnitTests
{
    public class CachingServiceTest
    {
        private readonly CachingService _cachingService;

        public CachingServiceTest()
        {
            _cachingService = new CachingService();
        }

        [Fact]
        public void Get_NewSearchRequest_ReturnsNull()
        {
            var search = new SearchRequest("New");

            var searchResult = _cachingService.Get(search);

            Assert.Null(searchResult);
        }

        [Fact]
        public void Get_PutSearch_ReturnsFromCache()
        {
            var search = new SearchRequest("New");

            var searchResult = new SearchResult {TotalResults = 1};
            _cachingService.Put(search, searchResult);

            var result = _cachingService.Get(search);

            Assert.Equal(searchResult, result);
        }

        [Fact]
        public void Put_SixTimesSearch_RemovesTheOldestFromCacheRetainsTheRest()
        {
            _cachingService.Put(new SearchRequest("1"), new SearchResult());
            _cachingService.Put(new SearchRequest("2"), new SearchResult());
            _cachingService.Put(new SearchRequest("3"), new SearchResult());
            _cachingService.Put(new SearchRequest("4"), new SearchResult());
            _cachingService.Put(new SearchRequest("5"), new SearchResult());
            _cachingService.Put(new SearchRequest("6"), new SearchResult());

            var first = _cachingService.Get(new SearchRequest("1"));
            var second = _cachingService.Get(new SearchRequest("2"));
            var third = _cachingService.Get(new SearchRequest("3"));
            var fourth = _cachingService.Get(new SearchRequest("4"));
            var fifth = _cachingService.Get(new SearchRequest("5"));
            var sixth = _cachingService.Get(new SearchRequest("6"));

            Assert.Null(first);
            Assert.NotNull(second);
            Assert.NotNull(third);
            Assert.NotNull(fourth);
            Assert.NotNull(fifth);
            Assert.NotNull(sixth);

        }
    }
}
