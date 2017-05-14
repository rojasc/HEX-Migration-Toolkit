// -----------------------------------------------------------------------
// <copyright file="EnvironmentController.cs" company="Microsoft">
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
    using Filters.WebApi;
    using Logic;
    using Models;
    using Security;

    /// <summary>
    /// API controller responsible for environment interactions.
    /// </summary>
    [AuthorizationFilter(Roles = UserRole.Partner | UserRole.GlobalAdmin)]
    [RoutePrefix("api/environment")]
    public class EnvironmentController : BaseApiController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentController"/> class.
        /// </summary>
        /// <param name="service"></param>
        public EnvironmentController(IMigrationService service) : base(service)
        {
        }

        /// <summary>
        /// Creates a new environment based on the instance of <see cref="EnvironmentViewModel"/>.
        /// </summary>
        /// <param name="environmentViewModel">An aptly configured instance of <see cref="EnvironmentViewModel"/>.</param>
        /// <returns>An instance of <see cref="EnvironmentViewModel"/> that represents the newly created environment.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="environmentViewModel"/> is null.
        /// </exception>
        [HttpPost]
        [Route("")]
        public async Task<EnvironmentViewModel> CreateAsync([FromBody] EnvironmentViewModel environmentViewModel)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            EnvironmentEntity entity;
            string password;

            environmentViewModel.AssertNotNull(nameof(environmentViewModel));

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                if (string.IsNullOrEmpty(environmentViewModel.Id))
                {
                    environmentViewModel.Id = Guid.NewGuid().ToString();
                }

                password = Guid.NewGuid().ToString();
                await Service.Vault.StoreAsync(password, environmentViewModel.Password);

                entity = new EnvironmentEntity(principal.CustomerId, environmentViewModel.Id)
                {
                    Endpoint = environmentViewModel.Endpoint,
                    ETag = "*",
                    Name = environmentViewModel.Name,
                    Organization = principal.Organization,
                    Password = password,
                    Username = environmentViewModel.Username
                };

                await Service.Storage.WriteToTableAsync(MigrationConstants.EnvironmentTableName, entity);

                await Service.ServiceBus.WriteToQueueAsync(MigrationConstants.EnvironmentQueueName, entity);

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "EnvironmentId", environmentViewModel.Id },
                    { "EnvironmentName", environmentViewModel.Name },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                Service.Telemetry.TrackEvent("/api/environment/create", eventProperties, eventMetrics);

                return environmentViewModel;
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
        /// Deletes the specified environment.
        /// </summary>
        /// <param name="environmentViewModel">An instance of <see cref="EnvironmentsViewModel"/> that represents the environment to be deleted.</param>
        /// <returns>A list of configured environments.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="environmentViewModel"/> is null.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Unable to locate the specified entity or the user is not authorized to perform this operation.
        /// </exception>
        [HttpPost]
        [Route("delete")]
        public async Task<EnvironmentsViewModel> DeleteAsync([FromBody] EnvironmentViewModel environmentViewModel)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            EnvironmentEntity entity;
            EnvironmentsViewModel model;
            List<MailboxEntity> mailboxes;

            environmentViewModel.AssertNotNull(nameof(environmentViewModel));

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                entity = await Service.Storage.GetEntityAsync<EnvironmentEntity>(
                    MigrationConstants.EnvironmentTableName,
                    principal.CustomerId,
                    environmentViewModel.Id);

                if (entity == null)
                {
                    throw new EntityNotFoundException(Resources.EntityNotFoundException);
                }

                if (!string.IsNullOrEmpty(entity.Password))
                {
                    await Service.Vault.DeleteAsync(entity.Password);
                }

                mailboxes = await Service.Storage.GetEntitiesAsync<MailboxEntity>(
                    MigrationConstants.MailboxTableName,
                    m => m.PartitionKey.Equals(environmentViewModel.Id));

                await Service.Storage.DeleteEntitiesAsync(MigrationConstants.MailboxTableName, mailboxes);
                await Service.Storage.DeleteEntityAsync(MigrationConstants.EnvironmentTableName, entity);

                model = await GetEnvironmentsAsync();

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "EnvironmentId", environmentViewModel.Id },
                    { "EnvironmentName", environmentViewModel.Name },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfEnvironments", model.Environments.Count }
                };

                Service.Telemetry.TrackEvent("/api/environment/delete", eventProperties, eventMetrics);

                return model;
            }
            finally
            {
                entity = null;
                eventMetrics = null;
                eventProperties = null;
                mailboxes = null;
                principal = null;
            }
        }

        /// <summary>
        /// Gets a list of the configured environments.
        /// </summary>
        /// <returns>A list of configured environments.</returns>
        [HttpGet]
        [Route("")]
        public async Task<EnvironmentsViewModel> GetEnvironmentsAsync()
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            List<EnvironmentEntity> entities;

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                entities = await Service.Storage.GetEntitiesAsync<EnvironmentEntity>(
                    MigrationConstants.EnvironmentTableName,
                    e => e.PartitionKey.Equals(principal.CustomerId));


                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfEnvironments", entities.Count }
                };

                Service.Telemetry.TrackEvent("/api/environment", eventProperties, eventMetrics);

                return new EnvironmentsViewModel
                {
                    Environments = entities.Select(e => new EnvironmentViewModel
                    {
                        Endpoint = e.Endpoint,
                        Id = e.RowKey,
                        Name = e.Name,
                        Organization = e.Organization,
                        Password = string.Empty,
                        Username = e.Username
                    }).ToList()
                };
            }
            finally
            {
                entities = null;
            }
        }

        /// <summary>
        /// Refreshes the specified environment.
        /// </summary>
        /// <param name="environmentId">Identifier for the environmnet.</param>
        /// <returns>An instance of <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException">
        /// <paramref name="environmentId"/> is empty or null.</exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="environmentId"/> is empty or null.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// Unable to locate the specified entity or the user is not authorized to perform this operation.
        /// </exception>
        [HttpGet]
        [Route("refresh")]
        public async Task RefreshAsync(string environmentId)
        {
            CustomerPrincipal principal;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;
            EnvironmentEntity entity;

            environmentId.AssertNotEmpty(nameof(environmentId));

            try
            {
                startTime = DateTime.Now;
                principal = (CustomerPrincipal)HttpContext.Current.User;

                entity = await Service.Storage.GetEntityAsync<EnvironmentEntity>(
                    MigrationConstants.EnvironmentTableName,
                    principal.CustomerId,
                    environmentId);

                if (entity == null)
                {
                    throw new EntityNotFoundException(Resources.EntityNotFoundException);
                }

                await Service.ServiceBus.WriteToQueueAsync(MigrationConstants.EnvironmentQueueName, entity);

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "Email", principal.Email },
                    { "PrincipalCustomerId", principal.CustomerId }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                };

                Service.Telemetry.TrackEvent("/api/environment/refresh", eventProperties, eventMetrics);
            }
            finally
            {
                entity = null;
            }
        }
    }
}