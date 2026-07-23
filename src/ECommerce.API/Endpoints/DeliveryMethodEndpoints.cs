using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.Domain.Constants;
using ECommerce.UseCases.DeliveryMethods.Commands.CreateDeliveryMethod;
using ECommerce.UseCases.DeliveryMethods.Commands.DeleteDeliveryMethod;
using ECommerce.UseCases.DeliveryMethods.Commands.UpdateDeliveryMethod;
using ECommerce.UseCases.DeliveryMethods.Dtos;
using ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethodById;
using ECommerce.UseCases.DeliveryMethods.Queries.GetDeliveryMethods;
using ECommerce.UseCases.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class DeliveryMethodEndpoints
{
    public static IEndpointRouteBuilder MapDeliveryMethodEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/delivery-methods")
            .WithTags("DeliveryMethods")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .AddEndpointFilter<AuditEndpointFilter>();

        group.MapGet("/", async (
            [FromQuery] bool? availableOnly,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new GetDeliveryMethodsQuery(availableOnly ?? true),
                ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodsRetrieved);
        })
        .AllowAnonymous()
        .WithSummary("List delivery methods")
        .WithDescription("Returns delivery methods. Optionally filter to available ones only.")
        .Produces<ApiResponse<IReadOnlyList<DeliveryMethodResponse>>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new GetDeliveryMethodByIdQuery(id), ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodRetrieved);
        })
        .AllowAnonymous()
        .WithSummary("Get delivery method by id")
        .Produces<ApiResponse<DeliveryMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", async (
            [FromBody] CreateDeliveryMethodCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodCreated);
        })
        .RequireAuthorization(policy => policy.RequireRole(Roles.Admin, Roles.SuperAdmin))
        .WithSummary("Create delivery method")
        .Produces<ApiResponse<DeliveryMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapPut("/{id:guid}", async (
            Guid id,
            [FromBody] UpdateDeliveryMethodRequest request,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var command = new UpdateDeliveryMethodCommand(
                id,
                request.Name,
                request.Price,
                request.EstimatedDeliveryTime,
                request.Description,
                request.IsAvailable,
                request.DisplayOrder);

            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodUpdated);
        })
        .RequireAuthorization(policy => policy.RequireRole(Roles.Admin, Roles.SuperAdmin))
        .WithSummary("Update delivery method")
        .Produces<ApiResponse<DeliveryMethodResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", async (
            Guid id,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteDeliveryMethodCommand(id), ct);
            return result.FromResult(httpContext, ApiMessages.DeliveryMethodDeleted);
        })
        .RequireAuthorization(policy => policy.RequireRole(Roles.Admin, Roles.SuperAdmin))
        .WithSummary("Delete delivery method")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        return endpoints;
    }
}

public sealed record UpdateDeliveryMethodRequest(
    string Name,
    decimal Price,
    string EstimatedDeliveryTime,
    string? Description = null,
    bool IsAvailable = true,
    int DisplayOrder = 0);
