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
            string repositoryPath = TestHelper.TestRepository;

            // make sure there is a repo
            TestHelper.CreateRepository(repositoryPath, false);

            using (var look = new SvnLook("youngest"))
            {
                look.RepositoryPath = repositoryPath;
                var r = look.Execute();

                Assert.IsTrue(r);

                int rev;
                Assert.IsTrue(look.TryGetRevision(out rev));
            }
        }
    }
}
