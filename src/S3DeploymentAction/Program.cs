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
  }
}
