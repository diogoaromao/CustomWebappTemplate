using Microsoft.Extensions.Options;
using System.Text.Json;

namespace MyTemplate.Api.Common.Configuration;

public interface IEnvironmentSecretsService
{
    Task<string?> GetConnectionStringAsync(string environment, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetAllSecretsAsync(string environment, CancellationToken cancellationToken = default);
}

public class EnvironmentSecretsOptions
{
    public const string SectionName = "EnvironmentSecrets";
    
    /// <summary>
    /// GitHub Personal Access Token for accessing repository variables
    /// </summary>
    public string? GitHubToken { get; set; }
    
    /// <summary>
    /// Repository owner (username or organization)
    /// </summary>
    public string? Owner { get; set; }
    
    /// <summary>
    /// Repository name
    /// </summary>
    public string? Repository { get; set; }
    
    /// <summary>
    /// Whether to fetch secrets from GitHub Variables during startup
    /// </summary>
    public bool FetchFromGitHub { get; set; } = false;
    
    /// <summary>
    /// Fallback connection strings for local development
    /// </summary>
    public Dictionary<string, string> LocalConnectionStrings { get; set; } = new();
}

public class EnvironmentSecretsService : IEnvironmentSecretsService
{
    private readonly EnvironmentSecretsOptions _options;
    private readonly ILogger<EnvironmentSecretsService> _logger;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, string> _cachedSecrets = new();

    public EnvironmentSecretsService(
        IOptions<EnvironmentSecretsOptions> options, 
        ILogger<EnvironmentSecretsService> logger,
        HttpClient httpClient)
    {
        _options = options.Value;
        _logger = logger;
        _httpClient = httpClient;

        // Priority order for GitHub token:
        // 1. Environment variable (most secure)
        // 2. User secrets (development only)
        // 3. Configuration file (not recommended)
        var gitHubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN") 
                         ?? Environment.GetEnvironmentVariable("GH_TOKEN")
                         ?? _options.GitHubToken;

        if (!string.IsNullOrEmpty(gitHubToken))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {gitHubToken}");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MyTemplate-Api");
            _logger.LogInformation("GitHub API client configured with token");
        }
        else
        {
            _logger.LogWarning("No GitHub token found. GitHub Variables will not be accessible.");
        }
    }

    public async Task<string?> GetConnectionStringAsync(string environment, CancellationToken cancellationToken = default)
    {
        var secretName = $"{environment.ToUpper()}_CONNECTION_STRING";
        
        // Try cache first
        if (_cachedSecrets.TryGetValue(secretName, out var cachedValue))
        {
            return cachedValue;
        }

        // Try GitHub Variables if enabled
        if (_options.FetchFromGitHub && !string.IsNullOrEmpty(_options.Owner) && !string.IsNullOrEmpty(_options.Repository))
        {
            var githubValue = await FetchFromGitHubVariablesAsync(secretName, cancellationToken);
            if (!string.IsNullOrEmpty(githubValue))
            {
                _cachedSecrets[secretName] = githubValue;
                return githubValue;
            }
        }

        // Try environment variables
        var envValue = Environment.GetEnvironmentVariable(secretName);
        if (!string.IsNullOrEmpty(envValue))
        {
            _cachedSecrets[secretName] = envValue;
            return envValue;
        }

        // Try local configuration fallback
        if (_options.LocalConnectionStrings.TryGetValue(environment.ToLower(), out var localValue))
        {
            _cachedSecrets[secretName] = localValue;
            return localValue;
        }

        _logger.LogWarning("Connection string for environment {Environment} not found", environment);
        return null;
    }

    public async Task<Dictionary<string, string>> GetAllSecretsAsync(string environment, CancellationToken cancellationToken = default)
    {
        var secrets = new Dictionary<string, string>();
        
        var connectionString = await GetConnectionStringAsync(environment, cancellationToken);
        if (!string.IsNullOrEmpty(connectionString))
        {
            secrets[$"{environment.ToUpper()}_CONNECTION_STRING"] = connectionString;
        }

        return secrets;
    }

    private async Task<string?> FetchFromGitHubVariablesAsync(string variableName, CancellationToken cancellationToken)
    {
        try
        {
            var url = $"https://api.github.com/repos/{_options.Owner}/{_options.Repository}/actions/variables/{variableName}";
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var variable = JsonSerializer.Deserialize<GitHubVariable>(content);
                return variable?.Value;
            }
            
            _logger.LogWarning("GitHub variable {VariableName} not found. Status: {StatusCode}", variableName, response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch GitHub variable {VariableName}", variableName);
            return null;
        }
    }

    private class GitHubVariable
    {
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}