using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SvnTools.Utility;

namespace SvnTools.Tests
{
    [TestFixture]
    public class BackupTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void RunCompressHistoryTen()
        {
            string repositoryPath = Path.Combine(TestHelper.ParentPath, "RunCompressHistoryTen");
            string workingPath = Path.Combine(TestHelper.WorkingPath, "RunCompressHistoryTen");

            PathHelper.DeleteDirectory(TestHelper.BackupPath);
            PathHelper.CreateDirectory(TestHelper.BackupPath);

            TestHelper.CreateAndImport(repositoryPath);
            TestHelper.CreateWorking(repositoryPath, workingPath);

            string readmePath = Path.Combine(workingPath, "Readme.txt");

            BackupArguments args = new BackupArguments();
            args.RepositoryRoot = TestHelper.ParentPath;
            args.BackupRoot = TestHelper.BackupPath;
            args.Compress = true;
            args.History = 10;

            Backup.Run(args);

            // simulate changes ...
            for (int i = 0; i < 11; i++)
            {
                File.AppendAllText(readmePath, "Ticks: " + DateTime.Now.Ticks + Environment.NewLine);
                TestHelper.CommitWorking(workingPath);
                Backup.Run(args);
            }

        }


        [Test]
        public void RunCompressHistoryTwo()
        {
            string repositoryPath = Path.Combine(TestHelper.ParentPath, "RunCompressHistoryTwo");
            string workingPath = Path.Combine(TestHelper.WorkingPath, "RunCompressHistoryTwo");

            PathHelper.DeleteDirectory(TestHelper.BackupPath);
            PathHelper.CreateDirectory(TestHelper.BackupPath);

            TestHelper.CreateAndImport(repositoryPath);
            TestHelper.CreateWorking(repositoryPath, workingPath);

            string readmePath = Path.Combine(workingPath, "Readme.txt");

            BackupArguments args = new BackupArguments();
            args.RepositoryRoot = TestHelper.ParentPath;
            args.BackupRoot = TestHelper.BackupPath;
            args.Compress = true;
            args.History = 2;

            Backup.Run(args);

            // simulate changes ...
            for (int i = 0; i < 11; i++)
            {
                File.AppendAllText(readmePath, "Ticks: " + DateTime.Now.Ticks + Environment.NewLine);
                TestHelper.CommitWorking(workingPath);
                Backup.Run(args);
            }
        }


        [Test]
        public void RunHistoryTen()
        {

            string repositoryPath = Path.Combine(TestHelper.ParentPath, "RunHistoryTen");
            string workingPath = Path.Combine(TestHelper.WorkingPath, "RunHistoryTen");

            PathHelper.DeleteDirectory(TestHelper.BackupPath);
            PathHelper.CreateDirectory(TestHelper.BackupPath);

            TestHelper.CreateAndImport(repositoryPath);
            TestHelper.CreateWorking(repositoryPath, workingPath);

            string readmePath = Path.Combine(workingPath, "Readme.txt");

            BackupArguments args = new BackupArguments();
            args.RepositoryRoot = TestHelper.ParentPath;
            args.BackupRoot = TestHelper.BackupPath;
            args.Compress = false;
            args.History = 10;

            Backup.Run(args);

            // simulate changes ...
            for (int i = 0; i < 11; i++)
            {
                File.AppendAllText(readmePath, "Ticks: " + DateTime.Now.Ticks + Environment.NewLine);
                TestHelper.CommitWorking(workingPath);
                Backup.Run(args);
            }
        }

    }
}
