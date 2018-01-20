using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public static int RunMdrun( string s             // -s          topol.tpr       Input           Run input file: tpr tpb tpa  
                                  , string o      = null // -o          traj.trr        Output          Full precision trajectory: trr trj cpt  
                                  , string x      = null // -x          traj.xtc        Output, Opt.    Compressed trajectory (portable xdr format)  
                                  , string cpi    = null // -cpi        state.cpt       Input, Opt.     Checkpoint file  
                                  , string cpo    = null // -cpo        state.cpt       Output, Opt.    Checkpoint file  
                                  , string c      = null // -c          confout.gro     Output          Structure file: gro g96 pdb etc.  
                                  , string e      = null // -e          ener.edr        Output          Energy file  
                                  , string g      = null // -g          md.log          Output          Log file  
                                  , string dhdl   = null // -dhdl       dhdl.xvg        Output, Opt.    xvgr/xmgr file  
                                  , string field  = null // -field      field.xvg       Output, Opt.    xvgr/xmgr file  
                                  , string table  = null // -table      table.xvg       Input, Opt.     xvgr/xmgr file  
                                  , string tablep = null // -tablep     tablep.xvg      Input, Opt.     xvgr/xmgr file  
                                  , string tableb = null // -tableb     table.xvg       Input, Opt.     xvgr/xmgr file  
                                  , string rerun  = null // -rerun      rerun.xtc       Input, Opt.     Trajectory: xtc trr trj gro g96 pdb cpt  
                                  , string tpi    = null // -tpi        tpi.xvg         Output, Opt.    xvgr/xmgr file  
                                  , string tpid   = null // -tpid       tpidist.xvg     Output, Opt.    xvgr/xmgr file  
                                  , string ei     = null // -ei         sam.edi         Input, Opt.     ED sampling input  
                                  , string eo     = null // -eo         sam.edo         Output, Opt.    ED sampling output  
                                  , string j      = null // -j          wham.gct        Input, Opt.     General coupling stuff  
                                  , string jo     = null // -jo         bam.gct         Output, Opt.    General coupling stuff  
                                  , string ffout  = null // -ffout      gct.xvg         Output, Opt.    xvgr/xmgr file  
                                  , string devout = null // -devout     deviatie.xvg    Output, Opt.    xvgr/xmgr file  
                                  , string runav  = null // -runav      runaver.xvg     Output, Opt.    xvgr/xmgr file  
                                  , string px     = null // -px         pullx.xvg       Output, Opt.    xvgr/xmgr file  
                                  , string pf     = null // -pf         pullf.xvg       Output, Opt.    xvgr/xmgr file  
                                  , string mtx    = null // -mtx        nm.mtx          Output, Opt.    Hessian matrix  
                                  , string dn     = null // -dn         dipole.ndx      Output, Opt.    Index file  
                                  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                  // Other options         // option   type    default     description 
                                  , string nt     = null   // -nt      int     0           Number of threads to start (0 is guess)  
                                  //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                  , string etc    = null
                                  , List<string> lineStderr = null
                                  , List<string> lineStdout = null
                                  , bool silent_run         = true
                                  )
        {
            List<string> commands = new List<string>();
            commands.Add("mdrun_d");
            if(s      != null) commands.Add(" -s      " + s     );
            if(o      != null) commands.Add(" -o      " + o     );
            if(x      != null) commands.Add(" -x      " + x     );
            if(cpi    != null) commands.Add(" -cpi    " + cpi   );
            if(cpo    != null) commands.Add(" -cpo    " + cpo   );
            if(c      != null) commands.Add(" -c      " + c     );
            if(e      != null) commands.Add(" -e      " + e     );
            if(g      != null) commands.Add(" -g      " + g     );
            if(dhdl   != null) commands.Add(" -dhdl   " + dhdl  );
            if(field  != null) commands.Add(" -field  " + field );
            if(table  != null) commands.Add(" -table  " + table );
            if(tablep != null) commands.Add(" -tablep " + tablep);
            if(tableb != null) commands.Add(" -tableb " + tableb);
            if(rerun  != null) commands.Add(" -rerun  " + rerun );
            if(tpi    != null) commands.Add(" -tpi    " + tpi   );
            if(tpid   != null) commands.Add(" -tpid   " + tpid  );
            if(ei     != null) commands.Add(" -ei     " + ei    );
            if(eo     != null) commands.Add(" -eo     " + eo    );
            if(j      != null) commands.Add(" -j      " + j     );
            if(jo     != null) commands.Add(" -jo     " + jo    );
            if(ffout  != null) commands.Add(" -ffout  " + ffout );
            if(devout != null) commands.Add(" -devout " + devout);
            if(runav  != null) commands.Add(" -runav  " + runav );
            if(px     != null) commands.Add(" -px     " + px    );
            if(pf     != null) commands.Add(" -pf     " + pf    );
            if(mtx    != null) commands.Add(" -mtx    " + mtx   );
            if(dn     != null) commands.Add(" -dn     " + dn    );
            if(etc    != null) commands.Add(" "+etc);

            string command = commands.HStrcat();
            int exitcode = Run(command, lineStderr:lineStderr, lineStdout:lineStdout, silent_run:silent_run);
            return exitcode;
        }
    }
}
