using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

internal static class BaseEntityConfiguration
{
    public static void Configure<TEntity>(EntityTypeBuilder<TEntity> builder)
        where TEntity : BaseEntity
    {
        builder.Property(entity => entity.IsDeleted)
            .HasDefaultValue(false);

        builder.HasQueryFilter(entity => !entity.IsDeleted);
    }
}
