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
        public static void CopyPdb(string pathbase, params string[] pdbids)
        {
            if(pathbase.EndsWith("\\") == false)
                pathbase += "\\";
            foreach(string pdbid in pdbids)
            {
                string       pdbpath  = pathbase+pdbid.ToUpper()+".pdb";
                List<string> pdblines = GetPdbLines(pdbid);
                HFile.WriteAllLines(pdbpath, pdblines);
            }
        }
    }
}
