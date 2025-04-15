using ASPWebApp.Models;

namespace ASPWebApp.Services
{
    public interface IImageService
    {
        Task<ImageResponse> SaveImageAsync(IFormFile file);
        Task<byte[]> GetImageAsync(string fileName);
        Task<byte[]> ApplyGrayscaleAsync(string fileName);
        Task<byte[]> ResizeImageAsync(string fileName, int width, int height);
        Task<byte[]> CropImageAsync(string fileName, int x, int y, int width, int height);
        Task<byte[]> MultiModificationsImageAsync(IFormFile file, int width, int height, int x, int y, int cropWidth, int cropHeight);
        Task<string> TestOpenCVAsync();
        Task DeleteImageAsync(string fileName);
    }
}
