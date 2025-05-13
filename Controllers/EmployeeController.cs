using System.Text.RegularExpressions;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agri_EnergyConnect.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFarmerRepository _farmerRepository;

        private readonly IUserRepository _userRepository;
        public EmployeeController(IProductRepository productRepository, ICurrentUserService currentUserService, IFarmerRepository farmerRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _farmerRepository = farmerRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
        }
        
        // GET: EmployeeController
        [HttpGet]
        [Authorize(Roles="employee")]
        public async Task<ActionResult> ViewFarmerIndex()
        {
            // Get all farmers
            var allFarmers = await _farmerRepository.GetAll();

            // Create a list to hold farmer details with usernames
            var farmerDetailsWithUsernames = new List<(Farmer Farmer, string Username)>();

            foreach (var farmer in allFarmers)
            {
                // Get the username associated with the farmer's UserId
                var user = await _userRepository.GetById(farmer.UserId ?? 0);
                var username = user?.Username ?? "Unknown";

                // Add the farmer and username to the list
                farmerDetailsWithUsernames.Add((farmer, username));
            }

            // Pass the list to the view
            return View(farmerDetailsWithUsernames);
        }

        [Authorize(Roles = "employee")]
        [HttpGet]
        public async Task<ActionResult> ViewFarmerProducts(int farmerId, string? productType, DateTime? startDate, DateTime? endDate)
        {
            // Get products for the farmer
            var products = await _productRepository.GetProductByFarmerId(farmerId);

            // Filter by product type using regex
            if (!string.IsNullOrEmpty(productType))
            {
                try
                {
                    var regex = new Regex(productType, RegexOptions.IgnoreCase);
                    products = products.Where(p => regex.IsMatch(p.ProductType));
                    if(products == null){
                        ViewBag.ErrorMessage = "No products found matching the specified type.";
                    }
                }
                catch (ArgumentException)
                {
                    // Handle invalid regex
                    ViewBag.ErrorMessage = "Invalid regex pattern.";
                }
            }

            // Filter by date range
            if (startDate.HasValue && endDate.HasValue)
            {
                products = products.Where(p => p.CreatedAt >= startDate.Value && p.CreatedAt <= endDate.Value);
                if(products == null){
                    ViewBag.ErrorMessage = "No products found in the specified date range.";
                }
            }

            // Pass the farmerId to the view for the reset button
            ViewBag.FarmerId = farmerId;
            return View(products);
        }

    }
}
