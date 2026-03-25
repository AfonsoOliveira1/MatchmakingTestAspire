using StackExchange.Redis;

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
            var players = await _redis.ListRangeAsync("queue", 0, 1);

            if (players.Length >= 2)
            {
                var p1 = players[0].ToString();
                var p2 = players[1].ToString();

                var matchId = Guid.NewGuid().ToString();

                await _redis.StringSetAsync($"match:{p1}", matchId);
                await _redis.StringSetAsync($"match:{p2}", matchId);

                await _redis.ListLeftPopAsync("queue");
                await _redis.ListLeftPopAsync("queue");
            }
        }
    }

}
