using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public static int Run( bool silent_run         //= true
                             , bool? pause             //= null
                             , List<string> lineStderr //= null
                             , List<string> lineStdout //= null
                             //, string batPathToCopy    //= null
                             , params string[]  commands
                             )
        {
            return Run("", commands, lineStderr: lineStderr, lineStdout: lineStdout, silent_run: silent_run, pause: pause); //, batPathToCopy: batPathToCopy);
        }
        public static int Run( string command
                             , string[] commandsNext   = null
                             , List<string> lineStderr = null
                             , List<string> lineStdout = null
                             , bool silent_run         = true
                             , bool? pause             = null
                             //, string batPathToCopy    = null
                             )
        {
            List<string> lines = new List<string>();
            {

                lines.Add(@"set GMXLIB=C:\Program Files (x86)\Gromacs\share\gromacs\top");
                lines.Add(@"set GMXBIN=C:\Program Files (x86)\Gromacs\bin"              );
                lines.Add(@"set GMXDATA=C:\Program Files (x86)\Gromacs\share"           );
                lines.Add(@"set GMXLDLIB=C:\Program Files (x86)\Gromacs\lib"            );
                lines.Add(@"set GMXMAN=C:\Program Files (x86)\Gromacs\share\man"        );
                lines.Add(command                                                       );
                if(commandsNext != null) lines.AddRange(commandsNext);
            }

            int exitcode;
            //bool silent_run = true;
            if(silent_run)
            {
                // int StartAsBatchSilent(string batpath, List<string> lineStdout, List<string> lineStderr, params string[] commands)
              //exitcode = HProcess.StartAsBatchSilent(batPathToCopy, lineStdout, lineStderr, lines.ToArray());
                exitcode = HProcess.StartAsBatchSilent(null,         lineStdout, lineStderr, lines.ToArray());
            }
            else
            {
                // int StartAsBatchInConsole(string batpath, bool pause=false, params string[] commands)
              //exitcode = HProcess.StartAsBatchInConsole(batPathToCopy, (pause == null || pause == true), lines.ToArray());
                exitcode = HProcess.StartAsBatchInConsole(null,         (pause == null || pause == true), lines.ToArray());
            }

            return exitcode;
        }
    }
}
