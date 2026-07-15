using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Basket.Dtos;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Basket.Commands;

public sealed record AddBasketItemCommand(Guid BuyerId, Guid ProductId, int Quantity)
    : ICommand<Result<GetBasketResponse>>;

public sealed class AddBasketItemCommandHandler(
    IBasketStore basketStore,
    IReadRepository<Product> productRepository)
    : IRequestHandler<AddBasketItemCommand, Result<GetBasketResponse>>
{
    public async Task<Result<GetBasketResponse>> Handle(
        AddBasketItemCommand request,
        CancellationToken cancellationToken)
    {
        var product = await productRepository.FirstOrDefaultAsync(
            new ProductForBasketSpecification(request.ProductId),
            cancellationToken);

        if (product is null)
            return Result<GetBasketResponse>.Failure(ProductErrors.NotFound);

        var basket = await basketStore.GetOrCreateAsync(request.BuyerId, cancellationToken);

        var addResult = basket.AddItem(
            product.Id,
            product.Name,
            product.PictureUrl,
            product.Price,
            request.Quantity);

        if (addResult.IsFailure)
            return Result<GetBasketResponse>.Failure(addResult.Error);

        // to save in redis
        await basketStore.SaveAsync(basket, cancellationToken);
        return Result<GetBasketResponse>.Success(GetBasketResponse.From(basket));
    }
}
