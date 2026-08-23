using System.Security.Claims;

namespace TourManagement.Web.Helpers;

public static class UserHelper
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var claim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claim, out var id) ? id : 0;
    }

    public static bool IsInRole(this ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }

    public static string GetFullName(this ClaimsPrincipal user)
    {
        return user.FindFirstValue("FullName") ?? user.Identity?.Name ?? "";
    }
}