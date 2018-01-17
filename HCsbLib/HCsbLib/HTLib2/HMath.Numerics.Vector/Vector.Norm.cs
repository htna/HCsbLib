using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	public partial class Vector : ICloneable
	{
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
                norm += Math.Abs(_data[i]);
            return norm;
        }
        public double Norm2()
        {
            double norm = 0;
            for(int i=0; i<Size; i++)
                norm += _data[i]*_data[i];
            norm = Math.Sqrt(norm);
            return norm;
        }
        public double NormP(int p)
        {
            double norm = 0;
            for(int i=0; i<Size; i++)
                norm += Math.Pow(Math.Abs(_data[i]), p);
            norm = Math.Pow(norm, 1.0/p);
            return norm;
        }
        public double NormInf()
        {
            HDebug.Assert(Size >= 1);
            double norm = -1;
            for(int i=0; i<Size; i++)
                norm = Math.Max(norm, Math.Abs(_data[i]));
            return norm;
        }
    }
}
