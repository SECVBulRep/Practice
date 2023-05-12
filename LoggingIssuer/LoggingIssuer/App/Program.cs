﻿// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Logging;

namespace App;

class Program
{
    static void Main(string[] args)
    {
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Information)
                .AddConsole();
        });


        int intParam = 11248;

        ILogger logger = loggerFactory.CreateLogger<Program>();

        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("Info Debug. Param {intParam}", intParam);
    }
}