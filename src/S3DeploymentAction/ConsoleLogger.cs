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

  public static void Info(string message)
  {
    if (Level < LogLevel.Info)
    {
      return;
    }

    ConsoleColor currentColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Blue;

    Console.WriteLine($"[{DateTimeOffset.UtcNow:u}] [INFO]: {message}");

    Console.ForegroundColor = currentColor;
  }

  public static void Success(string message)
  {
    if (Level < LogLevel.Info)
    {
      return;
    }

    ConsoleColor currentColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Green;

    Console.WriteLine($"[{DateTimeOffset.UtcNow:u}] [INFO]: {message}");

    Console.ForegroundColor = currentColor;
  }

  public static void Verbose(string message)
  {
    if (Level < LogLevel.Verbose)
    {
      return;
    }

    Console.WriteLine($"[{DateTimeOffset.UtcNow:u}] [VERBOSE]: {message}");
  }
}
