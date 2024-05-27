namespace S3DeploymentAction;

public record struct Configuration(
  string AwsAccessKey,
  string AwsSecretAccessKey,
  string BucketName,
  Uri? DeploymentLogUrl,
  string? EnvironmentName,
  Uri? EnvironmentUrl,
  string? GitHubToken,
  string? GitRef,
  LogLevel LogLevel,
  string? ObjectPrefix,
  bool ObjectPrefixGuid,
  bool ProductionEnvironment,
  Uri? S3Endpoint,
  bool SkipGitHubDeployment,
  string SourceDirectory);