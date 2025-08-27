# Environment Secrets Configuration

This template supports multiple ways to configure database connection strings for different environments using GitHub integration.

## 🚀 Quick Start (Recommended)

**For maximum security, use environment variables:**

1. **Set your GitHub token:**
   ```bash
   # Windows
   $env:GITHUB_TOKEN="ghp_your_token_here"
   
   # Linux/macOS  
   export GITHUB_TOKEN="ghp_your_token_here"
   ```

2. **Set your connection string:**
   ```bash
   # Windows
   $env:DEVELOPMENT_CONNECTION_STRING="Host=localhost;Database=MyApp_Dev;Username=postgres;Password=yourpassword"
   
   # Linux/macOS
   export DEVELOPMENT_CONNECTION_STRING="Host=localhost;Database=MyApp_Dev;Username=postgres;Password=yourpassword"
   ```

3. **Configure appsettings.Development.json:**
   ```json
   {
     "EnvironmentSecrets": {
       "Owner": "your-username",
       "Repository": "your-repo",
       "FetchFromGitHub": true
     }
   }
   ```

4. **Run:** `dotnet run --environment Development`

✅ **No secrets in source control!**

## Setup Options

### Option 1: GitHub Variables (Recommended for Development)

1. **Create GitHub Variables** in your repository:
   - Go to your repository on GitHub
   - Navigate to `Settings` → `Secrets and variables` → `Actions`
   - Click on the `Variables` tab
   - Add these variables:
     - `DEVELOPMENT_CONNECTION_STRING`: Your development database connection
     - `STAGING_CONNECTION_STRING`: Your staging database connection  
     - `PRODUCTION_CONNECTION_STRING`: Your production database connection

2. **Create a GitHub Personal Access Token**:
   - Go to GitHub Settings → Developer settings → Personal access tokens → Tokens (classic)
   - Create a token with `repo` scope (or just `public_repo` if using a public repository)
   - Store this token securely using one of the methods below

3. **Configure GitHub Token (Choose ONE secure method)**:

   **Method A: Environment Variable (Recommended)**
   ```bash
   # Windows (PowerShell) 
   $env:GITHUB_TOKEN="your_personal_access_token"
   
   # Linux/macOS
   export GITHUB_TOKEN="your_personal_access_token"
   
   # Alternative name (also supported)
   export GH_TOKEN="your_personal_access_token"
   ```

   **Method B: .NET User Secrets (Development)**
   ```bash
   # From API project directory
   dotnet user-secrets set "EnvironmentSecrets:GitHubToken" "your_personal_access_token"
   ```

   **Method C: Configuration File (NOT RECOMMENDED)**
   ```json
   // Only use for testing - never commit tokens to source control
   {
     "EnvironmentSecrets": {
       "GitHubToken": "your_personal_access_token"
     }
   }
   ```

4. **Update appsettings.Development.json** (keep token field empty):
```json
{
  "EnvironmentSecrets": {
    "Owner": "your-github-username",
    "Repository": "your-repo-name", 
    "GitHubToken": "",
    "FetchFromGitHub": true
  }
}
```

### Option 2: Environment Variables

Set environment variables directly on your system:

**Windows (PowerShell):**
```powershell
$env:DEVELOPMENT_CONNECTION_STRING="Host=localhost;Database=MyApp_Dev;Username=postgres;Password=yourpassword"
$env:STAGING_CONNECTION_STRING="Host=staging-db;Database=MyApp_Staging;Username=postgres;Password=yourpassword"
$env:PRODUCTION_CONNECTION_STRING="Host=prod-db;Database=MyApp_Prod;Username=postgres;Password=yourpassword"
```

**Linux/macOS:**
```bash
export DEVELOPMENT_CONNECTION_STRING="Host=localhost;Database=MyApp_Dev;Username=postgres;Password=yourpassword"
export STAGING_CONNECTION_STRING="Host=staging-db;Database=MyApp_Staging;Username=postgres;Password=yourpassword" 
export PRODUCTION_CONNECTION_STRING="Host=prod-db;Database=MyApp_Prod;Username=postgres;Password=yourpassword"
```

### Option 3: .NET User Secrets (Development Only)

For local development, you can use .NET User Secrets:

```bash
# Initialize user secrets (run from API project directory)
dotnet user-secrets init

# Set connection string
dotnet user-secrets set "DEVELOPMENT_CONNECTION_STRING" "Host=localhost;Database=MyApp_Dev;Username=postgres;Password=yourpassword"

# Set GitHub token (if using GitHub Variables)
dotnet user-secrets set "EnvironmentSecrets:GitHubToken" "your_personal_access_token"
dotnet user-secrets set "EnvironmentSecrets:Owner" "your-github-username" 
dotnet user-secrets set "EnvironmentSecrets:Repository" "your-repo-name"
```

Or copy `secrets.template.json` to set up multiple secrets at once:
```bash
# From the API project directory
dotnet user-secrets clear
cat ../secrets.template.json | dotnet user-secrets set
```

### Option 4: Local Configuration (Fallback)

