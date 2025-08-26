using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using MyTemplate.Api.Common.Errors;
using MyTemplate.Api.Common.Extensions;
using MyTemplate.Api.Common.Models;

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
        // In a real application, you would inject a repository or DbContext
        private static readonly Dictionary<string, Cart> _carts = new();

        public async Task<ErrorOr<Success>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Simulate async database operation
            await Task.Delay(10, cancellationToken);

            if (!_carts.TryGetValue(request.UserId, out var cart))
            {
                return Errors.Cart.NotFound;
            }

            cart.Items.Clear();
            cart.UpdatedAt = DateTime.UtcNow;

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
            return result.ToHttpResult();
        })
        .WithName("ClearCart")
        .WithTags("Shopping Cart")
        .WithOpenApi();
    }
}