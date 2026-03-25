var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("dev-environment");

var redis = builder.AddRedis("Redis").WithRedisInsight();

var api = builder.AddProject<Projects.Matchmaking_Api>("api")
                 .WithReference(redis);

var worker = builder.AddProject<Projects.Matchmaking_Worker>("worker")
                    .WithReference(redis);

builder.AddProject<Projects.MatchMakingTest>("matchmakingtest");

builder.Build().Run();
