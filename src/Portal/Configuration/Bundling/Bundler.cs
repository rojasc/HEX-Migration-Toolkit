// -----------------------------------------------------------------------
// <copyright file="Bundler.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Configuration.Bundling
{
    using System;
    using System.Web.Optimization;

    /// <summary>
    /// Abstracts bundling client files.
    /// </summary>
    public sealed class Bundler
    {
        /// <summary>
        /// The singleton bundler instance.
        /// </summary>
        private static readonly Lazy<Bundler> BundlerInstance = new Lazy<Bundler>(() => new Bundler());

        /// <summary>
        /// The bundles collection.
        /// </summary>
        private readonly BundleCollection bundles;

        /// <summary>
        /// Prevents a default instance of the <see cref="Bundler"/> class from being created.
        /// </summary>
        private Bundler()
        {
            this.bundles = BundleTable.Bundles;
        }

        /// <summary>
        /// Gets the bundler instance.
        /// </summary>
        public static Bundler Instance => BundlerInstance.Value;

        /// <summary>
        /// Clears all previously configured bundles.
        /// </summary>
        public void Clear()
        {
            this.bundles.Clear();
        }

        /// <summary>
        /// Bundles startup assets.
        /// </summary>
        /// <param name="javaScriptFiles">The JS files to include in the startup bundle.</param>
        /// <param name="cssFiles">The CSS files to include in the startup bundle.</param>
        public void BundleStartupAssets(string[] javaScriptFiles, string[] cssFiles)
        {
            bundles.Add(new ScriptBundle("~/StartupClasses/").Include(javaScriptFiles));
            bundles.Add(new StyleBundle("~/StartupStyles/").Include(cssFiles));
        }

        /// <summary>
        /// Bundles non startup assets.
        /// </summary>
        /// <param name="javaScriptFiles">The JS files to include in the non startup bundle.</param>
        /// <param name="cssFiles">The CSS files to include in the non startup bundle.</param>
        public void BundleNonStartupAssets(string[] javaScriptFiles, string[] cssFiles)
        {
            bundles.Add(new ScriptBundle("~/WebPortalClasses/").Include(javaScriptFiles));
            bundles.Add(new StyleBundle("~/WebPortalStyles/").Include(cssFiles));
        }
    }
}