using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using S3DeploymentAction.GitHub;
using S3DeploymentAction.GitHub.Model;
using System.Text;

namespace S3DeploymentAction;

public class Program
{
  public static async Task Main(string[] args)
  {
    Configuration config = Inputs.ParseConfiguration();

    ConsoleLogger.Level = config.LogLevel;

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
      string objectPrefix = await UploadToS3(config);

      if (config.EnvironmentUrl is not null)
      {
        config.EnvironmentUrl = new(config.EnvironmentUrl.ToString().Replace("{prefix}", objectPrefix));
        ConsoleLogger.Info($"Environment URL after prefix replacement: {config.EnvironmentUrl}");
        AddOutput("environment-url", config.EnvironmentUrl.ToString());
      }

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

    GitHubClient client = ConfigureGitHubClient(config.GitHubToken);

    string? reference = config.GitRef ?? Environment.GetEnvironmentVariable("GITHUB_REF");

    if (string.IsNullOrWhiteSpace(reference))
    {
      throw new Exception(
        "Git ref could not be established. "
        + "Either git-ref input or GITHUB_REF environment variable has to be specified.");
    }

    CreateDeploymentRequest req = new()
    {
      Description = "Deployment to AWS S3 created via GitHub Actions",
      Environment = config.EnvironmentName,
      ProductionEnvironment = config.ProductionEnvironment,
      Ref = reference
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

    GitHubClient client = ConfigureGitHubClient(config.GitHubToken);

    // GitHub requires that the description is no longer than 140 characters.
    if (description is not null && description.Length > 140)
    {
      description = description.Substring(0, 140);
    }

    string? logUrl = config.DeploymentLogUrl;

    if (logUrl is null)
    {
      string? runId = Environment.GetEnvironmentVariable("GITHUB_RUN_ID");

      if (!string.IsNullOrWhiteSpace(runId))
      {
        logUrl = new($"https://github.com/{client.FullRepositoryName}/actions/runs/{runId}");
      }
      else
      {
        ConsoleLogger.Warning("GITHUB_RUN_ID was not provided, skipping setting LogUrl.");
      }
    }

    string? environmentUrl = null;

    if (config.EnvironmentUrl is not null && state != "in_progress")
    {
      environmentUrl = config.EnvironmentUrl;
    }

    CreateDeploymentStatusRequest req = new()
    {
      Description = description,
      Environment = config.EnvironmentName,
      EnvironmentUrl = environmentUrl is not null ? new Uri(environmentUrl) : null,
      LogUrl = logUrl is not null ? new Uri(logUrl) : null,
      State = state
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

  private static async Task<string> UploadToS3(Configuration config)
  {
    ConsoleLogger.Info("Preparing to upload files to S3.");

    BasicAWSCredentials clientCredentials = new(config.AwsAccessKey, config.AwsSecretAccessKey);

    AmazonS3Config clientConfig = new()
    {
      RegionEndpoint = RegionEndpoint.GetBySystemName(config.AwsRegion)
    };

    if (config.S3Endpoint is not null)
    {
      clientConfig.ServiceURL = config.S3Endpoint.ToString();
    }

    AmazonS3Client client = new(clientCredentials, clientConfig);

    StringBuilder prefix = new();

    prefix.Append(config.ObjectPrefix ?? string.Empty);

    if (prefix.Length > 0 && prefix[0] == '/')
    {
      prefix.Remove(0, 1);
    }

    if (prefix.Length > 0 && prefix[^1] != '/')
    {
      prefix.Insert(prefix.Length, '/');
    }

    if (config.ObjectPrefixGuid)
    {
      Guid prefixGuid = Guid.NewGuid();
      prefix.Insert(prefix.Length, $"{prefixGuid.ToString()}/");
      AddOutput("object-prefix-guid", prefixGuid.ToString());
    }

    ConsoleLogger.Info($"Files will be uploaded with the following prefix: {prefix}");
    AddOutput("object-prefix", prefix.ToString());

    if (!Path.Exists(config.SourceDirectory))
    {
      throw new DirectoryNotFoundException($"Directory at path {config.SourceDirectory} does not exist.");
    }

    IEnumerable<string> filesToUpload = Directory
      .EnumerateFiles(
        config.SourceDirectory,
        "*",
        SearchOption.AllDirectories)
      .Select(Path.GetFullPath);

    ConsoleLogger.Info("Beginning to upload files to S3.");

    foreach (string filePath in filesToUpload)
    {
      string mimeType = MimeMapping.MimeUtility.GetMimeMapping(Path.GetFileName(filePath));
      string fileLocation = Path.GetRelativePath(config.SourceDirectory, filePath)
        .Replace(Path.DirectorySeparatorChar, '/');
      string objectKey = $"{prefix}{fileLocation}";

      ConsoleLogger.Verbose($"Uploading {filePath} ({mimeType}) with key {objectKey}.");

      PutObjectRequest req = new()
      {
        BucketName = config.BucketName,
        ContentType = mimeType,
        Key = objectKey,
        FilePath = filePath
      };

      await client.PutObjectAsync(req);
    }

    ConsoleLogger.Info("Finished uploading files to S3.");

    return prefix.ToString();
  }
}
