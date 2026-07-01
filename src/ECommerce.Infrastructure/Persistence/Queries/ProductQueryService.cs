using ECommerce.Infrastructure.Persistence.DbContexts;
using ECommerce.UseCases.Products;
using ECommerce.UseCases.Products.Dtos;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Queries;

public class ProductQueryService(StoreDbContext dbContext) : IProductQueryService
{
    public async Task<IReadOnlyList<GetAllProductsResponse>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .ProjectToType<GetAllProductsResponse>()
            .ToListAsync(cancellationToken);
    }

    public async Task<GetByIdProductResponse?> GetByIdProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
           .AsNoTracking()
           .Where(product => product.Id == id)
           .ProjectToType<GetByIdProductResponse>()
           .FirstOrDefaultAsync(cancellationToken);
    }
}
