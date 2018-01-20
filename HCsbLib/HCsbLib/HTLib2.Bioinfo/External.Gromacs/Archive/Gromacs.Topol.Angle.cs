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
            public class Angle : Element
            {
                public readonly int ai;
                public readonly int aj;
                public readonly int ak;
                public readonly int funct; // function number
                //c0            c1            c2            c3
                public Angle(string line,
                            int ai, int aj, int ak, int funct)
                    : base(line)
                {
                    this.ai    = ai;
                    this.aj    = aj;
                    this.ak    = ak;
                    this.funct = funct;
                }

                public static Angle FromString(string line)
                {
                    //[ angles ]
                    //;  ai    aj    ak funct            c0            c1            c2            c3
                    //    2     1     3     5 
                    string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    Debug.Assert(tokens.Length == 4);
                    int ai    = int.Parse(tokens[0]);
                    int aj    = int.Parse(tokens[1]);
                    int ak    = int.Parse(tokens[2]);
                    int funct = int.Parse(tokens[3]);
                    return new Angle(line, ai, aj, ak, funct);
                }
            }
        }
    }
}
*/