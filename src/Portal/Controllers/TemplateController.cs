// -----------------------------------------------------------------------
// <copyright file="TemplateController.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Common;
    using Configuration;
    using Configuration.Manager;

    /// <summary>
    /// Serves HTML templates to the browser.
    /// </summary>
    public class TemplateController : BaseController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateController"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public TemplateController(IMigrationService service) : base(service)
        {
        }

        /// <summary>
        /// Servers the HTML template for the environment add new page presenter. 
        /// </summary>
        /// <returns>The HTML for the environment add new page presenter.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult EnvironmentAddNew()
        {
            return PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the homepage presenter.
        /// </summary>
        /// <returns>The HTML template for the homepage presenter.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Home()
        {
            return PartialView();
        }

        /// <summary>
        /// Serves the HTML templates for the framework controls and services.
        /// </summary>
        /// <returns>The HTML template for the framework controls and services.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public async Task<ActionResult> FrameworkFragments()
        {
            WebPortalConfigurationManager builder = PortalConfiguration.WebPortalConfigurationManager;

            ViewBag.Templates = (await builder.AggregateNonStartupAssets()).Templates;

            return PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the migration batch page.
        /// </summary>
        /// <returns>The HTML template for the migration batch page.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult MigrationBatch()
        {
            return PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the migration batches page.
        /// </summary>
        /// <returns>The HTML template for the migration batches page.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult MigrationBatches()
        {
            return PartialView();
        }

        /// <summary>
        /// Servers the HTML template for the migration batch add new page presenter. 
        /// </summary>
        /// <returns>The HTML for the migration batch add new page presenter.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult MigrationBatchAddNew()
        {
            return PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the registration confirmation page.
        /// </summary>
        /// <returns>The HTML template for the registration confirmation page.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult RegistrationConfirmation()
        {
            return PartialView();
        }

        /// <summary>
        /// Serves the HTML template for the subscription page presenter.
        /// </summary>
        /// <returns>The HTML template for the subscription page presenter.</returns>
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Subscription()
        {
            return PartialView();
        }
    }
}