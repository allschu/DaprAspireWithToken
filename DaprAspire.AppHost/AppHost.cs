var builder = DistributedApplication.CreateBuilder(args);

const string daprApiToken = "8824A7E1-8A33-43C3-9F5D-54EC7FFA73FB";

var apiService = builder.AddProject<Projects.DaprAspire_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithDaprSidecar()
    .WithEnvironment("APP_API_TOKEN", daprApiToken);

builder.AddProject<Projects.DaprAspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WithDaprSidecar()
    .WithEnvironment("DAPR_API_TOKEN", daprApiToken);


builder.Build().Run();
