using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class PdbDatabase
	{
        //static string _RootPath = @"D:\PdbPackage\";
        //static string _RootPath = @"\\songlab1.student.iastate.edu\k\PdbPackage\";
        //static string _RootPath = @"\\songlab1.cs.iastate.edu\k\PdbPackage\";
        //static string _RootPath = @"K:\PdbPackage\";
        static string _RootPath = @"D:\ProtDataBank\pdb\";
        public static string RootPath
        {
            get
            {
                HDebug.Assert(_RootPath.EndsWith("\\") == true);
                return _RootPath;
            }
            set
            {
                if(value.EndsWith("\\") == false)
                    value = value + "\\";
                _RootPath = value;
            }
        }
        public static string GetPdbPath(string pdbid, string rootpath=null)
        {
            if(rootpath == null)
                rootpath = RootPath;
            if(rootpath.EndsWith("\\") == false)
                rootpath += "\\";
            return RootPath + pdbid + ".pdb";
        }
    }
}
