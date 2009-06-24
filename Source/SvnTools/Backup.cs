using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ionic.Zip;
using SvnTools.Services;
using SvnTools.Utility;

// $Id$

namespace SvnTools
{
    public static class Backup
    {
        #region Logging Definition
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(Backup));
        #endregion

        public static void Run(BackupArguments args)
        {
            DirectoryInfo repoRoot = new DirectoryInfo(args.RepositoryRoot);
            if (!repoRoot.Exists)
                throw new InvalidOperationException(string.Format(
                    "The repository root directory '{0}' does not exist.",
                    args.RepositoryRoot));

            var backupRoot = new DirectoryInfo(args.BackupRoot);
            if (!backupRoot.Exists)
                backupRoot.Create();


            foreach (var repo in repoRoot.GetDirectories())
            {
                try
                {
                    BackupRepository(args, repo, backupRoot);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
            }
        }

        private static void BackupRepository(BackupArguments args, DirectoryInfo repository, DirectoryInfo backupRoot)
        {
            string revString = GetRevision(args, repository);

            if (string.IsNullOrEmpty(revString))
                return; // couldn't find repo

            string backupRepoPath = Path.Combine(backupRoot.FullName, repository.Name);
            string backupRevPath = Path.Combine(backupRepoPath, revString);
            string backupZipPath = backupRevPath + ".zip";

            if (!Directory.Exists(backupRepoPath))
                Directory.CreateDirectory(backupRepoPath);

            if (Directory.Exists(backupRevPath) || File.Exists(backupZipPath))
                return;

            // hotcopy
            log.InfoFormat("Backing up '{0}' from '{1}'.", revString, repository.Name);
            RunHotCopy(args, repository, backupRevPath);

            // compress
            if (args.Compress && !File.Exists(backupZipPath))
                CompressBackup(backupRevPath, backupZipPath);

            // purge old
            PruneBackups(backupRepoPath, args.History);
        }

        private static void RunHotCopy(BackupArguments args, DirectoryInfo repo, string backupRevPath)
        {
            using (var hotCopy = new HotCopy())
            {
                if (!string.IsNullOrEmpty(args.SubverisonPath))
                    hotCopy.ToolPath = args.SubverisonPath;

                hotCopy.BackupPath = backupRevPath;
                hotCopy.RepositoryPath = repo.FullName;

                hotCopy.Execute();

                if (!string.IsNullOrEmpty(hotCopy.StandardError))
                    log.Info(hotCopy.StandardError);
            }
        }

        private static string GetRevision(BackupArguments args, DirectoryInfo repo)
        {
            int rev;

            // version
            using (SvnVersion version = new SvnVersion())
            {
                version.RepositoryPath = repo.FullName;
                if (!string.IsNullOrEmpty(args.SubverisonPath))
                {
                    version.ToolPath = args.SubverisonPath;
                }

                version.Execute();
                if (!string.IsNullOrEmpty(version.StandardError))
                    log.Info(version.StandardError);

                if (!version.TryGetRevision(out rev))
                {
                    log.WarnFormat("'{0}' is not a repository.", repo.Name);

                    if (!string.IsNullOrEmpty(version.StandardOutput))
                        log.Info(version.StandardOutput);

                    return null;
                }
            }

            return "v" + rev.ToString().PadLeft(7, '0');
        }

        private static void CompressBackup(string backupRevPath, string backupZipPath)
        {
            using (var zipFile = new ZipFile())
            {
                zipFile.AddDirectory(backupRevPath);
                zipFile.Save(backupZipPath);
            }

            DirectoryDelete(backupRevPath);
            Directory.Delete(backupRevPath);
        }

        private static void PruneBackups(string backupRepoPath, int historyCount)
        {
            var dirs = Directory.GetDirectories(backupRepoPath);
            if (dirs.Length > historyCount)
            {
                for (int i = 0; i < dirs.Length - historyCount; i++)
                {
                    string dir = dirs[i];

                    DirectoryDelete(dir);
                    Directory.Delete(dir);
                    log.InfoFormat("Removed backup '{0}'.", dir);
                }
            }

            var files = Directory.GetFiles(backupRepoPath, "*.zip");
            if (files.Length > historyCount)
            {
                for (int i = 0; i < files.Length - historyCount; i++)
                {
                    string file = files[i];

                    File.Delete(file);
                    log.InfoFormat("Removed backup '{0}'.", file);
                }
            }
        }

        private static void DirectoryDelete(string path)
        {
            //work around issues with Directory.Delete(path, true)
            var dir = new DirectoryInfo(path);

            foreach (var info in dir.GetFileSystemInfos())
            {
                // clear readonly and hidden flags
                if (EnumHelper.IsFlagOn(info.Attributes, FileAttributes.ReadOnly))
                    info.Attributes = EnumHelper.SetFlagOff(info.Attributes, FileAttributes.ReadOnly);
                if (EnumHelper.IsFlagOn(info.Attributes, FileAttributes.Hidden))
                    info.Attributes = EnumHelper.SetFlagOff(info.Attributes, FileAttributes.Hidden);

                if (EnumHelper.IsFlagOn(info.Attributes, FileAttributes.Directory))
                    DirectoryDelete(info.FullName);

                info.Delete(); 
                info.Refresh();
            }
        }
    }
}
