using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.DbContexts;

public class StoreDbContext(DbContextOptions<StoreDbContext> options) 
        : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductBrand> Brands => Set<ProductBrand>();
    public DbSet<ProductType> Types => Set<ProductType>();
    public DbSet<UserAddress> UserAddresses => Set<UserAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreDbContext).Assembly, 
            type => type.Namespace == "ECommerce.Infrastructure.Persistence.Configurations");
        
        
        base.OnModelCreating(modelBuilder);
    }
}
