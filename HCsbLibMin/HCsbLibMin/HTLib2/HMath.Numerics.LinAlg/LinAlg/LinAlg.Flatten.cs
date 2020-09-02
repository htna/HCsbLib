using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public static partial class LinAlg
	{
        static bool selftest_Flatten = HDebug.IsDebuggerAttached;
        public static Vector Flatten(this MatrixByArr mat)
        {
            #region self-test
            if(HDebug.IsDebuggerAttached && selftest_Flatten)
            {
                selftest_Flatten = false;
                MatrixByArr tmat = new double[,] { { 1, 2 }, { 3, 4 } };
                Vector vec0 = new double[] { 1, 2, 3, 4 };
                Vector vec1 = tmat.Flatten();
                HDebug.AssertAnd(vec0 == vec1);
            }
            #endregion
            int ColSize = mat.ColSize;
            int RowSize = mat.RowSize;
            Vector vec = new Vector(ColSize * RowSize);
            for(int i=0; i<vec.Size; i++)
            {
                int c = i / ColSize;
                int r = i % ColSize;
                vec[i] = mat[c, r];
            }
            return vec;
        }
        public static Vector FlattenRow(this MatrixByArr mat) /* <summary> flatten toward row    direction </summary>*/ { return mat.Flatten(); }
        public static Vector FlattenCol(this MatrixByArr mat) /* <summary> flatten toward column direction </summary>*/ { return mat.Tr().Flatten(); }
    }
}
