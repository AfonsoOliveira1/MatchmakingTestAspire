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

        public async Task<List<Match>> GetPlayerMatchHistoryAsync(string username)
        {
            return await _http.GetFromJsonAsync<List<Match>>($"players/matchhistory/{username}")
                   ?? new();
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
        public async Task AddToQueueAsync(string username)
        {
            await _http.PostAsync($"matchmaking/queue/{username}", null);
        }

        public async Task RemoveFromQueueAsync(string username)
        {
            await _http.DeleteAsync($"matchmaking/queue/{username}");
        }

        // Matches
        public async Task<Match?> GetMatchAsync(string matchid)
        {
            return await _http.GetFromJsonAsync<Match>($"matchmaking/match/{matchid}");
        }

        public async Task AddMatchAsync(Match match)
        {
            await _http.PostAsJsonAsync("matchmaking/addmatch", match);
        }

        public async Task EndMatchAsync(string matchid)
        {
            await _http.PutAsync($"matchmaking/matchend/{matchid}", null);
        }
    }
}
