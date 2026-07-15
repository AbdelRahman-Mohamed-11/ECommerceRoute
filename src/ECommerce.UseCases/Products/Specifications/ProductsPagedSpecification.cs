using ECommerce.Domain.Entities;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Enums;
using ECommerce.UseCases.Specifications;

namespace ECommerce.UseCases.Products.Specifications;

public class ProductsPagedSpecification : Specification<Product, GetAllProductsResponse>
{
    public ProductsPagedSpecification(string? search = null, Guid? brandId = null, 
        Guid? typeId = null, ProductSortField? sortBy = null, bool sortDescending = false, int? pageNumber = null,
        int? pageSize = null)
    {
        var query = Query;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();

            query.Where(product => product.Name.Contains(term) ||
            product.Description.Contains(term));
        }

        if(brandId.HasValue)
            query.Where(product => product.ProductBrandId == brandId.Value);

        if (typeId.HasValue)
            query = query.Where(product => product.ProductTypeId == typeId.Value);

        if (sortBy is ProductSortField sortField)
            ApplySort(query, sortBy, sortDescending);

        if(pageNumber.HasValue && pageSize.HasValue)
        {
            var skip = (pageNumber - 1) * pageSize.Value;

            query
                .Skip(skip.Value)
                .Take(pageSize.Value)
                .Select(p => new GetAllProductsResponse(p.Id, 
                p.Name,p.Description, p.Price, p.PictureUrl, p.ProductType.Name, p.ProductBrand.Name));
        }
    }

    private static void ApplySort(
    ISpecificationBuilder<Product, GetAllProductsResponse> query,
    ProductSortField? sortBy,
    bool sortDescending)
    {
        switch (sortBy)
        {
            case ProductSortField.Name:
                if (sortDescending)
                {
                    query.OrderByDescending(x => x.Name);
                }
                else
                {
                    query.OrderBy(x => x.Name);
                }
                break;

            case ProductSortField.Price:
                if (sortDescending)
                {
                    query.OrderByDescending(x => x.Price)
                         .ThenBy(x => x.Name);
                }
                else
                {
                    query.OrderBy(x => x.Price)
                         .ThenBy(x => x.Name);
                }
                break;

            case ProductSortField.Brand:
                if (sortDescending)
                {
                    query.OrderByDescending(x => x.ProductBrand.Name)
                         .ThenBy(x => x.Name);
                }
                else
                {
                    query.OrderBy(x => x.ProductBrand.Name)
                         .ThenBy(x => x.Name);
                }
                break;

            case ProductSortField.Type:
                if (sortDescending)
                {
                    query.OrderByDescending(x => x.ProductType.Name)
                         .ThenBy(x => x.Name);
                }
                else
                {
                    query.OrderBy(x => x.ProductType.Name)
                         .ThenBy(x => x.Name);
                }
                break;

            default:
                query.OrderBy(x => x.Name);
                break;
        }
    }
}
