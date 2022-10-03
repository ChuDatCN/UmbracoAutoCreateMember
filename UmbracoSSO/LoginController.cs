using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Cms.Infrastructure.Persistence;
using Umbraco.Cms.Web.Common.Security;
using Umbraco.Cms.Web.Website.Controllers;

namespace UmbracoSSO
{
    public class LoginController : SurfaceController
    {
        private readonly IMemberManager memberManager;
        public readonly IMemberService memberService;
        public readonly IMemberSignInManager signInManager;

        public LoginController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider, 
            IMemberManager _memberManager, IMemberService _memberService, IMemberSignInManager _signInManager)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            memberManager = _memberManager;
            memberService = _memberService;
            signInManager = _signInManager;

        }


        // surface controller boilerplate omitted for brevity

        [HttpGet]
        public IActionResult ExternalLogin(string returnUrl)
        {
            return Challenge(
            new AuthenticationProperties
            {
                // TODO: challenge OIDC scheme
                RedirectUri = Url.Action(nameof(ExternalLoginCallback)),
                Items = { { "returnUrl", returnUrl } }
}, "oidc");
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            
            // TODO: account linking & sign in
            var result = await HttpContext.AuthenticateAsync("temp-cookie");
            if (!result.Succeeded) throw new Exception("Missing external cookie");

            // auto-create account using email address
            var email = result.Principal.FindFirstValue(ClaimTypes.Email)
                        ?? result.Principal.FindFirstValue("email")
                        ?? throw new Exception("Missing email claim");

            var user = await memberManager.FindByEmailAsync(email);
            if (user == null)
            {

                var identityUser = MemberIdentityUser.CreateNew(email, email, Constants.Security.DefaultMemberTypeAlias, true, email);
                //memberService.CreateMemberWithIdentity("tester", "AliceSmith@email.com", "tester", Constants.Security.DefaultMemberTypeAlias);
                IdentityResult identityResult = await memberManager.CreateAsync(identityUser);
                //user = await memberManager.FindByNameAsync("tester");

                //await memberManager.AddToRolesAsync(user, new[] { "User" });
                await HttpContext.SignOutAsync("temp-cookie");
                await signInManager.SignInAsync(identityUser, false);

            }
            else { 

            // create the full membership session and cleanup the temporary cookie

            
            await HttpContext.SignOutAsync("temp-cookie");
#pragma warning disable CS8604 // Possible null reference argument.
            await signInManager.SignInAsync(user, false);
            }
#pragma warning restore CS8604 // Possible null reference argument. because we checked if if were null above

            // basic open redirect defense
            var returnUrl = result.Properties?.Items["returnUrl"];
            if (returnUrl == null || !Url.IsLocalUrl(returnUrl)) returnUrl = "~/";

            return new RedirectResult(returnUrl);
        }
    }
}
