// -----------------------------------------------------------------------
// <copyright file="IScriptManager.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Hex.Migration.Toolkit.WebJob.Automation
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;

    /// <summary>
    /// Represents the ability to invoke PowerShell commands.
    /// </summary>
    public interface IScriptManager
    {
        /// <summary>
        /// Invokes the given command in the specified runspace.
        /// </summary>
        /// <param name="runspace">An instance of <see cref="Runspace"/> used to invoke the command.</param>
        /// <param name="commmand">An instance of <see cref="Command"/> representing the command to be invoked.</param>
        /// <returns>A collection of <see cref="PSObject"/>s that represents the output from the command.</returns>
        Collection<PSObject> InvokeCommand(Runspace runspace, Command commmand);

        /// <summary>
        /// Invokes the given command in the specified runspace.
        /// </summary>
        /// <param name="runspace">An instance of <see cref="Runspace"/> used to invoke the command.</param>
        /// <param name="commmand">An instance of <see cref="Command"/> representing the command to be invoked.</param>
        /// <param name="parameters">A collection of parameters to be included when invoking the command.</param>
        /// <returns>A collection of <see cref="PSObject"/>s that represents the output from the command.</returns>
        Collection<PSObject> InvokeCommand(Runspace runspace, Command command, CommandParameterCollection parameters);

        /// <summary>
        /// Invokes the given commands in the specified runspace.
        /// </summary>
        /// <param name="runspace">An instance of <see cref="Runspace"/> used to invoke the command.</param>
        /// <param name="commands">A list of commands to be invoked.</param>
        /// <returns>A collection of <see cref="PSObject"/>s that represents the output from the commands.</returns>
        Collection<PSObject> InvokeCommand(Runspace runspace, List<Command> commands);
    }
}