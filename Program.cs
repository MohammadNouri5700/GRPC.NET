using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps("path/to/your/certificate.pfx", "your_certificate_password");
    });
});

var app = builder.Build();


app.MapGrpcService<GreeterService>();


app.MapGet("/", (context) =>
{

    var addressFeature = context.Request.HttpContext.Features.Get<IServerAddressesFeature>();
    var addresses = string.Join(", ", addressFeature.Addresses);


    return $"gRPC server is running on {addresses}";
});

app.Run();
