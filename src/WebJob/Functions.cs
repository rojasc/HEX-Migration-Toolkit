// -----------------------------------------------------------------------
// <copyright file="Functions.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.WebJob
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using Automation;
    using Azure.WebJobs;
    using Common;
    using Common.Storage;
    using ServiceBus.Messaging;

    /// <summary>
    /// Defines the WebJob functions that will be invoked according to the definition.
    /// </summary>
    public class Functions
    {
        /// <summary>
        /// Process a message that was written to the environments queue.
        /// </summary>
        /// <param name="message">An instance of <see cref="BrokeredMessage"/> that represents the message written to the queue.</param>
        /// <param name="log"></param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task ProcessEnvironmentQueueMessageAsync([ServiceBusTrigger(MigrationConstants.EnvironmentQueueName)] BrokeredMessage message, TextWriter log)
        {
            EnvironmentEntity entity;
            List<MailboxEntity> mailboxes;
            IMigrationManager manager;

            try
            {
                entity = message.GetBody<EnvironmentEntity>();
                manager = new MigrationManager(Program.Service);

                mailboxes = await manager.GetMailboxesAsync(entity);

                await manager.CreateMigrationEndpointAsync(entity);
                await Program.Service.Storage.WriteBatchToTableAsync(MigrationConstants.MailboxTableName, mailboxes);

                log.WriteLine(message);
            }
            finally
            {
                entity = null;
                manager = null;
            }
        }

        /// <summary>
        /// Process a message that was written to the migration batches queue.
        /// </summary>
        /// <param name="message">An instance of <see cref="BrokeredMessage"/> that represents the message written to the queue.</param>
        /// <param name="log"></param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task ProcessMigrationBatchQueueMessageAsync([ServiceBusTrigger(MigrationConstants.MigrationBatchQueueName)] BrokeredMessage message, TextWriter log)
        {
            MigrationBatchEntity entity;
            IMigrationManager manager;

            try
            {
                entity = message.GetBody<MigrationBatchEntity>();
                manager = new MigrationManager(Program.Service);

                await manager.CreateMigrationBatchAsync(entity);
            }
            finally
            {
                entity = null;
                manager = null;
            }
        }

        /// <summary>
        /// Processes a message that was written the migration batch delete queue.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="log"></param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task ProcessMigrationBatchDeleteQueueMessageAsync([ServiceBusTrigger(MigrationConstants.MigrationBatchDeleteQueueName)] BrokeredMessage message, TextWriter log)
        {
            MigrationBatchEntity entity;
            IMigrationManager manager;

            try
            {
                entity = message.GetBody<MigrationBatchEntity>();
                manager = new MigrationManager(Program.Service);

                await manager.MigrationBatchDeleteAsync(entity);
            }
            finally
            {
                entity = null;
                manager = null;
            }
        }

        /// <summary>
        /// Processes a message that was written to the migration batch start queue.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="log"></param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task ProcessMigrationBatchStartQueueMessageAsync([ServiceBusTrigger(MigrationConstants.MigrationBatchStartQueueName)] BrokeredMessage message, TextWriter log)
        {
            MigrationBatchEntity entity;
            IMigrationManager manager;

            try
            {
                entity = message.GetBody<MigrationBatchEntity>();
                manager = new MigrationManager(Program.Service);

                await manager.MigrationBatchStartAsync(entity);
            }
            finally
            {
                entity = null;
                manager = null;
            }
        }

        /// <summary>
        /// Queues any migration batch that should be started.
        /// </summary>
        /// <param name="timerInfo">Timer schedule information for the job.</param>
        /// <param name="log"></param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task QueueMigrationBatchAsync([TimerTrigger("0 0 * * * *")]TimerInfo timerInfo, TextWriter log)
        {
            List<MigrationBatchEntity> batches;

            try
            {
                batches = await Program.Service.Storage.GetEntitiesAsync<MigrationBatchEntity>(
                    MigrationConstants.MigrationBatchTable,
                    m => m.Started == false && m.StartTime >= DateTime.Now.AddHours(-1) && m.StartTime <= DateTime.Now);

                foreach (MigrationBatchEntity entity in batches)
                {
                    await Program.Service.ServiceBus.WriteToQueueAsync(MigrationConstants.MigrationBatchStartQueueName, entity);
                }
            }
            finally
            {
                batches = null;
            }
        }
    }
}