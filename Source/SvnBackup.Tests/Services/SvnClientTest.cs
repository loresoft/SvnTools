using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using SvnBackup.Services;

namespace SvnBackup.Tests.Services
{
    [TestFixture]
    public class SvnClientTest
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
        public void Checkout()
        {
            string repositoryPath = Path.GetFullPath("SvnClient-Repo");

            if (!Directory.Exists(repositoryPath))
            {
                using (var svnAdmin = new SvnAdmin("create"))
                {
                    svnAdmin.RepositoryPath = repositoryPath;
                    var r = svnAdmin.Execute();
                    Assert.IsTrue(r);
                }
            }

            Uri repoUri = new Uri(repositoryPath);

            using(var client = new SvnClient())
            {
                client.Command = "checkout";
                client.RepositoryPath = repoUri.ToString();
                client.LocalPath = Path.GetFullPath("SvnClient-Work");
                
                var result = client.Execute();

            }
        }
    }
}
