using ASPWebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASPWebApp.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IImageService _imageService;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(IImageService imageService, ILogger<ImagesController> logger)
        {
            _imageService = imageService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Upload attempted with no file or empty file.");
                TempData["Message"] = "Image wasn't uploaded.";
                return RedirectToAction("Index");
            }

            var result = await _imageService.SaveImageAsync(file);
            TempData["Message"] = "Image saved as: " + result.FileName;

            _logger.LogInformation("Image '{FileName}' uploaded successfully.", result.FileName);

            return RedirectToAction("Details", new { fileName = result.FileName });
        }

        public async Task<IActionResult> Details(string fileName)
        {
            ViewBag.FileName = fileName;
            return View(); 
        }

        public async Task<IActionResult> ViewImage(string fileName)
        {
            var image = await _imageService.GetImageAsync(fileName);
            return File(image, "image/png");
        }

        public async Task<IActionResult> Grayscale(string fileName)
        {
            _logger.LogDebug("Grayscale filter requested for file: {FileName}", fileName);
            var image = await _imageService.ApplyGrayscaleAsync(fileName);
            _logger.LogInformation("Grayscale applied to file: {FileName}", fileName);
            return File(image, "image/png");
        }

        public async Task<IActionResult> Resize(string fileName, int width = 512, int height = 512)
        {
            _logger.LogDebug("Resize requested for file: {FileName}", fileName);
            var image = await _imageService.ResizeImageAsync(fileName, width, height);
            _logger.LogInformation("Image resized for file: {FileName}", fileName);
            return File(image, "image/png");
        }

        public async Task<IActionResult> Crop(string fileName, int x = 100, int y = 100, int width = 300, int height = 300)
        {
            _logger.LogDebug("Crop requested for file: {FileName}", fileName);
            var image = await _imageService.CropImageAsync(fileName, x, y, width, height);
            _logger.LogInformation("Image cropped for file: {FileName}", fileName);
            return File(image, "image/png");
        }

        [HttpGet]
        public IActionResult MultiModifications()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> MultiModifications(
            IFormFile file,
            int width = 512,
            int height = 512,
            int x = 100,
            int y = 100,
            int cropWidth = 300,
            int cropHeight = 300)
        {
            _logger.LogDebug("Multimodification requested for file: {FileName}", file.Name);

            if (file == null || file.Length == 0)
            {
                _logger.LogError("Multimodification requested for file: {FileName}, but File wasn't uploaded.", file?.Name);

                TempData["Message"] = "File wasn't uploaded.";
                return RedirectToAction("MultiModifications");
            }

            try
            {
                var result = await _imageService.MultiModificationsImageAsync(file, width, height, x, y, cropWidth, cropHeight);
                _logger.LogInformation("Multimodification applied to file: {FileName}", file.Name);
                return File(result, "image/png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during multi image modifications for file: {FileName}", file?.FileName);
                TempData["Message"] = "Error: " + ex.Message;
                return RedirectToAction("MultiModifications");
            }
        }

        public async Task<IActionResult> Delete(string fileName)
        {
            _logger.LogInformation("Delete requested for file: {FileName}", fileName);
            await _imageService.DeleteImageAsync(fileName);
            TempData["Message"] = "File deleted.";
            _logger.LogInformation("File '{FileName}' deleted successfully.", fileName);
            return RedirectToAction("Index");
        }
    }
}
