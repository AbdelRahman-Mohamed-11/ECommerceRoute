using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.Infrastructure.Persistence.Seeding.Data.Models;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public class ProductTypeSeeder(StoreDbContext dbContext) : IDataSeeder
{
    public int Order => 2;

    public async Task SeedAsync(CancellationToken ct = default)
            => await JsonSeeder.SeedIfEmpty<ProductType, ProductTypeSeedModel>
                (dbContext.Types, "types.json", b => ProductType.Create(b.Id, b.Name).Value, ct);
}