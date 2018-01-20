using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Universe.Atom;
    using Bond = Universe.Bond;
    using RotableInfo = Universe.RotableInfo;
public static partial class Paper
{
    public partial class TNM
    {
        public static Vector[] RotateTorsionals( Vector[] coords
                                               , double[] dangles
                                               , List<Universe.RotableInfo> univ_rotinfos
                                               )
        {
            return RotateTorsionals(coords, dangles, univ_rotinfos, null);
        }
        public static Vector[] RotateTorsionals( Vector[] coords
                                               , double[] dangles
                                               , List<Universe.RotableInfo> univ_rotinfos
                                               , Vector[] alignto // do not align if null
                                               )
        {
            Universe.Molecule mole;
            {
                Universe.Molecule[] moles = univ_rotinfos.ListMolecule().HUnion().ToArray();
                if(moles.Length != 1)
                    throw new Exception();
                mole = moles[0];
            }

            Vector[] newcoords = coords.HClone<Vector>();
            HDebug.Assert(univ_rotinfos.Count == dangles.Length);
            for(int i=0; i<univ_rotinfos.Count; i++)
            {
                Universe.RotableInfo rotinfo = univ_rotinfos[i];
                Vector rotOrigin = newcoords[rotinfo.bondedAtom.ID];
                double rotAngle  = dangles[i];
                if(rotAngle == 0)
                    continue;
                Vector rotAxis   = newcoords[rotinfo.bond.atoms[1].ID] - newcoords[rotinfo.bond.atoms[0].ID];
                Quaternion rot = new Quaternion(rotAxis, rotAngle);
                MatrixByArr rotMat = rot.RotationMatrix;
                foreach(Atom atom in rotinfo.rotAtoms)
                {
                    int id = atom.ID;
                    Vector coord = rotMat * (newcoords[id] - rotOrigin) + rotOrigin;
                    newcoords[id] = coord;
                }
            }

            if(alignto != null)
            {
                int[] idxs = mole.GetAtomIDs();
                Vector[] lalignto = alignto.HSelectByIndex(idxs).ToArray();
                Vector[] lncoords = newcoords.HSelectByIndex(idxs).ToArray();
                Align.MinRMSD.Align(lalignto, ref lncoords);
                newcoords = newcoords.HSetByIndex(idxs, lncoords).ToArray();
            }

            return newcoords;
        }
    }
}
}
