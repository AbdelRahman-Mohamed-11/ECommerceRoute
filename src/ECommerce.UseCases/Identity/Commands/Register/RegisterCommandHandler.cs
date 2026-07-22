using System.Security.Cryptography;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using ECommerce.UseCases.Identity.Dtos;
using ECommerce.UseCases.Messaging;
using Microsoft.Extensions.Options;

namespace ECommerce.UseCases.Identity.Commands.Register;

public sealed class RegisterCommandHandler
    : IRequestHandler<RegisterCommand, Result<EmailSentResponse>>
{
    public const string RegisteredMessage =
        "Registration successful. A verification code was sent to your email. Confirm your email before logging in.";

    public const string UnconfirmedResendMessage =
        "This email is registered but not confirmed. A new verification code was sent to your email.";

    private readonly IIdentityService _identityService;
    private readonly IEmailVerificationCodeStore _verificationCodeStore;
    private readonly IEmailSender _emailSender;
    private readonly EmailVerificationSettings _settings;

    public RegisterCommandHandler(
        IIdentityService identityService,
        IEmailVerificationCodeStore verificationCodeStore,
        IEmailSender emailSender,
        IOptions<EmailVerificationSettings> settings)
    {
        _identityService = identityService;
        _verificationCodeStore = verificationCodeStore;
        _emailSender = emailSender;
        _settings = settings.Value;
    }

    public async Task<Result<EmailSentResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();

        var existing = await _identityService.GetUserByEmailAsync(email, cancellationToken);
        if (existing.IsSuccess)
        {
            if (await _identityService.IsEmailConfirmedAsync(email, cancellationToken))
                return Result<EmailSentResponse>.Failure(IdentityErrors.EmailAlreadyExists);

            return await SendVerificationAsync(
                email,
                verificationCodeResent: true,
                UnconfirmedResendMessage,
                cancellationToken);
        }

        var createResult = await _identityService.CreateUserAsync(
            email,
            request.Password,
            request.DisplayName,
            cancellationToken);

        if (createResult.IsFailure)
            return Result<EmailSentResponse>.Failure(createResult.Error);

        return await SendVerificationAsync(
            email,
            verificationCodeResent: false,
            RegisteredMessage,
            cancellationToken);
    }

    private async Task<Result<EmailSentResponse>> SendVerificationAsync(
        string email,
        bool verificationCodeResent,
        string message,
        CancellationToken cancellationToken)
    {
        var code = GenerateCode(_settings.CodeLength);

        await _verificationCodeStore.SaveAsync(email, code, cancellationToken);

        var sendResult = await _emailSender.SendAsync(
            email,
            "Confirm your ECommerce account",
            $"Your verification code is: {code}\n\nThis code expires in {_settings.ExpirationMinutes} minutes.\n\nYou cannot sign in until you confirm this email.",
            cancellationToken);

        if (sendResult.IsFailure)
            return Result<EmailSentResponse>.Failure(sendResult.Error);

        return Result<EmailSentResponse>.Success(
            new EmailSentResponse(email, verificationCodeResent, message));
    }

    private static string GenerateCode(int length)
    {
        var max = (int)Math.Pow(10, length);
        return RandomNumberGenerator.GetInt32(0, max).ToString($"D{length}");
    }
}
