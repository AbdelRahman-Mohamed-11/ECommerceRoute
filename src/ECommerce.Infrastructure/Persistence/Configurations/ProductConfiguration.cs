using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        BaseEntityConfiguration.Configure(builder);

        builder.Property(x => x.Name)
          .IsRequired()
          .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.PictureUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.ProductBrand)
            .WithMany(pb => pb.Products)
            .HasForeignKey(x => x.ProductBrandId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.ProductType)
            .WithMany(pt => pt.Products)
            .HasForeignKey(x => x.ProductTypeId)
            .OnDelete(DeleteBehavior.NoAction);


        builder.HasIndex(x => x.Name);
        builder.HasIndex(x => x.Price);
    }
}
