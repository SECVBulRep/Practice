// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging;

namespace App;

class Program
{
    static void Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Debug)
                .AddConsole();
        });


        int intParam = 10;
        
        ILogger logger = loggerFactory.CreateLogger<Program>();
        logger.LogTrace("Info Trace. Param {intParam}",intParam);
        logger.LogDebug("Info Debug. Param {intParam}",intParam);
        logger.LogInformation("Info Log. Param {intParam}",intParam);
        logger.LogWarning("Warning Log. Param {intParam}",intParam);
        logger.LogError("Error Log. Param {intParam}",intParam);
        logger.LogCritical("Critical Log. Param {intParam}",intParam);
        
        Console.ReadLine();
    }
}