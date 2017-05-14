// -----------------------------------------------------------------------
// <copyright file="MailboxViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Models
{
    /// <summary>
    /// Represents a view model for a mailbox.
    /// </summary>
    public class MailboxViewModel
    {
        /// <summary>
        /// Gets or sets the display name for the mailbox.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the global unique identifier for the mailbox.
        /// </summary>
        public string Guid { get; set; }

        /// <summary>
        /// Gets or sets the name for the mailbox.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the primary SMTP address for the mailbox.
        /// </summary>
        public string PrimarySmtpAddress { get; set; }

        /// <summary>
        /// Gets or sets the sAMAccountName for the mailbox.
        /// </summary>
        public string SamAccountName { get; set; }

        /// <summary>
        /// Gets or sets the user principal name for the mailbox.
        /// </summary>
        public string UserPrincipalName { get; set; }
    }
}