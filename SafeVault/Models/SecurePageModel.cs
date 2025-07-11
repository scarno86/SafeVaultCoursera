using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SafeVault.Models
{
    [Authorize(Policy = "RequireAdmin")]
    public class AdminPageModel : PageModel
    {
        // Only users in the "Admin" role can access this page
    }

    [Authorize(Policy = "User,Admin")]
    public class UserPageModel : PageModel
    {
        // Users in "User" or "Admin" roles can access this page
    }

    [Authorize(Policy = "Guest")]
    public class GuestPageModel : PageModel
    {
        // Only users in the "Guest" role can access this page
    }
}
