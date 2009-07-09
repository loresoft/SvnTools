using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SvnBackup.Utility;

// $Id$

namespace SvnBackup.Services
{
    /// <summary>
    /// A class encapsulating the hotcopy command from svnadmin.
    /// </summary>
    public class HotCopy : ProcessBase
    {
        /// <summary>
        /// Gets or sets the repository path.
        /// </summary>
        /// <value>The repository path.</value>
        public string RepositoryPath { get; set; }

        /// <summary>
        /// Gets or sets the backup path.
        /// </summary>
        /// <value>The backup path.</value>
        public string BackupPath { get; set; }

        /// <summary>
        /// Gets the name of the executable file to run.
        /// </summary>
        /// <value>The name of the executable file to run.</value>
        protected override string ToolName
        {
            get { return "svnadmin.exe"; }
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
            command.AppendSwitch("hotcopy");
            command.AppendFileNameIfNotNull(RepositoryPath);
            command.AppendFileNameIfNotNull(BackupPath);

            return command.ToString();
        }
    }
}
