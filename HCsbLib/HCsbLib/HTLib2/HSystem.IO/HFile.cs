using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib2
{
    public partial class HFile
    {
        public class FileLock
        {
            public string path;
            public FileStream file;
            public void Release()
            {
                file.Close();
                HFile.Delete(path);
                file = null;
            }
        }
        public static FileLock LockFile(string path, FileMode mode=FileMode.OpenOrCreate, FileAccess access=FileAccess.ReadWrite)
        {
            try
            {
                FileShare share = FileShare.None;
                var file = File.Open(path, mode, access, share);
                return new FileLock
                {
                    path=path,
                    file=file,
                };
            }
            catch
            {
                return null;
            }
        }

        public delegate TYPE Parser<TYPE>(string str);

        public static IEnumerable<string> ReadLines(string filename)
        {
            return File.ReadLines(filename);
        }
        //public static List<string> ReadLines(string filename)
        //{
        //    try
        //    {
        //        System.IO.StreamReader reader = new System.IO.StreamReader(filename);
        //        List<string> lines = new List<string>();
        //        while(reader.EndOfStream == false)
        //        {
        //            string line = reader.ReadLine();
        //            lines.Add(line);
        //        }
        //        reader.Close();
        //        reader.Dispose();
        //        return lines;
        //    }
        //    catch
        //    {
        //        HDebug.Assert(false);
        //        string todo = "throw";
        //        if(todo == "throw")
        //            throw;
        //    }
        //    return null;
        //}

        public static List<TYPE> ReadValues<TYPE>(string filename, Parser<TYPE> parser)
        {
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(filename);
                List<TYPE> values = new List<TYPE>();
                while(reader.EndOfStream == false)
                {
                    string line = reader.ReadLine();
                    TYPE value = parser(line);
                    values.Add(value);
                }
                reader.Close();
                reader.Dispose();
                return values;
            }
            catch
            {
                HDebug.Assert(false);
                throw;
            }
        }

        public static List<List<TYPE>> ReadTable<TYPE>(string filename, Parser<TYPE> parser)
        {
            try
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(filename);
                List<List<TYPE>> table = new List<List<TYPE>>();
                while(reader.EndOfStream == false)
                {
                    string line = reader.ReadLine();
                    string[] values = line.Split(' ', ',', '\t');
                    List<TYPE> values_ = new List<TYPE>();
                    foreach(string value in values)
                    {
                        TYPE value_ = parser(value);
                        values_.Add(value_);
                    }
                    table.Add(values_);
                }
                reader.Close();
                reader.Dispose();
                return table;
            }
            catch
            {
                HDebug.Assert(false);
                throw;
            }
        }

        public static System.IO.FileInfo GetFileInfo(string path) { return new System.IO.FileInfo(path); }

        public static string GetTempPath(string ext)
        {
            return GetTempPath(null, "."+ext);
        }
        public static string GetTempPath(string prefix, string suffix)
        {
            while(true)
            {
                string path = prefix + System.IO.Path.GetRandomFileName().Replace(".","") + suffix;
                if(System.IO.File.Exists(path) == false)
                    return path;
            }
        }


        public static void AppendAllLines(string path, params string[] contents) { System.IO.File.AppendAllLines(path, contents); }
        //////////////////////////////////////////////////////////////////////
        // System.IO.File
        public static void AppendAllLines(string path, IEnumerable<string> contents)                                                               {        System.IO.File.AppendAllLines(path, contents); }
        public static void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)                                            {        System.IO.File.AppendAllLines(path, contents, encoding); }
        public static void AppendAllText(string path, string contents)                                                                             {        System.IO.File.AppendAllText(path, contents); }
        public static void AppendAllText(string path, string contents, Encoding encoding)                                                          {        System.IO.File.AppendAllText(path, contents, encoding); }
        public static StreamWriter AppendText(string path)                                                                                         { return System.IO.File.AppendText(path); }
        public static void Copy(string sourceFileName, string destFileName)                                                                        {        System.IO.File.Copy(sourceFileName, destFileName); }
        public static void Copy(string sourceFileName, string destFileName, bool overwrite)                                                        {        System.IO.File.Copy(sourceFileName, destFileName, overwrite); }
        public static FileStream Create(string path)                                                                                               { return System.IO.File.Create(path); }
        public static FileStream Create(string path, int bufferSize)                                                                               { return System.IO.File.Create(path, bufferSize); }
        public static FileStream Create(string path, int bufferSize, FileOptions options)                                                          { return System.IO.File.Create(path, bufferSize, options); }
        public static FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)                               { return System.IO.File.Create(path, bufferSize, options, fileSecurity); }
        public static StreamWriter CreateText(string path)                                                                                         { return System.IO.File.CreateText(path); }
        public static void Decrypt(string path)                                                                                                    {        System.IO.File.Decrypt(path); }
        public static void Delete(string path)                                                                                                     {        System.IO.File.Delete(path); }
        public static void Encrypt(string path)                                                                                                    {        System.IO.File.Encrypt(path); }
        public static bool Exists(string path)                                                                                                     { return System.IO.File.Exists(path); }
        public static FileSecurity GetAccessControl(string path)                                                                                   { return System.IO.File.GetAccessControl(path); }
        public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections)                                            { return System.IO.File.GetAccessControl(path, includeSections); }
        public static FileAttributes GetAttributes(string path)                                                                                    { return System.IO.File.GetAttributes(path); }
        public static DateTime GetCreationTime(string path)                                                                                        { return System.IO.File.GetCreationTime(path); }
        public static DateTime GetCreationTimeUtc(string path)                                                                                     { return System.IO.File.GetCreationTimeUtc(path); }
        public static DateTime GetLastAccessTime(string path)                                                                                      { return System.IO.File.GetLastAccessTime(path); }
        public static DateTime GetLastAccessTimeUtc(string path)                                                                                   { return System.IO.File.GetLastAccessTimeUtc(path); }
        public static DateTime GetLastWriteTime(string path)                                                                                       { return System.IO.File.GetLastWriteTime(path); }
        public static DateTime GetLastWriteTimeUtc(string path)                                                                                    { return System.IO.File.GetLastWriteTimeUtc(path); }
        public static void Move(string sourceFileName, string destFileName)                                                                        {        System.IO.File.Move(sourceFileName, destFileName); }
        public static FileStream Open(string path, FileMode mode)                                                                                  { return System.IO.File.Open(path, mode); }
        public static FileStream Open(string path, FileMode mode, FileAccess access)                                                               { return System.IO.File.Open(path, mode, access); }
        public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)                                              { return System.IO.File.Open(path, mode, access, share); }
        public static FileStream OpenRead(string path)                                                                                             { return System.IO.File.OpenRead(path); }
        public static StreamReader OpenText(string path)                                                                                           { return System.IO.File.OpenText(path); }
        public static FileStream OpenWrite(string path)                                                                                            { return System.IO.File.OpenWrite(path); }
        public static byte[] ReadAllBytes(string path)                                                                                             { return System.IO.File.ReadAllBytes(path); }
        public static string[] ReadAllLines(string path)                                                                                           { return System.IO.File.ReadAllLines(path); }
        public static string[] ReadAllLines(string path, Encoding encoding)                                                                        { return System.IO.File.ReadAllLines(path, encoding); }
        public static string ReadAllText(string path)                                                                                              { return System.IO.File.ReadAllText(path); }
        public static string ReadAllText(string path, Encoding encoding)                                                                           { return System.IO.File.ReadAllText(path, encoding); }
        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)                            {        System.IO.File.Replace(sourceFileName, destinationFileName, destinationBackupFileName); }
        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors) {        System.IO.File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors); }
        public static void SetAccessControl(string path, FileSecurity fileSecurity)                                                                {        System.IO.File.SetAccessControl(path, fileSecurity); }
        public static void SetAttributes(string path, FileAttributes fileAttributes)                                                               {        System.IO.File.SetAttributes(path, fileAttributes); }
        public static void SetCreationTime(string path, DateTime creationTime)                                                                     {        System.IO.File.SetCreationTime(path, creationTime); }
        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)                                                               {        System.IO.File.SetCreationTimeUtc(path, creationTimeUtc); }
        public static void SetLastAccessTime(string path, DateTime lastAccessTime)                                                                 {        System.IO.File.SetLastAccessTime(path, lastAccessTime); }
        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)                                                           {        System.IO.File.SetLastAccessTimeUtc(path, lastAccessTimeUtc); }
        public static void SetLastWriteTime(string path, DateTime lastWriteTime)                                                                   {        System.IO.File.SetLastWriteTime(path, lastWriteTime); }
        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)                                                             {        System.IO.File.SetLastWriteTimeUtc(path, lastWriteTimeUtc); }
        public static void WriteAllBytes(string path, byte[] bytes)                                                                                {        System.IO.File.WriteAllBytes(path, bytes); }
        public static void WriteAllLines(string path, IEnumerable<string> contents)                                                                {        System.IO.File.WriteAllLines(path, contents); }
        public static void WriteAllLines(string path, string[] contents, Encoding encoding)                                                        {        System.IO.File.WriteAllLines(path, contents, encoding); }
        public static void WriteAllText(string path, string contents)                                                                              {        System.IO.File.WriteAllText(path, contents); }
        public static void WriteAllText(string path, string contents, Encoding encoding)                                                           {        System.IO.File.WriteAllText(path, contents, encoding); }

        public static void WriteAllText(string path, IEnumerable<string> contents)
        {
            using(StreamWriter file = new StreamWriter(path))
            {
                foreach(var str in contents)
                {
                    file.Write(str);
                }
            }
        }

        public static string[] ReadAllLines(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(stream);
            List<string> lines = new List<string>();
            while(true)
            {
                string line = reader.ReadLine();
                if(line == null) break;
                lines.Add(line);
            }
            return lines.ToArray();
        }
        public static void WriteAllLines(string path, params string[] contents)
        {
            WriteAllLines(path, (IEnumerable<string>)contents);
        }
        public static void WriteAllLines(Stream stream, params string[] contents)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var writer = new StreamWriter(stream);
            for(int i=0; i<contents.Length; i++)
            {
                if(i != 0) writer.WriteLine();
                writer.Write(contents[i]);
            }
            writer.Flush();
        }

        //public static FileStream Open(string path, FileMode mode)
        //{
        //    return System.IO.File.Open(path, mode);
        //}
        //public static FileStream Open(string path, FileMode mode, FileAccess access)
        //{
        //    return System.IO.File.Open(path, mode, access);
        //}
        //public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
        //{
        //    return System.IO.File.Open(path, mode, access, share);
        //}
        //public static StreamReader OpenText(string path)
        //{
        //    return System.IO.File.OpenText(path);
        //}
        //public static StreamWriter CreateText(string path)
        //{
        //    return System.IO.File.CreateText(path);
        //}
        //public static StreamWriter WriteText(string path)
        //{
        //    return new System.IO.StreamWriter(path);
        //}

        public static IEnumerable<string> HEnumAllLines(string path)
        {
            System.IO.StreamReader reader = HFile.OpenText(path);
            while(reader.EndOfStream == false)
            {
                string line = reader.ReadLine();
                yield return line;
            }
            reader.Close();
        }
        public static void WriteStream(string path, Stream stream)
        {
            System.IO.FileStream file = System.IO.File.Create(path);
            stream.CopyTo(file);
            file.Close();
            stream.Close();
        }
        public static bool ExistsAll(params string[] paths)
        {
            foreach(var path in paths)
                if(Exists(path) == false)
                    return false;
            return true;
        }
        public static bool ExistsAny(params string[] paths)
        {
            foreach(var path in paths)
                if(Exists(path))
                    return true;
            return false;
        }

        public static FileStream GetFileStream
            ( string path
            , object mode          // FileMode   = { CreateNew, Create, Open, OpenOrCreate, Truncate, Append }
            , object access = null // FileAccess = { Read, Write, ReadWrite }
            , object share  = null // FileShare  = { None, Read, Write, ReadWrite, Delete, Inheritable }
            )
        {
            if(mode != null && mode is string)
                switch((mode as string).ToLower())
                {
                    case "createnew"   : mode = FileMode.CreateNew    ; break;
                    case "create"      : mode = FileMode.Create       ; break;
                    case "open"        : mode = FileMode.Open         ; break;
                    case "openorcreate": mode = FileMode.OpenOrCreate ; break;
                    case "truncate"    : mode = FileMode.Truncate     ; break;
                    case "append"      : mode = FileMode.Append       ; break;
                    default: throw new HException();
                }
            if(access != null && access is string)
                switch((access as string).ToLower())
                {
                    case "read"        : access = FileAccess.Read     ; break;
                    case "write"       : access = FileAccess.Write    ; break;
                    case "readwrite"   : access = FileAccess.ReadWrite; break;
                    default: throw new HException();
                }
            if(share != null && share is string)
                switch((share as string).ToLower())
                {
                    case "none"        : share = FileShare.None       ; break;
                    case "read"        : share = FileShare.Read       ; break;
                    case "write"       : share = FileShare.Write      ; break;
                    case "readwrite"   : share = FileShare.ReadWrite  ; break;
                    case "delete"      : share = FileShare.Delete     ; break;
                    case "inheritable" : share = FileShare.Inheritable; break;
                    default: throw new HException();
                }

            if(mode != null && access != null && share != null) return new FileStream(path, (FileMode)mode, (FileAccess)access, (FileShare)share);
            if(mode != null && access != null && share == null) return new FileStream(path, (FileMode)mode, (FileAccess)access);
            if(mode != null && access == null && share == null) return new FileStream(path, (FileMode)mode);
            throw new HException();
        }
        public static System.IO.Compression.GZipStream GzToStream
            ( string path
            , object mode          // FileMode   = { CreateNew, Create, Open, OpenOrCreate, Truncate, Append }
            , object access = null // FileAccess = { Read, Write, ReadWrite }
            , object share  = null // FileShare  = { None, Read, Write, ReadWrite, Delete, Inheritable }
            )
        {
            FileStream gzfile = GetFileStream(path, mode, access, share);
            return GzToStream(gzfile);
        }
        public static System.IO.Compression.GZipStream GzToStream(FileStream gzfile)
        {
            var gzstream = new System.IO.Compression.GZipStream(gzfile, System.IO.Compression.CompressionMode.Decompress);
            return gzstream;
        }
        public static string[] GzToLines(FileStream gzfile)
        {
            var gzstream = new System.IO.Compression.GZipStream(gzfile, System.IO.Compression.CompressionMode.Decompress);
            string[] lines = gzstream.HReadAllLines();
            return lines;
        }
        public static MemoryStream LinesToMemoryStream(IList<string> lines)
        {
            var mem = new MemoryStream();
            var txt = new StreamWriter(mem);
            foreach(var line in lines)
                txt.WriteLine(line);
            txt.Flush();
            mem.Position = 0;
            return mem;
        }
        public static Stream WebToStream(string address)
        {
            System.Net.WebClient webClient = new System.Net.WebClient();
            Stream stream = webClient.OpenRead(address);
            webClient.Dispose();
            return stream;
        }
    }
}
