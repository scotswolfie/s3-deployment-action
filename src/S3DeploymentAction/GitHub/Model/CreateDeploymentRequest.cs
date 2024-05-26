using System.Text.Json.Serialization;

namespace S3DeploymentAction.GitHub.Model;

public class CreateDeploymentRequest()
{
  [JsonPropertyName("ref")]
  public required string Ref { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("task")]
  public string? Task { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("auto_merge")]
  public bool? AutoMerge { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("required_contexts")]
  public IEnumerable<string>? RequiredContexts { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("payload")]
  public Dictionary<string, object>? Payload { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("environment")]
  public string? Environment { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("description")]
  public string? Description { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("transient_environment")]
  public bool? TransientEnvironment { get; set; }

  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonPropertyName("production_environment")]
  public bool? ProductionEnvironment { get; set; }
}
