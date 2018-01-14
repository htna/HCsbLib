using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace HTLib2
{
    public abstract partial class ICP3
    {
        public static Trans3 OptimalTransformWeighted(IList<Vector> source, IList<Vector> target, IList<double> weight
                                                     , double? rotateRatio=null
                                                     )
        {
            if(HDebug.IsDebuggerAttached) foreach(double _w in weight) HDebug.Assert(_w >= 0);
            Vector[] p = source.ToArray();
            Vector[] x = target.ToArray();
            double[] w = weight.ToArray();
            HDebug.Assert(p.Length == x.Length);
            HDebug.Assert(w.Length == x.Length);

            Vector up = p.MeanWeighted(w);
            Vector ux = x.MeanWeighted(w);

            Quaternion quater = OptimalRotationWeighted(p, x, up, ux, w);
            if(rotateRatio != null)
            {
                double r = quater.RotationAngle;
                r = r % (2*Math.PI);
                r = (r > Math.PI) ? (r-2*Math.PI) : r;
                r = r / 2;
                quater = new Quaternion(quater.RotationAxis, r);
            }
            MatrixByArr qR = quater.RotationMatrix;
            Vector qT = OptimalTransWeighted(up, ux, qR, w);

            return new Trans3(qT, 1, quater);
        }
        public static Quaternion OptimalRotationWeighted(Vector[] p, Vector[] x, double[] w)
        {
            Vector up = p.MeanWeighted(w);
            Vector ux = x.MeanWeighted(w);
            return OptimalRotationWeighted(p, x, up, ux, w);
        }
        public static Quaternion OptimalRotationWeighted(Vector[] p, Vector[] x, Vector up, Vector ux, double[] w)
        {
            MatrixByArr cov = CovarianceWeighted(p, x, up, ux, w);
            double tr  = cov[0,0] + cov[1,1] + cov[2,2];
            double A23 = cov[1,2] - cov[2,1];
            double A31 = cov[2,0] - cov[0,2];
            double A12 = cov[0,1] - cov[1,0];

            MatrixByArr mat = new double[4, 4];
            mat[0, 0] = tr;
            mat[1, 0] = mat[0, 1] = A23;
            mat[2, 0] = mat[0, 2] = A31;
            mat[3, 0] = mat[0, 3] = A12;
            cov = cov + cov.Tr();
            cov = cov - LinAlg.Eye(3, tr);
            mat[1, 1] = cov[0,0];
            mat[1, 2] = cov[1,0];
            mat[1, 3] = cov[2,0];
            mat[2, 1] = cov[0,1];
            mat[2, 2] = cov[1,1];
            mat[2, 3] = cov[2,1];
            mat[3, 1] = cov[0,2];
            mat[3, 2] = cov[1,2];
            mat[3, 3] = cov[2,2];

            LinAlg.Eigen eigen = new LinAlg.Eigen(mat);
            double[,] eigenD = eigen.getD();
            double[,] eigenV = eigen.getV();

            int index_max = 0;
            if(eigenD[1, 1] > eigenD[index_max, index_max]) index_max = 1;
            if(eigenD[2, 2] > eigenD[index_max, index_max]) index_max = 2;
            if(eigenD[3, 3] > eigenD[index_max, index_max]) index_max = 3;

            Quaternion quater = new Quaternion(
                eigenV[0, index_max],
                eigenV[1, index_max],
                eigenV[2, index_max],
                eigenV[3, index_max]
            );

            return quater;
        }
        public static Vector OptimalTransWeighted(Vector up, Vector ux, MatrixByArr qR, double[] w)
        {
            return (ux - qR*up);
        }
        public static MatrixByArr CovarianceWeighted(Vector[] p, Vector[] x, Vector up, Vector ux, double[] w)
        {
            MatrixByArr cov  = new double[3, 3]; cov[0, 0] = cov[1, 1] = cov[2, 2] = 1;
            for(int i=0; i<p.Length; i++)
            {
                Vector pi = p[i] - up;
                Vector xi = x[i] - ux;
                double wi = w[i];
                cov[0,0] += wi * pi[0] * xi[0];  cov[0,1] += wi * pi[0] * xi[1];  cov[0,2] += wi * pi[0] * xi[2];
                cov[1,0] += wi * pi[1] * xi[0];  cov[1,1] += wi * pi[1] * xi[1];  cov[1,2] += wi * pi[1] * xi[2];
                cov[2,0] += wi * pi[2] * xi[0];  cov[2,1] += wi * pi[2] * xi[1];  cov[2,2] += wi * pi[2] * xi[2];
            }
            cov /= w.Sum();
            return cov;
        }
    }
}
