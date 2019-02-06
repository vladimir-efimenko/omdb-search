using System.Collections.Generic;

namespace OmdbSearchUI.Models
{
    public class SearchResult
    {
        public ICollection<Search> Search { get; set; }
        public int TotalResults { get; set; }
        public string Response { get; set; }
        public string Error { get; set; }

        public static readonly SearchResult Empty = new SearchResult
        {
            Search = new List<Search>(),
            TotalResults = 0,
            Response = "False"
        };
    }
}
    