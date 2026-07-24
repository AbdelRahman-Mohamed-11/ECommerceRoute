using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Dtos;

namespace ECommerce.UseCases.Products.Queries.GetAllProducts;

public sealed record GetAllProductsQuery : IQuery<Result<IReadOnlyList<GetAllProductsResponse>>>;