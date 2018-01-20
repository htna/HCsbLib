using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using ResInfo = PdbStatic.ResInfo;
	public partial class Hess
    {
        public class MixHessInfo : HessInfo
        {
            public HessInfo intmHessinfoAllMidBkbn = null;  // intermediate hessinfo for (all-middle-backbone)
            public object[] intmAtomCa   = null;            // intermediate Ca atoms for all-middle-backbone parts
            public object[] intmAtomAll  = null;            // intermediate atoms    for the selected all-atomic part
            public object[] intmAtomMid  = null;            // intermediate atoms    for the middle   all-atomic part
            public object[] intmAtomBkbn = null;            // intermediate atoms    for the mid-resolution backbone part
        }
        public static MixHessInfo GetHessMixEAnmDefault(Universe univ, IList<Vector> coords, ILinAlg la
                                                       , IList<ResInfo> lstResAllAtom, out string errmsg
                                                       , bool bGetIntmInfo
                                                       )
        {
            ///                                                 |  (    all,     ca,   mixd,   site)  |                  |  (    all,     ca,   mixd,   site)
            /// =============================================================================================================================================
            /// Nma-MT000000Anm                                 |  (    NaN, 0.6755, 0.6799, 0.5250)  |                  |                                   
            /// Nma-TCstCstAnmMix04                             |  (    NaN, 0.4992, 0.2807, 0.6534)  |                  |                                   
            /// Nma-TCstCstAnmMix13                             |  (    NaN, 0.6126, 0.6031, 0.5708)  |                  |                                   
            /// Nma-TCstCstAnmMix0413                           |  (    NaN, 0.7010, 0.7010, 0.7505)  |                  |                                   
            /// Nma-TCstCstVdwMix                               |  (    NaN, 0.4754, 0.2479, 0.6612)  |                  |                                   
            /// Nma-TFrcFrcFrcMix                               |  (    NaN, 0.4851, 0.2692, 0.6819)  |                  |                                   
            /// Nma-TCstCstVdw                                  |  ( 0.8664, 0.9107, 0.8832, 0.8689)  |                  |                                   
            /// Nma-TCstCstAnm                                  |  ( 0.7949, 0.8695, 0.8298, 0.7669)  |                  |                                   
            /// Nma-T000000Anm                                  |  ( 0.4127, 0.8141, 0.7096, 0.6308)  |                  |                                   
            /// Nma-CTStem                                      |  (    NaN, 0.7520,    NaN,    NaN)  |                  |                                   
            /// Nma-CTAnm                                       |  (    NaN, 0.7423,    NaN,    NaN)  |                  |                                   
            /// ---------------------------------------------------------------------------------------------------------------------------------------------
            /// Nma-TCstCstAnmMix-04010-NCaC13008-07009         |  (    NaN, 0.7649, 0.7857, 0.7665)  |  TCstCstAnm-...  |  (    NaN, 0.8753, 0.8969, 0.9616)
            /// Nma-TCstCstAnmMix-04010-NCaC13008-10009         |  (    NaN, 0.7363, 0.7636, 0.7335)  |  TCstCstAnm-...  |  (    NaN, 0.8443, 0.8778, 0.9422)
            /// ---------------------------------------------------------------------------------------------------------------------------------------------
            /// Nma-TCstCstAnmMix-04100-NCaC13070-07080         |  (    NaN, 0.7700, 0.7892, 0.7580)  |  TCstCstAnm-...  |  (    NaN, 0.8852, 0.9453, 0.9915)
            /// Nma-TCstCstAnmMix-04100-NCaC13070-10080         |  (    NaN, 0.7436, 0.7644, 0.7201)  |  TCstCstAnm-...  |  (    NaN, 0.8551, 0.9223, 0.9617)
            /// ---------------------------------------------------------------------------------------------------------------------------------------------
            ///<Nma-TCstCstAnmMix-04100-NCaC13070-07085 ******  |  (    NaN, 0.7704, 0.7893, 0.7575)  |  TCstCstAnm-...  |  (    NaN, 0.8853, 0.9456, 0.9911)>
            /// Nma-TCstCstAnmMix-04100-NCaC13070-10085         |  (    NaN, 0.7409, 0.7624, 0.7180)  |  TCstCstAnm-...  |  (    NaN, 0.8518, 0.9199, 0.9597)
            /// ---------------------------------------------------------------------------------------------------------------------------------------------
            /// Nma-TCstCstAnmMix-04100-NCaC13070-07090         |  (    NaN, 0.7707, 0.7894, 0.7570)  |  TCstCstAnm-...  |  (    NaN, 0.8853, 0.9459, 0.9908)
            /// Nma-TCstCstAnmMix-04100-NCaC13070-10090         |  (    NaN, 0.7383, 0.7604, 0.7160)  |  TCstCstAnm-...  |  (    NaN, 0.8486, 0.9176, 0.9577)

            double anmCutoffAll    =  4.5; double anmSprcstAll    = 1.00;   ///     04100 =>        04.5, 1.00
            double anmCutoffCoarse = 13.0; double anmSprcstCoarse = 0.70;   /// NCaC13070 => "NCaC" 13.0, 0.70
            double anmCutoffMix    =  7.0; double anmSprcstMix    = 0.85;   ///     07085 =>        07.0, 0.85
            bool bAllowAllToCoarse       = true;
            bool bOnlyCoarseConnBackbone = false;
            string strBkbnReso           = "NCaC";

            var anmSprcstCutoffAll       = new Tuple<double, double>(anmCutoffAll   , anmSprcstAll   ); ///     04100
            var anmSprcstCutoffCoarse    = new Tuple<double, double>(anmCutoffCoarse, anmSprcstCoarse); /// NCaC13070
            var anmSprcstCutoffMix       = new Tuple<double, double>(anmCutoffMix   , anmSprcstMix   ); ///     07085
            MixHessInfo hessinfo = GetHessMixEAnm(univ, coords, la, lstResAllAtom, bAllowAllToCoarse, bOnlyCoarseConnBackbone
                                                , anmSprcstCutoffAll, anmSprcstCutoffMix, anmSprcstCutoffCoarse
                                                , out errmsg, bGetIntmInfo, strBkbnReso);
            return hessinfo;
        }
        public static MixHessInfo GetHessMixEAnm(Universe univ, IList<Vector> coords, ILinAlg la
                                                , IList<ResInfo> lstResAllAtom, double anmCutoff, out string errmsg
                                                , bool bGetIntmInfo, string strBkbnReso
                                                )
        {
            HDebug.Assert(anmCutoff >= 0);
            var anmSprcstCutoffAll       = new Tuple<double, double>(1, anmCutoff);
            var anmSprcstCutoffCoarse    = new Tuple<double, double>(1, anmCutoff);
            var anmSprcstCutoffMix       = new Tuple<double, double>(1, anmCutoff);
            bool bAllowAllToCoarse       = true;
            bool bOnlyCoarseConnBackbone = false;
            MixHessInfo hessinfo = GetHessMixEAnm(univ, coords, la, lstResAllAtom, bAllowAllToCoarse, bOnlyCoarseConnBackbone
                                                , anmSprcstCutoffAll, anmSprcstCutoffMix, anmSprcstCutoffCoarse
                                                , out errmsg, bGetIntmInfo, strBkbnReso);
            return hessinfo;
        }
        public static MixHessInfo GetHessMixEAnm(Universe univ, IList<Vector> coords, ILinAlg la, IList<ResInfo> lstResAllAtom
                                                , bool bAllowAllToCoarse, bool bOnlyCoarseConnBackbone
                                                , Tuple<double, double> anmSprcstCutoffAll      // all-all
                                                , Tuple<double, double> anmSprcstCutoffMix      // all/buffer-coarse
                                                , Tuple<double, double> anmSprcstCutoffCoarse   // coarse-coarse
                                                , out string errmsg
                                                , bool bGetIntmInfo, string strBkbnReso
                                                )
        {
            double anmSprcstAll     = anmSprcstCutoffAll   .Item1;
            double anmSprcstMix     = anmSprcstCutoffMix   .Item1;
            double anmSprcstCoarse  = anmSprcstCutoffCoarse.Item1;
            double anmCutoffAll     = anmSprcstCutoffAll   .Item2;
            double anmCutoffMix     = anmSprcstCutoffMix   .Item2;
            double anmCutoffCoarse  = anmSprcstCutoffCoarse.Item2;
            HDebug.Assert(anmCutoffAll    >= 0);
            HDebug.Assert(anmCutoffCoarse >= 0);
            MixModel.FnGetHess GetHess = delegate(Universe luniv, IList<Vector> lcoords, int[] idxAll, int[] idxBuffer, int[] idxCoarse, int[] idxBackbone)
            {
                int size = coords.Count;

                HashSet<int> setIdxAll    = idxAll   .HToHashSet();
                HashSet<int> setIdxBuffer = idxBuffer.HToHashSet();
                HashSet<int> setIdxCoarse = idxCoarse.HToHashSet();
                HashSet<int> setIdxBackbone=idxBackbone.HToHashSet();

                Matrix anmKij = Matrix.Zeros(size, size);
                for(int c=0; c<size; c++)
                {
                    if(lcoords[c] == null) continue;
                    int typec = 0;
                    if(setIdxAll   .Contains(c)) { HDebug.Assert(typec == 0); typec = 1; }
                    if(setIdxBuffer.Contains(c)) { HDebug.Assert(typec == 0); typec = 2; }
                    if(setIdxCoarse.Contains(c)) { HDebug.Assert(typec == 0); typec = 3; }
                    HDebug.Assert(typec != 0);

                    for(int r=c+1; r<size; r++)
                    {
                        if(lcoords[r] == null) continue;
                        int typer = 0;
                        if(setIdxAll   .Contains(r)) { HDebug.Assert(typer == 0); typer = 1; }
                        if(setIdxBuffer.Contains(r)) { HDebug.Assert(typer == 0); typer = 2; }
                        if(setIdxCoarse.Contains(r)) { HDebug.Assert(typer == 0); typer = 3; }
                        HDebug.Assert(typer != 0);

                        double sprcst;
                        double cutoff;
                        int type0 = Math.Min(typec, typer);
                        int type1 = Math.Max(typec, typer);
                        int type01 = type0*10 + type1;
                        switch(type01)
                        {
                            case 11: sprcst = anmSprcstAll   ; cutoff = anmCutoffAll   ; break;   // All    - All
                            case 12: sprcst = anmSprcstAll   ; cutoff = anmCutoffAll   ; break;   // All    - Buffer
                            case 22: sprcst = anmSprcstAll   ; cutoff = anmCutoffAll   ; break;   // Buffer - Buffer
                            case 33: sprcst = anmSprcstCoarse; cutoff = anmCutoffCoarse; break;   // Coarse - Coarse
                            case 23: sprcst = anmSprcstMix   ; cutoff = anmCutoffMix   ; break;   // Buffer - Coarse
                            case 13:                                                              // All    - Buffer
                                if(bAllowAllToCoarse == false)
                                    continue;
                                sprcst = anmSprcstMix;
                                cutoff = anmCutoffMix;
                                break;
                            default: throw new Exception();
                        }

                        HDebug.Assert(bOnlyCoarseConnBackbone == false); // "true" makes the result bad
                        if(bOnlyCoarseConnBackbone)
                        {
                            bool c_bkbn = setIdxBackbone.Contains(c);
                            bool r_bkbn = setIdxBackbone.Contains(r);
                            if(c_bkbn && r_bkbn)
                            {
                                // special treatment that in-between backbones use only coarse grained onnections.
                                sprcst = anmSprcstCoarse;
                                cutoff = anmCutoffCoarse;
                            }
                            else if((type01 == 11) || (type01 == 12) || (type01 == 22))
                            {
                                // do nothing. use the above setting.
                            }
                            else
                            {
                                // all-to-coarse
                                HDebug.AssertOr(type01 == 13, type01 == 23);
                                if(type01 == 33)
                                {
                                    HDebug.Assert(type01 != 33); // should be treated by (c_bkbn && r_bkbn)
                                    throw new Exception("HDebug.Assert(type01 != 33)");
                                }
                                continue;
                            }
                        }
                        double cutoff2 = cutoff*cutoff;

                        double dist2 = (coords[c] - coords[r]).Dist2;
                        if(dist2 <= cutoff2)
                        {
                            anmKij[c, r] = sprcst;
                            anmKij[r, c] = sprcst;
                        }
                    }
                }

                var hessinfo = Hess.GetHessEAnm(luniv, lcoords, anmKij);
                return hessinfo;
            };
            return MixModel.GetHessMixModel(univ, coords, la, lstResAllAtom, GetHess, out errmsg, bGetIntmInfo, strBkbnReso);
        }
        public static MixHessInfo GetHessMixSbNMA(Universe univ, IList<Vector> coords, ILinAlg la
                                                , IList<ResInfo> lstResAllAtom, double nbondMaxDist, out string errmsg
                                                , bool bGetIntmInfo, string strBkbnReso
                                                )
        {
            // nbondMaxDist = double.PositiveInfinity
            MixModel.FnGetHess GetHess = delegate(Universe luniv, IList<Vector> lcoords, int[] idxAll, int[] idxBuffer, int[] idxCoarse, int[] idxBackbone)
            {
                var hessinfo = Hess.GetHessSbNMA(luniv, lcoords, nbondMaxDist);
                return hessinfo;
            };
            return MixModel.GetHessMixModel(univ, coords, la, lstResAllAtom, GetHess, out errmsg, bGetIntmInfo, strBkbnReso);
        }
        public static MixHessInfo GetHessMixSsNMA(Universe univ, IList<Vector> coords, ILinAlg la
                                                , IList<ResInfo> lstResAllAtom, double nbondMaxDist, out string errmsg
                                                , bool bGetIntmInfo, string strBkbnReso
                                                , double? maxAbsSpring = null
                                                //, bool setNanForEmptyAtom       // = true
                                                )
        {
            // nbondMaxDist = double.PositiveInfinity
            MixModel.FnGetHess GetHess = delegate(Universe luniv, IList<Vector> lcoords, int[] idxAll, int[] idxBuffer, int[] idxCoarse, int[] idxBackbone)
            {
                var hessinfo = Hess.GetHessSsNMA(luniv, lcoords, nbondMaxDist, maxAbsSpring);//, setNanForEmptyAtom);
                return hessinfo;
            };
            return MixModel.GetHessMixModel(univ, coords, la, lstResAllAtom, GetHess, out errmsg, bGetIntmInfo, strBkbnReso);
        }
        public static class MixModel
        {
            public delegate HessInfo FnGetHess(Universe univ, IList<Vector> coords, int[] idxAll, int[] idxBuffer, int[] idxCoarse, int[] idxBackbone);
            public static MixHessInfo GetHessMixModel(Universe univ, IList<Vector> coords, ILinAlg la
                                                    , IList<ResInfo> lstResAllAtom, FnGetHess GetHess, out string errmsg
                                                    , bool bGetIntmInfo, string strBkbnReso
                                                    )
            {
                FnGetHess lGetHess = delegate(Universe luniv, IList<Vector> lcoords, int[] lidxAll, int[] lidxBuffer, int[] lidxCoarse, int[] idxBackbone)
                {
                    // double check if all three indices are disjoint
                    HDebug.Assert(lidxAll   .HUnion().Length == lidxAll   .Length);
                    HDebug.Assert(lidxBuffer.HUnion().Length == lidxBuffer.Length);
                    HDebug.Assert(lidxCoarse.HUnion().Length == lidxCoarse.Length);
                    HDebug.Assert(lidxAll   .HListCommonT(lidxBuffer).Count == 0);
                    HDebug.Assert(lidxBuffer.HListCommonT(lidxCoarse).Count == 0);
                    HDebug.Assert(lidxCoarse.HListCommonT(lidxAll   ).Count == 0);
                    if(HDebug.IsDebuggerAttached)
                    {
                        foreach(int idx in lidxAll   ) HDebug.Assert(lcoords[idx] != null);
                        foreach(int idx in lidxBuffer) HDebug.Assert(lcoords[idx] != null);
                        foreach(int idx in lidxCoarse) HDebug.Assert(lcoords[idx] != null);
                    }

                    var hessinfo = GetHess(luniv, lcoords, lidxAll, lidxBuffer, lidxCoarse, idxBackbone);
                    return hessinfo;
                };

                bool bVerifySteps = false; // HDebug.IsDebuggerAttached;
                Vector bfactorFull = null;
                if(bVerifySteps && HDebug.IsDebuggerAttached)
                {
                    int[] idxAll = HEnum.HEnumCount(coords.Count).ToArray();
                    var hessinfo = lGetHess(univ, coords, idxAll, new int[0], new int[0], new int[0]);
                    var modes    = hessinfo.GetModesMassReduced();
                    var modesPosZero = modes.SeparateTolerants();
                    HDebug.Assert(modesPosZero.Item2.Length == 6);
                    bfactorFull = modesPosZero.Item1.GetBFactor().ToArray();
                }

                IList<Pdb.Atom> pdbatoms = univ.atoms.ListPdbAtoms();
                ResInfo[] resinfos = pdbatoms.ListResInfo(true);

                int[] idxCa  ; // idxs of Ca atoms
                int[] idxBkbn; // idxs of backbone atoms
                int[] idxSele; // idxs of all atoms of lstResAllAtom (selected)
                int[] idxCntk; // idxs of all atoms contacting idxSele
                {
                    string[] nameBkbn;
                    switch(strBkbnReso)
                    {
                        case "NCaC"         : nameBkbn = new string[] { "N", "CA", "C"                        }; break;
                        case "NCaC-O"       : nameBkbn = new string[] { "N", "CA", "C", "O"                   }; break;
                        case "NCaC-OHn"     : nameBkbn = new string[] { "N", "CA", "C", "O", "HN"             }; break;
                        case "NCaC-O-Ha"    : nameBkbn = new string[] { "N", "CA", "C", "O",       "HA"       }; break;
                        case "NCaC-OHn-HaCb": nameBkbn = new string[] { "N", "CA", "C", "O", "HN", "HA", "CB" }; break;
                        default: throw new NotImplementedException();
                    }

                    idxCa   = univ.atoms.ListPdbAtoms().IdxByName(true ,"CA");
                    //idxBkbn = univ.atoms.ListPdbAtoms().IdxByName(true, "N", "CA", "C", "O", "HA");
                    idxBkbn = univ.atoms.ListPdbAtoms().IdxByName(true, nameBkbn);

                               idxSele = resinfos.HIdxEqual(ResInfo.Equals, lstResAllAtom.ToArray());
                    Vector[] coordSele = coords.HSelectByIndex(idxSele);

                    int[]     idxCtof = coords.HIdxWithinCutoff(4.5, coordSele);
                    ResInfo[] resCtof = resinfos.HSelectByIndex(idxCtof);
                              resCtof = resCtof.HUnion();                                       // remove redundant residues
                              resCtof = resCtof.HRemoveAll(lstResAllAtom.ToArray()).ToArray();  // remove active site residues
                              idxCntk = resinfos.HIdxEqual(ResInfo.Equals, resCtof);

                    bool plot = false;
                    if(plot)
                        #region verify by plotting
                    {
                        Pdb.ToFile(@"C:\temp\mix.pdb", pdbatoms, coords);
                        Pymol.Py.WriteBlank (@"C:\temp\mix.py", false);
                        Pymol.Py.WriteSphere(@"C:\temp\mix.py", "Ca", coords.HSelectByIndex(idxCa).HRemoveAllNull(), 0.3);
                        Pymol.Py.WriteSphere(@"C:\temp\mix.py", "backbone", coords.HSelectByIndex(idxBkbn).HRemoveAllNull(), 0.3);
                        Pymol.Py.WriteSphere(@"C:\temp\mix.py", "active", coordSele.HRemoveAllNull()
                                                                        , 0.3
                                                                        , red:1, green:0, blue:0);
                        Pymol.Py.WriteSphere(@"C:\temp\mix.py", "connect", coords.HSelectByIndex(idxCntk).HRemoveAllNull()
                                                                         , 0.3
                                                                         , red:0, green:0, blue:1);
                        HFile.WriteAllLines(@"C:\temp\mix.pml", new string[]
                        {
                            "load mix.pdb",
                            "run mix.py",
                            "reset",
                        });
                    }
                        #endregion
                }

                //idxBkbn = HEnum.HSequence(coords.Count).ToArray();
                int[]       idxFull = idxSele.HUnionWith(idxCntk).HUnionWith(idxBkbn);
                Vector[] coordsFull = coords.HCopyIdx(idxFull, null);
                var    hessinfoFull = lGetHess(univ, coordsFull
                                            , idxAll: idxSele
                                            , idxBuffer: idxCntk.HRemoveAll(idxSele)
                                            , idxCoarse: idxBkbn.HRemoveAll(idxSele).HRemoveAll(idxCntk)
                                            , idxBackbone: idxBkbn
                                            );
                HDebug.Assert(hessinfoFull.hess.IsComputable() == false);
                //{
                //    var modes = hessinfoFull.GetModesMassReduced(false, la);
                //    var modesPosZero = modes.SeparateTolerants();
                //    Vector bf = modesPosZero.Item1.GetBFactor().ToArray();
                //    Vector nma = ext as Vector;
                //    double corr = BFactor.Corr(bf, nma);
                //}

                var hessinfoSub = hessinfoFull.GetSubHessInfo(idxFull);
                HDebug.Assert(hessinfoSub.hess.IsComputable() == true);
                if(HDebug.IsDebuggerAttached)
                {
                    Mode[] lmodes = hessinfoSub.GetModesMassReduced();
                    if(lmodes.SeparateTolerants().Item2.Length != 6)
                    {
                        HDebug.Assert(false);
                        //throw new Exception();
                    }
                }
                if(bVerifySteps && HDebug.IsDebuggerAttached)
                {
                    var modes = hessinfoSub.GetModesMassReduced();
                    var modesPosZero = modes.SeparateTolerants();
                    Vector bf0 = modesPosZero.Item1.GetBFactor().ToArray();
                    Vector bf = new double[coords.Count]; bf.SetValue(double.NaN);
                    for(int i=0; i<idxFull.Length; i++) bf[idxFull[i]] = bf0[i];
                    int[] idxCax = idxCa.HRemoveAll(idxSele).HRemoveAll(idxCntk);
                    double corr     = HBioinfo.BFactor.Corr(bf, bfactorFull, idxFull);
                    double corrCa   = HBioinfo.BFactor.Corr(bf, bfactorFull, idxCa);
                    double corrCax  = HBioinfo.BFactor.Corr(bf, bfactorFull, idxCax);
                    double corrBkbn = HBioinfo.BFactor.Corr(bf, bfactorFull, idxBkbn);
                    double corrSele = HBioinfo.BFactor.Corr(bf, bfactorFull, idxSele);
                    double corrCntk = HBioinfo.BFactor.Corr(bf, bfactorFull, idxCntk);
                }

                int[] idxMix;
                {
                    Universe.Atom[] mixAtoms    = hessinfoSub.atoms.HToType<object, Universe.Atom>();
                    Pdb.Atom[]      mixPdbAtoms = mixAtoms.ListPdbAtoms();
                    ResInfo[]       mixResinfos = mixPdbAtoms.ListResInfo(true);
                    int[]           mixIdxCa    = mixPdbAtoms.IdxByName(true, "CA");
                    int[]           mixIdxSele  = mixResinfos.HIdxEqual(ResInfo.Equals, lstResAllAtom.ToArray());
                    idxMix = mixIdxCa.HUnionWith(mixIdxSele).HSort();
                }

                HessMatrix hessMix = Hess.GetHessCoarseBlkmat(hessinfoSub.hess, idxMix, la);
                if(hessMix.IsComputable() == false)
                {
                    // hessSub (=hessinfoSub.hess) is computable, but hessMix become in-computable.
                    // This happens when 1. hessSub=[A,B;C,D] has more then 6 zero eigenvalues, 
                    //                   2. the D matrix become singular,
                    //                   3. inv(D) is incomputable
                    //                   4. "A - B inv(D) C" becomes incomputable.
                    errmsg = "hess(Sele,Cntk,Bkbn)->hess(Sele,Ca) becomes incomputable";
                    return null;
                }

                MixHessInfo hessinfoMix = new MixHessInfo
                {
                    hess          = hessMix,
                    mass          = hessinfoSub.mass.ToArray().HSelectByIndex(idxMix),
                    atoms         = hessinfoSub.atoms.ToArray().HSelectByIndex(idxMix),
                    coords        = hessinfoSub.coords.ToArray().HSelectByIndex(idxMix),
                    numZeroEigval = 6,
                };
                if(bGetIntmInfo)
                {
                    hessinfoMix.intmHessinfoAllMidBkbn = hessinfoSub;
                    hessinfoMix.intmAtomCa             = univ.atoms.ToArray().HSelectByIndex(idxCa  );
                    hessinfoMix.intmAtomAll            = univ.atoms.ToArray().HSelectByIndex(idxSele);
                    hessinfoMix.intmAtomMid            = univ.atoms.ToArray().HSelectByIndex(idxCntk);
                    hessinfoMix.intmAtomBkbn           = univ.atoms.ToArray().HSelectByIndex(idxBkbn);
                }

                if(HDebug.IsDebuggerAttached)
                {
                    Mode[] lmodes = hessinfoMix.GetModesMassReduced();
                    if(lmodes.SeparateTolerants().Item2.Length != 6)
                        throw new Exception();
                }
                if(bVerifySteps && HDebug.IsDebuggerAttached)
                {
                    var modes = hessinfoMix.GetModesMassReduced();
                    var modesPosZero = modes.SeparateTolerants();
                    Vector bf0 = modesPosZero.Item1.GetBFactor().ToArray();
                    Vector bf = new double[coords.Count]; bf.SetValue(double.NaN);
                    for(int i=0; i<idxMix.Length; i++) bf[idxFull[idxMix[i]]] = bf0[i];
                    int[] idxCax = idxCa.HRemoveAll(idxSele).HRemoveAll(idxCntk);
                    double corr     = HBioinfo.BFactor.Corr(bf, bfactorFull, true);
                    double corrCa   = HBioinfo.BFactor.Corr(bf, bfactorFull, idxCa);
                    double corrCax  = HBioinfo.BFactor.Corr(bf, bfactorFull, idxCax);
                    double corrBkbn = HBioinfo.BFactor.Corr(bf, bfactorFull, idxBkbn);
                    double corrSele = HBioinfo.BFactor.Corr(bf, bfactorFull, idxSele);
                    double corrCntk = HBioinfo.BFactor.Corr(bf, bfactorFull, idxCntk);
                }

                errmsg = null;
                return hessinfoMix;
            }
        }
    }
}
