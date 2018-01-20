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
            public static string GetProgramPath(string program)
            {
                string path = Tinker.tinkerpath + program;
                //     path = @"C:\Program Files\Tinker\tinker-6.3.2\bin-win64\"+program;
                path = "\""+path+"\"";
                return path;
            }

            public static int Exec(string command, params string[] parameters)
            {
                HDebug.ToDo();

                command = GetProgramPath(command);
                foreach(string parameter in parameters)
                    command += " " + parameter;
                //int exitcode = HtProcess.StartAsBatchSilent(null, null, null, command);
                int exitcode = HProcess.StartAsBatchInConsole(null, true, command);

                HDebug.Assert(false);
                return -1;
            }
        }
    }
}
