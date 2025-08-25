# Custom Web App Template

A .NET template that combines ASP.NET Core Web API with Vue.js frontend in a single solution.

## Features

- **Backend**: ASP.NET Core Web API (.NET 9) with OpenAPI/Swagger
- **Frontend**: Vue 3 with TypeScript, Vite, and ESLint
- **Docker**: Containerization support for both projects
- **CI/CD**: GitHub Actions workflow for automated deployment

## Installation

### PowerShell (Windows)
```powershell
# Download the template package
Invoke-WebRequest -Uri "https://github.com/diogoaromao/CustomWebappTemplate/releases/latest/download/DiogoRomao.CustomWebappTemplate.1.0.3.nupkg" -OutFile "DiogoRomao.CustomWebappTemplate.nupkg"

# Install the template
dotnet new install .\DiogoRomao.CustomWebappTemplate.nupkg
```

### Bash (Linux/macOS)
```bash
# Download the template package
curl -L -o DiogoRomao.CustomWebappTemplate.nupkg https://github.com/diogoaromao/CustomWebappTemplate/releases/latest/download/DiogoRomao.CustomWebappTemplate.1.0.3.nupkg

# Install the template
dotnet new install ./DiogoRomao.CustomWebappTemplate.nupkg
```

### Alternative: Direct Download
1. Go to [Releases](https://github.com/diogoaromao/CustomWebappTemplate/releases/latest)
2. Download `DiogoRomao.CustomWebappTemplate.1.0.3.nupkg`
3. Run `dotnet new install .\DiogoRomao.CustomWebappTemplate.nupkg`

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

## CI/CD

The template includes a GitHub Actions workflow (`deploy.yml`) that:
- Builds Docker images for both API and Web
- Deploys to staging and production environments
- Uses Portainer for container management

To enable the workflow:
1. Uncomment the `on:` trigger section in `.github/workflows/deploy.yml`
2. Set up the required GitHub secrets (Docker credentials, Portainer settings)

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