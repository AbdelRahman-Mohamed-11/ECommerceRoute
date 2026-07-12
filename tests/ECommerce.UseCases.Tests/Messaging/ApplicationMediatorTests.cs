using ECommerce.Domain.Entities;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Repositories;
using ECommerce.Domain.Shared;
using ECommerce.Domain.Specifications;
using ECommerce.UseCases.Messaging;
using ECommerce.UseCases.Products.Dtos;
using ECommerce.UseCases.Products.Queries;
using ECommerce.UseCases.Products.Queries.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace ECommerce.UseCases.Tests.Messaging;

public sealed class ApplicationMediatorTests
{
    [Fact]
    public async Task Send_GetAllProductsQuery_returns_products_from_repository()
    {
        var products = new List<GetAllProductsResponse>
        {
            new(
                Guid.Parse("11111111-1111-1111-1111-111111111111"),
                "Test Product",
                "Description",
                19.99m,
                "https://example.com/p.jpg",
                "Type",
                "Brand")
        };

        var sender = CreateSender(
            new GetAllProductsQueryHandler(new FakeProductReadRepository(listProducts: products)));

        var result = await sender.Send(new GetAllProductsQuery());

        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(1);
        result.Value[0].Name.ShouldBe("Test Product");
    }

    [Fact]
    public async Task Send_GetByIdProductQuery_returns_not_found_when_product_missing()
    {
        var sender = CreateSender(
            new GetByIdProductQueryHandler(new FakeProductReadRepository(firstOrDefaultProduct: null)));

        var result = await sender.Send(
            new GetByIdProductQuery(Guid.Parse("22222222-2222-2222-2222-222222222222")));

        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(ProductErrors.NotFound);
    }

    [Fact]
    public async Task Send_GetByIdProductQuery_returns_product_when_found()
    {
        var product = new GetByIdProductResponse(
            Guid.Parse("33333333-3333-3333-3333-333333333333"),
            "Found Product",
            "Description",
            9.99m,
            "https://example.com/found.jpg",
            "Type",
            "Brand");

        var sender = CreateSender(
            new GetByIdProductQueryHandler(new FakeProductReadRepository(firstOrDefaultProduct: product)));

        var result = await sender.Send(new GetByIdProductQuery(product.Id));

        result.IsSuccess.ShouldBeTrue();
        result.Value.Name.ShouldBe("Found Product");
    }

    [Fact]
    public async Task AddMessaging_registers_real_application_handlers()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IReadRepository<Product>>(new FakeProductReadRepository());
        services.AddSingleton<IReadRepository<ProductBrand>>(new FakeBrandReadRepository());
        services.AddSingleton<IReadRepository<ProductType>>(new FakeTypeReadRepository());
        services.AddMessaging(typeof(GetAllProductsQueryHandler).Assembly);

        var sender = services.BuildServiceProvider().GetRequiredService<ISender>();
        var result = await sender.Send(new GetAllProductsQuery());

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();
    }

    private static ISender CreateSender<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> handler)
        where TRequest : IRequest<TResponse>
    {
        var services = new ServiceCollection();
        services.AddSingleton(handler);
        services.AddScoped<IRequestHandlerExecutor, RequestHandlerExecutor<TRequest, TResponse>>();
        services.AddScoped<HandlerExecutorRegistry>();
        services.AddScoped<ISender, Sender>();

        return services.BuildServiceProvider().GetRequiredService<ISender>();
    }

    private sealed class FakeProductReadRepository : IReadRepository<Product>
    {
        private readonly IReadOnlyList<GetAllProductsResponse>? _listProducts;
        private readonly GetByIdProductResponse? _firstOrDefaultProduct;

        public FakeProductReadRepository(
            IReadOnlyList<GetAllProductsResponse>? listProducts = null,
            GetByIdProductResponse? firstOrDefaultProduct = null)
        {
            _listProducts = listProducts;
            _firstOrDefaultProduct = firstOrDefaultProduct;
        }

        public Task<Product?> FirstOrDefaultAsync(ISpecification<Product> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<TResult?> FirstOrDefaultAsync<TResult>(
            ISpecification<Product, TResult> specification,
            CancellationToken ct = default)
        {
            if (_firstOrDefaultProduct is TResult typed)
                return Task.FromResult<TResult?>(typed);

            return Task.FromResult<TResult?>(default);
        }

        public Task<Product> SingleAsync(ISpecification<Product> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<TResult> SingleAsync<TResult>(
            ISpecification<Product, TResult> specification,
            CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<Product>> ListAsync(ISpecification<Product> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<TResult>> ListAsync<TResult>(
            ISpecification<Product, TResult> specification,
            CancellationToken ct = default)
        {
            if (_listProducts is IReadOnlyList<TResult> typed)
                return Task.FromResult(typed);

            return Task.FromResult<IReadOnlyList<TResult>>(Array.Empty<TResult>());
        }

        public Task<int> CountAsync(ISpecification<Product> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<bool> AnyAsync(ISpecification<Product> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<PagedResult<Product>> PagedListAsync(ISpecification<Product> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();
    }

    private sealed class FakeBrandReadRepository : IReadRepository<ProductBrand>
    {
        public Task<IReadOnlyList<TResult>> ListAsync<TResult>(
            ISpecification<ProductBrand, TResult> specification,
            CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<TResult>>(Array.Empty<TResult>());

        public Task<ProductBrand?> FirstOrDefaultAsync(ISpecification<ProductBrand> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<ProductBrand, TResult> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<ProductBrand> SingleAsync(ISpecification<ProductBrand> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<TResult> SingleAsync<TResult>(ISpecification<ProductBrand, TResult> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<ProductBrand>> ListAsync(ISpecification<ProductBrand> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<int> CountAsync(ISpecification<ProductBrand> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<bool> AnyAsync(ISpecification<ProductBrand> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<PagedResult<ProductBrand>> PagedListAsync(ISpecification<ProductBrand> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();
    }

    private sealed class FakeTypeReadRepository : IReadRepository<ProductType>
    {
        public Task<IReadOnlyList<TResult>> ListAsync<TResult>(
            ISpecification<ProductType, TResult> specification,
            CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<TResult>>(Array.Empty<TResult>());

        public Task<ProductType?> FirstOrDefaultAsync(ISpecification<ProductType> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<ProductType, TResult> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<ProductType> SingleAsync(ISpecification<ProductType> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<TResult> SingleAsync<TResult>(ISpecification<ProductType, TResult> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<ProductType>> ListAsync(ISpecification<ProductType> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<int> CountAsync(ISpecification<ProductType> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<bool> AnyAsync(ISpecification<ProductType> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();

        public Task<PagedResult<ProductType>> PagedListAsync(ISpecification<ProductType> specification, CancellationToken ct = default) =>
            throw new NotSupportedException();
    }
}
