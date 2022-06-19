using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.Logging;

namespace MT.SampleComponents.Consumers
{
    public class RoutingSlipEventConsumer : 
        IConsumer<RoutingSlipCompleted>, 
        IConsumer<RoutingSlipActivityCompleted>,
        IConsumer<RoutingSlipFaulted>
        
    {
        private ILogger<RoutingSlipEventConsumer> _logger;


        public RoutingSlipEventConsumer(ILogger<RoutingSlipEventConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(ConsumeContext<RoutingSlipCompleted> context)
        {
            _logger.LogInformation($"Routing slip completed. Tracking number {context.Message.TrackingNumber}");
            return Task.CompletedTask;
        }
        
        public Task Consume(ConsumeContext<RoutingSlipActivityCompleted> context)
        {
            _logger.LogInformation(
                $"Routing Slip Activity Completed. Tracking number {context.Message.TrackingNumber} ActivityName: {context.Message.ActivityName}");
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<RoutingSlipFaulted> context)
        {
            _logger.LogInformation(
                $"RoutingSlipFaulted. Tracking number {context.Message.TrackingNumber} Exception {context.Message.ActivityExceptions.FirstOrDefault()}");
            return Task.CompletedTask;
        }
    }
}