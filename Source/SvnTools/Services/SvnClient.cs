using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SvnTools.Utility;

namespace SvnTools.Services
{
    /// <summary>
    /// A class encapsulating svn process.
    /// </summary>
    public class SvnClient : ProcessBase
    {
        private static readonly Regex _revisionParse = new Regex(@"\b(?<Rev>\d+)", RegexOptions.Compiled);

        public SvnClient(string command)
            : this()
        {
            Command = command;
        }

        public SvnClient()
        {
            NonInteractive = true;
            NoAuthCache = true;
        }

        /// <summary>
        /// Gets or sets the repository path.
        /// </summary>
        /// <value>The repository path.</value>
        public string RepositoryPath { get; set; }

        /// <summary>
        /// Gets or sets the svn command.
        /// </summary>
        /// <value>The svn command.</value>
        public string Command { get; set; }

        /// <summary>
        /// Gets or sets the arguments to pass to svn.
        /// </summary>
        /// <value>The arguments to pass to svn.</value>
        public string Arguments { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the local path.
        /// </summary>
        /// <value>The local path.</value>
        public string LocalPath { get; set; }

        /// <summary>
        /// Gets or sets the verbose.
        /// </summary>
        /// <value>The verbose.</value>
        public bool Verbose { get; set; }

        /// <summary>
        /// Gets or sets the force.
        /// </summary>
        /// <value>The force.</value>
        public bool Force { get; set; }

        /// <summary>
        /// Gets or sets a value indicating to run in non interactive mode.
        /// </summary>
        /// <value><c>true</c> if non interactive; otherwise, <c>false</c>.</value>
        public bool NonInteractive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating no authorization cache.
        /// </summary>
        /// <value><c>true</c> if no authorization cache; otherwise, <c>false</c>.</value>
        public bool NoAuthCache { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the return result as XML.
        /// </summary>
        /// <value><c>true</c> to return XML; otherwise, <c>false</c>.</value>
        public bool Xml { get; set; }

        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value>The name of the executable file to run.</value>
        protected override string ToolName
        {
            get { return "svn.exe"; }
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
        /// Generates the svn command.
        /// </summary>
        /// <returns></returns>
        protected virtual void AppendCommand(CommandLineBuilder commandLine)
        {
            commandLine.AppendSwitch(Command);
            commandLine.AppendFileNameIfNotNull(RepositoryPath);
            commandLine.AppendFileNameIfNotNull(LocalPath);
        }

        /// <summary>
        /// Generates the svn arguments.
        /// </summary>
        /// <returns></returns>
        protected virtual void AppendArguments(CommandLineBuilder commandLine)
        {
            commandLine.AppendSwitchIfNotNull("--username", Username);
            commandLine.AppendSwitchIfNotNull("--password", Password);
            commandLine.AppendSwitchIfNotNull("--message", Message);

            commandLine.AppendSwitchIfTrue("--force", Force);
            commandLine.AppendSwitchIfTrue("--verbose", Verbose);

            commandLine.AppendSwitchIfTrue("--xml", Xml);
            commandLine.AppendSwitchIfTrue("--non-interactive", NonInteractive);
            commandLine.AppendSwitchIfTrue("--no-auth-cache", NoAuthCache);

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
    }
}

