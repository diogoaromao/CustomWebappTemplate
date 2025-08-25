using Carter;
using MediatR;
using MyTemplate.Api.Common.Exceptions;
using MyTemplate.Api.Common.Models;

namespace MyTemplate.Api.Features.Products;

public static class DeleteProduct
{
    public class Command : IRequest
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Command>
    {
        // In a real application, you would inject a repository or DbContext
        private static readonly List<Product> _products = new()
        {
            new() { Id = 1, Name = "Sample Product 1", Description = "A sample product", Price = 19.99m, CreatedAt = DateTime.UtcNow.AddDays(-5), UpdatedAt = DateTime.UtcNow.AddDays(-5) },
            new() { Id = 2, Name = "Sample Product 2", Description = "Another sample product", Price = 29.99m, CreatedAt = DateTime.UtcNow.AddDays(-3), UpdatedAt = DateTime.UtcNow.AddDays(-3) },
            new() { Id = 3, Name = "Sample Product 3", Description = "Yet another sample product", Price = 39.99m, CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow.AddDays(-1) }
        };

        public Task Handle(Command request, CancellationToken cancellationToken)
        {
            var product = _products.FirstOrDefault(p => p.Id == request.Id);
            
            if (product is null)
                throw new NotFoundException(nameof(Product), request.Id);

            _products.Remove(product);
            
            return Task.CompletedTask;
        }
    }
}

public class DeleteProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/products/{id:int}", async (int id, ISender sender) =>
        {
            var command = new DeleteProduct.Command { Id = id };
            await sender.Send(command);
            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .WithTags("Products")
        .WithOpenApi();
    }
}