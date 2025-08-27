using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Api.Common.Errors;
using MyTemplate.Api.Common.Models;
using MyTemplate.Api.Common.Persistence;

namespace MyTemplate.Api.Features.Products;

public static class GetProductById
{
    public class Query : IRequest<ErrorOr<Response>>
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

    internal sealed class Handler : IRequestHandler<Query, ErrorOr<Response>>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<Response>> Handle(Query request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            
            if (product is null)
                return Errors.Product.NotFound;

            var response = new Response
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return response;
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
            var result = await sender.Send(query);
            return result.Match(
                response => Results.Ok(response),
                error => Results.NotFound(error)
            );
        })
        .WithName("GetProductById")
        .WithTags("Products")
        .WithOpenApi();
    }
}