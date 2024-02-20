using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Element = Tinker.TkFile.Element;
    using Vel     = Tinker.Vel;
    public static partial class TinkerStatic
    {
        public static Tinker.Vel.Atom[] HSelectVelByAtoms(this IList<Tinker.Vel.Atom> vels, IList<Tinker.Xyz.Atom> atoms, IList<Tinker.Xyz.Atom> atomstosel, bool reindex=true)
        {
            HDebug.Assert(vels.Count == atoms     .Count);
            HDebug.Assert(vels.Count >= atomstosel.Count);
            //Dictionary<int, Tinker.Xyz.Atom>  id_atom         = atoms     .HToDictionaryIdAtom();
            //Dictionary<int, Tinker.Xyz.Atom>  id_atomtosel    = atomstosel.HToDictionaryIdAtom();
            Dictionary<int, Tinker.Vel.Atom>  id_vel          = vels      .HToDictionaryIdAtom();
            KDTreeDLL.KDTree<Tinker.Xyz.Atom> coord_atom      = atoms     .HToKDTreeByCoord();
            //KDTreeDLL.KDTree<Tinker.Xyz.Atom> coord_atomtosel = atomstosel.HToKDTreeByCoord();
            // check if atoms and vels are same
            foreach(var atom in atoms)
            {
                int id = atom.Id;
                HDebug.Assert(id_vel.ContainsKey(id));
                var vel = id_vel[id];
                HDebug.Assert(atom.Id       == vel.Id      );
                HDebug.Assert(atom.AtomType == vel.AtomType);
            }
            // select vels
            List<Tinker.Vel.Atom> velstosel = new List<Vel.Atom>(atomstosel.Count);
            foreach(var atomtosel in atomstosel)
            {
                var coord = atomtosel.Coord;
                var atom  = coord_atom.nearest(coord);
                HDebug.Assert((atom.Coord,atomtosel.Coord).Dist2() == 0);
                HDebug.Assert(atom.AtomType == atomtosel.AtomType);
                HDebug.Assert(atom.X        == atomtosel.X       );
                HDebug.Assert(atom.Y        == atomtosel.Y       );
                HDebug.Assert(atom.Z        == atomtosel.Z       );
                HDebug.Assert(atom.AtomId   == atomtosel.AtomId  );
                int id    = atom.Id;
                var vel   = id_vel[id];
                HDebug.Assert(atom.Id       == vel.Id            );
                HDebug.Assert(atom.AtomType == vel.AtomType      );

                Vel.Atom veltosel;
                if(reindex == false)
                    veltosel = vel;
                else
                {
                    veltosel = Vel.Atom.FromData(vel.format, atomtosel.Id, vel.AtomType, vel.DX, vel.DY, vel.DZ);
                }

                if(HDebug.IsDebuggerAttached)
                {
                    int    idxVelAtomType = vel.line.IndexOf(vel.AtomType);
                    string substrVel      = vel     .line.Substring(idxVelAtomType);
                    string substrVeltosel = veltosel.line.Substring(idxVelAtomType);
                    if(substrVel != substrVeltosel)
                    {
                        //HDebug.Assert(vel.Id       == veltosel.Id      );
                        HDebug.Assert(vel.AtomType == veltosel.AtomType);
                        HDebug.Assert(vel.DX       == veltosel.DX      );
                        HDebug.Assert(vel.DY       == veltosel.DY      );
                        HDebug.Assert(vel.DZ       == veltosel.DZ      );
                    }
                }
                velstosel.Add(veltosel);
            }

            HDebug.Assert(atomstosel.Count == velstosel.Count);
            for(int i=0; i<atomstosel.Count; i++)
            {
                            HDebug.Assert(atomstosel[i].AtomType == velstosel[i].AtomType);
                if(reindex) HDebug.Assert(atomstosel[i].Id       == velstosel[i].Id      );
            }

            return velstosel.ToArray();
        }
        public static Dictionary<int, Tinker.Vel.Atom> ToIdDictionary(this IEnumerable<Tinker.Vel.Atom> atoms)
        {
            Dictionary<int, Tinker.Vel.Atom> dict = new Dictionary<int, Tinker.Vel.Atom>();
            foreach(Tinker.Vel.Atom atom in atoms)
                dict.Add(atom.Id, atom);
            return dict;
        }
        public static Tinker.Vel.Atom[] HSelectCorrectAtomType(this IList<Tinker.Vel.Atom> atoms)
        {
            List<Tinker.Vel.Atom> sels = new List<Tinker.Vel.Atom>();
            foreach(var atom in atoms)
                if(atom.AtomType.Trim().Length != 0)
                    sels.Add(atom);
            return sels.ToArray();
        }
        public static Vector[] HListVelXYZ(this IList<Tinker.Vel.Atom> atoms)
        {
            Vector[] coords = new Vector[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                coords[i] = atoms[i].VelXYZ;
            return coords;
        }
        public static IList<int> HListId(this IList<Tinker.Vel.Atom> atoms)
        {
            int[] ids = new int[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                ids[i] = atoms[i].Id;
            return ids;
        }
        public static IEnumerable<int> HEnumId(this IEnumerable<Tinker.Vel.Atom> atoms)
        {
            foreach(var atom in atoms)
                yield return atom.Id;
        }
        public static Dictionary<int, Tinker.Vel.Atom> HToDictionaryIdAtom
            ( this IEnumerable<Tinker.Vel.Atom> atoms
            )
        {
            Dictionary<int, Tinker.Vel.Atom> dict = new Dictionary<int, Tinker.Vel.Atom>();
            foreach(var atom in atoms)
                dict.Add(atom.Id, atom);
            return dict;
        }
        public static Dictionary<int, int> HToDictionaryIdIndex
            ( this IList<Tinker.Vel.Atom> atoms
            )
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for(int idx=0; idx<atoms.Count(); idx++)
            {
                int id = atoms[idx].Id;
                dict.Add(id, idx);
            }
            return dict;
        }
        public static IEnumerable<Tinker.Vel.Atom> HSelectByAtomType(this IEnumerable<Tinker.Vel.Atom> atoms, string AtomType)
        {
            foreach(var atom in atoms)
            {
                if(atom.AtomType == AtomType)
                    yield return atom;
            }
        }
    }
}
