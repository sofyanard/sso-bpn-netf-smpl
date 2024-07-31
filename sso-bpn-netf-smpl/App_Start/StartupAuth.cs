using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security;
using Owin;
using System;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Notifications;
using System.Security.Claims;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Http;
using System.Collections.Generic;
using System.Web;

[assembly: OwinStartup(typeof(sso_bpn_netf_smpl.App_Start.StartupAuth))]

namespace sso_bpn_netf_smpl.App_Start
{
    public class StartupAuth
    {
        string _clientId = System.Configuration.ConfigurationManager.AppSettings["clientId"];
        string _clientSecret = System.Configuration.ConfigurationManager.AppSettings["clientSecret"];
        string _redirectUri = System.Configuration.ConfigurationManager.AppSettings["redirectUri"];
        // string _logoutRedirectUri = System.Configuration.ConfigurationManager.AppSettings["logoutRedirectUri"];
        static string _tenant = System.Configuration.ConfigurationManager.AppSettings["tenant"];
        static string _authority = String.Format(System.Globalization.CultureInfo.InvariantCulture, System.Configuration.ConfigurationManager.AppSettings["authority"], _tenant);
        static string _metadataAddress = _authority + "/.well-known/openid-configuration";
        string authenticatedPage = System.Configuration.ConfigurationManager.AppSettings["authenticatedUri"];

        public void Configuration(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret,
                Authority = _authority,
                RedirectUri = _redirectUri,
                PostLogoutRedirectUri = _redirectUri,
                // PostLogoutRedirectUri = _logoutRedirectUri,
                Scope = OpenIdConnectScope.OpenIdProfile,
                ResponseType = OpenIdConnectResponseType.Code,
                RedeemCode = true,
                TokenValidationParameters = new TokenValidationParameters { NameClaimType = "name", RoleClaimType = ClaimTypes.Role },
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthorizationCodeReceived = (context) => {
                        string authorizationCode = context.Code;
                        // AuthenticationResult tokenResult = await context.AcquireTokenByAuthorizationCodeAsync(authorizationCode, new Uri(redirectUri), credential);
                        return Task.FromResult(0);
                    },
                    TokenResponseReceived = (context) =>
                    {
                        string accessToken = context.TokenEndpointResponse.AccessToken;
                        string idToken = context.TokenEndpointResponse.IdToken;
                        string refreshToken = context.TokenEndpointResponse.RefreshToken;

                        MyClass.accessToken = accessToken;
                        MyClass.idToken = idToken;
                        MyClass.refreshToken = refreshToken;

                        return Task.FromResult(0);
                    },
                    /*
                    SecurityTokenValidated = n =>
                    {
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("id_token", n.ProtocolMessage.IdToken));
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("access_token", n.ProtocolMessage.AccessToken));

                        var handler = new JwtSecurityTokenHandler();
                        var jwtSecurityToken = handler.ReadJwtToken(n.ProtocolMessage.AccessToken);

                        JObject obj = JObject.Parse(jwtSecurityToken.Claims.First(c => c.Type == "resource_access").Value);
                        var roleAccess = obj.GetValue(_clientId).ToObject<JObject>().GetValue("roles");
                        foreach (JToken role in roleAccess)
                        {
                            n.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                        }

                        JObject obj2 = JObject.Parse(jwtSecurityToken.Claims.First(c => c.Type == "atrbpn-profile").Value);

                        foreach (var x in obj2)
                        {
                            n.AuthenticationTicket.Identity.AddClaim(new Claim(x.Key, x.Value.ToString()));
                        }

                        return Task.FromResult(0);
                    },

                    RedirectToIdentityProvider = n =>
                    {
                        // if signing out, add the id_token_hint
                        if (n.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var idTokenHint = n.OwinContext.Authentication.User.FindFirst("id_token");

                            if (idTokenHint != null)
                            {
                                n.ProtocolMessage.IdTokenHint = idTokenHint.Value;
                            }
                        }

                        return Task.FromResult(0);
                    },
                    */

                    AuthenticationFailed = OnAuthenticationFailed
                },
                RequireHttpsMetadata = false,
                MetadataAddress = _metadataAddress,
                ProtocolValidator = new CustomOpenIdConnectProtocolValidator(false),
                SaveTokens = true,
				//accept  self sign cert
                BackchannelHttpHandler = new HttpClientHandler() { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
				}
			});
        }

        private Task OnAuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Exception.Message);
            return Task.FromResult(0);
        }
    }

    public class CustomOpenIdConnectProtocolValidator : OpenIdConnectProtocolValidator
    {
        public CustomOpenIdConnectProtocolValidator(bool shouldValidateNonce)
        {
            this.ShouldValidateNonce = shouldValidateNonce;
            this.RequireStateValidation = false;
        }
        protected override void ValidateNonce(OpenIdConnectProtocolValidationContext validationContext)
        {
            if (this.ShouldValidateNonce)
            {
                base.ValidateNonce(validationContext);
            }
        }

        private bool ShouldValidateNonce { get; set; }
    }
}
