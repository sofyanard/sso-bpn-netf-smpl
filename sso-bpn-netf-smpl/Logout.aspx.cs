using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sso_bpn_netf_smpl
{
    public partial class Logout : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var openIdConnectAuthenticationType = OpenIdConnectAuthenticationDefaults.AuthenticationType;
            var cookieAuthenticationType = CookieAuthenticationDefaults.AuthenticationType;

            // Add custom parameters as a dictionary
            var customParameters = new Dictionary<string, string>
{
    { "param1", "value1" },
    { "param2", "value2" }
};

            // Serialize the custom parameters into a query string
            string customParamsQueryString = string.Join("&", customParameters.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value)}"));

            // Construct the post-logout redirect URI
            string postLogoutRedirectUri = "http://localhost:8888";

            // Replace with your actual ID token
            string idTokenHint = MyClass.idToken;

            // Build the logout URL with id_token_hint, post_logout_redirect_uri, and custom parameters
            // string logoutUrl = HttpContext.GetOwinContext().Authentication.GetOpenIdConnectConfiguration(openIdConnectAuthenticationType).EndSessionEndpoint;
            string logoutUrl = "http://localhost:8888";
            logoutUrl += "?id_token_hint=" + HttpUtility.UrlEncode(idTokenHint);
            logoutUrl += "&post_logout_redirect_uri=" + HttpUtility.UrlEncode(postLogoutRedirectUri);
            logoutUrl += "&" + customParamsQueryString; // Add custom parameters

            // Redirect the user to the OIDC identity provider's logout endpoint
            // Response.Redirect(logoutUrl);

            // Sign out the user from both OIDC and cookie authentication
            // HttpContext.Current.GetOwinContext().Authentication.SignOut(new AuthenticationProperties(), openIdConnectAuthenticationType, cookieAuthenticationType);



            HttpContext.Current.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties(),
                OpenIdConnectAuthenticationDefaults.AuthenticationType,
                CookieAuthenticationDefaults.AuthenticationType);

            Response.Redirect("~/Default.aspx");
        }
    }
}