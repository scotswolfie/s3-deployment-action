using System.Text.Json.Serialization;

namespace S3DeploymentAction.GitHub.Model;

public class CreateDeploymentResponse
{
  [JsonPropertyName("id")]
  public required long Id { get; set; }

  [JsonPropertyName("node_id")]
  public required string NodeId { get; set; }
  
  [JsonPropertyName("ref")]
  public required string Ref { get; set; }

  [JsonPropertyName("sha")]
  public required string Sha { get; set; }

  [JsonPropertyName("environment")]
  public required string Environment { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("original_environment")]
  public string? OriginalEnvironment { get; set; }
  
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("production_environment")]
  public bool? ProductionEnvironment { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("transient_environment")]
  public bool? TransientEnvironment { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("description")]
  public string? Description { get; set; }

  [JsonPropertyName("payload")]
  public required Dictionary<string, object> Payload { get; set; }
  
  [JsonPropertyName("task")]
  public required string Task { get; set; }
  
  [JsonPropertyName("url")]
  public required Uri Url { get; set; }

  [JsonPropertyName("statuses_url")]
  public required Uri StatusesUrl { get; set; }
  
  [JsonPropertyName("repository_url")]
  public required Uri RepositoryUrl { get; set; }
  
  [JsonPropertyName("created_at")]
  public required DateTimeOffset CreatedAt { get; set; }

  [JsonPropertyName("updated_at")]
  public required DateTimeOffset UpdatedAt { get; set; }
}
