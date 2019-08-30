using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib2
{
    public static class HDirectory
    {
        public static DirectoryInfo CreateTempDirectory(string tempbase=null)
        {
            while(true)
            {
                string path = "$"+System.IO.Path.GetRandomFileName();
                if(System.IO.File.Exists(tempbase+path) == false)
                {
                    return HDirectory.CreateDirectory(tempbase+path);
                }
            }
        }

        /// http://msdn.microsoft.com/en-us/library/bb762914(v=vs.110).aspx
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if(!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if(!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach(FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if(copySubDirs)
            {
                foreach(DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        //////////////////////////////////////////////////////////////////////
        // System.IO.Directory
        public static DirectoryInfo CreateDirectory(string path)                                                                             { return System.IO.Directory.CreateDirectory(path)                                         ;}
        //public static DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity)                                        { return System.IO.Directory.CreateDirectory(path, directorySecurity)                      ;}
        public static void Delete(string path)                                                                                               {        System.IO.Directory.Delete(path)                                                  ;}
        public static void Delete(string path, bool recursive)                                                                               {        System.IO.Directory.Delete(path, recursive)                                       ;}
        public static IEnumerable<string> EnumerateDirectories(string path)                                                                  { return System.IO.Directory.EnumerateDirectories(path)                                    ;}
        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)                                            { return System.IO.Directory.EnumerateDirectories(path, searchPattern)                     ;}
        public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, System.IO.SearchOption searchOption)       { return System.IO.Directory.EnumerateDirectories(path, searchPattern, searchOption)       ;}
        public static IEnumerable<string> EnumerateFiles(string path)                                                                        { return System.IO.Directory.EnumerateFiles(path)                                          ;}
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)                                                  { return System.IO.Directory.EnumerateFiles(path, searchPattern)                           ;}
        public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, System.IO.SearchOption searchOption)             { return System.IO.Directory.EnumerateFiles(path, searchPattern, searchOption)             ;}
        public static IEnumerable<string> EnumerateFileSystemEntries(string path)                                                            { return System.IO.Directory.EnumerateFileSystemEntries(path)                              ;}
        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)                                      { return System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern)               ;}
        public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, System.IO.SearchOption searchOption) { return System.IO.Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption) ;}
        public static bool Exists(string path)                                                                                               { return System.IO.Directory.Exists(path)                                                  ;}
        public static DirectorySecurity GetAccessControl(string path)                                                                        { return System.IO.Directory.GetAccessControl(path)                                        ;}
        public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)                                 { return System.IO.Directory.GetAccessControl(path, includeSections)                       ;}
        public static DateTime GetCreationTime(string path)                                                                                  { return System.IO.Directory.GetCreationTime(path)                                         ;}
        public static DateTime GetCreationTimeUtc(string path)                                                                               { return System.IO.Directory.GetCreationTimeUtc(path)                                      ;}
        public static string GetCurrentDirectory()                                                                                           { return System.IO.Directory.GetCurrentDirectory()                                         ;}
        public static string[] GetDirectories(string path)                                                                                   { return System.IO.Directory.GetDirectories(path)                                          ;}
        public static string[] GetDirectories(string path, string searchPattern)                                                             { return System.IO.Directory.GetDirectories(path, searchPattern)                           ;}
        public static string[] GetDirectories(string path, string searchPattern, System.IO.SearchOption searchOption)                        { return System.IO.Directory.GetDirectories(path, searchPattern, searchOption)             ;}
        public static string GetDirectoryRoot(string path)                                                                                   { return System.IO.Directory.GetDirectoryRoot(path)                                        ;}
        public static string[] GetFiles(string path)                                                                                         { return System.IO.Directory.GetFiles(path)                                                ;}
        public static string[] GetFiles(string path, string searchPattern)                                                                   { return System.IO.Directory.GetFiles(path, searchPattern)                                 ;}
        public static string[] GetFiles(string path, string searchPattern, System.IO.SearchOption searchOption)                              { return System.IO.Directory.GetFiles(path, searchPattern, searchOption)                   ;}
        public static string[] GetFileSystemEntries(string path)                                                                             { return System.IO.Directory.GetFileSystemEntries(path)                                    ;}
        public static string[] GetFileSystemEntries(string path, string searchPattern)                                                       { return System.IO.Directory.GetFileSystemEntries(path, searchPattern)                     ;}
        public static string[] GetFileSystemEntries(string path, string searchPattern, System.IO.SearchOption searchOption)                  { return System.IO.Directory.GetFileSystemEntries(path, searchPattern, searchOption)       ;}
        public static DateTime GetLastAccessTime(string path)                                                                                { return System.IO.Directory.GetLastAccessTime(path)                                       ;}
        public static DateTime GetLastAccessTimeUtc(string path)                                                                             { return System.IO.Directory.GetLastAccessTimeUtc(path)                                    ;}
        public static DateTime GetLastWriteTime(string path)                                                                                 { return System.IO.Directory.GetLastWriteTime(path)                                        ;}
        public static DateTime GetLastWriteTimeUtc(string path)                                                                              { return System.IO.Directory.GetLastWriteTimeUtc(path)                                     ;}
        public static string[] GetLogicalDrives()                                                                                            { return System.IO.Directory.GetLogicalDrives()                                            ;}
        public static DirectoryInfo GetParent(string path)                                                                                   { return System.IO.Directory.GetParent(path)                                               ;}
        public static void Move(string sourceDirName, string destDirName)                                                                    {        System.IO.Directory.Move(sourceDirName, destDirName)                              ;}
        public static void SetAccessControl(string path, DirectorySecurity directorySecurity)                                                {        System.IO.Directory.SetAccessControl(path, directorySecurity)                     ;}
        public static void SetCreationTime(string path, DateTime creationTime)                                                               {        System.IO.Directory.SetCreationTime(path, creationTime)                           ;}
        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)                                                         {        System.IO.Directory.SetCreationTimeUtc(path, creationTimeUtc)                     ;}
        public static void SetCurrentDirectory(string path)                                                                                  {        System.IO.Directory.SetCurrentDirectory(path)                                     ;}
        public static void SetLastAccessTime(string path, DateTime lastAccessTime)                                                           {        System.IO.Directory.SetLastAccessTime(path, lastAccessTime)                       ;}
        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)                                                     {        System.IO.Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc)                 ;}
        public static void SetLastWriteTime(string path, DateTime lastWriteTime)                                                             {        System.IO.Directory.SetLastWriteTime(path, lastWriteTime)                         ;}
        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)                                                       {        System.IO.Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc)                   ;}
    }
}
