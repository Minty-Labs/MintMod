using Serilog;

namespace MintyServer.Logger;

public class MintyLogger
{
    public static Serilog.Core.Logger _Logger { get; set; }

    public static void Init()
    {
        _Logger = new LoggerConfiguration()
            .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u4}] {CorrelationId}{Message:lj}{NewLine}{Exception}")
            .WriteTo.File("Logs/Log-.log", rollingInterval: RollingInterval.Day, outputTemplate:"[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
    
    public static void info(string message)
    {
        _Logger.Information(message);
    }
    
    public static void error(string message)
    {
        _Logger.Error(message);
    }

    public static void debug(string message)
    {
        _Logger.Debug(message);
    }
    
}