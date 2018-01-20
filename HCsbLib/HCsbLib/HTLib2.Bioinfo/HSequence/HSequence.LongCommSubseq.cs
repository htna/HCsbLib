using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class HSequence
	{
        public class LongCommSubseq
        {
            public class OResult<T>
            {
                public Tuple<T, int>[] prot1_resn_resi;
                public Tuple<T, int>[] prot2_resn_resi;
                public T  [] lcs_resn ;
                public int[] lcs_resi1;
                public int[] lcs_resi2;
                public int[] lcs_idx1 ;
                public int[] lcs_idx2 ;
                public Tuple<LCS.Oper, T, int?>[] lcs_oper1to2; // (operation, res name, res seq)
                public Tuple<LCS.Oper, T, int?>[] lcs_oper2to1; // (operation, res name, res seq)

                public Tuple<T, int, int>[] lcs_resn_resi1_resi2
                {
                    get
                    {
                        int length = lcs_resn.Length;
                        Tuple<T, int, int>[] lcs_resn_resi1_resi2 = new Tuple<T, int, int>[length];
                        for(int i=0; i<length; i++)
                            lcs_resn_resi1_resi2[i] = new Tuple<T, int, int>(lcs_resn[i], lcs_resi1[i], lcs_resi2[i]);
                        return lcs_resn_resi1_resi2;
                    }
                }
            }
            public static bool Equals<T>(Tuple<T, int> resn_resi1, Tuple<T, int> resn_resi2)
            {
                dynamic resn1 = resn_resi1.Item1;
                dynamic resn2 = resn_resi2.Item1;
                bool    equal = (resn1 == resn2);
                return  equal;
            }
            public static OResult<string> GetLongCommSubseq(Pdb.Atom[] prot1, Pdb.Atom[] prot2)
            {
                Tuple<string, int>[] prot1_resn_resi = prot1.ListResidue().HListResNameSeq();
                Tuple<string, int>[] prot2_resn_resi = prot2.ListResidue().HListResNameSeq();
                HashSet<string> HIS = (new string[] {"HSD", "HSE", "HSP"}).HToHashSet();
                for(int i=0; i<prot1_resn_resi.Length; i++) if(HIS.Contains(prot1_resn_resi[i].Item1)) prot1_resn_resi[i] = new Tuple<string, int>("HIS", prot1_resn_resi[i].Item2);
                for(int i=0; i<prot2_resn_resi.Length; i++) if(HIS.Contains(prot2_resn_resi[i].Item1)) prot2_resn_resi[i] = new Tuple<string, int>("HIS", prot2_resn_resi[i].Item2);
                var lcs = Bioinfo.HSequence.LongCommSubseq.GetLongCommSubseq(prot1_resn_resi, prot2_resn_resi);
                return lcs;
            }
            public static OResult<T> GetLongCommSubseq<T>(Tuple<T, int>[] prot1_resn_resi, Tuple<T, int>[] prot2_resn_resi)
            {
                var lcs = LCS.LongestCommonSubsequence(prot1_resn_resi, prot2_resn_resi, Equals);
                
                T  [] lcs_resn  = new T  [lcs.Length];
                int[] lcs_resi1 = new int[lcs.Length];
                int[] lcs_resi2 = new int[lcs.Length];
                int[] lcs_idx1  = new int[lcs.Length];
                int[] lcs_idx2  = new int[lcs.Length];
                for(int i=0; i<lcs.Length; i++)
                {
                    T   resn  = lcs.lcs[i].Item1;
                    int idx1  = lcs.lcsidx1[i];
                    int idx2  = lcs.lcsidx2[i];
                    int resi1 = prot1_resn_resi[idx1].Item2;
                    int resi2 = prot2_resn_resi[idx2].Item2;
                    if(HDebug.IsDebuggerAttached)
                    {
                        bool check1 = (dynamic)resn == prot1_resn_resi[idx1].Item1;
                        bool check2 = (dynamic)resn == prot2_resn_resi[idx2].Item1;
                        HDebug.Assert(check1 && check2);
                    }
                
                    lcs_resn [i] = resn ;
                    lcs_resi1[i] = resi1;
                    lcs_resi2[i] = resi2;
                    lcs_idx1 [i] = idx1 ;
                    lcs_idx2 [i] = idx2 ;
                }
                Tuple<LCS.Oper, T, int?>[] lcs_oper1to2 = new Tuple<LCS.Oper, T, int?>[lcs.oper1to2.Length];
                for(int i=0; i<lcs.oper1to2.Length; i++)
                {
                    LCS.Oper oper = lcs.oper1to2[i].Item1;
                    T        resn = lcs.oper1to2[i].Item2.Item1;
                    int?     resi = lcs.oper1to2[i].Item2.Item2;
                    if(oper == LCS.Oper.Insert) resi = null;
                    lcs_oper1to2[i] = new Tuple<LCS.Oper, T, int?>(oper, resn, resi);
                }
                Tuple<LCS.Oper, T, int?>[] lcs_oper2to1 = new Tuple<LCS.Oper, T, int?>[lcs.oper2to1.Length];
                for(int i=0; i<lcs.oper2to1.Length; i++)
                {
                    LCS.Oper oper = lcs.oper2to1[i].Item1;
                    T        resn = lcs.oper2to1[i].Item2.Item1;
                    int?     resi = lcs.oper2to1[i].Item2.Item2;
                    if(oper == LCS.Oper.Insert) resi = null;
                    lcs_oper2to1[i] = new Tuple<LCS.Oper, T, int?>(oper, resn, resi);
                }
                
                return new OResult<T>
                {
                    prot1_resn_resi = prot1_resn_resi,
                    prot2_resn_resi = prot2_resn_resi,
                    lcs_resn  = lcs_resn ,
                    lcs_resi1 = lcs_resi1,
                    lcs_resi2 = lcs_resi2,
                    lcs_idx1  = lcs_idx1 ,
                    lcs_idx2  = lcs_idx2 ,
                    lcs_oper1to2 = lcs_oper1to2,
                    lcs_oper2to1 = lcs_oper2to1,
                };
            }
        }
    }
}
