using System;
using System.Collections.Generic;
using System.Text;
using SvnTools.Utility;

// $Id$

namespace SvnTools.Services
{
    /// <summary>
    /// A class encapsulating SvnAdmin process.
    /// </summary>
    public class SvnAdmin : ProcessBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SvnAdmin"/> class.
        /// </summary>
        public SvnAdmin()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnAdmin"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        public SvnAdmin(string command)
        {
            Command = command;
        }

        /// <summary>
        /// Gets or sets the repository path.
        /// </summary>
        /// <value>The repository path.</value>
        public string RepositoryPath { get; set; }

        /// <summary>
        /// Gets or sets the SvnAdmin command.
        /// </summary>
        /// <value>The SvnAdmin command.</value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the arguments to pass to SvnAdmin.
        /// </summary>
        /// <value>The arguments to pass to SvnAdmin.</value>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="SvnAdmin"/> is quiet.
        /// </summary>
        /// <value><c>true</c> if quiet; otherwise, <c>false</c>.</value>
        public bool Quiet { get; set; }

        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value>The name of the executable file to run.</value>
        protected override string ToolName
        {
            get { return "svnadmin.exe"; }
        }
        /// <summary>
        /// Indicates whether all task parameters are valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if all task parameters are valid; otherwise, <c>false</c>c&gt;.
        /// </returns>
        protected internal override bool ValidateParameters()
        {
            if (string.IsNullOrEmpty(Command))
            {
                AppendStandardError("The Command property is required.");
                return false;
            }
            return base.ValidateParameters();
        }

        /// <summary>
        /// Generates the command line arguments.
        /// </summary>
        /// <returns>
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {
            var commandLine = new CommandLineBuilder();
            AppendCommand(commandLine);
            AppendArguments(commandLine);
            return commandLine.ToString();
        }


        /// <summary>
        /// Generates the SvnAdmin command.
        /// </summary>
        /// <returns></returns>
        protected virtual void AppendCommand(CommandLineBuilder commandLine)
        {
            commandLine.AppendSwitch(Command);
            commandLine.AppendFileNameIfNotNull(RepositoryPath);
        }

        /// <summary>
        /// Generates the SvnAdmin arguments.
        /// </summary>
        /// <returns></returns>
        protected virtual void AppendArguments(CommandLineBuilder commandLine)
        {
            commandLine.AppendSwitchIfTrue("--quiet", Quiet);

            // raw arguments
            commandLine.AppendSwitchIfNotNull("", Arguments);
        }
    }

}
