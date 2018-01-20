using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public bool Verify()
        {
            if(atoms.Verify() == false) return false;
            return true;
        }
	}
}
