using Carter;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Api.Common.Behaviors;
using MyTemplate.Api.Common.Configuration;
using MyTemplate.Api.Common.Persistence;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var assembly = Assembly.GetExecutingAssembly();

// Configure Environment Secrets Service
builder.Services.Configure<EnvironmentSecretsOptions>(
    builder.Configuration.GetSection(EnvironmentSecretsOptions.SectionName));
builder.Services.AddHttpClient<IEnvironmentSecretsService, EnvironmentSecretsService>();

// Get connection string based on environment
var environmentSecretsService = builder.Services.BuildServiceProvider()
    .GetRequiredService<IEnvironmentSecretsService>();
var environment = builder.Environment.EnvironmentName.ToLower();
var connectionString = await environmentSecretsService.GetConnectionStringAsync(environment);

// Fallback to appsettings if no connection string found
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException($"No connection string found for environment: {environment}");
}

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddCarter();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add Carter for minimal API endpoints
app.MapCarter();

app.Run();
