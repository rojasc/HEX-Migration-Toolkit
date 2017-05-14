// -----------------------------------------------------------------------
// <copyright file="MigrationService.cs" company="Microsoft">
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
    /// Provides access to core services.
    /// </summary>
    public class MigrationService : IMigrationService
    {
        /// <summary>
        /// Provides the ability to cache often used objects. 
        /// </summary>
        private static ICacheService cache;

        /// <summary>
        /// Provides the ability to access various configurations.
        /// </summary>
        private static IConfigurationService configuration;

        /// <summary>
        /// Provides the ability to interact with Azure Service Bus.
        /// </summary>
        private static IServiceBusClient serviceBus;

        /// <summary>
        /// Provides the ability to interact with Azure Storage.
        /// </summary>
        private static IStorageService storage;

        /// <summary>
        /// Provides the ability to track telemetry data.
        /// </summary>
        private static ITelemetryProvider telemetry;

        /// <summary>
        /// Provides the ability to manage access tokens.
        /// </summary>
        private static ITokenManagement tokenManagement;

        /// <summary>
        /// Provides the ability to retrieve and store data in a secure vault.
        /// </summary>
        private static IVaultService vault;

        /// <summary>
        /// Gets the service that provides caching functionality.
        /// </summary>
        public ICacheService Cache => cache ?? (cache = new CacheService(this));

        /// <summary>
        /// Gets a reference to the available configurations.
        /// </summary>
        public IConfigurationService Configuration => (configuration) ?? (configuration = new ConfigurationService(this));

        /// <summary>
        /// Gets a reference to the service bus client.
        /// </summary>
        public IServiceBusClient ServiceBus => (serviceBus) ?? (serviceBus = new ServiceBusClient(this));

        /// <summary>
        /// Gets a reference to the storage service.
        /// </summary>
        public IStorageService Storage => (storage) ?? (storage = new StorageService(this));

        /// <summary>
        /// Gets the telemetry service reference.
        /// </summary>
        public ITelemetryProvider Telemetry
        {
            get
            {
                if (telemetry != null)
                {
                    return telemetry;
                }

                if (string.IsNullOrEmpty(Configuration.InstrumentationKey))
                {
                    telemetry = new EmptyTelemetryProvider();
                }
                else
                {
                    telemetry = new ApplicationInsightsTelemetryProvider();
                }

                return telemetry;
            }
        }

        /// <summary>
        /// Gets the a reference to the token management service.
        /// </summary>
        public ITokenManagement TokenManagement => (tokenManagement) ?? (tokenManagement = new TokenManagement(this));

        /// <summary>
        /// Gets a reference to the vault service.
        /// </summary>
        public IVaultService Vault => (vault) ?? (vault = new VaultService(this));
    }
}