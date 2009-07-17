using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using SvnTools.Services;
using SvnTools.Utility;

namespace SvnTools.Tests.Services
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
            string repositoryPath = TestHelper.TestRepository;
            string backupPath = Path.Combine(TestHelper.BackupPath, "Hotcopy.Test");

            // remove existing
            if (Directory.Exists(backupPath))
                PathHelper.DeleteDirectory(backupPath);
            
            Directory.CreateDirectory(backupPath);

            // make sure there is a repo
            TestHelper.CreateRepository(TestHelper.TestRepository, false);

            HotCopy hotcopy = new HotCopy();
            hotcopy.RepositoryPath = repositoryPath;
            hotcopy.BackupPath = backupPath;

            bool result = hotcopy.Execute();

            Assert.IsTrue(result);
            Assert.IsTrue(PathHelper.IsRepository(backupPath));
        }
    }
}
