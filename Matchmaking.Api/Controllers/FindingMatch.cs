using Microsoft.AspNetCore.Mvc;
using MatchmakingTest.Data.Models;
using System.Text.Json;
using MatchmakingTest.Services.Services;

namespace Matchmaking.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatchmakingController(IMatchService service) : ControllerBase
    {
        private readonly IMatchService _service = service;

        // GET /matchmaking/match/{username}
        [HttpGet("match/{matchid}")]
        public async Task<IActionResult> GetMatch(string matchid)
        {
            try
            {
                var result = await _service.GetMatch(matchid);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST /matchmaking/addmatch
        [HttpPost("addmatch")]
        public async Task<IActionResult> AddAMatch(Match match)
        {
            try
            {
                await _service.AddMatch(match);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /matchmaking/queue/{username}
        [HttpPost("queue/{username}")]
        public async Task<IActionResult> AddToQueue(string username)
        {
            try
            {
                await _service.AddToQueue(username);
                return Ok();
            }
            catch (InvalidOperationException ex1)
            {
                return NotFound(ex1.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /matchmaking/queue/{username}
        [HttpDelete("queue/{username}")]
        public async Task<IActionResult> RemoveFromQueue(string username)
        {
            try
            {
                await _service.RemoveFromQueue(username);
                return Ok();
            }
            catch (InvalidOperationException ex1)
            {
                return NotFound(ex1.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /matchmaking/matchend/{matchid}
        [HttpPut("matchendID/{matchid}")]
        public async Task<IActionResult> EndMatch(string matchid)
        {
            try
            {
                await _service.EndMatchId(matchid);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /matchmaking/matchend/{username}
        [HttpPut("matchendUsername/{username}")]
        public async Task<IActionResult> EndMatchUsername(string username)
        {
            try
            {
                await _service.EndMatchUsername(username);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}