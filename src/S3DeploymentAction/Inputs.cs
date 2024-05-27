namespace S3DeploymentAction;

public static class Inputs
{
  public static Configuration ParseConfiguration()
  {
    // Null-forgiving operator is used for required inputs, since the GetInput
    // function will throw an exception if they're not present.
    // S3 upload settings
    string awsAccessKey = GetInput("aws-access-key", true)!;
    string awsSecretAccessKey = GetInput("aws-secret-access-key", true)!;
    string bucketName = GetInput("bucket-name", true)!;
    string sourceDirectory = GetInput("source-directory", true)!;
    Uri? s3Endpoint = GetUriInput("s3-endpoint");

    string? objectPrefix = GetInput("object-prefix");
    bool objectPrefixGuid = GetBooleanInput("object-prefix-guid") ?? true;

    // Deployment settings
    Uri? deploymentLogUrl = GetUriInput("deployment-log-url");
    string environmentName = GetInput("environment-name") ?? "production";
    Uri? environmentUrl = GetUriInput("environment-url");
    string? githubToken = GetInput("github-token");
    bool productionEnvironment = GetBooleanInput("production-environment") ?? environmentName == "production";
    string? gitRef = GetInput("git-ref");
    bool skipGitHubDeployment = GetBooleanInput("skip-github-deployment") ?? false;

    // Miscellaneous settings
    LogLevel logLevel = GetInput("log-level") switch
    {
      "off" => LogLevel.Off,
      "error" => LogLevel.Error,
      "warning" => LogLevel.Warning,
      "info" => LogLevel.Info,
      "verbose" => LogLevel.Verbose,
      _ => LogLevel.Info
    };

    return new(
      AwsAccessKey: awsAccessKey,
      AwsSecretAccessKey: awsSecretAccessKey,
      BucketName: bucketName,
      DeploymentLogUrl: deploymentLogUrl,
      EnvironmentName: environmentName,
      EnvironmentUrl: environmentUrl,
      GitHubToken: githubToken,
      GitRef: gitRef,
      LogLevel: logLevel,
      ObjectPrefix: objectPrefix,
      ObjectPrefixGuid: objectPrefixGuid,
      ProductionEnvironment: productionEnvironment,
      S3Endpoint: s3Endpoint,
      SkipGitHubDeployment: skipGitHubDeployment,
      SourceDirectory: sourceDirectory);
  }

  /// <summary>
  /// Provides the value of a named input from the GitHub Action.
  /// </summary>
  /// <remarks>
  /// Implementation was borrowed from the official GitHub Actions Toolkit for JavaScript.
  /// </remarks>
  /// <param name="name">The name of the input to retrieve.</param>
  /// <param name="required">If the input is required (default: false).</param>
  /// <param name="trimWhitespace">If the input's value should be trimmed (default: true).</param>
  /// <returns>The value of the named input.</returns>
  /// <exception cref="ArgumentException">Thrown when the input is marked as required but was not provided.</exception>
  public static string? GetInput(string name, bool required = false, bool trimWhitespace = true)
  {
    string parsedName = $"INPUT_{name.Replace(" ", "_").ToUpper()}";

    string? envVar = Environment.GetEnvironmentVariable(parsedName);

    if (required && string.IsNullOrWhiteSpace(envVar))
    {
      throw new ArgumentException($"Input required and not supplied: {name}");
    }

    if (trimWhitespace && envVar is not null)
    {
      envVar = envVar.Trim();
    }

    // GitHub sets all defined inputs to empty strings, so if they are not
    // required we'll treat them as null.
    return string.IsNullOrWhiteSpace(envVar) ? null : envVar;
  }

  public static bool? GetBooleanInput(string name, bool required = false)
  {
    string? input = GetInput(name, required);

    if (input is not null)
    {
      return input.ToLower() switch
      {
        "true" => true,
        "false" => false,
        _ => throw new ArgumentException($"Input {name} has to have one of the following values: true, false")
      };
    }

    return null;
  }

  public static Uri? GetUriInput(string name, bool required = false)
  {
    string? inputValue = GetInput(name, required);

    if (inputValue is not null)
    {
      return new(inputValue);
    }

    return null;
  }
}
