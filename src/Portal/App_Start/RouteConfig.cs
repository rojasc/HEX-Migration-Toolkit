// -----------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Provides the ability to configure routes.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// Registers the specified routes.
        /// </summary>
        /// <param name="routes">A collection of routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}