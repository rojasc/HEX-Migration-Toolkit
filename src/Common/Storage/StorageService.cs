// -----------------------------------------------------------------------
// <copyright file="StorageService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using WindowsAzure.Storage;
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// Provides the ability to interact with an Azure storage account.
    /// </summary>
    public class StorageService : IStorageService
    {
        /// <summary>
        /// An instance of the <see cref="CloudStorageAccount"/> class.
        /// </summary>
        private static CloudStorageAccount storageAccount;

        /// <summary>
        /// An instance of the <see cref="CloudTableClient"/> class.
        /// </summary>
        private static CloudTableClient tableClient;

        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IMigrationService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageService"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public StorageService(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Gets a reference to an instance of the <see cref="CloudStorageAccount"/> class.
        /// </summary>
        private CloudStorageAccount StorageAccount => storageAccount ?? (storageAccount = CloudStorageAccount.Parse(service.Configuration.StorageConnectionString));

        /// <summary>
        /// Gets a reference to an instance of the <see cref="CloudTableClient"/> class.
        /// </summary>
        private CloudTableClient TableClient => tableClient ?? (tableClient = StorageAccount.CreateCloudTableClient());

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T">The type of entity to be deleted.</typeparam>
        /// <param name="tableName">A string containing the name of the table.</param>
        /// <param name="entity">The <see cref="TableEntity"/> object to be deleted.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="tableName"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        public async Task DeleteEntityAsync<T>(string tableName, T entity) where T : TableEntity, new()
        {
            CloudTable table;
            TableOperation operation;

            tableName.AssertNotEmpty(nameof(tableName));
            entity.AssertNotNull(nameof(entity));

            try
            {
                table = tableClient.GetTableReference(tableName);
                entity.ETag = "*";

                operation = TableOperation.Delete(entity);
                await table.ExecuteAsync(operation);
            }
            finally
            {
                operation = null;
                table = null;
            }
        }

        /// <summary>
        /// Deletes the entities from the specified table.
        /// </summary>
        /// <typeparam name="T">The type of entity to be deleted.</typeparam>
        /// <param name="tableName">A string containing the name of the table.</param>
        /// <param name="entities">A list of entities to be deleted.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="tableName"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entities"/> is null.
        /// </exception>
        public async Task DeleteEntitiesAsync<T>(string tableName, List<T> entities) where T : TableEntity, new()
        {
            CloudTable table;
            TableBatchOperation operation;

            entities.AssertNotNull(nameof(entities));
            tableName.AssertNotEmpty(nameof(tableName));

            try
            {
                table = TableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();

                operation = new TableBatchOperation();

                foreach (T entity in entities)
                {
                    entity.ETag = "*";
                    operation.Delete(entity);

                    if (operation.Count == 100)
                    {
                        await table.ExecuteBatchAsync(operation);
                        operation = new TableBatchOperation();
                    }
                }

                if (operation.Count > 0)
                {
                    await table.ExecuteBatchAsync(operation);
                }
            }
            finally
            {
                operation = null;
                table = null;
            }
        }

        /// <summary>
        /// Gets the entity from the specified table.
        /// </summary>
        /// <typeparam name="T">The class of type for the entity to retrieve.</typeparam>
        /// <param name="tableName">Name of the table to query.</param>
        /// <param name="partitionKey">A string containing the partition key of the entity to retrieve.</param>
        /// <param name="rowKey">A string containing the row key of the entity to retrieve.</param>
        /// <returns>An entity that represents the requested entity.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="tableName"/> is empty or null.
        /// or
        /// <paramref name="partitionKey"/> is empty or null.
        /// or
        /// <paramref name="rowKey"/> is empty or null.
        /// </exception>
        public async Task<T> GetEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : TableEntity, new()
        {
            CloudTable table;
            TableOperation operation;
            TableResult result;

            tableName.AssertNotEmpty(nameof(tableName));
            partitionKey.AssertNotEmpty(nameof(partitionKey));
            rowKey.AssertNotEmpty(nameof(rowKey));

            try
            {
                table = TableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();

                operation = TableOperation.Retrieve<T>(partitionKey, rowKey);
                result = await table.ExecuteAsync(operation);

                return result.Result as T;
            }
            finally
            {
                operation = null;
                table = null;
            }
        }

        /// <summary>
        /// Gets all entities that match the specified query.
        /// </summary>
        /// <typeparam name="T">The type of entity to be retrieved.</typeparam>
        /// <param name="tableName">A string containing the name of the storage table.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A list of entities that match the specified query.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="tableName"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="predicate"/> is null.
        /// </exception>
        public async Task<List<T>> GetEntitiesAsync<T>(string tableName, Func<T, bool> predicate) where T : TableEntity, new()
        {
            CloudTable table;

            tableName.AssertNotEmpty(nameof(tableName));
            predicate.AssertNotNull(nameof(predicate));

            try
            {
                table = TableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();

                return table.CreateQuery<T>().Where(predicate).ToList();
            }
            finally
            {
                table = null;
            }
        }

        /// <summary>
        /// Writes the entities to the specified storage table.
        /// </summary>
        /// <typeparam name="T">The type of entity to be written to the storage table.</typeparam>
        /// <param name="tableName">Name of the table where to write the entity.</param>
        /// <param name="entities">A list of entities to be written to the table.</param>
        /// <returns>An instance of <see cref="Task"/> that represents asyncrhonous.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="tableName"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entities"/> is empty.
        /// </exception>
        public async Task WriteBatchToTableAsync<T>(string tableName, List<T> entities) where T : TableEntity, new()
        {
            CloudTable table;
            TableBatchOperation operation;

            entities.AssertNotNull(nameof(entities));
            tableName.AssertNotEmpty(nameof(tableName));

            try
            {
                operation = new TableBatchOperation();
                table = TableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();

                foreach (T entity in entities)
                {
                    operation.InsertOrReplace(entity);

                    if (operation.Count == 100)
                    {
                        await table.ExecuteBatchAsync(operation);
                        operation = new TableBatchOperation();
                    }
                }

                if (operation.Count > 0)
                {
                    await table.ExecuteBatchAsync(operation);
                }
            }
            finally
            {
                operation = null;
                table = null;
            }
        }

        /// <summary>
        /// Writes the entity to the specified storage table.
        /// </summary>
        /// <typeparam name="T">The type of entity to be written to the storage table.</typeparam>
        /// <param name="tableName">Name of the table where to write the entity.</param>
        /// <param name="entity">Entity to be written to the table.</param>
        /// <returns>
        /// A <see cref="Task"/> that represents the asynchronous operation.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="tableName"/> is either empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="StorageOperationException">
        /// The PartitionKey and RowKey must be specified.
        /// </exception>
        /// <remarks>
        /// If there is an entity with a matching PartitionKey and RowKey then the entity 
        /// will be replaced. Otherwise, the entity is inserted into the table as new entity.
        /// </remarks>
        public async Task WriteToTableAsync<T>(string tableName, T entity) where T : TableEntity, new()
        {
            CloudTable table;
            TableOperation operation;

            tableName.AssertNotEmpty(nameof(tableName));
            entity.AssertNotNull(nameof(entity));

            if (string.IsNullOrEmpty(entity.PartitionKey))
            {
                throw new StorageOperationException(Resources.NoPartitionKeyException);
            }

            if (string.IsNullOrEmpty(entity.RowKey))
            {
                throw new StorageOperationException(Resources.NoRowKeyException);
            }

            try
            {
                table = TableClient.GetTableReference(tableName);
                await table.CreateIfNotExistsAsync();

                operation = TableOperation.InsertOrReplace(entity);
                await table.ExecuteAsync(operation);
            }
            catch (Exception ex)
            {
                service.Telemetry.TrackException(ex);
            }
            finally
            {
                operation = null;
                table = null;
            }
        }
    }
}