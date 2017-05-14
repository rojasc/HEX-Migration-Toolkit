// -----------------------------------------------------------------------
// <copyright file="IGraphClient.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Logic
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    /// <summary>
    /// Represents an object that interacts with Microsoft Graph.
    /// </summary>
    public interface IGraphClient
    {
        /// <summary>
        /// Gets a list of roles that the specified user is associated with.
        /// </summary>
        /// <param name="objectId">Object identifier for the object to be checked.</param>
        /// <returns>A list of directory roles that the specified object identifier is associated with.</returns>
        Task<List<RoleModel>> GetDirectoryRolesAsync(string objectId);
    }
}