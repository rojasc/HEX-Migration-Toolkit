// -----------------------------------------------------------------------
// <copyright file="IMigrationService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common
{
    using Cache;
    using Security;
    using ServiceBus;
    using Storage;
    using Telemetry;

    /// <summary>
    /// Represents a service that provides access to core services.
    /// </summary>
    public interface IMigrationService
    {
        /// <summary>
        /// Gets the service that provides caching functionality.
        /// </summary>
        ICacheService Cache { get; }

        /// <summary>
        /// Gets a reference to the available configurations.
        /// </summary>
        IConfigurationService Configuration { get; }

        /// <summary>
        /// Gets a reference to the service bus client.
        /// </summary>
        IServiceBusClient ServiceBus { get; }

        /// <summary>
        /// Gets a reference to the storage service.
        /// </summary>
        IStorageService Storage { get; }

        /// <summary>
        /// Gets a reference to the telemetry service.
        /// </summary>
        ITelemetryProvider Telemetry { get; }

        /// <summary>
        /// Gets a reference to the token management service.
        /// </summary>
        ITokenManagement TokenManagement { get; }

        /// <summary>
        /// Gets a reference to the vault service. 
        /// </summary>
        IVaultService Vault { get; }
    }
}