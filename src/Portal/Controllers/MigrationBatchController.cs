// -----------------------------------------------------------------------
// <copyright file="MigrationBatchController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using Common;
    using Common.Storage;
    using Filters.Mvc;
    using Logic;
    using Models;
    using Security;
    using Store.PartnerCenter.Models.Customers;

    /// <summary>
    /// API controller responsible for migration batch interactions.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner | UserRole.GlobalAdmin)]
    [RoutePrefix("api/migrationbatch")]
    public class MigrationBatchController : BaseApiController
    {
        /// <summary>
        /// Provides the ability to perform partner operations.
        /// </summary>
        private IPartnerOperations operations;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationBatchController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <param name="operations">Provides the ability to perform partner operations.</param>
        public MigrationBatchController(IMigrationService service, IPartnerOperations operations) : base(service)
        {
            this.operations = operations;
        }

        /// <summary>
        /// Creates a new migration batch based on the instance of <see cref="MigrationBatchViewModel"/>.
        /// </summary>
        /// <param name="batch">An aptly populated instance of <see cref="MigrationBatchViewModel"/></param>
        /// <returns>An instance of <see cref="MigrationBatchViewModel"/> that represents the newly create migration batch.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="batch"/> is null.
        /// </exception>
        [HttpPost]
        [Route("")]
        public async Task<MigrationBatchViewModel> CreateBatchAsync([FromBody]MigrationBatchViewModel batch)
        {
            Customer customer;
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            List<MailboxEntity> entries;
            MigrationBatchEntity entity;
            string targetDeliveryDomain;

            batch.AssertNotNull(nameof(batch));

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (string.IsNullOrEmpty(batch.Id))
                {
                    batch.Id = Guid.NewGuid().ToString();
                }

                customer = await operations.GetCustomerAsync(principal.CustomerId);
                targetDeliveryDomain = $"{customer.CompanyProfile.Domain.Split('.')[0]}.{MigrationConstants.TargetDeliveryDomainSuffix}";

                entity = new MigrationBatchEntity(batch.EnvironmentId, batch.Id)
                {
                    CustomerId = principal.CustomerId,
                    ETag = "*",
                    Name = batch.Name,
                    StartTime = DateTime.Parse(batch.StartTime),
                    Started = false,
                    TargetDeliveryDomain = targetDeliveryDomain
                };

                entries = batch.Mailboxes.Select(m => new MailboxEntity(batch.EnvironmentId, m.Guid)
                {
                    DisplayName = m.DisplayName,
                    ETag = "*",
                    Name = m.Name,
                    MigrationBatchId = batch.Id,
                    PrimarySmtpAddress = m.PrimarySmtpAddress,
                    SamAccountName = m.SamAccountName,
                    UserPrincipalName = m.UserPrincipalName
                }).ToList();

                await Service.Storage.WriteToTableAsync(MigrationConstants.MigrationBatchTable, entity);
                await Service.Storage.WriteBatchToTableAsync(MigrationConstants.MailboxTableName, entries);
                await Service.ServiceBus.WriteToQueueAsync(MigrationConstants.MigrationBatchQueueName, entity);

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "EnvironmentId", batch.EnvironmentId },
                    { "MigrationBatchName", batch.Name },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfMailboxes", batch.Mailboxes.Count }
                };

                Service.Telemetry.TrackEvent("/api/migrationbatch/create", eventProperties, eventMetrics);

                return batch;
            }
            finally
            {
                entity = null;
                eventMetrics = null;
                eventProperties = null;
                principal = null;
            }
        }

        /// <summary>
        /// Delete the specified migration batch.
        /// </summary>
        /// <param name="environmentId">Identifier for the environment.</param>
        /// <param name="batchId">Identifier for the migration batch.</param>
        /// <returns>A list of configured migration batches.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="migrationBatchViewModel"/> is null.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Unable to locate the specified entity or the user is not authorized to perform this operation.
        /// </exception>
        [HttpPost]
        [Route("delete")]
        public async Task<MigrationBatchesViewModel> DeleteMigrationBatchAsync([FromBody] MigrationBatchViewModel migrationBatchViewModel)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            EnvironmentEntity environment;
            List<MailboxEntity> mailboxes;
            MigrationBatchEntity entity;
            MigrationBatchesViewModel migrationBatches;

            migrationBatchViewModel.AssertNotNull(nameof(migrationBatchViewModel));

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                environment = await Service.Storage.GetEntityAsync<EnvironmentEntity>(
                    MigrationConstants.EnvironmentTableName,
                    principal.CustomerId,
                    migrationBatchViewModel.EnvironmentId);

                entity = await Service.Storage.GetEntityAsync<MigrationBatchEntity>(
                    MigrationConstants.MigrationBatchTable,
                    migrationBatchViewModel.EnvironmentId,
                    migrationBatchViewModel.Id);

                if (environment == null || entity == null)
                {
                    throw new EntityNotFoundException(Resources.EntityNotFoundException);
                }

                mailboxes = await Service.Storage.GetEntitiesAsync<MailboxEntity>(
                    MigrationConstants.MailboxTableName,
                    m => m.PartitionKey.Equals(environment.RowKey)
                        && !string.IsNullOrEmpty(m.MigrationBatchId)
                        && m.MigrationBatchId.Equals(entity.RowKey));

                if (mailboxes?.Count > 0)
                {
                    foreach (MailboxEntity mailbox in mailboxes)
                    {
                        mailbox.MigrationBatchId = string.Empty;
                    }

                    await Service.Storage.WriteBatchToTableAsync(
                        MigrationConstants.MailboxTableName,
                        mailboxes);
                }

                await Service.ServiceBus.WriteToQueueAsync(
                    MigrationConstants.MigrationBatchDeleteQueueName,
                    entity);

                await Service.Storage.DeleteEntityAsync(
                    MigrationConstants.MigrationBatchTable,
                    entity);

                migrationBatches = await GetMigrationBatchesAsync(migrationBatchViewModel.EnvironmentId);

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "EnvironmentId", migrationBatchViewModel.EnvironmentId },
                    { "MigrationBatchName", entity.Name },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfBatches", migrationBatches.MigrationBatches.Count }
                };

                Service.Telemetry.TrackEvent("/api/migrationbatch/delete", eventProperties, eventMetrics);

                return migrationBatches;
            }
            finally
            {
                entity = null;
                environment = null;
                eventMetrics = null;
                eventProperties = null;
                mailboxes = null;
                principal = null;
            }
        }

        /// <summary>
        /// Get migration batch details for the specified migration batch.
        /// </summary>
        /// <param name="environmentId">Identifier for the environment.</param>
        /// <param name="batchId">Identifier for the migration batch.</param>
        /// <returns>An instance of <see cref="MigrationBatchViewModel"/> that represents the migration batch details.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="environmentId"/> is empty or null.
        /// or
        /// <paramref name="batchId"/> is empty or null.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Unable to locate the specified entity or the user is not authorized to perform this operation.
        /// </exception>
        [HttpGet]
        [Route("details")]
        public async Task<MigrationBatchViewModel> GetMigrationBatchAsync(string environmentId, string batchId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            EnvironmentEntity environment;
            List<MailboxEntity> mailboxes;
            MigrationBatchEntity entity;

            batchId.AssertNotEmpty(nameof(batchId));
            environmentId.AssertNotEmpty(nameof(environmentId));

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                environment = await Service.Storage.GetEntityAsync<EnvironmentEntity>(
                    MigrationConstants.EnvironmentTableName,
                    principal.CustomerId,
                    environmentId);

                entity = await Service.Storage.GetEntityAsync<MigrationBatchEntity>(
                    MigrationConstants.MigrationBatchTable,
                    environmentId,
                    batchId);

                if (environment == null || entity == null)
                {
                    throw new EntityNotFoundException(Resources.EntityNotFoundException);
                }

                mailboxes = await Service.Storage.GetEntitiesAsync<MailboxEntity>(
                    MigrationConstants.MailboxTableName,
                    m => m.PartitionKey.Equals(environmentId) && !string.IsNullOrEmpty(m.MigrationBatchId)
                        && m.MigrationBatchId.Equals(batchId));

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "EnvironmentId", environmentId },
                    { "EnvironmentName", environment.Name },
                    { "MigrationBatchName", entity.Name },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfMailboxes", mailboxes.Count }
                };

                Service.Telemetry.TrackEvent("/api/migrationbatch/details", eventProperties, eventMetrics);

                return new MigrationBatchViewModel
                {
                    EnvironmentId = environmentId,
                    Id = batchId,
                    Mailboxes = mailboxes.Select(m => new MailboxViewModel
                    {
                        DisplayName = m.DisplayName,
                        Guid = m.RowKey,
                        Name = m.Name,
                        PrimarySmtpAddress = m.PrimarySmtpAddress,
                        SamAccountName = m.SamAccountName,
                        UserPrincipalName = m.UserPrincipalName
                    }).ToList(),
                    Name = entity.Name,
                    StartTime = entity.StartTime.ToString(),
                    TargetDeliveryDomain = entity.TargetDeliveryDomain
                };
            }
            finally
            {
                entity = null;
                environment = null;
                eventMetrics = null;
                eventProperties = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets a list of the configured migration batches.
        /// </summary>
        /// <param name="environmentId">Identifier for the environment.</param>
        /// <returns>A list of configured migration batches.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="environmentId"/> is empty or null.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Unable to locate the specified entity or the user is not authorized to perform this operation.
        /// </exception>
        [HttpGet]
        [Route("")]
        public async Task<MigrationBatchesViewModel> GetMigrationBatchesAsync(string environmentId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            EnvironmentEntity environment;
            List<MigrationBatchEntity> entities;

            environmentId.AssertNotEmpty(nameof(environmentId));

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                environment = await Service.Storage.GetEntityAsync<EnvironmentEntity>(
                    MigrationConstants.EnvironmentTableName,
                    principal.CustomerId,
                    environmentId);

                if (environment == null)
                {
                    throw new EntityNotFoundException(Resources.EntityNotFoundException);
                }

                entities = await Service.Storage.GetEntitiesAsync<MigrationBatchEntity>(
                    MigrationConstants.MigrationBatchTable,
                    m => m.PartitionKey.Equals(environmentId));

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "EnvironmentId", environmentId },
                    { "EnvironmentName", environment.Name },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfMigrationBatches", entities.Count }
                };

                Service.Telemetry.TrackEvent("/api/migrationbatch", eventProperties, eventMetrics);

                return new MigrationBatchesViewModel
                {
                    MigrationBatches = entities.Select(b => new MigrationBatchViewModel
                    {
                        EnvironmentId = b.PartitionKey,
                        Id = b.RowKey,
                        Name = b.Name,
                        StartTime = b.StartTime.ToString(),
                        TargetDeliveryDomain = b.TargetDeliveryDomain
                    }).ToList()
                };
            }
            finally
            {
                entities = null;
                environment = null;
                eventMetrics = null;
                eventProperties = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets all mailboxes associated with the specified environment.
        /// </summary>
        /// <param name="environmentId">Identifier for the environment.</param>
        /// <returns>A list of mailboxes associated with the specified environment.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="environmentId"/> is empty or null.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Unable to locate the specified entity or the user is not authorized to perform this operation.
        /// </exception>
        [HttpGet]
        [Route("mailboxes")]
        public async Task<MailboxesViewModel> GetMailboxesAsync(string environmentId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            EnvironmentEntity environment;
            List<MailboxEntity> mailboxes;

            environmentId.AssertNotEmpty(nameof(environmentId));

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                environment = await Service.Storage.GetEntityAsync<EnvironmentEntity>(
                    MigrationConstants.EnvironmentTableName,
                    principal.CustomerId,
                    environmentId);

                if (environment == null)
                {
                    throw new EntityNotFoundException(Resources.EntityNotFoundException);
                }

                mailboxes = await Service.Storage.GetEntitiesAsync<MailboxEntity>(
                    MigrationConstants.MailboxTableName,
                    m => m.PartitionKey.Equals(environmentId) && string.IsNullOrEmpty(m.MigrationBatchId));

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "EnvironmentId", environmentId },
                    { "EnvironmentName", environment.Name },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfMailboxes", mailboxes.Count }
                };

                Service.Telemetry.TrackEvent("/api/migrationbatch", eventProperties, eventMetrics);

                return new MailboxesViewModel
                {
                    Mailboxes = mailboxes.Select(m => new MailboxViewModel
                    {
                        DisplayName = m.DisplayName,
                        Guid = m.RowKey,
                        Name = m.Name,
                        PrimarySmtpAddress = m.PrimarySmtpAddress,
                        SamAccountName = m.SamAccountName,
                        UserPrincipalName = m.UserPrincipalName
                    }).ToList()
                };
            }
            finally
            {
                environment = null;
                eventMetrics = null;
                eventProperties = null;
                mailboxes = null;
                principal = null;
            }
        }
    }
}