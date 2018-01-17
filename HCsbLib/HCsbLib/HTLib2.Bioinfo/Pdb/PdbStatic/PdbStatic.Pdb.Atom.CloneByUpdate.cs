using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;

namespace HTLib2.Bioinfo
{
    using Atom = Pdb.Atom;
    using IAtom = Pdb.IAtom;
    public static partial class PdbStatic
    {
        public static List<Atom> CloneByUpdateCoord(this IList<Atom> atoms, IList<Vector> coords)
        {
            string optNullAtom = null;
            return CloneByUpdateCoord(atoms, coords, optNullAtom);
        }
        public static List<Atom> CloneByUpdateCoord(this IList<Atom> atoms, IList<Vector> coords, string optNullAtom)
        {
            HDebug.Assert(optNullAtom == null);
            HDebug.Assert(atoms.Count == coords.Count);
            int count = Math.Max(atoms.Count, coords.Count);
            Atom[] uatoms = new Atom[count];
            
            for(int i=0; i<count; i++)
            {
                Atom atom = atoms[i];
                if(atom == null)
                {
                    switch(optNullAtom)
                    {
                        case null: goto default;
                        case "if null, set as hydrogen":
                            {
                                int maxserial = atoms.HRemoveAll(null).ListSerial().Max();
                                // "ATOM      2  HT1 ARG A  17     -21.666 -13.812 202.912  0.00  0.00      P1   H"
                                atom = Atom.FromData
                                    (
                                        serial : maxserial+1        ,
                                        name   : "H"                ,
                                        resName: uatoms[i-1].resName,
                                        chainID: uatoms[i-1].chainID,
                                        resSeq : uatoms[i-1].resSeq ,
                                        x      : 0                  ,
                                        y      : 0                  ,
                                        z      : 0                  ,
                                        element: "H"
                                    );
                                maxserial++;
                            }
                            break;
                        default:
                            throw new Exception();
                    }
                }
                uatoms[i] = Atom.FromString(atom.GetUpdatedLine(coords[i]));
            }
            return uatoms.ToList();
        }
        public static List<Atom> CloneByUpdateTempFactor(this IList<Atom> atoms, IList<double> tempfactors)
        {
            HDebug.Assert(atoms.Count == tempfactors.Count);
            List<Atom> uatoms = new List<Atom>(atoms);
            for(int i=0; i<uatoms.Count; i++)
                uatoms[i] = Atom.FromString(atoms[i].GetUpdatedLineTempFactor(tempfactors[i]));
            return uatoms;
        }
        public static List<Atom> CloneByUpdateResSeqByAdd(this IList<Atom> atoms, int resSeqAdd)
        {
            List<Atom> uatoms = new List<Atom>(atoms);
            for(int i=0; i<uatoms.Count; i++)
                uatoms[i] = Atom.FromString(atoms[i].GetUpdatedLineResSeq(atoms[i].resSeq+resSeqAdd));
            return uatoms;
        }
        public static List<Atom> CloneByUpdateResSeq(this IList<Atom> atoms, int resSeq)
        {
            List<Atom> uatoms = new List<Atom>(atoms);
            for(int i=0; i<uatoms.Count; i++)
                uatoms[i] = Atom.FromString(atoms[i].GetUpdatedLineResSeq(resSeq));
            return uatoms;
        }
        public static List<Atom> CloneByUpdateResSeq(this IList<Atom> atoms, int resSeqFrom, int resSeqTo, bool bSetNullOthers)
        {
            return atoms.CloneByUpdateResSeq(new int[] { resSeqFrom }, new int[] { resSeqTo }, bSetNullOthers);
        }
        public static List<Atom> CloneByUpdateResSeq(this IList<Atom> atoms, IList<int> resSeqFrom, IList<int> resSeqTo, bool bSetNullOthers)
        {
            if(resSeqFrom.Count != resSeqTo.Count)
                throw new ArgumentException("resSeqFrom.Count != resSeqTo.Count");

            if(bSetNullOthers)
                return atoms.CloneByUpdateResSeq(resSeqFrom, resSeqTo);

            {
                Dictionary<int, int> from_to = new Dictionary<int, int>();
                for(int i=0; i<resSeqFrom.Count; i++)
                    from_to.Add(resSeqFrom[i], resSeqTo[i]);
                foreach(var atom in atoms)
                    if(from_to.ContainsKey(atom.resSeq) == false)
                        from_to.Add(atom.resSeq, atom.resSeq);
                return atoms.CloneByUpdateResSeq(from_to.Keys.ToList(), from_to.Values.ToList());
            }
        }
        public static List<Atom> CloneByUpdateResSeq(this IList<Atom> atoms, IList<int> resSeqFrom, IList<int> resSeqTo)
        {
            if(resSeqFrom.Count != resSeqTo.Count)
                throw new ArgumentException("resSeqFrom.Count != resSeqTo.Count");
            List<Atom> uatoms = new List<Atom>(atoms);
            for(int i=0; i<uatoms.Count; i++)
            {
                int idx = resSeqFrom.IndexOf(uatoms[i].resSeq);
                if(idx == -1)
                    uatoms[i] = null;
                else
                {
                    int resSeq = resSeqTo[idx];
                    uatoms[i] = Atom.FromString(atoms[i].GetUpdatedLineResSeq(resSeq));
                }
            }
            return uatoms;
        }

