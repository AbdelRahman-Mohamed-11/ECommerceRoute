using Asp.Versioning;
using Asp.Versioning.Builder;
using ECommerce.API.Extensions;
using ECommerce.API.Filters;
using ECommerce.API.Models;
using ECommerce.UseCases.Identity.Commands.ConfirmEmail;
using ECommerce.UseCases.Identity.Commands.Login;
using ECommerce.UseCases.Identity.Commands.Logout;
using ECommerce.UseCases.Identity.Commands.RefreshToken;
using ECommerce.UseCases.Identity.Commands.Register;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(
        this IEndpointRouteBuilder endpoints,
        ApiVersionSet apiVersionSet)
    {
        var group = endpoints
            .MapGroup("/api/v{version:apiVersion}/auth")
            .WithTags("Auth")
            .WithApiVersionSet(apiVersionSet)
            .HasApiVersion(new ApiVersion(1, 0))
            .AllowAnonymous()
            .AddEndpointFilter<AuditEndpointFilter>();

        group.MapPost("/register", async (
            [FromBody] RegisterCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            Domain.Shared.Result<EmailSentResponse>? result = await sender.Send(command, ct);
            if (result.IsFailure)
                return result.Problem(httpContext);

            return ResultExtensions.Success(result.Value, httpContext, result.Value.Message);
        })
        .WithSummary("Register a new user")
        .WithDescription("Creates an unconfirmed account and emails a verification code. Does not return a JWT.")
        .Produces<ApiResponse<EmailSentResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/confirm-email", async (
            [FromBody] ConfirmEmailCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.EmailConfirmed);
        })
        .WithSummary("Confirm email with verification code")
        .WithDescription("Validates the code and confirms the account. Does not return tokens — client should redirect to login.")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/login", async (
            [FromBody] LoginCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.LoggedIn);
        })
        .WithSummary("Login")
        .WithDescription("Returns access + refresh tokens for a confirmed account.")
        .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status403Forbidden);

        group.MapPost("/refresh", async (
            [FromBody] RefreshTokenCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.TokenRefreshed);
        })
        .WithSummary("Refresh tokens")
        .WithDescription("Exchanges a valid refresh token for a new access token and rotated refresh token.")
        .Produces<ApiResponse<AuthResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", async (
            [FromBody] LogoutCommand command,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var result = await sender.Send(command, ct);
            return result.FromResult(httpContext, ApiMessages.LoggedOut);
        })
        .WithSummary("Logout")
        .WithDescription("Revokes the provided refresh token.")
        .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized);

        return endpoints;
    }
}
