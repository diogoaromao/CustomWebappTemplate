using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Api.Common.Errors;
using MyTemplate.Api.Common.Models;
using MyTemplate.Api.Common.Persistence;

namespace MyTemplate.Api.Features.ShoppingCart;

public static class ClearCart
{
    public class Command : IRequest<ErrorOr<Success>>
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");
        }
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
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

            if (cart is null)
            {
                return Errors.Cart.NotFound;
            }

            cart.Items.Clear();
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}

public class ClearCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/cart/{userId}", async (string userId, ISender sender) =>
        {
            var command = new ClearCart.Command { UserId = userId };
            var result = await sender.Send(command);
            return result.Match(
                success => Results.NoContent(),
                error => Results.BadRequest(error)
            );
        })
        .WithName("ClearCart")
        .WithTags("Shopping Cart")
        .WithOpenApi();
    }
}