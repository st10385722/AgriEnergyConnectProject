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
                TempData["FarmerDetailsNotSetup"] = "You have not setup your profile yet. Please setup your profile to add products";
                TempData.Keep();
                return RedirectToAction("Index", "Home");
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
        public async Task<ActionResult> CreateProduct()
        {
            var userId = _currentUserService.GetCurrentUserId();
            var farmer = await _farmerRepository.GetFarmerByUserId(userId);
            if (farmer == null)
            {
                TempData["FarmerDetailsNotSetup"] = "You have not setup your profile yet. Please setup your profile to add products";
                TempData.Keep();
                return RedirectToAction("Index", "Home");
            }
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
                if(ProductImage.Length > 10 * 1024 * 1024){
                    ViewBag.FileToBigError = "File size is too large. The limt is 10MB.";
                    return View(product);
                }
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


                        product.ProductImageId = productImage.ImageId;
                        _productRepository.Update(product);
                        await _productRepository.SaveAsync();
                        //save updated product image
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

        [Authorize(Roles = "farmer, employee")]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
        {
            var product = await _productRepository.GetById(productId);
            return View(product);
        }

        [Authorize(Roles = "farmer,employee")]
        [HttpPost]
        public async Task<IActionResult> EditProduct(Product updatedProduct, IFormFile? ProductImage)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetById(updatedProduct.ProductId);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                // Update fields
                existingProduct.ProductName = updatedProduct.ProductName ?? existingProduct.ProductName;
                existingProduct.ProductType = updatedProduct.ProductType ?? existingProduct.ProductType;
                existingProduct.ProductDescription = updatedProduct.ProductDescription ?? existingProduct.ProductDescription;
                existingProduct.Quantity = updatedProduct.Quantity ?? existingProduct.Quantity;
                existingProduct.Price = updatedProduct.Price ?? existingProduct.Price;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                // Update image if a new one is uploaded
                if (ProductImage != null)
                {
                    //delete old image first
                    await _productImageService.Delete((int)existingProduct.ProductImageId);
                    await _productImageService.SaveAsync();

                    //upload new image after
                    var productImage = await _productImageService.ProcessImageAsync(ProductImage, existingProduct.ProductId);
                    productImage.ImageId = Math.Abs(Guid.NewGuid().GetHashCode());
                    productImage.ProductId = existingProduct.ProductId;
                    productImage.CreatedAt = DateTime.UtcNow;

                    // Save the new image
                    await _productImageService.Insert(productImage);
                    await _productImageService.SaveAsync();

                    // Update the product's image reference
                    existingProduct.ProductImageId = productImage.ImageId;
                }

                // Save changes
                _productRepository.Update(existingProduct);
                await _productRepository.SaveAsync();

                if(User.IsInRole("farmer")){
                    return RedirectToAction("MyProducts");
                }
                if(User.IsInRole("employee")){
                    return RedirectToAction("ViewFarmerProducts", "Employee", new { farmerId = existingProduct.FarmerId });
                }

                //Something went wrong, return view
                return View();
            }

            return View(updatedProduct);
        }

        [HttpPost]
        [Authorize(Roles = "farmer,employee")]
        public async Task<IActionResult> DeleteProduct(int productId, int productImageId){
            //delete image first
            await _productImageService.Delete(productImageId);
            await _productImageService.SaveAsync();
            //then delete product
            await _productRepository.Delete(productId);
            await _productRepository.SaveAsync();
            if(User.IsInRole("farmer")){
            return RedirectToAction("MyProducts");
            }
            if(User.IsInRole("employee")){
                return RedirectToAction("ViewFarmerProducts", "Employee");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
