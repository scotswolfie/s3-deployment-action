using S3DeploymentAction.GitHub;

namespace S3DeploymentAction;

public class Program
{
  public static async Task Main(string[] args)
  {
    Configuration config = Inputs.ParseConfiguration();

    // The owner and name of the repository
    string fullRepositoryName = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY")!;

    GitHubClient client = new(config.Token, fullRepositoryName);

    string? reference = config.Reference
                        ?? Environment.GetEnvironmentVariable("GITHUB_SHA")
                        ?? Environment.GetEnvironmentVariable("GITHUB_REF_NAME");

    if (reference is null)
    {
      throw new Exception(
        "Git ref could not be established. "
        + "Either ref input, GITHUB_SHA env var, or GITHUB_REF_NAME env var has to be specified.");
    }
  }
}
