// -----------------------------------------------------------------------
// <copyright file="MigrationConstants.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Common
{
    /// <summary>
    /// Defines commonly used strings.
    /// </summary>
    public static class MigrationConstants
    {
        /// <summary>
        /// Name of the application used for identification purposes with the Partner Center API. 
        /// </summary>
        public const string ApplicationName = "US HMSP HEX Migration Toolkit v1.0";

        /// <summary>
        /// Name of the environment Service Bus queue.
        /// </summary>
        public const string EnvironmentQueueName = "environments";

        /// <summary>
        /// Name of the storage table where environment entities are stored.
        /// </summary>
        public const string EnvironmentTableName = "environments";

        /// <summary>
        /// Endpoint address to utilize when connecting to Exchange Online using PowerShell remoting.
        /// </summary>
        public const string ExchangeOnlineEndpoint = "https://ps.outlook.com/powershell-liveid?DelegatedOrg=";

        /// <summary>
        /// Name of the storage table where mailbox entities are stored.
        /// </summary>
        public const string MailboxTableName = "mailboxes";

        /// <summary>
        /// Name of the migration batch Service Bus queue.
        /// </summary>
        public const string MigrationBatchQueueName = "migrationbatches";

        /// <summary>
        /// Name of the migration batch delete Service Bus queue.
        /// </summary>
        public const string MigrationBatchDeleteQueueName = "migrationbatchdelete";

        /// <summary>
        /// Name of the start migration batch Server Bus queue.
        /// </summary>
        public const string MigrationBatchStartQueueName = "migrationbatchstart";

        /// <summary>
        /// Name of the storage table where migration batch entities are stored.
        /// </summary>
        public const string MigrationBatchTable = "migrationbatches";

        /// <summary>
        /// Schema to utilize when connecting to Microsoft Exchange.
        /// </summary>
        public const string SchemaUri = "http://schemas.microsoft.com/powershell/Microsoft.Exchange";

        /// <summary>
        /// Last part of the target delivery domain.
        /// </summary>
        public const string TargetDeliveryDomainSuffix = "mail.onmicrosoft.com";
    }
}