using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        public partial class Atom
		{
            public static bool CompareNameResName(Atom atom1, Atom atom2)
            {
                if(atom1.name    != atom2.name   ) return false;
                if(atom1.resName != atom2.resName) return false;
                return true;
            }
		}
	}
}
