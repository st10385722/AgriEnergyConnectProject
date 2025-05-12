using System;
using System.Security.Claims;

namespace Agri_EnergyConnect.Services;

public interface ICurrentUserService{
    int GetCurrentUserId();
    string GetCurrentUserRole();
}
public class CurrentUserService: ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor){
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetCurrentUserId(){
        var userIdClaim = _httpContextAccessor.HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    public string GetCurrentUserRole(){
        return _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
    }
}
