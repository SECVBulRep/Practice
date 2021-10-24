using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GrpcClientFactory
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((builderContext, config) =>
                    {
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.InitQuartz(hostContext.Configuration,hostContext.HostingEnvironment);
                        
                        services.AddGrpcClient<Greeter.GreeterClient>(o =>
                        {
                            o.Address = new Uri("https://localhost:5001");
                        }); 
                    })
                    .ConfigureLogging((context, builder) => { });
                ;

            await builder.RunConsoleAsync();
        }
    }

   
    
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        private Greeter.GreeterClient _greeterClient;
        private readonly ILogger<TestJob> _logger;
      
        public TestJob(ILogger<TestJob> logger, Greeter.GreeterClient greeterClient)
        {
            _logger = logger;
            _greeterClient = greeterClient;
        }

        public async Task Execute(IJobExecutionContext context) {
            try
            {
                var response = await _greeterClient.UnaryCallAsync(new ExampleRequest { RequestId = "sdfsdf"});

                if (response != null) _logger.LogInformation("from grpc - " + response.RequestId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
    
    
}