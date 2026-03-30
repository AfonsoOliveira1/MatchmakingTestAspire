using MatchmakingTest.Data.Models;
using MatchmakingTest.Data;
using MatchmakingTest.Services.Services;
using StackExchange.Redis;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MatchmakingTest.Services.Controllers
{
    public class PlayerService : IPlayerService
    {
        private readonly IDatabase _redis;
        private readonly AppDbContext _context;

        public PlayerService(IConnectionMultiplexer redis, AppDbContext context)
        {
            _redis = redis.GetDatabase();
            _context = context;
        }

        public async Task<List<Player>> GetAll()
        {
            /* Acho que aqui é sempre ir a base de dados a cache nao vai ter todos os players
            HashEntry[] entries = await _redis.HashGetAllAsync("players");
            List<Player> players = entries
                .Select(e => JsonSerializer.Deserialize<Player>(e.Value.ToString()))
                .Where(p => p != null)
                .ToList();

            if (players == null)
                return new List<Player>();
            */
            var players = await _context.Players.ToListAsync();
            return players;
        }

        public async Task<Player> GetPlayer(string username)
        {
            RedisValue value = await _redis.HashGetAsync("players", username);
            if (!value.HasValue)
            {
                var player = await _context.Players.FirstOrDefaultAsync(p => p.Username == username);
                if(player != null)
                    return player;

                throw new InvalidOperationException("Player not found");
            }
            else
            {
                Player? player = JsonSerializer.Deserialize<Player>((string)value!);
                return player;
            }
        }

        public async Task<List<Match>> GetPlayerMatches(string username)
        {
            var player = await GetPlayer(username);
            player.MatchHistory = await _context.Matches
                .Where(m => m.Player1 == username || m.Player2 == username)
                .ToListAsync();
            return player.MatchHistory;
        }

        public async Task CreatePlayer(string username)
        {
            bool usernameTaken = await _redis.HashExistsAsync("players", username);
            if (usernameTaken)
                throw new Exception("Username already in use");

            var player = new Player
            {
                Username = username,
            };

            await _context.Players.AddAsync(player);
            await _context.SaveChangesAsync();
            await _redis.HashSetAsync("players", username, JsonSerializer.Serialize(player));
        }

        public async Task DeletePlayer(string username)
        {
            Player player = await GetPlayer(username);

            if(player != null)
            {
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
                await _redis.HashDeleteAsync("players", username);
            }
            else
            {
                throw new InvalidOperationException("Player not found");
            }
        }
    }
}