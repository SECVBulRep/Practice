using Microsoft.Extensions.Logging;

namespace Benchy;



public class LoggerAdapter<T> : ILoggerAdapter<T>
{
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInFormation(string message)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message);
        }
    }

    public void LogInFormation<T0>(string message, T0 arg0)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message,arg0);
        }
    }

    public void LogInFormation<T0, T1>(string message, T0 arg0, T1 arg1)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message,arg0,arg1);
        }
    }
    
    public void LogInFormation<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation(message,arg0,arg1,arg2);
        }
    }
}


public interface ILoggerAdapter<T>
{
    void LogInFormation(string message);
    
    void LogInFormation<T0>(string message,T0 arg0);
    
    void LogInFormation<T0,T1>(string message,T0 arg0,T1 arg1);
    
    void LogInFormation<T0,T1,T2>(string message,T0 arg0,T1 arg1,T2 arg2);
}