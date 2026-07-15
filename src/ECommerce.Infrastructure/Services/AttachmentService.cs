using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce.Domain.Errors;
using ECommerce.Domain.Shared;
using ECommerce.UseCases.Common.Interfaces;
using ECommerce.UseCases.Common.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Services;

public class AttachmentService : IAttachmentService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<AttachmentService> _logger;

    public AttachmentService(
        IOptions<CloudinarySettings> config,
        ILogger<AttachmentService> logger)
    {
        _logger = logger;

        var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret);

        _cloudinary = new Cloudinary(account);
    }

    public async Task<Result<string>> UploadAttachmentAsync(IFormFile file)
    {
        if (file.Length <= 0)
        {
            _logger.LogWarning("Upload failed: file is empty");
            return Result<string>.Failure(AttachmentErrors.EmptyFile);
        }

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "products",
            Transformation = new Transformation()
                .Width(500)
                .Height(500)
                .Crop("fill")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        if (result.Error is not null)
        {
            _logger.LogError(
                "Cloudinary upload failed: {Error}",
                result.Error.Message);

            return Result<string>.Failure(AttachmentErrors.UploadFailed);
        }

        _logger.LogInformation(
            "Image uploaded successfully: {Url}",
            result.SecureUrl);

        return Result<string>.Success(result.SecureUrl.ToString());
    }
}
