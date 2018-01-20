using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Namd
    {
        public static Tuple<string, string>[] GetResourcePaths(string version, params string[] resnames)
        {
            HashSet<Tuple<string, string>> lstRespathFilename = new HashSet<Tuple<string, string>>();
            foreach(string resname in resnames)
            {
                // get common libraries, and resource base
                string resbase;
                switch(version)
                {
                    case "2.8":
                        resbase = "HTLib2.HTLib2.Bioinfo.External.Namd.Resources.name_2_8.";
                        lstRespathFilename.Add(new Tuple<string, string>(resbase+"tcl85.dll", "tcl85.dll"));
                        break;
                    case "2.10":
                        resbase = "HTLib2.HTLib2.Bioinfo.External.Namd.Resources.name_2_10.";
                        lstRespathFilename.Add(new Tuple<string, string>(resbase+"tcl85t.dll", "tcl85t.dll"));
                        break;
                    default:
                        throw new ArgumentException("version");
                }
                switch(resname)
                {
                    case "namd2" : lstRespathFilename.Add(new Tuple<string, string>(resbase+"namd2.exe" , "namd2.exe" )); break;
                    case "psfgen": lstRespathFilename.Add(new Tuple<string, string>(resbase+"psfgen.exe", "psfgen.exe")); break;
                    default:
                        throw new ArgumentException(resname +" in resnames");
                }
            }
            return lstRespathFilename.ToArray();
        }
    }
}
