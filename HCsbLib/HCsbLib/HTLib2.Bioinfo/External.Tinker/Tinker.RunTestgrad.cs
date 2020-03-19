using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class TinkerStatic
    {
        public static Vector[] GetForces(this IList<Tinker.Run.CTestgrad.Anlyt> anlyts, IList<Tinker.Xyz.Atom> atoms)
        {
            Dictionary<int,int> id2idx = new Dictionary<int, int>();
            for(int idx=0; idx<atoms.Count; idx++)
                id2idx.Add(atoms[idx].Id, idx);

            Vector[] forces = new Vector[atoms.Count];
            foreach(var anlyt in anlyts)
            {
                int id = anlyt.id;
                int idx = id2idx[id];
                forces[idx] = new double[] { anlyt.fx, anlyt.fy, anlyt.fz };
            }
            return forces;
        }
        public static double GetTinkerRmsGradient(this IList<Tinker.Run.CTestgrad.Anlyt> anlyts)
        {
            /// RMS Gradient per Atom
            /// xtalmin.alt
            /// 
            /// gnorm = 0.0d0
            /// do i = 1, n
            ///     do j = 1, 3
            ///         gnorm = gnorm + derivs(j,i)**2
            ///     end do
            /// end do
            /// gnorm = sqrt(gnorm)
            /// grms = gnorm / sqrt(dble(nvar/3))
            double gnorm = 0;
            foreach(var analyt in anlyts)
            {
                double x2 = analyt.fx * analyt.fx;
                double y2 = analyt.fy * analyt.fy;
                double z2 = analyt.fz * analyt.fz;
                gnorm += (x2 + y2 + z2);
            }
            gnorm = Math.Sqrt(gnorm);
            double grms = gnorm / Math.Sqrt(anlyts.Count);
            return grms;
        }
        public static double GetTinkerRmsGradient(this Vector[] forcs)
        {
            double gnorm = 0;
            foreach(Vector forc in forcs)
            {
                HDebug.Assert(forc.Size == 3);
                double x2 = forc[0] * forc[0];
                double y2 = forc[1] * forc[1];
                double z2 = forc[2] * forc[2];
                gnorm += (x2 + y2 + z2);
            }
            gnorm = Math.Sqrt(gnorm);
            double grms = gnorm / Math.Sqrt(forcs.Length);
            return grms;
        }
    }
    public partial class Tinker
    {
        public partial class Run
        {
            public class CTestgrad
            {
                public double potential;
                [Serializable]
                public class EnrgCmpnt
                {
                    public double EB   ,EA  ,EBA ,EUB ,EAA ,EOPB; /// bond ,angle    ,___  ,Urey-Bradley ,___ ,____
                    public double EOPD ,EID ,EIT ,ET  ,EPT ,EBT ; /// ____ ,improper ,___  ,torsional    ,___ ,___ 
                    public double ETT  ,EV  ,EC  ,ECD ,ED  ,EM  ; /// ___  ,vdW      ,elec ,___          ,__  ,__  
                    public double EP   ,ER  ,ES  ,ELF ,EG  ,EX  ; /// __   ,__       ,__   ,___          ,__  ,__  

                    public double Energy
                    {
                        get
                        {
                            double enrg = ( EB   + EA  + EBA + EUB + EAA + EOPB
                                          + EOPD + EID + EIT + ET  + EPT + EBT 
                                          + ETT  + EV  + EC  + ECD + ED  + EM  
                                          + EP   + ER  + ES  + ELF + EG  + EX  
                                          );
                            HDebug.AssertTolerance(0.00000001, enrg - (EnergyBondeds+EnergyNonBondeds+EnergySolvent+EnergyOther));
                            return enrg;
                        }
                    }
                    public double EnergyBondeds
                    {
                        get { return (this.EB + this.EA + this.EUB + this.EID); }
                    }
                    public double EnergyNonBondeds
                    {
                        get { return (this.ET + this.EV + this.EC); }
                    }
                    public double EnergySolvent
                    {
                        get { return this.ES; }
                    }
                    public double EnergyOther
                    {
                        get
                        {
                            return  (      +     + EBA +     + EAA + EOPB
                                    + EOPD +     + EIT +     + EPT + EBT 
                                    + ETT  +     +     + ECD + ED  + EM  
                                    + EP   + ER  +     + ELF + EG  + EX  
                                    );
                        }
                    }
                }
                public EnrgCmpnt enrgCmpnt;
                public class Anlyt
                {
                    public int id;
                    public double fx, fy, fz, norm;
                }
                public Anlyt[] anlyts;

                public double G_RMS { get { return anlyts.GetTinkerRmsGradient(); } }
            };

            public static CTestgrad Testgrad(Tinker.Xyz xyz
                                            , Tinker.Prm prm
                                            , string tempbase //=null
                                            // http://www-jmg.ch.cam.ac.uk/cil/tinker/c7.html
                                            , bool? VDWTERM      = null
                                            , bool? CHARGETERM   = null
                                            , bool? BONDTERM     = null
                                            , bool? ANGLETERM    = null
                                            , bool? UREYTERM     = null
                                            , bool? IMPROPTERM   = null
                                            , bool? TORSIONTERM  = null
                                            , string solvate     = null /// solvate options : GBSA or [ASP/SASA/ONION/STILL/HCT/ACE/GBSA]
                                            , IList<string> keys = null /// other keys
                                            , Dictionary<string, string[]> optOutSource = null
                                            )
            {
                return Testgrad
                    ( testgradpath  : null
                    , xyz           : xyz
                    , prm           : prm
                    , tempbase      : tempbase 
                    , VDWTERM       : VDWTERM 
                    , CHARGETERM    : CHARGETERM 
                    , BONDTERM      : BONDTERM 
                    , ANGLETERM     : ANGLETERM 
                    , UREYTERM      : UREYTERM 
                    , IMPROPTERM    : IMPROPTERM 
                    , TORSIONTERM   : TORSIONTERM 
                    , solvate       : solvate 
                    , keys          : keys
                    , optOutSource  : optOutSource
                    );
            }
            public static CTestgrad Testgrad( string testgradpath
                                            , Tinker.Xyz xyz
                                            , Tinker.Prm prm
                                            , string tempbase //=null
                                            // http://www-jmg.ch.cam.ac.uk/cil/tinker/c7.html
                                            , bool? VDWTERM      = null
                                            , bool? CHARGETERM   = null
                                            , bool? BONDTERM     = null
                                            , bool? ANGLETERM    = null
                                            , bool? UREYTERM     = null
                                            , bool? IMPROPTERM   = null
                                            , bool? TORSIONTERM  = null
                                            , string solvate     = null /// solvate options : GBSA or [ASP/SASA/ONION/STILL/HCT/ACE/GBSA]
                                            , IList<string> keys = null /// other keys
                                            , Dictionary<string, string[]> optOutSource = null
                                            )
            {
                List<string> lkeys = new List<string>();
                if(VDWTERM     != null) { if(VDWTERM    .Value == true) lkeys.Add("VDWTERM    "); else lkeys.Add("VDWTERM     NONE"); }
                if(CHARGETERM  != null) { if(CHARGETERM .Value == true) lkeys.Add("CHARGETERM "); else lkeys.Add("CHARGETERM  NONE"); }
                if(BONDTERM    != null) { if(BONDTERM   .Value == true) lkeys.Add("BONDTERM   "); else lkeys.Add("BONDTERM    NONE"); }
                if(ANGLETERM   != null) { if(ANGLETERM  .Value == true) lkeys.Add("ANGLETERM  "); else lkeys.Add("ANGLETERM   NONE"); }
                if(UREYTERM    != null) { if(UREYTERM   .Value == true) lkeys.Add("UREYTERM   "); else lkeys.Add("UREYTERM    NONE"); }
                if(IMPROPTERM  != null) { if(IMPROPTERM .Value == true) lkeys.Add("IMPROPTERM "); else lkeys.Add("IMPROPTERM  NONE"); }
                if(TORSIONTERM != null) { if(TORSIONTERM.Value == true) lkeys.Add("TORSIONTERM"); else lkeys.Add("TORSIONTERM NONE"); }
                if(keys        != null) lkeys.AddRange(keys);

                return Testgrad(testgradpath, xyz, prm, tempbase, lkeys.ToArray(), optOutSource);

                //, "VDWTERM      NONE"
                //, "CHARGETERM   NONE"
                //, "BONDTERM     NONE"
                //, "ANGLETERM    NONE"
                //, "UREYTERM     NONE"
                //, "IMPROPTERM   NONE"
                //, "TORSIONTERM  NONE"
            }
            public static Tinker.Run.CTestgrad TestgradCached
                ( Tinker.Xyz xyz
                , Tinker.Prm prm
                , string[] keys
                , string xyzgradpath_txt     //= cachebase + "prot_solv_testgrad_output.txt";
                , string tinkerpath_testgrad //= "\"" + @"C:\Program Files\Tinker\bin-win64-8.2.1\testgrad.exe" + "\"";
                , string tempbase            //=null
                )
            {
                Tinker.Run.CTestgrad testgrad;

                if(HFile.Exists(xyzgradpath_txt) == false)
                {
                    Dictionary<string, string[]> optOutSource = new Dictionary<string, string[]>();
                    testgrad = Tinker.Run.Testgrad(tinkerpath_testgrad, xyz, prm, tempbase, keys
                        , optOutSource: optOutSource
                        );
                    HFile.WriteAllLines(xyzgradpath_txt, optOutSource["output.txt"]);
                }
                testgrad = Tinker.Run.ReadGrad(xyzgradpath_txt, null);

                return testgrad;
            }
            public static CTestgrad Testgrad(Tinker.Xyz xyz
                                            , Tinker.Prm prm
                                            , string tempbase //=null
                                            , string[] keys
                                            , Dictionary<string, string[]> optOutSource // = null
                                            )
            {
                return Testgrad
                    ( testgradpath  : null
                    , xyz           : xyz
                    , prm           : prm
                    , tempbase      : tempbase 
                    , keys          : keys
                    , optOutSource  : optOutSource
                    );
            }
            public static CTestgrad Testgrad( string testgradpath
                                            , Tinker.Xyz xyz
                                            , Tinker.Prm prm
                                            , string tempbase //=null
                                            , string[] keys
                                            , Dictionary<string, string[]> optOutSource // = null
                                            )
            {
                var tmpdir = HDirectory.CreateTempDirectory(tempbase);
                string currpath = HEnvironment.CurrentDirectory;
                CTestgrad testgrad;
                {
                    HEnvironment.CurrentDirectory = tmpdir.FullName;
                    if(testgradpath == null)
                    {
                        string resbase = "HTLib2.Bioinfo.HTLib2.Bioinfo.External.Tinker.Resources.tinker_6_2_06.";
                        HResource.CopyResourceTo<Tinker>(resbase+"testgrad.exe", "testgrad.exe");
                        testgradpath = "testgrad.exe";
                    }
                    xyz.ToFile("test.xyz", false);
                    prm.ToFile("test.prm");
                    string keypath = null;
                    if(keys != null && keys.Length > 0)
                    {
                        keypath = "test.key";
                        HFile.WriteAllLines(keypath, keys);
                    }
                    testgrad = TestgradImpl(testgradpath, "test.xyz", "test.prm", keypath, optOutSource);
                }
                HEnvironment.CurrentDirectory = currpath;
                try{ tmpdir.Delete(true); } catch {}
                return testgrad;
            }
            public static CTestgrad TestgradImpl
                ( string testgradpath
                , string xyzpath
                , string prmpath
                , string keypath //=null
                , Dictionary<string, string[]> optOutSource // = null
                )
            {
                {
                    bool ComputeAnalyticalGradientVector    = true;
                    bool ComputeNumericalGradientVector     = false;
                    bool OutputBreakdownByGradientComponent = false;
                    string command = "";
                    command += testgradpath; //command += GetProgramPath("testgrad.exe");
                    command += " " + xyzpath;
                    //command += " " + prmpath;
                    command += " " + (ComputeAnalyticalGradientVector    ? "Y" : "N");
                    command += " " + (ComputeNumericalGradientVector     ? "Y" : "N");
                    command += " " + (OutputBreakdownByGradientComponent ? "Y" : "N");
                    if(keypath != null) command += " -k "+keypath;
                    command += " > output.txt";
                    bool pause = false;
                    HProcess.StartAsBatchInConsole(null, pause, command);
                    //int exitcode = HProcess.StartAsBatchSilent(null, null, null, command);
                }

                return ReadGrad("output.txt", optOutSource);
            }
            public static CTestgrad ReadGrad
                ( string outputpath
                , Dictionary<string, string[]> optOutSource // = null
                )
            {
                string[] lines = HFile.ReadAllLines(outputpath);
                if(optOutSource != null)
                    optOutSource.Add("output.txt", lines);

                #region output.txt
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
                    ///  Total Potential Energy :              -3753.7592 Kcal/mole
                    /// 
                    ///  Potential Energy Breakdown by Individual Components :
                    /// 
                    ///   Energy      EB          EA          EBA         EUB         EAA         EOPB
                    ///   Terms       EOPD        EID         EIT         ET          EPT         EBT
                    ///               ETT         EV          EC          ECD         ED          EM
                    ///               EP          ER          ES          ELF         EG          EX
                    /// 
                    ///           134.1618    490.7835      0.0000     27.0850      0.0000      0.0000
                    ///             0.0000     22.5749      0.0000    644.4690      0.0000      0.0000
                    ///             0.0000   -645.8696  -4426.9638      0.0000      0.0000      0.0000
                    ///             0.0000      0.0000      0.0000      0.0000      0.0000      0.0000
                    /// 
                    ///  Cartesian Gradient Breakdown over Individual Atoms :
                    /// 
                    ///   Type      Atom              dE/dX       dE/dY       dE/dZ          Norm
                    /// 
                    ///  Anlyt         1            -0.2655     -0.4022      0.3265        0.5821
                    ///  Anlyt         2            -0.0118      0.1880     -0.2657        0.3257
                    ///  Anlyt         3             0.1314      0.0874     -0.0147        0.1585
                    ///  Anlyt         4             0.1543     -0.0348     -0.1401        0.2113
                    ///  Anlyt         5             0.0216      0.4618      0.4205        0.6250
                    ///  Anlyt         6            -0.0560     -0.0821     -0.0337        0.1050
                    /// ...
                    ///  Anlyt      2510             0.2699      0.2529      0.4340        0.5703
                    /// 
                    ///  Total Gradient Norm and RMS Gradient per Atom :
                    /// 
                    ///  Anlyt      Total Gradient Norm Value               27.5518
                    /// 
                    ///  Anlyt      RMS Gradient over All Atoms              0.5499
                    #endregion

                double potential = double.NaN;
                List<CTestgrad.Anlyt> analyts = new List<CTestgrad.Anlyt>();
                CTestgrad.EnrgCmpnt enrgCmpnt = null;
                for(int i=0; i<lines.Length; i++)
                {
                    string line = lines[i];
                    string[] tokens = line.Split().HRemoveAll("");
                    if(tokens.Length == 0)
                        continue;
                    int tmpint;
                    if(tokens[0] == "Anlyt" && int.TryParse(tokens[1], out tmpint))
                    {
                        CTestgrad.Anlyt analyt = new CTestgrad.Anlyt();
                        analyt.id   = int   .Parse(tokens[1]);  //if(int.TryParse(tokens[1], out analyt.id) == false) continue;
                        analyt.fx   = double.Parse(tokens[2]);  //if(double.TryParse(tokens[2], out analyt.fx) == false) continue;
                        analyt.fy   = double.Parse(tokens[3]);  //if(double.TryParse(tokens[3], out analyt.fy) == false) continue;
                        analyt.fz   = double.Parse(tokens[4]);  //if(double.TryParse(tokens[4], out analyt.fz) == false) continue;
                        analyt.norm = double.Parse(tokens[5]);  //if(double.TryParse(tokens[5], out analyt.norm) == false) continue;
                        analyts.Add(analyt);
                        continue;
                    }
                    if(tokens[0] == "Total" && tokens[1] == "Potential" && tokens[2] == "Energy")
                    {
                        double.TryParse(tokens[4], out potential);
                        continue;
                    }
                    if(line.Contains("Potential Energy Breakdown by Individual Components :"))
                    {
                        int ii=1;
                        if(lines[i+ii].Trim().Length != 0) continue;
                        ii++;

                        Dictionary<string,int> energy_term_sequence = new Dictionary<string, int>();
                        while(lines[i+ii].Trim().Length != 0)
                        {
                            string[] iitokens = lines[i+ii].Split().HRemoveAll("");
                            iitokens = iitokens.HRemoveAll("Energy");
                            iitokens = iitokens.HRemoveAll("Terms");
                            foreach(string iitoken in iitokens)
                                energy_term_sequence.Add(iitoken, energy_term_sequence.Count);
                            ii++;
                        }

                        if(lines[i+ii].Trim().Length != 0) continue;
                        ii++;

                        List<double> energy_value_sequence = new List<double>();

                        // find location of toker break
                        List<int> tokenbreak = new List<int>();
                        {
                            int jj = ii;
                            while(lines[i+jj].Trim().Length != 0)
                            {
                                string   iiline = lines[i+jj];
                                string[] iitokens = iiline.Split().HRemoveAll("");
                                foreach(string iitoken in iitokens)
                                {
                                    string iitoken2 = iitoken.Substring(0, iitoken.Length-1) + "@";
                                    HDebug.Assert(iitoken2.Length == iitoken.Length);
                                    HDebug.Assert(iitoken2.Last() == '@');
                                    iiline = iiline.Replace(iitoken, iitoken2);
                                }
                                for(int idx = 0; idx < iiline.Length; idx++)
                                    if(iiline[idx] == '@')
                                        tokenbreak.Add(idx);

                                jj++;
                            }
                            tokenbreak.Add(-1);
                            tokenbreak = tokenbreak.HToHashSet()
                                                   .ToList()
                                                   .HSort()
                                                   .ToList()
                                                   ;
                        }

                        while(lines[i+ii].Trim().Length != 0)
                        {
                            string   iiline = lines[i+ii];
                            //string[] iitokens = iiline.Split().HRemoveAll("");
                            List<string> iitokens = new List<string>();
                            {
                                string iiline2 = "";
                                for(int jj=0; jj<tokenbreak.Count-1; jj++)
                                {
                                    int idx0 = tokenbreak[jj]+1;
                                    int idx1 = tokenbreak[jj+1];
                                    if(idx1 >= iiline.Length)
                                        break;
                                    int leng = idx1 - idx0 + 1;
                                    iitokens.Add(iiline.Substring(idx0, leng));
                                    iiline2 += iitokens[jj];
                                }
                                HDebug.Assert(iiline == iiline2);
                            }
                            foreach(string iitoken in iitokens)
                            {
                                if(iitoken.Contains("*"))
                                {
                                    /// Handle "**********"
                                    ////  Energy       EB              EA              EBA             EUB
                                    ////  Terms        EAA             EOPB            EOPD            EID
                                    ////               EIT             ET              EPT             EBT
                                    ////               EAT             ETT             EV              EC
                                    ////               ECD             ED              EM              EP
                                    ////               ER              ES              ELF             EG
                                    ////               EX
                                    //// 
                                    ////        56384.95472959  41770.94471911      0.00000000      0.00000000
                                    ////            0.00000000      0.00000000      0.00000000      0.00000000
                                    ////            0.00000000      0.00000000      0.00000000      0.00000000
                                    ////            0.00000000      0.00000000 232979.82170784****************
                                    ////            0.00000000      0.00000000      0.00000000      0.00000000
                                    ////            0.00000000      0.00000000      0.00000000      0.00000000
                                    ////            0.00000000
                                    if(iitoken.StartsWith("*"))
                                    {
                                        HDebug.Assert(iitoken.EndsWith("*"));
                                        energy_value_sequence.Add(double.NaN);
                                    }
                                    else
                                    {
                                        HDebug.Assert(iitoken.EndsWith("*"));
                                        string liitoken = iitoken.Replace("*", "");
                                        energy_value_sequence.Add(double.Parse(liitoken));
                                        energy_value_sequence.Add(double.NaN);
                                    }
                                }
                                else
                                {
                                    energy_value_sequence.Add(double.Parse(iitoken));
                                }
                            }
                            ii++;
                        }

                        if(lines[i+ii].Trim().Length != 0) continue;
                        ii++;

                        if(energy_term_sequence.Count != energy_value_sequence.Count) continue;

                        #region format
                        ///0         1         2         3         4         5         6         7         |8
                        ///01234567890123456789012345678901234567890123456789012345678901234567890123456789|0
                        /// 
                        /// Potential Energy Breakdown by Individual Components :
                        ///
                        ///  Energy      EB          EA          EBA         EUB         EAA         EOPB
                        ///  Terms       EOPD        EID         EIT         ET          EPT         EBT
                        ///              ETT         EV          EC          ECD         ED          EM
                        ///              EP          ER          ES          ELF         EG          EX
                        ///
                        ///          134.0512    490.5575      0.0000     27.1282      0.0000      0.0000
                        ///            0.0000     22.6363      0.0000    775.1337      0.0000      0.0000
                        ///            0.0000************  -4039.6760      0.0000      0.0000      0.0000
                        ///            0.0000      0.0000      0.0000      0.0000      0.0000      0.0000
                        ///            
                        ///0         1         2         3         4         5         6         7         |8
                        ///01234567890123456789012345678901234567890123456789012345678901234567890123456789|0
                        ///
                        /// Potential Energy Breakdown by Individual Components :
                        ///
                        ///  Energy       EB              EA              EBA             EUB
                        ///  Terms        EAA             EOPB            EOPD            EID
                        ///               EIT             ET              EPT             EBT
                        ///               ETT             EV              EC              ECD
                        ///               ED              EM              EP              ER
                        ///               ES              ELF             EG              EX
                        ///
                        ///         1827.83562614    115.54269101      0.00000000    139.97155716
                        ///            0.00000000      0.00000000      0.00000000      6.08959854
                        ///            0.00000000    176.68828153      0.00000000      0.00000000
                        ///            0.00000000      0.00000000      0.00000000      0.00000000
                        ///            0.00000000      0.00000000      0.00000000      0.00000000
                        ///            0.00000000      0.00000000      0.00000000      0.00000000
                        #endregion

                        CTestgrad.EnrgCmpnt ec = new CTestgrad.EnrgCmpnt();
                        if(energy_term_sequence.ContainsKey("EB"  )) ec.EB   = energy_value_sequence[energy_term_sequence["EB"  ]];
                        if(energy_term_sequence.ContainsKey("EA"  )) ec.EA   = energy_value_sequence[energy_term_sequence["EA"  ]];
                        if(energy_term_sequence.ContainsKey("EBA" )) ec.EBA  = energy_value_sequence[energy_term_sequence["EBA" ]];
                        if(energy_term_sequence.ContainsKey("EUB" )) ec.EUB  = energy_value_sequence[energy_term_sequence["EUB" ]];
                        if(energy_term_sequence.ContainsKey("EAA" )) ec.EAA  = energy_value_sequence[energy_term_sequence["EAA" ]];
                        if(energy_term_sequence.ContainsKey("EOPB")) ec.EOPB = energy_value_sequence[energy_term_sequence["EOPB"]];
                        if(energy_term_sequence.ContainsKey("EOPD")) ec.EOPD = energy_value_sequence[energy_term_sequence["EOPD"]];
                        if(energy_term_sequence.ContainsKey("EID" )) ec.EID  = energy_value_sequence[energy_term_sequence["EID" ]];
                        if(energy_term_sequence.ContainsKey("EIT" )) ec.EIT  = energy_value_sequence[energy_term_sequence["EIT" ]];
                        if(energy_term_sequence.ContainsKey("ET"  )) ec.ET   = energy_value_sequence[energy_term_sequence["ET"  ]];
                        if(energy_term_sequence.ContainsKey("EPT" )) ec.EPT  = energy_value_sequence[energy_term_sequence["EPT" ]];
                        if(energy_term_sequence.ContainsKey("EBT" )) ec.EBT  = energy_value_sequence[energy_term_sequence["EBT" ]];
                        if(energy_term_sequence.ContainsKey("ETT" )) ec.ETT  = energy_value_sequence[energy_term_sequence["ETT" ]];
                        if(energy_term_sequence.ContainsKey("EV"  )) ec.EV   = energy_value_sequence[energy_term_sequence["EV"  ]];
                        if(energy_term_sequence.ContainsKey("EC"  )) ec.EC   = energy_value_sequence[energy_term_sequence["EC"  ]];
                        if(energy_term_sequence.ContainsKey("ECD" )) ec.ECD  = energy_value_sequence[energy_term_sequence["ECD" ]];
                        if(energy_term_sequence.ContainsKey("ED"  )) ec.ED   = energy_value_sequence[energy_term_sequence["ED"  ]];
                        if(energy_term_sequence.ContainsKey("EM"  )) ec.EM   = energy_value_sequence[energy_term_sequence["EM"  ]];
                        if(energy_term_sequence.ContainsKey("EP"  )) ec.EP   = energy_value_sequence[energy_term_sequence["EP"  ]];
                        if(energy_term_sequence.ContainsKey("ER"  )) ec.ER   = energy_value_sequence[energy_term_sequence["ER"  ]];
                        if(energy_term_sequence.ContainsKey("ES"  )) ec.ES   = energy_value_sequence[energy_term_sequence["ES"  ]];
                        if(energy_term_sequence.ContainsKey("ELF" )) ec.ELF  = energy_value_sequence[energy_term_sequence["ELF" ]];
                        if(energy_term_sequence.ContainsKey("EG"  )) ec.EG   = energy_value_sequence[energy_term_sequence["EG"  ]];
                        if(energy_term_sequence.ContainsKey("EX"  )) ec.EX   = energy_value_sequence[energy_term_sequence["EX"  ]];
                        enrgCmpnt = ec;
                    }
                }

                return new CTestgrad { potential=potential, enrgCmpnt=enrgCmpnt, anlyts=analyts.ToArray() };
            }
        }
    }
}
