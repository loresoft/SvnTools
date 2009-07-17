using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SvnTools.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Determines whether the specified path is repository.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the specified path is repository; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRepository(string path)
        {
            string formatFile = Path.Combine(Path.GetFullPath(path), "format");
            return File.Exists(formatFile);
        }

        /// <summary>
        /// Creates the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
                return;

            Directory.CreateDirectory(path);
        }


        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        public static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
                return;

            DeleteDirectoryInternal(path);
            Directory.Delete(path, true);
        }

        private static void DeleteDirectoryInternal(string path)
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
                    DeleteDirectoryInternal(info.FullName);

                info.Delete();
                info.Refresh();
            }
        }
    }
}
