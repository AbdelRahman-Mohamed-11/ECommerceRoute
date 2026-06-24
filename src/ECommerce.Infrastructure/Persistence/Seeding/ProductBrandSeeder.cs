using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.Infrastructure.Persistence.Seeding.Data.Models;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public class ProductBrandSeeder(StoreDbContext dbContext) : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAsync(CancellationToken ct = default)
            => await JsonSeeder.SeedIfEmpty<ProductBrand, ProductBrandSeedModel>
                (dbContext.Brands, "brands.json", b => ProductBrand.Create(b.Id, b.Name), ct);
}
