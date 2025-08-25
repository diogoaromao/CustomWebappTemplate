using Carter;
using FluentValidation;
using MediatR;
using MyTemplate.Api.Common.Models;

namespace MyTemplate.Api.Features.Products;

public static class CreateProduct
{
    public class Command : IRequest<Response>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class Response
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);

            RuleFor(x => x.Price)
                .GreaterThan(0);
        }
    }

    internal sealed class Handler : IRequestHandler<Command, Response>
    {
        // In a real application, you would inject a repository or DbContext
        private static readonly List<Product> _products = new();
        private static int _nextId = 1;

        public Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = _nextId++,
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _products.Add(product);

            var response = new Response
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt
            };

            return Task.FromResult(response);
        }
    }
}

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/products", async (CreateProduct.Command command, ISender sender) =>
        {
            var response = await sender.Send(command);
            return Results.Created($"/api/products/{response.Id}", response);
        })
        .WithName("CreateProduct")
        .WithTags("Products")
        .WithOpenApi();
    }
}