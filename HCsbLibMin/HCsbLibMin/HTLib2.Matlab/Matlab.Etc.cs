using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace HTLib2
{
	public partial class Matlab
	{
		public static double Det(MatrixByArr mat)
		{
			PutMatrix("htlib2_matlab_det", mat.ToArray());
			double det = GetValue("det(htlib2_matlab_det)");
			Execute("clear htlib2_matlab_det;");
			return det;
		}
		public static void Clear()
		{
            Execute("clear;");
        }
		public static void Clear(string name)
		{
			Execute("clear "+name+";");
		}
        public static string Pwd()
        {
            return new string(Matlab.GetVectorChar("pwd"));
        }
        public static void Cd(string path)
        {
            Matlab.Execute("cd '"+path+"';");
        }
        public static bool Exist(string name, HPack<int> existvalue)
        {
            int value = GetValueInt("exist('"+name+"')");
            if(existvalue != null)
                existvalue.value = value;
            if(value == 0)
                return false;
            return true;
        }
        public static Dictionary<string, Mutex> _NamedLocks = new Dictionary<string, Mutex>();
        public static void NamedLockWait(string name)
        {
            // refer "public static TResult LockedCall<TResult>(string name, Func<TResult> func)"
            while(true)
            {
                try
                {
                    var mutex = new Mutex(false, name);
                    mutex.WaitOne();
                    HDebug.Assert(_NamedLocks.ContainsKey(name) == false);
                    _NamedLocks.Add(name, mutex);
                    break;
                }
                catch(AbandonedMutexException)
                {
                    // repeat if the exception (by closing another program using this mutex is closed by ctrl-c) happens.
                }
            }
        }
        public static void NamedLockRelease(string name)
        {
            HDebug.Assert(_NamedLocks.ContainsKey(name) == false);
            Mutex mutex = _NamedLocks[name];
            try
            {
                mutex.ReleaseMutex();
            }
            catch(AbandonedMutexException)
            {
                HDebug.Assert(false);
            }
            HDebug.Verify(_NamedLocks.Remove(name));
        }
    }
}
