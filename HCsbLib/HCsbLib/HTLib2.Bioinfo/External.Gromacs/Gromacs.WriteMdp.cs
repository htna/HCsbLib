using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public static void WriteMdpSteep(string mdppath, double emtol=5, long nsteps=500000)
        {
            WriteMdp(mdppath
                , integrator: "steep"
                , emtol: emtol
                , emstep: 0.01
                , nsteps: nsteps
                , nstlist: 10
                );
        }
        public static void WriteMdpLBFGS(string mdppath, double emtol=0.001, double emstep=0.001, long nsteps=5000000000, double rcoulomb_switch=0.7, double rvdw_switch=0.7)
        {
            WriteMdp(mdppath
                , integrator: "l-bfgs"
                , emtol: emtol
                , emstep: emstep
                , nsteps: nsteps
                , rcoulomb_switch: rcoulomb_switch
                , rvdw_switch: rvdw_switch
                );
        }

        public static void WriteMdpNM(string mdppath, double emtol=0.001, double emstep=0.001, long nsteps=5000000000, double rcoulomb_switch=0.7, double rvdw_switch=0.7)
        {
            WriteMdp(mdppath
                , integrator: "nm"
                , emtol: emtol
                , emstep: emstep
                , nsteps: nsteps
                , rcoulomb_switch: rcoulomb_switch
                , rvdw_switch: rvdw_switch
                );
        }

        public static void WriteMdp(string mdppath
                                   ,string integrator      // = "steep"
                                   ,double emtol           // = 5    // Stop minimization when the maximum force < 1.0 kJ/mol/nm
                                   ,double emstep          = 0.01    // Energy step size
                                   ,long   nsteps          = 500000  // Maximum number of (minimization) steps to perform
                                                                     // Parameters describing how to find the neighbors of each atom and how to calculate the interactions
                                   ,int    nstlist         = 1       // Frequency to update the neighbor list and long range forces
                                   ,string ns_type         = "grid"  // Method to determine neighbor list (simple, grid)
                                   ,double rlist           = 1.0     // Cut-off for making neighbor list (short range forces) ; htna 1.0-1.2
                                   ,string coulombtype     = "Shift"
                                   ,double rcoulomb        = 1.0
                                   ,double rcoulomb_switch = 0.0
                                   ,string vdwtype         = "Shift"
                                   ,double rvdw            = 1.0
                                   ,double rvdw_switch     = 0.0
                                   )
        {
            string[] lines = new string[]
            {
                "; Parameters describing what to do, when to stop and what to save                                                               ",
                "integrator      = $$integrator$$                                                                                                             ",
                "emtol           = $$emtol$$              ; Stop minimization when the maximum force < 1.0 kJ/mol/nm                                          ",
                "emstep          = $$emstep$$             ; Energy step size                                                                                  ",
                "nsteps          = $$nsteps$$             ; Maximum number of (minimization) steps to perform                                                 ",
                "                                         ; Parameters describing how to find the neighbors of each atom and how to calculate the interactions",
                "nstlist         = $$nstlist$$            ; Frequency to update the neighbor list and long range forces                                       ",
                "ns_type         = $$ns_type$$            ; Method to determine neighbor list (simple, grid)                                                  ",
                "rlist           = $$rlist$$              ; Cut-off for making neighbor list (short range forces) ; htna 1.0-1.2                              ",
                "coulombtype     = $$coulombtype$$                                                                                                            ",
                "rcoulomb        = $$rcoulomb$$                                                                                                               ",
                "rcoulomb_switch = $$rcoulomb_switch$$                                                                                                        ",
                "vdwtype         = $$vdwtype$$                                                                                                                ",
                "rvdw            = $$rvdw$$                                                                                                                   ",
                "rvdw_switch     = $$rvdw_switch$$                                                                                                            ",
            }
            .HReplace("$$integrator$$     ", integrator     )
            .HReplace("$$emtol$$          ", emtol          )
            .HReplace("$$emstep$$         ", emstep         )
            .HReplace("$$nsteps$$         ", nsteps         )
            .HReplace("$$nstlist$$        ", nstlist        )
            .HReplace("$$ns_type$$        ", ns_type        )
            .HReplace("$$rlist$$          ", rlist          )
            .HReplace("$$coulombtype$$    ", coulombtype    )
            .HReplace("$$rcoulomb$$       ", rcoulomb       )
            .HReplace("$$rcoulomb_switch$$", rcoulomb_switch)
            .HReplace("$$vdwtype$$        ", vdwtype        )
            .HReplace("$$rvdw$$           ", rvdw           )
            .HReplace("$$rvdw_switch$$    ", rvdw_switch    )
            ;
            HFile.WriteAllLines(mdppath, lines);
        }
    }
}
