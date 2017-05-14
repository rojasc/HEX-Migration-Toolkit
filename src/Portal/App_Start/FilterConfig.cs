// -----------------------------------------------------------------------
// <copyright file="FilterConfig.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal
{
    using System.Web.Http.Filters;
    using System.Web.Mvc;
    using Logic; 

    /// <summary>
    /// Provides the ability to configure filtering.
    /// </summary>
    public class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The global filter collection.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new Filters.Mvc.AuthenticationFilter());
            filters.Add(new TelemetryHandleErrorAttribute());
        }

        /// <summary>
        /// Registers the specified filters.
        /// </summary>
        /// <param name="filters">A collection of HTTP filters.</param>
        public static void RegisterWebApiFilters(HttpFilterCollection filters)
        {
            filters.Add(new Filters.WebApi.AuthenticationFilter());
        }
    }
}