using Microsoft.AspNetCore.Http;

namespace ECommerce.UseCases.Common.Interfaces;

public interface IPhotoService
{
    Task<string> UploadPhotoAsync(IFormFile file);
}