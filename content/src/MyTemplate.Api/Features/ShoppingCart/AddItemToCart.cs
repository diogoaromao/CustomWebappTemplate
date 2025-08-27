using Carter;
using ErrorOr;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Api.Common.Errors;
using MyTemplate.Api.Common.Models;
using MyTemplate.Api.Common.Persistence;
using MyTemplate.Api.Features.Products;

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
        private readonly ISender _sender;
        private readonly ApplicationDbContext _context;

        public Handler(ISender sender, ApplicationDbContext context)
        {
            _sender = sender;
            _context = context;
        }

        public async Task<ErrorOr<Response>> Handle(Command request, CancellationToken cancellationToken)
        {
            // Get the product using MediatR - proper separation of concerns
            var getProductQuery = new GetProductById.Query { Id = request.ProductId };
            var productResult = await _sender.Send(getProductQuery, cancellationToken);
            
            if (productResult.IsError)
            {
                return productResult.FirstError;
            }
            
            var product = new Product
            {
                Id = productResult.Value.Id,
                Name = productResult.Value.Name,
                Description = productResult.Value.Description,
                Price = productResult.Value.Price
            };
            
            return await AddProductToCart(request, product, cancellationToken);
        }

        private async Task<ErrorOr<Response>> AddProductToCart(Command request, Product product, CancellationToken cancellationToken)
        {
            // Get or create cart for user
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

            if (cart is null)
            {
                cart = new Cart
                {
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.Carts.Add(cart);
            }

            // Check if item already exists in cart
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (existingItem is not null)
            {
                existingItem.Quantity += request.Quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = request.UserId,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = request.Quantity
                };
                cart.Items.Add(cartItem);
            }

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
            return result.Match(
                response => Results.Ok(response),
                error => Results.BadRequest(error)
            );
        })
        .WithName("AddItemToCart")
        .WithTags("Shopping Cart")
        .WithOpenApi();
    }

    public record AddItemToCartRequest(int ProductId, int Quantity);
}