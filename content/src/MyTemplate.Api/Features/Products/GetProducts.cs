using Carter;
using MediatR;
using MyTemplate.Api.Common.Models;

namespace MyTemplate.Api.Features.Products;

public static class GetProducts
{
    public class Query : IRequest<Response>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class Response
    {
        public List<ProductDto> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
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
            var skip = (request.Page - 1) * request.PageSize;
            var products = _products
                .Skip(skip)
                .Take(request.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    CreatedAt = p.CreatedAt
                })
                .ToList();

            var response = new Response
            {
                Products = products,
                TotalCount = _products.Count,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return Task.FromResult(response);
        }
    }
}

public class GetProductsEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/products", async (ISender sender, int page = 1, int pageSize = 10) =>
        {
            var query = new GetProducts.Query { Page = page, PageSize = pageSize };
            var response = await sender.Send(query);
            return Results.Ok(response);
        })
        .WithName("GetProducts")
        .WithTags("Products")
        .WithOpenApi();
    }
}