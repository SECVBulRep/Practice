// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using WM.TheGame.Infrastructure;

internal class Program
{
    static async Task Main(string[] args)
    {

        var host = await SiloStartConfigurator.StartSiloAsync();
        Console.WriteLine("\n\n Press Enter to terminate...\n\n");
        Console.ReadLine();
        await host.StopAsync();
    }
}