using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Dtos;

namespace ECommerce.UseCases.Products.Queries.GetByIdProduct;

public sealed record GetByIdProductQuery(Guid Id) : IQuery<Result<GetByIdProductResponse>>;