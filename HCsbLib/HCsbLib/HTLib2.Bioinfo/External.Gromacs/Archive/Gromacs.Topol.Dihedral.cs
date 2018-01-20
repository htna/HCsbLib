/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo.External
{
    public partial class Gromacs
    {
        public partial class Topol
        {
            public class Dihedral : Element
            {
                public readonly int ai;
                public readonly int aj;
                public readonly int ak;
                public readonly int al;
                public readonly int funct; // function number
                //c0            c1            c2            c3
                public Dihedral(string line,
                            int ai, int aj, int ak, int al, int funct)
                    : base(line)
                {
                    this.ai    = ai;
                    this.aj    = aj;
                    this.ak    = ak;
                    this.al    = al;
                    this.funct = funct;
                }

                public static Dihedral FromString(string line)
                {
                    //[ dihedrals ]
                    //;  ai    aj    ak    al funct            c0            c1            c2            c3            c4            c5
                    //    2     1     5     6     9 
                    string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    Debug.Assert(tokens.Length == 5);
                    int ai    = int.Parse(tokens[0]);
                    int aj    = int.Parse(tokens[1]);
                    int ak    = int.Parse(tokens[2]);
                    int al    = int.Parse(tokens[3]);
                    int funct = int.Parse(tokens[4]);
                    return new Dihedral(line, ai, aj, ak, al, funct);
                }
            }
        }
    }
}
*/