using S3DeploymentAction.GitHub;
using S3DeploymentAction.GitHub.Model;

namespace S3DeploymentAction;

public class Program
{
  public static async Task Main(string[] args)
  {
    Configuration config = Inputs.ParseConfiguration();

    ConsoleLogger.Level = config.LogLevel;

    #if DEBUG
    // Override the configured verbosity in the debug configuration
    ConsoleLogger.Level = LogLevel.Verbose;
    #endif

    long deploymentId = default;

    if (!config.SkipGitHubDeployment)
    {
      deploymentId = await CreateGitHubDeployment(config);

      AddOutput("deployment-id", deploymentId.ToString());

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
    ConsoleLogger.Info("Creating a GitHub deployment.");

    GitHubClient client = ConfigureGitHubClient(config.Token);

    string? reference = config.Reference ?? Environment.GetEnvironmentVariable("GITHUB_REF");

    if (string.IsNullOrWhiteSpace(reference))
    {
      throw new Exception(
        "Git ref could not be established. "
        + "Either ref input or GITHUB_REF environment variable has to be specified.");
    }

    CreateDeploymentRequest req = new()
    {
      Ref = reference
      // TODO: Add more options via inputs
    };

    CreateDeploymentResponse res = await client.CreateDeployment(req);

    ConsoleLogger.Success($"GitHub deployment was successfully created with id {res.Id}.");
    return res.Id;
  }

  private static async Task UpdateDeploymentStatus(
    long deploymentId,
    string state,
    string? description,
    Configuration config)
  {
    ConsoleLogger.Info($"Updating GitHub deployment {deploymentId} to {state} state.");

    GitHubClient client = ConfigureGitHubClient(config.Token);

    // GitHub requires that the description is no longer than 140 characters.
    if (description is not null && description.Length > 140)
    {
      description = description.Substring(0, 140);
    }

    Uri? logUrl = config.DeploymentLogUrl;

    if (logUrl is null)
    {
      string? runId = Environment.GetEnvironmentVariable("GITHUB_RUN_ID");

      if (!string.IsNullOrWhiteSpace(runId))
      {
        logUrl = new($"https://github.com/{client.FullRepositoryName}/actions/runs/{runId}");
      }
      else
      {
        ConsoleLogger.Warning("GITHUB_RUN_ID was not provided, skipping setting LogUrl");
      }
    }

    CreateDeploymentStatusRequest req = new()
    {
      State = state,
      Description = description,
      LogUrl = logUrl
      // TODO: Add more options via inputs
    };

    CreateDeploymentStatusResponse res = await client.CreateDeploymentStatus(deploymentId, req);

    ConsoleLogger.Success($"Deployment status {state} was successfully created with id {res.Id}.");
  }

  private static void AddOutput(string name, string value)
  {
    ConsoleLogger.Verbose($"Setting output {name} with value {value}.");

    string? outputFile = Environment.GetEnvironmentVariable("GITHUB_OUTPUT");

    if (string.IsNullOrWhiteSpace(outputFile))
    {
      ConsoleLogger.Warning($"GITHUB_OUTPUT was not defined, skipping output {name}.");
      return;
    }

    try
    {
      using StreamWriter writer = File.AppendText(outputFile);

      writer.WriteLine($"{name}={value}");
    }
    catch (Exception ex)
    {
      ConsoleLogger.Error($"Could not set output {name}, reason: {ex.Message}");
    }
  }
}
