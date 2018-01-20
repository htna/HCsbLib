using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public partial class BatchRun
        {
            public static Vector[] MinimizeLBfgs( IList<Pdb.Atom> atoms
                                                , IList<Vector>   coords = null
                                                , string ff    = null // -ff          string select  Force field, interactive by default. Use -h for information.
                                                                      //              "charmm27", ...
                                                , string water = null // -water       enum   select  Water model to use: select, none, spc, spce, tip3p, tip4p or tip5p
                                                , double mdp_emtol = 0.001 // the minimization is converged when the maximum force is smaller than this value
                                                , double mdp_emstep = 0.001 // initial step-size
                                                )
            {
                Vector[] confout;
                {
                    string currdir = HEnvironment.CurrentDirectory;
                    System.IO.DirectoryInfo temproot = HDirectory.CreateTempDirectory();
                    HEnvironment.CurrentDirectory = temproot.FullName;
                    {
                        if(coords != null) HDebug.Assert(atoms.Count == coords.Count);
                        Pdb.ToFile("conf.pdb", atoms, coords: coords
                                  , headers: new string[]{"CRYST1                                                                          "}
                                  );
                        HFile.WriteAllLines("grompp-nm.mdp"
                                          ,new string[]{"; Parameters describing what to do, when to stop and what to save                                                                    "
                                                       ,"integrator      = l-bfgs                                                                                                             "
                                                       ,"emtol           = 0.001                                                                                                              "
                                                       ,"emstep          = 0.001      ; Energy step size                                                                                      "
                                                       ,"nsteps          = 5000000000 ; Maximum number of (minimization) steps to perform                                                              "
                                                       ,"                             ; Parameters describing how to find the neighbors of each atom and how to calculate the interactions    "
                                                       ,"nstlist         = 1          ; Frequency to update the neighbor list and long range forces                                           "
                                                       ,"ns_type         = grid       ; Method to determine neighbor list (simple, grid)                                                      "
                                                       ,"rlist           = 1.0        ; Cut-off for making neighbor list (short range forces) ; htna 1.0-1.2                                  "
                                                       ,"coulombtype     = Shift                                                                                                              "
                                                       ,"rcoulomb        = 1.0                                                                                                                "
                                                       ,"rcoulomb_switch = 0.7                                                                                                                "
                                                       ,"vdwtype         = Shift                                                                                                              "
                                                       ,"rvdw            = 1.0                                                                                                                "
                                                       ,"rvdw_switch     = 0.7                                                                                                                "
                                                       //,"                                                                                                                                     "
                                                       //,"                                                                                                                                     "
                                                       //,"nstxout         = 1; (100) [steps]  frequency to write coordinates to output trajectory file, the last coordinates are always written" 
                                                       //,"nstvout         = 1; (100) [steps]  frequency to write velocities to output trajectory, the last velocities are always written       "
                                                       //,"nstfout         = 1; (  0) [steps]  frequency to write forces to output trajectory.                                                  "
                                          });

                        RunPdb2gmx( f: "conf.pdb"
                                  , o: "confout.pdb"
                                  , p: "topol.top"
                                  , i: "posre.itp"
                                  , n: "clean.ndx"
                                  , q: "clean.pdb"
                                  , ff: "charmm27"
                                  , water: "none"
                                  , merge: "all"
                                  , lineStderr: null
                                  , lineStdout: null
                                  );

                        RunGrompp( f:"grompp-nm.mdp"
                                 , p:"topol.top"
                                 , o:"topol-nm.tpr"
                                 , c:"confout.pdb"
                                 , lineStderr: null
                                 , lineStdout: null
                                 );


                        RunMdrun( s:"topol-nm.tpr"
                                , c:"confout.pdb"
                                , o:"traj.trr"
                                , g:"md.log"
                                //, mtx:"hessian.mtx"
                                //, pf:"pullf.xvg"
                                //, px:"pullx.xvg"
                                //, nt:"3"
                                , lineStderr: null
                                , lineStdout: null
                                );

                        Pdb pdbout = Pdb.FromFile("confout.pdb");
                        confout = pdbout.atoms.ListCoord().ToArray();
                    }
                    HEnvironment.CurrentDirectory = currdir;
                    Thread.Sleep(100);
                    try{ temproot.Delete(true); } catch(Exception){}
                }

                HDebug.Assert(confout.Length == atoms.Count);
                return confout;
            }
        }
    }
}
