using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public static partial class HStaticBioinfo
    {
        public static string[] HListMutation(this IList<HSequence.HMutation> mutations)
        {
            List<string> list = new List<string>();
            foreach(var mut in mutations)
                list.Add(mut.mutation);
            return list.ToArray();
        }
    }
    public partial class HSequence
    {
        public class HMutation
        {
            public static HMutation[] FromPdb(string wtpdbid, IList<string> pdbids, Func<string,Pdb> FnLoadPdb)
            {
                HMutation[] mutations = new HMutation[pdbids.Count];
                for(int i=0; i<pdbids.Count; i++)
                {
                    Pdb  wtpdb = FnLoadPdb(wtpdbid);
                    Pdb mutpdb = FnLoadPdb(pdbids[i]);
                    mutations[i] = FromPdb
                        ( wtpdb, mutpdb
                        , wt_pdbid: wtpdbid
                        , mut_pdbid: pdbids[i]
                        );
                }
                return mutations;
            }
            public static HMutation FromPdb
                ( Pdb wtpdb, Pdb mutpdb
                , string wt_pdbid=null, string mut_pdbid=null
                , int?  wt_pdb_model=null, char?  wt_pdb_altLoc=null, char?  wt_pdb_chainID=null, char?  wt_pdb_iCode=null
                , int? mut_pdb_model=null, char? mut_pdb_altLoc=null, char? mut_pdb_chainID=null, char? mut_pdb_iCode=null
                )
            {
                if(( wtpdb.models.Length >= 2) &&  wt_pdb_model==null) {  wt_pdb_model = 0; } // HDebug.Assert(false); 
                if((mutpdb.models.Length >= 2) && mut_pdb_model==null) { mut_pdb_model = 0; } // HDebug.Assert(false); 

                Pdb.Atom[]            wt_atoms =  wtpdb.SelectAtoms(imodel: wt_pdb_model, altLoc: wt_pdb_altLoc, chainID: wt_pdb_chainID, iCode: wt_pdb_iCode);
                Pdb.Atom[]           mut_atoms = mutpdb.SelectAtoms(imodel: mut_pdb_model, altLoc: mut_pdb_altLoc, chainID: mut_pdb_chainID, iCode: mut_pdb_iCode);
                Pdb.Residue[]         wt_resis =  wt_atoms.ListResidue();
                Pdb.Residue[]        mut_resis = mut_atoms.ListResidue();
                Tuple<string, int>[]  wt_resn_resi =  wt_resis.HListResNameSeq();
                Tuple<string, int>[] mut_resn_resi = mut_resis.HListResNameSeq();

                Tuple<string, string, int, string>[] mutationinfos =
                    FromResnResi
                    ( wt_resn_resi
                    , mut_resn_resi
                    , rngLcsLen: new Tuple<int, int>((int)(wt_resis.Length*0.8), (int)(wt_resis.Length*1.2))
                    );

                return new HMutation
                {
                    wt_pdb         = wtpdb                  , mut_pdb         = mutpdb,
                    wt_pdbid       = wt_pdbid               , mut_pdbid       = mut_pdbid,
                    wt_ResSeq      = wt_resis.HListResSeq() , mut_ResSeq      = mut_resis.HListResSeq(),
                    wt_ResName     = wt_resis.HListResName(), mut_ResName     = mut_resis.HListResName(),
                    wt_pdb_model   = wt_pdb_model           , mut_pdb_model   = mut_pdb_model,
                    wt_pdb_altLoc  = wt_pdb_altLoc          , mut_pdb_altLoc  = mut_pdb_altLoc,
                    wt_pdb_chainID = wt_pdb_chainID         , mut_pdb_chainID = mut_pdb_chainID,
                    wt_pdb_iCode   = wt_pdb_iCode           , mut_pdb_iCode   = mut_pdb_iCode,
                    mutationinfos  = mutationinfos
                };
            }
            public static Tuple<string, string, int, string>[] FromResnResi  // (operation, wt_resn, wt_resi, mut_resn)[]
                ( Tuple<string, int>[]  wt_resn_resi
                , Tuple<string, int>[] mut_resn_resi
                , Tuple<int, int> rngLcsLen
                )
            {
                var lcs = HSequence.LongCommSubseq.GetLongCommSubseq(wt_resn_resi, mut_resn_resi);
                if(rngLcsLen != null)
                    if((lcs.lcs_resn.Length < rngLcsLen.Item1) || (rngLcsLen.Item2 < lcs.lcs_resn.Length))
                        return null;

                List<Tuple<string, string, int, string>> mutationinfos = new List<Tuple<string, string, int, string>>();
                                                                    // (operation, wt_resn, wt_resi, mut_resn)
                //var aminoacids = HBioinfo.AminoAcids.ToDictionaryBy3Letter(true);
                //string[] mutate = new string[0];
                for(int i=0; i<lcs.lcs_oper1to2.Length; i++)
                {
                    if(lcs.lcs_oper1to2[i].Item1 == LCS.Oper.Match)
                        continue;

                    /// 1. collecting deletes and inserts until next match
                    List<int> idxs_del = new List<int>();
                    List<int> idxs_ins = new List<int>();
                    int last_j = i;
                    for(int j=i; j<lcs.lcs_oper1to2.Length; j++)
                    {
                        if(lcs.lcs_oper1to2[j].Item1 == LCS.Oper.Match) break;
                        if(lcs.lcs_oper1to2[j].Item1 == LCS.Oper.Delete) idxs_del.Add(j);
                        if(lcs.lcs_oper1to2[j].Item1 == LCS.Oper.Insert) idxs_ins.Add(j);
                        last_j = j;
                    }
                    /// 2. determine matching del-ins
                    List<Tuple<int, int>> idxs_del_ins = new List<Tuple<int, int>>();
                    while((idxs_del.Count != 0) && (idxs_ins.Count != 0))
                    {
                        int idx_del = (i==0) ? idxs_del.Last() : idxs_del.First();
                        int idx_ins =                            idxs_ins.First();
                        idxs_del_ins.Add(new Tuple<int, int>(idx_del, idx_ins));
                        HDebug.Verify(idxs_del.Remove(idx_del));
                        HDebug.Verify(idxs_ins.Remove(idx_ins));
                    }
                    /// 3. handle mutation informations
                    while((idxs_del.Count != 0) || (idxs_ins.Count != 0) || (idxs_del_ins.Count != 0))
                    {
                        Tuple<int,int> idx_del_ins = null;
                        int idx_del = int.MaxValue; if(idxs_del    .Count > 0) idx_del = idxs_del    .First();
                        int idx_ins = int.MaxValue; if(idxs_ins    .Count > 0) idx_ins = idxs_ins    .First();
                        int idx_rep = int.MaxValue; if(idxs_del_ins.Count > 0) { idx_del_ins = idxs_del_ins.First(); idx_rep = idx_del_ins.HToArray().Min(); }
                        int min_idx = HMath.HMin(idx_del, idx_ins, idx_rep);
                        HDebug.Assert(min_idx != int.MaxValue);

                        if(idx_del == min_idx)
                        {
                            string resn1 = lcs.lcs_oper1to2[idx_del].Item2;
                            int    resi1 = lcs.lcs_oper1to2[idx_del].Item3.Value;
                            //string mutatei = string.Format("delete {0}{1}", aminoacids[resn].name1, resi);
                            //mutate = mutate.HAdd(mutatei);
                            mutationinfos.Add(new Tuple<string, string, int, string>("delete", resn1, resi1, null));
                            HDebug.Verify(idxs_del.Remove(idx_del));
                            continue;
                        }
                        if(idx_ins == min_idx)
                        {
                            string resn_insertafter = (i==0) ? "   "        : lcs.lcs_oper1to2[i-1].Item2;
                            int    resi_insertafter = (i==0) ? int.MinValue : lcs.lcs_oper1to2[i-1].Item3.Value;
                            string resn2 = lcs.lcs_oper1to2[idx_ins].Item2;
                            HDebug.Assert (lcs.lcs_oper1to2[idx_ins].Item3 == null);
                            mutationinfos.Add(new Tuple<string, string, int, string>("insert", resn_insertafter, resi_insertafter, resn2));
                            HDebug.Verify(idxs_ins.Remove(idx_ins));
                            continue;
                        }
                        if(idx_rep == min_idx)
                        {
                            int    idel  = idx_del_ins.Item1;
                            int    iins  = idx_del_ins.Item2;
                            string resn1 = lcs.lcs_oper1to2[idel].Item2;
                            int    resi1 = lcs.lcs_oper1to2[idel].Item3.Value;
                            string resn2 = lcs.lcs_oper1to2[iins].Item2;
                            HDebug.Assert( lcs.lcs_oper1to2[iins].Item3 == null);
                            //string mutatei = string.Format("{0}{1}{2}", aminoacids[resn1].name1, resi1, aminoacids[resn2].name1);
                            //mutate = mutate.HAdd(mutatei);
                            mutationinfos.Add(new Tuple<string, string, int, string>("replace", resn1, resi1, resn2));
                            HDebug.Verify(idxs_del_ins.Remove(idx_del_ins));
                            continue;
                        }
                    }
                    i = last_j;
                    continue;
                }

                return mutationinfos.ToArray();
            }

            public Pdb      wt_pdb;
            public string   wt_pdbid;
            public int[]    wt_ResSeq;
            public string[] wt_ResName;
            public int?     wt_pdb_model;
            public char?    wt_pdb_altLoc;
            public char?    wt_pdb_chainID;
            public char?    wt_pdb_iCode;

            public Pdb      mut_pdb;
            public string   mut_pdbid;
            public int[]    mut_ResSeq;
            public string[] mut_ResName;
            public int?     mut_pdb_model;
            public char?    mut_pdb_altLoc;
            public char?    mut_pdb_chainID;
            public char?    mut_pdb_iCode;

            public Tuple<string, string, int, string>[] mutationinfos; // (operation, wt_resn, wt_resi, mut_resn)

            public int num_mutation
            {
                get
                {
                    return mutationinfos.Length;
                }
            }
            public int NumMutation(bool only_replace)
            {
                return mutationinfos.HListItem1().HCountEqual("replace");
            }
            public string[] mutations
            {
                get
                {
                    //var aminoacids = HBioinfo.AminoAcids.ToDictionaryBy3Letter(true);
                    Func<string, HBioinfo.AminoAcid> aminoacids = delegate(string resn)
                    {
                        var aa = HBioinfo.AminoAcid.From3Letter(resn);
                        HDebug.Exception(aa.Count == 1);
                        return aa[0];
                    };

                    List<string> mutations = new List<string>();
                    foreach(var mutationinfo in mutationinfos)
                    {
                        string operation = mutationinfo.Item1;
                        string wt_resn   = mutationinfo.Item2;
                        int    wt_resi   = mutationinfo.Item3;
                        string mut_resn  = mutationinfo.Item4;

                        string mutation;
                        switch(operation)
                        {
                            case "replace": mutation = string.Format("{0}{1}{2}" , aminoacids(wt_resn).name1, wt_resi, aminoacids(mut_resn).name1); break;
                            case "delete" : mutation = string.Format("del {0}{1}", aminoacids(wt_resn).name1, wt_resi                            ); break;
                            case "insert" :
                                {
                                    string resn_after = wt_resn; if(aminoacids(resn_after) != null) resn_after = aminoacids(resn_after).name1.ToString();
                                    int    resi_after = wt_resi;
                                    mutation         = string.Format("ins {0} after {1}{2}"   ,                  aminoacids(mut_resn).name1, resn_after, resi_after);
                                }
                                break;
                            default: throw new NotImplementedException();
                        }
                        mutations.Add(mutation);
                    }
                    return mutations.ToArray();
                }
            }
            public static string GetMutationType(string wt_resn, int wt_resi, string mut_resn)
            {
                HDebug.Assert(GetMutationType("HIS", 64, "TRP") == "H64W"); // selftest

                //var aminoacids = HBioinfo.AminoAcids.ToDictionaryBy3Letter(true);
                Func<string, HBioinfo.AminoAcid> aminoacids = delegate(string resn)
                {
                        var aa = HBioinfo.AminoAcid.From3Letter(resn);
                        HDebug.Exception(aa.Count == 1);
                        return aa[0];
                };

                string muttype = string.Format("{0}{1}{2}", aminoacids(wt_resn).name1, wt_resi, aminoacids(mut_resn).name1);
                return muttype;
            }
            static bool DecomposeMutationType_selftest = HDebug.IsDebuggerAttached;
            public static Tuple<string, char, int, string, char> DecomposeMutationType(string muttype)
            {
                if(DecomposeMutationType_selftest)
                {
                    DecomposeMutationType_selftest = false;
                    var tdec = DecomposeMutationType("H64W");
                    HDebug.Exception(tdec.Item1.ToUpper() == "HIS");
                    HDebug.Exception(tdec.Item2           == 'H');
                    HDebug.Exception(tdec.Item3           == 64);
                    HDebug.Exception(tdec.Item4.ToUpper() == "TRP");
                    HDebug.Exception(tdec.Item5           == 'W');
                }
                //var aminoacids = HBioinfo.AminoAcids.ToDictionaryBy1Letter();
                Func<char, HBioinfo.AminoAcid> aminoacids = delegate(char resn)
                {
                    return HBioinfo.AminoAcid.From1Letter(resn);
                };

                char    wt_resn1 = muttype.First(); string  wt_resn3 = aminoacids( wt_resn1).name3;
                char   mut_resn1 = muttype.Last (); string mut_resn3 = aminoacids(mut_resn1).name3;
                string lwt_resi  = muttype.Substring(1, muttype.Length-2);
                int     wt_resi  = int.Parse(lwt_resi);

                return new Tuple<string,char,int,string,char>
                (  wt_resn3, wt_resn1
                ,  wt_resi
                , mut_resn3, mut_resn1
                );
            }
            public string mutation
            {
                get
                {
                    return mutations.HToString(",");
                }
            }
        }
    }
}
