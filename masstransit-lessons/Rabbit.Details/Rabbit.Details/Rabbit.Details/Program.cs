// See https://aka.ms/new-console-template for more information

using Components;
using Contracts;
using MassTransit;

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
            Console.WriteLine($"command recived: {context.Message.AccountNumber}");

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
    class MyFormatter: IEntityNameFormatter
    {
        public string FormatEntityName<T>()
        {
            return typeof(T).Name;
        }
    }
    
    
    static async Task Main(string[] args)
    {
        var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
        {
            cfg.Host("localhost", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });


            cfg.MessageTopology.SetEntityNameFormatter(new MyFormatter());
            //cfg.Message<IUpdateAccount>(m=>m.SetEntityName("update-account"));

            cfg.ReceiveEndpoint("account-service", e =>
            {
                //нужно рассказать про каждый параметр
                //e.Durable true по умолчанию
                //e.Exclusive один процесс одноврменно имеет доступ к ендпойинту
                e.Lazy = true; // не хранит очечред пришедший на эндпойнт всю в памяти  
                //e.AutoDelete
                // e.BindQueue создается только эксчейндж без очерди, когда ты хочешь сам настроить байндинги в админке напмеример, не рекаменджуется 
                // e.ConsumerPriority
                // e.ExchangeType  тип экчейнджа / по умолчанию fanout/  обсудим позже
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
            });
            
            
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