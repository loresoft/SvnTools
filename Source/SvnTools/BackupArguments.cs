using System;
using System.Collections.Generic;
using System.Text;
using SvnTools.CommandLine;

// $Id$

namespace SvnTools
{
    /// <summary>
    /// A class representing the command line arguments.
    /// </summary>
    public class BackupArguments
    {
        /// <summary>
        /// The number of backups to keep. Default 10.
        /// </summary>
        [Argument(ArgumentType.AtMostOnce,
            ShortName = "n",
            LongName = "history",
            HelpText = "Number of backups to keep. Default 10.")]
        public int History = 10;

        /// <summary>
        /// Compress backup folders.
        /// </summary>
        [Argument(ArgumentType.AtMostOnce,
           ShortName = "c",
           LongName = "compress",
           HelpText = "Compress backup folders.")]
        public bool Compress;

        /// <summary>
        /// Repository root folder.
        /// </summary>
        [Argument(ArgumentType.AtMostOnce | ArgumentType.Required,
           ShortName = "r",
           LongName = "repository",
           HelpText = "Repository root folder.")]
        public string RepositoryRoot;

        /// <summary>
        /// Backup root folder.
        /// </summary>
        [Argument(ArgumentType.AtMostOnce | ArgumentType.Required,
           ShortName = "b",
           LongName = "backup",
           HelpText = "Backup root folder.")]
        public string BackupRoot;

        /// <summary>
        /// Backup root folder.
        /// </summary>
        [Argument(ArgumentType.AtMostOnce,
           LongName = "svn",
           HelpText = "Path to subversion bin folder.")]
        public string SubverisonPath;


    }
}
