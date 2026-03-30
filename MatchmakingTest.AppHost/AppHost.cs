var builder = DistributedApplication.CreateBuilder(args);

builder.AddDockerComposeEnvironment("dev-environment");

var redis = builder.AddRedis("Redis").WithRedisInsight();

var postgres = builder.AddPostgres("PostgreSql").WithDataVolume().WithPgAdmin();
var db = postgres.AddDatabase("Matchmaking");

var api = builder.AddProject<Projects.Matchmaking_Api>("api")
                     .WithReference(db, "DefaultConnection")
                     .WithReference(redis);

var worker = builder.AddProject<Projects.Matchmaking_Worker>("worker")
                     .WithReference(db, "DefaultConnection")
                    .WithReference(redis);

builder.AddProject<Projects.MatchMakingTest>("web")
    .WithReference(api); 

builder.Build().Run();
