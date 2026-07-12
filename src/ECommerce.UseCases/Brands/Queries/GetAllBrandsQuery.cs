using ECommerce.Domain.Shared;
using ECommerce.UseCases.Brands.Dtos;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Brands.Queries;

public sealed record GetAllBrandsQuery : IQuery<Result<IReadOnlyList<GetAllBrandsResponse>>>;
