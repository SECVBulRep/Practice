using Microsoft.Extensions.Logging;

namespace Benchy;

public static class LogMessageDefinition
{
    private static readonly Action<ILogger, int, int, int, Exception?> BenchmarkedLogMessageDefinition =
        LoggerMessage.Define<int, int, int>(LogLevel.Information, 0, "This is log wit params {first}{second}{third}");

    public static void LogBenchmarkMessage(this ILogger logger, int first, int second, int third)
    {
        BenchmarkedLogMessageDefinition(logger, first, second, third,null);
    }

}