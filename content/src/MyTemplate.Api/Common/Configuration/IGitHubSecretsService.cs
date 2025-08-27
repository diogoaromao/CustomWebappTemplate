namespace MyTemplate.Api.Common.Configuration;

public interface IGitHubSecretsService
{
    Task<string?> GetSecretAsync(string secretName, CancellationToken cancellationToken = default);
    Task<Dictionary<string, string>> GetSecretsAsync(string[] secretNames, CancellationToken cancellationToken = default);
}