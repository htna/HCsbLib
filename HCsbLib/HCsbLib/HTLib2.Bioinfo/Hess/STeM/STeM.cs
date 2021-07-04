using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Hess
{
    public partial class STeM
    {
        static STeM()
        {
            //SelfTest();
        }

        class VECTORS
        {
            public IList<Vector> caArray;
            public VECTORS(IList<Vector> caArray) { this.caArray = caArray; }
            public double this[int i1, int i2]
            {
                get { double value = caArray[i1-1][i2-1]; return value; }
                set {                caArray[i1-1][i2-1] = value; }
            }
            public Vector GetRow(int i)
            {
                return caArray[i-1];
            }
        }
        class MATRIX<MAT>
                where MAT : IMatrix<double>
        {
            public MAT matrix;
            public MATRIX(MAT matrix) { this.matrix = matrix; }
            public double this[int i1, int i2]
            {
                get { return matrix[i1-1, i2-1]; }
                set {        matrix[i1-1, i2-1] = value; }
            }
        }
        public static double glength(Vector X)           { return X.Dist; }
        public static double dot    (Vector a, Vector b) { return LinAlg.VtV(a, b); }
        public static Vector cross  (Vector a, Vector b) { return LinAlg.CrossProd(a, b); }
        public static double sqrt   (double x)           { return Math.Sqrt(x); }
        public static double pow    (double x)           { return x*x; }
        public static double pow    (double x, double y) { return Math.Pow(x, y); }
        public static double abs    (double x)           { return Math.Abs(x); }
    }
}
}
