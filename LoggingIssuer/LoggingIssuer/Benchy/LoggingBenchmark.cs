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
    private readonly ILoggerAdapter<LoggingBenchmark> _loggerAdapter;

    public LoggingBenchmark()
    {
        _logger = new Logger<LoggingBenchmark>(_loggerFactory);
        _loggerAdapter = new LoggerAdapter<LoggingBenchmark>(_logger);
    }

    [Benchmark()]
    public void LogWithOutIf()
    {
        _logger.LogInformation(LoggingMessageWithParams, 1, 2, 3);
    }

    [Benchmark()]
    public void LogWithIf()
    {
        if (_logger.IsEnabled(LogLevel.Information))
            _logger.LogInformation(LoggingMessageWithParams, 1, 2, 3);
    }

    [Benchmark()]
    public void LogWithAdapter()
    {
        _loggerAdapter.LogInFormation(LoggingMessageWithParams, 1, 2, 3);
    }
}