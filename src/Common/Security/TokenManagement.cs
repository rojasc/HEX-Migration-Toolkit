// -----------------------------------------------------------------------
// <copyright file="TokenManagement.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common.Security
{
    using System.Threading.Tasks;
    using Common.Cache;
    using IdentityModel.Clients.ActiveDirectory;

    /// <summary>
    /// Provides the ability to manage access tokens.
    /// </summary>
    /// <seealso cref="ITokenManagement" />
    public class TokenManagement : ITokenManagement
    {
        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private readonly IMigrationService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenManagement"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        public TokenManagement(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <returns>An instance of <see cref="AuthenticationToken"/> that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// or
        /// <paramref name="resource"/> is empty or null.
        /// </exception>
        public async Task<AuthenticationResult> GetAppOnlyTokenAsync(string authority, string resource)
        {
            AuthenticationContext authContext;
            DistributedTokenCache tokenCache;

            authority.AssertNotEmpty(nameof(authority));
            resource.AssertNotEmpty(nameof(resource));

            try
            {
                if (service.Cache.IsEnabled)
                {
                    tokenCache = new DistributedTokenCache(service, resource, $"AppOnly::{resource}");
                    authContext = new AuthenticationContext(authority, tokenCache);
                }
                else
                {
                    authContext = new AuthenticationContext(authority);
                }

                return await authContext.AcquireTokenAsync(
                    resource,
                    new ClientCredential(
                        service.Configuration.ApplicationId,
                        service.Configuration.ApplicationSecret));

            }
            finally
            {
                authContext = null;
                tokenCache = null;
            }
        }

        /// <summary>
        /// Gets an access token from the authority using app only authentication.
        /// </summary>
        /// <param name="authority">Address of the authority to issue the token.</param>
        /// <param name="resource">Identifier of the target resource that is the recipient of the requested token.</param>
        /// <param name="scope">Permissions the requested token will need.</param>
        /// <returns>A string that represented the access token.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="authority"/> is empty or null.
        /// or
        /// <paramref name="resource"/> is empty or null.
        /// </exception>
        public async Task<string> GetAppOnlyTokenAsync(string authority, string resource, string scope)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;

            authority.AssertNotEmpty(nameof(authority));
            resource.AssertNotEmpty(nameof(resource));

            try
            {
                authContext = new AuthenticationContext(authority);

                authResult = await authContext.AcquireTokenAsync(
                    resource,
                    new ClientCredential(
                        service.Configuration.KeyVaultApplicationId,
                        service.Configuration.KeyVaultApplicationSecret));

                return authResult.AccessToken;
            }
            finally
            {
                authContext = null;
                authResult = null;
            }
        }
    }
}