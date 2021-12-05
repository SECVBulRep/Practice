// See https://aka.ms/new-console-template for more information
using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

Console.WriteLine("Hello, World!");

var busControl = Bus.Factory.CreateUsingRabbitMq();

var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

await busControl.StartAsync(source.Token);
try
{
    while (true)
    {
        string? value = await Task.Run(() =>
        {
            Console.WriteLine("Enter message (or quit to exit)");
            Console.Write("> ");
            return Console.ReadLine();
        });

        if("quit".Equals(value, StringComparison.OrdinalIgnoreCase))
            break;

        await busControl.Publish<ValueEntered>(new
        {
            Value = value
        });
    }
}
finally
{
    await busControl.StopAsync();
}


public interface ValueEntered
{
    string Value { get; }
}