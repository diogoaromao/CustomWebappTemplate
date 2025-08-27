using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using MyTemplate.Api.Common.Models;
using MyTemplate.Api.Common.Persistence;

namespace MyTemplate.Api.Features.Products;

public static class CreateProduct
{
    public class Command : IRequest<ErrorOr<Response>>
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

    internal sealed class Handler : IRequestHandler<Command, ErrorOr<Response>>
    {
        private readonly ApplicationDbContext _context;

        public Handler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ErrorOr<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new Response
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt
            };

            return response;
        }
    }
}

public class CreateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/products", async (CreateProduct.Command command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return result.Match(
                response => Results.Created($"/api/products/{response.Id}", response),
                error => Results.BadRequest(error)
            );
        })
        .WithName("CreateProduct")
        .WithTags("Products")
        .WithOpenApi();
    }
}