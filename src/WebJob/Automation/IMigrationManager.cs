// -----------------------------------------------------------------------
// <copyright file="IMigrationManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.WebJob.Automation
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common.Storage;

    /// <summary>
    /// Represents the management capability for managing Exchange Online migrations.
    /// </summary>
    public interface IMigrationManager
    {
        /// <summary>
        /// Creates a new migration batch in Exchange Online.
        /// </summary>
        /// <param name="entity">An instance of <see cref="MigrationBatchEntity"/> that represents the new migration batch.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task CreateMigrationBatchAsync(MigrationBatchEntity entity);

        /// <summary>
        /// Creates a new migration endpoint in Exchange Online.
        /// </summary>
        /// <param name="entity">An instance of <see cref="EnvironmentEntity"/> that represents the new migration endpoint.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task CreateMigrationEndpointAsync(EnvironmentEntity entity);

        /// <summary>
        /// Gets list of mailboxes from the on permise environment.
        /// </summary>
        /// <param name="entity">An instance of <see cref="EnvironmentEntity"/> that represents the on permise environment.</param>
        /// <returns>A list of mailboxes from the on permise environment.</returns>
        Task<List<MailboxEntity>> GetMailboxesAsync(EnvironmentEntity entity);

        /// <summary>
        /// Deletes the migration batch in Exchange Online.
        /// </summary>
        /// <param name="entity">An instance of <see cref="MigrationBatchEntity"/> that represents the migration batch to delete.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task MigrationBatchDeleteAsync(MigrationBatchEntity entity);

        /// <summary>
        /// Starts the migration batch in Exchange Online.
        /// </summary>
        /// <param name="entity">An instance of <see cref="MigrationBatchEntity"/> that represents the migration batch to start.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task MigrationBatchStartAsync(MigrationBatchEntity entity);
    }
}