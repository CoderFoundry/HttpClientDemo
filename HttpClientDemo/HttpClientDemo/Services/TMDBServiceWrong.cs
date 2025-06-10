using HttpClientDemo.Client.Services;
using HttpClientDemo.Client.Models;
using System.Text.Json;

namespace HttpClientDemo.Services
{
    // Note: no IHttpClientFactory or HttpClient injected here!
    public class TMDBServiceWrong : ITMDBService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };

        public TMDBServiceWrong(IConfiguration config)
        {
            // BAD: creating a brand-new HttpClient per service instance
            _http = new HttpClient
            {
                BaseAddress = new Uri("https://api.themoviedb.org/3/")
            };

            const string configKey = "TmdbApiKey";
            string? devKey = config[configKey];
            string? prodKey = Environment.GetEnvironmentVariable(configKey);
            string? key = string.IsNullOrEmpty(devKey) ? prodKey : devKey;

            if (!string.IsNullOrEmpty(key))
            {
                _http.DefaultRequestHeaders.Authorization = new("Bearer", key);
            }
            else
            {
                Console.WriteLine($"No TMDB API key found in {configKey} or environment variables.");
            }
        }

        public async Task<List<Movie>> SearchMoviesAsync(string query, int page = 1)
        {
            // BAD: each call reuses the same HttpClient, never disposed
            var url = $"search/movie?query={Uri.EscapeDataString(query)}&region=US&include_adult=false&language=en-US&page={page}";
            var response = await _http.GetFromJsonAsync<MovieResponse>(url, _jsonOptions)
                           ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Search results could not be loaded");
            return response.Results.Where(r => r.VoteCount > 150).ToList();
        }

        public async Task<MovieDetails> GetMovieDetailsAsync(int movieId)
        {
            var url = $"movie/{movieId}?append_to_response=videos,credits&language=en-US";
            var details = await _http.GetFromJsonAsync<MovieDetails>(url, _jsonOptions)
                              ?? throw new HttpIOException(HttpRequestError.InvalidResponse, "Movie details could not be loaded");

            details.PosterPath = string.IsNullOrEmpty(details.PosterPath)
                                ? "/images/poster.png"
                                : $"http://image.tmdb.org/t/p/w500{details.PosterPath}";
            details.BackdropPath = string.IsNullOrEmpty(details.BackdropPath)
                                ? "/images/backdrop.jpg"
                                : $"http://image.tmdb.org/t/p/w500{details.BackdropPath}";

            return details;
        }


    }
}