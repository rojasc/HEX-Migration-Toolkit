// -----------------------------------------------------------------------
// <copyright file="Startup.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[assembly: Microsoft.Owin.OwinStartup(typeof(Microsoft.Hex.Migration.Toolkit.Portal.Startup))]

namespace Microsoft.Hex.Migration.Toolkit.Portal
{
    using global::Owin;

    /// <summary>
    /// Provides methods and properties used to start the application.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Configure the application accordingly.
        /// </summary>
        /// <param name="app">The application to be configured.</param>
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}