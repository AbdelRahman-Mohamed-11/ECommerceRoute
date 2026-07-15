using ECommerce.Domain.Entities;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging;

namespace ECommerce.UseCases.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPhotoService _photoService;
    private readonly IRepository<Product> _productRepository;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        IPhotoService photoService,
        IRepository<Product> productRepository)
    {
        _unitOfWork = unitOfWork;
        _photoService = photoService;
        _productRepository = productRepository;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var pictureUrl = await _photoService
            .UploadPhotoAsync(request.Image);

        var id = Guid.NewGuid();

        var productResult = Product.Create(
            id,
            request.Name,
            request.Description,
            pictureUrl,
            request.Price,
            request.ProductBrandId,
            request.ProductTypeId);

        if (productResult.IsFailure)
            return Result<Guid>.Failure(productResult.Error);

       _productRepository.Add(
            productResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(id);
    }
}