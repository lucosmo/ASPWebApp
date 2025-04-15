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
                TempData["Message"] = "Image wasn't uploaded.";
                return RedirectToAction("Index");
            }

            var result = await _imageService.SaveImageAsync(file);
            TempData["Message"] = "Image saved as: " + result.FileName;
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
            var image = await _imageService.ApplyGrayscaleAsync(fileName);
            return File(image, "image/png");
        }

        public async Task<IActionResult> Resize(string fileName, int width = 512, int height = 512)
        {
            var image = await _imageService.ResizeImageAsync(fileName, width, height);
            return File(image, "image/png");
        }

        public async Task<IActionResult> Crop(string fileName, int x = 100, int y = 100, int width = 300, int height = 300)
        {
            var image = await _imageService.CropImageAsync(fileName, x, y, width, height);
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
            if (file == null || file.Length == 0)
            {
                TempData["Message"] = "File wasn't uploaded.";
                return RedirectToAction("MultiModifications");
            }

            try
            {
                var result = await _imageService.MultiModificationsImageAsync(file, width, height, x, y, cropWidth, cropHeight);
                return File(result, "image/png");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Error: " + ex.Message;
                return RedirectToAction("MultiModifications");
            }
        }

        public async Task<IActionResult> Delete(string fileName)
        {
            await _imageService.DeleteImageAsync(fileName);
            TempData["Message"] = "File deleted.";
            return RedirectToAction("Index");
        }
    }
}
