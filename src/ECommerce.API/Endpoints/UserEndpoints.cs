using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.UseCases.Identity.Commands.AddUserAddress;
using ECommerce.UseCases.Identity.Commands.UpdateUserProfile;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Identity.Queries.GetCurrentUser;
using ECommerce.UseCases.Identity.Queries.GetUserAddresses;
using ECommerce.UseCases.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/users")
            .WithTags("Users")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .RequireAuthorization()
            .AddEndpointFilter<AuditEndpointFilter>();

        group.MapGet("/me", async (
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetCurrentUserQuery(), ct);
            return result.FromResult(httpContext, ApiMessages.CurrentUserRetrieved);
        })
        .WithSummary("Get current user")
        .WithDescription("Returns the authenticated user's profile.")
        .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPut("/me", async (
            [FromBody] UpdateUserProfileCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.UserProfileUpdated);
        })
        .WithSummary("Update current user profile")
        .Produces<ApiResponse<UserProfileResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/me/addresses", async (
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetUserAddressesQuery(), ct);
            return result.FromResult(httpContext, ApiMessages.UserAddressesRetrieved);
        })
        .WithSummary("Get current user addresses")
        .Produces<ApiResponse<IReadOnlyList<UserAddressResponse>>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/me/addresses", async (
            [FromBody] AddUserAddressCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.UserAddressAdded);
        })
        .WithSummary("Add address for current user")
        .Produces<ApiResponse<UserAddressResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status400BadRequest);

        return endpoints;
    }
}
