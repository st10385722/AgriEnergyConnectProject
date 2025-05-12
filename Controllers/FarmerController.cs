using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agri_EnergyConnect.Controllers
{
    public class FarmerController : Controller
    {
        private readonly IFarmerRepository _farmerRepository;
        public FarmerController(IFarmerRepository farmerRepository)
        {
            _farmerRepository = farmerRepository;
        }
        // GET: FarmerController
        [Authorize(Roles ="employee,admin")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles ="farmer,employee,admin")]
        [HttpGet]
        public ActionResult FillFarmerDetails(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [Authorize(Roles ="farmer,employee,admin")]
        [HttpPost]
        public async Task<ActionResult> FillFarmerDetails(Farmer farmer){
            if (ModelState.IsValid)
            {
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

    }
}
