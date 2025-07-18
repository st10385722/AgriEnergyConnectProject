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
        //getting services from services folder
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFarmerRepository _farmerRepository;
        private readonly IUserRepository _userRepository;

        //constructor
        public EmployeeController(IProductRepository productRepository, ICurrentUserService currentUserService, IFarmerRepository farmerRepository, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _farmerRepository = farmerRepository;
            _productRepository = productRepository;
            _currentUserService = currentUserService;
        }
        
        // GET: EmployeeController
        //authorize tag means only user with employee role can access
        [HttpGet]
        [Authorize(Roles = "employee")]
        public async Task<ActionResult> ViewFarmerIndex(DateTime? startDate, DateTime? endDate)
        {
            // Get all farmers
            var allFarmers = await _farmerRepository.GetAll();

            // Filter by harvesting date range
            if (startDate.HasValue && endDate.HasValue)
            {
                allFarmers = allFarmers.Where(f => f.HavestingDate >= startDate.Value && f.HavestingDate <= endDate.Value).ToList();

                if (!allFarmers.Any())
                {
                    ViewBag.ErrorMessage = "No farmers found in the specified harvesting date range.";
                }
            }

            // Create a list to hold farmer details with usernames
            var farmerDetailsWithUsernames = new List<(Farmer Farmer, string Username)>();

            foreach (var farmer in allFarmers)
            {
                // Get the username associated with the farmer's UserId, default to zero if not found
                var user = await _userRepository.GetById((int)farmer.UserId);
                var username = user?.Username;

                // Add the farmer and username to the list
                farmerDetailsWithUsernames.Add((farmer, username));
            }

            // Pass the list to the view
            return View(farmerDetailsWithUsernames);
        }

        //This class is used to view products of a specific farmer
        [Authorize(Roles = "employee")]
        [HttpGet]
        public async Task<ActionResult> ViewFarmerProducts(int farmerId, string? productType, DateTime? startDate, DateTime? endDate)
        {
            // Get products for the farmer
            var products = await _productRepository.GetProductByFarmerId(farmerId);

            // Get unique product types for dropdown
            //Inspired by Omar, D. 2024. Ways to bind dropdown List in ASP.NET MVC. C-SharpCorner. [Online].
            //Available at: https://www.c-sharpcorner.com/UploadFile/deveshomar/ways-to-bind-dropdown-list-in-Asp-Net-mvc/ [Accessed 23 June 2025]
            var productTypes = products.Select(p => p.ProductType).Distinct().ToList();
            ViewBag.ProductTypes = productTypes;

            // Filter by product type (exact match from dropdown)
            if (!string.IsNullOrEmpty(productType))
            {
                products = products.Where(p => p.ProductType == productType);
                if (!products.Any())
                {
                    ViewBag.NoProducts = "No products found matching the specified type.";
                }
            }

            // Filter by date range
            if (startDate.HasValue && endDate.HasValue)
            {
                products = products.Where(p => p.CreatedAt >= startDate.Value && p.CreatedAt <= endDate.Value);
                if (!products.Any())
                {
                    ViewBag.ErrorMessage = "No products found in the specified date range.";
                }
            }
            // Pass the farmerId to the view for the reset button
            ViewBag.FarmerId = farmerId;
            return View(products);
        }

    }
}
