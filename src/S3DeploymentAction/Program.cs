using System.Collections;

namespace S3DeploymentAction;

public class Program
{
  public static void Main(string[] args)
  {
    Console.WriteLine("Hello, world!");

    foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
    {
      Console.WriteLine($"{entry.Key} = {entry.Value}");
    }

    Console.WriteLine($"Current directory: {Environment.CurrentDirectory}");

    DirectoryInfo info = new(Environment.CurrentDirectory);

    foreach (var file in info.GetFiles())
    {
      Console.WriteLine($"{file.Name} {file.UnixFileMode:X}");
    }
  }
}
