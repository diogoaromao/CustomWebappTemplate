using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Api.Common.Errors;
using MyTemplate.Api.Common.Models;
using MyTemplate.Api.Common.Persistence;

namespace MyTemplate.Api.Features.ShoppingCart;

public static class RemoveItemFromCart
{
    public class Command : IRequest<ErrorOr<Response>>
    {
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
    }

    public class Response
    {
        public string UserId { get; set; } = string.Empty;
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("Product ID must be greater than zero");
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
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

            if (cart is null)
            {
                return Errors.Cart.NotFound;
            }

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item is null)
            {
                return Errors.Cart.ItemNotFound;
            }

            cart.Items.Remove(item);
            cart.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            var response = new Response
            {
                UserId = cart.UserId,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    TotalPrice = i.TotalPrice
                }).ToList(),
                TotalAmount = cart.TotalAmount,
                TotalItems = cart.TotalItems
            };

            return response;
        }
    }
}

public class RemoveItemFromCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/cart/{userId}/items/{productId:int}", async (string userId, int productId, ISender sender) =>
        {
            var command = new RemoveItemFromCart.Command
            {
                UserId = userId,
                ProductId = productId
            };

            var result = await sender.Send(command);
            return result.Match(
                response => Results.Ok(response),
                error => Results.BadRequest(error)
            );
        })
        .WithName("RemoveItemFromCart")
        .WithTags("Shopping Cart")
        .WithOpenApi();
    }
}