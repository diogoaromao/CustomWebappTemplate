using ErrorOr;

namespace MyTemplate.Api.Common.Errors;

public static partial class Errors
{
    public static class Cart
    {
        public static Error NotFound => Error.NotFound(
            code: "Cart.NotFound", 
            description: "Cart was not found.");

        public static Error ItemNotFound => Error.NotFound(
            code: "Cart.ItemNotFound", 
            description: "Item not found in cart.");

        public static Error InvalidQuantity => Error.Validation(
            code: "Cart.InvalidQuantity", 
            description: "Quantity must be greater than zero.");

        public static Error ProductNotFound => Error.NotFound(
            code: "Cart.ProductNotFound", 
            description: "Product not found. Cannot add to cart.");

        public static Error EmptyUserId => Error.Validation(
            code: "Cart.EmptyUserId", 
            description: "User ID cannot be empty.");
    }
}