using System;
using System.Collections.Generic;
using System.Text;
using SvnTools;
using SvnTools.CommandLine;

// $Id$

namespace SvnBackup
{
    class Program
    {
        static int Main(string[] args)
        {
            if (Parser.ParseHelp(args))
            {
                OutputHeader();
                OutputUsageHelp();
                return 0;
            }

            StringBuilder errorBuffer = new StringBuilder();
            BackupArguments arguments = new BackupArguments();
            if (!Parser.ParseArguments(args, arguments, s => errorBuffer.AppendLine(s)))
            {
                OutputHeader();
                Console.Error.WriteLine(errorBuffer.ToString());
                OutputUsageHelp();
                return 1;
            }

            Backup.Run(arguments);

            return 0;
        }

        private static void OutputUsageHelp()
        {
            Console.WriteLine();
            Console.WriteLine("SvnBackup.exe /r:<directory> /b:<directory> /c");
            Console.WriteLine();
            Console.WriteLine("     - BACKUP OPTIONS -");
            Console.WriteLine();
            Console.WriteLine(Parser.ArgumentsUsage(typeof(BackupArguments)));
        }

        private static void OutputHeader()
        {
            Console.WriteLine("SvnBackup v{0}", ThisAssembly.AssemblyInformationalVersion);
            Console.WriteLine();
        }
    }
}
