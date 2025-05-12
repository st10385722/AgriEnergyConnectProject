using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Services.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Agri_EnergyConnect.Services;
using Microsoft.AspNetCore.Authorization;

namespace Agri_EnergyConnect.Controllers
{
    public class UserAccountController : Controller
    {
        private IUserRepository _ur;
        // private HttpContext _httpContext;
        private IHttpContextAccessor _httpContext;

        private readonly ICurrentUserService _cus;

        public UserAccountController(IUserRepository ur, 
        //HttpContext httpContext, 
        IHttpContextAccessor httpContext,
        ICurrentUserService cus){
            _ur = ur;
            //_httpContext = httpContext;
            _httpContext = httpContext;
            _cus = cus;
        }
        // GET: UserAccountController
        public ActionResult Index()
        {        
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "admin, employee")]
        public ActionResult CreateUser(){
            return View();
        }

        [Authorize(Roles = "admin, employee")]
        [HttpPost]
        public async Task<ActionResult> CreateUserAction(User user, string Password, string ConfirmPassword){
            ModelState.Remove("PasswordHash");
            ModelState.Remove("RoleId");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Role");
            ModelState.Remove("CreatedByNavigation"); 

            if(ModelState.IsValid){
                var creatorId = _cus.GetCurrentUserId();
                //if(creatorId == 0) {return Unauthorized();}
                var currentUserRole = _cus.GetCurrentUserRole();
                if(!Password.Equals(ConfirmPassword)){
                    ModelState.AddModelError("Password", "Password does not match confirm password!");
                    return View(user);
                }
                user.UserId = Math.Abs(Guid.NewGuid().GetHashCode());
                var hasher = new PasswordHasher<object>();
                string hashedPassword = hasher.HashPassword(user, Password);
                user.PasswordHash = hashedPassword;
                int roleToAssign;
                //to verify
                //var result = hasher.VerifyHashedPassword(null, hashedPassword, "input_password");
                // Returns PasswordVerificationResult.Success or Failed
                switch(currentUserRole){
                    case "admin":
                        //change for auto generating
                        roleToAssign = 2;
                    break;
                    case "employee":
                        roleToAssign = 3;
                    break;
                    default: roleToAssign = -1; 
                    break;
                }
                user.RoleId = roleToAssign;
                user.CreatedAt = DateTime.UtcNow;
                user.CreatedBy = creatorId;

                await _ur.Insert(user);
                await _ur.SaveAsync();
                //come back to this
                // if (currentUserRole == "employee")
                // {
                //     return RedirectToAction("FillFarmerDetails", "Farmer", new { userId = user.UserId });
                // }
                return RedirectToAction("Index", "Home");
            }

            //go this far, something wrong, return view
            return View();
        }
        [HttpGet]
        public ActionResult Login(){
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string email, string password){
            //after user details verified
            // In your Login action
            var user = _ur.GetUserWithRole(email);
            if(user == null){
                ModelState.AddModelError("Password", "Username or password is incorrect!");
                ViewBag.ErrorMessage = "Username or password is incorrect!";
                return View(user);
            }
            var role = _ur.getUserRole(user.RoleId);
            var hasher = new PasswordHasher<object>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            // Returns PasswordVerificationResult.Success or Failed

            if (result == PasswordVerificationResult.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, role.RoleName),
                };

                var identity = new ClaimsIdentity(claims, "CustomAuthentication");
                var principal = new ClaimsPrincipal(identity);
                
                await _httpContext.HttpContext.SignInAsync("CustomAuthentication", principal);
                return RedirectToAction("Index", "Home");
            }
            //error, return page
            return View(email, password);
        }
        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            // Sign out the user from the "CustomAuthentication" scheme
            await _httpContext.HttpContext.SignOutAsync("CustomAuthentication");

            // Redirect to the login page or home page after logout
            return RedirectToAction("Login", "UserAccount");
        }
    }
}
