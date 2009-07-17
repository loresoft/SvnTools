using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SvnTools.Services;
using SvnTools.Utility;

namespace SvnTools.Tests
{
    public static class TestHelper
    {
        static TestHelper()
        {
            TestRoot = Path.GetFullPath(@"..\..\Test");
            if (!Directory.Exists(TestRoot))
                Directory.CreateDirectory(TestRoot);

            ParentPath = Path.Combine(TestRoot, @"svn\repo");
            if (!Directory.Exists(ParentPath))
                Directory.CreateDirectory(ParentPath);

            TestRepository = Path.Combine(ParentPath, "Test");
            TestRepositoryUri = new Uri(TestRepository);

            BackupPath = Path.Combine(TestRoot, @"svn\backup");
            if (!Directory.Exists(BackupPath))
                Directory.CreateDirectory(BackupPath);

            WorkingPath = Path.Combine(TestRoot, @"working");
            if (!Directory.Exists(WorkingPath))
                Directory.CreateDirectory(WorkingPath);

        }

        public static string TestRoot { get; private set; }
        public static string ParentPath { get; private set; }
        public static string TestRepository { get; private set; }
        public static Uri TestRepositoryUri { get; private set; }
        public static string BackupPath { get; private set; }

        public static string WorkingPath { get; private set; }

        public static bool CreateRepository(string repositoryPath, bool recreate)
        {
            if (PathHelper.IsRepository(repositoryPath))
                if (recreate)
                    PathHelper.DeleteDirectory(repositoryPath);
                else
                    return true;    

            using (var svnAdmin = new SvnAdmin(SvnAdmin.Commands.Create))
            {
                svnAdmin.RepositoryPath = repositoryPath;
                return svnAdmin.Execute();
            }
        }

        public static bool CreateAndImport(string repositoryPath)
        {
            CreateRepository(repositoryPath, true);

            string workingPath = Path.Combine(WorkingPath, "Create.Readme");
            PathHelper.CreateDirectory(workingPath);

            string testFile = Path.Combine(workingPath, "Readme.txt");
            File.WriteAllText(testFile, "This is a read me text file." + Environment.NewLine);

            Uri repo = new Uri(repositoryPath);

            using (var client = new SvnClient(SvnClient.Commands.Import))
            {
                client.RepositoryPath = repo.ToString();
                client.LocalPath = workingPath;
                client.Message = "First Import";

                var r = client.Execute();
                Console.WriteLine(client.StandardOutput);
                Console.WriteLine(client.StandardError);
                return r;
            }
        }

        public static bool CreateWorking(string repositoryPath, string workingPath)
        {
            PathHelper.DeleteDirectory(workingPath);
            Uri repo = new Uri(repositoryPath);

            using (var client = new SvnClient(SvnClient.Commands.Checkout))
            {
                client.RepositoryPath = repo.ToString();
                client.LocalPath = workingPath;
                
                var r = client.Execute();
                Console.WriteLine(client.StandardOutput);
                Console.WriteLine(client.StandardError);
                return r;
            }
        }

        public static bool CommitWorking(string workingPath)
        {
            using (var client = new SvnClient(SvnClient.Commands.Commit))
            {
                client.LocalPath = workingPath;
                client.Message = "Commit progress";

                var r = client.Execute();
                Console.WriteLine(client.StandardOutput);
                Console.WriteLine(client.StandardError);
                return r;
            }
        }
    }
}
