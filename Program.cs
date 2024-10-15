using Microsoft.AspNetCore.Server.Kestrel.Core;
using GrpcServiceSample.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Grpc.Net.Compression;
using Microsoft.ApplicationInsights.AspNetCore;


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

    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2); 
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);

});

builder.Services.AddMemoryCache();

builder.Services.AddGrpc(options =>
{
    options.CompressionProviders.Add(new GzipCompressionProvider(System.IO.Compression.CompressionLevel.Fastest));
    options.ResponseCompressionAlgorithm = "gzip";
    options.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.AddApplicationInsightsTelemetry();
ThreadPool.SetMinThreads(workerThreads: 100, completionPortThreads: 100);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Map the gRPC service to the app
app.MapGrpcService<GreeterService>();

// Add a simple HTTP GET route
app.MapGet("/", () => "GRPC SERVICE IS RUNNING");

// Start the application
app.Run();
