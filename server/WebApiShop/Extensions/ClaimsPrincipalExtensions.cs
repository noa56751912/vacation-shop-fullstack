using System.Security.Claims;

namespace WebApiShop.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>The signed-in user's id, from the token the server itself signed.</summary>
    public static int GetUserId(this ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>A user may reach their own data; an admin may reach anyone's.</summary>
    public static bool CanAccess(this ClaimsPrincipal user, int userId) =>
        user.IsInRole("Admin") || user.GetUserId() == userId;
}
