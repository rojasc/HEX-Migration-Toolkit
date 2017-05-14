// -----------------------------------------------------------------------
// <copyright file="EnvironmentsViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of environment view models.
    /// </summary>
    public class EnvironmentsViewModel
    {
        /// <summary>
        /// Gets or sets a list of configured environments.
        /// </summary>
        public List<EnvironmentViewModel> Environments { get; set; }
    }
}