using System.Threading.Tasks;
using Moq;
using OmdbSearchUI.Controllers;
using OmdbSearchUI.Models;
using OmdbSearchUI.Services;
using Xunit;

namespace OmdbSearchUnitTests
{
    public class SearchControllerTest
    {
        private readonly Mock<IOmdbApiGateway> _mockOmdbApiGateway;
        private readonly Mock<ICachingService> _mockCachingService;
        private readonly SearchController _searchController;

        public SearchControllerTest()
        {
            _mockOmdbApiGateway = new Mock<IOmdbApiGateway>();
            _mockCachingService = new Mock<ICachingService>();
            _searchController = new SearchController(_mockOmdbApiGateway.Object, _mockCachingService.Object);
        }

        [Theory]
        [InlineData("  ")]
        [InlineData(null)]
        public async Task Search_InvalidSearchRequest_ReturnsEmptyList(string title)
        {
            var searchResult = await _searchController.Search(new SearchRequest(title));

            Assert.Equal(SearchResult.Empty, searchResult);
        }

        [Fact]
        public async Task Search_EarthContent_ReturnsDeserializedSearchResult()
        {
            var searchContent = GetTestSearchContent();

            _mockOmdbApiGateway.Setup(x => x.Search("Earth", 1))
                .Returns(Task.FromResult(searchContent));

            var searchResult = await _searchController.Search(new SearchRequest("Earth"));

            Assert.NotNull(searchResult);
            Assert.Equal(1534, searchResult.TotalResults);
        }

        [Fact]
        public async Task SearchById_MovieContent_ReturnsDeserializedMovie()
        {
            var movieContent = "{\"Title\":\"The Day the Earth Stood Still\"," +
                               "\"Year\":\"1951\"," +
                               "\"Rated\":\"G\"," +
                               "\"Released\":\"25 Dec 1951\"," +
                               "\"Runtime\":\"92 min\"," +
                               "\"Genre\":\"Drama, Sci-Fi\"," +
                               "\"Director\":\"Robert Wise\"," +
                               "\"Writer\":\"Edmund H. North (screen play), Harry Bates (based on a story by)\"," +
                               "\"Actors\":\"Michael Rennie, Patricia Neal, Hugh Marlowe, Sam Jaffe\"," +
                               "\"Plot\":\"An alien lands and tells the people of Earth that they must live peacefully or be destroyed as a danger to other planets.\"," +
                               "\"Language\":\"English, French, Hindi, Russian\"," +
                               "\"Country\":\"USA\"," +
                               "\"Awards\":\"Won 1 Golden Globe. Another 2 wins & 1 nomination.\"," +
                               "\"Poster\":\"https://m.media-amazon.com/images/M/MV5BMTU5NTBmYTAtOTgyYi00NGM0LWE0ODctZjNiYWM5MmIxYzE4XkEyXkFqcGdeQXVyNTAyODkwOQ@@._V1_SX300.jpg\"," +
                               "\"Ratings\":[{\"Source\":\"Internet Movie Database\",\"Value\":\"7.8/10\"},{\"Source\":\"Rotten Tomatoes\",\"Value\":\"94%\"}]," +
                               "\"Metascore\":\"N/A\"," +
                               "\"imdbRating\":\"7.8\"," +
                               "\"imdbVotes\":\"71,164\"," +
                               "\"imdbID\":\"tt0043456\"," +
                               "\"Type\":\"movie\"," +
                               "\"DVD\":\"04 Mar 2003\"," +
                               "\"BoxOffice\":\"N/A\"," +
                               "\"Production\":\"20th Century Fox\"," +
                               "\"Website\":\"N/A\"," +
                               "\"Response\":\"True\"}";

            _mockOmdbApiGateway.Setup(x => x.SearchById("tt0043456"))
                .Returns(Task.FromResult(movieContent));

            var searchResult = await _searchController.SearchById("tt0043456");

            Assert.NotNull(searchResult);
            Assert.Equal("tt0043456", searchResult.ImdbId);
        }

