// See https://aka.ms/new-console-template for more information

using Components;
using Contracts;
using MassTransit;
using MassTransit.Transports.Fabric;

namespace Contracts
{
    public interface IUpdateAccount
    {
        string AccountNumber { get; set; }
    }
    public interface IDeleteAccount
    {
        string AccountNumber { get; set; }
    }
}

namespace Components
{
    public class AccountConsumer : IConsumer<IUpdateAccount>
    {
        public Task Consume(ConsumeContext<IUpdateAccount> context)
        {
            Console.WriteLine($"command recived: {context.Message.AccountNumber} on {context.ReceiveContext.InputAddress}");

            return Task.CompletedTask;
        }
    }
    
    public class AnotherAccountConsumer : IConsumer<IUpdateAccount>
    {
        public Task Consume(ConsumeContext<IUpdateAccount> context)
        {
            Console.WriteLine($"Another command recived: {context.Message.AccountNumber}");

            return Task.CompletedTask;
        }
    }
    
    
    
}


internal class Program
{
    static async Task Main(string[] args)
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("localhost", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.Publish<IUpdateAccount>(m=>m.ExchangeType = ExchangeType.Direct.ToString().ToLower());
            
            cfg.ReceiveEndpoint("account-service-a", e =>
            {
                e.ConfigureConsumeTopology = false;
                
                e.Bind<IUpdateAccount>(b =>
                    {
                        b.ExchangeType = ExchangeType.Direct.ToString().ToLower();
                        b.RoutingKey = "A";
                    }
                );
                
                e.Lazy = true; // не хранит очечред пришедший на эндпойнт всю в памяти  
                e.PrefetchCount = 20; // ОЧЕНЬ важный параметр! Сколько  можем одновременно принять сообщении.
                e.Consumer<AccountConsumer>();
            });
            
            cfg.ReceiveEndpoint("account-service-b", e =>
            {
                e.ConfigureConsumeTopology = false;
                
                e.Bind<IUpdateAccount>(b =>
                    {
                        b.ExchangeType = ExchangeType.Direct.ToString().ToLower();
                        b.RoutingKey = "B";
                    }
                );
                
                e.Lazy = true; // не хранит очечред пришедший на эндпойнт всю в памяти  
                e.PrefetchCount = 20; // ОЧЕНЬ важный параметр! Сколько  можем одновременно принять сообщении.
                e.Consumer<AccountConsumer>();
            });
            
        });


        var cancelationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        // для задержки запуска 
        await busControl.StartAsync(cancelationToken.Token);

        try
        {
            Console.WriteLine("bus started");

           /* var endpoind = await busControl.GetSendEndpoint(new Uri("exchange:account"));
            await endpoind.Send<IUpdateAccount>(new
            {
                AccountNumber ="12345"
            });
           
            await endpoind.Send<IDeleteAccount>(new
            {
                AccountNumber ="2234234234"
            });
            */

            await busControl.Publish<IUpdateAccount>(new
            {
                AccountNumber ="12345"
            },s=>s.SetRoutingKey("B"));
            
            
            await Task.Run(() => { Console.ReadKey(); });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            await busControl.StopAsync(CancellationToken.None);
        }
    }
}