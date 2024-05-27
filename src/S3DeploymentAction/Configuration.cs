namespace S3DeploymentAction;

public record struct Configuration(
  string AwsAccessKey,
  string AwsSecretAccessKey,
  string AwsRegion,
  string BucketName,
  string? DeploymentLogUrl,
  string? EnvironmentName,
  string? EnvironmentUrl,
  string? GitHubToken,
  string? GitRef,
  LogLevel LogLevel,
  string? ObjectPrefix,
  bool ObjectPrefixGuid,
  bool ProductionEnvironment,
  string? S3Endpoint,
  bool SkipGitHubDeployment,
  string SourceDirectory);
