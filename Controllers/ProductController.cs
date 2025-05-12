using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agri_EnergyConnect.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFarmerRepository _farmerRepository;
        private readonly IProductImageService _productImageService;
        public ProductController(IProductRepository productRepository, ICurrentUserService currentUserService, IFarmerRepository farmerRepository, IProductImageService productImageService)
        {
            _farmerRepository = farmerRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _productImageService = productImageService;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles="farmer")]
        [HttpGet]
        public async Task<ActionResult> MyProducts()
        {
            var userId = _currentUserService.GetCurrentUserId();

            // Get the FarmerId associated with the logged-in UserId
            var farmer = await _farmerRepository.GetFarmerByUserId(userId);
            if (farmer == null)
            {
                ViewBag.WarningMessage = "You are not associated with any farmer profile. Please contact the administrator.";
                return View(new List<Product>());
            }

            // Check if the farmer has any products
            var products = await _productRepository.GetProductByFarmerId(farmer.FarmerId);
            if (!products.Any())
            {
                ViewBag.WarningMessage = "You have no products listed. Please add products to your profile.";
            }

            return View(products);
        }
        [Authorize(Roles = "farmer")]
        [HttpGet]
        public ActionResult CreateProduct()
        {
            return View();
        }

        [Authorize(Roles = "farmer")]
        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product, IFormFile ProductImage)
        {
            ModelState.Remove("createdAt"); 
            ModelState.Remove("farmerId");
            ModelState.Remove("ProductImage");
            if (ModelState.IsValid)
            {
                var userId = _currentUserService.GetCurrentUserId();
                var farmer = await _farmerRepository.GetFarmerByUserId(userId);
                if (farmer != null)
                {
                    product.ProductId = Math.Abs(Guid.NewGuid().GetHashCode());
                    product.FarmerId = farmer.FarmerId;
                    product.CreatedAt = DateTime.UtcNow;
                    await _productRepository.Insert(product);
                    await _productRepository.SaveAsync();
                    if (ProductImage != null)
                    {
                        var productImage = await _productImageService.ProcessImageAsync(ProductImage, product.ProductId);
                        productImage.ImageId = Math.Abs(Guid.NewGuid().GetHashCode());
                        productImage.ProductId = product.ProductId;
                        productImage.CreatedAt = DateTime.UtcNow;
                        // Save the image to the database
                        await _productImageService.Insert(productImage);
                        await _productImageService.SaveAsync();
                    }

                    return RedirectToAction("MyProducts");
                }
                ModelState.AddModelError("", "Unable to create product. Farmer not found.");
            }
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductImage(int imageId)
        {
            var productImage = await _productImageService.GetProductImageById(imageId);
            if (productImage == null)
            {
                return NotFound();
            }
            return File(productImage.ImageData, productImage.ContentType);
        }
    }
}