        [Fact]
        public async Task Search_PutsSearchToCache()
        {
            var searchContent = GetTestSearchContent();

            _mockOmdbApiGateway.Setup(x => x.Search("Earth", 1))
                .Returns(Task.FromResult(searchContent));
            _mockCachingService.Setup(x => x.Get(It.IsAny<SearchRequest>()));
            _mockCachingService.Setup(x => x.Put(It.IsAny<SearchRequest>(), It.IsAny<SearchResult>()));

            var searchRequest = new SearchRequest("Earth");

            await _searchController.Search(searchRequest);

            _mockCachingService.Verify(cache => cache.Get(searchRequest), Times.Once);
            _mockCachingService.Verify(cache => cache.Put(searchRequest, It.IsAny<SearchResult>()), Times.Once);
            _mockOmdbApiGateway.Verify(gateway => gateway.Search(searchRequest.Title, searchRequest.Page), Times.Once);
        }

        private string GetTestSearchContent()
        {
            return "{\"Search\":[" +
                                "{\"Title\":\"After Earth\",\"Year\":\"2013\",\"imdbID\":\"tt1815862\",\"Type\":\"movie\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BMTY3MzQyMjkwMl5BMl5BanBnXkFtZTcwMDk2OTE0OQ@@._V1_SX300.jpg\"}," +
                                "{\"Title\":\"The Man from Earth\",\"Year\":\"2007\",\"imdbID\":\"tt0756683\",\"Type\":\"movie\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BMzQ5NGQwOTUtNWJlZi00ZTFiLWI0ZTEtOGU3MTA2ZGU5OWZiXkEyXkFqcGdeQXVyMTczNjQwOTY@._V1_SX300.jpg\"}," +
                                "{\"Title\":\"The Day the Earth Stood Still\",\"Year\":\"2008\",\"imdbID\":\"tt0970416\",\"Type\":\"movie\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BMTI5NTg1MzU5Nl5BMl5BanBnXkFtZTcwMDU1ODMwMg@@._V1_SX300.jpg\"}," +
                                "{\"Title\":\"Planet Earth\",\"Year\":\"2006\",\"imdbID\":\"tt0795176\",\"Type\":\"series\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BNmZlYzIzMTItY2EzYS00YTEyLTg0ZjEtMDMzZjM3ODdhN2UzXkEyXkFqcGdeQXVyNjI0MDg2NzE@._V1_SX300.jpg\"}," +
                                "{\"Title\":\"Like Stars on Earth\",\"Year\":\"2007\",\"imdbID\":\"tt0986264\",\"Type\":\"movie\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BNTVmYTk2NjAtYzY3MS00YjFjLTlkYzktYzg3YzMyZDQyOWRiXkEyXkFqcGdeQXVyNjQ2MjQ5NzM@._V1_SX300.jpg\"}," +
                                "{\"Title\":\"Journey to the Center of the Earth\",\"Year\":\"2008\",\"imdbID\":\"tt0373051\",\"Type\":\"movie\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BMTk1MzY1MzU1MF5BMl5BanBnXkFtZTcwOTQ2NjM3MQ@@._V1_SX300.jpg\"}," +
                                "{\"Title\":\"Another Earth\",\"Year\":\"2011\",\"imdbID\":\"tt1549572\",\"Type\":\"movie\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BMTAzNTIzMjkxNjJeQTJeQWpwZ15BbWU3MDEwNDQ2OTU@._V1_SX300.jpg\"}," +
                                "{\"Title\":\"Battlefield Earth\",\"Year\":\"2000\",\"imdbID\":\"tt0185183\",\"Type\":\"movie\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BMTg0Njk2OTM3OF5BMl5BanBnXkFtZTYwNTAyMzc3._V1_SX300.jpg\"}," +
                                "{\"Title\":\"The Day the Earth Stood Still\",\"Year\":\"1951\",\"imdbID\":\"tt0043456\",\"Type\":\"movie\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BMTU5NTBmYTAtOTgyYi00NGM0LWE0ODctZjNiYWM5MmIxYzE4XkEyXkFqcGdeQXVyNTAyODkwOQ@@._V1_SX300.jpg\"}," +
                                "{\"Title\":\"Planet Earth II\",\"Year\":\"2016\",\"imdbID\":\"tt5491994\",\"Type\":\"series\",\"Poster\":\"https://m.media-amazon.com/images/M/MV5BZWYxODViMGYtMGE2ZC00ZGQ3LThhMWUtYTVkNGE3OWU4NWRkL2ltYWdlL2ltYWdlXkEyXkFqcGdeQXVyMjYwNDA2MDE@._V1_SX300.jpg\"}],\"totalResults\":\"1534\",\"Response\":\"True\"}";
        }
    }
}
