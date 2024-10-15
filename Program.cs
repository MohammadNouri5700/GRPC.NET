using Microsoft.AspNetCore.Server.Kestrel.Core;
using GrpcServiceSample.Services;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add gRPC services to the DI container.
builder.Services.AddGrpc();

// Configure Kestrel server to use HTTPS
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5001, listenOptions =>
    {
        string certPath = "./certificate.pfx";
        string certPassword = "123456";
        listenOptions.UseHttps(certPath, certPassword);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Map the gRPC service to the app
app.MapGrpcService<GreeterService>();

// Add a simple HTTP GET route
app.MapGet("/", () => "Hello, the gRPC server is running on https://localhost:5001");

// Start the application
app.Run();
