using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyTemplate.Api.Common.Models;

namespace MyTemplate.Api.Common.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(ci => new { ci.UserId, ci.ProductId });

        builder.Property(ci => ci.UserId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(ci => ci.ProductId)
            .IsRequired();

        builder.Property(ci => ci.ProductName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(ci => ci.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(ci => ci.Quantity)
            .IsRequired();

        builder.Ignore(ci => ci.TotalPrice);
    }
}