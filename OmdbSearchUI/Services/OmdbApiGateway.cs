using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OmdbSearchUI.Services
{
    public class OmdbApiGateway : IOmdbApiGateway
    {
        private static readonly string BASE_SERVICE_URL = "http://www.omdbapi.com/";
        private static readonly string API_KEY = "6c658e1";

        public async Task<string> Search(string title, int page)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("title cannot be null or empty");
            }

            return await DownloadContent(GetUrlForSearchTerm($"?s={title}&page={page}"));
        }

        public async Task<string> SearchById(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("title cannot be null or empty");
            }

            return await DownloadContent(GetUrlForSearchTerm("?i=" + title));
        }

        private async Task<string> DownloadContent(string url)
        {
            using (var client = new HttpClient())
            {
                return await client.GetStringAsync(url);
            }
        }

        private string GetUrlForSearchTerm(string searchTerm)
        {
            return $"{BASE_SERVICE_URL}{searchTerm}&apikey={API_KEY}";
        }
    }
}