        public static List<Atom> CloneByRemoveSymbolAltLoc(this IList<Atom> atoms)
        {
            Atom[] natoms = new Atom[atoms.Count];
            for(int i=0; i<natoms.Length; i++)
            {
                Atom atom = atoms[i];
                if(atom.altLoc != ' ')
                {
                    atom = Atom.FromData(
                        serial    : atom.serial    ,
                        name      : atom.name      ,
                        resName   : atom.resName   ,
                        chainID   : atom.chainID   ,
                        resSeq    : atom.resSeq    ,
                        x         : atom.x         ,
                        y         : atom.y         ,
                        z         : atom.z         ,
                        altLoc    : ' '            ,
                        iCode     : atom.iCode     ,
                        occupancy : atom.occupancy ,
                        tempFactor: atom.tempFactor,
                        element   : atom.element   ,
                        charge    : atom.charge    
                        );
                }
                natoms[i] = atom;
            }
            return natoms.ToList();
        }
        public static IList<Atom> CloneByReindexSerials(this IList<Atom> atoms, int serialStart=1)
        {
            Atom[] natoms = new Atom[atoms.Count];
            for(int i=0; i<natoms.Length; i++)
            {
                Atom atom = atoms[i];
                natoms[i] = Atom.FromData(serial     : serialStart+i
                                         , name      : atom.name
                                         , resName   : atom.resName
                                         , chainID   : atom.chainID
                                         , resSeq    : atom.resSeq
                                         , x         : atom.x
                                         , y         : atom.y
                                         , z         : atom.z
                                         , altLoc    : atom.altLoc
                                         , iCode     : atom.iCode
                                         , occupancy : atom.occupancy
                                         , tempFactor: atom.tempFactor
                                         , element   : atom.element
                                         , charge    : atom.charge
                                         , segment   : atom.segment
                                         );
            }
            return natoms;
        }
        public static IList<Atom> CloneByReindexByCoords(this IList<Atom> atoms, IList<Vector> coords, int serialStart=1)
        {
            KDTree.KDTree<object> kdtree = new KDTree.KDTree<object>(3);
            for(int i=0; i<coords.Count; i++)
                kdtree.insert(coords[i], i);

            Atom[] natoms = new Atom[atoms.Count];
            for(int ai=0; ai<natoms.Length; ai++)
            {
                Atom   atom  = atoms[ai];
                Vector coord = atom.coord;
                int ni = (int)(kdtree.nearest(coord));
                Vector ncoord = coords[ni];

                double dist = (coord - ncoord).Dist;
                HDebug.AssertTolerance(0.001, dist);
                HDebug.Assert(natoms[ni] == null);

                natoms[ni] = Atom.FromData(serial     : serialStart+ni
                                         , name      : atom.name
                                         , resName   : atom.resName
                                         , chainID   : atom.chainID
                                         , resSeq    : atom.resSeq
                                         , x         : atom.x
                                         , y         : atom.y
                                         , z         : atom.z
                                         , altLoc    : atom.altLoc
                                         , iCode     : atom.iCode
                                         , occupancy : atom.occupancy
                                         , tempFactor: atom.tempFactor
                                         , element   : atom.element
                                         , charge    : atom.charge
                                         );
            }

            if(HDebug.IsDebuggerAttached)
            {
                for(int i=0; i<natoms.Length; i++)
                    HDebug.Assert(natoms[i] != null);
            }

            return natoms;
        }
    }
}
