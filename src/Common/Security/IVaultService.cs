// -----------------------------------------------------------------------
// <copyright file="IVaultService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common.Security
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a secure mechanism for retrieving and store information. 
    /// </summary>
    public interface IVaultService
    {
        /// <summary>
        /// Gets a value indicating whether the vault service is enabled or not.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Deletes the specified identifier from the vault.
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be deleted.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task DeleteAsync(string identifier);

        /// <summary>
        /// Gets the specified entity from the vault. 
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be retrieved.</param>
        /// <returns>The value retrieved from the vault.</returns>
        string Get(string identifier);

        /// <summary>
        /// Gets the specified entity from the vault. 
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be retrieved.</param>
        /// <returns>The value retrieved from the vault.</returns>
        Task<string> GetAsync(string identifier);

        /// <summary>
        /// Stores the specified value in the vault.
        /// </summary>
        /// <param name="identifier">Identifier of the entity to be stored.</param>
        /// <param name="value">The value to stored.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        Task StoreAsync(string identifier, string value);
    }
}