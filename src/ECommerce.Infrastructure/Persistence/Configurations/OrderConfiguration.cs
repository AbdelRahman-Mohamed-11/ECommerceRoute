using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        BaseEntityConfiguration.Configure(builder);

        builder.ToTable("Orders");

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.DeliveryMethodName)
            .IsRequired()
            .HasMaxLength(Order.MaxDeliveryMethodNameLength);

        builder.Property(x => x.DeliveryMethodPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.DeliveryMethodEstimatedTime)
            .IsRequired()
            .HasMaxLength(Order.MaxDeliveryTimeLength);

        builder.Property(x => x.ShippingRecipientFirstName)
            .IsRequired()
            .HasMaxLength(Order.MaxNameLength);

        builder.Property(x => x.ShippingRecipientLastName)
            .IsRequired()
            .HasMaxLength(Order.MaxNameLength);

        builder.Property(x => x.ShippingPhoneNumber)
            .IsRequired()
            .HasMaxLength(Order.MaxPhoneLength);

        builder.Property(x => x.ShippingCountry)
            .IsRequired()
            .HasMaxLength(Order.MaxCountryLength);

        builder.Property(x => x.ShippingCity)
            .IsRequired()
            .HasMaxLength(Order.MaxCityLength);

        builder.Property(x => x.ShippingStreet)
            .IsRequired()
            .HasMaxLength(Order.MaxStreetLength);

        builder.Property(x => x.ShippingPostalCode)
            .IsRequired()
            .HasMaxLength(Order.MaxPostalCodeLength);

        builder.Property(x => x.SubTotal).HasPrecision(18, 2);
        builder.Property(x => x.ShippingCost).HasPrecision(18, 2);
        builder.Property(x => x.Total).HasPrecision(18, 2);

        builder.Property(x => x.PaymentIntentId)
            .HasMaxLength(200);

        builder.HasIndex(x => x.PaymentIntentId);

        builder.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Items)
            .HasField("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CreatedAt);
    }
}
