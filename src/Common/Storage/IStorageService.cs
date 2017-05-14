// -----------------------------------------------------------------------
// <copyright file="IStorageService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using WindowsAzure.Storage.Table;

    /// <summary>
    /// Represents a way to interact with storage.
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="T">The type of entity to be deleted.</typeparam>
        /// <param name="tableName">A string containing the name of the table.</param>
        /// <param name="entity">The <see cref="TableEntity"/> object to be deleted.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteEntityAsync<T>(string tableName, T entity) where T : TableEntity, new();

        /// <summary>
        /// Deletes the entities from the specified table.
        /// </summary>
        /// <typeparam name="T">The type of entity to be deleted.</typeparam>
        /// <param name="tableName">A string containing the name of the table.</param>
        /// <param name="entities">A list of entities to be deleted.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteEntitiesAsync<T>(string tableName, List<T> entities) where T : TableEntity, new();

        /// <summary>
        /// Gets all entities that match the specified query.
        /// </summary>
        /// <typeparam name="T">The type of entity to be retrieved.</typeparam>
        /// <param name="tableName">A string containing the name of the storage table.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A list of entities that match the specified query.</returns>
        Task<List<T>> GetEntitiesAsync<T>(string tableName, Func<T, bool> query) where T : TableEntity, new();

        /// <summary>
        /// Gets the entity from the specified table.
        /// </summary>
        /// <typeparam name="T">The class of type for the entity to retrieve.</typeparam>
        /// <param name="tableName">Name of the table to query.</param>
        /// <param name="partitionKey">A string containing the partition key of the entity to retrieve.</param>
        /// <param name="rowKey">A string containing the row key of the entity to retrieve.</param>
        /// <returns>An entity that represents the requested entity.</returns>
        Task<T> GetEntityAsync<T>(string tableName, string partitionKey, string rowKey) where T : TableEntity, new();

        /// <summary>
        /// Writes the entities to the specified storage table.
        /// </summary>
        /// <typeparam name="T">The type of entity to be written to the storage table.</typeparam>
        /// <param name="tableName">Name of the table where to write the entity.</param>
        /// <param name="entities">A list of entities to be written to the table.</param>
        /// <returns>An instance of <see cref="Task"/> that represents asyncrhonous.</returns>
        Task WriteBatchToTableAsync<T>(string tableName, List<T> entities) where T : TableEntity, new();

        /// <summary>
        /// Writes the entity to the specified storage table.
        /// </summary>
        /// <typeparam name="T">The type of entity to be written to the storage table.</typeparam>
        /// <param name="tableName">Name of the table where to write the entity.</param>
        /// <param name="entity">Entity to be written to the table.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task WriteToTableAsync<T>(string tableName, T entity) where T : TableEntity, new();
    }
}