using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ECommerce.Infrastructure.Persistence.Seeding;

public static class JsonSeeder
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static async Task SeedIfEmpty<TEntity, TModel>(
            DbSet<TEntity> dbSet,
            string fileName,
            Func<TModel, TEntity> map,
            CancellationToken ct = default
        ) where TEntity : BaseEntity
    {
        if (await dbSet.AnyAsync(ct))
            return;

        var filePath = Path.Combine(AppContext.BaseDirectory, "Persistence", "Seeding", "Data", fileName);

        if(!File.Exists(filePath)) return;

        await using var stream = File.OpenRead(filePath);

        // 1. 
        var models = await JsonSerializer.DeserializeAsync<List<TModel>>(stream, options, ct);

        if (models is null || models.Count == 0) return;


        await dbSet.AddRangeAsync(models.Select(map), ct);
    }
}

