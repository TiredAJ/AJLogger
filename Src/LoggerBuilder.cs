using CSharpFunctionalExtensions;

namespace AJLogger;

public class LoggerBuilder
{
    private Logger _Logger = null!;
    private readonly char Separator = Path.DirectorySeparatorChar;
    
    private static Maybe<LoggerBuilder> Instance = Maybe<LoggerBuilder>.None;

    public static Dictionary<string, Logger> Loggers { get; private set; } = new ();

    private LoggerBuilder()
    { }

    public static void CloseLoggers()
    {
        foreach (Logger L in Loggers.Values)
        {
            L.Info("Closing Log...");
            L.CloseWriter();
        }
    }
    
    /// <summary>
    /// Init
    /// </summary>
    public static LoggerBuilder Init()
    {
        if (Instance.HasNoValue)
        { Instance = new LoggerBuilder(); }
        
        return Instance.Value;
    }

    /// <summary>
    /// Creates a new <see cref="Logger"/> object.
    /// </summary>
    /// <returns></returns>
    public LoggerBuilder NewLogger()
    {
        _Logger = new Logger();
        return this;
    }

    /// <summary>
    /// Determines the location of the log file.
    /// </summary>
    /// <param name="_Location">Location where the log file should go</param>
    public LoggerBuilder SetLogLocation(string _Location)
    {
        _Logger.LogLocation = _Location;
        return this;
    }

    /// <summary>
    /// Uses the default OS logging directory. Suggested to be used with <see cref="LogName()"/>
    /// </summary>
    /// <returns></returns>
    public LoggerBuilder UseDefaultLoc()
    {
        _Logger.LogLocation = $"{Platformer.GetOSLogLocation()}{Separator}CerddoPod";
        return this;
    }

    /// <summary>
    /// Adjusts the log location to be specific to a project and component. So for the component "SAPlayer" in "CerddoPod",
    /// <see cref="_ProjectName"/> would be "CerddoPod" and <see cref="_ComponentName"/> would be "SAPlayer". So that log
    /// would look something like "/var/log/CerddoPod/SAPlayer-Log.md".
    /// MUST be used after either <see cref="UseDefaultLoc"/> or <see cref="SetLogLocation"/>, otherwise <see cref="UseDefaultLoc"/>
    /// will be used.
    /// </summary>
    /// <param name="_ProjectName">Name of project <see cref="_ComponentName"/> belongs to.</param>
    /// <param name="_ComponentName">Name of component to log.</param>
    /// <returns></returns>
    public LoggerBuilder LogName(string _ProjectName, string _ComponentName)
    {
        if (_Logger.LogLocation.HasNoValue)
        { UseDefaultLoc(); }
        
        _Logger.LogLocation = Maybe.From($"{_Logger.LogLocation.Value}{Separator}{_ProjectName}{Separator}{_ComponentName}-Log.md");
        
        _Logger.LogName = $"{_ProjectName}/{_ComponentName}";
        
        return this;
    }
    
    /// <summary>
    /// Adjusts the log location to be specific to a standalone component.
    /// MUST be used after either <see cref="UseDefaultLoc"/> or <see cref="SetLogLocation"/>, otherwise <see cref="UseDefaultLoc"/>
    /// will be used.
    /// </summary>
    /// <param name="_ComponentName">Name of component to log.</param>
    /// <returns></returns>
    public LoggerBuilder LogName(string _ComponentName)
    {
        if (_Logger.LogLocation.HasNoValue)
        { this.UseDefaultLoc(); }
        
        _Logger.LogName = $"{_ComponentName}";
        
        _Logger.LogLocation = Maybe.From($"{_Logger.LogLocation.Value}{Separator}{_Logger.LogName}-Log.md");
        
        return this;
    }

    private void _Build()
    {
        if (_Logger.LogLocation.HasValue && _Logger.LogLocation.Value != String.Empty)
        {
            if (!Directory.Exists(Path.GetDirectoryName(_Logger.LogLocation.Value)))
            { Directory.CreateDirectory(Path.GetDirectoryName(_Logger.LogLocation.Value)!); }
                
            if (!File.Exists(_Logger.LogLocation.Value))
            { File.Create(_Logger.LogLocation.Value).Close(); }
        }
        _Logger.WriterThreadRun = false;
    }

    /// <summary>
    /// Creates the logger object and adds it to <see cref="Loggers"/>.
    /// </summary>
    /// <returns>New Logger object to use.</returns>
    public Logger BuildAndStore()
    {
        _Build();
        Loggers.Add(_Logger.LogName, _Logger);
        return Loggers[_Logger.LogName];        
    }

    /// <summary>
    /// Creates the logger object without adding it to <see cref="Loggers"/>.
    /// </summary>
    /// <returns>New Logger object to use.</returns>
    public Logger Build()
    {
        _Build();
        return _Logger;
    }

    /// <summary>
    /// Stores the logger in <see cref="Loggers"/> without returning the new logger.
    /// </summary>
    public void Store()
    {
        _Build();
        Loggers.Add(_Logger.LogName, _Logger);
    }

    /// <summary>
    /// Copies settings from existing Logger.
    /// </summary>
    /// <param name="_L">Logger to copy from.</param>
    public LoggerBuilder CopyFrom(Logger _L)
    {
        _Logger = new Logger()
        {
            LogLocation = _L.LogLocation, 
            CustomStrLogger = _L.CustomStrLogger,
            LogName = _L.LogName
        };
        return this;
    }

    /// <summary>
    /// Creates the custom logger function called when a new log is created.
    /// </summary>
    /// <param name="_CustomStrLogger">A function that takes a string, returning void.</param>
    public LoggerBuilder CustomLogger(Action<string> _CustomStrLogger)
    {
        _Logger.CustomStrLogger = _CustomStrLogger;
        return this;
    }
}