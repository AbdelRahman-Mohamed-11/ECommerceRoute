using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations
{
    public sealed class ProductBrandConfiguration
    : IEntityTypeConfiguration<ProductBrand>
    {
        public void Configure(
            EntityTypeBuilder<ProductBrand> builder)
        {
            BaseEntityConfiguration.Configure(builder);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
