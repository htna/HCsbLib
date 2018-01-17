using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
    using Mutex = System.Threading.Mutex;
    using AbandonedMutexException = System.Threading.AbandonedMutexException;
    public partial class Matlab
    {
        static string _path_matlab_env = "";
        static string _path_temporary = null;
        public static string path_htlib { get { return _path_matlab_env + @"\+htlib"; } }
        static Matlab()
        {
            //string envpath = typeof(Matlab).Assembly.Location + ".env.txt";
            //if(HFile.Exists(envpath) == false)
            //{
            //    System.Console.Write("Enter additional matlab path: ");
            //    string env = System.Console.ReadLine();
            //    HFile.WriteAllLines(envpath, new string[] { env });
            //}
            //string[] envs = HFile.ReadAllLines(envpath);
            //_path_matlab_env = envs[0];
            //Matlab.Execute(@"addpath('$$ADDITIONALPATH$$');", "$$ADDITIONALPATH$$", envs[0]);
        }

        //public static void Init(MLApp.MLAppClass matlab)
        //{
        //    Matlab.matlab = matlab;
        //}

        //public static void Execute(string[] scripts)
        //{
        //    StringBuilder script = new StringBuilder();
        //    foreach(string script_ in scripts)
        //        script.AppendLine(script_);
        //    Execute(script.ToString());
        //}
        //public static void Execute(string script)
        //{
        //    string msg = matlab.Execute(script);
        //    Debug.Assert(msg == "");
        //}
        public static string Execute(string script, params string[] replaces)
        {
            return Execute(script, false, replaces);
        }
        public static string Execute(string script, bool ignoreAssert, params string[] replaces)
        {
            string nscript = CompleteScript(script, replaces);
            string msg = matlab.Execute(nscript);
            if(ignoreAssert==false)
            {
                HDebug.AssertIf(!ignoreAssert, msg == "");
                if(msg != "")
                {
                    System.Console.Error.WriteLine("Matlab error:\n"+msg);
                    throw new HException(string.Format("exception at Matlab.Execute(\"{0}\")", script));
                }
            }
            return msg;
        }
        public static void Quit()
        {
            matlab.Quit();
        }
        public static string CompleteScript(string script, params string[] replaces)
        {
            HDebug.Assert(replaces.Length%2 == 0);
            for(int i=0; i<replaces.Length/2; i++)
            {
                string from = replaces[i*2+0];
                string to   = replaces[i*2+1];
                script = script.Replace(from, to);
            }
            return script;
        }
        //public static void PutFullMatrix(string name, double[,] real)
        //{
        //    System.Array arr_real = real;
        //    System.Array arr_imag = new double[real.GetLength(0), real.GetLength(1)];
        //    matlab.PutFullMatrix(name, "base", arr_real, arr_imag);
        //}
        //public static void PutFullMatrix(string name, double[] real)
        //{
        //    System.Array arr_real = real;
        //    System.Array arr_imag = new double[real.GetLength(0)];
        //    matlab.PutFullMatrix(name, "base", arr_real, arr_imag);
        //}
        //public static void PutFullMatrix(string name, double real)
        //{
        //    System.Array arr_real = new double[1] { real };
        //    System.Array arr_imag = new double[1];
        //    matlab.PutFullMatrix(name, "base", arr_real, arr_imag);
        //}
    }
}
