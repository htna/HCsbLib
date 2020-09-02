using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib2
{
    public partial class HResource
	{
        public static string[] GetResourceNames<T>()
        {
            var assm = typeof(T).Assembly;
            string[] resnames = assm.GetManifestResourceNames();
            return resnames;
        }
        public static string FindResourceName<T>(string resname)
        {
            List<string> names = new List<string>();
            foreach(string name in GetResourceNames<T>())
            {
                if(name.Contains(resname))
                    names.Add(name);
            }
            if(names.Count() != 1)
                HDebug.Assert(false);
            return names[0];
        }
        public static void CopyResourceTo<T>(string resname, string path)
        {
            HDebug.Assert(GetResourceNames<T>().Contains(resname));
            var assm   = typeof(T).Assembly;
            var stream = assm.GetManifestResourceStream(FindResourceName<T>(resname));
            HFile.WriteStream(path, stream);
            stream.Close();
        }
        public static string[] GetResourceLines<T>(string resname)
        {
            HDebug.Assert(GetResourceNames<T>().Contains(resname));
            var assm   = typeof(T).Assembly;
            var stream = assm.GetManifestResourceStream(FindResourceName<T>(resname));
            string[] lines = stream.HReadAllLines();
            stream.Close();
            return lines;
        }
        public static Stream GetResourceStream<T>(string resname)
        {
            HDebug.Assert(GetResourceNames<T>().Contains(resname));
            var assm   = typeof(T).Assembly;
            Stream stream = assm.GetManifestResourceStream(FindResourceName<T>(resname));
            return stream;
        }
    }
}
