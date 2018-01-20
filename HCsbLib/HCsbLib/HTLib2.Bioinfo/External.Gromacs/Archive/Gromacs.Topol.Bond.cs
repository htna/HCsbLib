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
            public class Bond : Element
            {
                public readonly int ai;
                public readonly int aj;
                public readonly int funct; // function number
                //c0            c1            c2            c3
                public Bond(string line,
                            int ai, int aj, int funct)
                    : base(line)
                {
                    this.ai    = ai;
                    this.aj    = aj;
                    this.funct = funct;
                }

                public static Bond FromString(string line)
                {
                    //[ bonds ]
                    //;  ai    aj funct            c0            c1            c2            c3
                    //    1     2     1 
                    string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    Debug.Assert(tokens.Length == 3);
                    int ai    = int.Parse(tokens[0]);
                    int aj    = int.Parse(tokens[1]);
                    int funct = int.Parse(tokens[2]);
                    return new Bond(line, ai, aj, funct);
                }
            }
        }
    }
}
*/