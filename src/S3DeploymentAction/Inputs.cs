namespace S3DeploymentAction;

public static class Inputs
{
  public static Configuration ParseConfiguration()
  {
    string? token = GetInput("github-token");
    string? reference = GetInput("ref");

    bool skipGitHubDeployment = GetBooleanInput("skip-github-deployment") ?? false;
    
    string? deploymentLogUrlInput = GetInput("deployment-log-url");
    Uri? deploymentLogUrl = null;

    if (deploymentLogUrlInput is not null)
    {
      deploymentLogUrl = new(deploymentLogUrlInput);
    }

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
      Token: token,
      Reference: reference,
      DeploymentLogUrl: deploymentLogUrl,
      LogLevel: logLevel,
      SkipGitHubDeployment: skipGitHubDeployment);
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
}
