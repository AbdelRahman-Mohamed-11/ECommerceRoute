using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public sealed class DeliveryMethodSeeder(ApplicationDbContext dbContext) : IDataSeeder
{
    public int Order => 3;

    public async Task SeedAsync(CancellationToken ct = default)
    {
        if (await dbContext.DeliveryMethods.AnyAsync(ct))
            return;

        var methods = new[]
        {
            DeliveryMethod.Create(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Standard Delivery",
                5.00m,
                "3-5 business days",
                description: "Affordable ground shipping",
                displayOrder: 1).Value,

            DeliveryMethod.Create(
                Guid.Parse("22222222-2222-2222-2222-222222222222"),
                "Express Delivery",
                15.00m,
                "1-2 business days",
                description: "Fast priority shipping",
                displayOrder: 2).Value,

            DeliveryMethod.Create(
                Guid.Parse("33333333-3333-3333-3333-333333333333"),
                "Same Day Delivery",
                30.00m,
                "Same day",
                description: "Order before noon for same-day delivery",
                displayOrder: 3).Value
        };

        await dbContext.DeliveryMethods.AddRangeAsync(methods, ct);
    }
}
