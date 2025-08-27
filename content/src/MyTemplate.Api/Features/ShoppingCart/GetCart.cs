using Carter;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MyTemplate.Api.Common.Errors;
using MyTemplate.Api.Common.Models;
using MyTemplate.Api.Common.Persistence;

namespace MyTemplate.Api.Features.ShoppingCart;

public static class GetCart
{
    public class Query : IRequest<ErrorOr<Response>>
    {
        public string UserId { get; set; } = string.Empty;
    }

    public class Response
    {
        public string UserId { get; set; } = string.Empty;
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
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
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == request.UserId, cancellationToken);

            if (cart is null)
            {
                return Errors.Cart.NotFound;
            }

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
                TotalItems = cart.TotalItems,
                CreatedAt = cart.CreatedAt,
                UpdatedAt = cart.UpdatedAt
            };

            return response;
        }
    }
}

public class GetCartEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/cart/{userId}", async (string userId, ISender sender) =>
        {
            var query = new GetCart.Query { UserId = userId };
            var result = await sender.Send(query);
            return result.Match(
                response => Results.Ok(response),
                error => Results.NotFound(error)
            );
        })
        .WithName("GetCart")
        .WithTags("Shopping Cart")
        .WithOpenApi();
    }
}