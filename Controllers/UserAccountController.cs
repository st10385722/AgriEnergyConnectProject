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
    //this controller is responsible for the registration and login
    //of new users
    public class UserAccountController : Controller
    {
        //repository services 
        private IUserRepository _ur;
        //used for keeping cookies after leaving app
        private IHttpContextAccessor _httpContext;

        private readonly ICurrentUserService _cus;

        private readonly IFarmerRepository _fr;

        private readonly IProductRepository _pr;

        private readonly IProductImageService _pis;

        public UserAccountController(
            IUserRepository ur, 
            IHttpContextAccessor httpContext,
            IFarmerRepository fr,
            IProductRepository pr,
            IProductImageService pis,
            ICurrentUserService cus)
        {
            _ur = ur;
            _httpContext = httpContext;
            _cus = cus;
            _fr = fr;
            _pr = pr;
            _pis = pis;
        }
        // GET: UserAccountController
        public ActionResult Index()
        {        
            return View();
        }
        //admins and employees both create users
        [HttpGet]
        [Authorize(Roles = "admin, employee")]
        public ActionResult CreateUser(){
            return View();
        }

        [Authorize(Roles = "admin, employee")]
        [HttpPost]
        //passing the password and cofirm password for checking against each other
        public async Task<ActionResult> CreateUserAction(User user, string Password, string ConfirmPassword){
            //Removing all of these Model state checks as they are assigned in the method
            ModelState.Remove("PasswordHash");
            ModelState.Remove("RoleId");
            ModelState.Remove("CreatedAt");
            ModelState.Remove("CreatedBy");
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Role");
            ModelState.Remove("CreatedByNavigation"); 

            if(ModelState.IsValid){
                //gets the user id for assigning to the user
                var creatorId = _cus.GetCurrentUserId();
                var currentUserRole = _cus.GetCurrentUserRole();
                //checks if the username and password match
                if(!Password.Equals(ConfirmPassword)){
                    ModelState.AddModelError("Password", "Password does not match confirm password!");
                    return View(user);
                }
                //creates a new user id via the absolute valud of GetHashCode
                user.UserId = Math.Abs(Guid.NewGuid().GetHashCode());
                //Making a new Identity password hasher object for hashing passwords
                //preventing them from being stored in plaintext
                var hasher = new PasswordHasher<object>();
                //hashes the password to check if they match
                //another way is to unhash the password and see if they both match
                //assigning the new hashed password to the user
                string hashedPassword = hasher.HashPassword(user, Password);
                user.PasswordHash = hashedPassword;
                //this switch checks the role of the current user, and assigns the role based
                //on their own role
                int roleToAssign;
                //to verify
                switch(currentUserRole){
                    case "admin":
                        roleToAssign = 2;
                    break;
                    case "employee":
                        roleToAssign = 3;
                    break;
                    default: roleToAssign = -1; 
                    break;
                }
                //saves the role to the user, as well as populating the rest of
                //the values
                user.RoleId = roleToAssign;
                user.CreatedAt = DateTime.UtcNow;
                user.CreatedBy = creatorId;

                //inserting and saving the values
                await _ur.Insert(user);
                await _ur.SaveAsync();
                return RedirectToAction("Index", "Home");
            }

            //go this far, something wrong, return view
            return View();
        }
        //Httpget for loging into new account
        [HttpGet]
        public ActionResult Login(){
            return View();
        }

        //httppost method for loggining in
        [HttpPost]
        public async Task<ActionResult> Login(string email, string password){
            //after user details verified
            // In your Login action
            var user = _ur.GetUserWithRole(email);
            //couldnt find user
            if(user == null){
                ModelState.AddModelError("Password", "User does not exist");
                ViewBag.ErrorMessage = "User does not exist. Get in contact with our team to setup your profile";
                return View(user);
            }
            //gets user role
            var role = _ur.getUserRole(user.RoleId);
            //new hasher object, increase security if you dont use the same password hasher object
            var hasher = new PasswordHasher<object>();
            //verfied the inputted password with the hashed password
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            // Returns PasswordVerificationResult.Success or Failed

            //if it passes, do the following
            if (result == PasswordVerificationResult.Success)
            {
                //make a claim that allows the user to be registed as as a online entity
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, role.RoleName),
                };
                //uusing the claim from the custom auth created for identity in program.cs
                var identity = new ClaimsIdentity(claims, "CustomAuthentication");
                var principal = new ClaimsPrincipal(identity);
                
                //uses the httpcontext IHttpAccessor to keep the user signed in
                await _httpContext.HttpContext.SignInAsync("CustomAuthentication", principal);
                return RedirectToAction("Index", "Home");
            } else {
                ModelState.AddModelError("Password", "Username or password incorrect");
                ViewBag.ErrorMessage = "Username or password incorrect";
                return View(user);
            }
        }
        //simple method to log out of app
        [HttpPost]
        public async Task<ActionResult> Logout()
        {
            // Sign out the user from the "CustomAuthentication" scheme
            await _httpContext.HttpContext.SignOutAsync("CustomAuthentication");

            // Redirect to the login page or home page after logout
            return RedirectToAction("Login", "UserAccount");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<ActionResult> AllUsers()
        {
            // Fetch all users
            var users = await _ur.GetAll();

            // Create a list to hold user data along with role names
            var usersWithRoles = new List<(User User, string RoleName)>();

            foreach (var user in users)
            {
                // Fetch the role name using the RoleId
                var role = _ur.getUserRole(user.RoleId);
                var roleName = role?.RoleName ?? "Unknown";

                // Add the user and role name to the list
                usersWithRoles.Add((user, roleName));
            }

            // Pass the list to the view
            return View(usersWithRoles);
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> EditUser(int userId)
        {
            var user = await _ur.GetById(userId);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> EditUser(User updatedUser)
        {
            var existingUser = await _ur.GetById(updatedUser.UserId);

            // Update fields
            existingUser.Username = updatedUser.Username ?? existingUser.Username;
            existingUser.Email = updatedUser.Email ?? existingUser.Email;
            existingUser.RoleId = updatedUser.RoleId > 0 ? updatedUser.RoleId : existingUser.RoleId;

            // Save changes
            _ur.Update(existingUser);
            await _ur.SaveAsync();

            return RedirectToAction("AllUsers", "UserAccount");
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var user = await _ur.GetById(userId);

            // Fetch the role name using the RoleId
            var role = _ur.getUserRole(user.RoleId);
            var roleName = role?.RoleName ?? "Unknown";

            // Pass the user and role name to the view
            var userWithRole = (User: user, RoleName: roleName);
            return View(userWithRole);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> ConfirmDeleteUser(int userId)
        {
            // Check if the user has any associated farmers
            var farmer = await _fr.GetFarmerByUserId(userId);
                // Fetch all products associated with the farmer
                var products = await _pr.GetProductByFarmerId(farmer.FarmerId);
                foreach (var product in products)
                {
                    var productImages = await _pis.GetProductImageById((int)product.ProductImageId);
                    // Delete all product images associated with the product
                    await _pis.Delete((int)product.ProductImageId);

                    // Delete the product
                    await _pr.Delete(product.ProductId);
                }
                // Delete the farmer
                await _fr.Delete(farmer.FarmerId);
            // Finally, delete the user
            await _ur.Delete(userId);
            await _ur.SaveAsync();

            return RedirectToAction("AllUsers", "UserAccount");
        }
    }
}
