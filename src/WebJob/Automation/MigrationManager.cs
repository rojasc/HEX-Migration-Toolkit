// -----------------------------------------------------------------------
// <copyright file="MigrationManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.WebJob.Automation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;
    using System.Threading.Tasks;
    using Common;
    using Common.Storage;

    /// <summary>
    /// Provides the ability to manage Exchange Online migrations.
    /// </summary>
    public class MigrationManager : IMigrationManager
    {
        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private IMigrationService service;

        /// <summary>
        /// Provides the ability to invoke scripts.
        /// </summary>
        private IScriptManager scriptManager;

        /// <summary>
        /// Initialize a new instance of the <see cref="MigrationManager"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null
        /// </exception>
        public MigrationManager(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));
            scriptManager = new ScriptManager(service);
            this.service = service;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationManager"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <param name="scriptManager">Provides the ability to invoke scripts.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="service"/> is null
        /// or
        /// <paramref name="scriptManager"/> is null.
        /// </exception>
        public MigrationManager(IMigrationService service, IScriptManager scriptManager)
        {
            scriptManager.AssertNotNull(nameof(scriptManager));
            service.AssertNotNull(nameof(service));

            this.scriptManager = scriptManager;
            this.service = service;
        }

        /// <summary>
        /// Creates a new migration batch in Exchange Online.
        /// </summary>
        /// <param name="entity">An instance of <see cref="MigrationBatchEntity"/> that represents the new migration batch.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        public async Task CreateMigrationBatchAsync(MigrationBatchEntity entity)
        {
            Command command;
            CommandParameterCollection parameters;
            EnvironmentEntity environment;
            List<MailboxEntity> entites; ;
            PSCredential credential;
            Uri connectionUri;
            WSManConnectionInfo connectionInfo;
            string csvData;

            entity.AssertNotNull(nameof(entity));

            try
            {
                credential = new PSCredential(
                    service.Configuration.ExchangeOnlineUsername,
                    service.Configuration.ExchangeOnlinePassword.ToSecureString());

                environment = await service.Storage.GetEntityAsync<EnvironmentEntity>(
                     MigrationConstants.EnvironmentTableName,
                     entity.CustomerId,
                     entity.PartitionKey);

                connectionUri = new Uri($"{MigrationConstants.ExchangeOnlineEndpoint}{environment.Organization}");
                connectionInfo = GetConnectionInfo(connectionUri, MigrationConstants.SchemaUri, credential);

                entites = await service.Storage.GetEntitiesAsync<MailboxEntity>(
                    MigrationConstants.MailboxTableName,
                    m => m.PartitionKey.Equals(environment.RowKey) 
                        && !string.IsNullOrEmpty(m.MigrationBatchId) 
                        && m.MigrationBatchId.Equals(entity.RowKey));

                csvData = entites.Aggregate(new StringBuilder("EmailAddress\n"),
                        (sb, v) => sb.Append(v.PrimarySmtpAddress).Append("\n"),
                        sb => { if (0 < sb.Length) sb.Length--; return sb.ToString(); });

                command = new Command("New-MigrationBatch");

                parameters = new CommandParameterCollection
                {
                    { "CSVData", Encoding.ASCII.GetBytes(csvData) },
                    { "Name", entity.Name },
                    { "SourceEndpoint",  environment.Name },
                    { "TargetDeliveryDomain", entity.TargetDeliveryDomain }
                };

                using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                {
                    scriptManager.InvokeCommand(runspace, command, parameters);
                }

            }
            finally
            {
                connectionInfo = null;
                connectionUri = null;
                credential = null;
                parameters = null;
            }
        }

        /// <summary>
        /// Creates a new migration endpoint in Exchange Online.
        /// </summary>
        /// <param name="entity">An instance of <see cref="EnvironmentEntity"/> that represents the new migration endpoint.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        public async Task CreateMigrationEndpointAsync(EnvironmentEntity entity)
        {
            Command command;
            CommandParameterCollection parameters;
            PSCredential credential;
            PSCredential onPermiseCredential;
            Uri connectionUri;
            WSManConnectionInfo connectionInfo;
            string onPermisePassword;

            entity.AssertNotNull(nameof(entity));

            try
            {
                credential = new PSCredential(
                    service.Configuration.ExchangeOnlineUsername,
                    service.Configuration.ExchangeOnlinePassword.ToSecureString());

                onPermisePassword = await service.Vault.GetAsync(entity.Password);
                onPermiseCredential = new PSCredential(entity.Username, onPermisePassword.ToSecureString());

                connectionUri = new Uri($"{MigrationConstants.ExchangeOnlineEndpoint}{entity.Organization}");
                connectionInfo = GetConnectionInfo(connectionUri, MigrationConstants.SchemaUri, credential);

                command = new Command("New-MigrationEndpoint");

                parameters = new CommandParameterCollection
                {
                    { "Autodiscover" },
                    { "Credentials",  onPermiseCredential },
                    { "EmailAddress", entity.Username },
                    { "ExchangeRemoteMove" },
                    { "Name", entity.Name }
                };

                using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                {
                    scriptManager.InvokeCommand(runspace, command, parameters);
                }
            }
            finally
            {
                command = null;
                connectionInfo = null;
                connectionUri = null;
                credential = null;
                onPermiseCredential = null;
                parameters = null;
            }
        }

        /// <summary>
        /// Gets list of mailboxes from the on permise environment.
        /// </summary>
        /// <param name="entity">An instance of <see cref="EnvironmentEntity"/> that represents the on permise environment.</param>
        /// <returns>A list of mailboxes from the on permise environment.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        public async Task<List<MailboxEntity>> GetMailboxesAsync(EnvironmentEntity entity)
        {
            Command command;
            CommandParameterCollection parameters;
            Collection<PSObject> results;
            List<MailboxEntity> mailboxes;
            PSCredential credential;
            WSManConnectionInfo connectionInfo;
            string password;

            entity.AssertNotNull(nameof(entity));

            try
            {
                password = await service.Vault.GetAsync(entity.Password);
                credential = new PSCredential(entity.Username, password.ToSecureString());

                connectionInfo = GetConnectionInfo(new Uri(entity.Endpoint), MigrationConstants.SchemaUri, credential);

                command = new Command("Get-Mailbox");

                parameters = new CommandParameterCollection
                {
                    { "ResultSize",  "Unlimited" }
                };

                using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                {
                    results = scriptManager.InvokeCommand(runspace, command, parameters);
                }

                mailboxes = results.Select(m => new MailboxEntity
                {
                    DisplayName = m.Properties["DisplayName"].Value.ToString(),
                    ETag = "*",
                    Name = m.Properties["Name"].Value.ToString(),
                    PartitionKey = entity.RowKey,
                    PrimarySmtpAddress = m.Properties["PrimarySmtpAddress"].Value.ToString(),
                    RowKey = m.Properties["Guid"].Value.ToString(),
                    SamAccountName = m.Properties["SamAccountName"].Value.ToString(),
                    UserPrincipalName = m.Properties["UserPrincipalName"].Value.ToString()
                }).ToList();

                return mailboxes;
            }
            finally
            {
                command = null;
                connectionInfo = null;
                credential = null;
                parameters = null;
                results = null;
            }
        }

        /// <summary>
        /// Deletes the migration batch in Exchange Online.
        /// </summary>
        /// <param name="entity">An instance of <see cref="MigrationBatchEntity"/> that represents the migration batch to delete.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        public async Task MigrationBatchDeleteAsync(MigrationBatchEntity entity)
        {
            Command command;
            CommandParameterCollection parameters;
            EnvironmentEntity environment;
            PSCredential credential;
            Uri connectionUri;
            WSManConnectionInfo connectionInfo;

            entity.AssertNotNull(nameof(entity));

            try
            {
                credential = new PSCredential(
                    service.Configuration.ExchangeOnlineUsername,
                    service.Configuration.ExchangeOnlinePassword.ToSecureString());

                environment = await service.Storage.GetEntityAsync<EnvironmentEntity>(
                     MigrationConstants.EnvironmentTableName,
                     entity.CustomerId,
                     entity.PartitionKey);

                connectionUri = new Uri($"{MigrationConstants.ExchangeOnlineEndpoint}{environment.Organization}");
                connectionInfo = GetConnectionInfo(connectionUri, MigrationConstants.SchemaUri, credential);

                command = new Command("Remove-MigrationBatch");

                parameters = new CommandParameterCollection
                {
                    { "Confirm", false },
                    { "Identity", entity.Name }
                };

                using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                {
                    scriptManager.InvokeCommand(runspace, command, parameters);
                }
            }
            finally
            {
                connectionInfo = null;
                connectionUri = null;
                credential = null;
                parameters = null;
            }
        }

        /// <summary>
        /// Starts the migration batch in Exchange Online.
        /// </summary>
        /// <param name="entity">An instance of <see cref="MigrationBatchEntity"/> that represents the migration batch to start.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="entity"/> is null.
        /// </exception>
        public async Task MigrationBatchStartAsync(MigrationBatchEntity entity)
        {
            Command command;
            CommandParameterCollection parameters;
            EnvironmentEntity environment;
            PSCredential credential;
            Uri connectionUri;
            WSManConnectionInfo connectionInfo;

            entity.AssertNotNull(nameof(entity));

            try
            {
                credential = new PSCredential(
                    service.Configuration.ExchangeOnlineUsername,
                    service.Configuration.ExchangeOnlinePassword.ToSecureString());

                environment = await service.Storage.GetEntityAsync<EnvironmentEntity>(
                     MigrationConstants.EnvironmentTableName,
                     entity.CustomerId,
                     entity.PartitionKey);

                connectionUri = new Uri($"{MigrationConstants.ExchangeOnlineEndpoint}{environment.Organization}");
                connectionInfo = GetConnectionInfo(connectionUri, MigrationConstants.SchemaUri, credential);

                command = new Command("Start-MigrationBatch");

                parameters = new CommandParameterCollection
                {
                    { "Identity", entity.Name }
                };

                using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                {
                    scriptManager.InvokeCommand(runspace, command, parameters);
                }

                entity.Started = true;
                await service.Storage.WriteToTableAsync(MigrationConstants.MigrationBatchTable, entity);
            }
            finally
            {
                connectionInfo = null;
                connectionUri = null;
                credential = null;
                parameters = null;
            }
        }

        /// <summary>
        /// Get statistics for the specified migration batch.
        /// </summary>
        /// <param name="entity">An instance of <see cref="MigrationBatchEntity"/> that represents the migration batch</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task MigrationBatchStatisticsAsync(MigrationBatchEntity entity)
        {
            Command command;
            List<Command> commands;
            EnvironmentEntity environment;
            PSCredential credential;
            Uri connectionUri;
            WSManConnectionInfo connectionInfo;

            entity.AssertNotNull(nameof(entity));

            try
            {
                credential = new PSCredential(
                    service.Configuration.ExchangeOnlineUsername,
                    service.Configuration.ExchangeOnlinePassword.ToSecureString());

                environment = await service.Storage.GetEntityAsync<EnvironmentEntity>(
                     MigrationConstants.EnvironmentTableName,
                     entity.CustomerId,
                     entity.PartitionKey);

                connectionUri = new Uri($"{MigrationConstants.ExchangeOnlineEndpoint}{environment.Organization}");
                connectionInfo = GetConnectionInfo(connectionUri, MigrationConstants.SchemaUri, credential);

                commands = new List<Command>();

                command = new Command("Get-MigrationUser");
                command.Parameters.Add("BatchId", entity.Name);
                commands.Add(command);

                command = new Command("Get-MigrationUserStatistics");
                commands.Add(command);

                using (Runspace runspace = RunspaceFactory.CreateRunspace(connectionInfo))
                {
                    scriptManager.InvokeCommand(runspace, commands);
                }
            }
            finally
            {
                commands = null;
                connectionInfo = null;
                connectionUri = null;
                credential = null;
            }
        }

        /// <summary>
        /// Gets a configured instance of <see cref="WSManConnectionInfo"/>.
        /// </summary>
        /// <param name="connectionUri">URI that defines the connection endpoint.</param>
        /// <param name="schemaUri">URI of the shell that is launched when the connection is established.</param>
        /// <param name="credential">User account that has permission to make the connection.</param>
        /// <returns>An aptly configured instance of <see cref="WSManConnectionInfo"/>.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="schemaUri"/> is empty or null.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="connectionUri"/> is null.
        /// or
        /// <paramref name="credential"/> is null.
        /// </exception>
        private static WSManConnectionInfo GetConnectionInfo(Uri connectionUri, string schemaUri, PSCredential credential)
        {
            connectionUri.AssertNotNull(nameof(connectionUri));
            credential.AssertNotNull(nameof(credential));
            schemaUri.AssertNotEmpty(nameof(schemaUri));

            return new WSManConnectionInfo(connectionUri, schemaUri, credential)
            {
                AuthenticationMechanism = AuthenticationMechanism.Basic
            };
        }
    }
}