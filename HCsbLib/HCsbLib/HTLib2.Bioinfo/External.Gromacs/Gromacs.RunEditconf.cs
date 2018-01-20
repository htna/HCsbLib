using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        /// http://manual.gromacs.org/current/online/editconf.html
        public static int RunEditconf( string f             // -f     conf.gro      Input           Structure file: gro g96 pdb tpr etc.
                                     , string n     = null  // -n     index.ndx     Input, Opt.     Index file
                                     , string o     = null  // -o     out.gro       Output, Opt.    Structure file: gro g96 pdb etc.
                                     , string mead  = null  // -mead  mead.pqr      Output, Opt.    Coordinate file for MEAD
                                     , string bf    = null  // -bf    bfact.dat     Input, Opt.     Generic data file
                                     , List<string> lineStderr = null
                                     , List<string> lineStdout = null
                                     , bool silent_run         = true
                                     )
        {
            List<string> commands = new List<string>();
            commands.Add("editconf_d");
            if(f    != null) commands.Add(" -f "    + f   );
            if(n    != null) commands.Add(" -n "    + n   );
            if(o    != null) commands.Add(" -o "    + o   );
            if(mead != null) commands.Add(" -mead " + mead);
            if(bf   != null) commands.Add(" -bf "   + bf  );

            string command = commands.HStrcat();
            int exitcode = Run(command, lineStderr:lineStderr, lineStdout:lineStdout, silent_run:silent_run);
            return exitcode;
        }
    }
}
