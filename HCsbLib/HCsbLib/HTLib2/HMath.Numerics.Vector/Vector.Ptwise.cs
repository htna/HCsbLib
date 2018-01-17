using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	public partial class Vector : ICloneable
	{
        public static Vector PtwiseAdd(Vector l, Vector r)
        {
            HDebug.Assert(l.Size == r.Size);
            Vector result = new double[l.Size];
            for(int i=0; i<result.Size; i++)
                result[i] = l[i] + r[i];
            return result;
        }
        public static Vector PtwiseSub(Vector l, Vector r)
        {
            HDebug.Assert(l.Size == r.Size);
            Vector result = new double[l.Size];
            for(int i=0; i<result.Size; i++)
                result[i] = l[i] - r[i];
            return result;
        }
        public static Vector PtwiseMul(Vector l, Vector r)
        {
            HDebug.Assert(l.Size == r.Size);
            Vector result = new double[l.Size];
            for(int i=0; i<result.Size; i++)
                result[i] = l[i] * r[i];
            return result;
        }
        public static Vector PtwiseDiv(Vector l, Vector r)
        {
            HDebug.Assert(l.Size == r.Size);
            Vector result = new double[l.Size];
            for(int i=0; i<result.Size; i++)
                result[i] = l[i] / r[i];
            return result;
        }
    }
}
