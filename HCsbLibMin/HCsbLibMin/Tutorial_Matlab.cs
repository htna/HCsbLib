using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HCsbLib
{
    partial class Program
    {
        public class Tutorial_Matlab
        {
            public static void Main(string pathbase, string pdbbase, string[] args)
            {
                double[,] A = new double[3, 3];
                A[0, 0] = 1;
                A[1, 1] = 2;
                A[2, 2] = 3;
                Matlab.PutMatrix("A", A);
                Matlab.Execute("B = pinv(A);");
                double[,] B = Matlab.GetMatrix("B");
            }
        }
    }
}
