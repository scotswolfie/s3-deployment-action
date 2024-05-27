namespace S3DeploymentAction;

public record struct Configuration(
  string? Token,
  string? Reference,
  Uri? DeploymentLogUrl,
  LogLevel LogLevel,
  bool SkipGitHubDeployment);