using FluentValidation;

namespace ECommerce.UseCases.Products.Queries.GetPagedProducts;

public sealed class GetPagedProductsQueryValidator : AbstractValidator<GetPagedProductsQuery>
{
    public GetPagedProductsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithErrorCode("Products.PageNumber.Invalid")
            .WithMessage("Page Number must be at least 1");

        RuleFor(query => query.PageSize)
           .InclusiveBetween(1, 1000)
           .WithErrorCode("Products.PageSize.Invalid")
           .WithMessage("Page Size must be between 1 and 1000");
    }
}