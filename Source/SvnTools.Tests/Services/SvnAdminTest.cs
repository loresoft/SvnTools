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
    public class SvnAdminTest
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
        public void Create()
        {
            string repositoryPath = Path.Combine(TestHelper.ParentPath, "SvnAdminTest.Create");
            PathHelper.DeleteDirectory(repositoryPath);

            SvnAdmin svnAdmin = new SvnAdmin("create");
            svnAdmin.RepositoryPath = repositoryPath;
            bool r = svnAdmin.Execute();

            Assert.IsTrue(r);
            Assert.IsTrue(PathHelper.IsRepository(repositoryPath));
        }

        [Test]
        public void Dump()
        {
            string repositoryPath = Path.Combine(TestHelper.ParentPath, "SvnAdminTest.Create");

            TestHelper.CreateRepository(repositoryPath, false);

            SvnAdmin svnAdmin = new SvnAdmin(SvnAdmin.Commands.Dump);
            svnAdmin.RepositoryPath = repositoryPath;
            
            bool r = svnAdmin.Execute();
            
            Assert.IsTrue(r);
        }
    }
}
