using Grpc.Core;
using GrpcServiceSample;
using Microsoft.Extensions.Caching.Memory;

namespace GrpcServiceSample.Services;


public class GreeterService : Greeter.GreeterBase
{

     private readonly IMemoryCache _cache;
 

    public GreeterService(IMemoryCache cache)
    {
         _cache = cache;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        if (!_cache.TryGetValue(request.Name, out HelloReply cachedResponse))
        {
            var response = new HelloReply
            {
                Message = $"Hello {request.Name}"
            };

            _cache.Set(request.Name, response, TimeSpan.FromMinutes(10)); // Store 10 minutes

            return Task.FromResult(response);
        }

        return Task.FromResult(cachedResponse);
    }
}
