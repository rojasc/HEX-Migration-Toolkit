// -----------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal
{
    using System.IO;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Common;
    using Configuration;
    using Configuration.Bundling;
    using Configuration.Manager;
    using Practices.Unity;

    /// <summary>
    /// Defines the methods and properties that are common to application objects.
    /// </summary>
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Gets the unity container for the application.
        /// </summary>
        internal static IUnityContainer UnityContainer { get; private set; }

        /// <summary>
        /// Invoked when the application starts.
        /// </summary>
        protected void Application_Start()
        {
            IMigrationService service;

            try
            {
                AreaRegistration.RegisterAllAreas();

                UnityContainer = UnityConfig.GetConfiguredContainer();

                service = UnityContainer.Resolve<IMigrationService>();

                ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey =
                    service.Configuration.InstrumentationKey;

                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                FilterConfig.RegisterWebApiFilters(GlobalConfiguration.Configuration.Filters);
                GlobalConfiguration.Configure(WebApiConfig.Register);
                RouteConfig.RegisterRoutes(RouteTable.Routes);

                string path = Path.Combine(HttpRuntime.AppDomainAppPath, @"Configuration\WebPortalConfiguration.json");

                IWebPortalConfigurationFactory webPortalConfigFactory = new WebPortalConfigurationFactory();
                PortalConfiguration.WebPortalConfigurationManager = webPortalConfigFactory.Create(path);

                // Setup the application assets bundles
                PortalConfiguration.WebPortalConfigurationManager.UpdateBundles(Bundler.Instance).Wait();
            }
            finally
            {
                service = null;
            }

        }
    }
}