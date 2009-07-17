using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using SvnTools.Services;

namespace SvnTools.Tests.Services
{
    [TestFixture]
    public class SvnLookTest
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
        public void Youngest()
        {
            string repositoryPath = Path.GetFullPath("SvnLook-Repo");

            if (!Directory.Exists(repositoryPath))
            {
                using (var svnAdmin = new SvnAdmin("create"))
                {
                    svnAdmin.RepositoryPath = repositoryPath;
                    var r = svnAdmin.Execute();
                    Assert.IsTrue(r);
                }                
            }

            using (var look = new SvnLook("youngest"))
            {
                look.RepositoryPath = repositoryPath;
                var lr = look.Execute();

                Assert.IsTrue(lr);

                int rev;
                Assert.IsTrue(look.TryGetRevision(out rev));
            }
        }
    }
}
