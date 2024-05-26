namespace S3DeploymentAction;

public static class Inputs
{
  public static Configuration ParseConfiguration()
  {
    string token = GetInput("github-token", true)!;
    string? reference = GetInput("ref");

    return new(
      Token: token,
      Reference: reference);
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

    return envVar;
  }
}
