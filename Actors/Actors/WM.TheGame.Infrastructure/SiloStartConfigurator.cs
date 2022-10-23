using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using WM.TheGame.Contracts.Contracts;


namespace WM.TheGame.Infrastructure;

public class SiloStartConfigurator
{
    public static async Task<IHost> StartSiloAsync()
    {
        var invariant = "System.Data.SqlClient";

        var silo = new HostBuilder()
            .ConfigureAppConfiguration((hostingConext, config) => { config.AddJsonFile("appsettings.json", true); })
            .UseOrleans((context,builder) =>
            {
                string? connectionString = context.Configuration.GetConnectionString("service");
                
                int siloPort = context.Configuration.GetValue<int>(
                    "Silo:siloPort");
                
                int gatewayPort = context.Configuration.GetValue<int>(
                    "Silo:gatewayPort");
                
                builder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "WM.Cluster";
                        options.ServiceId = "Wm.Service";
                    })
                    .ConfigureApplicationParts(part =>
                    {
                        part.AddApplicationPart(typeof(IGameGrain).Assembly);
                        part.AddFromAppDomain();
                    })
                    .UseAdoNetClustering(options =>
                    {
                        options.ConnectionString = connectionString;
                        options.Invariant = invariant;
                    })
                    .UseAdoNetReminderService(options =>
                    {
                        options.ConnectionString = connectionString;
                        options.Invariant = invariant;
                    })
                   
                    .ConfigureEndpoints(siloPort: siloPort, gatewayPort: gatewayPort)
                    .ConfigureLogging(builder => builder.SetMinimumLevel(LogLevel.Information).AddConsole());
                
                builder.AddAdoNetGrainStorage("Wm.GrainStorage", options =>
                {
                    options.Invariant = invariant;
                    options.ConnectionString = connectionString;
                });
            })
            .Build();

        await silo.StartAsync();
        return silo;
    }
}