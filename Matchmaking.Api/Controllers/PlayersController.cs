using Microsoft.AspNetCore.Mvc;
using MatchmakingTest.Data.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace Matchmaking.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly IDatabase _redis;

        public PlayersController(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        // GET /players
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            HashEntry[] entries = await _redis.HashGetAllAsync("players");
            List<Player> players = entries
                .Select(e => JsonSerializer.Deserialize<Player>(e.ToString()))
                .Where(p => p != null)!
                .ToList();

            return Ok(players);
        }

        // GET /players/{username}
        [HttpGet("{username}")]
        public async Task<IActionResult> GetPlayer(string username)
        {
            RedisValue value = await _redis.HashGetAsync("players", username);
            if (!value.HasValue)
                return NotFound("Player not found");

            Player? player = JsonSerializer.Deserialize<Player>((string)value!);
            return Ok(player);
        }

        // POST /players
        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] string username)
        {
            bool usernameTaken = await _redis.HashExistsAsync("players", username);
            if (usernameTaken)
                return Conflict("Username already in use");

            var player = new Player
            {
                Username = username,
                MatchHistory = new List<Match>()
            };

            await _redis.HashSetAsync("players", username, JsonSerializer.Serialize(player));
            return Ok(player);
        }

        // DELETE /players/{username}
        [HttpDelete("{username}")]
        public async Task<IActionResult> DeletePlayer(string username)
        {
            bool deleted = await _redis.HashDeleteAsync("players", username);
            if (!deleted)
                return NotFound("Player not found");

            return Ok();
        }
    }
}