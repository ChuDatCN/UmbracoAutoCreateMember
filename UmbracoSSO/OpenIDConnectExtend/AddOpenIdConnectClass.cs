using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Web.BackOffice.Security;

namespace UmbracoSSO.OpenIDConnectExtend
{
    public static class AddOpenIdConnectClass
    {
        public static IUmbracoBuilder AddOpenIdConnectAuthentication(this IUmbracoBuilder builder)
        {
            // Register OpenIdConnectBackOfficeExternalLoginProviderOptions here rather than require it in startup
            builder.Services.ConfigureOptions<OpenIdConnectBackOfficeExternalLoginProviderOptions>();

            builder.AddBackOfficeExternalLogins(logins =>
            {
                logins.AddBackOfficeLogin(
                    backOfficeAuthenticationBuilder =>
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        backOfficeAuthenticationBuilder.AddOpenIdConnect(
                            // The scheme must be set with this method to work for the back office
                            backOfficeAuthenticationBuilder.SchemeForBackOffice(OpenIdConnectBackOfficeExternalLoginProviderOptions.SchemeName),
                            options =>
                            {
                                // use cookies
                                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                                // pass configured options along
                                options.Authority = "https://localhost:5001/";
                                options.ClientId = "Umbraco-SSO";
                                options.ClientSecret = "secret";
                                // Use the authorization code flow
                                options.ResponseType = OpenIdConnectResponseType.Code;
                                options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                                options.CallbackPath = "/signin-oidc";
                                options.ResponseMode = "query";
                                // map claims
                                options.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
                                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                                options.RequireHttpsMetadata = true;        // = false khi Authority là http://
                                options.GetClaimsFromUserInfoEndpoint = true;
                                options.SaveTokens = true;
                                // add scopes
                                options.Scope.Add("openid");
                                options.Scope.Add("profile");
                                options.Scope.Add("email");
                                //options.Scope.Add("roles");
                                options.UsePkce = true;
                                //options.UsePkce = false;

                               
                            });
#pragma warning restore CS8604 // Possible null reference argument.
                    });
            });
            return builder;
        }
    }
}
