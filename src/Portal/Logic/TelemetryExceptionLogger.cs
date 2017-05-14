// -----------------------------------------------------------------------
// <copyright file="TelemetryExceptionLogger.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.Portal.Logic
{
    using System.Web.Http.ExceptionHandling;
    using Common;
    using Practices.Unity;

    /// <summary>
    /// Represents custom exception handler that logs the exception to the configured telemetry provider.
    /// </summary>
    public class TelemetryExceptionLogger : ExceptionLogger
    {
        public override void Log(ExceptionLoggerContext context)
        {
            IMigrationService service;

            try
            {
                if (context?.Exception != null)
                {
                    service = MvcApplication.UnityContainer.Resolve<IMigrationService>();
                    service.Telemetry.TrackException(context.Exception);
                }

                base.Log(context);
            }
            finally
            {
                service = null;
            }
        }
    }
}