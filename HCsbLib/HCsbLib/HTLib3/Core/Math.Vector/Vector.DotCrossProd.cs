using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib3
{
    public partial class Vector : Vector<double>
    {
        public static double DotProd(Vector v1, Vector v2)
        {
            if(Debug.SelftestDo())
            {
                Vector tv1 = new double[] { -1, 2, 3 };
                Vector tv2 = new double[] { 4, -5, 6 };
                double tdp = DotProd(tv1, tv2);
                Debug.AssertTolerant(0, tdp-4);
            }
            int size = v1.Size; Debug.Assert(v1.Size == v2.Size);
            double dot = 0;
            for(int i=0; i<size; i++)
                dot += v1[i]*v2[i];
            return dot;
        }
        public Matrix AlterDotProd()
        {
            return AlterDotProd(this, this);
        }
        public static Matrix AlterDotProd(Vector v1, Vector v2)
        {
            Matrix mat = new Matrix(v1.Size, v2.Size);
            for(int c=0; c<mat.ColSize; c++)
                for(int r=0; r<mat.RowSize; r++)
                    mat[c, r] = v1[c] * v2[r];
            return mat;
        }
        public static Vector CrossProd(Vector v1, Vector v2)
        {
            int size = v1.Size; Debug.Assert(v1.Size == v2.Size);
            if(size == 3) return CrossProd3(v1, v2);
            Debug.ToDo();
            return null;
        }
        static Vector CrossProd3(Vector v1, Vector v2)
        {
            Debug.Assert(v1.Size == 3);
            Debug.Assert(v2.Size == 3);
            if(Debug.SelftestDo())
            {
                Vector tv1 = new double[] { 1, 2, 3 };
                Vector tv2 = new double[] { 4, 5, 6 };
                Vector tv  = CrossProd3(tv1, tv2) ;
                Debug.AssertTolerant(0, tv - new Vector(-3, 6, -3));
            }
            Vector cro = new double[3];
            cro[0] = v1[1]*v2[2] - v1[2]*v2[1];
            cro[1] = v1[2]*v2[0] - v1[0]*v2[2];
            cro[2] = v1[0]*v2[1] - v1[1]*v2[0];
            return cro;
        }
    }
}
