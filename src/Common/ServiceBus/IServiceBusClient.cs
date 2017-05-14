// -----------------------------------------------------------------------
// <copyright file="IServiceBusClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------


namespace Microsoft.Hex.Migration.Toolkit.Common.ServiceBus
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a client that can interact with Azure Service Bus.
    /// </summary>
    public interface IServiceBusClient
    {
        /// <summary>
        /// Writes the entity to specified Service Bus queue.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="queueName">Name of the queue.</param>
        /// <param name="entity">Entity to be written to the queue.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task WriteToQueueAsync<T>(string queueName, T entity);
    }
}