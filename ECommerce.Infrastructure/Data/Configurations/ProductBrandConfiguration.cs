using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Data.Configurations
{
    public sealed class ProductBrandConfiguration
    : IEntityTypeConfiguration<ProductBrand>
    {
        public void Configure(
            EntityTypeBuilder<ProductBrand> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(x => x.Name)
                .IsUnique();
        }
    }
}
