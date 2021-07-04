using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public partial class Run
        {
            public class CTesthess
            {
                public HessMatrix hess;
                public Xyz xyz;
                public Prm prm;

                /// call Hess.HessInfo.FromTinker(...)
                //public Hess.HessInfo GetHessInfo()
                //{
                //    Hess.HessInfo hessinfo = new Hess.HessInfo
                //    {
                //        coords = xyz.atoms.HListCoords(),
                //        hess   = hess,
                //        mass   = prm.atoms.SelectByXyzAtom(xyz.atoms).ListMass(),
                //        atoms  = xyz.atoms,
                //    };
                //    return hessinfo;
                //}
            };

            public static CTesthess Testhess(Tinker.Xyz xyz, Tinker.Prm prm
                                            , string tempbase //=null
                                            // http://www-jmg.ch.cam.ac.uk/cil/tinker/c7.html
                                            // http://dasher.wustl.edu/tinkerwiki/index.php/Main_Page
                                            , int?   digits       = null // the number of digits of precision output by TINKER in reporting potential energies and atomic coordinates.
                                            , string cutoff       = null // [""?or real, A] cutoff distance value for all nonbonded potential energy interactions
                                            , string chg_cutoff   = null // ["" or real, A] cutoff distance value for charge-charge electrostatic potential energy interactions. (default: 9A)
                                            , string vdw_cutoff   = null // ["" or real, A] cutoff distance value for van der Waals potential energy interactions                (default: 9A)
                                            , string ewald_cutoff = null // ["" or real, A] cutoff for use during Ewald summation.                                               (default: 9A)
                                            , string taper        = null // ["" or real, A] cutoff windows for nonbonded potential energy interactions.                          (default: ranges 0.65-0.9 depending on potential function)
                                            , string solvate      = null // [ASP/SASA/ONION/STILL/HCT/ACE/GBSA] solvate options : GBSA or 
                                            , IList<string> keys  = null // other keys
                                            , Dictionary<string, string[]> optOutSource = null
                                            )
            {
                return Testhess
                    ( testhesspath  : null
                    , xyz           : xyz
                    , prm           : prm
                    , tempbase      : tempbase 
                    , digits        : digits 
                    , cutoff        : cutoff 
                    , chg_cutoff    : chg_cutoff 
                    , vdw_cutoff    : vdw_cutoff 
                    , ewald_cutoff  : ewald_cutoff
                    , taper         : taper 
                    , solvate       : solvate 
                    , keys          : keys
                    , optOutSource  : optOutSource
                    );
            }
            public static CTesthess Testhess
                ( string testhesspath
                , Tinker.Xyz xyz, Tinker.Prm prm
                , string tempbase //=null
                // http://www-jmg.ch.cam.ac.uk/cil/tinker/c7.html
                // http://dasher.wustl.edu/tinkerwiki/index.php/Main_Page
                , int?   digits       = null // the number of digits of precision output by TINKER in reporting potential energies and atomic coordinates.
                , string cutoff       = null // [""?or real, A] cutoff distance value for all nonbonded potential energy interactions
                , string chg_cutoff   = null // ["" or real, A] cutoff distance value for charge-charge electrostatic potential energy interactions. (default: 9A)
                , string vdw_cutoff   = null // ["" or real, A] cutoff distance value for van der Waals potential energy interactions                (default: 9A)
                , string ewald_cutoff = null // ["" or real, A] cutoff for use during Ewald summation.                                               (default: 9A)
                , string taper        = null // ["" or real, A] cutoff windows for nonbonded potential energy interactions.                          (default: ranges 0.65-0.9 depending on potential function)
                , string solvate      = null // [ASP/SASA/ONION/STILL/HCT/ACE/GBSA] solvate options : GBSA or 
                , IList<string> keys  = null // other keys
                , Dictionary<string, string[]> optOutSource = null
                , Func<int, int, HessMatrix> HessMatrixZeros =null
                )
            {
                /// cutoff and taper are better to be used together.
                List<string> lkeys = new List<string>();
                if(digits       != null) lkeys.Add("digits                  "+digits.Value);
                if(cutoff       != null) lkeys.Add("CUTOFF                  "+cutoff      );
                if(chg_cutoff   != null) lkeys.Add("CHG-CUTOFF              "+chg_cutoff  );
                if(vdw_cutoff   != null) lkeys.Add("VDW-CUTOFF              "+vdw_cutoff  );
                if(ewald_cutoff != null) lkeys.Add("EWALD-CUTOFF            "+ewald_cutoff);
                if(taper        != null) lkeys.Add("TAPER                   "+taper       );
                if(solvate      != null) lkeys.Add("SOLVATE                 "+solvate     );
                if(keys         != null) lkeys.AddRange(keys);

                var hess = Testhess(testhesspath, xyz, prm, tempbase, lkeys.ToArray(), optOutSource, HessMatrixZeros);
                return hess;
            }
            public static CTesthess Testhess(Tinker.Xyz xyz
                                            , Tinker.Prm prm
                                            , string tempbase //=null
                                            , params string[] keys
                                            )
            {
                return Testhess
                    ( testhesspath  : null
                    , xyz           : xyz
                    , prm           : prm
                    , tempbase      : tempbase 
                    , keys          : keys
                    );
            }
            public static CTesthess Testhess
                ( string testhesspath
                , Tinker.Xyz xyz
                , Tinker.Prm prm
                , string tempbase //=null
                , string[] keys
                , Dictionary<string, string[]> optOutSource // =null
                , Func<int, int, HessMatrix> HessMatrixZeros // =null
                )
            {
                var tmpdir = HDirectory.CreateTempDirectory(tempbase);
                string currpath = HEnvironment.CurrentDirectory;
                CTesthess testhess;
                {
                    HEnvironment.CurrentDirectory = tmpdir.FullName;
                    if(testhesspath == null)
                    {
                        string resbase = "HTLib2.Bioinfo.HTLib2.Bioinfo.External.Tinker.Resources.tinker_6_2_01.";
                        HResource.CopyResourceTo<Tinker>(resbase+"testhess.exe", "testhess.exe");
                        testhesspath = "testhess.exe";
                    }
                    xyz.ToFile("test.xyz", false);
                    prm.ToFile("test.prm");
                    string keypath = null;
                    if((keys != null) && (keys.Length > 0))
                    {
                        keypath = "test.key";
                        HFile.WriteAllLines(keypath, keys);
                    }
                    testhess = Testhess(testhesspath, "test.xyz", "test.prm", keypath, optOutSource, HessMatrixZeros);
                    testhess.xyz = xyz;
                    testhess.prm = prm;
                }
                HEnvironment.CurrentDirectory = currpath;
                try{ tmpdir.Delete(true); } catch {}

                string test_eig = "true";
                if(test_eig == "false")
                {
                    Vector D;
                    using(new Matlab.NamedLock(""))
                    {
                        Matlab.PutSparseMatrix("testeig.H", testhess.hess);
                        Matlab.Execute("testeig.H = (testeig.H + testeig.H')/2;");
                        Matlab.Execute("[testeig.V, testeig.D] = eig(full(testeig.H));");
                        Matlab.Execute("testeig.D = diag(testeig.D);");
                        D = Matlab.GetVector("testeig.D");
                        Matlab.Execute("clear testeig;");
                    }
                }

                return testhess;
            }
            public static CTesthess Testhess
                ( string testhesspath
                , string xyzpath
                , string prmpath
                , string keypath //=null
                , Dictionary<string, string[]> optOutSource // =null
                , Func<int, int, HessMatrix> HessMatrixZeros // =null
                )
            {
                int size;
                {
                    size = Tinker.Xyz.FromFile(xyzpath, false).atoms.Length;
                }
                {
                    //bool ComputeAnalyticalHessianMatrix = true;
                    string command = "";
                    command += testhesspath; //command += GetProgramPath("testhess.exe");
                    command += " " + xyzpath;
                    command += " " + prmpath;
                    if(keypath != null) command += " -k "+keypath;
                    //command += " " + (ComputeAnalyticalHessianMatrix    ? "Y" : "N");
                    command += " > output.txt";
                    bool pause = false;
                    HProcess.StartAsBatchInConsole(null, pause, command);
                    //int exitcode = HProcess.StartAsBatchSilent(null, null, null, command);
                }

                var testhess = ReadHess(xyzpath, "output.txt", optOutSource, HessMatrixZeros);
                testhess.prm = Prm.FromFile(prmpath);
                if(optOutSource != null)
                {
                    optOutSource.Add("output.txt", HFile.ReadAllLines("output.txt"));
                }
                return testhess;
            }
            public static CTesthess ReadHess
                ( string xyzpath
                , string outputpath
                , Dictionary<string, string[]> optOutSource // =null
                , Func<int, int, HessMatrix> HessMatrixZeros // =null
                )
            {
                var xyz = Tinker.Xyz.FromFile(xyzpath, false);
                return ReadHess
                ( xyz
                , outputpath
                , optOutSource
                , HessMatrixZeros
                );
            }
            public static CTesthess ReadHess
                ( Xyz xyz
                , string outputpath
                , Dictionary<string, string[]> optOutSource // =null
                , Func<int, int, HessMatrix> HessMatrixZeros // =null
                )
            {
                int size = xyz.atoms.Length;

                #region format: output.txt
                ///      ######################################################################
                ///    ##########################################################################
                ///   ###                                                                      ###
                ///  ###            TINKER  ---  Software Tools for Molecular Design            ###
                ///  ##                                                                          ##
                ///  ##                        Version 6.2  February 2013                        ##
                ///  ##                                                                          ##
                ///  ##               Copyright (c)  Jay William Ponder  1990-2013               ##
                ///  ###                           All Rights Reserved                          ###
                ///   ###                                                                      ###
                ///    ##########################################################################
                ///      ######################################################################
                /// 
                /// 
                ///  Atoms with an Unusual Number of Attached Atoms :
                /// 
                ///  Type           Atom Name      Atom Type       Expected    Found
                /// 
                ///  Valence        1496-NR2           69              2         3
                ///  Valence        2438-FE           107              6         5
                /// 
                ///  Diagonal Hessian Elements for Each Atom :
                /// 
                ///       Atom                 X               Y               Z
                /// 
                ///          1            1445.9830       1358.3447       1484.8642
                ///          2             212.9895        294.4584        604.5062
                ///          3             187.1986        751.6727        125.0336
                ///          4             813.4799        132.1743        151.5707
                ///        ...
                ///       2507             178.7277        479.7434        243.4103
                ///       2508             969.8813        977.2507       1845.6726
                ///       2509             390.1648        232.0577       1029.6844
                ///       2510             322.3674        337.8749        996.8230
                /// 
                ///  Sum of Diagonal Hessian Elements :          5209042.9060
                /// 
                ///  Hessian Matrix written to File :  test.hes                                
                #endregion
                Tuple<int, double, double, double>[] outDiagHessElems   = new Tuple<int, double, double, double>[size];
                double?                              outSumDiagHessElem = null;
                string                               outHessMatFile     = null;
                string                               outHessFormat      = null;
                {
                    string step = "";
                    foreach(string _line in HFile.ReadLines(outputpath))
                    {
                        string line = _line.Trim();
                        if(line.Length == 0)
                            continue;
                        if(line.Trim().StartsWith("#"))
                            continue;
                        if(line.Contains(" :"))
                        {
                            if     (line.Contains("Atoms with an Unusual Number of Attached Atoms")) { step = "";                  continue; }
                            else if(line.Contains("Diagonal Hessian Elements for Each Atom"       )) { step = "hess diagonal";     continue; }
                            else if(line.Contains("Sum of Diagonal Hessian Elements"              )) { step = "sum hess diagonal";           }
                            else if(line.Contains("Hessian Matrix written to File"                )) { step = "full hess file"; }
                            else if(line.Contains("Hessian Sparse Matrix written to File"         )) { step = "sparse hess file"; }
                            else if(line.Contains("Select Smaller Size for Sparse Hessian Matrix" )) { continue; }
                            else
                            {
                                HDebug.Assert(false);
                                continue;
                            }
                        }
                        switch(step)
                        {
                            case "":
                                continue;
                            case "hess diagonal":
                                {
                                    string[] tokens = line.Split().HRemoveAll("");
                                    if(tokens[0] == "Atom") continue;
                                    int Atom =    int.Parse(tokens[0]); // ; if(   int.TryParse(tokens[0], out Atom) == false) continue;
                                    double X = double.Parse(tokens[1]); // ; if(double.TryParse(tokens[1], out    X) == false) continue;
                                    double Y = double.Parse(tokens[2]); // ; if(double.TryParse(tokens[2], out    Y) == false) continue;
                                    double Z = double.Parse(tokens[3]); // ; if(double.TryParse(tokens[3], out    Z) == false) continue;
                                    HDebug.Assert(outDiagHessElems[Atom-1] == null);
                                    outDiagHessElems[Atom-1] = new Tuple<int, double, double, double>(Atom, X, Y, Z);
                                }
                                continue;
                            case "sum hess diagonal":
                                {
                                    string[] tokens = line.Split().HRemoveAll("");
                                    double sum = double.Parse(tokens.Last()); // if(double.TryParse(tokens.Last(), out sum) == false) continue;
                                    outSumDiagHessElem = sum;
                                }
                                step = "";
                                continue;
                            case   "full hess file": outHessFormat =   "full matrix"; goto case "hess file";
                            case "sparse hess file": outHessFormat = "sparse matrix"; goto case "hess file";
                            case "hess file":
                                {
                                    string[] tokens = line.Split().HRemoveAll("");
                                    string outputpath_dir = HFile.GetFileInfo(outputpath).Directory.FullName;
                                    outHessMatFile = outputpath_dir + "\\" + tokens.Last();
                                }
                                step = "";
                                continue;
                            default:
                                HDebug.Assert(false);
                                step = "";
                                continue;
                        }
                    }
                }
                #region Format: test.hes
                /// Diagonal Hessian Elements  (3 per Atom)
                /// 
                ///   1445.9830   1358.3447   1484.8642    212.9895    294.4584    604.5062
                ///    187.1986    751.6727    125.0336    813.4799    132.1743    151.5707
                ///   1167.7472   1188.3459   1162.5475    268.8291    498.0188    143.2438
                ///   ...
                /// Off-diagonal Hessian Elements for Atom     1 X
                ///     23.0878     14.6380   -214.0545     96.9308   -186.7740   -205.1460
                ///   -208.7035    -28.9630   -788.9124     39.6748    126.4491   -190.5074
                ///   ...
                /// Off-diagonal Hessian Elements for Atom     1 Y
                ///    -64.1616     97.2697   -292.3813    264.1922   -184.4701   -728.8687
                ///   -109.9103     15.4168   -152.5717    -11.7733     18.8369   -188.4875
                ///   ...
                /// Off-diagonal Hessian Elements for Atom     1 Z
                ///   -155.9409    219.8782   -610.0622    -10.0041    -62.8209   -156.1112
                ///     77.0829    -14.2127   -166.0525     53.1834    -97.4207   -347.9315
                ///   ...
                /// Off-diagonal Hessian Elements for Atom     2 X
                ///   -106.5166    185.8395     19.7546    -10.0094     25.8072     -8.1335
                ///     35.8501    -64.8320      3.8761    -13.8893      9.4266      0.1312
                ///   ...
                /// 
                ///   ...
                /// 
                /// Off-diagonal Hessian Elements for Atom  2508 X
                ///    488.4590    -69.2721   -338.4824   -155.1656    253.3732   -297.4214
                ///   -164.8301   -191.3769
                /// Off-diagonal Hessian Elements for Atom  2508 Y
                ///     74.0161   -149.1060   -273.4863    176.4975   -170.6527   -341.1639
                ///   -263.6141
                /// Off-diagonal Hessian Elements for Atom  2508 Z
                ///    304.9942    223.7496   -851.3028   -242.9481   -310.7953   -821.2532
                /// Off-diagonal Hessian Elements for Atom  2509 X
                ///    198.4498   -331.6530     78.8172     17.3909    -30.0512
                /// Off-diagonal Hessian Elements for Atom  2509 Y
                ///   -227.5916     25.7235     74.5131    -53.2690
                /// Off-diagonal Hessian Elements for Atom  2509 Z
                ///     46.7873     20.7615   -168.0704
                /// Off-diagonal Hessian Elements for Atom  2510 X
                ///    252.6853    247.2963
                /// Off-diagonal Hessian Elements for Atom  2510 Y
                ///    343.6797
                #endregion
                HessMatrix hess;
                {
                    if(HessMatrixZeros != null)
                    {
                        hess = HessMatrixZeros(size * 3, size * 3);
                    }
                    else
                    {
                        switch(outHessFormat)
                        {
                            case "full matrix"  : hess = HessMatrix.ZerosHessMatrix(size * 3, size * 3); break;
                            case "sparse matrix": hess = HessMatrix.ZerosHessMatrix(size * 3, size * 3); break;
                            default:              hess = null;                                           break;
                        }
                    }
                    //HDebug.SetEpsilon(hess);
                    string step = "";
                    int diagidx = -1;
                    int offidx1 = -1;
                    int offidx2 = -1;

                /// string[] lines = HFile.ReadAllLines(outHessMatFile);
                /// GC.WaitForFullGCComplete();
                /// object[] tokenss = new object[lines.Length];
                /// 
                /// Parallel.For(0, lines.Length, delegate(int i)
                /// //for(int i=0; i<lines.Length; i++)
                /// {
                ///     string line = lines[i];
                ///     string[] stokens = line.Trim().Split().RemoveAll("");
                ///     if(stokens.Length != 0)
                ///     {
                ///         double[] dtokens = new double[stokens.Length];
                ///         for(int j=0; j<stokens.Length; j++)
                ///             if(double.TryParse(stokens[j], out dtokens[j]) == false)
                ///             {
                ///                 tokenss[i] = stokens;
                ///                 goto endfor;
                ///             }
                ///         tokenss[i] = dtokens;
                ///     }
                /// endfor: ;
                /// });

                /// System.IO.StreamReader matreader = HFile.OpenText(outHessMatFile);
                /// while(matreader.EndOfStream == false)
                /// for(int i=0; i<lines.Length; i++)
                /// foreach(string line in lines)
                    foreach(string line in HFile.HEnumAllLines(outHessMatFile))
                    {
                ///     string line = lines[i];
                ///     string line = matreader.ReadLine();
                        string[] tokens = line.Trim().Split().HRemoveAll("");
                        if(tokens.Length == 0)
                            continue;
                ///     if(tokenss[i] == null)
                ///         continue;
                ///     string[] stokens = (tokenss[i] as string[]);
                        if(tokens[0]=="Diagonal"     && tokens[1]=="Hessian" && tokens[2]=="Elements") { step = "Diagonal"    ; diagidx = 0; continue; }
                        if(tokens[0]=="Off-diagonal" && tokens[1]=="Hessian" && tokens[2]=="Elements") { step = "Off-diagonal"; offidx1++; offidx2 = offidx1+1; continue; }
                        if(tokens[0]=="Off-diagonal" && tokens[1]=="sparse"  && tokens[2]=="Hessian" &&
                           tokens[3]=="Indexes/Elements" && tokens[4]=="for" && tokens[5]=="Atom"    )
                        {
                            step = "Off-diagonal sparse";
                            offidx1 = (int.Parse(tokens[6]) - 1)*3;
                            switch(tokens[7])
                            {
                                case "X": break;
                                case "Y": offidx1 += 1; break;
                                case "Z": offidx1 += 2; break;
                                default: throw new HException();
                            }
                            continue;
                        }

                ///     double[] dtokens = (tokenss[i] as double[]);
                ///     tokens = new string[20];
                ///     for(int i=0; i*12<line.Length; i++)
                ///         tokens[i] = line.Substring(i*12, 12).Trim();
                ///     tokens = tokens.HRemoveAll("").ToArray();
                ///     tokens = tokens.HRemoveAll(null).ToArray();

                        switch(step)
                        {
                            case "Diagonal":
                                {
                ///                 foreach(double val in dtokens)
                                    foreach(string token in tokens)
                                    {
                                        HDebug.Assert(token.Last() != ' ');
                                        double val = double.Parse(token);
                                        HDebug.Assert(hess[diagidx, diagidx] == 0);
                                        hess[diagidx, diagidx] = val;
                                        diagidx++;
                                    }
                                }
                                continue;
                            case "Off-diagonal":
                                {
                ///                 foreach(double val in dtokens)
                                    foreach(string token in tokens)
                                    {
                                        HDebug.Assert(token.Last() != ' ');
                                        //double val = double.Parse(token);
                                        double val;
                                        bool succ = double.TryParse(token, out val);
                                        if(succ == false)
                                        {
                                            HDebug.Assert(false);
                                            goto case "exception 0";
                                        }
                                        if(size < 3000)
                                        {
                                            HDebug.Assert(hess[offidx1, offidx2] == 0);
                                            HDebug.Assert(hess[offidx2, offidx1] == 0);
                                        }
                                        if(val != 0)
                                        {
                                            hess[offidx1, offidx2] = val;
                                            hess[offidx2, offidx1] = val;
                                        }
                                        offidx2++;
                                    }
                                }
                                continue;
                            case "Off-diagonal sparse":
                                {
                                    HDebug.Assert(tokens.Length % 2 == 0);
                                    for(int i=0; i<tokens.Length; i+=2)
                                    {
                                        HDebug.Assert(tokens[i  ].Last() != ' ');
                                        HDebug.Assert(tokens[i+1].Last() != ' ');
                                        //double val = double.Parse(token);
                                        double val;
                                        bool succ = true;
                                        succ &= int   .TryParse(tokens[i  ], out offidx2); offidx2--;
                                        succ &= double.TryParse(tokens[i+1], out val    );
                                        if(succ == false)
                                        {
                                            HDebug.Assert(false);
                                            goto case "exception 0";
                                        }
                                        if(size < 3000)
                                        {
                                            HDebug.Assert(hess[offidx1, offidx2] == 0);
                                            HDebug.Assert(hess[offidx2, offidx1] == 0);
                                        }
                                        if(val != 0)
                                        {
                                            hess[offidx1, offidx2] = val;
                                            hess[offidx2, offidx1] = val;
                                        }
                                    }
                                }
                                continue;
                            default:
                                HDebug.Assert(false);
                                goto case "exception 1";
                            case "exception 0":
                                throw new HException("incomplete hessian file (0)");
                            case "exception 1":
                                throw new HException("incomplete hessian file (1)");
                        }
                        throw new HException("incomplete hessian file (2)");
                    }
                /// matreader.Close();

                    if(HDebug.IsDebuggerAttached && size < 3000)
                    {
                        //for(int c=0; c<size*3; c++)
                        //    for(int r=0; r<size*3; r++)
                        //        if(hess[c, r] == double.Epsilon)
                        //            throw new Exception("incomplete hessian file (3)");
                        //
                        //for(int c=0; c<size*3; c++)
                        //{
                        //    double sum = 0;
                        //    double max = double.NegativeInfinity;
                        //    for(int r=0; r<size*3; r++)
                        //    {
                        //        max = Math.Max(max, hess[c, r]);
                        //        sum += hess[c, r];
                        //        HDebug.Assert(hess[c, r] != double.Epsilon);
                        //    }
                        //    HDebug.Assert(Math.Abs(sum) < max/1000);
                        //    HDebug.AssertTolerance(0.1, sum);
                        //}

                        Hess.CheckHessDiag(hess, 0.1, null);

                        for(int i=0; i<size; i++)
                        {
                            HDebug.Assert(outDiagHessElems[i].Item1 == i+1);
                            HDebug.AssertTolerance(0.000001, outDiagHessElems[i].Item2 - hess[i*3+0, i*3+0]);
                            HDebug.AssertTolerance(0.000001, outDiagHessElems[i].Item3 - hess[i*3+1, i*3+1]);
                            HDebug.AssertTolerance(0.000001, outDiagHessElems[i].Item4 - hess[i*3+2, i*3+2]);
                        }
                    }
                }

                if(optOutSource != null)
                {
                    optOutSource.Add("test.hes", HFile.HEnumAllLines(outHessMatFile).ToArray());
                }

                return new CTesthess
                {
                    hess = hess,
                    xyz  = xyz,
                };
            }
        }
    }
}
