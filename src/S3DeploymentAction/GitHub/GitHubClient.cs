namespace S3DeploymentAction.GitHub;

public class GitHubClient
{
  private readonly HttpClient _client;

  public string? RepositoryOwner { get; set; }

  public string? RepositoryName { get; set; }

  public GitHubClient(string token)
  {
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
    string owner,
    string repository) : this(token)
  {
    RepositoryOwner = owner;
    RepositoryName = repository;
  }

  public async Task CreateDeployment()
  {
  }
}