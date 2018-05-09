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
            public class OMinimize
            {
                public Tinker.Xyz minxyz;
                public string[]   minlog;
            }
            public static OMinimize Minimize(Tinker.Xyz xyz
                                        , Tinker.Prm prm
                                        , string tempbase
                                        , string copytemp                   // = null
                                        , string param
                                        , IList<Tinker.Xyz.Atom> atomsToFix // = null
                                        , bool pause                        // = false
                                        , params string[] keys
                                        )
            {
                return Minimize
                    ( xyz
                    , xyz.atoms_format
                    , prm
                    , tempbase
                    , copytemp                   // = null
                    , param
                    , atomsToFix // = null
                    , pause                        // = false
                    , keys
                    );
            }
            public static OMinimize Minimize(Tinker.Xyz xyz
                                        , Tinker.Xyz.Atom.Format xyz_atoms_format
                                        , Tinker.Prm prm
                                        , string tempbase
                                        , string copytemp                   // = null
                                        , string param
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
                    xyz.ToFile("prot.xyz", false);
                    prm.ToFile("prot.prm");
                    List<string> keylines = new List<string>();
                    //if(grdmin != null)
                    //{
                    //    string keyline = string.Format("GRDMIN                {0}", grdmin.Value);
                    //    keylines.Add(keyline);
                    //}
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
                    if(keys != null)
                        keylines.AddRange(keys);
                    HFile.WriteAllLines("prot.key", keylines);
                    // Enter RMS Gradient per Atom Criterion [0.01] :
                    string command = Tinker.Run.GetProgramPath("minimize");
                    command += string.Format("  prot.xyz  prot.prm");
                    command += string.Format("  -k  prot.key");
                    command += string.Format("  {0}", param);
                    //command += string.Format("  >> prot.log");
                    HProcess.StartAsBatchInConsole("minimize.bat", pause
                                                    , "time /t >> prot.log"
                                                    , command
                                                    , "time /t >> prot.log"
                                                    );
                    HDebug.Assert(HFile.Exists("prot.xyz_2"));
                    HDebug.Assert(HFile.Exists("prot.xyz_3") == false);
                    minxyz = Tinker.Xyz.FromFile("prot.xyz_2", false, xyz.atoms_format);
                    minlog = HFile.ReadAllLines("prot.log");
                    temp.QuitTemp();
                }

                return new OMinimize
                {
                    minxyz = minxyz,
                    minlog = minlog
                };
            }
        }
    }
}
