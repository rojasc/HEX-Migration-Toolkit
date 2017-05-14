// -----------------------------------------------------------------------
// <copyright file="MailboxesViewModel.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Models
{
    using System.Collections.Generic;

    public class MailboxesViewModel
    {
        /// <summary>
        /// Gets or sets a list of mailboxes.
        /// </summary>
        public List<MailboxViewModel> Mailboxes { get; set; }
    }
}