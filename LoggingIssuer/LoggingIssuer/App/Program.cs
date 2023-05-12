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
      
        ILogger logger = loggerFactory.CreateLogger<Program>();
        logger.LogTrace("Info Trace");
        logger.LogDebug("Info Debug");
        logger.LogInformation("Info Log");
        logger.LogWarning("Warning Log");
        logger.LogError("Error Log");
        logger.LogCritical("Critical Log");
        
        Console.ReadLine();
    }
}