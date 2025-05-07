using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;

namespace Agri_EnergyConnect.Controllers
{
    public class UserAccountController : Controller
    {
        private IUserRepository _ur;

        public UserAccountController(IUserRepository ur){
            _ur = ur;
        }
        // GET: UserAccountController
        public ActionResult Index()
        {        return View();
        }
        [HttpGet]
        public ActionResult CreateFarmer(){
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateFarmer(User user, string Password, string ConfirmPassword){
            ModelState.Remove("PasswordHash");
            ModelState.Remove("RoleId");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Role");
            ModelState.Remove("CreatedByNavigation"); 

            if(ModelState.IsValid){
                if(!Password.Equals(ConfirmPassword)){
                    ModelState.AddModelError("Password", "Password does not match confirm password!");
                    return View(user);
                }
                var hasher = new PasswordHasher<object>();
                string hashedPassword = hasher.HashPassword(null, Password);
                user.PasswordHash = hashedPassword;
                //to verify
                //var result = hasher.VerifyHashedPassword(null, hashedPassword, "input_password");
                // Returns PasswordVerificationResult.Success or Failed

                await _ur.Insert(user);
                await _ur.SaveAsync();
                return RedirectToAction("Index", "Home");
            }

            //go this far, something wrong, return view
            return View();
        }
        public ActionResult Login(){
            return View();

            //after user details verified
            // In your Login action
// var user = _dbContext.Users
//     .Include(u => u.UserRole)  // Join with role table
//     .FirstOrDefault(u => u.Username == model.Username);

// if (user != null && VerifyPassword(model.Password, user.PasswordHash))
// {
//     var claims = new List<Claim>
//     {
//         new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
//         new Claim(ClaimTypes.Name, user.Username),
//         new Claim(ClaimTypes.Role, user.UserRole.RoleName) // From joined table
//     };

//     var identity = new ClaimsIdentity(claims, "CustomAuth");
//     var principal = new ClaimsPrincipal(identity);
    
//     await HttpContext.SignInAsync("CustomScheme", principal);
//     return RedirectToAction("Index", "Home");
// }
        }
        
    }
}
