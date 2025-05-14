using System;
using System.Security.Claims;

namespace Agri_EnergyConnect.Services;

//this file is used to get the currently logged in user
//role and userid. Useful for checking whether a user
//is in a correct role via the claims
//or checking if they have a valid userId

//declaration of the ICurrentUserService is done
//within the CurrentUserService file
//this still performs the job of 
//abstracting access to the httpcontext
public interface ICurrentUserService{
    int GetCurrentUserId();
    string GetCurrentUserRole();
}

//the current user service class
//that gets the httpcontext accessor
public class CurrentUserService: ICurrentUserService
{
    //httpconextAccessor declaration
    private readonly IHttpContextAccessor _httpContextAccessor;

    //constructor
    public CurrentUserService(IHttpContextAccessor httpContextAccessor){
        _httpContextAccessor = httpContextAccessor;
    }

    //gets the currently logged in user id that is used for various ActionResult
    //around the website
    public int GetCurrentUserId(){
        var userIdClaim = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    //gets the currently logged in user role. Used for checking whether
    //a user is able to access a page
    public string GetCurrentUserRole(){
        return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
    }
}
