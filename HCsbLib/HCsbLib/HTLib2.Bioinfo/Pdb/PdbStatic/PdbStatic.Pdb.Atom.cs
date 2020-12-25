using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using IAtom = Pdb.IAtom;
    public static partial class PdbStatic
    {
        public static void ToFile(this IList<Pdb.Atom> atoms, string filepath)
        {
            string[] lines = new string[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                lines[i] = atoms[i].line;
            HFile.WriteAllLines(filepath, lines);
        }

        public static List<   int> ListSerial    <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<   int> list = new List<   int>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.serial    ); return list; }
        public static List<string> ListName      <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<string> list = new List<string>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.name      ); return list; }
        public static List<  char> ListAltLoc    <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<  char> list = new List<  char>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.altLoc    ); return list; }
        public static List<string> ListResName   <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<string> list = new List<string>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.resName   ); return list; }
        public static List<  char> ListChainID   <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<  char> list = new List<  char>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.chainID   ); return list; }
        public static List<   int> ListResSeq    <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<   int> list = new List<   int>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.resSeq    ); return list; }
        public static List<  char> ListICode     <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<  char> list = new List<  char>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.iCode     ); return list; }
        public static List<double> ListX         <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<double> list = new List<double>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.x         ); return list; }
        public static List<double> ListY         <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<double> list = new List<double>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.y         ); return list; }
        public static List<double> ListZ         <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<double> list = new List<double>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.z         ); return list; }
        public static List<double> ListOccupancy <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<double> list = new List<double>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.occupancy ); return list; }
        public static List<double> ListTempFactor<ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<double> list = new List<double>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.tempFactor); return list; }
        public static List<string> ListElement   <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<string> list = new List<string>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.element   ); return list; }
        public static List<string> ListCharge    <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<string> list = new List<string>(atoms.Count); foreach(ATOM atom in atoms) list.Add(atom.charge    ); return list; }
        public static List<Vector> ListCoord     <ATOM>(this IList<ATOM> atoms) where ATOM:IAtom  { List<Vector> list = new List<Vector>(atoms.Count); foreach(ATOM atom in atoms) list.Add(new Vector(atom.x, atom.y, atom.z)); return list; }

        public static List<char?> ListResNameSyn(this IList<Pdb.Atom> atoms)
        {
            List<char?> list = new List<char?>(atoms.Count);
            foreach(var atom in atoms)
                list.Add(atom.resNameSyn);
            return list;
        }
        public static string ListResNameSynString(this IList<Pdb.Atom> atoms)
        {
            StringBuilder str = new StringBuilder();
            List<char?> syns = ListResNameSyn(atoms);
            foreach(char? syn in syns)
            {
                if(syn == null) str.Append('?');
                else            str.Append(syn.Value);
            }
            return str.ToString();
        }

        public static char[] ListSecondStruc<ATOM>(this IList<ATOM> atoms, IList<Pdb.Helix> helix, IList<Pdb.Sheet> sheet)
            where ATOM : IAtom
        {
            Dictionary<ATOM, char> atom_struc = new Dictionary<ATOM, char>();
            {
                foreach(Tuple<Pdb.Helix[], ATOM[]> helix_atoms in helix.HSelectAtoms(atoms))
                    foreach(ATOM atom in helix_atoms.Item2)
                    {
                        if(atom_struc.ContainsKey(atom) == false)
                            atom_struc.Add(atom, 'H');
                        HDebug.Assert(atom_struc[atom] == 'H');
                    }
                foreach(Tuple<Pdb.Sheet[], ATOM[]> sheet_atoms in sheet.HSelectAtoms(atoms))
                    foreach(ATOM atom in sheet_atoms.Item2)
                    {
                        if(atom_struc.ContainsKey(atom) == false)
                            atom_struc.Add(atom, 'S');
                        HDebug.Assert(atom_struc[atom] == 'S');
                    }
            }
            int leng = atoms.Count;
            char[] secondstruc = new char[leng];
            for(int i=0; i<leng; i++)
            {
                var atom = atoms[i];
                if(atom_struc.ContainsKey(atom))
                    secondstruc[i] = atom_struc[atom];
                else
                    secondstruc[i] = 'L';
            }
            return secondstruc;
        }
        
        public static List<ATOM> SelectBySerial    <ATOM>(this IList<ATOM> atoms, params    int[] serial    ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(serial    .Contains(atom.serial    )) list.Add(atom); return list; }
        public static List<ATOM> SelectByName      <ATOM>(this IList<ATOM> atoms, params string[] name      ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); name=name.HTrim().ToArray(); foreach(var atom in atoms) if(name      .Contains(atom.name.Trim())) list.Add(atom); return list; }
        public static List<ATOM> SelectByAltLoc    <ATOM>(this IList<ATOM> atoms, params   char[] altLoc    ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(altLoc    .Contains(atom.altLoc    )) list.Add(atom); return list; }
        public static List<ATOM> SelectByResName   <ATOM>(this IList<ATOM> atoms, params string[] resName   ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(resName   .Contains(atom.resName   )) list.Add(atom); return list; }
        public static List<ATOM> SelectByChainID   <ATOM>(this IList<ATOM> atoms, params   char[] chainID   ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(chainID   .Contains(atom.chainID   )) list.Add(atom); return list; }
        public static List<ATOM> SelectByResSeq    <ATOM>(this IList<ATOM> atoms, params    int[] resSeq    ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(resSeq    .Contains(atom.resSeq    )) list.Add(atom); return list; }
        public static List<ATOM> SelectByICode     <ATOM>(this IList<ATOM> atoms, params   char[] iCode     ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(iCode     .Contains(atom.iCode     )) list.Add(atom); return list; }
      //public static List<ATOM> SelectByX         <ATOM>(this IList<ATOM> atoms, params double[] x         ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(x         .Contains(atom.x         )) list.Add(atom); return list; }
      //public static List<ATOM> SelectByY         <ATOM>(this IList<ATOM> atoms, params double[] y         ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(y         .Contains(atom.y         )) list.Add(atom); return list; }
      //public static List<ATOM> SelectByZ         <ATOM>(this IList<ATOM> atoms, params double[] z         ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(z         .Contains(atom.z         )) list.Add(atom); return list; }
      //public static List<ATOM> SelectByOccupancy <ATOM>(this IList<ATOM> atoms, params double[] occupancy ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(occupancy .Contains(atom.occupancy )) list.Add(atom); return list; }
      //public static List<ATOM> SelectByTempFactor<ATOM>(this IList<ATOM> atoms, params double[] tempFactor) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(tempFactor.Contains(atom.tempFactor)) list.Add(atom); return list; }
        public static List<ATOM> SelectByElement   <ATOM>(this IList<ATOM> atoms, params string[] element   ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(element   .Contains(atom.element   )) list.Add(atom); return list; }
        public static List<ATOM> SelectByCharge    <ATOM>(this IList<ATOM> atoms, params string[] charge    ) where ATOM:IAtom { List<ATOM> list = new List<ATOM>(); foreach(var atom in atoms) if(charge    .Contains(atom.charge    )) list.Add(atom); return list; }

        public static int[] IdxByName<ATOM>(this IList<ATOM> atoms, bool skipNullAtom, params string[] names)
            where ATOM: IAtom
        {
            HashSet<string> setnames = new HashSet<string>(names.HTrim());
            List<int> idxs = new List<int>();
            for(int i=0; i<atoms.Count; i++)
            {
                if(atoms[i] == null && skipNullAtom)
                    continue;
                string name = atoms[i].name.Trim();
                if(setnames.Contains(name))
                    idxs.Add(i);
            }
            return idxs.ToArray();
        }

        public static ResInfo[] ListResInfo<ATOM>(this IList<ATOM> atoms, bool skipNullAtom)
            where ATOM : IAtom
        {
            ResInfo[] resinfos = new ResInfo[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
                if(atoms[i] != null && skipNullAtom)
                    resinfos[i] = atoms[i].GetResInfo();
            return resinfos.ToArray();
        }

        public static List<ATOM> SelectByDefault<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            List<ATOM> latoms = new List<ATOM>(atoms);
            latoms = SelectByAltLoc (latoms);
            latoms = SelectByChainID(latoms);
            latoms = SelectByICode  (latoms);
            return latoms;
        }
        public static List<ATOM> SelectByAltLoc<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            return SelectByAltLoc(atoms, ' ', 'A', '1');
        }
        public static List<ATOM> SelectByChainID<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            char chainID = atoms[0].chainID;
            HDebug.Assert(chainID != ' ');
            return atoms.SelectByChainID(chainID);
        }
        public static List<ATOM> SelectByICode<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            return SelectByICode(atoms, ' ');
        }

        public static List<ATOM> ListNonHydrogen<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            List<ATOM> list = new List<ATOM>();
            foreach(var atom in atoms)
            {
                string name = atom.name.Trim();
                bool bHydrogen = false;
                if(name[0] == 'H') bHydrogen = true;
                if((name[0]>='0' && name[0]<='9') && (name[1] == 'H')) bHydrogen = true;

                if(bHydrogen == false)
                    list.Add(atom);
            }
            return list;
        }
        public static Dictionary<char, List<ATOM>> GroupChainID<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            Dictionary<char,List<ATOM>> group = new Dictionary<char, List<ATOM>>();
            foreach(var atom in atoms)
            {
                char key = atom.chainID;
                if(group.ContainsKey(key) == false)
                    group.Add(key, new List<ATOM>());
                group[key].Add(atom);
            }
            return group;
        }
        public static Dictionary<char, Dictionary<int, List<ATOM>>> GroupChainIDResSeq<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            Dictionary<char, List<ATOM>> chain_atoms = atoms.GroupChainID();

            Dictionary<char, Dictionary<int, List<ATOM>>> chain_resi_atoms = new Dictionary<char, Dictionary<int, List<ATOM>>>();
            foreach(var item in chain_atoms)
            {
                char chain = item.Key;
                var  resi_atoms = item.Value.GroupResSeq();
                chain_resi_atoms.Add(chain, resi_atoms);
            }
            return chain_resi_atoms;
        }
        public static Dictionary<char, Dictionary<int, Dictionary<string, ATOM>>> GroupChainIDResSeqName<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            Dictionary<char, Dictionary<int, List<ATOM>>> chain_resi_atoms = atoms.GroupChainIDResSeq();

            Dictionary<char, Dictionary<int, Dictionary<string, ATOM>>> chain_resi_name_atom = new Dictionary<char, Dictionary<int, Dictionary<string, ATOM>>>();
            foreach(char chain in chain_resi_atoms.Keys)
            {
                chain_resi_name_atom.Add(chain, new Dictionary<int, Dictionary<string, ATOM>>());
                var chain_resinameatom = chain_resi_name_atom[chain];
                var chain_resiatoms    = chain_resi_atoms    [chain];
                foreach(int resi in chain_resiatoms.Keys)
                {
                    chain_resinameatom.Add(resi, new Dictionary<string, ATOM>());
                    var chainresi_nameatom = chain_resinameatom[resi];
                    var chainresi_atoms    = chain_resiatoms   [resi];
                    foreach(var atom in chainresi_atoms)
                    {
                        chainresi_nameatom.Add(atom.name.Trim(), atom);
                    }
                }
            }
            return chain_resi_name_atom;
        }
        public static Dictionary<int, List<ATOM>> GroupResSeq<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            if(atoms.ListChainID().HToHashSet().Count != 1)
                throw new HException();
            Dictionary<int,List<ATOM>> group = new Dictionary<int, List<ATOM>>();
            foreach(ATOM atom in atoms)
            {
                int key = atom.resSeq;
                if(group.ContainsKey(key) == false)
                    group.Add(key, new List<ATOM>());
                group[key].Add(atom);
            }
            return group;
        }
        public static int GetResSeqByIndex<ATOM>(this IList<ATOM> atoms, int idx, string optNullAtom)
            where ATOM : IAtom
        {
            if(atoms[idx] == null)
            {
                HDebug.Assert(optNullAtom == null);
                switch(optNullAtom)
                {
                    case "if null, search back": while(atoms[idx] == null) idx--; break;
                    default: throw new Exception();
                }
            }
            int resseq = atoms[idx].resSeq;
            return resseq;
        }
        public static int[] GetResSeqByIndex<ATOM>(this IList<ATOM> atoms, string optNullAtom, params int[] idxs)
            where ATOM : IAtom
        {
            int[] resseqs = new int[idxs.Length];
            for(int i=0; i<idxs.Length; i++)
            {
                resseqs[i] = GetResSeqByIndex(atoms, idxs[i], optNullAtom);
            }
            return resseqs;
        }
        public static Tuple<int,int,int> GetResSeqByIndex<ATOM>(this IList<ATOM> atoms, Tuple<int,int,int> idx)
            where ATOM : IAtom
        {
            string optNullAtom = null;
            return GetResSeqByIndex(atoms, optNullAtom, idx.HToArray()).HToTuple3();
        }
        public static Tuple<int,int,int>[] GetResSeqByIndex<ATOM>(this IList<ATOM> atoms, IList<Tuple<int,int,int>> idx)
            where ATOM : IAtom
        {
            string optNullAtom = null;
            return GetResSeqByIndex(atoms, idx, optNullAtom);
        }
        public static Tuple<int,int,int>[] GetResSeqByIndex<ATOM>(this IList<ATOM> atoms, IList<Tuple<int,int,int>> idx, string optNullAtom)
            where ATOM : IAtom
        {
            Tuple<int,int,int>[] resseq = new Tuple<int, int, int>[idx.Count];
            for(int i=0; i<resseq.Length; i++)
                resseq[i] = atoms.GetResSeqByIndex(optNullAtom, idx[i].HToArray()).HToTuple3();
            return resseq;
        }
        public static List<ATOM> FindAtoms<ATOM>(this IList<ATOM> atoms, string name, int resSeq)
            where ATOM : IAtom
        {
            List<ATOM> list = new List<ATOM>();
            foreach(var atom in atoms)
            {
                string name1 = atom.name.Trim().ToUpper();
                string name2 =      name.Trim().ToUpper();
                if(name1 != name2) continue;
                if(atom.resSeq != resSeq) continue;
                list.Add(atom);
            }
            return list;
        }

        public static List<int> IndexOfNames<ATOM>(this IList<ATOM> atoms, string name)
            where ATOM : IAtom
        {
            List<int> index = new List<int>();
            for(int i=0; i<atoms.Count; i++)
                if(atoms[i].name.Trim() == name)
                    index.Add(i);
            return index;
        }

        public static int IndexOfAtom<ATOM>(this IList<ATOM> atoms, string name, int resSeq)
            where ATOM : IAtom
        {
            for(int idx=0; idx<atoms.Count; idx++)
            {
                ATOM atom = atoms[idx];
                string name1 = atom.name.Trim().ToUpper();
                string name2 =      name.Trim().ToUpper();
                if(name1 != name2) continue;
                if(atom.resSeq != resSeq) continue;
                return idx;
            }
            return -1;
        }
        public static List<int> IndexOfAtoms<ATOM>(this IList<ATOM> atoms, IList<ATOM> atomsToFind)
            where ATOM : IAtom
        {
            int[] index = new int[atomsToFind.Count];
            for(int i=0; i<atomsToFind.Count; i++)
                index[i] = atoms.IndexOfAtom(atomsToFind[i].name, atomsToFind[i].resSeq);
            return new List<int>(index);
        }


        public static List<Vector>[] ListCoords<ATOM>(this IList<Pdb> pdbs)
            where ATOM : IAtom
        {
            List<Vector>[] coordss = new List<Vector>[pdbs.Count];
            for(int i=0; i<pdbs.Count; i++)
                coordss[i] = pdbs[i].atoms.ListCoord();
            return coordss;
        }

        public static Dictionary<int,ATOM> ToDictionaryBySerial<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            Dictionary<int, ATOM> dict = new Dictionary<int, ATOM>();
            foreach(var atom in atoms)
                dict.Add(atom.serial, atom);
            return dict;
        }
        public static Dictionary<int, ATOM[]> ToDictionaryByResseq<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            Dictionary<int, ATOM[]> dict = new Dictionary<int, ATOM[]>();
            foreach(var atom in atoms)
            {
                if(dict.ContainsKey(atom.resSeq) == false)
                    dict.Add(atom.resSeq, new ATOM[0]);
                dict[atom.resSeq] = dict[atom.resSeq].HAdd(atom).ToArray();
            }
            return dict;
        }
        public static Dictionary<int, int> ToDictionaryAsSerialToIndex<ATOM>(this IList<ATOM> atoms)
            where ATOM : IAtom
        {
            Dictionary<int, int> dict = new Dictionary<int, int>();
            for(int ia=0; ia<atoms.Count; ia++)
            {
                ATOM atom = atoms[ia];
                dict.Add(atom.serial, ia);
            }
            return dict;
        }

        public static IList<ATOM> SelectByRefNameResSeq<ATOM>(this IList<ATOM> atoms, IList<ATOM> refatoms)
            where ATOM : IAtom
        {
            //HDebug.Assert(atoms.Count == refatoms.Count);
            ATOM[] select = new ATOM[refatoms.Count];

            var resseq2atoms = atoms.ToDictionaryByResseq();
            for(int i=0; i<select.Length; i++)
            {
                string name    = refatoms[i].name;
                int    resseq  = refatoms[i].resSeq;
                List<ATOM> latoms = resseq2atoms[resseq].SelectByName(name);
                if(latoms.Count >= 1)
                {
                    HDebug.Assert(latoms.Count == 1);
                    string refResName = refatoms[i].resName;
                    string lResName   = latoms[0].resName;
                    //HDebug.Assert(refResName.Trim() == lResName.Trim());
                    select[i] = latoms[0];
                }
                else
                {
                    HDebug.Assert(false);
                    select[i] = null;
                }
            }
            return select;
        }
    }
}
