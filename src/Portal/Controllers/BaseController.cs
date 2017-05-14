// -----------------------------------------------------------------------
// <copyright file="BaseController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Controllers
{
    using System.Web.Mvc;
    using Common;

    /// <summary>
    /// Base controller for all MVC controllers.
    /// </summary>
    /// <seealso cref="Controller" />
    public abstract class BaseController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="service"/> is null.
        /// </exception>
        protected BaseController(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));
            Service = service;
        }

        /// <summary>
        /// Gets a reference to the core migration service.
        /// </summary>
        protected IMigrationService Service { get; private set; }
    }
}