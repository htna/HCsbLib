using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
    using Mutex = System.Threading.Mutex;
    using AbandonedMutexException = System.Threading.AbandonedMutexException;
    public partial class Matlab
	{
        public static void Call(string functionbody, string retval, params string[] parvals)
        {
            string currdir = HEnvironment.CurrentDirectory;
            {
                string funcpath = System.IO.Path.GetTempFileName();
                System.IO.File.WriteAllText(funcpath, functionbody);
                System.IO.FileInfo finfo = new System.IO.FileInfo(funcpath);
                HEnvironment.CurrentDirectory = finfo.DirectoryName;
                {
                    string command = finfo.Name + "(";
                    foreach(string parval in parvals)
                        command += "," + parval;
                    command = command.Replace("(,", "(");
                    command = command + ");";
                    if(retval != null)
                        command = retval + " = " + command;
                    Matlab.Execute(command);
                }
                System.IO.File.Delete(funcpath);
            }
            HEnvironment.CurrentDirectory = currdir;
        }
	}
}
