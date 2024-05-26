namespace S3DeploymentAction;

public record struct Configuration(
  string Token,
  bool SkipGitHubDeployment,
  string? Reference = null);