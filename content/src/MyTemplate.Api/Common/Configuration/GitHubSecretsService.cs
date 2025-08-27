using Microsoft.Extensions.Options;
using Octokit;

namespace MyTemplate.Api.Common.Configuration;

public class GitHubSecretsService : IGitHubSecretsService
{
    private readonly GitHubClient _client;
    private readonly GitHubSecretsOptions _options;
    private readonly ILogger<GitHubSecretsService> _logger;

    public GitHubSecretsService(IOptions<GitHubSecretsOptions> options, ILogger<GitHubSecretsService> logger)
    {
        _options = options.Value;
        _logger = logger;
        
        if (!string.IsNullOrEmpty(_options.Token))
        {
            _client = new GitHubClient(new ProductHeaderValue("MyTemplate-Api"))
            {
                Credentials = new Credentials(_options.Token)
            };
        }
        else
        {
            _client = new GitHubClient(new ProductHeaderValue("MyTemplate-Api"));
        }
    }

    public async Task<string?> GetSecretAsync(string secretName, CancellationToken cancellationToken = default)
    {
        if (!_options.Enabled || string.IsNullOrEmpty(_options.Owner) || string.IsNullOrEmpty(_options.Repository))
        {
            _logger.LogWarning("GitHub Secrets service is not enabled or not properly configured");
            return null;
        }

        try
        {
            // Note: GitHub API doesn't allow reading secret values for security reasons
            // This is a conceptual implementation - you'd need to use GitHub Actions variables instead
            // or consider using Azure Key Vault, AWS Secrets Manager, etc.
            
            _logger.LogWarning("GitHub Secrets cannot be read via API. Consider using GitHub Actions variables or external secret management.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve secret {SecretName} from GitHub", secretName);
            return null;
        }
    }

    public async Task<Dictionary<string, string>> GetSecretsAsync(string[] secretNames, CancellationToken cancellationToken = default)
    {
        var secrets = new Dictionary<string, string>();
        
        foreach (var secretName in secretNames)
        {
            var value = await GetSecretAsync(secretName, cancellationToken);
            if (value != null)
            {
                secrets[secretName] = value;
            }
        }
        
        return secrets;
    }
}