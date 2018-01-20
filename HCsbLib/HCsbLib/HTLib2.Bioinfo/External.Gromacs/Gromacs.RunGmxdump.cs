using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public static int RunGmxdump( string capturepath
                                    , string s   = null // -s      topol.tpr       Input, Opt.     Run input file: tpr tpb tpa  
                                    , string f   = null // -f      traj.xtc        Input, Opt.     Trajectory: xtc trr trj gro g96 pdb cpt  
                                    , string e   = null // -e      ener.edr        Input, Opt.     Energy file  
                                    , string cp  = null // -cp     state.cpt       Input, Opt.     Checkpoint file  
                                    , string p   = null // -p      topol.top       Input, Opt.     Topology file  
                                    , string mtx = null // -mtx    hessian.mtx     Input, Opt.     Hessian matrix  
                                    , string om  = null // -om     grompp.mdp      Output, Opt.    grompp input file with MD parameter 
                                    , List<string> lineStderr = null
                                    , List<string> lineStdout = null
                                    , bool silent_run         = true
                                    )
        {
            List<string> commands = new List<string>();
            commands.Add("gmxdump_d");
            if(s   != null) commands.Add(" -s   " + s  );
            if(f   != null) commands.Add(" -f   " + f  );
            if(e   != null) commands.Add(" -e   " + e  );
            if(cp  != null) commands.Add(" -cp  " + cp );
            if(p   != null) commands.Add(" -p   " + p  );
            if(mtx != null) commands.Add(" -mtx " + mtx);
            if(om  != null) commands.Add(" -om  " + om );

            string command = commands.HStrcat();
            command = command + " > " + capturepath;
            int exitcode = Run(command, lineStderr:lineStderr, lineStdout:lineStdout, silent_run:silent_run);
            return exitcode;
        }
    }
}
