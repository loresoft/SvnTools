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
    public class SvnClientTest
    {
        [SetUp]
        public void Setup()
        {
            TestHelper.CreateRepository(TestHelper.TestRepository, false);
        }

        [TearDown]
        public void TearDown()
        {
            //TODO: NUnit TearDown
        }

        [Test]
        public void Checkout()
        {
            string workingPath = Path.Combine(TestHelper.WorkingPath, "Checkout.Test");            
            PathHelper.DeleteDirectory(workingPath);
            
            using(var client = new SvnClient(SvnClient.Commands.Checkout))
            {
                client.RepositoryPath = TestHelper.TestRepositoryUri.ToString();
                client.LocalPath = workingPath;
                
                var result = client.Execute();

                Assert.IsTrue(result);

                Console.WriteLine(client.StandardOutput);
                Console.WriteLine(client.StandardError);
            }
        }

        [Test]
        public void Import()
        {
            string repositoryPath = Path.Combine(TestHelper.ParentPath, "SvnClientTest.Import");
            TestHelper.CreateRepository(repositoryPath, true);

            string workingPath = Path.Combine(TestHelper.WorkingPath, "Import.Test");
            PathHelper.CreateDirectory(workingPath);

            string testFile = Path.Combine(workingPath, "Readme.txt");
            File.WriteAllText(testFile, "This is a read me text file.");

            Uri repo = new Uri(repositoryPath);

            using (var client = new SvnClient(SvnClient.Commands.Import))
            {
                client.RepositoryPath = repo.ToString();
                client.LocalPath = workingPath;
                client.Message = "First Import";

                var result = client.Execute();

                Assert.IsTrue(result);

                Console.WriteLine(client.StandardOutput);
                Console.WriteLine(client.StandardError);
            }
        }
    }
}
