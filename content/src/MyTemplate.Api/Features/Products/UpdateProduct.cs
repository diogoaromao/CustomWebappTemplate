using Carter;
using FluentValidation;
using MediatR;
using MyTemplate.Api.Common.Exceptions;
using MyTemplate.Api.Common.Models;

namespace MyTemplate.Api.Features.Products;

public static class UpdateProduct
{
    public class Command : IRequest<Response>
    {
        public int Id { get; set; }
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
        public DateTime UpdatedAt { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

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
        private static readonly List<Product> _products = new()
        {
            new() { Id = 1, Name = "Sample Product 1", Description = "A sample product", Price = 19.99m, CreatedAt = DateTime.UtcNow.AddDays(-5), UpdatedAt = DateTime.UtcNow.AddDays(-5) },
            new() { Id = 2, Name = "Sample Product 2", Description = "Another sample product", Price = 29.99m, CreatedAt = DateTime.UtcNow.AddDays(-3), UpdatedAt = DateTime.UtcNow.AddDays(-3) },
            new() { Id = 3, Name = "Sample Product 3", Description = "Yet another sample product", Price = 39.99m, CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow.AddDays(-1) }
        };

        public async Task<Response> Handle(Command request, CancellationToken cancellationToken)
        {
            // Simulate async database operation
            await Task.Delay(10, cancellationToken);

            var product = _products.FirstOrDefault(p => p.Id == request.Id);
            
            if (product is null)
                throw new NotFoundException(nameof(Product), request.Id);

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.UpdatedAt = DateTime.UtcNow;

            var response = new Response
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                UpdatedAt = product.UpdatedAt
            };

            return response;
        }
    }
}

public class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/products/{id:int}", async (int id, UpdateProduct.Command command, ISender sender) =>
        {
            command.Id = id;
            var response = await sender.Send(command);
            return Results.Ok(response);
        })
        .WithName("UpdateProduct")
        .WithTags("Products")
        .WithOpenApi();
    }
}