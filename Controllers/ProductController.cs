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
        public ProductController(IProductRepository productRepository, ICurrentUserService currentUserService, IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
        }
        // GET: ProductController
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> MyProducts()
        {
            var userId = _currentUserService.GetCurrentUserId();
            var role = _currentUserService.GetCurrentUserRole();

            if (role == "Farmer")
            {
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
            //if we got this far, something broke, display view
            return View(new List<Product>());
        }
        [Authorize(Roles = "farmer")]
        public ActionResult CreateProduct()
        {
            return View();
        }

    }
}
