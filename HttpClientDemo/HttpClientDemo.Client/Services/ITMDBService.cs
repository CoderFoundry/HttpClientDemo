using HttpClientDemo.Client.Models;

namespace HttpClientDemo.Client.Services
{
    public interface ITMDBService
    {
        Task<List<Movie>> SearchMoviesAsync(string query, int page = 1);

        Task<MovieDetails> GetMovieDetailsAsync(int movieId);
       
    }
}
