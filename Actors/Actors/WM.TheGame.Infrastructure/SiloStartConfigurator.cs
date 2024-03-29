﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;


namespace WM.TheGame.Infrastructure;

public class SiloStartConfigurator
{
    public static async Task<IHost> StartSiloAsync()
    {
        var invariant = "System.Data.SqlClient";

        var silo = new HostBuilder()
            .ConfigureAppConfiguration((hostingConext, config) => { config.AddJsonFile("appsettings.json", true); })
            .UseOrleans((context, builder) =>
            {
                string? connectionString = context.Configuration.GetConnectionString("service");

                int siloPort = context.Configuration.GetValue<int>(
                    "Silo:siloPort");

                int gatewayPort = context.Configuration.GetValue<int>(
                    "Silo:gatewayPort");

                string? siloName = context.Configuration.GetValue<string>(
                    "Silo:name");

                builder.Configure<SiloOptions>(options =>
                {
                    options.SiloName = Environment.MachineName + "_" + siloName;
                });

                builder.Configure<ClusterOptions>(options =>
                    {
                        options.ClusterId = "WM.Cluster";
                        options.ServiceId = "Wm.Service";
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

              /*  builder.AddAdoNetGrainStorage("Wm.GrainStorage", options =>
                {
                    options.Invariant = invariant;
                    options.ConnectionString = connectionString;
                });*/

                builder.AddAdoNetGrainStorageAsDefault(options =>
                {
                    options.Invariant = invariant;
                    options.ConnectionString = connectionString;
                });


                builder.UseDashboard(options =>
                {
                    options.Username = "user";
                    options.Password = "user";
                    options.Host = "*";
                    options.Port = context.Configuration.GetValue<int>(
                        "Silo:dashBoardPort");;
                    options.HostSelf = true;
                    options.CounterUpdateIntervalMs = 1000;
                });

                //builder.AddLogStorageBasedLogConsistencyProvider("StateStorage");
                builder.AddLogStorageBasedLogConsistencyProvider("LogStorage");
                
                // builder.AddMemoryGrainStorageAsDefault();

                builder.UseTransactions();
            })
            .Build();

        await silo.StartAsync();
        return silo;
    }
}