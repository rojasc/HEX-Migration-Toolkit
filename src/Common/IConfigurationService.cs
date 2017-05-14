// -----------------------------------------------------------------------
// <copyright file="IConfigurationService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common
{
    /// <summary>
    /// Represents the ability to obtain various configuration information.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// Gets the Azure Active Directory endpoint address.
        /// </summary>
        string ActiveDirectoryEndpoint { get; }

        /// <summary>
        /// Gets the application identifier for the Azure AD application.
        /// </summary>
        string ApplicationId { get; }

        /// <summary>
        /// Gets the application secret for the Azure AD application.
        /// </summary>
        string ApplicationSecret { get; }

        /// <summary>
        /// Gets the tenant identifier for the Azure AD application.
        /// </summary>
        string ApplicationTenantId { get; }

        /// <summary>
        /// Gets the password used to access Exchange Online through PowerShell remoting.
        /// </summary>
        string ExchangeOnlinePassword { get; }

        /// <summary>
        /// Gets the username used to access Exchange Online through PowerShell remoting.
        /// </summary>
        string ExchangeOnlineUsername { get; }

        /// <summary>
        /// Gets the Exchange schema URI value.
        /// </summary>
        string ExchangeSchemaUri { get; }

        /// <summary>
        /// Endpoint address for Microsoft Graph.
        /// </summary>
        string GraphEndpoint { get; }

        /// <summary>
        /// Gets the Application Insights instrumentation key.
        /// </summary>
        string InstrumentationKey { get; }

        /// <summary>
        /// Gets the Key Vault application identifier.
        /// </summary>
        string KeyVaultApplicationId { get; }

        /// <summary>
        /// Gets the Key Vault application secret.
        /// </summary>
        string KeyVaultApplicationSecret { get; }

        /// <summary>
        /// Gets the Key Vault application tenant identifier.
        /// </summary>
        string KeyVaultApplicationTenantId { get; }

        /// <summary>
        /// Gets the base address for the instance of Key Vault.
        /// </summary>
        string KeyVaultBaseAddress { get; }

        /// <summary>
        /// Gets the Partner Center application identifier.
        /// </summary>
        string PartnerCenterApplicationId { get; }

        /// <summary>
        /// Gets the Partner Center application secret.
        /// </summary>
        string PartnerCenterApplicationSecret { get; }

        /// <summary>
        /// Gets the Partner Center application tenant identifier.
        /// </summary>
        string PartnerCenterApplicationTenantId { get; }

        /// <summary>
        /// Gets the Partner Center endpoint address.
        /// </summary>
        string PartnerCenterEndpoint { get; }

        /// <summary>
        /// Gets the Azure Redis Cache connection string.
        /// </summary>
        string RedisCacheConnectionString { get; }

        /// <summary>
        /// Gets the Azure Service Bus connection string.
        /// </summary>
        string ServiceBusConnectionString { get; }

        /// <summary>
        /// Gets the Azure Storage Account connection string.
        /// </summary>
        string StorageConnectionString { get; }
    }
}