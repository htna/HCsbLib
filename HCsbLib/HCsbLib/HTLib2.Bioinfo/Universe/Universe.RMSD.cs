using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public double GetRMSD(Pdb pdb, char selAltLoc='A', bool includeHydrogen=false)
        {
            List<Tuple<int,int>> idxs_univ2pdb = GetIndex(pdb, selAltLoc, includeHydrogen);

            List<int>    atomidxs   = new List<int>();
            List<Vector> atomcoords = new List<Vector>();
            foreach(Tuple<int,int> idx_univ2pdb in idxs_univ2pdb)
            {
                int    atomidx   = idx_univ2pdb.Item1;
                Vector atomcoord = pdb.atoms[idx_univ2pdb.Item2].coord;
                atomidxs.Add(atomidx);
                atomcoords.Add(atomcoord);
            }

            return GetRMSD(atomidxs, atomcoords);
        }
        public double GetRMSD(IList<int> atomidxs, IList<Vector> atomcoords)
        {
            List<Vector> coords = new List<Vector>();
            foreach(int atomidx in atomidxs)
                coords.Add(atoms[atomidx].Coord.Clone());

            Trans3 trans = ICP3.OptimalTransform(coords, atomcoords);

            List<double> SDs = new List<double>();
            for(int i=0; i<coords.Count; i++)
            {
                Vector moved = trans.DoTransform(coords[i]);
                Vector to    = atomcoords[i];
                double SD = (moved - to).Dist2;
                SDs.Add(SD);
            }

            double RMSD = Math.Sqrt(SDs.Average());
            return RMSD;
        }
	}
}
