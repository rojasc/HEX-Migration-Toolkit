﻿// -----------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Controllers
{
    using System.Web;
    using System.Web.Mvc;
    using Owin.Security;
    using Owin.Security.Cookies;
    using Owin.Security.OpenIdConnect;

    /// <summary>
    /// Manages user logins and logouts.
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// Logs a user in to the application.
        /// </summary>
        public void SignIn()
        {
            // Send an OpenID Connect sign-in request.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        /// <summary>
        /// Logs a user out of the application.
        /// </summary>s
        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }
    }
}