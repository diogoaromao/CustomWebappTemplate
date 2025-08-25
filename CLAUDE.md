# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Architecture

This is a custom web application template combining ASP.NET Core Web API backend with Vue.js frontend in a single Visual Studio solution. The project consists of two main applications:

- **MyTemplate.Api**: ASP.NET Core Web API (.NET 9) located in `content/src/MyTemplate.Api/`
- **MyTemplate.Web**: Vue.js 3 frontend with TypeScript located in `content/src/Mytemplate.Web/`

## Development Commands

### Frontend (MyTemplate.Web)
Navigate to `content/src/Mytemplate.Web/` and run:
- `npm run dev` - Start development server (runs on port 51472)
- `npm run build` - Build for production (runs type-check and vite build)
- `npm run type-check` - Run TypeScript type checking with vue-tsc
- `npm run lint` - Run ESLint with auto-fix
- `npm run preview` - Preview production build

### Backend (MyTemplate.Api)
Navigate to `content/src/MyTemplate.Api/` and run:
- `dotnet run` - Start the API development server
- `dotnet build` - Build the API project
- `dotnet test` - Run tests (if any exist in tests directory)

### Solution Level
From the `content/` directory:
- `dotnet build MyTemplate.sln` - Build entire solution
- Visual Studio: Open `MyTemplate.sln` to work with both projects

## Key Configuration Files

- **Frontend**: `vite.config.ts` configures Vite dev server on port 51472
- **Backend**: `Program.cs` contains minimal API setup with OpenAPI/Swagger in development
- **Docker**: Both projects include Dockerfiles for containerization

## Technology Stack

- Backend: ASP.NET Core Web API, .NET 9, OpenAPI/Swagger
- Frontend: Vue 3 with Composition API, TypeScript, Vite
- Build: npm-run-all2 for parallel build tasks
- Linting: ESLint with Vue and TypeScript configurations

## Project Structure Notes

- The solution follows a standard .NET project structure with `src/` and `tests/` folders
- Frontend uses modern Vue 3 Composition API with `<script setup>` syntax
- Both projects are configured for Docker deployment
- The frontend esproj integrates with MSBuild and supports Vitest for testing