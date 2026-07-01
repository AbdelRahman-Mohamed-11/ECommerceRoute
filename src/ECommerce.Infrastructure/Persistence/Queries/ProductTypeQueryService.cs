using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.UseCases.Types;
using ECommerce.UseCases.Types.Dtos;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Queries;

public class ProductTypeQueryService(StoreDbContext dbContext) : IProductTypeQueryService
{
    public async Task<IReadOnlyList<GetAllTypesResponse>> GetAllTypesAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Types
            .AsNoTracking()
            .ProjectToType<GetAllTypesResponse>()
            .ToListAsync(cancellationToken);
    }
}
