// -----------------------------------------------------------------------
// <copyright file="StandardWebPortalConfigurationManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Configuration.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Bundling;
    using WebPortal;

    /// <summary>
    /// The standard web portal configuration manager. Reads the provided configuration file and serves the client with the configured asset versions.
    /// </summary>
    public class StandardWebPortalConfigurationManager : WebPortalConfigurationManager
    {
        /// <summary>
        /// The aggregated portal startup assets.
        /// </summary>
        private static Lazy<Assets> startupAssets;

        /// <summary>
        /// The aggregated portal non startup assets.
        /// </summary>
        private static Lazy<Assets> nonStartupAssets;

        /// <summary>
        /// Indicates whether the bundles have been generated yet.
        /// </summary>
        private bool isBundlesGenerated;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardWebPortalConfigurationManager"/> class.
        /// </summary>
        /// <param name="configurationFilePath">The web portal configuration file path.</param>
        public StandardWebPortalConfigurationManager(string configurationFilePath)
            : base(configurationFilePath)
        {
            // we only want to generate the assets once since all users of the system will be served the same assets
            startupAssets = new Lazy<Assets>(GenerateStartupAssets);
            nonStartupAssets = new Lazy<Assets>(GenerateNonStartupAssets);
        }

        /// <summary>
        /// Updates the web portal bundle files.
        /// </summary>
        /// <param name="bundler">The bundler instance.</param>
        /// <returns>A task which is complete when the bundles are updated.</returns>
        public override async Task UpdateBundles(Bundler bundler)
        {
            if (isBundlesGenerated)
            {
                // only generate the bundles once since they will remain constant across the web application's life span
                return;
            }

            // call the standard bundling implementation
            await base.UpdateBundles(bundler);

            isBundlesGenerated = true;
        }

        /// <summary>
        /// Aggregates client side files which are needed during the portal start up.
        /// </summary>
        /// <returns>The aggregated startup assets.</returns>
        public override async Task<Assets> AggregateStartupAssets()
        {
            // return the startup assests we had already built
            return await Task.FromResult(startupAssets.Value);
        }

        /// <summary>
        /// Aggregates client side files which are needed once the portal is up and running.
        /// </summary>
        /// <returns>The aggregated non startup assets.</returns>
        public override async Task<Assets> AggregateNonStartupAssets()
        {
            // return the non startup assests we had already built
            return await Task.FromResult(nonStartupAssets.Value);
        }

        /// <summary>
        /// Generates the plugins which will be sent down to the client.
        /// </summary>
        /// <returns>The plugins configuration.</returns>
        public override async Task<PluginsSegment> GeneratePlugins()
        {
            // return the plugin configuration as found in the configuration file
            return (await Task.FromResult(Configuration.Plugins)).Clone() as PluginsSegment;
        }

        /// <summary>
        /// Generates the configuration settings which will be sent down to the client.
        /// </summary>
        /// <returns>A dictionary of configuration settings.</returns>
        public override async Task<Dictionary<string, dynamic>> GenerateConfigurationDictionary()
        {
            // return the configuration as is
            return await Task.FromResult(Configuration.Configuration);
        }

        /// <summary>
        /// Builds the portal startup assets.
        /// </summary>
        /// <returns>The portal startup assets.</returns>
        private Assets GenerateStartupAssets()
        {
            // aggregate asset files in this order: dependency, core startup
            Assets dependencies = Configuration.Dependencies.Assets.GetAssetsByVersion(Configuration.Dependencies.DefaultAssetVersion);
            Assets coreStartup = Configuration.Core.Startup.Assets.GetAssetsByVersion(Configuration.Core.Startup.DefaultAssetVersion);

            return dependencies + coreStartup;
        }

        /// <summary>
        /// Builds the portal non startup assets.
        /// </summary>
        /// <returns>The portal non startup assets.</returns>
        private Assets GenerateNonStartupAssets()
        {
            // aggregate asset files in this order: core non startup, services, views, plugins
            Assets nonStartup = Configuration.Core.NonStartup.Assets.GetAssetsByVersion(Configuration.Core.NonStartup.DefaultAssetVersion);
            Assets services = Configuration.Services.AggregateAssets();
            Assets views = Configuration.Views.AggregateAssets();
            Assets plugins = Configuration.Plugins.Commons.Assets.GetAssetsByVersion(Configuration.Plugins.Commons.DefaultAssetVersion);

            plugins = Configuration.Plugins.Plugins
                .Aggregate(plugins, (current, plugin) => current + plugin.Features.AggregateAssets());

            return nonStartup + services + views + plugins;
        }
    }
}