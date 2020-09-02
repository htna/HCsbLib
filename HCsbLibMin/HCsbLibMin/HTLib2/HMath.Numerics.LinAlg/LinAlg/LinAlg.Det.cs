using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
    {
        public static double? Det(this MatrixByArr mat)
        {
            if(mat.ColSize == 2 && mat.RowSize == 2) return Det2(mat);
            if(mat.ColSize == 3 && mat.RowSize == 3) return Det3(mat);
            if(mat.ColSize == 4 && mat.RowSize == 4) return Det4(mat);
            HDebug.Assert(false);
            return null;
        }
        public static double? Det(double[,] mat)
        {
            if(mat.GetLength(0) == 2 && mat.GetLength(1) == 2) return Det2(mat);
            if(mat.GetLength(0) == 3 && mat.GetLength(1) == 3) return Det3(mat);
            if(mat.GetLength(0) == 4 && mat.GetLength(1) == 4) return Det4(mat);
            HDebug.Assert(false);
            return null;
        }
        public static double Det2(MatrixByArr mat)
        {
            HDebug.Assert(mat.ColSize == 2, mat.RowSize == 2);
            double a=mat[0, 0], b=mat[0, 1];
            double c=mat[1, 0], d=mat[1, 1];
            double det = a*d - b*c;
            return det;
        }
        public static double Det3(MatrixByArr mat)
        {
            HDebug.Assert(mat.ColSize == 3, mat.RowSize == 3);
            double a1=mat[0,0], a2=mat[0,1], a3=mat[0,2];
            double b1=mat[1,0], b2=mat[1,1], b3=mat[1,2];
            double c1=mat[2,0], c2=mat[2,1], c3=mat[2,2];
            double det = + a1*b2*c3 - a1*b3*c2
                         + a2*b3*c1 - a2*b1*c3
                         + a3*b1*c2 - a3*b2*c1;
            return det;
        }
        public static double Det4(MatrixByArr mat)
        {
            HDebug.Assert(mat.ColSize == 4, mat.RowSize == 4);
            double a1=mat[0, 0], a2=mat[0, 1], a3=mat[0, 2], a4=mat[0, 3];
            double b1=mat[1, 0], b2=mat[1, 1], b3=mat[1, 2], b4=mat[1, 3];
            double c1=mat[2, 0], c2=mat[2, 1], c3=mat[2, 2], c4=mat[2, 3];
            double d1=mat[3, 0], d2=mat[3, 1], d3=mat[3, 2], d4=mat[3, 3];
            double det = a4*b3*c2*d1 - a3*b4*c2*d1 - a4*b2*c3*d1 + a2*b4*c3*d1
                       + a3*b2*c4*d1 - a2*b3*c4*d1 - a4*b3*c1*d2 + a3*b4*c1*d2
                       + a4*b1*c3*d2 - a1*b4*c3*d2 - a3*b1*c4*d2 + a1*b3*c4*d2
                       + a4*b2*c1*d3 - a2*b4*c1*d3 - a4*b1*c2*d3 + a1*b4*c2*d3
                       + a2*b1*c4*d3 - a1*b2*c4*d3 - a3*b2*c1*d4 + a2*b3*c1*d4
                       + a3*b1*c2*d4 - a1*b3*c2*d4 - a2*b1*c3*d4 + a1*b2*c3*d4;
            return det;
        }
    }
}
