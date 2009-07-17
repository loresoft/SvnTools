using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SvnBackup.Tests
{
    [TestFixture]
    public class BackupTest
    {
        [SetUp]
        public void Setup()
        {
            if (Directory.Exists("Backup"))
                Directory.Delete("Backup", true);
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void RunCompressHistoryTen()
        {

            BackupArguments args = new BackupArguments();
            args.BackupRoot = Path.GetFullPath("backup");
            args.Compress = true;
            args.History = 10;
            args.RepositoryRoot = @"D:\svn\repo";

            Backup.Run(args);

        }


        [Test]
        public void RunCompressHistoryTwo()
        {

            BackupArguments args = new BackupArguments();
            args.BackupRoot = Path.GetFullPath("backup");
            args.Compress = true;
            args.History = 2;
            args.RepositoryRoot = @"D:\svn\repo";

            Backup.Run(args);

        }


        [Test]
        public void RunHistoryTen()
        {

            BackupArguments args = new BackupArguments();
            args.BackupRoot = Path.GetFullPath("backup");
            args.Compress = false;
            args.History = 10;
            args.RepositoryRoot = @"D:\svn\repo";

            Backup.Run(args);

        }


       
    }
}
