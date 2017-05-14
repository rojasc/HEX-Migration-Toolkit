// -----------------------------------------------------------------------
// <copyright file="IPartnerOperations.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Logic
{
    using System.Threading.Tasks;
    using Store.PartnerCenter.Models.Customers;

    /// <summary>
    /// Represents the ability to perform various partner operations.
    /// </summary>
    public interface IPartnerOperations
    {
        /// <summary>
        /// Gets the specified customer.
        /// </summary>
        /// <param name="customerId">Identifier for the customer.</param>
        /// <returns>An instance of <see cref="Customer"/> that represents the specified customer.</returns>
        Task<Customer> GetCustomerAsync(string customerId);
    }
}