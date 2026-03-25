using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder(args);

// worker vai consumir redis
var redisConnection = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnection)
);


builder.AddServiceDefaults();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
