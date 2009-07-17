using System;
using System.IO;
using Ionic.Zip;
using SvnTools.Services;
using SvnTools.Utility;

// $Id$

namespace SvnTools
{
    public static class Backup
    {
        #region Logging Definition
        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(Backup));
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

            // first try repoRoot as a repository
            if (IsRepository(repoRoot))
                BackupRepository(args, repoRoot, backupRoot);
            // next try as partent folder for repositories
            else
                foreach (var repo in repoRoot.GetDirectories())
                    BackupRepository(args, repo, backupRoot);
        }

        private static void BackupRepository(BackupArguments args, DirectoryInfo repository, DirectoryInfo backupRoot)
        {
            try
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
                    return; // this rev is already backed up

                // hotcopy
                _log.InfoFormat("Backing up '{0}' from '{1}'.", revString, repository.Name);
                RunHotCopy(args, repository, backupRevPath);

                // compress
                if (args.Compress && !File.Exists(backupZipPath))
                    CompressBackup(backupRevPath, backupZipPath);

                // purge old
                PruneBackups(backupRepoPath, args.History);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
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
                    _log.Info(hotCopy.StandardError);
            }
        }

        private static string GetRevision(BackupArguments args, DirectoryInfo repo)
        {
            int rev;

            // version
            using (SvnLook look = new SvnLook("youngest"))
            {
                look.RepositoryPath = repo.FullName;
                if (!string.IsNullOrEmpty(args.SubverisonPath))
                {
                    look.ToolPath = args.SubverisonPath;
                }

                look.Execute();
                if (!string.IsNullOrEmpty(look.StandardError))
                    _log.Info(look.StandardError);

                if (!look.TryGetRevision(out rev))
                {
                    _log.WarnFormat("'{0}' is not a repository.", repo.Name);

                    if (!string.IsNullOrEmpty(look.StandardOutput))
                        _log.Info(look.StandardOutput);

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

            PathHelper.DeleteDirectory(backupRevPath);
        }

        private static void PruneBackups(string backupRepoPath, int historyCount)
        {
            if (historyCount < 1)
                return;

            var dirs = Directory.GetDirectories(backupRepoPath);
            if (dirs.Length > historyCount)
            {
                for (int i = 0; i < dirs.Length - historyCount; i++)
                {
                    string dir = dirs[i];

                    PathHelper.DeleteDirectory(dir);
                    _log.InfoFormat("Removed backup '{0}'.", dir);
                }
            }

            var files = Directory.GetFiles(backupRepoPath, "*.zip");
            if (files.Length > historyCount)
            {
                for (int i = 0; i < files.Length - historyCount; i++)
                {
                    string file = files[i];

                    File.Delete(file);
                    _log.InfoFormat("Removed backup '{0}'.", file);
                }
            }
        }


        private static bool IsRepository(DirectoryInfo path)
        {
            string formatFile = Path.Combine(path.FullName, "format");
            return File.Exists(formatFile);
        }
    }
}
