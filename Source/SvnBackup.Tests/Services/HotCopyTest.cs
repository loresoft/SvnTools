using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using SvnBackup.Services;
using SvnBackup.Utility;

namespace SvnBackup.Tests.Services
{
    [TestFixture]
    public class HotCopyTest
    {
        [SetUp]
        public void Setup()
        {
            //TODO: NUnit setup
        }

        [TearDown]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test]
        public void Execute()
        {
            string repositoryPath = @"D:\svn\repo\Calculator";
            string backupPath = Path.GetFullPath(@"Calculator-backup");

            if (Directory.Exists(backupPath))
                PathHelper.DeleteDirectory(backupPath);
            
            if (!Directory.Exists(backupPath))
                Directory.CreateDirectory(backupPath);
            
            HotCopy hotcopy = new HotCopy();
            hotcopy.RepositoryPath = repositoryPath;
            hotcopy.BackupPath = backupPath;

            bool result = hotcopy.Execute();

            Assert.IsTrue(result);
        }
    }
}
