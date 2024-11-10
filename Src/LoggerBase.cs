using System.Collections.Concurrent;
using System.Diagnostics;
using CSharpFunctionalExtensions;

namespace AJLogger;

public abstract class LoggerBase
{
    #region Protected members
    protected ConcurrentQueue<string?> FileTemp = new();
    protected static readonly string DefaultLoc = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
    #endregion

    #region Internal Members
    internal Maybe<string> LogLocation
    {
        get => _LogLocation;
        set
        {
            if (value.HasValue)
            {
                if (!Directory.Exists(Path.GetDirectoryName(value.Value)))
                { 
                    try
                    { 
                        Directory.CreateDirectory(value.Value);
                        _LogLocation = value;
                        return; 
                    }
                    catch (Exception EXC)
                    {
                        Debug.WriteLine($"Exception caught trying to make log directory at [{value.Value}]. " +
                                        $"Exception: {EXC.Message}");
                        _LogLocation = $"{DefaultLoc}{Path.DirectorySeparatorChar}CerddoPod-Log.md"; 
                    }
                }
                
                _LogLocation = value;
            }
            else
            { _LogLocation = $"{DefaultLoc}{Path.DirectorySeparatorChar}CerddoPod-Log.md"; }
        }
    }
    internal Maybe<string> _LogLocation = string.Empty;
    internal Maybe<Action<string>> CustomStrLogger = Maybe<Action<string>>.None;
    internal string LogName = String.Empty;
    internal bool WriterThreadRun = false;
    #endregion

    /// <summary>
    /// Private, base function to push to all the logs
    /// </summary>
    /// <param name="_Severity">Character to represent severity</param>
    /// <param name="_Msg">string message to push</param>
    protected void _PushLog(char _Severity, string _Msg)
    {
        string MSG = $"[{_Severity}] - [{DateTime.Now:yyyy-MM-dd HH:mm:ss}] => {_Msg}";

        SendLog_Debug(MSG);
        SendLog_File(MSG);

        if (CustomStrLogger.HasValue)
        { CustomStrLogger.Value(_Msg); }
    }
    
    [Conditional("DEBUG")]
    private static void SendLog_Debug(string? _Msg)
    { Debug.WriteLine(_Msg); }

    private void SendLog_File(string? _Msg)
    {
        //enques the current message to the file queue
        FileTemp.Enqueue(_Msg);

        if (WriterThreadRun)
        { return; }
        
        WriterThreadRun = true;
        _ = WriterThread();
    }

    private async Task WriterThread()
    {
        await Task.Run(async() =>
        {
            await using StreamWriter Writer = new (_LogLocation.Value);

            while (WriterThreadRun)
            {
                if (!FileTemp.TryDequeue(out string? MSG))
                { continue; }
                        
                await Writer.WriteLineAsync(MSG);
                        
                await Writer.FlushAsync();
            }
        });
    }

    public void CloseWriter()
    { WriterThreadRun = false; }
}

static class Extensions
{
    internal static string StrDisplay(this (DateTime Stamp, string Msg) _Msg)
    { return $"[{_Msg.Stamp:dd/MM/yyyy HH:mm:ss}]: {_Msg.Msg}"; }
}