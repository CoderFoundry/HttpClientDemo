using HttpClientDemo.Client.Models;
using HttpClientDemo.Client.Services;
using System.Text.Json;


namespace HttpClientDemo.Services
{
    public class TMDBService(HttpClient http) : ITMDBService
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public async Task<List<Movie>> SearchMoviesAsync(string query, int page = 1)
        {
            // BAD: each call reuses the same HttpClient, never disposed
            var url = $"search/movie?query={Uri.EscapeDataString(query)}&region=US&include_adult=false&language=en-US&page={page}";
            var response = await http.GetFromJsonAsync<MovieResponse>(url, _jsonOptions)
                           ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Search results could not be loaded");
            return response.Results.Where(r => r.VoteCount > 150).ToList();
        }

        public async Task<MovieDetails> GetMovieDetailsAsync(int movieId)
        {
            var url = $"movie/{movieId}?append_to_response=videos,credits&language=en-US";
           
            MovieDetails? response = await http.GetFromJsonAsync<MovieDetails>(url, _jsonOptions)
                ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Movie details could not be loaded");

            response.PosterPath = string.IsNullOrEmpty(response.PosterPath)
                                ? "/images/poster.png"
                                : $"http://image.tmdb.org/t/p/w500{response.PosterPath}";

            response.BackdropPath = string.IsNullOrEmpty(response.BackdropPath)
                               ? "/images/backdrop.jpg"
                               : $"http://image.tmdb.org/t/p/w500{response.BackdropPath}";

            return response;
        }

       
    }
}
