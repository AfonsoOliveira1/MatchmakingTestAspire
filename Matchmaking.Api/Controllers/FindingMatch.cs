using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

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
        [HttpGet]
        public async Task<IActionResult> ola()
        {
            return Ok("a");
        }

        // GET /matchmaking/match/{playerId}
        [HttpGet("match/{playerId}")]
        public async Task<IActionResult> GetMatch(string playerId)
        {
            var matchId = await _redis.StringGetAsync($"match:{playerId}");
            if (matchId.IsNullOrEmpty)
                return NotFound();

            return Ok(matchId.ToString());
        }

        // POST /matchmaking/queue/{playerId}
        [HttpPost("queue/{playerId}")]
        public async Task<IActionResult> AddToQueue(string playerId)
        {
            var queue = await _redis.ListRangeAsync("queue");
            if (queue.Any(x => x.ToString() == playerId))
                return BadRequest("Player is already in the queue.");

            await _redis.ListRightPushAsync("queue", playerId);

            return Ok();
        }   
    }
}
