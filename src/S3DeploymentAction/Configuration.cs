namespace S3DeploymentAction;

public record struct Configuration(
  string? Token,
  string? Reference,
  LogLevel LogLevel,
  bool SkipGitHubDeployment);