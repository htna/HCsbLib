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

        public static (double,Vector)[] Eig(IMatrix<double> mat)
        {
            if(HDebug.Selftest())
            {
                Matrix _mat = new double[,]
                { { 2,  6,  6 },
                  { 6, 10, 12 },
                  { 6, 12, 18 } };
                PutMatrix("HTLib2_Matlab_Eig.mat", _mat);
                Execute  ("[HTLib2_Matlab_Eig.V, HTLib2_Matlab_Eig.D] = eig(HTLib2_Matlab_Eig.mat);");
                Execute  ("HTLib2_Matlab_Eig.D = diag(HTLib2_Matlab_Eig.D);");
                double[]  _eigvals = GetVector("HTLib2_Matlab_Eig.D");
                double[,] _eigvec  = GetMatrix("HTLib2_Matlab_Eig.V");
                Execute  ("clear HTLib2_Matlab_Eig;");

                var _eig = Eig(_mat);

                HDebug.Assert(_eigvals[0] == _eig[0].Item1);
                HDebug.Assert(_eigvals[1] == _eig[1].Item1);
                HDebug.Assert(_eigvals[2] == _eig[2].Item1);
                HDebug.Assert( _eigvec[0,0] == _eig[0].Item2[0]  &&  _eigvec[0,1] == _eig[1].Item2[0]  &&  _eigvec[0,2] == _eig[2].Item2[0] );
                HDebug.Assert( _eigvec[1,0] == _eig[0].Item2[1]  &&  _eigvec[1,1] == _eig[1].Item2[1]  &&  _eigvec[1,2] == _eig[2].Item2[1] );
                HDebug.Assert( _eigvec[2,0] == _eig[0].Item2[2]  &&  _eigvec[2,1] == _eig[1].Item2[2]  &&  _eigvec[2,2] == _eig[2].Item2[2] );
            }

            PutMatrix("HTLib2_Matlab_Eig.mat", mat);
            Execute  ("[HTLib2_Matlab_Eig.V, HTLib2_Matlab_Eig.D] = eig(HTLib2_Matlab_Eig.mat);");
            Execute  ("HTLib2_Matlab_Eig.D = diag(HTLib2_Matlab_Eig.D);");
            double[]  eigvals = GetVector("HTLib2_Matlab_Eig.D");
            double[,] eigvec  = GetMatrix("HTLib2_Matlab_Eig.V");
            Execute  ("clear HTLib2_Matlab_Eig;");

            Vector[]  eigvecs = eigvec.GetColVectorList();

            (double,Vector)[] eigs = new (double,Vector)[eigvals.Length];
            for(int i=0; i<eigvals.Length; i++)
                eigs[i] = (eigvals[i], eigvecs[i]);

            return eigs;
        }
    }
}
