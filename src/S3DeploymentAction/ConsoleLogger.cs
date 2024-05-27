using System.Runtime.CompilerServices;

namespace S3DeploymentAction;

public enum LogLevel
{
  Off = 0,
  Error = 1,
  Warning = 2,
  Info = 3,
  Verbose = 4
}

public static class ConsoleLogger
{
  public static LogLevel Level { get; set; } = LogLevel.Info;

  private static void WriteLog(
    string message,
    string callerName,
    LogLevel level,
    ConsoleColor? color = null)
  {
    ConsoleColor currentColor = Console.ForegroundColor;
    Console.ForegroundColor = color ?? currentColor;

    Console.WriteLine(
      "[{0:u}] [{1}]: {2}: {3}",
      DateTimeOffset.UtcNow,
      Enum.GetName(typeof(LogLevel), level),
      callerName,
      message);

    Console.ForegroundColor = currentColor;
  }

  public static void Info(string message, [CallerMemberName] string callerName = "")
  {
    if (Level < LogLevel.Info)
    {
      return;
    }
    
    WriteLog(message, callerName, LogLevel.Info, ConsoleColor.Blue);
  }

  public static void Success(string message, [CallerMemberName] string callerName = "")
  {
    if (Level < LogLevel.Info)
    {
      return;
    }
    
    WriteLog(message, callerName, LogLevel.Info, ConsoleColor.Green);
  }

  public static void Verbose(string message, [CallerMemberName] string callerName = "")
  {
    if (Level < LogLevel.Verbose)
    {
      return;
    }
    
    WriteLog(message, callerName, LogLevel.Verbose);
  }

  public static void Warning(string message, [CallerMemberName] string callerName = "")
  {
    if (Level < LogLevel.Warning)
    {
      return;
    }

    WriteLog(message, callerName, LogLevel.Warning, ConsoleColor.Yellow);
  }
  
  public static void Error(string message, [CallerMemberName] string callerName = "")
  {
    if (Level < LogLevel.Error)
    {
      return;
    }

    WriteLog(message, callerName, LogLevel.Warning, ConsoleColor.Red);
  }
}
