# Custom Web App Template

A .NET template that combines ASP.NET Core Web API with Vue.js frontend in a single solution.

## Features

- **Backend**: ASP.NET Core Web API (.NET 9) with OpenAPI/Swagger
- **Frontend**: Vue 3 with TypeScript, Vite, and ESLint
- **Database**: Entity Framework Core with PostgreSQL (uses project-named schema)
- **Architecture**: Vertical slice architecture with CQRS and ErrorOr pattern
- **Docker**: Containerization support for both projects
- **CI/CD**: GitHub Actions workflow with automated database migrations

## Installation

### PowerShell (Windows)
```powershell
# Download the template package (replace with latest version number)
Invoke-WebRequest -Uri "https://github.com/diogoaromao/CustomWebappTemplate/releases/download/v1.2.2/DiogoRomao.CustomWebappTemplate.1.2.2.nupkg" -OutFile "DiogoRomao.CustomWebappTemplate.nupkg"

# Install the template
dotnet new install .\DiogoRomao.CustomWebappTemplate.nupkg
```

### Bash (Linux/macOS)
```bash
# Download the template package (replace with latest version number)
curl -L -o DiogoRomao.CustomWebappTemplate.nupkg https://github.com/diogoaromao/CustomWebappTemplate/releases/download/v1.2.2/DiogoRomao.CustomWebappTemplate.1.2.2.nupkg

# Install the template
dotnet new install ./DiogoRomao.CustomWebappTemplate.nupkg
```

### Easy Method: Manual Download
1. Go to [Releases](https://github.com/diogoaromao/CustomWebappTemplate/releases/latest)
2. Download the latest `.nupkg` file
3. Run `dotnet new install .\[downloaded-file-name].nupkg`

### Troubleshooting

If you get "File already exists" error:
```powershell
# Uninstall the existing template first
dotnet new uninstall DiogoRomao.CustomWebappTemplate

# Then install the new version
dotnet new install .\DiogoRomao.CustomWebappTemplate.nupkg
```

## Usage

Create a new project using the template:

```bash
dotnet new webappvue -n MyAwesomeProject
```

This will create a new solution with:
- `MyAwesomeProject.Api` - ASP.NET Core Web API
- `MyAwesomeProject.Web` - Vue.js frontend

## Development

### Backend (API)
```bash
cd src/MyAwesomeProject.Api
dotnet run
```

### Frontend (Web)
```bash
cd src/MyAwesomeProject.Web
npm run dev
```

### Build Everything
```bash
# From the solution root
dotnet build MyAwesomeProject.sln

# Frontend production build
cd src/MyAwesomeProject.Web
npm run build
```

## Database Configuration

The template supports multiple environments with flexible secret management:

- **Development**: Automatically loads connection strings from GitHub Variables, environment variables, or local configuration
- **Staging/Production**: Uses environment variables or CI/CD secrets
- **Schema**: All database objects are created in a schema named after the project (e.g., "MyAwesomeProject" schema)

For detailed setup instructions, see [SECRETS_SETUP.md](SECRETS_SETUP.md).

For database schema information, see [DATABASE_SCHEMA.md](DATABASE_SCHEMA.md).

### Quick Start for Development

1. Set environment variable:
   ```bash
   # Windows (PowerShell)
   $env:DEVELOPMENT_CONNECTION_STRING="Host=localhost;Database=MyApp_Dev;Username=postgres;Password=yourpassword"
   
   # Linux/macOS  
   export DEVELOPMENT_CONNECTION_STRING="Host=localhost;Database=MyApp_Dev;Username=postgres;Password=yourpassword"
   ```

2. Or use the fallback configuration in `appsettings.json` (already configured for local PostgreSQL)

## CI/CD

The template includes a GitHub Actions workflow (`deploy.yml`) that:
- Builds Docker images for both API and Web
- Runs Entity Framework database migrations
- Deploys to staging and production environments
- Uses Portainer for container management

### Database Migrations

The workflow automatically applies Entity Framework migrations to both staging and production databases before deploying the API containers. This ensures the database schema is always up-to-date with your code.

### Required GitHub Secrets

To enable the workflow:
1. Uncomment the `on:` trigger section in `.github/workflows/deploy.yml`
2. Set up the required GitHub secrets:
   - `DOCKER_USERNAME` & `DOCKER_PASSWORD`: Docker Hub credentials
   - `PORTAINER_URL`, `PORTAINER_USERNAME`, `PORTAINER_PASSWORD`, `PORTAINER_ENDPOINT_ID`: Portainer settings
   - `STAGING_CONNECTION_STRING` & `PRODUCTION_CONNECTION_STRING`: PostgreSQL database connection strings

Example connection string format:
```
Host=your-db-host;Database=your-db-name;Username=your-username;Password=your-password;SSL Mode=Require
```

## Project Structure

```
MyAwesomeProject/
├── src/
│   ├── MyAwesomeProject.Api/     # ASP.NET Core Web API
│   └── MyAwesomeProject.Web/     # Vue.js frontend
├── tests/                        # Test projects
├── .github/workflows/
│   └── deploy.yml               # CI/CD pipeline (disabled by default)
└── MyAwesomeProject.sln         # Visual Studio solution
```

### Database Schema

All database objects are organized under a schema that matches your project name:
- Tables: `MyAwesomeProject.Products`, `MyAwesomeProject.Carts`, etc.
- This provides better organization and avoids conflicts with other applications
- The template engine automatically replaces "MyTemplate" with your project name