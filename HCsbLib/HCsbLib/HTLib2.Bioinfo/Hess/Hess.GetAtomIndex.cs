using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public partial class GetAtomIndex
        {
            //public static List<Tuple<int, int[], int[]>> GetListResiICaIOthr(Universe univ)
            //{
            //    Universe.Atom[] atoms = univ.atoms.ToArray();
            //    // Determine residue group
            //    Dictionary<int, Universe.Atom[]> list_resi_atoms = atoms.GroupByResidueId();
            //    // Split residue atoms by Ca and others
            //    List<Tuple<int, int[], int[]>> lstResiCaIOthr = new List<Tuple<int, int[], int[]>>();
            //    foreach(var resi_atoms in list_resi_atoms)
            //    {
            //        int resi     = resi_atoms.Key;
            //        var resatoms = resi_atoms.Value;
            //        var keep_remv = resatoms.HSplitByNames("CA");
            //        var keep = keep_remv.match;
            //        var remv = keep_remv.other;
            //        HDebug.Assert(keep.Length == 1);
            //        lstResiCaIOthr.Add(new Tuple<int, int[], int[]>(
            //            resi,           // res seq
            //            keep.ListIDs(), // index   of Ca
            //            remv.ListIDs()  // indices of other
            //            ));
            //    }
            //    return lstResiCaIOthr;
            //}
        }
    }
}
