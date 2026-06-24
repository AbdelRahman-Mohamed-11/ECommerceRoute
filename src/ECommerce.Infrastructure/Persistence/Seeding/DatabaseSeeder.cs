using ECommerce.Domain.Repositories;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public sealed class DatabaseSeeder(
    IUnitOfWork unitOfWork,
    IEnumerable<IDataSeeder> seeders)
{
    public async Task SeedAll(CancellationToken ct = default)
    {
        foreach (var seeder in seeders.OrderBy(s => s.Order))
        {
            await seeder.SeedAsync(ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}
