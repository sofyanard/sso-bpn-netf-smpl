﻿using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sso_bpn_netf_smpl
{
    public partial class SiteMaster : MasterPage
    {
        string authenticatedPage = System.Configuration.ConfigurationManager.AppSettings["authenticatedUri"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
            {
                if (!this.Page.Request.FilePath.ToLower().Contains("/default"))
                {
                    Response.Redirect("~/Default.aspx");
                }
            }
            else
            {
                var userClaims = HttpContext.Current.User.Identity as System.Security.Claims.ClaimsIdentity;
                string nama = userClaims?.FindFirst("name")?.Value;
                string email = userClaims?.Claims.Where(x => x.Type.Contains("emailaddress")).FirstOrDefault()?.Value;

                

                Label1.Text = nama;
            }
        }
    }
}