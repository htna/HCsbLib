using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Pdb.Atom;
    using IAtom = Pdb.IAtom;
    public static partial class PdbStatic
    {
        public static List<Atom> ListCommon<Atom>( this IList<Atom> atoms, IList<Atom> atomsToCompare
                                           , HPack<List<int>> outAtomIdx=null // index for the returned list
                                           )
            where Atom : IAtom
        {
            List<Atom> list = new List<Atom>();
            if(outAtomIdx != null)
                outAtomIdx.value = new List<int>();
            foreach(Atom atom in atoms)
            {
                int idx = atomsToCompare.IndexOfAtom(atom.name, atom.resSeq);
                if(idx == -1)
                    continue;
                list.Add(atom);
                if(outAtomIdx != null)
                    outAtomIdx.value.Add(idx);
            }
            return list;
        }

        public static bool CheckCommonAtoms<Atom>(params List<Atom>[] atomss)
            where Atom : IAtom
        {
            for(int k=0; k<atomss.Length; k++) if(atomss[0].Count != atomss[k].Count) return false;
            for(int i=0; i<atomss[0].Count; i++)
            {
                for(int k=0; k<atomss.Length; k++) if(atomss[0][i].name    != atomss[k][i].name   ) return false;
                for(int k=0; k<atomss.Length; k++) if(atomss[0][i].resName != atomss[k][i].resName) return false;
                for(int k=0; k<atomss.Length; k++) if(atomss[0][i].resSeq  != atomss[k][i].resSeq ) return false;
            }
            return true;
        }
        public static void SelectCommonAtoms<Atom>(ref List<Atom>[] atomss)
            where Atom : IAtom
        {
            if(CheckCommonAtoms(atomss))
                return;

            int length = atomss.Length;
            List<Atom>[] latomss = atomss.HClone<List<Atom>>();

            for(int skip=1; skip<length; skip*=2)
            {
                //for(int i=0; i<length; i+=skip*2)
                HParallel.For(0, length, skip*2, delegate(int i)
                {
                    int idx0 = i;
                    int idx1 = i+skip;
                    if((idx0 < length) && (idx1 < length))
                        SelectCommonAtoms(ref latomss[idx0], ref latomss[idx1]);
                }
                );
            }
            System.Threading.Tasks.Parallel.For(1, length, delegate(int i)
            {
                List<Atom> commatoms = latomss[0].HClone();
                SelectCommonAtoms(ref commatoms, ref latomss[i]);
                HDebug.Assert(CheckCommonAtoms(commatoms, latomss[0]));
            }
            );
            HDebug.Assert(CheckCommonAtoms(latomss));
            atomss = latomss;
        }
        public static void SelectCommonAtoms(Pdb pdb1, string ChainIDs1, out List<Atom> atoms1
                                            ,Pdb pdb2, string ChainIDs2, out List<Atom> atoms2
                                            )
        {
            SelectCommonAtoms( pdb1.atoms, ChainIDs1, out atoms1
                             , pdb2.atoms, ChainIDs2, out atoms2
                             );
        }
        public static void SelectCommonAtoms<Atom>(IList<Atom> pdb1_atoms, string ChainIDs1, out List<Atom> atoms1
                                                  ,IList<Atom> pdb2_atoms, string ChainIDs2, out List<Atom> atoms2
                                                  )
            where Atom : IAtom
        {
            HDebug.Assert(ChainIDs1.Length == ChainIDs2.Length);

            atoms1 = new List<Atom>();
            atoms2 = new List<Atom>();

            for(int i=0; i<ChainIDs1.Length; i++)
            {
                char ChainID1 = ChainIDs1[i];
                char ChainID2 = ChainIDs2[i];
                List<Atom> latoms1 = new List<Atom>(pdb1_atoms.SelectByChainID(ChainID1).SelectByAltLoc());
                List<Atom> latoms2 = new List<Atom>(pdb2_atoms.SelectByChainID(ChainID2).SelectByAltLoc());
                PdbStatic.SelectCommonAtoms(ref latoms1, ref latoms2);
                atoms1.AddRange(latoms1);
                atoms2.AddRange(latoms2);
            }
        }
        public static void SelectCommonAtoms<Atom>(ref List<Atom> atoms1, ref List<Atom> atoms2)
            where Atom : IAtom
        {
            List<Atom> _atoms1 = new List<Atom>(atoms1);
            List<Atom> _atoms2 = new List<Atom>(atoms2);

            _atoms1 = _atoms1.ListCommon(_atoms2);
            HPack<List<int>> idxAtom2ToAtom1 = new HPack<List<int>>();
            //{
            //    List<Atom> cas1 = _atoms1.SelectByName("CA");
            //    List<Atom> cas2 = _atoms2.SelectByName("CA");
            //    List<int> idx1 = new List<int>();
            //    List<int> idx2 = new List<int>();
            //    LCS.LongestCommonSubsequence( cas1 , cas2 , idx1, idx2, Atom.CompareNameResName);
            //    cas1 = cas1.SelectByIndex(idx1);
            //    cas2 = cas2.SelectByIndex(idx2);
            //}
            _atoms2 = _atoms2.ListCommon(_atoms1, idxAtom2ToAtom1);
            _atoms1 = _atoms1.HSelectByIndex(idxAtom2ToAtom1.value).ToList();

            if(HDebug.IsDebuggerAttached)
            {
                HDebug.AssertAllEquals(_atoms1.Count, _atoms2.Count);
                for(int i=0; i<_atoms1.Count; i++)
                {
                    HDebug.AssertAllEquals(_atoms1[i].name.Trim()    ,_atoms2[i].name.Trim()   );
                    HDebug.AssertAllEquals(_atoms1[i].resName.Trim() ,_atoms1[i].resName.Trim());
                    HDebug.AssertAllEquals(_atoms1[i].resSeq  ,_atoms1[i].resSeq );
                }
            }

            atoms1 = _atoms1;
            atoms2 = _atoms2;
        }
        public static void SelectCommonAtoms<Atom>(ref List<Atom> atoms1, ref List<Atom> atoms2, ref List<Atom> atoms3)
            where Atom : IAtom
        {
            List<Atom> atoms4 = new List<Atom>(atoms1);
            List<Atom> atoms5 = new List<Atom>(atoms1);
            SelectCommonAtoms(ref atoms1, ref atoms2, ref atoms3, ref atoms4, ref atoms5);
        }
        public static void SelectCommonAtoms<Atom>(ref List<Atom> atoms1, ref List<Atom> atoms2, ref List<Atom> atoms3, ref List<Atom> atoms4)
            where Atom : IAtom
        {
            List<Atom> atoms5 = new List<Atom>(atoms1);
            SelectCommonAtoms(ref atoms1, ref atoms2, ref atoms3, ref atoms4, ref atoms5);
        }
        public static void SelectCommonAtoms<Atom>(ref List<Atom> atoms1, ref List<Atom> atoms2, ref List<Atom> atoms3, ref List<Atom> atoms4, ref List<Atom> atoms5)
            where Atom : IAtom
        {
            List<Atom> _atoms1 = new List<Atom>(atoms1);
            List<Atom> _atoms2 = new List<Atom>(atoms2);
            List<Atom> _atoms3 = new List<Atom>(atoms3);
            List<Atom> _atoms4 = new List<Atom>(atoms4);
            List<Atom> _atoms5 = new List<Atom>(atoms5);

            _atoms1 = _atoms1.ListCommon(_atoms2);
            _atoms1 = _atoms1.ListCommon(_atoms3);
            _atoms1 = _atoms1.ListCommon(_atoms4);
            _atoms1 = _atoms1.ListCommon(_atoms5);

            HPack<List<int>> idxAtom2ToAtom1 = new HPack<List<int>>();
            _atoms2 = _atoms2.ListCommon(_atoms1, idxAtom2ToAtom1);
            _atoms2 = _atoms2.HSelectByIndex(idxAtom2ToAtom1.value).ToList();

            HPack<List<int>> idxAtom3ToAtom1 = new HPack<List<int>>();
            _atoms3 = _atoms3.ListCommon(_atoms1, idxAtom3ToAtom1);
            _atoms3 = _atoms3.HSelectByIndex(idxAtom3ToAtom1.value).ToList();

            HPack<List<int>> idxAtom4ToAtom1 = new HPack<List<int>>();
            _atoms4 = _atoms4.ListCommon(_atoms1, idxAtom4ToAtom1);
            _atoms4 = _atoms4.HSelectByIndex(idxAtom4ToAtom1.value).ToList();

            HPack<List<int>> idxAtom5ToAtom1 = new HPack<List<int>>();
            _atoms5 = _atoms5.ListCommon(_atoms1, idxAtom5ToAtom1);
            _atoms5 = _atoms5.HSelectByIndex(idxAtom5ToAtom1.value).ToList();

            if(HDebug.IsDebuggerAttached)
            {
                HDebug.AssertAllEquals(_atoms1.Count, _atoms2.Count, _atoms3.Count, _atoms4.Count, _atoms5.Count);
                for(int i=0; i<_atoms1.Count; i++)
                {
                    HDebug.AssertAllEquals(_atoms1[i].name    ,_atoms2[i].name    ,_atoms3[i].name    ,_atoms4[i].name    ,_atoms5[i].name   );
                    HDebug.AssertAllEquals(_atoms1[i].resName ,_atoms1[i].resName ,_atoms3[i].resName ,_atoms4[i].resName ,_atoms5[i].resName);
                    HDebug.AssertAllEquals(_atoms1[i].resSeq  ,_atoms1[i].resSeq  ,_atoms3[i].resSeq  ,_atoms4[i].resSeq  ,_atoms5[i].resSeq );
                }
            }

            atoms1 = _atoms1;
            atoms2 = _atoms2;
            atoms3 = _atoms3;
            atoms4 = _atoms4;
            atoms5 = _atoms5;
        }

        //public static List<Tuple<int,int>> ListCommonSerials(IList<Atom> atoms1, IList<Atom> atoms2)
        //{
        //    List<Tuple<int,int>> list = new List<Tuple<int, int>>();
        //    List<Atom> __atoms2 = new List<Atom>(atoms2);
        //    foreach(Atom atom1 in atoms1)
        //    {
        //        int idx = __atoms2.IndexOfAtom(atom1.name, atom1.resSeq);
        //        if(idx == -1)
        //            continue;
        //        Atom atom2 = __atoms2[idx];
        //        __atoms2.RemoveAt(idx);
        //        list.Add(new Tuple<int, int>(atom1.serial, atom2.serial));
        //    }
        //    return list;
        //}
        //
        //public static List<Tuple<int,int,int>> ListCommonSerials(IList<Atom> atoms1, IList<Atom> atoms2, IList<Atom> _atoms3)
        //{
        //    List<Tuple<int,int>> serials12 = ListCommonSerials(atoms1, atoms2);
        //    List<Tuple<int,int>> serials23 = ListCommonSerials(atoms2, _atoms3);
        //    List<int> idx12, idx23;
        //    {
        //        int[] serials2a = serials12.Items2();
        //        int[] serials2b = serials23.Items1();
        //        idx12 = serials2a.IndexCommon(serials2b);
        //        idx23 = serials2b.IndexCommon(serials2a);
        //        Debug.Assert(idx12.Count == idx23.Count);
        //    }
        //    List<Tuple<int,int,int>> serials123 = new List<Tuple<int,int,int>>();
        //    for(int i=0; i<idx12.Count; i++)
        //    {
        //        int serial1  = serials12[idx12[i]].Item1;
        //        int serial2  = serials12[idx12[i]].Item2;
        //        int serial2a = serials23[idx23[i]].Item1;
        //        int serial3  = serials23[idx23[i]].Item2;
        //        Debug.Assert(serial2 == serial2a);
        //        serials123.Add(new Tuple<int, int, int>(serial1, serial2, serial3));
        //    }
        //    return serials123;
        //}

    }
}
