using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SvnTools.Utility
{
    public static class PathHelper
    {
        public static void DeleteDirectory(string path)
        {
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
