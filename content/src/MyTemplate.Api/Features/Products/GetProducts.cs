using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Api.Common.Models;
using MyTemplate.Api.Common.Persistence;

namespace MyTemplate.Api.Features.Products;

public static class GetProducts
{
    public class Query : IRequest<ErrorOr<Response>>
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

    internal sealed class Handler : IRequestHandler<Query, ErrorOr<Response>>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var skip = (request.Page - 1) * request.PageSize;
            
            var totalCount = await _context.Products.CountAsync(cancellationToken);
            
            var products = await _context.Products
                .OrderBy(p => p.Id)
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
                .ToListAsync(cancellationToken);

            var response = new Response
            {
                Products = products,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return response;
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
            var result = await sender.Send(query);
            return result.Match(
                response => Results.Ok(response),
                error => Results.BadRequest(error)
            );
        })
        .WithName("GetProducts")
        .WithTags("Products")
        .WithOpenApi();
    }
}