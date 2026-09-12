var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");
var authenticationDatabase = builder.AddSqlServer("auth-sql")
    .AddDatabase("authdb");

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(authenticationDatabase)
    .WaitFor(authenticationDatabase)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
