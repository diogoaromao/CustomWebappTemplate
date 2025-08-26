using ErrorOr;

namespace MyTemplate.Api.Common.Errors;

public static partial class Errors
{
    public static class Product
    {
        public static Error NotFound => Error.NotFound(
            code: "Product.NotFound",
            description: "Product was not found.");

        public static Error InvalidName => Error.Validation(
            code: "Product.InvalidName",
            description: "Product name cannot be empty and must be less than 100 characters.");

        public static Error InvalidPrice => Error.Validation(
            code: "Product.InvalidPrice",
            description: "Product price must be greater than zero.");

        public static Error InvalidDescription => Error.Validation(
            code: "Product.InvalidDescription",
            description: "Product description must be less than 500 characters.");
    }
}