using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public static Tuple<string,string>[] GetResourcePaths(string version, params string[] resnames)
        {
            HashSet<Tuple<string, string>> lstRespathFilename = new HashSet<Tuple<string, string>>();
            foreach(string resname in resnames)
            {
                // get common libraries, and resource base
                string resbase;
                switch(version)
                {
                    case "6.2.1":
                        resbase = "Tinker.Resources.tinker_6_2_01.";
                        break;
                    case "6.2.6":
                        resbase = "Tinker.Resources.tinker_6_2_06.";
                        break;
                    case "6.3.3":
                        resbase = "Tinker.Resources.tinker_6_3_03.";
                        lstRespathFilename.Add(new Tuple<string, string>(resbase+"libiomp5md.dll", "libiomp5md.dll"));
                        break;
                    default:
                        throw new ArgumentException("version");
                }
                switch(resname)
                {
                    case "pdbxyz": lstRespathFilename.Add(new Tuple<string, string>(resbase+"pdbxyz.exe", "pdbxyz.exe")); break;
                    default:
                        throw new ArgumentException(resname +" in resnames");
                }
            }
            return lstRespathFilename.ToArray();
        }
        public static string[] GetResourceLines(string name)
        {
            string resbase = "HTLib2.Bioinfo.HTLib2.Bioinfo.External.Tinker.Resources.";
            resbase = "";
            string[] lines = HResource.GetResourceLines<Tinker>(resbase+name);
            return lines;
        }
    }
}
