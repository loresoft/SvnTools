using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SvnTools.Utility;

// $Id$

namespace SvnTools.Services
{
    /// <summary>
    /// A class encapsulating SvnLook process.
    /// </summary>
    public class SvnLook : ProcessBase
    {
        private static readonly Regex _revisionParse = new Regex(@"\b(?<Rev>\d+)", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnLook"/> class.
        /// </summary>
        public SvnLook()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SvnLook"/> class.
        /// </summary>
        /// <param name="command">The SvnLook command.</param>
        public SvnLook(string command)
        {
            Command = command;
        }

        /// <summary>
        /// Gets or sets the repository path.
        /// </summary>
        /// <value>The repository path.</value>
        public string RepositoryPath { get; set; }

        /// <summary>
        /// Gets or sets the SvnLook command.
        /// </summary>
        /// <value>The SvnLook command.</value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the arguments to pass to SvnLook.
        /// </summary>
        /// <value>The arguments to pass to SvnLook.</value>
        public string Arguments { get; set; }


        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value>The name of the executable file to run.</value>
        protected override string ToolName
        {
            get { return "svnlook.exe"; }
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
        /// Generates the SvnLook command.
        /// </summary>
        /// <returns></returns>
        protected virtual void AppendCommand(CommandLineBuilder commandLine)
        {
            commandLine.AppendSwitch(Command);
            commandLine.AppendFileNameIfNotNull(RepositoryPath);
        }

        /// <summary>
        /// Generates the SvnLook arguments.
        /// </summary>
        /// <returns></returns>
        protected virtual void AppendArguments(CommandLineBuilder commandLine)
        {
            // raw arguments
            if (!string.IsNullOrEmpty(Arguments))
                commandLine.AppendSwitch(Arguments);
        }

        /// <summary>
        /// Try to get the revision from the StandardOutput.
        /// </summary>
        /// <param name="revision">The revision.</param>
        /// <returns><c>true</c> if revision was found; otherwise <c>false</c>.</returns>
        public bool TryGetRevision(out int revision)
        {
            revision = 0;

            if (string.IsNullOrEmpty(StandardOutput))
                return false;

            Match revMatch = _revisionParse.Match(StandardOutput);
            if (!revMatch.Success)
                return false;

            string tempRev = revMatch.Groups["Rev"].Value;
            return int.TryParse(tempRev, out revision);
        }

        /// <summary>
        /// The commands for <see cref="SvnLook"/>.
        /// </summary>
        public static class Commands
        {
            /// <summary>History command.</summary>
            public const string History = "history";
            /// <summary>Info command.</summary>
            public const string Info = "info";
            /// <summary>Log command.</summary>
            public const string Log = "log";
            /// <summary>Tree command.</summary>
            public const string Tree = "tree";
            /// <summary>Youngest command.</summary>
            public const string Youngest = "youngest";
        }

    }
}
