using System.Threading.Tasks;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agri_EnergyConnect.Controllers
{
    public class FarmerController : Controller
    {
        private readonly IFarmerRepository _farmerRepository;
        private readonly ICurrentUserService _currentUserService;
        public FarmerController(IFarmerRepository farmerRepository, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _farmerRepository = farmerRepository;
        }
        // GET: FarmerController
        [Authorize(Roles ="employee,admin")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles ="farmer")]
        [HttpGet]
        public async Task<ActionResult> FillFarmerDetails()
        {
            var userId = _currentUserService.GetCurrentUserId();
            var farmer = await _farmerRepository.GetFarmerByUserId(userId);
            if(farmer != null){
                TempData["FarmerDetailsAlreadyExistError"] = "You have already filled in your details";
                TempData.Keep();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.UserId = userId;
            return View();
        }

        [Authorize(Roles ="farmer,employee,admin")]
        [HttpPost]
        public async Task<ActionResult> FillFarmerDetails(Farmer farmer){
            if (ModelState.IsValid)
            {
                farmer.FarmerId = Math.Abs(Guid.NewGuid().GetHashCode());
                // Save the farmer to the database
                await _farmerRepository.Insert(farmer);
                await _farmerRepository.SaveAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(farmer);
        }

        [Authorize(Roles ="employee,admin")]
        [HttpGet]
        public ActionResult FarmerIndex(){
            return View();
        }

        [Authorize(Roles ="employee,admin")]
        [HttpGet]
        public async Task<ActionResult> ViewFarmerDetails(int farmerId, string username)
        {
            var farmer = await _farmerRepository.GetById(farmerId);
            // Create a list to hold farmer details with usernames
            var farmerDetailsWithUsernames = new List<(Farmer Farmer, string Username)>
            {
                // Add the farmer and username to the list
                (farmer, username)
            };

            // Pass the list to the view
            return View(farmerDetailsWithUsernames);
        }

    }
}
