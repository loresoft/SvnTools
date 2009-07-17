using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using SvnTools.Services;

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
            SvnAdmin svnAdmin = new SvnAdmin("create");
            svnAdmin.RepositoryPath = Path.GetFullPath("SvnAdmin-Repo");
            bool r = svnAdmin.Execute();

            Assert.IsTrue(r);
        }

        [Test]
        public void Dump()
        {
            SvnAdmin svnAdmin;
            bool r;

            string repositoryPath = Path.GetFullPath("SvnAdmin-Repo");

            if (!Directory.Exists(repositoryPath))
            {
                svnAdmin = new SvnAdmin("create");

                svnAdmin.RepositoryPath = repositoryPath;
                r = svnAdmin.Execute();

                Assert.IsTrue(r);
            }

            svnAdmin = new SvnAdmin("dump");
            svnAdmin.RepositoryPath = repositoryPath;
            
            r = svnAdmin.Execute();
            
            Assert.IsTrue(r);
        }
    }
}
