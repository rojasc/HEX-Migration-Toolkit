﻿// -----------------------------------------------------------------------
// <copyright file="PartnerCenterTokenModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Models
{
    using System;
    using Store.PartnerCenter;

    /// <summary>
    /// Represents an access token used to access the Partner Center API.
    /// </summary>
    /// <seealso cref="IPartnerCredentials" />
    public class PartnerCenterTokenModel : IPartnerCredentials
    {
        /// <summary>
        /// Gets the expiry time in UTC for the token.
        /// </summary>
        public DateTimeOffset ExpiresAt { get; private set; }

        /// <summary>
        /// Gets the token needed to authenticate with the partner API service.
        /// </summary>
        public string PartnerServiceToken { get; private set; }

        /// <summary>
        /// Indicates whether the partner credentials have expired or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if credentials have expired; otherwise <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This function is implemented differently than the built in token types.
        /// Since this token is used for serialization purposes it does not have the
        /// Azure AD token. That being the case different logic haas to be used in
        /// order to verify that the token has not expired. Also, it is important to
        /// note that if this token is stored in Redis Cache then the cache is configured
        /// to expire based upon the ExpiresAt property value.
        /// </remarks>
        public bool IsExpired()
        {
            return DateTime.UtcNow >= ExpiresAt;
        }
    }
}