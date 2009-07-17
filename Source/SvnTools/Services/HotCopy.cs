using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SvnTools.Utility;

// $Id$

namespace SvnTools.Services
{
    /// <summary>
    /// A class encapsulating the hotcopy command from svnadmin.
    /// </summary>
    public class HotCopy : SvnAdmin
    {
        public HotCopy()
        {
            Command = "hotcopy";
            Arguments = "--clean-logs";
        }

        /// <summary>
        /// Gets or sets the backup path.
        /// </summary>
        /// <value>The backup path.</value>
        public string BackupPath { get; set; }
        
        /// <summary>
        /// Generates the command line arguments.
        /// </summary>
        /// <returns>
        /// Returns a string value containing the command line arguments to pass directly to the executable file.
        /// </returns>
        protected override void AppendCommand(CommandLineBuilder commandLine)
        {
            base.AppendCommand(commandLine);
            commandLine.AppendFileNameIfNotNull(BackupPath);
        }
    }
}
