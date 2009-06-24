using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SvnTools.Utility;

// $Id$

namespace SvnTools.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class SvnVersion : ProcessBase
    {
        private static readonly Regex _revisionParse = new Regex(@"\b(?<Rev>\d+)", RegexOptions.Compiled);

        /// <summary>
        /// Gets or sets the repository path.
        /// </summary>
        /// <value>The repository path.</value>
        public string RepositoryPath { get; set; }


        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value>The name of the executable file to run.</value>
        protected override string ToolName
        {
            get { return "svnlook.exe"; }
        }

        /// <summary>
        /// Generates the command line arguments.
        /// </summary>
        /// <returns>
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </returns>
        protected override string GenerateCommandLineCommands()
        {
            var command = new CommandLineBuilder();
            command.AppendSwitch("youngest");
            command.AppendFileNameIfNotNull(RepositoryPath);

            return command.ToString();
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
