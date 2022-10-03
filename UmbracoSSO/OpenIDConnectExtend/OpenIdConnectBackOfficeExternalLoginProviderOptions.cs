using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Security;

namespace UmbracoSSO.OpenIDConnectExtend
{
    public class OpenIdConnectBackOfficeExternalLoginProviderOptions : IConfigureNamedOptions<BackOfficeExternalLoginProviderOptions>
    {
        public const string SchemeName = "OpenIdConnect";
        public void Configure(string name, BackOfficeExternalLoginProviderOptions options)
        {
            if (name != "Umbraco." + SchemeName)
            {
                return;
            }

            Configure(options);
        }

        public void Configure(BackOfficeExternalLoginProviderOptions options)
        {

            options.ButtonStyle = "btn-danger";
            options.Icon = "fa fa-cloud";

            options.AutoLinkOptions = new ExternalSignInAutoLinkOptions(
                // must be true for auto-linking to be enabled
                autoLinkExternalAccount: true,

                // Optionally specify default user group, else
                // assign in the OnAutoLinking callback
                // (default is editor)
                defaultUserGroups: new[] { Constants.Security.EditorGroupAlias },

                // Optionally specify the default culture to create
                // the user as. If null it will use the default
                // culture defined in the web.config, or it can
                // be dynamically assigned in the OnAutoLinking
                // callback.

                defaultCulture: "en-US",
                // Optionally you can disable the ability to link/unlink
                // manually from within the back office. Set this to false
                // if you don't want the user to unlink from this external
                // provider.
                allowManualLinking: true
            )
            {
                // Optional callback
                OnAutoLinking = (autoLinkUser, loginInfo) =>
                {
                    //if (!autoLinkUser.IsApproved)
                    //{
                    //    autoLinkUser.IsApproved = true;
                    //}
                    // You can customize the user before it's linked.
                    // i.e. Modify the user's groups based on the Claims returned


                },
                OnExternalLogin = (user, loginInfo) =>
                {
                    // You can customize the user before it's saved whenever they have
                    // logged in with the external provider.
                    // i.e. Sync the user's name based on the Claims returned
                    // in the externalLogin info

                    return true; //returns a boolean indicating if sign in should continue or not.
                }
            };

            // Optionally you can disable the ability for users to login with a username/password.
            //  If this is set to true, it will disable username/password login even if there are other external login providers installed.

            options.DenyLocalLogin = false;

            // Optionally choose to automatically redirect to the external login provider so the user doesn't have to click the login button. This is

            options.AutoRedirectLoginToExternalProvider = false ;
        }


    }
}