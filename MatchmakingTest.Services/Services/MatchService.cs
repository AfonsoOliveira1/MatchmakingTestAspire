using MatchmakingTest.Data.Models;
using MatchmakingTest.Services.Services;
using StackExchange.Redis;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace MatchmakingTest.Services.Controllers
{
    public class MatchService : IMatchService
    {
        private readonly IDatabase _redis;
        public MatchService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task<Match> GetMatch(string matchid)
        {
            var matchId = await _redis.StringGetAsync($"match:{matchid}");
            if (matchId.IsNullOrEmpty)
                throw new InvalidOperationException("No match found");
            Match match = JsonSerializer.Deserialize<Match>(matchId.ToString())!;
            return match;
        }

        public async Task AddToQueue(string username)
        {
            RedisValue value = await _redis.HashGetAsync("players", username);
            if (!value.HasValue)
                throw new InvalidOperationException("Player not found");

            Player player = JsonSerializer.Deserialize<Player>(value.ToString())!;

            if (player.IsOnQueue)
                throw new Exception("Player is already on queue");

            player.IsOnQueue = true;
            await _redis.HashSetAsync("players", username, JsonSerializer.Serialize(player));
            await _redis.ListRightPushAsync("queue", JsonSerializer.Serialize(player));
        }

        public async Task RemoveFromQueue(string username)
        {
            RedisValue value = await _redis.HashGetAsync("players", username);
            if (!value.HasValue)
                throw new InvalidOperationException("Player not found");

            Player player = JsonSerializer.Deserialize<Player>(value.ToString())!;

            if (!player.IsOnQueue)
                throw new Exception("Player is not in queue");

            string originalJson = JsonSerializer.Serialize(player);

            player.IsOnQueue = false;
            string updatedJson = JsonSerializer.Serialize(player);

            await _redis.HashSetAsync("players", username, updatedJson);
            await _redis.ListRemoveAsync("queue", originalJson);
        }
    }
}