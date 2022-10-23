// See https://aka.ms/new-console-template for more information


using System;
using System.Threading.Tasks;
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