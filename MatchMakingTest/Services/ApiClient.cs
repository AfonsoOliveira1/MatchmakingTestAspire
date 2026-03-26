using MatchmakingTest.Data.Models;

namespace MatchMakingTest.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;

        public ApiClient(HttpClient client)
        {
            _http = client;
        }

        // Players
        public async Task<List<Player>> GetAllPlayersAsync()
        {
            return await _http.GetFromJsonAsync<List<Player>>("players") ?? new();
        }

        public async Task<Player?> GetPlayerAsync(string username)
        {
            return await _http.GetFromJsonAsync<Player>($"players/{username}");
        }

        public async Task CreatePlayerAsync(string username)
        {
            await _http.PostAsJsonAsync("players", username);
        }

        public async Task DeletePlayerAsync(string username)
        {
            await _http.DeleteAsync($"players/{username}");
        }

        // Queue
        public async Task<Match?> GetMatchAsync(string matchid)
        {
            return await _http.GetFromJsonAsync<Match>($"matchmaking/match/{matchid}");
        }

        public async Task AddToQueueAsync(string username)
        {
            await _http.PostAsync($"matchmaking/queue/{username}", null);
        }

        public async Task RemoveFromQueueAsync(string username)
        {
            await _http.DeleteAsync($"matchmaking/queue/{username}");
        }
    }
}
