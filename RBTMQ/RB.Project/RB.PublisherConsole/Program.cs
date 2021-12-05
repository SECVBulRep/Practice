using MassTransit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RB.PublisherConsole
{
    class Program
    {
        public static async Task Main()
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("172.16.29.35", "NPD", h =>
                {
                    h.Username("bulat");
                    h.Password("123qweASD");
                });

                cfg.ReceiveEndpoint("event-listener", e =>
                {
                    e.Consumer<EventConsumer>();
                });

                cfg.ReceiveEndpoint("event-listener2", e =>
                {
                    e.Consumer<EventConsumer>();
                });

            });


            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await busControl.StartAsync(source.Token);
           
            try
            {
                do
                {
                    string value = await Task.Run(() =>
                    {
                        Console.WriteLine("Enter message (or quit to exit)");
                        Console.Write("> ");
                        return Console.ReadLine();
                    });

                    if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
                        break;

                    await busControl.Publish<ValueEntered>(new
                    {
                        Value = value
                    });
                }
                while (true);
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }


    class EventConsumer :
           IConsumer<ValueEntered>
    {
        public async Task Consume(ConsumeContext<ValueEntered> context)
        {
            Console.WriteLine("Value: {0}", context.Message.Value);
        }
    }

    public interface ValueEntered
    {
        string Value { get; }
    }
}
