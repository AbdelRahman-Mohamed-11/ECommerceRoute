using ECommerce.Domain.Shared;
using ECommerce.UseCases.Messaging;
using Microsoft.AspNetCore.Http;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    IFormFile Image,
    Guid ProductBrandId,
    Guid ProductTypeId
) : ICommand<Result<Guid>>;