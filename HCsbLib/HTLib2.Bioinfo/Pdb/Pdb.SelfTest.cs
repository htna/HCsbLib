using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        public static void SelfTest(string rootpath, string[] args)
        {
            {
                Pdb pdb = Pdb.FromFile(rootpath + @"\Sample\alanin.pdb");
                List<Vector> coords = pdb.atoms.ListCoord();
                coords[0] = new Vector(0, 1, 2);
                pdb.ToFile(rootpath + @"\Sample\alanin.updated.pdb", coords);
            }
            {
                Pdb pdb = Pdb.FromFile(rootpath + @"\Sample\1a6g.pdb");
                //pdb.CollectResidues();
            }
        }
	}
}
