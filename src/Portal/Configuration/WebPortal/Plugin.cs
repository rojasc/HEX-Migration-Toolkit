// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Configuration.WebPortal
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a Plugin in the web portal.
    /// </summary>
    [JsonObject]
    public class Plugin : PluginDefaults, ICloneable
    {
        /// <summary>
        /// Gets or sets the plugin's default feature.
        /// </summary>
        [JsonProperty]
        public string DefaultFeature { get; set; }

        /// <summary>
        /// Gets or sets the plugin's features.
        /// </summary>
        [JsonProperty]
        public IList<AssetsSegment> Features { get; set; }

        /// <summary>
        /// Gets or sets the plugin Name.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Sets the plugin default property values using the given defaults if these properties were missing.
        /// </summary>
        /// <param name="defaults">The plugin defaults.</param>
        public void SetDefaults(PluginDefaults defaults)
        {
            if (defaults == null)
            {
                throw new ArgumentNullException(nameof(defaults), "defaults cannot be null");
            }

            defaults.Validate();

            if (string.IsNullOrWhiteSpace(AlternateColor))
            {
                AlternateColor = defaults.AlternateColor;
            }

            if (string.IsNullOrWhiteSpace(Color))
            {
                Color = defaults.Color;
            }

            if (string.IsNullOrWhiteSpace(DisplayName))
            {
                DisplayName = defaults.DisplayName;
            }

            if (string.IsNullOrWhiteSpace(Image))
            {
                Image = defaults.Image;
            }
        }

        /// <summary>
        /// Validates the plugin's settings.
        /// </summary>
        /// <param name="featureHashtable">A feature hash table useful for cross referencing duplications in other plugins.</param>
        /// <exception cref="InvalidOperationException">If the plugin settings are invalid.</exception>
        public override void Validate(IDictionary<string, int> featureHashtable = null)
        {
            base.Validate(featureHashtable);

            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new InvalidOperationException("Name not set");
            }

            if (Features == null || Features.Count <= 0)
            {
                throw new InvalidOperationException("Features not set");
            }

            if (string.IsNullOrWhiteSpace(DefaultFeature))
            {
                // if no default feature is set, set it to the first feature
                DefaultFeature = Features[0].Name;
            }

            foreach (AssetsSegment feature in Features)
            {
                if (string.IsNullOrWhiteSpace(feature.Name))
                {
                    throw new InvalidOperationException("A feature name can't be null or empty");
                }

                foreach (Assets featureAssets in feature.Assets)
                {
                    featureAssets.Validate();
                }

                if (featureHashtable.ContainsKey(feature.Name))
                {
                    throw new InvalidOperationException($"Duplicate feature: {feature.Name}");
                }

                featureHashtable[feature.Name] = 0;
            }

            if (!featureHashtable.ContainsKey(DefaultFeature))
            {
                throw new InvalidOperationException("Invalid default feature. Please make sure it is addded to feature list");
            }
        }

        /// <summary>
        /// Clones the plugin instance.
        /// </summary>
        /// <returns>A deep copy of the plugin instance.</returns>
        public new object Clone()
        {
            return new Plugin()
            {
                AlternateColor = AlternateColor,
                Color = Color,
                CssClass = CssClass,
                DefaultFeature = DefaultFeature,
                DisplayName = DisplayName,
                Features = Features.Clone(),
                Image = Image,
                Name = Name
            };
        }
    }
}