using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Api.Common.Errors;
using MyTemplate.Api.Common.Models;
using MyTemplate.Api.Common.Persistence;

namespace MyTemplate.Api.Features.Products;

public static class DeleteProduct
{
    public class Command : IRequest<ErrorOr<Success>>
    {
        public int Id { get; set; }
    }

    internal sealed class Handler : IRequestHandler<Command, ErrorOr<Success>>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<Success>> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            
            if (product is null)
                return Errors.Product.NotFound;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            
            return Result.Success;
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
            var result = await sender.Send(command);
            return result.Match(
                success => Results.NoContent(),
                error => Results.NotFound(error)
            );
        })
        .WithName("DeleteProduct")
        .WithTags("Products")
        .WithOpenApi();
    }
}