using Carter;
using MediatR;
using MyTemplate.Api.Common.Exceptions;
using MyTemplate.Api.Common.Models;

namespace MyTemplate.Api.Features.Products;

public static class GetProductById
{
    public class Query : IRequest<Response>
    {
        public int Id { get; set; }
    }

    public class Response
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Query, Response>
    {
        // In a real application, you would inject a repository or DbContext
        private static readonly List<Product> _products = new()
        {
            new() { Id = 1, Name = "Sample Product 1", Description = "A sample product", Price = 19.99m, CreatedAt = DateTime.UtcNow.AddDays(-5), UpdatedAt = DateTime.UtcNow.AddDays(-5) },
            new() { Id = 2, Name = "Sample Product 2", Description = "Another sample product", Price = 29.99m, CreatedAt = DateTime.UtcNow.AddDays(-3), UpdatedAt = DateTime.UtcNow.AddDays(-3) },
            new() { Id = 3, Name = "Sample Product 3", Description = "Yet another sample product", Price = 39.99m, CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow.AddDays(-1) }
        };

        public Task<Response> Handle(Query request, CancellationToken cancellationToken)
        {
            var product = _products.FirstOrDefault(p => p.Id == request.Id);
            
            if (product is null)
                throw new NotFoundException(nameof(Product), request.Id);

            var response = new Response
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return Task.FromResult(response);
        }
    }
}

public class GetProductByIdEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products/{id:int}", async (int id, ISender sender) =>
        {
            var query = new GetProductById.Query { Id = id };
            var response = await sender.Send(query);
            return Results.Ok(response);
        })
        .WithName("GetProductById")
        .WithTags("Products")
        .WithOpenApi();
    }
}