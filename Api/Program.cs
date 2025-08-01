using Api;
using Api.Endpoints;
using Api.Extensions;
using Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Dependencies
builder.RegisterSnakeDependencies();

var app = builder.Build();

app.UseConfiguredStaticFiles();

app.UseCors();

app.UseMiddleware<SimpleLoginMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

// OpenApi and Scalar endpoints - only enabled in development mode
app.MapOpenApiAndScalarForDevelopment();

// Handle uncaught exceptions
app.UseExceptionHandler();

// Application endpoints
app.MapApplicationEndpoints();

// Output cache
app.UseOutputCache();

app.LogStartup();

app.Run();