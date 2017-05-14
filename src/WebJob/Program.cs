// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.WebJob
{
    using System.Diagnostics;
    using Azure.WebJobs;
    using Azure.WebJobs.ServiceBus;
    using Common;

    /// <summary>
    /// Defines the core objects and properties for the WebJob.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Provides access to core services. 
        /// </summary>
        private static IMigrationService service;

        /// <summary>
        /// Gets a reference to the migration service.
        /// </summary>
        internal static IMigrationService Service => (service) ?? (service = new MigrationService());

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        internal static void Main()
        {
            JobHost host;
            JobHostConfiguration config;
            ServiceBusConfiguration serviceBusConfig;

            try
            {
                ApplicationInsights.Extensibility.TelemetryConfiguration.Active.InstrumentationKey =
                    Service.Configuration.InstrumentationKey;

                config = new JobHostConfiguration(Service.Configuration.StorageConnectionString);

                serviceBusConfig = new ServiceBusConfiguration
                {
                    ConnectionString = Service.Configuration.ServiceBusConnectionString
                };

                config.Queues.MaxDequeueCount = 3;
                config.Queues.BatchSize = 10;
                config.Queues.NewBatchThreshold = 15;
                config.Tracing.ConsoleLevel = TraceLevel.Verbose;
                config.UseServiceBus(serviceBusConfig);
                config.UseTimers();

                host = new JobHost(config);
                host.RunAndBlock();
            }
            finally
            {
                config = null;
                host = null;
                serviceBusConfig = null;
            }
        }
    }
}