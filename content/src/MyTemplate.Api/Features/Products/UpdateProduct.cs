using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Api.Common.Errors;
using MyTemplate.Api.Common.Models;
using MyTemplate.Api.Common.Persistence;

namespace MyTemplate.Api.Features.Products;

public static class UpdateProduct
{
    public class Command : IRequest<ErrorOr<Response>>
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

    internal sealed class Handler : IRequestHandler<Command, ErrorOr<Response>>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            
            if (product is null)
                return Errors.Product.NotFound;

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

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
            var result = await sender.Send(command);
            return result.Match(
                response => Results.Ok(response),
                error => Results.NotFound(error)
            );
        })
        .WithName("UpdateProduct")
        .WithTags("Products")
        .WithOpenApi();
    }
}