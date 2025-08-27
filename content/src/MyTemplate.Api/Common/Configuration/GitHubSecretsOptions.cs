namespace MyTemplate.Api.Common.Configuration;

public class GitHubSecretsOptions
{
    public const string SectionName = "GitHubSecrets";
    
    public string? Owner { get; set; }
    public string? Repository { get; set; }
    public string? Token { get; set; }
    public bool Enabled { get; set; } = false;
}