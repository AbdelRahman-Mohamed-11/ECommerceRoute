using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public sealed class UserAddressConfiguration : IEntityTypeConfiguration<UserAddress>
{
    public void Configure(EntityTypeBuilder<UserAddress> builder)
    {
        builder.ToTable("UserAddresses");

        BaseEntityConfiguration.Configure(builder);


        builder.Property(x => x.RecipientFirstName)
            .HasMaxLength(UserAddress.MaxNameLength)
            .IsRequired();

        builder.Property(x => x.RecipientLastName)
            .HasMaxLength(UserAddress.MaxNameLength)
            .IsRequired();

        builder.Property(x => x.PhoneNumber)
            .HasMaxLength(UserAddress.MaxPhoneLength)
            .IsRequired();

        builder.Property(x => x.Country)
            .HasMaxLength(UserAddress.MaxCountryLength)
            .IsRequired();

        builder.Property(x => x.City)
            .HasMaxLength(UserAddress.MaxCityLength)
            .IsRequired();

        builder.Property(x => x.Street)
            .HasMaxLength(UserAddress.MaxStreetLength)
            .IsRequired();

        builder.Property(x => x.PostalCode)
            .HasMaxLength(UserAddress.MaxPostalCodeLength)
            .IsRequired();

        builder.HasIndex(x => x.UserId);
    }
}