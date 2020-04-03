/*
#pragma warning disable CS0414

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;
//using HTLib2Private.Bioinfo;

namespace SolvEffect
{
    public partial class Program_htna_20200315
    {
        public static class CGetHessInfoExsolvIsoImsolv
        {
            public static HessInfoExsolvIsoImsolv GetHessInfoExsolvIsoImsolv
                ( Config config
                , bool doAnalysis
                , string[] coarse_options
                , bool compSolvGaps
                , bool lock_computation = true
                )
            {
                Tinker.Prm prm = Tinker.Prm.FromFile(config.prmpath);

                string[] keys_cutoff12        = HFile.ReadAllLines(config.keypath_cutoff12       );
                string[] keys_cutoff12_imsolv = HFile.ReadAllLines(config.keypath_cutoff12_imsolv);

                if(lock_computation)
                {
                    var filelock = HFile.LockFile(config.cachebase+"lock");
                    System.Console.Write(config.cachebase + " : ");
                    if(filelock == null)
                    {
                        System.Console.WriteLine("locked. skip.");
                        return null;
                    }
                    System.Console.WriteLine("working...");
                    filelocks.Add(filelock);
                }

                Tinker.Xyz prot_solv;
                string xyzpath = config.cachebase + "prot_solv.xyz";
                HDebug.Assert(HFile.Exists(xyzpath));
                prot_solv = Tinker.Xyz.FromFile(xyzpath, false, config.prot_xyz_format);

                if(HDebug.True)
                {
                    Vector[] coords = prot_solv.atoms.HListCoords();
                    Vector   center = coords.Mean();
                    double maxdist = 0;
                    foreach(var coord in coords)
                        maxdist = Math.Max(maxdist, (center - coord).Dist);
                }

                if(HFile.Exists(xyzpath.Replace(".xyz", "_pymol.xyz")) == false)
                {
                    prot_solv.ToFile(xyzpath.Replace(".xyz", "_pymol.xyz"), false, Tinker.Xyz.Atom.Format.defformat_digit06, true);
                }

                Tinker.Xyz prot_iso;
                string xyzprotpath = config.cachebase + "prot_solv_prot.xyz";
                if(HFile.Exists(xyzprotpath) == false)
                {
                    prot_iso = prot_solv.CreateXyzWithProtOnly();
                    prot_iso.ToFile(xyzprotpath, false, config.prot_xyz_format);
                    prot_iso.ToFile(xyzprotpath.Replace(".xyz", "_pymol.xyz"), false, Tinker.Xyz.Atom.Format.defformat_digit06);
                }
                prot_iso = Tinker.Xyz.FromFile(xyzprotpath, false, config.prot_xyz_format);

                Dictionary<int, ValueTuple<int, Pdb.Atom, Pdb.Atom>> xyzid_NearResi_NearAtom_NearConnAtom = null;
                xyzid_NearResi_NearAtom_NearConnAtom = config.GetResiPdbatom( prot_solv );

                string filename_prot_solv_protexsolv_hessforc            = "prot_solv_protexsolv.hessforc"           ;
                string filename_prot_solv_protexsolv_testhesssparse_hess = "prot_solv_protexsolv_testhesssparse.hess";
                string filename_prot_solv_protexsolv_testgrad_output_txt = "prot_solv_protexsolv_testgrad_output.txt";
                HessForc.Coarse.HessForcInfo hessforc_exsolv =
                    null;
                    //  config.GetHessForceExsolv
                    //  ( prot_solv, prm, keys_cutoff12
                    //  , filename_prot_solv_protexsolv_hessforc            : filename_prot_solv_protexsolv_hessforc
                    //  , filename_prot_solv_protexsolv_testhesssparse_hess : filename_prot_solv_protexsolv_testhesssparse_hess
                    //  , filename_prot_solv_protexsolv_testgrad_output_txt : filename_prot_solv_protexsolv_testgrad_output_txt
                    //  , thres_zeroblk: 1.0E-07
                    //  , coarse_options: coarse_options
                    //  );

                string filename_prot_solv_protexsolv_modes              = "prot_solv_protexsolv_modes.data";
                string filename_prot_solv_protexsolv_modes_eigval       = "prot_solv_protexsolv_modes_eigval.m";
                string filename_prot_solv_protexsolv_modes_freq         = "prot_solv_protexsolv_modes_freq.m";
                string filename_prot_solv_protexsolv_modes_crosscorr    = "prot_solv_protexsolv_modes_crosscorr.mat";
                Mode[]               hessforc_exsolv_modes              = null;
                Matrix               hessforc_exsolv_crosscorr          = null;
                //  if(config.minimized)
                //      (hessforc_exsolv_modes, hessforc_exsolv_crosscorr) = config.GetModesCorr
                //      ( hessforc_exsolv
                //      , filename_prot_solv_protexsolv_modessel            : filename_prot_solv_protexsolv_modes
                //      , filename_prot_solv_protexsolv_modes_eigval        : filename_prot_solv_protexsolv_modes_eigval
                //      , filename_prot_solv_protexsolv_modes_freq          : filename_prot_solv_protexsolv_modes_freq
                //      , filename_prot_solv_protexsolv_modes_crosscorr     : filename_prot_solv_protexsolv_modes_crosscorr
                //      , modessel_num : null
                //      );

                string filename_prot_solv_protexsolv_hess_blksumeig     = "prot_solv_protexsolv_hess_blksumeig.mat";
                Matrix hessforc_exsolv_hess_blksumeig = null;
                //  if(hessforc_exsolv != null)
                //  {
                //      string cachebase = config.cachebase;
                //      string path_hess_blksumeig  = cachebase + filename_prot_solv_protexsolv_hess_blksumeig;
                //  
                //      if(HFile.ExistsAll(path_hess_blksumeig) == false)
                //      {
                //          var hess = hessforc_exsolv.hess;
                //          Matrix hess_blksumeig = Matrix.Zeros(hess.ColBlockSize, hess.RowBlockSize);
                //          for(int bc=0; bc<hess.ColBlockSize; bc++)
                //          {
                //              for(int br=0; br<hess.RowBlockSize; br++)
                //              {
                //                  if(bc >= br)
                //                      continue;
                //                  var blk = hess.GetBlock(bc, br);
                //                  if(blk != null)
                //                  {
                //                      var sumeigval = blk.Trace();
                //                      hess_blksumeig[bc, br] = hess_blksumeig[br, bc] = sumeigval;
                //                  }
                //              }
                //          }
                //          Matlab.SaveMatrix(path_hess_blksumeig, hess_blksumeig.ToArray(), true);
                //      }
                //      hessforc_exsolv_hess_blksumeig = Matlab.LoadMatrix(path_hess_blksumeig, true);
                //  }

                string filename_prot_solv_prot_hessforc            = "prot_solv_prot.hessforc"           ;
                string filename_prot_solv_prot_testhesssparse_hess = "prot_solv_prot_testhesssparse.hess";
                string filename_prot_solv_prot_testgrad_output_txt = "prot_solv_prot_testgrad_output.txt";
                HessForc.Coarse.HessForcInfo hessforc_iso =
                    config.GetHessForceIso
                    ( prot_iso, prm, keys_cutoff12
                    , filename_prot_solv_prot_hessforc            : filename_prot_solv_prot_hessforc
                    , filename_prot_solv_prot_testhesssparse_hess : filename_prot_solv_prot_testhesssparse_hess
                    , filename_prot_solv_prot_testgrad_output_txt : filename_prot_solv_prot_testgrad_output_txt
                    );

                //string filename_prot_solv_prot_sbnmamodes              = "prot_solv_prot_sbnmamodes.data";
                //string filename_prot_solv_prot_sbnmamodes_eigval       = "prot_solv_prot_sbnmamodes_eigval.m";
                //string filename_prot_solv_prot_sbnmamodes_freq         = "prot_solv_prot_sbnmamodes_freq.m";
                //string filename_prot_solv_prot_sbnmamodes_crosscorr    = "prot_solv_prot_sbnmamodes_crosscorr.mat";
                //Mode[]           hessforc_prot_sbnmamodes              = null;
                //Matrix           hessforc_prot_sbnmacrosscorr          = null;
                //(hessforc_prot_sbnmamodes, hessforc_prot_sbnmacrosscorr) = config.GetSbnmaModesCorr
                //    ( prot_iso, prm
                //    , filename_prot_solv_prot_sbnmamodes               : filename_prot_solv_prot_sbnmamodes
                //    , filename_prot_solv_prot_sbnmamodes_eigval        : filename_prot_solv_prot_sbnmamodes_eigval
                //    , filename_prot_solv_prot_sbnmamodes_freq          : filename_prot_solv_prot_sbnmamodes_freq
                //    , filename_prot_solv_prot_sbnmamodes_crosscorr     : filename_prot_solv_prot_sbnmamodes_crosscorr
                //    , modessel_num : null
                //    );

                string filename_prot_solv_protimsolv_hessforc            = "prot_solv_protimsolv.hessforc"           ;
                string filename_prot_solv_protimsolv_testhesssparse_hess = "prot_solv_protimsolv_testhesssparse.hess";
                string filename_prot_solv_protimsolv_testgrad_output_txt = "prot_solv_protimsolv_testgrad_output.txt";
                HessForc.Coarse.HessForcInfo hessforc_imsolv =
                    config.GetHessForceIso
                    ( prot_iso, prm, keys_cutoff12_imsolv
                    , filename_prot_solv_prot_hessforc            : filename_prot_solv_protimsolv_hessforc
                    , filename_prot_solv_prot_testhesssparse_hess : filename_prot_solv_protimsolv_testhesssparse_hess
                    , filename_prot_solv_prot_testgrad_output_txt : filename_prot_solv_protimsolv_testgrad_output_txt
                    );

                if(HDebug.False)
                    #region
                {
                    double maxabs_hessforc_exsolv = hessforc_exsolv.hess.HAbsMax();
                    double maxabs_hessforc_iso    = hessforc_iso   .hess.HAbsMax();
                    double maxabs_hessforc_imsolv = hessforc_imsolv.hess.HAbsMax();
                    double maxabs_diff_hess       = (hessforc_exsolv.hess - hessforc_iso.hess).HAbsMax();
                }
                    #endregion

                double[] solvgaps = new double[] { 8, 12, 16 };
                double[] solvgap__protsolv_hess_freq = new double[] { 0, 2, 3, 4, 8 };
                Dictionary<double, SolvGapInfo> solvgapinfos
                    = null;
                if(compSolvGaps)
                {
                    solvgapinfos  =
                    GetSolvgapXyzHessForceExsolvModesCrosscorr
                    ( config, prm, keys_cutoff12
                    , prot_solv     : prot_solv
                    , prot_iso      : prot_iso
                    , coarse_options: coarse_options
                    , solvgaps      : solvgaps
                    , xyzid_NearResi_NearAtom_NearConnAtom : xyzid_NearResi_NearAtom_NearConnAtom
                    , solvgap__protsolv_hess_freq: solvgap__protsolv_hess_freq
                    );
                }

                if(HDebug.False)
                    #region
                if(solvgapinfos != null)
                {
                    Dictionary<double, SolvGapInfo> loc_solvgap_xyz_hessforc = new Dictionary<double, SolvGapInfo>();
                    loc_solvgap_xyz_hessforc.Add(0, new SolvGapInfo
                        {
                            xyzpath   = null,
                            solvgap   = 0,
                            xyz       = null,
                            hessforc  = hessforc_iso,
                            modes     = null,
                            crosscorr = null,
                        }
                    );
                    foreach(var gap_hessforc in solvgapinfos)
                        loc_solvgap_xyz_hessforc.Add(gap_hessforc.Key, gap_hessforc.Value);

                    // determine solvgap->hess->maxabs
                    Dictionary<double, double> gap_maxabs = new Dictionary<double, double>();
                    foreach(var gap_hessforc in loc_solvgap_xyz_hessforc)
                    {
                        var gap      = gap_hessforc.Key;
                        double maxabs = gap_hessforc.Value.hessforc.hess.HAbsMax();
                        gap_maxabs.Add(gap, maxabs);
                    }


                    Dictionary<double, (double, double)> gap_hessdiff = new Dictionary<double, (double, double)>();
                    foreach(var gap_hessforc in loc_solvgap_xyz_hessforc)
                    {
                        var gap   = gap_hessforc.Key;
                        var hess0 = hessforc_exsolv.hess;
                        var hessx = gap_hessforc.Value.hessforc.hess;
                        double hessdiff2 = 0;
                        double hessdiff22 = 0;
                        double[,] zeroblk = new double[3,3];
                        List<Tuple<int, int, double>> lst_bc_br_diff2 = new List<Tuple<int, int, double>>();    // list of (arom_c, atom_r, (exsolv.hess - solvgap.hess).sum_diff2)
                        for(int bc=0; bc<hess0.ColBlockSize; bc++)
                            for(int br=0; br<hess0.RowBlockSize; br++)
                            {
                                var blk0 = hess0.GetBlock(bc, br); if(blk0 == null) blk0 = zeroblk;
                                var blkx = hessx.GetBlock(bc, br); if(blkx == null) blkx = zeroblk;

                                double diff2 = 0;
                                for(int c=0; c<3; c++)
                                    for(int r=0; r<3; r++)
                                    {
                                        diff2 = (blk0[c,r] - blkx[c,r]);
                                        diff2 = diff2 * diff2;
                                        hessdiff2  += diff2;
                                        hessdiff22 += diff2 * diff2;
                                    }
                                if(bc < br)
                                    lst_bc_br_diff2.Add(new Tuple<int, int, double>(bc, br, diff2));
                            }

                        // sort list(arom_c, atom_r, (exsolv.hess - solvgap.hess).sum_diff2) by sum_diff2
                        // to find the max-diff block
                        lst_bc_br_diff2 = lst_bc_br_diff2.HSelectByIndex(lst_bc_br_diff2.HListItem3().HIdxSorted().HReverse()).ToList();

                        //gap_maxabs.Add(gap, maxabs);
                        double hessdiff2_mean = hessdiff2 / (hess0.RowSize * hess0.ColSize);
                        double hessdiff2_var  = hessdiff22 / (hess0.RowSize * hess0.ColSize);
                               hessdiff2_var  = hessdiff2_var - hessdiff2_mean * hessdiff2_mean;

                        gap_hessdiff.Add(gap, (hessdiff2_mean, hessdiff2_var));
                    }
                }
                #endregion

                if(HDebug.IsDebuggerAttached && HDebug.False)
                    #region
                {
                    var dhess1 = (hessforc_exsolv.hess - hessforc_iso   .hess).HEnumElement().HEnumCall(HMath.HPow2).Sum().HPow(0.5);
                    var dhess2 = (hessforc_imsolv.hess - hessforc_iso   .hess).HEnumElement().HEnumCall(HMath.HPow2).Sum().HPow(0.5);
                    var dhess3 = (hessforc_exsolv.hess - hessforc_imsolv.hess).HEnumElement().HEnumCall(HMath.HPow2).Sum().HPow(0.5);

                    var dforc1 = (hessforc_exsolv.forc.ToVector() - hessforc_iso   .forc.ToVector()).Dist;
                    var dforc2 = (hessforc_imsolv.forc.ToVector() - hessforc_iso   .forc.ToVector()).Dist;
                    var dforc3 = (hessforc_exsolv.forc.ToVector() - hessforc_imsolv.forc.ToVector()).Dist;
                }
                    #endregion

                //if(doAnalysis)
                //{
                //    config.WriteAnalysisData(prot_solv, hessforc_exsolv, hessforc_iso   , "data_exsolv_iso.m"   , xyzid_NearResi_NearAtom_NearConnAtom);
                //    config.WriteAnalysisData(prot_solv, hessforc_imsolv, hessforc_iso   , "data_imsolv_iso.m"   , xyzid_NearResi_NearAtom_NearConnAtom);
                //    config.WriteAnalysisData(prot_solv, hessforc_exsolv, hessforc_imsolv, "data_exsolv_imsolv.m", xyzid_NearResi_NearAtom_NearConnAtom);
                //}

                string filename_prot_solv_protexsolv_cahess      = "prot_solv_protexsolv_cahess.data";
                string filename_prot_solv_protexsolv_camodes     = "prot_solv_protexsolv_camodes.data";
                string filename_prot_solv_protexsolv_cacrosscorr = "prot_solv_protexsolv_cacrosscorr.data";
                HessMatrix      prot_solv_protexsolv_cahess      = null;
                Mode[]          prot_solv_protexsolv_camodes     = null;
                Matrix          prot_solv_protexsolv_cacrosscorr = null;
                {
                    if(HFile.ExistsAll
                        ( config.cachebase + filename_prot_solv_protexsolv_cahess     
                        , config.cachebase + filename_prot_solv_protexsolv_camodes    
                        , config.cachebase + filename_prot_solv_protexsolv_cacrosscorr
                        ) == false)
                    {
                        HDebug.Assert(hessforc_exsolv != null);
                        List<int> idxcas = HessInfoExsolvIsoImsolv.GetProtCaIdxs(prot_iso, xyzid_NearResi_NearAtom_NearConnAtom);

                        prot_solv_protexsolv_cahess      = Hess.GetHessCoarseBlkmat(hessforc_exsolv.hess, idxcas, "inv");
                        prot_solv_protexsolv_camodes     = Hess.GetModesFromHess(prot_solv_protexsolv_cahess, null);
                        prot_solv_protexsolv_cacrosscorr = prot_solv_protexsolv_camodes.HSelectFrom(6).GetCorrMatrixMatlab();

                        HSerialize.Serialize(config.cachebase + filename_prot_solv_protexsolv_cahess     , null, prot_solv_protexsolv_cahess     );
                        HSerialize.Serialize(config.cachebase + filename_prot_solv_protexsolv_camodes    , null, prot_solv_protexsolv_camodes    );
                        HSerialize.Serialize(config.cachebase + filename_prot_solv_protexsolv_cacrosscorr, null, prot_solv_protexsolv_cacrosscorr);
                    }
                    {
                        HSerialize.Deserialize(config.cachebase + filename_prot_solv_protexsolv_cahess     , null, out prot_solv_protexsolv_cahess     );
                        HSerialize.Deserialize(config.cachebase + filename_prot_solv_protexsolv_camodes    , null, out prot_solv_protexsolv_camodes    );
                        HSerialize.Deserialize(config.cachebase + filename_prot_solv_protexsolv_cacrosscorr, null, out prot_solv_protexsolv_cacrosscorr);
                    }
                }

                return new HessInfoExsolvIsoImsolv
                {
                    config                                            = config                                           ,
                    keys_cutoff12                                     = keys_cutoff12                                    ,
                    keys_cutoff12_imsolv                              = keys_cutoff12_imsolv                             ,
                    prot_solv                                         = prot_solv                                        ,
                    prot_iso                                          = prot_iso                                         ,
                    prm                                               = prm                                              ,
                    xyzid_NearResi_NearAtom_NearConnAtom              = xyzid_NearResi_NearAtom_NearConnAtom             ,

                    filename_prot_solv_protexsolv_hessforc            = filename_prot_solv_protexsolv_hessforc           ,
                    filename_prot_solv_protexsolv_testhesssparse_hess = filename_prot_solv_protexsolv_testhesssparse_hess,
                    filename_prot_solv_protexsolv_testgrad_output_txt = filename_prot_solv_protexsolv_testgrad_output_txt,
                    hessforc_exsolv                                   = hessforc_exsolv                                  ,

                    filename_prot_solv_prot_hessforc                  = filename_prot_solv_prot_hessforc                 ,
                    filename_prot_solv_prot_testhesssparse_hess       = filename_prot_solv_prot_testhesssparse_hess      ,
                    filename_prot_solv_prot_testgrad_output_txt       = filename_prot_solv_prot_testgrad_output_txt      ,
                    hessforc_iso                                      = hessforc_iso                                     ,

                    filename_prot_solv_protimsolv_hessforc            = filename_prot_solv_protimsolv_hessforc           ,
                    filename_prot_solv_protimsolv_testhesssparse_hess = filename_prot_solv_protimsolv_testhesssparse_hess,
                    filename_prot_solv_protimsolv_testgrad_output_txt = filename_prot_solv_protimsolv_testgrad_output_txt,
                    hessforc_imsolv                                   = hessforc_imsolv                                  ,

                    solvgapinfos              = solvgapinfos             ,

                    hessforc_exsolv_modes          = hessforc_exsolv_modes         ,
                    hessforc_exsolv_crosscorr      = hessforc_exsolv_crosscorr     ,
                    hessforc_exsolv_hess_blksumeig = hessforc_exsolv_hess_blksumeig,
                    //name_object = new Dictionary<string, object>
                    //{
                    //    {"hessforc_exsolv_modes100"         , hessforc_exsolv_modes100          },
                    //    {"hessforc_exsolv_crosscorr"        , hessforc_exsolv_crosscorr         },
                    //    {"hessforc_exsolv_hess_blksumeig"   , hessforc_exsolv_hess_blksumeig    },
                    //},

                    exsolv_cahess      = prot_solv_protexsolv_cahess     ,
                    exsolv_camodes     = prot_solv_protexsolv_camodes    ,
                    exsolv_cacrosscorr = prot_solv_protexsolv_cacrosscorr,

                  //hessforc_prot_sbnmamodes     = hessforc_prot_sbnmamodes    ,
                  //hessforc_prot_sbnmacrosscorr = hessforc_prot_sbnmacrosscorr,
                };
            }

            public static Dictionary<double, SolvGapInfo> GetSolvgapXyzHessForceExsolvModesCrosscorr
                ( Config config
                , Tinker.Prm prm
                , string[] keys_cutoff12
                , Tinker.Xyz prot_solv
                , Tinker.Xyz prot_iso
                , string[] coarse_options
                , double[] solvgaps
                , Dictionary<int, ValueTuple<int, Pdb.Atom, Pdb.Atom>> xyzid_NearResi_NearAtom_NearConnAtom
                , double[] solvgap__protsolv_hess_freq //= new double[] { 2, 3, 4, 8 };
                )
            {
                Dictionary<double, SolvGapInfo> solvgapinfos = new Dictionary<double, SolvGapInfo>();

                foreach(double solvgap in solvgaps)//.Reverse())
                {
                    SolvGapInfo solvgapinfo = GetSolvgapXyzHessForceExsolvModesCrosscorr
                        ( config
                        , prm
                        , keys_cutoff12
                        , prot_solv
                        , prot_iso
                        , coarse_options
                        , solvgap
                        , xyzid_NearResi_NearAtom_NearConnAtom
                        , solvgap__protsolv_hess_freq
                        );

                    if(solvgapinfo == null)
                        continue;

                    solvgapinfos.Add(solvgap, solvgapinfo);
                }

                return solvgapinfos;
            }

            public static SolvGapInfo GetSolvgapXyzHessForceExsolvModesCrosscorr
                ( Config config
                , Tinker.Prm prm
                , string[] keys_cutoff12
                , Tinker.Xyz prot_solv
                , Tinker.Xyz prot_iso
                , string[] coarse_options
                , double solvgap
                , Dictionary<int, ValueTuple<int, Pdb.Atom, Pdb.Atom>> xyzid_NearResi_NearAtom_NearConnAtom
                , double[] solvgap__protsolv_hess_freq //= new double[] { 2, 3, 4, 8 };
                )
            {
                string prot_solvgap_base = string.Format("prot_solvgap{0:00}", solvgap);
                string filename_prot_gap_xyz = config.cachebase + prot_solvgap_base + ".xyz";
                if(HFile.Exists(filename_prot_gap_xyz) == false)
                {
                    int               prot_cnt       = prot_iso.atoms.Length;
                    Tinker.Xyz.Atom[] prot_iso_atoms = prot_iso .atoms;
                    Tinker.Xyz.Atom[] prot_slv_atoms = prot_solv.atoms;

                    KDTreeDLL.KDTree<Tinker.Xyz.Atom> kdtree = new KDTreeDLL.KDTree<Tinker.Xyz.Atom>(3);
                    List<Tinker.Xyz.Atom> prot_atoms = new List<Tinker.Xyz.Atom>();
                    List<ValueTuple<Tinker.Xyz.Atom, Tinker.Xyz.Atom, Tinker.Xyz.Atom>> solvs = new List<(Tinker.Xyz.Atom, Tinker.Xyz.Atom, Tinker.Xyz.Atom)>();
                    for(int i=0; i< prot_slv_atoms.Length; i++)
                    {
                        var atom = prot_slv_atoms[i];
                        if(i < prot_cnt)
                        {
                            // check if a protein atom in prot_iso are same to that in prot_solv
                            HDebug.Assert( atom.AtomType == prot_iso_atoms[i].AtomType);
                            HDebug.Assert((atom.Coord     - prot_iso_atoms[i].Coord).Dist < 0.0001);
                            // add atom in prot_gat
                            prot_atoms.Add(atom);
                            // add atom into kdtree, in order to check the distance between protein and a solvent atom
                            kdtree.insert     (atom.Coord, atom);
                        }
                        else
                        {
                            if(atom.AtomType == "OT ")
                            {
                                var atom1 = prot_slv_atoms[i + 1];
                                var atom2 = prot_slv_atoms[i + 2];
                                HDebug.Assert(atom.BondedIds.Contains(atom1.Id) && atom1.AtomType == "HT ");
                                HDebug.Assert(atom.BondedIds.Contains(atom2.Id) && atom2.AtomType == "HT ");
                                solvs.Add(new ValueTuple<Tinker.Xyz.Atom, Tinker.Xyz.Atom, Tinker.Xyz.Atom>(atom, atom1, atom2));
                            }
                            else if(atom.AtomType == "HT ")
                            {
                                HDebug.Assert(solvs.Last().ToTuple().HContains(atom));
                            }
                            else
                            {
                                HDebug.Assert(false);
                            }
                        }
                    }

                    List<int> prot_gap_idsidsSelect = new List<int>();
                    foreach(var atm in prot_atoms)
                        prot_gap_idsidsSelect.Add(atm.Id);

                    foreach(var solv in solvs)
                    {
                        var OT  = solv.Item1; HDebug.Assert(OT .AtomType == "OT ");
                        var HT1 = solv.Item2; HDebug.Assert(HT1.AtomType == "HT ");
                        var HT2 = solv.Item3; HDebug.Assert(HT2.AtomType == "HT ");
                        HDebug.Assert(HT1.Id != HT2.Id);

                        var near = kdtree.nearest(OT.Coord);
                        double dist = (OT.Coord  - near.Coord).Dist;
                        if(dist <= solvgap)
                        {
                            prot_gap_idsidsSelect.Add(OT .Id);
                            prot_gap_idsidsSelect.Add(HT1.Id);
                            prot_gap_idsidsSelect.Add(HT2.Id);
                        }
                    }
                    if(prot_gap_idsidsSelect.Count == prot_slv_atoms.Length)
                        return null; //break;

                    Tinker.Xyz prot_solvgap = prot_solv.CloneBySelectIds(prot_gap_idsidsSelect);

                    prot_solvgap.ToFile(filename_prot_gap_xyz, false);
                    prot_solvgap.ToFile(filename_prot_gap_xyz.Replace(".xyz", "_pymol.xyz"), false, Tinker.Xyz.Atom.Format.defformat_digit06);
                }

                string prot_solvmin_base = string.Format("prot_solvgap{0:00}_minimized", solvgap);
                string filename_prot_min_xyz = config.cachebase + prot_solvmin_base + ".xyz";
                if(HFile.Exists(filename_prot_min_xyz) == false)
                {
                    Tinker.Xyz prot_solvgap = Tinker.Xyz.FromFile(filename_prot_gap_xyz, false, config.prot_xyz_format);

                    var min = Tinker.Run.Minimize
                        ( tinkerpath_minimize
                        , prot_solvgap
                        , prm
                        , tempbase: @"K:\Temp\"
                        , copytemp: null
                        , param: "0.000001"
                        , atomsToFix: null
                        , pause: false
                        , keys: HFile.ReadAllLines(config.keypath_cutoff12)
                    );

                    min.minxyz.ToFile(filename_prot_min_xyz, false);
                }

                {
                    Tinker.Xyz prot_solvgap = Tinker.Xyz.FromFile(filename_prot_min_xyz, false, config.prot_xyz_format);
                
                    string filename_prot_solvgap_protexsolv_hessforc            = prot_solvmin_base+"_protexsolv.hessforc"           ;
                    string filename_prot_solvgap_protexsolv_testhesssparse_hess = prot_solvmin_base+"_protexsolv_testhesssparse.hess";
                    string filename_prot_solvgap_protexsolv_testgrad_output_txt = prot_solvmin_base+"_protexsolv_testgrad_output.txt";
                    HessForc.Coarse.HessForcInfo solvgap_hessforc_exsolv;
                    {
                        solvgap_hessforc_exsolv =
                            config.GetHessForceExsolv
                            ( prot_solvgap, prm, keys_cutoff12
                            , filename_prot_solv_protexsolv_hessforc            : filename_prot_solvgap_protexsolv_hessforc
                            , filename_prot_solv_protexsolv_testhesssparse_hess : filename_prot_solvgap_protexsolv_testhesssparse_hess
                            , filename_prot_solv_protexsolv_testgrad_output_txt : filename_prot_solvgap_protexsolv_testgrad_output_txt
                            , thres_zeroblk: 1.0E-07
                            , coarse_options: coarse_options
                            );

                        if(HFile.Exists(config.cachebase + filename_prot_solvgap_protexsolv_testhesssparse_hess)) HFile.Delete(config.cachebase + filename_prot_solvgap_protexsolv_testhesssparse_hess);
                        if(HFile.Exists(config.cachebase + filename_prot_solvgap_protexsolv_testgrad_output_txt)) HFile.Delete(config.cachebase + filename_prot_solvgap_protexsolv_testgrad_output_txt);
                    }

                    SolvGapInfo.ModeMsfContb[] protsolv_mode_msfinfo = null;
                    if (solvgap__protsolv_hess_freq.Contains(solvgap))
                    {
                        string filename_prot_solvgap_protsolv_testhesssparse_hess_freq = prot_solvmin_base + "_protsolv_mode_msfinfo";
                        if(HFile.Exists(config.cachebase + filename_prot_solvgap_protsolv_testhesssparse_hess_freq+".data") == false)
                        {
                            string path_prot_solvgap_protexsolv_testhesssparse_hess_temp = filename_prot_solvgap_protexsolv_testhesssparse_hess + ".temp";
                            string path_prot_solvgap_protexsolv_testgrad_output_txt_temp = filename_prot_solvgap_protexsolv_testgrad_output_txt + ".temp";
                            HessForc.Coarse.HessForcInfo protsolv_hessforcinfo =
                                config.GetHessForceFull
                                ( prot_solvgap
                                , prm
                                , keys_cutoff12
                                , path_prot_solvgap_protexsolv_testhesssparse_hess_temp
                                , path_prot_solvgap_protexsolv_testgrad_output_txt_temp
                                );

                            var protsolv_hessinfo = protsolv_hessforcinfo.GetHessInfo();
                            var protsolv_modes    = protsolv_hessinfo.GetModesMassReduced();
                            var protsolv_freqs    = protsolv_modes.ListFreq();
                                protsolv_mode_msfinfo = new SolvGapInfo.ModeMsfContb[protsolv_modes.Length];
                            {
                                int prot_cnt_3 = prot_iso.atoms.Length * 3;
                                for(int im=0; im<protsolv_modes.Length; im++)
                                {
                                    Mode mode = protsolv_modes[im];
                                    double   freq = mode.freq;
                                    double[] vec  = mode.eigvec.ToArray();
                                    double prot_SumMsf = 0;
                                    double solv_SumMsf = 0;
                                    for(int j=0         ; j<prot_cnt_3; j++) prot_SumMsf += vec[j] * vec[j];
                                    for(int j=prot_cnt_3; j<vec.Length; j++) solv_SumMsf += vec[j] * vec[j];
                                    HDebug.Assert((prot_cnt_3/3) + ((vec.Length-prot_cnt_3)/3) == vec.Length/3);
                                    double all_SumMsf = prot_SumMsf + solv_SumMsf;
                                    prot_SumMsf /= all_SumMsf;
                                    solv_SumMsf /= all_SumMsf;
                                    double prot_AvgMsf = prot_SumMsf / ( prot_cnt_3              /3); if(double.IsNaN(prot_AvgMsf)) prot_AvgMsf = 0;
                                    double solv_AvgMsf = solv_SumMsf / ((vec.Length - prot_cnt_3)/3); if(double.IsNaN(solv_AvgMsf)) solv_AvgMsf = 0;
                                    protsolv_mode_msfinfo[im] = new SolvGapInfo.ModeMsfContb
                                    {
                                        freq        = freq       ,
                                        prot_SumMsf = prot_SumMsf,
                                        solv_SumMsf = solv_SumMsf,
                                        prot_AvgMsf = prot_AvgMsf,
                                        solv_AvgMsf = solv_AvgMsf,
                                    };
                                }
                            }

                            HSerialize.Serialize(config.cachebase + filename_prot_solvgap_protsolv_testhesssparse_hess_freq+".data", null, protsolv_mode_msfinfo);
                            HFile.WriteAllLines (config.cachebase + filename_prot_solvgap_protsolv_testhesssparse_hess_freq+"_freq.m", Mathematica.ToString2(protsolv_freqs));

                            if(HFile.Exists(config.cachebase + path_prot_solvgap_protexsolv_testhesssparse_hess_temp)) HFile.Delete(config.cachebase + path_prot_solvgap_protexsolv_testhesssparse_hess_temp);
                            if(HFile.Exists(config.cachebase + path_prot_solvgap_protexsolv_testgrad_output_txt_temp)) HFile.Delete(config.cachebase + path_prot_solvgap_protexsolv_testgrad_output_txt_temp);
                        }
                        HSerialize.Deserialize(config.cachebase + filename_prot_solvgap_protsolv_testhesssparse_hess_freq+".data", null, out protsolv_mode_msfinfo);
                    }

                    string filename_prot_solvgap_protexsolv_modes               = config.cachebase + prot_solvmin_base+"_protexsolv.modes";
                    string filename_prot_solvgap_protexsolv_modes__crosscorr    = config.cachebase + prot_solvmin_base+"_protexsolv.modes_crosscorr.mat";
                    Mode[] modes;
                    Matrix crosscorr;
                    if(HFile.ExistsAll
                        ( filename_prot_solvgap_protexsolv_modes
                        , filename_prot_solvgap_protexsolv_modes__crosscorr
                        ) == false)
                    {
                        modes               = solvgap_hessforc_exsolv.GetHessInfo().GetModesMassReduced();
                        var crosscorr_modes = modes.HSelectFrom(6);
                        crosscorr           = crosscorr_modes.GetCorrMatrixMatlab();

                        HSerialize.Serialize(filename_prot_solvgap_protexsolv_modes     , null, modes);
                        Matlab.SaveMatrix   (filename_prot_solvgap_protexsolv_modes__crosscorr, crosscorr.ToArray(), true);
                        HFile.WriteAllText  (filename_prot_solvgap_protexsolv_modes__crosscorr+"_freq.m", Mathematica.ToString(crosscorr_modes.ListFreq()).Replace(",", "\n,"));
                    }
                    {
                        HSerialize.Deserialize       (filename_prot_solvgap_protexsolv_modes, null, out modes);
                        crosscorr = Matlab.LoadMatrix(filename_prot_solvgap_protexsolv_modes__crosscorr, true);

                        //{
                        //    double[] freqs = modes.ListFreq();
                        //    HFile.WriteAllLines(filename_prot_solvgap_protexsolv_modes + ".freq.m", Mathematica.ToString2(freqs));
                        //}
                    }

                    string filename_prot_solvgap_protexsolv_cahess            = config.cachebase + prot_solvmin_base+"_protexsolv.cahess";
                    string filename_prot_solvgap_protexsolv_camodes           = config.cachebase + prot_solvmin_base+"_protexsolv.camodes";
                    string filename_prot_solvgap_protexsolv_camodes_crosscorr = config.cachebase + prot_solvmin_base+"_protexsolv.camodes_crosscorr.mat";
                    HessMatrix                              cahess      = null;
                    Mode[]                                  camodes     = null;
                    Matrix                                  cacrosscorr = null;
                    {
                        if(HFile.ExistsAll
                            ( filename_prot_solvgap_protexsolv_cahess     
                            , filename_prot_solvgap_protexsolv_camodes    
                            , filename_prot_solvgap_protexsolv_camodes_crosscorr
                            ) == false)
                        {
                            HDebug.Assert(solvgap_hessforc_exsolv != null);
                            List<int> idxcas = HessInfoExsolvIsoImsolv.GetProtCaIdxs(prot_iso, xyzid_NearResi_NearAtom_NearConnAtom);

                            cahess      = Hess.GetHessCoarseBlkmat(solvgap_hessforc_exsolv.hess, idxcas, "inv");
                            camodes     = Hess.GetModesFromHess(cahess, null);
                            cacrosscorr = camodes.HSelectFrom(6).GetCorrMatrixMatlab();

                            HSerialize.Serialize(filename_prot_solvgap_protexsolv_cahess           , null, cahess     );
                            HSerialize.Serialize(filename_prot_solvgap_protexsolv_camodes          , null, camodes    );
                            HSerialize.Serialize(filename_prot_solvgap_protexsolv_camodes_crosscorr, null, cacrosscorr);
                        }
                        {
                            HSerialize.Deserialize(filename_prot_solvgap_protexsolv_cahess           , null, out cahess     );
                            HSerialize.Deserialize(filename_prot_solvgap_protexsolv_camodes          , null, out camodes    );
                            HSerialize.Deserialize(filename_prot_solvgap_protexsolv_camodes_crosscorr, null, out cacrosscorr);
                        }
                    }

                    //string filename_prot_solv_protexsolv_modes_eigval       = prot_solvgap_base+"_protexsolv_modes_eigval.m";
                    //string filename_prot_solv_protexsolv_modes_freq         = prot_solvgap_base+"_protexsolv_modes_freq.m";
                    //string filename_prot_solv_protexsolv_modes_crosscorr    = prot_solvgap_base+"_protexsolv_modes_crosscorr.mat";
                    //Matrix solvgap_hessforc_exsolv_crosscorr = null;
                    //if(config.minimized)
                    //    solvgap_hessforc_exsolv_crosscorr = Program_htna_20180225.GetCrossCorr
                    //    ( solvgap_hessforc_exsolv
                    //    , config.cachebase
                    //    , filename_prot_solv_protexsolv_modes_eigval        : filename_prot_solv_protexsolv_modes_eigval
                    //    , filename_prot_solv_protexsolv_modes_freq          : filename_prot_solv_protexsolv_modes_freq
                    //    , filename_prot_solv_protexsolv_modes_crosscorr     : filename_prot_solv_protexsolv_modes_crosscorr
                    //    );

                    return new SolvGapInfo
                    {
                        xyzpath   = filename_prot_min_xyz,
                        solvgap   = solvgap,
                        xyz       = prot_solvgap,
                        hessforc  = solvgap_hessforc_exsolv,
                        modes     = modes,
                        crosscorr = crosscorr,
                        cahess      = cahess,
                        camodes     = camodes,
                        cacrosscorr = cacrosscorr,
                        protsolv_mode_msfinfo = protsolv_mode_msfinfo,
                    };
                }
            }
        }
    }
}
*/