using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Account.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        public UserManager<ApplicationUser> _userManager { get; set; }

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [BindProperty]
        public ReturnViewModel Input { get; set; }
        [BindProperty]
        public bool success { get; set; }
        public IActionResult OnGet(string returnUrl)
        {
            Input = new ReturnViewModel{
                returnUrl = returnUrl
            };
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            if (Input.Button != "register") return Redirect("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Email = Input.Email,
                    UserName = Input.UserName,
                    EmailConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user, Input.password);

                if (result.Succeeded)
                {
                    await _userManager.AddClaimsAsync(user, new Claim[]
                    {
                        new Claim(JwtClaimTypes.Name, Input.FullName)
                    });

                    success = true;
                }
            }
            return Page();
        }
    }
}