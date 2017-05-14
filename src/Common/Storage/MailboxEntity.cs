// -----------------------------------------------------------------------
// <copyright file="MailboxEntity.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common.Storage
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class MailboxEntity : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MailboxEntity"/> class.
        /// </summary>
        public MailboxEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MailboxEntity"/> class.
        /// </summary>
        /// <param name="partitionKey">Partition key for the entity.</param>
        /// <param name="rowKey">Row key for the entity.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="partitionKey"/> is null.
        /// or
        /// <paramref name="rowKey"/> is null.
        /// </exception>
        public MailboxEntity(string partitionKey, string rowKey)
        {
            partitionKey.AssertNotEmpty(nameof(partitionKey));
            rowKey.AssertNotEmpty(nameof(rowKey));

            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        /// <summary>
        /// Gets or sets the display name for the mailbox.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the migration batch this mailbox is associated with.
        /// </summary>
        public string MigrationBatchId { get; set; }

        /// <summary>
        /// Gets or sets the name for the mailbox.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the primary SMTP address for the mailbox.
        /// </summary>
        public string PrimarySmtpAddress { get; set; }

        /// <summary>
        /// Gets or sets the sAMAccountName for the mailbox.
        /// </summary>
        public string SamAccountName { get; set; }

        /// <summary>
        /// Gets or sets the user principal name for the mailbox.
        /// </summary>
        public string UserPrincipalName { get; set; }
    }
}