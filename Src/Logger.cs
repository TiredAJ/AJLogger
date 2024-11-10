using CSharpFunctionalExtensions;

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

//For Results
public sealed partial class Logger
{
    public Result InfoResult(string _Msg)
    {
        Info(_Msg);
        return Result.Success();
    }
    
    public Result<T> InfoResult<T>(T _Msg)
    {
        if (_Msg is null)
        { return Result.Success<T>(default!); }
        
        Info(_Msg.ToString()!);
        return Result.Success<T>(_Msg);
    }
    
    /// <summary>
    /// Pushes an error to the log. User should investigate/report.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public Result<T> ErrorResult<T>(string _Msg)
    {
        Error(_Msg);
        return Result.Failure<T>(_Msg);
    }

    /// <summary>
    /// Pushes a fatal error to the log. User really should investigate/report.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public Result<T> FatalResult<T>(string _Msg)
    {
        Fatal(_Msg);
        return Result.Failure<T>(_Msg);
    }
    
    /// <summary>
    /// Pushes an error to the log. User should investigate/report.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public Result ErrorResult(string _Msg)
    {
        Error(_Msg);
        return Result.Failure(_Msg);
    }

    /// <summary>
    /// Pushes a fatal error to the log. User really should investigate/report.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public Result FatalResult(string _Msg)
    {
        Fatal(_Msg);
        return Result.Failure(_Msg);
    }
    
    /// <summary>
    /// Pushes a fatal error to the log. User really should investigate/report.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public T FatalThrow<T>(string _Msg) where T : Exception
    {
        Fatal(_Msg);
        return (T)new Exception(_Msg);
    }

    /// <summary>
    /// Pushes a fatal error to the log. User really should investigate/report.
    /// </summary>
    /// <param name="_Msg">String message to push</param>
    public Exception FatalThrow(string _Msg)
    {
        Fatal(_Msg);
        return new(_Msg);
    }
}