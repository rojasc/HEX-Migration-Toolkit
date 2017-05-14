// -----------------------------------------------------------------------
// <copyright file="EnvironmentViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Models
{
    /// <summary>
    /// Represents an environment and the connected Office 365 tenant.
    /// </summary>
    public class EnvironmentViewModel
    {
        /// <summary>
        /// Gets or sets the endpoint address.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the environment.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the environment.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the organization for the environment.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }
    }
}