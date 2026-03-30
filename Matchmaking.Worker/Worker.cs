using MatchmakingTest.Data.Models;
using MatchmakingTest.Services.Services;
using StackExchange.Redis;
using System.Text.Json;

public class Worker : BackgroundService
{
    private readonly IDatabase _redis;
    private readonly IServiceProvider _provider;

    public Worker(IConnectionMultiplexer redis, IServiceProvider provider)
    {
        _redis = redis.GetDatabase();
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMatchService>();

            var (p1, p2) = await service.GetTwoPlayersAsync();

            if (p1 == null || p2 == null)
            {
                await Task.Delay(1000, stoppingToken);
                continue;
            }

            Match match = new Match
            {
                Id = Guid.NewGuid().ToString(),
                Player1 = p1.Username,
                Player2 = p2.Username,
            };

            p1.IsOnQueue = false; p2.IsOnQueue = false;
            p1.OnMatch = true; p2.OnMatch = true;

            await service.StartMatch(match);

            await _redis.HashSetAsync("players", p1.Username, JsonSerializer.Serialize(p1));
            await _redis.HashSetAsync("players", p2.Username, JsonSerializer.Serialize(p2));

            await Task.Delay(500, stoppingToken);
        }
    }
}
