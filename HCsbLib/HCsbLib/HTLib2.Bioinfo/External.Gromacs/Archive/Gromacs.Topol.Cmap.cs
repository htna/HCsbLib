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
            public class Cmap : Element
            {
                public readonly int ai;
                public readonly int aj;
                public readonly int ak;
                public readonly int al;
                public readonly int am;
                public readonly int funct; // function number
                //c0            c1            c2            c3
                public Cmap(string line,
                            int ai, int aj, int ak, int al, int am, int funct)
                    : base(line)
                {
                    this.ai    = ai;
                    this.aj    = aj;
                    this.ak    = ak;
                    this.al    = al;
                    this.am    = am;
                    this.funct = funct;
                }

                public static Cmap FromString(string line)
                {
                    //[ cmap ]
                    //;  ai    aj    ak    al    am funct
                    //   25    27    29    44    46     1 
                    string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                    Debug.Assert(tokens.Length == 6);
                    int ai    = int.Parse(tokens[0]);
                    int aj    = int.Parse(tokens[1]);
                    int ak    = int.Parse(tokens[2]);
                    int al    = int.Parse(tokens[3]);
                    int am    = int.Parse(tokens[4]);
                    int funct = int.Parse(tokens[5]);
                    return new Cmap(line, ai, aj, ak, al, am, funct);
                }
            }
        }
    }
}
*/