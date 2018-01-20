using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public static int RunPdb2gmx( string f            // -f    eiwit.pdb   Input           Structure file: gro g96 pdb tpr etc.  
                                    , string o     = null // -o    conf.gro    Output          Structure file: gro g96 pdb etc.  
                                    , string p     = null // -p    topol.top   Output          Topology file  
                                    , string i     = null // -i    posre.itp   Output          Include file for topology  
                                    , string n     = null // -n    clean.ndx   Output, Opt.    Index file  
                                    , string q     = null // -q    clean.pdb   Output, Opt.    Structure file: gro g96 pdb etc.  
                                    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                    // Other options      // option    type    default     description 
                                    , string merge = null // -merge    enum    no          Merge multiple chains into a single [moleculetype]: no, all or interactive
                                                          //                               no, all, interactive
                                    , string ff    = null // -ff       string  select      Force field, interactive by default. Use -h for information.
                                                          //                               "charmm27", ...
                                    , string water = null // -water    enum    select      Water model to use: select, none, spc, spce, tip3p, tip4p or tip5p
                                                          //                               select, none, spc, spce, tip3p, tip4p or tip5p
                                    , List<string> lineStderr = null
                                    , List<string> lineStdout = null
                                    , bool silent_run         = true
                                    )
        {
            List<string> commands = new List<string>();
            commands.Add("pdb2gmx_d");
            if(f     != null) commands.Add("  -f "     + f);
            if(o     != null) commands.Add("  -o "     + o);
            if(p     != null) commands.Add("  -p "     + p);
            if(i     != null) commands.Add("  -i "     + i);
            if(n     != null) commands.Add("  -n "     + n);
            if(q     != null) commands.Add("  -q "     + q);
            if(merge != null) commands.Add("  -merge " + merge);
            if(ff    != null) commands.Add("  -ff "    + ff);
            if(water != null) commands.Add("  -water " + water);

            string command = commands.HStrcat();
            int exitcode = Run(command, lineStderr:lineStderr, lineStdout:lineStdout, silent_run:silent_run);
            return exitcode;
        }
    }
}
