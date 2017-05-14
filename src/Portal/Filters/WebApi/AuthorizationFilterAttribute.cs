// -----------------------------------------------------------------------
// <copyright file="AuthorizationFilterAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Filters.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using Common;
    using Security;

    /// <summary>
    /// Authorization filter attribute used to verify authenticated user has the specified claim and value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class AuthorizationFilterAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Gets or sets the required roles.
        /// </summary>
        public new UserRole Roles { get; set; }

        /// <summary>
        /// Indicates whether the authenticated user is authorized.
        /// </summary>
        /// <param name="actionContext">The action context.</param>
        /// <returns><c>true</c> if the user is authorized; otherwise <c>false</c>.</returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            CustomerPrincipal principal;

            try
            {
                principal = actionContext.RequestContext.Principal as CustomerPrincipal;

                foreach (string role in GetRoles(Roles))
                {
                    if (principal.HasClaim(System.Security.Claims.ClaimTypes.Role, role))
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                principal = null;
            }
        }

        /// <summary>
        /// Gets a list of roles that required to perform the operation.
        /// </summary>
        /// <param name="requiredRole">User role required to perform the operation.</param>
        /// <returns>A list of roles that required to perform the operation.</returns>
        private List<string> GetRoles(UserRole requiredRole)
        {
            List<string> required = new List<string>();

            if (requiredRole.HasFlag(UserRole.AdminAgents))
            {
                required.Add(UserRole.AdminAgents.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.BillingAdmin))
            {
                required.Add(UserRole.BillingAdmin.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.GlobalAdmin))
            {
                required.Add(UserRole.GlobalAdmin.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.HelpdeskAgent))
            {
                required.Add(UserRole.HelpdeskAgent.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.SalesAgent))
            {
                required.Add(UserRole.SalesAgent.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.User))
            {
                required.Add(UserRole.User.GetDescription());
            }

            if (requiredRole.HasFlag(UserRole.UserAdministrator))
            {
                required.Add(UserRole.UserAdministrator.GetDescription());
            }

            return required;
        }
    }
}