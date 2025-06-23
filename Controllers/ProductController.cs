using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agri_EnergyConnect.Controllers
{
    public class ProductController : Controller
    {
        //dependency injection
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFarmerRepository _farmerRepository;
        private readonly IProductImageService _productImageService;
        //getting 1 instance of user id for entire controller
        private readonly int userId;

        //constructor
        public ProductController(IProductRepository productRepository, ICurrentUserService currentUserService, IFarmerRepository farmerRepository, IProductImageService productImageService)
        {
            _farmerRepository = farmerRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
            _productImageService = productImageService;
            //assigning user id in constructor for all methods to use
            userId = _currentUserService.GetCurrentUserId();
        }
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        //Farmer uses this action to view their products
        //similar to Employee ViewFarmerProducts, but seperation
        //of concerns
        [Authorize(Roles="farmer")]
        [HttpGet]
        public async Task<ActionResult> MyProducts()
        {
            //gets currently logged user id

            // Get the FarmerId associated with the logged-in UserId
            var farmer = await _farmerRepository.GetFarmerByUserId(userId);
            if (farmer == null)
            {
                //checking for the null farmer details so no products can be seen if its null
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

        //HttpGet for create product
        [Authorize(Roles = "farmer")]
        [HttpGet]
        public async Task<ActionResult> CreateProduct()
        {
            //again gets the current user id
            var farmer = await _farmerRepository.GetFarmerByUserId(userId);
            //checks whether farmer has submitted their details
            if (farmer == null)
            {
                TempData["FarmerDetailsNotSetup"] = "You have not setup your profile yet. Please setup your profile to add products";
                TempData.Keep();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        //Http post for create product
        [Authorize(Roles = "farmer")]
        [HttpPost]
        public async Task<ActionResult> CreateProduct(Product product, IFormFile ProductImage)
        {
            //remove these variables from the model state check as they are
            //assigned in the method itself
            ModelState.Remove("createdAt"); 
            ModelState.Remove("farmerId");
            ModelState.Remove("ProductImage");
            if (ModelState.IsValid)
            {
                //checks if the image is too big, 10MB is the limit
                if(ProductImage.Length > 10 * 1024 * 1024){
                    ViewBag.FileToBigError = "File size is too large. The limt is 10MB.";
                    return View(product);
                }
                //getting farmer again
                var farmer = await _farmerRepository.GetFarmerByUserId(userId);
                if (farmer != null)
                {
                    //store product table related information
                    product.ProductId = Math.Abs(Guid.NewGuid().GetHashCode());
                    product.FarmerId = farmer.FarmerId;
                    product.CreatedAt = DateTime.UtcNow;
                    //saves to product table via the repository
                    await _productRepository.Insert(product);
                    await _productRepository.SaveAsync();
                    //now moves on to product image
                    if (ProductImage != null)
                    {
                        //uses repository to upload and save the image
                        var productImage = await _productImageService.ProcessImageAsync(ProductImage, product.ProductId);
                        productImage.ImageId = Math.Abs(Guid.NewGuid().GetHashCode());
                        productImage.ProductId = product.ProductId;
                        productImage.CreatedAt = DateTime.UtcNow;
                        // Save the image to the database
                        await _productImageService.Insert(productImage);
                        await _productImageService.SaveAsync();

                        //adding relationship for product and product image
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

        //getting the product image for viewing in the my products or viewfarmerproducts employee view
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

        //method to edit the product, can be done by both farmer and employee
        [Authorize(Roles = "farmer, employee")]
        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
        {
            //gets product via passed id from the view, to be sent to the edit product view
            var product = await _productRepository.GetById(productId);
            return View(product);
        }

        //updates the product based on the information
        [Authorize(Roles = "farmer,employee")]
        [HttpPost]
        //nullable iformfile as user does not have to upload a new file when editing
        public async Task<IActionResult> EditProduct(Product updatedProduct, IFormFile? ProductImage)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = await _productRepository.GetById(updatedProduct.ProductId);
     
                // Update fields with a null check to revert back to original data
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

        //Method to delete a product, done by both employees and farmers
        [HttpPost]
        [Authorize(Roles = "farmer,employee")]
        //gets passed values from the view asp-route tags
        public async Task<IActionResult> DeleteProduct(int productId, int productImageId){

            //getting the farmer id to pass back to the view

            var product = await _productRepository.GetById(productId);
            var farmerId = product.FarmerId;
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
                //passes farmer id back to view
                return RedirectToAction("ViewFarmerProducts", "Employee", new {farmerId});
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
