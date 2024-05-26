using System.Text.Json;
using S3DeploymentAction.GitHub.Model;

namespace S3DeploymentAction.GitHub;

public class GitHubClient
{
  private readonly HttpClient _client;

  public string? FullRepositoryName { get; set; }

  public GitHubClient(string token)
  {
    ArgumentNullException.ThrowIfNull(token);

    _client = new()
    {
      BaseAddress = new("https://api.github.com"),
    };

    _client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
    _client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
    _client.DefaultRequestHeaders.Add("User-Agent", "s3-deployment-action");
  }

  public GitHubClient(
    string token,
    string fullRepositoryName) : this(token)
  {
    ArgumentNullException.ThrowIfNull(fullRepositoryName);

    // Any correct GitHub repository name will contain a single forward slash.
    if (!fullRepositoryName.Contains('/') || fullRepositoryName.IndexOf('/') != fullRepositoryName.LastIndexOf('/'))
    {
      throw new ArgumentException(
        "Repository name must be in the following format: owner/repository.",
        nameof(fullRepositoryName));
    }

    FullRepositoryName = fullRepositoryName;
  }

  private async Task<TResponse> PostRequestAsync<TRequest, TResponse>(
    string path,
    TRequest request)
  {
    ArgumentNullException.ThrowIfNull(path);
    ArgumentNullException.ThrowIfNull(request);

    if (FullRepositoryName is null)
    {
      throw new InvalidOperationException($"{nameof(FullRepositoryName)} must be set before calling the GitHub API.");
    }

    using MemoryStream stream = new();

    await JsonSerializer.SerializeAsync(stream, request);

    stream.Position = 0;

    StreamReader reader = new(stream);
    ConsoleLogger.Verbose($"Sending POST request to {path}. Request body:\n{reader.ReadToEnd()}");

    stream.Position = 0;

    using HttpResponseMessage response = await _client.PostAsync(path, new StreamContent(stream));

    if (!response.IsSuccessStatusCode)
    {
      string message = await response.Content.ReadAsStringAsync();

      string errorMessage = "GitHub API error encountered:\n"
                            + $"Response code: {response.StatusCode}\n"
                            + $"Response body: {message}";

      throw new Exception(errorMessage);
    }

    try
    {
      await using Stream responseStream = await response.Content.ReadAsStreamAsync();

      TResponse? parsedResponse = await JsonSerializer.DeserializeAsync<TResponse>(responseStream);

      if (parsedResponse is null)
      {
        throw new Exception("The response from GitHub API could not be parsed.");
      }

      return parsedResponse;
    }
    catch (JsonException ex)
    {
      string responseBody = await response.Content.ReadAsStringAsync();

      throw new Exception($"Following body was returned from GitHub and could not be parsed:\n{responseBody}", ex);
    }
  }

  public async Task<CreateDeploymentResponse> CreateDeployment(CreateDeploymentRequest request)
  {
    ArgumentNullException.ThrowIfNull(request);

    string path = $"/repos/{FullRepositoryName}/deployments";

    return await PostRequestAsync<CreateDeploymentRequest, CreateDeploymentResponse>(path, request);
  }

  public async Task<CreateDeploymentStatusResponse> CreateDeploymentStatus(
    long deploymentId,
    CreateDeploymentStatusRequest request)
  {
    ArgumentNullException.ThrowIfNull(request);

    string path = $"/repos/{FullRepositoryName}/deployments/{deploymentId}/statuses";

    return await PostRequestAsync<CreateDeploymentStatusRequest, CreateDeploymentStatusResponse>(path, request);
  }
}
