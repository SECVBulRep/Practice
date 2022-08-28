// See https://aka.ms/new-console-template for more information


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal class Program
{
    static HttpClient _client;

    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        await using var provider = services.BuildServiceProvider(true);
        
        await Task.Run(() => Client(provider), CancellationToken.None);
        
    }


    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(configure => configure.AddConsole());
    }

    static async Task Client(IServiceProvider provider)
    {
        var logger = provider.GetRequiredService<ILogger<Program>>();

        while (true)
        {
            Console.Write("Enter # of courier to visit, or empty to quit: ");
           
            var line = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(line))
                break;

            int limit;
            int loops = 1;
            var segments = line.Split(',');
            if (segments.Length == 2)
            {
                loops = int.TryParse(segments[1], out int result) ? result : 1;
                limit = int.TryParse(segments[0], out result) ? result : 1;
            }
            else if (!int.TryParse(line, out limit))
                limit = 1;

            logger.LogInformation("Running {LoopCount} loops of {Limit} courier each", loops, limit);

            using var serviceScope = provider.CreateScope();

            var random = new Random();

            for (var pass = 0; pass < loops; pass++)
            {
                try
                {
                    var tasks = new List<Task>();

                    await Task.WhenAll(tasks.ToArray());
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Loop Faulted");
                }
            }
        }
    }
    
   
}