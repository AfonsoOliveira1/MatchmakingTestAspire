using Microsoft.AspNetCore.Mvc;
using MatchmakingTest.Data.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Matchmaking.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatchmakingController : ControllerBase
    {
        private readonly IDatabase _redis;

        public MatchmakingController(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        // GET /matchmaking/match/{username}
        [HttpGet("match/{matchid}")]
        public async Task<IActionResult> GetMatch(string matchid)
        {
            var matchId = await _redis.StringGetAsync($"match:{matchid}");
            if (matchId.IsNullOrEmpty)
                return NotFound("No match found");

            return Ok(matchId.ToString());
        }

        // POST /matchmaking/queue/{username}
        [HttpPost("queue/{username}")]
        public async Task<IActionResult> AddToQueue(string username)
        {
            RedisValue value = await _redis.HashGetAsync("players", username);
            if (!value.HasValue)
                return NotFound("Player not found");

            Player player = JsonSerializer.Deserialize<Player>(value.ToString())!;

            if (player.IsOnQueue)
                return Conflict("Player is already on queue");

            player.IsOnQueue = true;
            await _redis.HashSetAsync("players", username, JsonSerializer.Serialize(player));
            await _redis.ListRightPushAsync("queue", JsonSerializer.Serialize(player));

            return Ok();
        }

        // DELETE /matchmaking/queue/{username}
        [HttpDelete("queue/{username}")]
        public async Task<IActionResult> RemoveFromQueue(string username)
        {
            RedisValue value = await _redis.HashGetAsync("players", username);
            if (!value.HasValue)
                return NotFound("Player not found");

            Player player = JsonSerializer.Deserialize<Player>(value.ToString())!;

            if (!player.IsOnQueue)
                return Conflict("Player is not in queue");

            player.IsOnQueue = false;
            await _redis.HashSetAsync("players", username, JsonSerializer.Serialize(player));
            await _redis.ListRemoveAsync("queue", JsonSerializer.Serialize(player));

            return Ok();
        }
    }
}