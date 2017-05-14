// -----------------------------------------------------------------------
// <copyright file="ScriptManager.cs" company="Microsoft">
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
    using Common;

    /// <summary>
    /// Provides the ability to invoke PowerShell commands.
    /// </summary>
    public class ScriptManager : IScriptManager
    {
        /// <summary>
        /// Provides access to core services.
        /// </summary>
        private IMigrationService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptManager"/> class.
        /// </summary>
        /// <param name="service">Provides access to core services.</param>
        public ScriptManager(IMigrationService service)
        {
            service.AssertNotNull(nameof(service));
            this.service = service;
        }

        /// <summary>
        /// Invokes the given command in the specified runspace.
        /// </summary>
        /// <param name="runspace">An instance of <see cref="Runspace"/> used to invoke the command.</param>
        /// <param name="commmand">An instance of <see cref="Command"/> representing the command to be invoked.</param>
        /// <returns>A collection of <see cref="PSObject"/>s that represent the output from the command.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="runspace"/> is null.
        /// or
        /// <paramref name="command"/> is null.
        /// </exception>
        public Collection<PSObject> InvokeCommand(Runspace runspace, Command command)
        {
            Collection<PSObject> results;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;

            command.AssertNotNull(nameof(command));
            runspace.AssertNotNull(nameof(runspace));

            try
            {
                startTime = DateTime.Now;

                runspace.Open();

                using (Pipeline pipeline = runspace.CreatePipeline())
                {
                    pipeline.Commands.Add(command);
                    results = pipeline.Invoke();

                    // Ensure that no errors were encountered with the script execution.
                    ValidatePipeline(pipeline);
                }

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CommandText", command.CommandText }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                service.Telemetry.TrackEvent("InvokeCommand", eventProperties, eventMetrics);

                return results;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
            }
        }

        /// <summary>
        /// Invokes the given command in the specified runspace.
        /// </summary>
        /// <param name="runspace">An instance of <see cref="Runspace"/> used to invoke the command.</param>
        /// <param name="commmand">An instance of <see cref="Command"/> representing the command to be invoked.</param>
        /// <param name="parameters">A collection of parameters to be included when invoking the command.</param>
        /// <returns>A collection of <see cref="PSObject"/>s that represent the output from the command.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="runspace"/> is null.
        /// or
        /// <paramref name="command"/> is null.
        /// or
        /// <paramref name="parameters"/> is null.
        /// </exception>
        public Collection<PSObject> InvokeCommand(Runspace runspace, Command command, CommandParameterCollection parameters)
        {
            Collection<PSObject> results;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;

            command.AssertNotNull(nameof(command));
            parameters.AssertNotNull(nameof(parameters));
            runspace.AssertNotNull(nameof(runspace));

            try
            {
                startTime = DateTime.Now;

                runspace.Open();

                using (Pipeline pipeline = runspace.CreatePipeline())
                {
                    foreach (CommandParameter parameter in parameters)
                    {
                        command.Parameters.Add(parameter);
                    }

                    pipeline.Commands.Add(command);
                    results = pipeline.Invoke();

                    // Ensure that no errors were encountered with the script execution.
                    ValidatePipeline(pipeline);
                }

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CommandText", command.CommandText }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds }
                };

                service.Telemetry.TrackEvent("InvokeCommand", eventProperties, eventMetrics);

                return results;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
            }
        }

        /// <summary>
        /// Invokes the given commands in the specified runspace.
        /// </summary>
        /// <param name="runspace">An instance of <see cref="Runspace"/> used to invoke the command.</param>
        /// <param name="commands">A list of commands to be invoked.</param>
        /// <param name="parameters">A list of command parameters to be included when invoking the commands.</param>
        /// <returns>A collection of <see cref="PSObject"/>s that represents the output from the commands.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="runspace"/> is null.
        /// or 
        /// <paramref name="commands"/> is null.
        /// </exception>
        public Collection<PSObject> InvokeCommand(Runspace runspace, List<Command> commands)
        {
            Collection<PSObject> results;
            DateTime startTime;
            Dictionary<string, double> eventMetrics;
            Dictionary<string, string> eventProperties;

            runspace.AssertNotNull(nameof(runspace));
            commands.AssertNotNull(nameof(commands));

            try
            {
                startTime = DateTime.Now;

                runspace.Open();

                using (Pipeline pipeline = runspace.CreatePipeline())
                {
                    foreach (Command command in commands)
                    {
                        pipeline.Commands.Add(command);
                    }

                    results = pipeline.Invoke();

                    // Ensure that no errors were encountered with the script execution.
                    ValidatePipeline(pipeline);
                }

                // Capture the request for the customer summary for analysis.
                eventProperties = new Dictionary<string, string>
                {
                    { "CommandText", "" }
                };

                // Track the event measurements for analysis.
                eventMetrics = new Dictionary<string, double>
                {
                    { "ElapsedMilliseconds", DateTime.Now.Subtract(startTime).TotalMilliseconds },
                    { "NumberOfCommands", commands.Count }
                };

                service.Telemetry.TrackEvent("InvokeCommand", eventProperties, eventMetrics);

                return results;
            }
            finally
            {
                eventMetrics = null;
                eventProperties = null;
            }
        }

        /// <summary>
        /// Validates that the pipeline was invoked successfully. 
        /// </summary>
        /// <param name="pipeline">An instance of <see cref="Pipeline"/> that has been invoked.</param>
        /// <exception cref="AutomationException">
        /// Error message from the pipeline.
        /// </exception>
        private void ValidatePipeline(Pipeline pipeline)
        {
            string errorMessage;

            pipeline.AssertNotNull(nameof(pipeline));

            if (pipeline.HadErrors)
            {

                errorMessage = pipeline.Error.ReadToEnd().Aggregate(new StringBuilder(),
                    (sb, e) => sb.Append(e.ToString()).Append("\n"),
                    sb => { if (0 < sb.Length) sb.Length--; return sb.ToString(); });

                throw new AutomationException(errorMessage);
            }
        }
    }
}