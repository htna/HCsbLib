using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Charmm
    {
        public class Crd
        {
            public static bool SelfTest()
            {
                string crdpath = @"K:\svn\amazonaws-htna-VisualStudioSolutions\Library2\HTLib2.Updating\HTLib2.Bioinfo\External.Charmm\CHARMM data\water_sphere_825.crd";
                FromFile(crdpath);
                return true;
            }
            public struct Atom
            {
                public string line;
                /// https://gist.github.com/sunhwan/7176913
                /// crd.write("  %8d  %8d  %-8s  %-8s  %18.10f  %18.10f  %18.10f  %-8s  %-8d  %18.10f\n" %
                ///   (atom['num'],
                ///                    resid,
                ///                       atom['resname'],
                ///                                 atom['name'],
                ///                                                    pdb[i][0],
                ///                                                                        pdb[i][1],
                ///                                                                                            pdb[i][2],
                ///                                                                                                       atom['segid'],
                ///                                                                                                                 atom['resid'],
                ///                                                                                                                                 pdb[i][3]))
                /// "         1         1  TIP3      OH2           -79.8000000000      -20.4000000000       -4.2000000000  BWAT      1               0.0000000000"
                /// "         2         1  TIP3      H1            -79.8170000000      -20.3060000000       -3.2480000000  BWAT      1               0.0000000000"
                ///  01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                ///  0         1         2         3         4         5         6         7         8         9         10        11        12        13
                public string _serial  { get { return line.Substring(  0, 10); } } public int    serial  { get { return int.Parse(_serial); } } 
                public string _resSeq  { get { return line.Substring( 10, 10); } } public int    resSeq  { get { return int.Parse(_resSeq); } } 
                public string  resName { get { return line.Substring( 20, 10); } }
                public string  name    { get { return line.Substring( 30, 10); } }
                public string _x       { get { return line.Substring( 40, 20); } } public double x       { get { return double.Parse(_x); } } 
                public string _y       { get { return line.Substring( 60, 20); } } public double y       { get { return double.Parse(_y); } } 
                public string _z       { get { return line.Substring( 80, 20); } } public double z       { get { return double.Parse(_z); } } 
                public string  segi    { get { return line.Substring(100, 10); } }
                public string _resi2   { get { return line.Substring(110, 10); } } public int    resi2   { get { return int.Parse(_resi2); } } 
                public string  ext     { get { return line.Substring(120, 20); } }
                public override string ToString()
                {
                    return line;
                }
            }

            public Atom[] atoms;
            public static Crd FromFile(string path)
            {
                string[] lines = HFile.ReadAllLines(path);

                int?   num_elems = null;
                List<Atom> elems = null;
                foreach(string line in lines)
                {
                    if(line.StartsWith("*"))
                        // comment
                        continue;

                    // "    358272  EXT"
                    // "         1         1  TIP3      OH2           -79.8000000000      -20.4000000000       -4.2000000000  BWAT      1               0.0000000000"
                    //  01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                    //  0         1         2         3         4         5         6         7         8         9         10        11        12        13
                    // "  %8d  %8d  %-8s  %-8s  %18.10f  %18.10f  %18.10f  %-8s  %-8d  %18.10f"
                    if(elems == null)
                    {
                        // "    358272  EXT"
                        //  012345678901234
                        //  0         1    
                        string[] tokens = new string[]
                        {
                            line.Substring(0,10),
                            line.Substring(10),
                        };
                        num_elems = int.Parse(tokens[0]);
                        HDebug.Exception(tokens[1] == "  EXT");
                        elems = new List<Atom>(num_elems.Value);
                    }
                    else
                    {
                        elems.Add(new Atom { line = line });
                    }
                }
                HDebug.Assert(elems.Count == num_elems);

                return new Crd
                {
                    atoms = elems.ToArray(),
                };
            }
        }
    }
}
