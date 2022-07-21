// See https://aka.ms/new-console-template for more information

using MassTransit;

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
        });


        var cancelationToken = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        
        // для задержки запуска 
        await busControl.StartAsync(cancelationToken.Token);

        try
        {
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