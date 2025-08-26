using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using MyTemplate.Api.Common.Errors;
using MyTemplate.Api.Common.Extensions;
using MyTemplate.Api.Common.Models;

namespace MyTemplate.Api.Features.ShoppingCart;

public static class AddItemToCart
{
    public class Command : IRequest<ErrorOr<Response>>
    {
        public string UserId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; }
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

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero");
        }
    }

    internal sealed class Handler : IRequestHandler<Command, ErrorOr<Response>>
    {
        // In a real application, you would inject repositories or DbContext
        private static readonly List<Product> _products = new()
        {
            new() { Id = 1, Name = "Sample Product 1", Description = "A sample product", Price = 19.99m, CreatedAt = DateTime.UtcNow.AddDays(-5), UpdatedAt = DateTime.UtcNow.AddDays(-5) },
            new() { Id = 2, Name = "Sample Product 2", Description = "Another sample product", Price = 29.99m, CreatedAt = DateTime.UtcNow.AddDays(-3), UpdatedAt = DateTime.UtcNow.AddDays(-3) },
            new() { Id = 3, Name = "Sample Product 3", Description = "Yet another sample product", Price = 39.99m, CreatedAt = DateTime.UtcNow.AddDays(-1), UpdatedAt = DateTime.UtcNow.AddDays(-1) }
        };

        private static readonly Dictionary<string, Cart> _carts = new();

        public async Task<ErrorOr<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Simulate async database operations
            await Task.Delay(10, cancellationToken);

            // Find the product (depends on Products feature)
            var product = _products.FirstOrDefault(p => p.Id == request.ProductId);
            if (product is null)
            {
                return Errors.Product.NotFound;
            }

            // Get or create cart for user
            if (!_carts.TryGetValue(request.UserId, out var cart))
            {
                cart = new Cart
                {
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _carts[request.UserId] = cart;
            }

            // Check if item already exists in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existingItem is not null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = request.Quantity
                });
            }

            cart.UpdatedAt = DateTime.UtcNow;

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

public class AddItemToCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/cart/{userId}/items", async (string userId, AddItemToCartRequest request, ISender sender) =>
        {
            var command = new AddItemToCart.Command
            {
                UserId = userId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            var result = await sender.Send(command);
            return result.ToHttpResult();
        })
        .WithName("AddItemToCart")
        .WithTags("Shopping Cart")
        .WithOpenApi();
    }

    public record AddItemToCartRequest(int ProductId, int Quantity);
}