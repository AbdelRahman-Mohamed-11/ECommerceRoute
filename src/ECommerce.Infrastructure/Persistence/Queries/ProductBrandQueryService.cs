using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.UseCases.Brands;
using ECommerce.UseCases.Brands.Dtos;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Queries;

public class ProductBrandQueryService(StoreDbContext dbContext) : IProductBrandQueryService
{
    public async Task<IReadOnlyList<GetAllBrandsResponse>> GetAllBrandsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Brands
            .AsNoTracking()
            .ProjectToType<GetAllBrandsResponse>()
            .ToListAsync(cancellationToken);
    }
}
