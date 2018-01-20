using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class HydroBond
    {
        public class PHydroBond
        {
            // \               /
            //  Ni-Hi ... Oj=Cj
            // /               \
            public Universe.Atom Ni, Hi;
            public Universe.Atom Oj, Cj;

            public double energy;
            public double spring; // spring between Hi..Oj
        }
    }
}
