namespace AJLogger;

public sealed partial class Logger : LoggerBase
{
    internal Logger()
    { }
    
    /// <summary>
    /// Pushes a warning to the log. User can ignore.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public void Info(string _Msg)
    { _PushLog('i', _Msg); }

    /// <summary>
    /// Pushes a warning to the log. User might want to note.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public void Warning(string _Msg)
    { _PushLog('@', _Msg); }

    /// <summary>
    /// Pushes an error to the log. User should investigate/report.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public void Error(string _Msg)
    { _PushLog('*', _Msg); }
    
    /// <summary>
    /// Pushes a fatal error to the log. User really should investigate/report.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public void Fatal(string _Msg)
    { _PushLog('!', _Msg); }
}