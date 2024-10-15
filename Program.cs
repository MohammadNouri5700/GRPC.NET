using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using GrpcServiceSample.Services;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps("./certificate.pfx", "Nouri5700");
    });
});

var app = builder.Build();


app.MapGrpcService<GreeterService>();


app.MapGet("/", () => Task.FromResult("gRPC server is running on https://localhost:5001"));


app.Run();
