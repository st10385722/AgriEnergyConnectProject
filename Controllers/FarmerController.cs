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
        //getting services
        private readonly IFarmerRepository _farmerRepository;
        private readonly ICurrentUserService _currentUserService;

        //constructor
        public FarmerController(IFarmerRepository farmerRepository, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _farmerRepository = farmerRepository;
        }
        //since employees make farmers, they only have access to this page
        // GET: FarmerController
        [Authorize(Roles ="employee")]
        public ActionResult Index()
        {
            return View();
        }

        //Farmers need to enter the details, so they only have access to this
        [HttpGet]
        [Authorize(Roles ="farmer")]
        [HttpGet]
        public async Task<ActionResult> FillFarmerDetails()
        {
            //get current user id via the service and claims
            var userId = _currentUserService.GetCurrentUserId();
            //uses user id to get farmer id
            var farmer = await _farmerRepository.GetFarmerByUserId(userId);
            //a check to see if a farmer already exists, used for both actions if the user
            //doesnt have a farmer profile
            if(farmer != null){
                TempData["FarmerDetailsAlreadyExistError"] = "You have already filled in your details";
                TempData.Keep();
                return RedirectToAction("Index", "Home");
            }
            ViewBag.UserId = userId;
            return View();
        }
        //farmer only action
        [Authorize(Roles ="farmer")]
        [HttpPost]
        public async Task<ActionResult> FillFarmerDetails(Farmer farmer){
            if (ModelState.IsValid)
            {
                //gets the absolute value of the guid hash code to always get a positive value, getHashCode
                //has a low clash probability
                farmer.FarmerId = Math.Abs(Guid.NewGuid().GetHashCode());
                // Save the farmer to the database
                await _farmerRepository.Insert(farmer);
                //save farmer
                await _farmerRepository.SaveAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(farmer);
        }

        //employee only view
        [Authorize(Roles ="employee")]
        [HttpGet]
        public ActionResult FarmerIndex(){
            return View();
        }

        //View farmer details from the farmer index, all details listed here
        [Authorize(Roles ="employee")]
        [HttpGet]
        public async Task<ActionResult> ViewFarmerDetails(int farmerId, string username)
        {
            //find farmer via passed farmer id
            var farmer = await _farmerRepository.GetById(farmerId);
            // Create a list to hold farmer details with usernames
            //make a new list with username value being used for first and last name
            //and the farmer details
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
