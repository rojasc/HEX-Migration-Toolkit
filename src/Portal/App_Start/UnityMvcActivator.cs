// -----------------------------------------------------------------------
// <copyright file="UnityMvcActivator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Microsoft.Hex.Migration.Toolkit.Portal.UnityWebActivator), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(Microsoft.Hex.Migration.Toolkit.Portal.UnityWebActivator), "Shutdown")]

namespace Microsoft.Hex.Migration.Toolkit.Portal
{
    using System.Linq;
    using System.Web.Mvc;
    using Practices.Unity;
    using Practices.Unity.Mvc;

    /// <summary>Provides the bootstrapping for integrating Unity with ASP.NET MVC.</summary>
    public static class UnityWebActivator
    {
        /// <summary>Integrates Unity when the application starts.</summary>
        public static void Start() 
        {
            IUnityContainer container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        /// <summary>Disposes the Unity container when the application is shut down.</summary>
        public static void Shutdown()
        {
            IUnityContainer container = UnityConfig.GetConfiguredContainer();
            container.Dispose();
        }
    }
}