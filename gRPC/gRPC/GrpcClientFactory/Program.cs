using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GrpcClientFactory
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, config) => { })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<LoggerInterceptor>();
                    services.InitQuartz(hostContext.Configuration, hostContext.HostingEnvironment);

                    services.AddGrpcClient<Greeter.GreeterClient>(o =>
                        {
                            o.Address = new Uri("https://localhost:5001");
                        })
                        .AddInterceptor<LoggerInterceptor>()
                        ;
                })
                .ConfigureLogging((context, builder) => { })
                ;
            ;

            await builder.RunConsoleAsync();
        }
    }
}