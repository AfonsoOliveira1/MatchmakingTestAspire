using MatchmakingTest.Data.Models;
using MatchmakingTest.Services.Services;
using StackExchange.Redis;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace MatchmakingTest.Services.Controllers
{
    public class PlayerService : IPlayerService
    {
        private readonly IDatabase _redis;
        public PlayerService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<List<Player>> GetAll()
        {
            HashEntry[] entries = await _redis.HashGetAllAsync("players");
            List<Player> players = entries
                .Select(e => JsonSerializer.Deserialize<Player>(e.Value.ToString()))
                .Where(p => p != null)
                .ToList();

            if (players == null)
                return new List<Player>();
            return players;
        }

        public async Task<Player> GetPlayer(string username)
        {
            RedisValue value = await _redis.HashGetAsync("players", username);
            if (!value.HasValue)
                throw new InvalidOperationException("Player not found");

            Player? player = JsonSerializer.Deserialize<Player>((string)value!);
            return player;
        }

        public async Task CreatePlayer(string username)
        {
            bool usernameTaken = await _redis.HashExistsAsync("players", username);
            if (usernameTaken)
                throw new Exception("Username already in use");

            var player = new Player
            {
                Username = username,
                MatchHistory = new List<Match>()
            };

            await _redis.HashSetAsync("players", username, JsonSerializer.Serialize(player));
        }
        public async Task DeletePlayer(string username)
        {
            bool deleted = await _redis.HashDeleteAsync("players", username);
            if (!deleted)
                throw new InvalidOperationException("Player not found");
        }
    }
}