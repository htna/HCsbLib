using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Vector : Vector<double>
    {
        public double Dist2
        {
            get { double dist2 = 0; for(int i=0; i<Size; i++) dist2 += data[i]*data[i]; return dist2; }
        }
        public double Dist
        {
            get { return Math.Sqrt(Dist2); }
        }

        public double Norm(int p)
        {
            if(p == 1) return Norm1();
            if(p == 2) return Norm2();
            return NormP(p);
        }
        public double Norm1()
        {
            double norm = 0;
            for(int i=0; i<Size; i++)
                norm += Math.Abs(data[i]);
            return norm;
        }
        public double Norm2()
        {
            double norm = 0;
            for(int i=0; i<Size; i++)
                norm += data[i]*data[i];
            norm = Math.Sqrt(norm);
            return norm;
        }
        public double NormP(int p)
        {
            double norm = 0;
            for(int i=0; i<Size; i++)
                norm += Math.Pow(Math.Abs(data[i]), p);
            norm = Math.Pow(norm, 1.0/p);
            return norm;
        }
        public double NormInf()
        {
            Debug.Assert(Size >= 1);
            double norm = -1;
            for(int i=0; i<Size; i++)
                norm = Math.Max(norm, Math.Abs(data[i]));
            return norm;
        }

        public Matrix ToDiagMatrix()
        {
            Matrix mat = new Matrix(Size, Size);
            for(int i=0; i<Size; i++)
                mat[i, i] = this[i];
            return mat;
        }
    }
}
