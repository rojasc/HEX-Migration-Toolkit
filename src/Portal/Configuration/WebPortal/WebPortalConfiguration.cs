// -----------------------------------------------------------------------
// <copyright file="WebPortalConfiguration.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Configuration.WebPortal
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Holds the Web portal configuration.
    /// </summary>
    public class WebPortalConfiguration
    {
        /// <summary>
        /// The dependencies assets segment.
        /// </summary>
        private AssetsSegment dependencies;

        /// <summary>
        /// Gets or sets the portal dependencies.
        /// </summary>
        public AssetsSegment Dependencies
        {
            get
            {
                return dependencies;
            }

            set
            {
                dependencies = value;
                dependencies.Name = "Dependencies";
            }
        }

        /// <summary>
        /// Gets or sets the portal core assets.
        /// </summary>
        public CoreSegment Core { get; set; }

        /// <summary>
        /// Gets or sets the portal services assets.
        /// </summary>
        public IEnumerable<AssetsSegment> Services { get; set; }

        /// <summary>
        /// Gets or sets the portal views assets.
        /// </summary>
        public IEnumerable<AssetsSegment> Views { get; set; }

        /// <summary>
        /// Gets or sets the portal plugins assets.
        /// </summary>
        public PluginsSegment Plugins { get; set; }

        /// <summary>
        /// Gets or sets the portal configuration.
        /// </summary>
        public Dictionary<string, dynamic> Configuration { get; set; }

        /// <summary>
        /// Processes the configuration and ensures it is valid.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the configuration is invalid.</exception>
        public void Process()
        {
            Dependencies?.Validate();

            if (Core == null || (Core.Startup == null && Core.NonStartup == null))
            {
                throw new InvalidOperationException("Portal core not present.");
            }

            Core.Startup?.Validate();

            Core.NonStartup?.Validate();

            if (Services != null)
            {
                foreach (AssetsSegment service in Services)
                {
                    service.Validate();
                }
            }

            if (Views != null)
            {
                foreach (AssetsSegment view in Views)
                {
                    view.Validate();
                }
            }

            Plugins.Validate();
        }

        /// <summary>
        /// A container for portal core asset segments.
        /// </summary>
        public class CoreSegment
        {
            /// <summary>
            /// The start up assets segment.
            /// </summary>
            private AssetsSegment startup;

            /// <summary>
            /// The non start up assets segment.
            /// </summary>
            private AssetsSegment nonStartup;

            /// <summary>
            /// Gets or sets startup assets.
            /// </summary>
            public AssetsSegment Startup
            {
                get
                {
                    return startup;
                }

                set
                {
                    startup = value;
                    startup.Name = "Startup";
                }
            }

            /// <summary>
            /// Gets or sets non startup assets.
            /// </summary>
            public AssetsSegment NonStartup
            {
                get
                {
                    return nonStartup;
                }

                set
                {
                    nonStartup = value;
                    nonStartup.Name = "Nonstartup";
                }
            }
        }
    }
}