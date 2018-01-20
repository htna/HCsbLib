using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public static int RunGrompp( string f            // -f     grompp.mdp      Input           grompp input file with MD parameters  
                                   , string po    = null // -po    mdout.mdp       Output          grompp input file with MD parameters  
                                   , string c     = null // -c     conf.gro        Input           Structure file: gro g96 pdb tpr etc.  
                                   , string r     = null // -r     conf.gro        Input, Opt.     Structure file: gro g96 pdb tpr etc.  
                                   , string rb    = null // -rb    conf.gro        Input, Opt.     Structure file: gro g96 pdb tpr etc.  
                                   , string n     = null // -n     index.ndx       Input, Opt.     Index file  
                                   , string p     = null // -p     topol.top       Input           Topology file  
                                   , string pp    = null // -pp    processed.top   Output, Opt.    Topology file  
                                   , string o     = null // -o     topol.tpr       Output          Run input file: tpr tpb tpa  
                                   , string t     = null // -t     traj.trr        Input, Opt.     Full precision trajectory: trr trj cpt  
                                   , string e     = null // -e     ener.edr        Input, Opt.     Energy file  
                                   , string ref_  = null // -ref   rotref.trr      In/Out, Opt.    Full precision trajectory: trr trj cpt  
                                   , List<string> lineStderr = null
                                   , List<string> lineStdout = null
                                   , bool silent_run         = true
                                   )
        {
            List<string> commands = new List<string>();
            commands.Add("grompp_d");
            if(f    != null) commands.Add(" -f   " + f   );
            if(po   != null) commands.Add(" -po  " + po  );
            if(c    != null) commands.Add(" -c   " + c   );
            if(r    != null) commands.Add(" -r   " + r   );
            if(rb   != null) commands.Add(" -rb  " + rb  );
            if(n    != null) commands.Add(" -n   " + n   );
            if(p    != null) commands.Add(" -p   " + p   );
            if(pp   != null) commands.Add(" -pp  " + pp  );
            if(o    != null) commands.Add(" -o   " + o   );
            if(t    != null) commands.Add(" -t   " + t   );
            if(e    != null) commands.Add(" -e   " + e   );
            if(ref_ != null) commands.Add(" -ref " + ref_);

            string command = commands.HStrcat();
            int exitcode = Run(command, lineStderr:lineStderr, lineStdout:lineStdout, silent_run:silent_run);
            return exitcode;
        }
    }
}
