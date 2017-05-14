// -----------------------------------------------------------------------
// <copyright file="EnvironmentEntity.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common.Storage
{
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// Represents an environment object stored in a storage table.
    /// </summary>
    public class EnvironmentEntity : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentEntity"/> class.
        /// </summary>
        public EnvironmentEntity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentEntity"/> class.
        /// </summary>
        /// <param name="partitionKey">Partition key for the entity.</param>
        /// <param name="rowKey">Row key for the entity.</param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="partitionKey"/> is null.
        /// or
        /// <paramref name="rowKey"/> is null.
        /// </exception>
        public EnvironmentEntity(string partitionKey, string rowKey)
        {
            partitionKey.AssertNotEmpty(nameof(partitionKey));
            rowKey.AssertNotEmpty(nameof(rowKey));

            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        /// <summary>
        /// Gets or sets the endpoint address.
        /// </summary>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the name of the environment.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the organization for the environment.
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Gets or sets the password key.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }
    }
}