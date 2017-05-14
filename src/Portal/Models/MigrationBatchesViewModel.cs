// -----------------------------------------------------------------------
// <copyright file="MigrationBatchesViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of <see cref="MigrationBatchViewModel"/>s.
    /// </summary>
    public class MigrationBatchesViewModel
    {
        /// <summary>
        /// Gets or sets the list of migration batches.
        /// </summary>
        public List<MigrationBatchViewModel> MigrationBatches { get; set; }
    }
}