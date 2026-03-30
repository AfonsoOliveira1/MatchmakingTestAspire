using MatchmakingTest.Data.Models;
using MatchmakingTest.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Matchmaking.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayersController(IPlayerService service) : ControllerBase
    {
        private readonly IPlayerService _service = service;

        // GET /players
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        // GET /players/{username}
        [HttpGet("{username}")]
        public async Task<IActionResult> GetPlayer(string username)
        {
            try
            {
                var result = await _service.GetPlayer(username);
                return Ok(result);
            }
            catch (InvalidOperationException ex1)
            {
                return NotFound(ex1.Message);
            }
        }

        // GET /players/matchhistory/{username}
        [HttpGet("matchhistory/{username}")]
        public async Task<IActionResult> MatchHistory(string username)
        {
            try
            {
                var result = await _service.GetPlayerMatches(username);
                return Ok(result);
            }
            catch (Exception ex1)
            {
                return NotFound(ex1.Message);
            }
        }

        // POST /players
        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] string username)
        {
            try
            {
                await _service.CreatePlayer(username);
                return Ok();
            }
            catch (Exception ex1)
            {
                return NotFound(ex1.Message);
            }
        }

        // DELETE /players/{username}
        [HttpDelete("{username}")]
        public async Task<IActionResult> DeletePlayer(string username)
        {
            try
            {
                await _service.DeletePlayer(username);
                return Ok();
            }
            catch (InvalidOperationException ex1)
            {
                return NotFound(ex1.Message);
            }
        }
    }
}