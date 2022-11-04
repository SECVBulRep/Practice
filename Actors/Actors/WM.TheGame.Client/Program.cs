// See https://aka.ms/new-console-template for more information


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using WM.TheGame.Contracts.Contracts.Game;

namespace WM.TheGame.Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        
        Thread.Sleep(3000);
        
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(AppContext.BaseDirectory))
            .AddJsonFile("appsettings.json", optional: true);

        var configuration = builder.Build();
        var connectionString = configuration.GetConnectionString("service");
       
        var client = new ClientBuilder()
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "WM.Cluster";
                options.ServiceId = "Wm.Service";
            })
            .UseAdoNetClustering(options =>
            {
                options.ConnectionString = connectionString;
                options.Invariant = "System.Data.SqlClient";
            })
            .ConfigureApplicationParts(part =>
            {
                part.AddApplicationPart(typeof(IGameGrain).Assembly).WithReferences();
            })
            .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Warning).AddConsole())
            .Build();

        await client.Connect();
        
        string? key = "";
        
        Console.WriteLine("Game started");
        Console.WriteLine("print 'q' to exit ");
        Console.WriteLine("print 'start' to start the  game ");
        
        while (true && key!="q")
        {
            key = Console.ReadLine();

            try
            {
                switch (key)
                {
                    case "start":                       
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
    }
}