using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace GrpcClientFactory
{
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

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var response = await _greeterClient.UnaryCallAsync(new ExampleRequest { RequestId = "sdfsdf" });

                if (response != null) _logger.LogInformation("from grpc - " + response.RequestId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }
    }
}