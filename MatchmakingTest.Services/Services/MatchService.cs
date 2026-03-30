using MatchmakingTest.Data;
using MatchmakingTest.Data.Models;
using MatchmakingTest.Services.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace MatchmakingTest.Services.Controllers
{
    public class MatchService : IMatchService
    {
        private readonly IPlayerService _playerService; 
        private readonly IDatabase _redis;
        private readonly AppDbContext _context;
        public MatchService(IConnectionMultiplexer redis, AppDbContext context, IPlayerService playerservice)
        {
            _redis = redis.GetDatabase();
            _context = context;
            _playerService = playerservice;
        }

        public async Task<Match> GetMatch(string matchid)
        {
            var matchId = await _redis.StringGetAsync($"match:{matchid}");
            if (matchId.IsNullOrEmpty)
            {
                var match = await _context.Matches.FirstOrDefaultAsync(i => i.Id == matchid);
                if(match != null)
                {
                    return match;
                }

                throw new InvalidOperationException("No match found");
            }
            else
            {
                Match match = JsonSerializer.Deserialize<Match>(matchId.ToString())!;
                return match;
            }
        }
        public async Task StartMatch(Match match)
        {
            match.Start = DateTime.UtcNow;
            match.Ended = null;
            await _redis.StringSetAsync($"match:{match.Id}", JsonSerializer.Serialize(match));
            await AddMatch(match);
        }
        public async Task AddMatch(Match match)
        {
            _context.Matches.Add(match);
            await _context.SaveChangesAsync();
        }

        public async Task AddToQueue(string username)
        {
            //nao quero guardar o estado do player na queue na base de dados
            Player player = await _playerService.GetPlayer(username);

            if (player.IsOnQueue)
                throw new Exception("Player is already on queue");

            player.IsOnQueue = true;
            player.QueueStart = DateTime.UtcNow;
            await _redis.HashSetAsync("players", username, JsonSerializer.Serialize(player));
            await _redis.ListRightPushAsync("queue", JsonSerializer.Serialize(player));
        }


        public async Task RemoveFromQueue(string username)
        {   
            //nao quero guardar o estado do player na queue na base de dados
            Player player = await _playerService.GetPlayer(username);

            if (!player.IsOnQueue)
                throw new Exception("Player is not in queue");

            string originalJson = JsonSerializer.Serialize(player);

            player.IsOnQueue = false;
            player.QueueStart = null;
            string updatedJson = JsonSerializer.Serialize(player);

            await _redis.HashSetAsync("players", username, updatedJson);
            await _redis.ListRemoveAsync("queue", originalJson);
        }

        public async Task<(Player p1, Player p2)> GetTwoPlayersAsync()
        {
            var queue = await _redis.ListRangeAsync("queue", 0, 1);
            
            if(queue.Length < 2)
                return (null, null);

            var p1Json = await _redis.ListLeftPopAsync("queue");
            var p2Json = await _redis.ListLeftPopAsync("queue");

            Player p1 = JsonSerializer.Deserialize<Player>(p1Json.ToString());
            Player p2 = JsonSerializer.Deserialize<Player>(p2Json.ToString());

            return (p1, p2);
        }


        public async Task EndMatch(string matchid)
        {
            Match match = await GetMatch(matchid);

            match.Ended = DateTime.UtcNow;

            _context.Matches.Update(match);
            await _context.SaveChangesAsync();

            var p1 = await _playerService.GetPlayer(match.Player1);
            var p2 = await _playerService.GetPlayer(match.Player2);

            p1.OnMatch = false;
            p2.OnMatch = false;

            await _redis.HashSetAsync("players", p1.Username, JsonSerializer.Serialize(p1));
            await _redis.HashSetAsync("players", p2.Username, JsonSerializer.Serialize(p2));
        }
    }
}