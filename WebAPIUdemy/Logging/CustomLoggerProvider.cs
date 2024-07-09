using System.Collections.Concurrent;

namespace WebAPIUdemy.Logging;

public class CustomLoggerProvider : ILoggerProvider
{
    readonly CustomLoggerProviderConfig? loggerConfig;
    readonly ConcurrentDictionary<string, CustomerLogger> loggers =
        new ConcurrentDictionary<string, CustomerLogger>();

    public CustomLoggerProvider(CustomLoggerProviderConfig config)
    {
        loggerConfig = config;
    }

    public ILogger CreateLogger(string categoryName) 
    { 
        return loggers.GetOrAdd(categoryName, name => new CustomerLogger(name, loggerConfig));
    }

    public void Dispose() 
    { 
        loggers.Clear();
    }

}
