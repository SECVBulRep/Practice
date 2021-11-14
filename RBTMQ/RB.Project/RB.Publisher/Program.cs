using MassTransit;
using System;
using System.Threading.Tasks;

namespace RB.Publisher
{
    class Program
    {
        public static async Task Main()
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                sbc.Host("localhost", "/", h =>
                {
                    h.Username("rabbitmq");
                    h.Password("rabbitmq");
                });


                sbc.ReceiveEndpoint("test_queue", ep =>
                {
                    ep.Handler<Message>(context =>
                    {
                        return Console.Out.WriteLineAsync($"Received: {context.Message.Text}");
                    });
                });
            });

            await bus.StartAsync(); // This is important!

            await bus.Publish(new Message { Text = "Hi" });

            Console.WriteLine("Press any key to exit");
            await Task.Run(() => Console.ReadKey());

            await bus.StopAsync();
        }
    }

    public class Message
    {
        public string Text { get; set; }
    }

}