The template includes fallback configuration in `appsettings.json`:

```json
{
  "EnvironmentSecrets": {
    "LocalConnectionStrings": {
      "development": "Host=localhost;Database=MyTemplateDb_Dev;Username=postgres;Password=postgres",
      "staging": "Host=localhost;Database=MyTemplateDb_Staging;Username=postgres;Password=postgres",
      "production": "Host=prod-server;Database=MyTemplateDb_Prod;Username=postgres;Password=secure_password"
    }
  }
}
```

## Priority Order

The application searches for connection strings in this order:

1. **GitHub Variables** (if `FetchFromGitHub: true`)
2. **Environment Variables** (e.g., `DEVELOPMENT_CONNECTION_STRING`)
3. **Local Configuration** (`EnvironmentSecrets.LocalConnectionStrings`)
4. **Standard Configuration** (`ConnectionStrings.DefaultConnection`)

## Security Best Practices

### ✅ DO:
- **Store GitHub tokens as environment variables** (`GITHUB_TOKEN` or `GH_TOKEN`)
- Use .NET User Secrets for development (`dotnet user-secrets`)
- Use GitHub Variables for non-sensitive configuration
- Use GitHub Secrets for sensitive data in CI/CD
- Use environment variables on production servers
- Keep Personal Access Tokens secure and rotate regularly
- Use different databases for each environment
- Set minimal permissions on Personal Access Tokens (only `repo` or `public_repo`)

### ❌ DON'T:
- **Never commit Personal Access Tokens to source control**
- **Never put tokens in appsettings.json files**
- Don't commit connection strings to source control
- Don't share Personal Access Tokens
- Don't use production credentials in development
- Don't store secrets in plain text files
- Don't give excessive permissions to tokens

### 🔐 **Token Security Priority Order:**
1. **Environment Variables** (`$env:GITHUB_TOKEN` or `export GITHUB_TOKEN`) - ⭐ **RECOMMENDED**
2. **.NET User Secrets** (`dotnet user-secrets set`) - Good for development
3. **Configuration files** (appsettings.json) - ❌ **AVOID**

## Connection String Format

PostgreSQL connection string format:
```
Host=your-host;Database=your-database;Username=your-username;Password=your-password;SSL Mode=Require
```

## Environment-Specific Setup

### Development
- Uses `ASPNETCORE_ENVIRONMENT=Development`
- Looks for `DEVELOPMENT_CONNECTION_STRING`
- Falls back to local development database

### Staging  
- Uses `ASPNETCORE_ENVIRONMENT=Staging`
- Looks for `STAGING_CONNECTION_STRING`
- Configured via CI/CD environment variables

### Production
- Uses `ASPNETCORE_ENVIRONMENT=Production` 
- Looks for `PRODUCTION_CONNECTION_STRING`
- Configured via CI/CD environment variables

## Troubleshooting

### Connection String Not Found
If you see: `No connection string found for environment: development`

1. Check that your environment name matches the configuration
2. Verify GitHub token has correct permissions
3. Confirm environment variables are set correctly
4. Check the `LocalConnectionStrings` fallback configuration

### GitHub API Issues
- Ensure Personal Access Token has `repo` scope
- Verify repository owner and name are correct
- Check that GitHub Variables are created (not Secrets)
- Review application logs for specific API errors

## Example: Complete Development Setup

1. **Create GitHub Variables in your repository:**
   ```
   DEVELOPMENT_CONNECTION_STRING = "Host=localhost;Database=MyApp_Dev;Username=postgres;Password=devpassword"
   ```

2. **Set GitHub token as environment variable (SECURE METHOD):**
   ```bash
   # Windows (PowerShell) - Set for current session
   $env:GITHUB_TOKEN="ghp_your_actual_token_here"
   
   # Windows (PowerShell) - Set permanently (restart terminal after)
   [Environment]::SetEnvironmentVariable("GITHUB_TOKEN", "ghp_your_actual_token_here", "User")
   
   # Linux/macOS - Add to ~/.bashrc or ~/.zshrc for persistence
   export GITHUB_TOKEN="ghp_your_actual_token_here"
   ```

3. **Configure appsettings.Development.json (NO TOKEN IN FILE):**
   ```json
   {
     "EnvironmentSecrets": {
       "Owner": "myusername",
       "Repository": "myapp-repo",
       "GitHubToken": "",
       "FetchFromGitHub": true
     }
   }
   ```

4. **Alternative: Use .NET User Secrets (Development Only):**
   ```bash
   # From API project directory
   dotnet user-secrets set "EnvironmentSecrets:GitHubToken" "ghp_your_actual_token_here"
   dotnet user-secrets set "EnvironmentSecrets:Owner" "myusername"
   dotnet user-secrets set "EnvironmentSecrets:Repository" "myapp-repo"
   ```

5. **Run the application:**
   ```bash
   dotnet run --environment Development
   ```

The application will automatically:
- Get the GitHub token from environment variable (or user secrets)
- Fetch the connection string from GitHub Variables
- Connect to your development database

✅ **Your token is never stored in source control!**