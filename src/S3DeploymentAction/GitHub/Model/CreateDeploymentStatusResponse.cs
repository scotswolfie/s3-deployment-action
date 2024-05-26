using System.Text.Json.Serialization;

namespace S3DeploymentAction.GitHub.Model;

public class CreateDeploymentStatusResponse
{
  [JsonPropertyName("id")]
  public required long Id { get; set; }

  [JsonPropertyName("node_id")]
  public required string NodeId { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("environment")]
  public string? Environment { get; set; }

  [JsonPropertyName("state")]
  public required string State { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("description")]
  public string? Description { get; set; }

  [JsonPropertyName("deployment_url")]
  public required Uri DeploymentUrl { get; set; }

  [JsonPropertyName("url")]
  public required Uri Url { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("log_url")]
  public Uri? LogUrl { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("environment_url")]
  public Uri? EnvironmentUrl { get; set; }

  [JsonPropertyName("repository_url")]
  public required Uri RepositoryUrl { get; set; }

  [JsonPropertyName("created_at")]
  public DateTimeOffset CreatedAt { get; set; }

  [JsonPropertyName("updated_at")]
  public DateTimeOffset UpdatedAt { get; set; }
}
