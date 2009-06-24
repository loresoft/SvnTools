using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SvnTools.Tests
{
    [TestFixture]
    public class BackupTest
    {
        [Test]
        public void BasicRun()
        {
            BackupArguments args = new BackupArguments();
            args.BackupRoot = @"D:\svn\backup";
            args.Compress = true;
            args.RepositoryRoot = @"D:\svn\repo";

            Backup.Run(args);

        }
    }
}
