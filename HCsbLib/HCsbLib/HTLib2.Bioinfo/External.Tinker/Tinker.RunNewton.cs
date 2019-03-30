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
            public class ONewton
            {
                public Tinker.Xyz minxyz;
                public string[]   minlog;
            }
            public static ONewton Newton( Tinker.Xyz xyz
                                        , Tinker.Prm prm
                                        , string tempbase
                                        , string copytemp                   // = null
                                        , string param
                                        //, string precondition               // = "A"  // Precondition via Auto/None/Diag/Block/SSOR/ICCG [A] :  A
                                        //, double gradCriterion              // = 0.01 // Enter RMS Gradient per Atom Criterion [0.01] : 0.001
                                        , IList<Tinker.Xyz.Atom> atomsToFix // = null
                                        , bool pause                        // = false
                                        , params string[] keys
                                        )
            {
                return Newton
                    ( tinkerpath        : null
                    , xyz               : xyz
                    , xyz_atoms_format  : xyz.atoms_format
                    , prm               : prm
                    , tempbase          : tempbase
                    , copytemp          : copytemp
                    , param             : param
                    , atomsToFix        : atomsToFix
                    , pause             : pause
                    , keys              : keys
                    );
            }
            public static ONewton Newton( string tinkerpath
                                        , Tinker.Xyz xyz
                                        , Tinker.Xyz.Format xyz_atoms_format
                                        , Tinker.Prm prm
                                        , string tempbase
                                        , string copytemp                   // = null
                                        , string param
                                        //, string precondition               // = "A"  // Precondition via Auto/None/Diag/Block/SSOR/ICCG [A] :  A
                                        //, double gradCriterion              // = 0.01 // Enter RMS Gradient per Atom Criterion [0.01] : 0.001
                                        , IList<Tinker.Xyz.Atom> atomsToFix // = null
                                        , bool pause                        // = false
                                        , params string[] keys
                                        )
            {
                if(HDebug.IsDebuggerAttached && atomsToFix != null)
                {
                    Dictionary<int,Tinker.Xyz.Atom> xyzatoms = xyz.atoms.ToIdDictionary();
                    foreach(var atom in atomsToFix)
                        HDebug.Assert(object.ReferenceEquals(atom, xyzatoms[atom.Id]));
                }
                Tinker.Xyz minxyz;
                string[]   minlog;
                using(var temp=new HTempDirectory(tempbase, copytemp))
                {
                    temp.EnterTemp();

                    if(tinkerpath == null)
                    {
                        string resbase = "HTLib2.Bioinfo.HTLib2.Bioinfo.External.Tinker.Resources.tinker_6_2_06.";
                        HResource.CopyResourceTo<Tinker>(resbase+"newton.exe", "newton.exe");
                        tinkerpath = "newton.exe";
                    }
                    xyz.ToFile("prot.xyz", false);
                    prm.ToFile("prot.prm");
                    List<string> keylines = new List<string>(keys);
                    if(atomsToFix != null)
                    {
                        foreach(var atom in atomsToFix)
                        {
                            Vector coord   = atom.Coord;
                            double force_constant = 10000; // force constant in kcal/Å2 for the harmonic restraint potential
                            string keyline = string.Format("RESTRAIN-POSITION     {0}  {1}  {2}  {3}  {4}", atom.Id, coord[0], coord[1], coord[2], force_constant);
                            keylines.Add(keyline);
                        }
                    }
                    HFile.WriteAllLines("prot.key", keylines);
                    // Precondition via Auto/None/Diag/Block/SSOR/ICCG [A] :  A
                    // Enter RMS Gradient per Atom Criterion [0.01] : 0.001
                    //bool pause = false;
                    string command = tinkerpath;
                    command += string.Format("  prot.xyz  prot.prm");
                    command += string.Format("  -k  prot.key");
                    command += string.Format("  {0}", param);
                    HProcess.StartAsBatchInConsole("newton.bat", pause
                                                    , "time /t  >> prot.log"
                                                    , command//+" >> prot.log"
                                                    , "time /t  >> prot.log"
                                                    );
                    HDebug.Assert(HFile.Exists("prot.xyz_2"));
                    HDebug.Assert(HFile.Exists("prot.xyz_3") == false);
                    minxyz = Tinker.Xyz.FromFile("prot.xyz_2", false, xyz_atoms_format);
                    minlog = HFile.ReadAllLines("prot.log");
                    temp.QuitTemp();
                }

                return new ONewton
                {
                    minxyz = minxyz,
                    minlog = minlog
                };
            }
        }
    }
}
