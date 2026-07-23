using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.ItemOrdered, snapshot =>
        {
            snapshot.Property(p => p.ProductId)
                .HasColumnName("ProductId")
                .IsRequired();

            snapshot.Property(p => p.ProductName)
                .HasColumnName("ProductName")
                .IsRequired()
                .HasMaxLength(ProductItemOrdered.MaxProductNameLength);

            snapshot.Property(p => p.PictureUrl)
                .HasColumnName("PictureUrl")
                .IsRequired()
                .HasMaxLength(ProductItemOrdered.MaxPictureUrlLength);

            snapshot.Property(p => p.UnitPrice)
                .HasColumnName("UnitPrice")
                .HasPrecision(18, 2);

            snapshot.HasIndex(p => p.ProductId);
        });

        builder.Navigation(x => x.ItemOrdered).IsRequired();

        builder.HasIndex(x => x.OrderId);

        builder.Ignore(x => x.LineTotal);
    }
}
