using System.Collections;

namespace S3DeploymentAction;

public class Program
{
  public static async Task Main(string[] args)
  {
    foreach (var arg in args)
    {
      Console.WriteLine($"ARG: {arg}");
    }

    foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
    {
      Console.WriteLine($"ENV: {entry.Key} = {entry.Value}");
    }
    // GitHubClient client = new(Environment.GetEnvironmentVariable("GITHUB_TOKEN"));
    //
    // await client.CreateDeployment();
  }
}
