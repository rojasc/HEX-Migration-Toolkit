// -----------------------------------------------------------------------
// <copyright file="PortalConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Manager;

    /// <summary>
    /// Provides access to web portal configurations.
    /// </summary>
    public static class PortalConfiguration
    {
        /// <summary>
        /// A lazy reference to client configuration.
        /// </summary>
        private static readonly Lazy<IDictionary<string, dynamic>> ClientConfig = new Lazy<IDictionary<string, dynamic>>(
            () => WebPortalConfigurationManager.GenerateConfigurationDictionary().Result);

        /// <summary>
        /// Gets the client configuration dictionary. 
        /// </summary>
        public static IDictionary<string, dynamic> ClientConfiguration => ClientConfig.Value;

        /// <summary>
        /// Gets or sets the web portal configuration manager instance.
        /// </summary>
        public static WebPortalConfigurationManager WebPortalConfigurationManager
        {
            get
            {
                return HttpContext.Current.Application["WebPortalConfigurationManager"] as WebPortalConfigurationManager;
            }

            set
            {
                HttpContext.Current.Application["WebPortalConfigurationManager"] = value;
            }
        }
    }
}