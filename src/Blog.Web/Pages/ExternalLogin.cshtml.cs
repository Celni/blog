using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blog.Web.Pages
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser<long>> _signInManager;
        private readonly UserManager<IdentityUser<long>> _userManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(SignInManager<IdentityUser<long>> signInManager, 
            UserManager<IdentityUser<long>> userManager,
            RoleManager<IdentityRole<long>> roleManager,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public IActionResult OnGet(string providerName)
        {
            var redirectUrl = Url.Page("ExternalLogin", pageHandler: "Callback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(providerName, redirectUrl);
            return new ChallengeResult(providerName, properties);
        }

        public async Task<IActionResult> OnGetCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return LocalRedirect("/");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: true);

            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect("/");
            }


            if (result.IsLockedOut)
            {
                return LocalRedirect("/");
            }

            var existUserRole = await _roleManager.RoleExistsAsync("User");
            if (!existUserRole)
            {
                await _roleManager.CreateAsync(new IdentityRole<long>("User"));
                _logger.LogInformation("Create role {Role}", "User");
            }

            var existAdminRole = await _roleManager.RoleExistsAsync("Admin");
            if (!existAdminRole)
            {
                await _roleManager.CreateAsync(new IdentityRole<long>("Admin"));
                _logger.LogInformation("Create role {Role}", "Admin");
            }

            var clamsIdentity = ((ClaimsIdentity) info.Principal.Identity);
            
            var name = clamsIdentity.FindFirst(ClaimTypes.Name);
            var email = clamsIdentity.FindFirst(ClaimTypes.Email);

            var user = new IdentityUser<long>(name.Value);
            user.Email = email.Value;
            user.NormalizedUserName = clamsIdentity.FindFirst("urn:github:name").Value;

            var userResult = await _userManager.CreateAsync(user);

            if (userResult.Succeeded)
            {
                var loginResult = await _userManager.AddLoginAsync(user, info);
                if (loginResult.Succeeded)
                {
                    var claimsResult = await _userManager.AddClaimsAsync(user, clamsIdentity.Claims);
                    if (claimsResult.Succeeded)
                    {
                        var countUsers = await _userManager.Users.CountAsync();
                        if (countUsers == 1)
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
                        }
                        else
                        {
                            var roleResult = await _userManager.AddToRoleAsync(user,"User");
                        }

                        await _signInManager.ExternalLoginSignInAsync(
                            info.LoginProvider,
                            info.ProviderKey,
                            isPersistent: true);
                    }
                }
            }

            return LocalRedirect("/");
        }

    }
}