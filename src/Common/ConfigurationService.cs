// -----------------------------------------------------------------------
// <copyright file="ConfigurationService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;

    /// <summary>
    /// Provides the ability to quickly access various configurations.
    /// </summary>
    public class ConfigurationService : IConfigurationService
    {
        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IMigrationService service;

        /// <summary>
        /// Initialize a new instance of the <see cref="ConfigurationService"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public ConfigurationService(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Gets the Azure Active Directory endpoint address.
        /// </summary>
        public string ActiveDirectoryEndpoint => ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"];

        /// <summary>
        /// Gets the application identifier for the Azure AD application.
        /// </summary>
        public string ApplicationId => ConfigurationManager.AppSettings["ApplicationId"];

        /// <summary>
        /// Gets the application secret for the Azure AD application.
        /// </summary>
        public string ApplicationSecret => GetConfigurationValue("ApplicationSecret");

        /// <summary>
        /// Gets the tenant identifier for the Azure AD application.
        /// </summary>
        public string ApplicationTenantId => ConfigurationManager.AppSettings["ApplicationTenantId"];

        /// <summary>
        /// Gets the password used to access Exchange Online through PowerShell remoting.
        /// </summary>
        public string ExchangeOnlinePassword => GetConfigurationValue("ExchangeOnlinePassword");

        /// <summary>
        /// Gets the username used to access Exchange Online through PowerShell remoting.
        /// </summary>
        public string ExchangeOnlineUsername => ConfigurationManager.AppSettings["ExchangeOnlineUsername"];

        /// <summary>
        /// Gets the Exchange schema URI value.
        /// </summary>
        public string ExchangeSchemaUri => ConfigurationManager.AppSettings["ExchangeSchemaUri"];

        /// <summary>
        /// Endpoint address for Microsoft Graph.
        /// </summary>
        public string GraphEndpoint => ConfigurationManager.AppSettings["GraphEndpoint"];

        /// <summary>
        /// Gets the Application Insights instrumentation key.
        /// </summary>
        public string InstrumentationKey => ConfigurationManager.AppSettings["InstrumentationKey"];

        /// <summary>
        /// Gets the Partner Center application identifier.
        /// </summary>
        public string PartnerCenterApplicationId => ConfigurationManager.AppSettings["PartnerCenterApplicationId"];

        /// <summary>
        /// Gets the Key Vault application tenant identifier.
        /// </summary>
        public string KeyVaultApplicationId => ConfigurationManager.AppSettings["KeyVaultApplicationId"];

        /// <summary>
        /// Gets the Key Vault application secret.
        /// </summary>
        public string KeyVaultApplicationSecret => ConfigurationManager.AppSettings["KeyVaultApplicationSecret"];

        /// <summary>
        /// Gets the Key Vault application tenant identifier.
        /// </summary>
        public string KeyVaultApplicationTenantId => ConfigurationManager.AppSettings["KeyVaultApplicationTenantId"];

        /// <summary>
        /// Gets the base address for the instance of Key Vault.
        /// </summary>
        public string KeyVaultBaseAddress => ConfigurationManager.AppSettings["KeyVaultBaseAddress"];

        /// <summary>
        /// Gets the Partner Center application secret.
        /// </summary>
        public string PartnerCenterApplicationSecret => this.GetConfigurationValue("PartnerCenterApplicationSecret");

        /// <summary>
        /// Gets the Partner Center application tenant identifier.
        /// </summary>
        public string PartnerCenterApplicationTenantId => ConfigurationManager.AppSettings["PartnerCenterApplicationTenantId"];

        /// <summary>
        /// Gets the Partner Center endpoint address.
        /// </summary>
        public string PartnerCenterEndpoint => ConfigurationManager.AppSettings["PartnerCenterEndpoint"];

        /// <summary>
        /// Gets the Azure Redis Cache connection string.
        /// </summary>
        public string RedisCacheConnectionString => ConfigurationManager.AppSettings["RedisCacheConnectionString"];

        /// <summary>
        /// Gets the Azure Service Bus connection string.
        /// </summary>
        public string ServiceBusConnectionString => GetConfigurationValue("ServiceBusConnectionString");

        /// <summary>
        /// Gets the Azure Storage Account connection string.
        /// </summary>
        public string StorageConnectionString => GetConfigurationValue("StorageConnectionString");

        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="identifier">Identifier of the resource being requested.</param>
        /// <returns>A string represented the value of the configuration.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// </exception>
        private string GetConfigurationValue(string identifier)
        {
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            string value;

            identifier.AssertNotNull(nameof(identifier));

            try
            {
                startTime = DateTime.Now;

                value = service.Vault.Get(identifier);

                if (string.IsNullOrEmpty(value))
                {
                    value = ConfigurationManager.AppSettings[identifier];
                }

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Identifier", identifier }
                };

                service.Telemetry.TrackEvent("Configuration/GetConfigurationValue", eventProperties, eventMetrics);

                return value;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
            }
        }
    }
}
