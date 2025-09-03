# Dapr Aspire Integration Example

This repository demonstrates a working integration between **Dapr** and **.NET Aspire**, showcasing service-to-service communication with token-based authentication in a microservices architecture.

## Security & Token Configuration

The solution implements **token-based authentication** using Dapr's built-in authentication capabilities:

### Key Security Features

- **DAPR_API_TOKEN**: Used by the Web frontend to authenticate with Dapr sidecar
- **APP_API_TOKEN**: Used by the API service for service-to-service authentication
- Both tokens are configured with the same value for this example: `8824A7E1-8A33-43C3-9F5D-54EC7FFA73FB`

### Token Flow

1. The **Web frontend** uses `DAPR_API_TOKEN` to communicate with its Dapr sidecar
2. The **API service** validates incoming requests using `APP_API_TOKEN`
3. Service-to-service communication is secured through Dapr's authentication middleware


## Key Features

### Dapr Integration
- **Service Invocation**: Direct service-to-service communication via Dapr
- **Sidecar Pattern**: Each service runs with its own Dapr sidecar
- **Authentication**: Token-based security for all Dapr operations

### .NET Aspire Integration
- **Service Discovery**: Automatic service registration and discovery
- **Health Checks**: Built-in health monitoring for all services
- **External Endpoints**: Web frontend exposed with external HTTP endpoints
- **Environment Configuration**: Centralized token management

### Service Communication
- The Web frontend calls the API service using `DaprClient.InvokeMethodAsync()`
- API endpoints are protected with `[RequireAuthorization]`
- Weather forecast data is retrieved and displayed through the Blazor UI

## Setup & Running

### Prerequisites
- .NET 8 SDK
- Dapr CLI
- Docker (for Dapr dependencies)

### Running the Application

1. **Initialize Dapr** (if not already done):
   ```bash
   dapr init
   ```

2. **Start the Aspire Host**:
   ```bash
   cd DaprAspire.AppHost
   dotnet run
   ```

3. **Access the Application**:
   - Web Frontend: Available through Aspire dashboard
   - API Service: Available through Aspire dashboard
   - Aspire Dashboard: Typically at `https://localhost:5000`

## Configuration Details

### AppHost Configuration (DaprAspire.AppHost/AppHost.cs)

```csharp

// FAKE DO NOT USE THIS TOKEN IN PRODUCTION!
const string daprApiToken = "8824A7E1-8A33-43C3-9F5D-54EC7FFA73FB";

var apiService = builder.AddProject<Projects.DaprAspire_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithDaprSidecar()
    .WithEnvironment("APP_API_TOKEN", daprApiToken); // <-- IMPORTANT >

builder.AddProject<Projects.DaprAspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WithDaprSidecar()
    .WithEnvironment("DAPR_API_TOKEN", daprApiToken); // <-- IMPORTANT >
```

### API Service Authentication

The API service uses Dapr authentication extensions:

```csharp
builder.Services
    .AddAuthentication()
    .AddDapr();

builder.Services.AddAuthorization(options => options.AddDapr());
```

### Web Client Communication

The web frontend uses `DaprClient` for service invocation:

```csharp
await client.InvokeMethodAsync<List<WeatherForecast>>(
    HttpMethod.Get,
    "apiservice",
    "weatherforecast",
    cancellationToken);
```

## Security Notes

**Important**: The tokens used in this example are for demonstration purposes only. In production environments:

- Use secure, randomly generated tokens
- Store tokens in secure configuration (Azure Key Vault, etc.)
- Implement token rotation policies
- Use different tokens for different environments

## SOURCES & REFERENCES

- [.NET Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Dapr Documentation](https://docs.dapr.io/)
- [Dapr .NET SDK](https://docs.dapr.io/developing-applications/sdks/dotnet/)
- [Blazor Documentation](https://learn.microsoft.com/en-us/aspnet/core/blazor/)