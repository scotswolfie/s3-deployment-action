using System.Text.Json.Serialization;

namespace S3DeploymentAction.GitHub.Model;

public class CreateDeploymentStatusRequest()
{
  [JsonPropertyName("state")]
  public required string State { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("log_url")]
  public Uri? LogUrl { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("description")]
  public string? Description { get; set; }
  
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("environment")]
  public string? Environment { get; set; }
  
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("environment_url")]
  public Uri? EnvironmentUrl { get; set; }
  
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("auto_inactive")]
  public string? AutoInactive { get; set; }
}
