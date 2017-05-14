// -----------------------------------------------------------------------
// <copyright file="ServiceBusClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common.ServiceBus
{
    using System.Threading.Tasks;
    using Microsoft.ServiceBus;
    using Microsoft.ServiceBus.Messaging;

    /// <summary>
    /// Provides the ability to interact with the Azure Service Bus service.
    /// </summary>
    public class ServiceBusClient : IServiceBusClient
    {
        /// <summary>
        /// An instance of the <see cref="NamespaceManager"/> class.
        /// </summary>
        private static NamespaceManager manager;

        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IMigrationService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusClient"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public ServiceBusClient(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Gets a reference to an instance of the <see cref="NamespaceManager"/> class.
        /// </summary>
        private NamespaceManager Namespace => (manager) ?? (manager = NamespaceManager.CreateFromConnectionString(service.Configuration.ServiceBusConnectionString));

        /// <summary>
        /// Writes the entity to specified Service Bus queue.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="entity">Entity to be written to the queue.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="queueName"/> is empty or null.
        /// </exception>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        public async Task WriteToQueueAsync<T>(string queueName, T entity)
        {
            BrokeredMessage message;
            QueueClient queue;
            bool exists;

            entity.AssertNotNull(nameof(entity));
            queueName.AssertNotEmpty(nameof(queueName));

            try
            {
                message = new BrokeredMessage(entity);
                queue = QueueClient.CreateFromConnectionString(service.Configuration.ServiceBusConnectionString, queueName);

                exists = await Namespace.QueueExistsAsync(queueName);

                if (!exists)
                {
                    await Namespace.CreateQueueAsync(queueName);
                }

                await queue.SendAsync(message);
            }
            finally
            {
                message = null;
                queue = null;
            }
        }
    }
}