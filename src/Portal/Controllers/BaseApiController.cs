// -----------------------------------------------------------------------
// <copyright file="BaseApiController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Controllers
{
    using System.Web.Http;
    using Common; 

    /// <summary>
    /// Base controller for all API controllers.
    /// </summary>
    public class BaseApiController : ApiController
    {
        /// <summary>
        /// Provides access to the core services.
        /// </summary>
        private readonly IMigrationService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseApiController"/> class.
        /// </summary>
        /// <param name="service">Provides access to the core application services.</param>
        protected BaseApiController(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));

            this.service = service;
        }

        /// <summary>
        /// Provides access to the core application services.
        /// </summary>
        protected IMigrationService Service => service;
    }
}