using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Caching.Memory;

namespace Order_ms
{
    public static class WebApplicationExtensions {

        public static WebApplication ConfigureRoutes( this WebApplication endpoints)
        {
            endpoints.MapGet("/", () => "Hello World!");
            endpoints.MapPost("/test", Handler);
            return endpoints;
        }
 
        private static async Task Handler(MyRequest tm, IMemoryCache cash)
        {
            new MyResponse();
        }
    }
}