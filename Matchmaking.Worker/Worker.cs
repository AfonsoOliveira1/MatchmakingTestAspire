using MatchmakingTest.Data.Models;
using StackExchange.Redis;
using System.Text.Json;

public class Worker : BackgroundService
{
    private readonly IDatabase _redis;

    public Worker(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var queue = await _redis.ListRangeAsync("queue", 0, 1);

            if (queue.Length >= 2)
            {
                List<Player> players = queue.Select(p => JsonSerializer.Deserialize<Player>(p.ToString()))
                                                .Where(p => p.IsOnQueue == true && p != null)
                                                .ToList();
                Player p1 = players[0];
                Player p2 = players[1];

                p1.IsOnQueue = false; p2.IsOnQueue = false;
                p1.OnMatch = true; p2.OnMatch = true;

                Match match = new Match
                {
                    Id = Guid.NewGuid().ToString(),
                    Player1 = p1.Username,
                    Player2 = p2.Username,
                    Start = DateTime.Now,
                };

                p1.MatchHistory.Add(match);
                p2.MatchHistory.Add(match);

                await _redis.StringSetAsync($"match:{match.Id}", JsonSerializer.Serialize(match));

                await _redis.HashSetAsync("players", p1.Username, JsonSerializer.Serialize(p1));
                await _redis.HashSetAsync("players", p2.Username, JsonSerializer.Serialize(p2));

                await _redis.ListLeftPopAsync("queue");
                await _redis.ListLeftPopAsync("queue");
            }
            await Task.Delay(5, stoppingToken);
        }
    }

}
