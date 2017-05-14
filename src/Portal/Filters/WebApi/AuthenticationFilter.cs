// -----------------------------------------------------------------------
// <copyright file="AuthenticationFilter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Filters.WebApi
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http.Filters;
    using Security;

    /// <summary>
    /// Augments Web API authentication by replacing the principal with a more usable custom principal object.
    /// </summary>
    public class AuthenticationFilter : ActionFilterAttribute, IAuthenticationFilter
    {
        /// <summary>
        /// Authenticates the request.
        /// </summary>
        /// <param name="context">The authentication context.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            context.Principal = new CustomerPrincipal(HttpContext.Current.User as System.Security.Claims.ClaimsPrincipal);

            await Task.FromResult(0);
        }

        /// <summary>
        /// Performs an authentication challenge.
        /// </summary>
        /// <param name="context">The authentication content.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            await Task.FromResult(0);
        }
    }
}