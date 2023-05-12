using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging;

namespace Benchy;

[MemoryDiagnoser()]
public class LoggingBenchmark
{
    private const string LoggingMessageWithParams = "This is log wit params {first}{second}{third}";

    private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .SetMinimumLevel(LogLevel.Information);
    });


    private readonly ILogger<LoggingBenchmark> _logger;

    public LoggingBenchmark()
    {
        _logger = new Logger<LoggingBenchmark>(_loggerFactory);
    }

    [Benchmark()]
    public void LogWithOutIf()
    {
        _logger.LogInformation(LoggingMessageWithParams,1,2,3);
    }
    
    [Benchmark()]
    public void LogWithIf()
    {
        if(_logger.IsEnabled(LogLevel.Information))
        _logger.LogInformation(LoggingMessageWithParams,1,2,3);
    }

}