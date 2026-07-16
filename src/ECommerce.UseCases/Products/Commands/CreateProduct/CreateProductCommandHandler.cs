using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Specifications;

namespace ECommerce.UseCases.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAttachmentService _attachmentService;
    private readonly IRepository<Product> _productRepository;

    public CreateProductCommandHandler(
        IUnitOfWork unitOfWork,
        IAttachmentService attachmentService,
        IRepository<Product> productRepository)
    {
        _unitOfWork = unitOfWork;
        _attachmentService = attachmentService;
        _productRepository = productRepository;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var existingProduct = await _productRepository.AnyAsync(
            new ProductByNameSpecification(request.Name), cancellationToken);

        if (existingProduct)
            return Result<Guid>.Failure(ProductErrors.DuplicateName);

        var uploadResult = await _attachmentService
            .UploadAttachmentAsync(request.Image);

        if (uploadResult.IsFailure)
            return Result<Guid>.Failure(uploadResult.Error);

        var id = Guid.NewGuid();

        var productResult = Product.Create(
            id,
            request.Name,
            request.Description,
            uploadResult.Value,
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