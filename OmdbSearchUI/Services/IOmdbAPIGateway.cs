using System.Threading.Tasks;

namespace OmdbSearchUI.Services
{
    public interface IOmdbApiGateway
    {
        /// <summary>
        /// Search movies by the specified title.
        /// </summary>
        Task<string> Search(string title, int page);

        /// <summary>
        /// Searches a movie by the specified id and returns a movie with detailed information if found
        /// </summary>
        Task<string> SearchById(string id);
    }
}
