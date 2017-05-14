// -----------------------------------------------------------------------
// <copyright file="MigrationBatchEntity.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common.Storage
{
    using System;
    using WindowsAzure.Storage.Table;

    public class MigrationBatchEntity : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationBatchEntity"/> class.
        /// </summary>
        public MigrationBatchEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationBatchEntity"/> class.
        /// </summary>
        /// <param name="partitionKey">Partition key for the entity.</param>
        /// <param name="rowKey">Row key for the entity.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="partitionKey"/> is null.
        /// or
        /// <paramref name="rowKey"/> is null.
        /// </exception>
        public MigrationBatchEntity(string partitionKey, string rowKey)
        {
            partitionKey.AssertNotEmpty(nameof(partitionKey));
            rowKey.AssertNotEmpty(nameof(rowKey));

            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        /// <summary>
        /// Gets or sets the customer identifier that owns the migration batch.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the name for the migration batch.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the start time for the migration batch.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating whether or not the migration batch has been started.
        /// </summary>
        public bool Started { get; set; }

        /// <summary>
        /// Gets or sets the target delivery domain.
        /// </summary>
        public string TargetDeliveryDomain { get; set; }
    }
}