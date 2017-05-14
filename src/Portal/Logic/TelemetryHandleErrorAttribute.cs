// -----------------------------------------------------------------------
// <copyright file="TelemetryHandleErrorAttribute.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Logic
{
    using System;
    using System.Web.Mvc;
    using Common;
    using Practices.Unity;

    /// <summary>
    /// Represents custom handle error attribute that logs the exception to the configured telemetry provider.
    /// </summary>
    /// <seealso cref="HandleErrorAttribute" />
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class TelemetryHandleErrorAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// Called when an exception occurs.
        /// </summary>
        /// <param name="filterContext">The action-filter context.</param>
        public override void OnException(ExceptionContext filterContext)
        {
            IMigrationService service;

            try
            {
                service = MvcApplication.UnityContainer.Resolve<IMigrationService>();

                if (filterContext?.HttpContext != null && filterContext.Exception != null)
                {
                    if (filterContext.HttpContext.IsCustomErrorEnabled)
                    {
                        service.Telemetry.TrackException(filterContext.Exception);
                    }
                }

                base.OnException(filterContext);
            }
            finally
            {
                service = null;
            }
        }
    }
}