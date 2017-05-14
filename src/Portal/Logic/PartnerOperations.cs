// -----------------------------------------------------------------------
// <copyright file="PartnerOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Common;
    using Common.Cache;
    using Models;
    using Practices.Unity;
    using Store.PartnerCenter;
    using Store.PartnerCenter.Extensions;
    using Store.PartnerCenter.Models.Customers;
    using Store.PartnerCenter.RequestContext;

    /// <summary>
    /// Provides the ability to perform various partner operations.
    /// </summary>
    public class PartnerOperations : IPartnerOperations
    {
        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IMigrationService service;

        /// <summary>
        /// Provides the ability to perform partner operation using app only authentication.
        /// </summary>
        private IAggregatePartner appOperations;

        /// <summary>
        /// Key utilized to retrieve and store Partner Center access tokens. 
        /// </summary>
        private const string PartnerCenterCacheKey = "Resource::PartnerCenter::AppOnly";

        /// <summary>
        /// Provides a way to ensure that <see cref="appOperations"/> is only being modified 
        /// by one thread at a time. 
        /// </summary>
        private object appLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerOperations"/> class.
        /// </summary>
        public PartnerOperations()
        {
            service = MvcApplication.UnityContainer.Resolve<IMigrationService>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerOperations"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public PartnerOperations(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Gets the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of <see cref="Customer"/> that represents the specified customer.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="customerId"/> is empty or null.
        /// </exception>
        public async Task<Customer> GetCustomerAsync(string customerId)
        {
            Customer customer;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            Guid correlationId;
            IPartner operations;

            customerId.AssertNotEmpty(nameof(customerId));

            try
            {
                startTime = DateTime.Now;
                correlationId = Guid.NewGuid();
                operations = await GetAppOperationsAsync(correlationId);

                customer = await operations.Customers.ById(customerId).GetAsync();

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "ParternCenterCorrelationId", correlationId.ToString() }
                };

                service.Telemetry.TrackEvent("GetCustomerAsync", eventProperties, eventMetrics);

                return customer;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
                operations = null;
            }
        }

        /// <summary>
        /// Gets an instance of the partner service that utilizes app only authentication.
        /// </summary>
        /// <param name="correlationId">Correlation identifier for the operation.</param>
        /// <returns>An instance of the partner service.</returns>
        private async Task<IPartner> GetAppOperationsAsync(Guid correlationId)
        {
            if (appOperations == null || appOperations.Credentials.ExpiresAt > DateTime.UtcNow)
            {
                IPartnerCredentials credentials = await GetPartnerCenterAppOnlyCredentialsAsync(
                        $"{service.Configuration.ActiveDirectoryEndpoint}/{service.Configuration.PartnerCenterApplicationTenantId}");

                lock (appLock)
                {
                    appOperations = PartnerService.Instance.CreatePartnerOperations(credentials);
                }

                PartnerService.Instance.ApplicationName = MigrationConstants.ApplicationName;
            }

            return appOperations.With(RequestContextFactory.Instance.Create(correlationId));
        }

        /// <summary>
        /// Gets an instance of <see cref="IPartnerCredentials"/> used to access the Partner Center Managed API.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <returns>
        /// An instance of <see cref="IPartnerCredentials" /> that represents the access token.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// </exception>
        /// <remarks>This function will use app only authentication to obtain the credentials.</remarks>
        private async Task<IPartnerCredentials> GetPartnerCenterAppOnlyCredentialsAsync(string authority)
        {
            authority.AssertNotEmpty(nameof(authority));

            // Attempt to obtain the Partner Center token from the cache.
            IPartnerCredentials credentials =
                 await service.Cache.FetchAsync<PartnerCenterTokenModel>(
                     CacheDatabaseType.Authentication, PartnerCenterCacheKey);

            if (credentials != null && !credentials.IsExpired())
            {
                return credentials;
            }

            // The access token has expired, so a new one must be requested.
            credentials = await PartnerCredentials.Instance.GenerateByApplicationCredentialsAsync(
                service.Configuration.PartnerCenterApplicationId,
                service.Configuration.PartnerCenterApplicationSecret,
                service.Configuration.PartnerCenterApplicationTenantId);

            await service.Cache.StoreAsync(
                CacheDatabaseType.Authentication, PartnerCenterCacheKey, credentials);

            return credentials;
        }
    }
}