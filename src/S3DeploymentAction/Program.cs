using System.Diagnostics;
using S3DeploymentAction.GitHub;
using S3DeploymentAction.GitHub.Model;

namespace S3DeploymentAction;

public class Program
{
  public static async Task Main(string[] args)
  {
    Trace.Listeners.Add(new ConsoleTraceListener());

    Configuration config = Inputs.ParseConfiguration();

    long deploymentId = default;

    if (!config.SkipGitHubDeployment)
    {
      deploymentId = await CreateGitHubDeployment(config);

      await UpdateDeploymentStatus(
        deploymentId,
        "in_progress",
        "Preparing to deploy files to the S3 bucket.",
        config);
    }

    try
    {
      // TODO: Upload files to S3
      if (!config.SkipGitHubDeployment && deploymentId != default)
      {
        await UpdateDeploymentStatus(
          deploymentId,
          "success",
          "The deployment files were successfully uploaded to S3.",
          config);
      }
    }
    catch (Exception ex)
    {
      if (!config.SkipGitHubDeployment && deploymentId != default)
      {
        await UpdateDeploymentStatus(deploymentId, "error", ex.Message, config);
      }

      throw;
    }
    finally
    {
      Trace.Flush();
      Trace.Close();
    }
  }

  private static GitHubClient ConfigureGitHubClient(string? token)
  {
    if (token is null)
    {
      throw new Exception("API token must be provided if skip-github-deployment input is not set to true.");
    }

    // The owner and name of the repository.
    string fullRepositoryName = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY")!;

    GitHubClient client = new(token, fullRepositoryName);

    return client;
  }

  private static async Task<long> CreateGitHubDeployment(Configuration config)
  {
    Trace.TraceInformation("Creating a GitHub deployment.");

    GitHubClient client = ConfigureGitHubClient(config.Token);

    string? reference = config.Reference
                        ?? Environment.GetEnvironmentVariable("GITHUB_SHA")
                        ?? Environment.GetEnvironmentVariable("GITHUB_REF_NAME");

    if (reference is null)
    {
      throw new Exception(
        "Git ref could not be established. "
        + "Either ref input, GITHUB_SHA env var, or GITHUB_REF_NAME env var has to be specified.");
    }

    CreateDeploymentRequest req = new()
    {
      Ref = reference
      // TODO: Add more options via inputs
    };

    CreateDeploymentResponse res = await client.CreateDeployment(req);

    Trace.TraceInformation($"GitHub deployment was successfully created with id {res.Id}.");
    return res.Id;
  }

  private static async Task UpdateDeploymentStatus(
    long deploymentId,
    string state,
    string? description,
    Configuration config)
  {
    Trace.TraceInformation($"Updating GitHub deployment {deploymentId} to {state} state.");

    GitHubClient client = ConfigureGitHubClient(config.Token);

    // GitHub requires that the description is no longer than 140 characters.
    if (description is not null && description.Length > 140)
    {
      description = description.Substring(0, 140);
    }

    CreateDeploymentStatusRequest req = new()
    {
      State = state,
      Description = description
      // TODO: Add more options via inputs
    };

    CreateDeploymentStatusResponse res = await client.CreateDeploymentStatus(deploymentId, req);

    Trace.TraceInformation($"Deployment status {state} was successfully created with id {res.Id}.");
  }
}
