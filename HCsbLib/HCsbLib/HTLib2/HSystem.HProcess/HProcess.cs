using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HTLib2
{
    public partial class HProcess
    {
        public static int StartAsBatchInConsole(string batpath, bool pause, params string[] commands)
        {
            bool delbat = false;
            if((batpath == null) || batpath.Trim().Length == 0)
            {
                delbat = true;
                batpath = HFile.GetTempPath("bat");
            }

            HFile.WriteAllLines(batpath, commands);

            if(pause)
                HFile.AppendAllLines(batpath, new string[] { "pause" });

            System.Diagnostics.Process pdb2gmx = System.Diagnostics.Process.Start(batpath);
            pdb2gmx.WaitForExit();
            int exitcode = pdb2gmx.ExitCode;

            Thread.Sleep(100);

            if(delbat)
                HFile.Delete(batpath);

            return exitcode;
        }
        public static int StartInConsole(bool pause, string command_format, params object[] args)
        {
            string command;
            if(args.Length == 0) command = command_format;
            else                 command = string.Format(command_format, args);
            command = command.Trim();

            string fileName  = command.HSplit(' ').First();
            string arguments = command.Substring(fileName.Length).Trim();

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(fileName, arguments);
            process.WaitForExit();
            int exitcode = process.ExitCode;

            Thread.Sleep(100);

            return exitcode;
        }

        public static int StartAsBatchSilent(Tuple<string,IList<string>,IList<string>> env,
                                             List<string> lineStdout, List<string> lineStderr, params string[] commands)
        {
            string        tempbase  = null;
            IList<string> filesIn  = null;
            IList<string> filesOut = null;
            
            if(env != null)
            {
                tempbase  = env.Item1; HDebug.Assert(tempbase != null); HDebug.Assert(HDirectory.Exists(tempbase));
                filesIn  = env.Item2;
                filesOut = env.Item3;
            }
            string currdir = HEnvironment.CurrentDirectory;
            if(tempbase != null)
            {
                var dirinfo = HDirectory.CreateTempDirectory(tempbase);
                HEnvironment.CurrentDirectory = dirinfo.FullName;
            }
            if(filesIn != null)
            {
                foreach(string file in filesIn)
                {
                    HFile.Copy(currdir+"\\"+file, file);
                }
            }

            int exitcode;
            {
                string tmpbatpath = HFile.GetTempPath("bat");
                HFile.WriteAllLines(tmpbatpath, commands);

                {
                    if(lineStderr == null) lineStderr = new List<string>(); // collect into garbage storage if they are initially null.
                    if(lineStdout == null) lineStdout = new List<string>(); // collect into garbage storage if they are initially null.
                    // capture error message in console
                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo.FileName = tmpbatpath;
                    proc.StartInfo.RedirectStandardError  = (lineStderr != null);
                    proc.StartInfo.RedirectStandardOutput = (lineStdout != null);
                    proc.StartInfo.UseShellExecute = false;
                    proc.Start();
                    if(lineStderr != null) { lineStderr.Clear(); lineStderr.AddRange(proc.StandardError.ReadToEnd().Replace("\r", "").Split('\n')); }
                    if(lineStdout != null) { lineStdout.Clear(); lineStdout.AddRange(proc.StandardOutput.ReadToEnd().Replace("\r", "").Split('\n')); }
                    proc.WaitForExit();
                    exitcode = proc.ExitCode;
                }

                Thread.Sleep(100);
                HFile.Delete(tmpbatpath);
            }

            if(filesOut != null)
            {
                foreach(string file in filesOut)
                {
                    if(HFile.Exists(file))
                        HFile.Copy(file, currdir+"\\"+file);
                }
            }
            if(HEnvironment.CurrentDirectory != currdir)
            {
                string delpath = HEnvironment.CurrentDirectory;
                HEnvironment.CurrentDirectory = currdir;
                try{HDirectory.Delete(delpath,true);} catch(System.IO.IOException) { }
            }

            return exitcode;
        }
    }
}
