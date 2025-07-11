using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace SafeVault.Models
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [BindProperty]
        public RegisterModel Register { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? RegisterMessage { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        
        }

        public class RegisterModel
        {
            [Required]
            public string Username { get; set; } = string.Empty;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }
        public LoginModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var loginUser = new LoginUser("YourConnectionStringHere");
            bool isAuthenticated = await loginUser.AuthenticateAsync(Input.Username, Input.Password);

            if (isAuthenticated)
            {
                //Issuing token
                var secretKey = "YourSecretKeyHere"; // Use a secure key in production  
                string jwtToken = JwtTokenHelper.GenerateToken(Input.Username, secretKey, 100000);

                Response.Cookies.Append("JwtToken", jwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set to true if using HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(1) // Set expiration as needed
                }); 

                // Set authentication cookie or session here as needed
                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }
        }

        
        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Ensure the role exists
            string role = "User"; // Or "Admin", "Guest" as needed
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }

            var user = new IdentityUser { UserName = Register.Username };
            var result = await _userManager.CreateAsync(user, Register.Password);

            if (result.Succeeded)
            {
                // Assign the role to the user
                await _userManager.AddToRoleAsync(user, role);
                RegisterMessage = "Registration successful. Please log in.";
            }
            else
            {
                RegisterMessage = "Registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }
            return Page();
        }
    }
}
