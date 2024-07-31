using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Claims;
using log4net;
using log4net.Config;

namespace sso_bpn_netf_smpl
{
    public partial class _Default : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(_Default));

        string authenticatedPage = System.Configuration.ConfigurationManager.AppSettings["authenticatedUri"];

        protected void Page_Load(object sender, EventArgs e)
        {
            XmlConfigurator.Configure(new System.IO.FileInfo(Server.MapPath("~") + "log4net.config"));
            log.Info("Starting application...");

            /*
            if (Request.IsAuthenticated)
            {
                var userClaims = HttpContext.Current.User.Identity as System.Security.Claims.ClaimsIdentity;
                var principalClaims = HttpContext.Current.User.Identity as System.Security.Claims.ClaimsPrincipal;
                IEnumerable<Claim> claims = userClaims.Claims;
                IEnumerable<Claim> principal = principalClaims.Claims;
            }
            */
            if (Request.IsAuthenticated)
            {
				var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;

				// Access claims from the ClaimsIdentity
				var userName = identity.FindFirst(ClaimTypes.Name)?.Value;
				var userEmail = identity.FindFirst(ClaimTypes.Email)?.Value;

                string accessToken = MyClass.accessToken;

				// var identity = (ClaimsPrincipal)HttpContext.Current.User.Identity;
				// var accessToken = identity.FindFirst("access_token")?.Value;

				if (accessToken != null)
                {
                    Label1.Text = accessToken;

				}
                else
                {
					Label1.Text = "null";
				}
            }
            else
            {
				Label1.Text = "Not Authenticated";
			}

		}

        protected void Button1_Click(object sender, EventArgs e)
        {
            log.Info("Button1 is clicked...");

            if (!Request.IsAuthenticated)
            {
                HttpContext.Current.GetOwinContext().Authentication.Challenge(
                new AuthenticationProperties { RedirectUri = authenticatedPage },
                OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                Response.Redirect(authenticatedPage);
            }
        }
    }
}