using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Redis (Aspire injeta a connection string automaticamente)
var redisConnection = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnection)
);

// Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//// Endpoints

//app.MapGet("/match/{playerId}", async (IDistributedCache cache, string playerId) =>
//{
//    var matchId = await cache.GetStringAsync($"match:{playerId}");
//    return matchId is null ? Results.NotFound() : Results.Ok(matchId);
//});

//app.MapPost("/queue/{playerId}", async (IDistributedCache cache, string playerId) =>
//{
//    var queue = await cache.GetStringAsync("queue") ?? "";
//    queue += playerId + ",";
//    await cache.SetStringAsync("queue", queue);
//    return Results.Ok();
//});

app.MapControllers();

app.Run();
