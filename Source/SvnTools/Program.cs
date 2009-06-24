using System;
using System.Collections.Generic;
using System.Text;
using SvnTools.CommandLine;

// $Id$

namespace SvnTools
{
    class Program
    {
        static int Main(string[] args)
        {
            if (Parser.ParseHelp(args))
            {
                OutputUsageHelp();
                return 0;
            }

            BackupArguments arguments = new BackupArguments();
            if (!Parser.ParseArguments(args, arguments))
            {
                OutputUsageHelp();
                return 1;
            }

            Backup.Run(arguments);

            return 0;
        }

        private static void OutputUsageHelp()
        {
            Console.WriteLine();
            Console.WriteLine("SvnTools.exe /r:<directory> /b:<directory> /c");
            Console.WriteLine();
            Console.WriteLine("     - BACKUP OPTIONS -");
            Console.WriteLine();
            Console.WriteLine(Parser.ArgumentsUsage(typeof(BackupArguments)));
        }

        private static void OutputHeader()
        {
            Console.WriteLine(@"SvnTools v" + ThisAssembly.AssemblyInformationalVersion);
        }
    }
}
