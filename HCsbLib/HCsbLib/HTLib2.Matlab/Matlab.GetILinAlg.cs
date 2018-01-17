using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
    public partial class Matlab
	{
        public static ILinAlg ila { get { return CLinAlg._la; } }
        public class CLinAlg : ILinAlg
        {
            public static CLinAlg _la = new CLinAlg();

            public class CMatrix : ILinAlgMat
            {
                public Matrix matrix;
                public CMatrix(Matrix matrix) { this.matrix = matrix; }
                public override void Dispose() { }

                public override ILinAlg la { get { return _la; } }
                public override int ColSize { get { return matrix.ColSize; } }
                public override int RowSize { get { return matrix.RowSize; } }
                public override double this[int c, int r]
                {
                    get { return matrix[c, r]; }
                    set { matrix[c, r] = value; }
                }
                public override ILinAlgMat Tr { get { return new CMatrix(matrix.Tr()); } }
                public override double[,] ToArray() { return matrix.ToArray(); }
                public override ILinAlgMat op_Add(/*ILinAlgMat mat1, */ ILinAlgMat mat2) { return new CMatrix(matrix + (mat2 as CMatrix).matrix); }
                public override ILinAlgMat op_Sub(/*ILinAlgMat mat1, */ ILinAlgMat mat2) { return new CMatrix(matrix - (mat2 as CMatrix).matrix); }
                public override ILinAlgMat op_Mul(/*ILinAlgMat mat1, */ ILinAlgMat mat2) { return new CMatrix(matrix * (mat2 as CMatrix).matrix); }
                public override ILinAlgMat op_Mul(/*ILinAlgMat mat1, */ double     val2) { return new CMatrix(matrix * val2                    ); }
                public override ILinAlgMat op_Div(/*ILinAlgMat mat1, */ double     val2) { return new CMatrix(matrix / val2                    ); }

                public static implicit operator CMatrix  (Matrix    mat) { return new CMatrix(mat); }
                public static implicit operator CMatrix  (double[,] mat) { return new CMatrix(mat); }
                public static implicit operator Matrix   (CMatrix mat)   { return mat.matrix; }
                public static implicit operator double[,](CMatrix mat)   { return mat.matrix.ToArray(); }
            }

            public override ILinAlgMat _ToILMat(Matrix mat)
            {
                return new CMatrix(mat);
            }
            public override ILinAlgMat _ToILMat(double[] mat)
            {
                Vector vec = mat;
                return new CMatrix(vec.ToColMatrix());
            }
            public override double _Det(ILinAlgMat mat)
            {
                using(new Matlab.NamedLock("LA"))
                {
                    Matlab.PutMatrix("LA.A", mat.ToArray());
                    double det = Matlab.GetValue("det(LA.A)");
                    Matlab.Clear();
                    return det;
                }
            }
            public override ILinAlgMat _Diag(ILinAlgMat mat)
            {
                using(new Matlab.NamedLock("LA"))
                {
                    Matlab.PutMatrix("LA.A", mat.ToArray());
                    CMatrix diag = Matlab.GetMatrix("diag(LA.A)");
                    Matlab.Clear();
                    return diag;
                }
            }
            public override Tuple<ILinAlgMat, double[]> _EigSymm(ILinAlgMat A)
            {
                using(new Matlab.NamedLock("LA"))
                {
                    Matlab.PutMatrix("LA.A", A.ToArray());
                    Matlab.Execute("LA.A = (LA.A + LA.A)/2;");
                    Matlab.Execute("[LA.V, LA.D] = eig(LA.A);");
                    Matlab.Execute("LA.D = diag(LA.D);");
                    CMatrix  V = Matlab.GetMatrix("LA.V");
                    double[] D = Matlab.GetVector("LA.D");
                    if(HDebug.IsDebuggerAttached)
                    {
                        Matlab.Execute("LA.ERR = LA.A - (LA.V * diag(LA.D) * LA.V');");
                        Matlab.Execute("LA.ERR = max(max(abs(LA.ERR)));");
                        double err = Matlab.GetValue("LA.ERR");
                        HDebug.AssertTolerance(0.00000001, err);
                    }
                    Matlab.Clear();
                    return new Tuple<ILinAlgMat,double[]>(V,D);
                }
            }
            public override ILinAlgMat _Mul(params ILinAlgMat[] mats)
            {
                using(new Matlab.NamedLock("LA"))
                {
                    Matlab.PutMatrix("LA.mul", mats[0].ToArray());
                    for(int i=1; i<mats.Length; i++)
                    {
                        Matlab.PutMatrix("LA.tmp", mats[i].ToArray());
                        Matlab.Execute("LA.mul = LA.mul * LA.tmp;");
                    }
                    CMatrix mul = Matlab.GetMatrix("LA.mul",true);
                    Matlab.Clear();
                    return mul;
                }
            }
            public override ILinAlgMat _PInv(ILinAlgMat A)
            {
                using(new Matlab.NamedLock("LA"))
                {
                    Matlab.PutMatrix("LA.A", A.ToArray());
                    CMatrix diag = Matlab.GetMatrix("pinv(LA.A)");
                    Matlab.Clear();
                    return diag;
                }
            }
            public override ILinAlgMat _LinSolve(ILinAlgMat A, ILinAlgMat B)
            {
                using(new Matlab.NamedLock("LA"))
                {
                    Matlab.PutMatrix("LA.A", A.ToArray());
                    Matlab.PutMatrix("LA.B", B.ToArray());
                    CMatrix X = Matlab.GetMatrix(@"LA.A \ LA.B"); /// A X = B   =>   X = A\B
                    Matlab.Clear();
                    return X;
                }
            }
            public override ILinAlgMat _Inv(ILinAlgMat A)
            {
                using(new Matlab.NamedLock("LA"))
                {
                    Matlab.PutMatrix("LA.A", A.ToArray());
                    CMatrix inv = Matlab.GetMatrix("inv(LA.A)");
                    Matlab.Clear();
                    return inv;
                }
            }
            public override ILinAlgMat _InvSymm(ILinAlgMat A)
            {
                using(new Matlab.NamedLock("LA"))
                {
                    Matlab.PutMatrix("LA.A", A.ToArray());
                    Matlab.Execute  ("LA.A = (LA.A + LA.A')/2;");
                    CMatrix inv = Matlab.GetMatrix("inv(LA.A)");
                    Matlab.Clear();
                    return inv;
                }
            }
            public override ILinAlgMat _Eye(int size)
            {
                CMatrix eye = Matlab.GetMatrix(string.Format("eye({0})",size));
                return eye;
            }
            public override ILinAlgMat _Zeros(int colsize, int rowsize)
            {
                CMatrix zeros = Matlab.GetMatrix(string.Format("zeros({0},{1})", colsize, rowsize));
                return zeros;
            }
            public override ILinAlgMat _Ones(int colsize, int rowsize)
            {
                CMatrix zeros = Matlab.GetMatrix(string.Format("ones({0},{1})", colsize, rowsize));
                return zeros;
            }
        }
	}
}
