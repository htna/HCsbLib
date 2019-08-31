using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    using Stream = System.IO.Stream;
    public partial class PdbDatabase
	{
        static Dictionary<string, object> _GetPdbLinesParallel = new Dictionary<string, object>();

        public static Pdb GetPdb(string pdbid, bool forceToRedownload=false)
        {
            List<string> pdblines = GetPdbLines(pdbid, forceToRedownload);
            Pdb pdb = Pdb.FromLines(pdblines);
            return pdb;
        }
        public static List<string> GetPdbLines(string pdbid, bool forceToRedownload=false)
        {
            throw new NotImplementedException();
            /// pdbid = pdbid.Trim();
            /// List<string> pdblines;
            /// string key = pdbid.Substring(0, 2);
            /// //object parallel_lock = null;
            /// //lock(_GetPdbLinesParallel)
            /// //{
            /// //    if(_GetPdbLinesParallel.ContainsKey(key) == false)
            /// //        _GetPdbLinesParallel.Add(key, new object());
            /// //    parallel_lock = _GetPdbLinesParallel[key];
            /// //}
            /// //lock(parallel_lock)
            /// {
            ///     System.IO.Packaging.Package pkg = null;
            ///     string pkgopen = "readonly";
            ///     switch(pkgopen)
            ///     {
            ///         case "readonly":
            ///             pkg = System.IO.Packaging.Package.Open(RootPath+key+"__.zip", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            ///             break;
            ///         case "create":
            ///             pkg = System.IO.Packaging.Package.Open(RootPath+key+"__.zip", System.IO.FileMode.OpenOrCreate);
            ///             break;
            ///         case "readwrite":
            ///             pkg = System.IO.Packaging.Package.Open(RootPath+key+"__.zip", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
            ///             break;
            ///     }
            ///     //System.IO.Packaging.Package pkg = System.IO.Packaging.Package.Open(RootPath+key+"__.zip", System.IO.FileMode.OpenOrCreate);
            ///     //System.IO.Packaging.Package pkg = System.IO.Packaging.Package.Open(RootPath+key+"__.zip", System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read);
            ///     {
            ///         Uri uri = new Uri(string.Format("/{0}.pdb", pdbid.ToUpper()), UriKind.Relative);
            ///         try
            ///         {
            ///             if(forceToRedownload || pkg.PartExists(uri) == false)
            ///             {
            ///                 System.Net.WebClient webClient = new System.Net.WebClient();
            ///                 string address = string.Format(@"http://www.pdb.org/pdb/files/{0}.pdb", pdbid.ToUpper());
            ///                 System.IO.Stream webstream = webClient.OpenRead(address);
            ///                 {
            ///                     // reopen package as read/write
            ///                     pkg.Close();
            ///                     pkg = System.IO.Packaging.Package.Open(RootPath+key+"__.zip", System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
            ///                 }
            ///                 if(pkg.PartExists(uri))
            ///                     pkg.DeletePart(uri);
            ///                 System.IO.Packaging.PackagePart part = pkg.CreatePart(uri, System.Net.Mime.MediaTypeNames.Text.Plain, System.IO.Packaging.CompressionOption.Normal);
            ///                 System.IO.Stream pdbstream = part.GetStream(System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.ReadWrite);
            ///                 webstream.CopyTo(pdbstream);
            ///                 pdbstream.Close();
            ///                 webstream.Close();
            ///                 webClient.Dispose();
            ///                 pkg.Flush();
            ///             }
            ///             {
            ///                 pdblines = new List<string>();
            ///                 System.IO.Packaging.PackagePart part = pkg.GetPart(uri);
            ///                 System.IO.StreamReader pdbreader = new System.IO.StreamReader(part.GetStream(System.IO.FileMode.Open, System.IO.FileAccess.Read));
            ///                 while(pdbreader.EndOfStream == false)
            ///                     pdblines.Add(pdbreader.ReadLine());
            ///                 pdbreader.Close();
            ///             }
            ///         }
            ///         catch(Exception)
            ///         {
            ///             pdblines = null;
            ///         }
            ///     }
            ///     pkg.Close();
            ///     HDebug.Assert(pdblines != null);
            /// }
            /// return pdblines;
        }
    }
}
