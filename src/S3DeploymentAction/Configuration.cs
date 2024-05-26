namespace S3DeploymentAction;

public record struct Configuration(
  string Token,
  string? Reference = null);