// -----------------------------------------------------------------------
// <copyright file="VaultService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common.Security
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.KeyVault;
    using Azure.KeyVault.Models;

    /// <summary>
    /// Provides a secure mechanism for retrieving and store information.
    /// </summary>
    /// <seealso cref="IVaultService" />
    public sealed class VaultService : IVaultService
    {
        /// <summary>
        /// Error code returned when a secret is not found in the vault.
        /// </summary>
        private const string NotFoundErrorCode = "SecretNotFound";

        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IMigrationService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="VaultService"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public VaultService(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));

            this.service = service;
        }

        /// <summary>
        /// Gets a value indicating whether the vault service is enabled or not.
        /// </summary>
        public bool IsEnabled => !string.IsNullOrEmpty(service.Configuration.KeyVaultBaseAddress);

        /// <summary>
        /// Gets the specified entity from the vault. 
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be retrieved.</param>
        /// <returns>The value retrieved from the vault.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// </exception>
        public string Get(string identifier)
        {
            identifier.AssertNotEmpty(nameof(identifier));

            return SynchronousExecute(() => GetAsync(identifier));
        }

        /// <summary>
        /// Deletes the specified identifier from the vault.
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be deleted.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// </exception>
        public async Task DeleteAsync(string identifier)
        {
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;

            identifier.AssertNotEmpty(nameof(identifier));

            try
            {
                startTime = DateTime.Now;

                if (!IsEnabled)
                {
                    return;
                }

                using (IKeyVaultClient client = new KeyVaultClient(service.TokenManagement.GetAppOnlyTokenAsync))
                {
                    try
                    {
                        await client.DeleteSecretAsync(service.Configuration.KeyVaultBaseAddress, identifier);
                    }
                    catch (KeyVaultErrorException ex)
                    {
                        if (!ex.Body.Error.Code.Equals(NotFoundErrorCode, StringComparison.CurrentCultureIgnoreCase))
                        {
                            throw;
                        }
                    }
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

                service.Telemetry.TrackEvent("Vault/DeleteAsync", eventProperties, eventMetrics);
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
            }
        }

        /// <summary>
        /// Gets the specified entity from the vault. 
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be retrieved.</param>
        /// <returns>The value retrieved from the vault.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// </exception>
        public async Task<string> GetAsync(string identifier)
        {
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            SecretBundle bundle;

            identifier.AssertNotEmpty(nameof(identifier));

            try
            {
                startTime = DateTime.Now;

                if (!IsEnabled)
                {
                    return null;
                }

                using (IKeyVaultClient client = new KeyVaultClient(service.TokenManagement.GetAppOnlyTokenAsync))
                {
                    try
                    {
                        bundle = await client.GetSecretAsync(service.Configuration.KeyVaultBaseAddress, identifier);
                    }
                    catch (KeyVaultErrorException ex)
                    {
                        if (ex.Body.Error.Code.Equals(NotFoundErrorCode, StringComparison.CurrentCultureIgnoreCase))
                        {
                            bundle = null;
                        }
                        else
                        {
                            throw;
                        }
                    }
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

                service.Telemetry.TrackEvent("Vault/GetAsync", eventProperties, eventMetrics);

                return bundle?.Value;
            }
            finally
            {
                bundle = null;
                eventMetrics = null;
                eventProperties = null;
            }
        }

        /// <summary>
        /// Stores the specified value in the vault.
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be stored.</param>
        /// <param name="value">The value to stored.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="identifier"/> is empty or null.
        /// or 
        /// <paramref name="value"/> is empty or null.
        /// </exception>
        public async Task StoreAsync(string identifier, string value)
        {
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;

            identifier.AssertNotEmpty(nameof(identifier));
            value.AssertNotEmpty(nameof(value));

            try
            {
                startTime = DateTime.Now;

                if (!IsEnabled)
                {
                    return;
                }

                using (IKeyVaultClient client = new KeyVaultClient(service.TokenManagement.GetAppOnlyTokenAsync))
                {
                    await client.SetSecretAsync(
                        service.Configuration.KeyVaultBaseAddress, identifier, value);
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

                service.Telemetry.TrackEvent("Vault/StoreAsync", eventProperties, eventMetrics);
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
            }
        }

        /// <summary>
        /// Executes an asynchronous method synchronously
        /// </summary>
        /// <typeparam name="T">The type to be returned.</typeparam>
        /// <param name="operation">The asynchronous operation to be executed.</param>
        /// <returns>The result from the operation.</returns>
        private static T SynchronousExecute<T>(Func<Task<T>> operation)
        {
            try
            {
                return Task.Run(async () => await operation()).Result;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException != null)
                {
                    throw ex.InnerException;
                }

                throw;
            }
        }
    }
}