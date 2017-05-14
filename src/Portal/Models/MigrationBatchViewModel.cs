// -----------------------------------------------------------------------
// <copyright file="MigrationBatchViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a batch of mailboxes to be migrated.
    /// </summary>
    public class MigrationBatchViewModel
    {
        /// <summary>
        /// Gets or sets the environment identifier for the migration batch.
        /// </summary>
        public string EnvironmentId { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the migration batch.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the mailboxes assigned to the migration batch.
        /// </summary>
        public List<MailboxViewModel> Mailboxes { get; set; }

        /// <summary>
        /// Gets or sets the name for the migration batch.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the start time for the migration batch.
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets the target delivery domain for the migration batch.
        /// </summary>
        public string TargetDeliveryDomain { get; set; }
    }
}