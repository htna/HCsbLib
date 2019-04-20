using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace HTLib2
{
    public abstract partial class ICP3
    {
        public static Trans3 OptimalTransform(IList<Vector> source, IList<Vector> target)
        {
            //int size = source.Count;
            //HDebug.Exception(size != 0, "Aligning empty points");
            //HTLib.DoubleVector3[] p = new HTLib.DoubleVector3[size];
            //HTLib.DoubleVector3[] x = new HTLib.DoubleVector3[size];
            //HDebug.Assert(source.Count == target.Count);
            //for(int i=0; i<size; i++)
            //{
            //    HDebug.Assert(source[i].Size == 3);
            //    HDebug.Assert(target[i].Size == 3);
            //    p[i] = new HTLib.DoubleVector3(source[i][0], source[i][1], source[i][2]);
            //    x[i] = new HTLib.DoubleVector3(target[i][0], target[i][1], target[i][2]);
            //}
            //
            //HTLib.Trans3 trans = HTLib.ICP3.OptimalTransform(p, x);
            //return new Trans3(trans);


            double[] weight = new double[source.Count];
            for(int i=0; i<weight.Length; i++)
                weight[i] = 1;
            return OptimalTransformWeighted(source, target, weight);
        }
        public static Quaternion OptimalRotation(IList<Vector> p, IList<Vector> x)
        {
            //HTLib.Vector[] pp = new HTLib.Vector[p.Count];
            //HTLib.Vector[] xx = new HTLib.Vector[x.Count];
            //for(int i=0; i<pp.Length; i++) pp[i] = p[i].ToArray();
            //for(int i=0; i<xx.Length; i++) xx[i] = x[i].ToArray();
            //HTLib.Quaternion quat = HTLib.ICP3.OptimalRotation(pp, xx);
            //return new Quaternion(quat.ToArray());
            HDebug.ToDo("check");
            double[] w = new double[p.Count];
            for(int i=0; i<w.Length; i++)
                w[i] = 1;
            return OptimalRotationWeighted(p.ToArray(), x.ToArray(), w);
        }
        public static Vector OptimalTrans(Vector up, Vector ux, MatrixByArr qR)
        {
            //HTLib.DoubleVector3 _up = new HTLib.DoubleVector3(up.ToArray());
            //HTLib.DoubleVector3 _ux = new HTLib.DoubleVector3(ux.ToArray());
            //HTLib.DoubleMatrix3 _qR = new HTLib.DoubleMatrix3(qR.ToArray());
            //return HTLib.ICP3.OptimalTrans(_up, _ux, _qR).ToArray();
            HDebug.ToDo("check");
            double[] w = null;
            return OptimalTransWeighted(up, ux, qR, w);
        }
        //public static Trans3 OptimalTransform(MatrixByArr source, MatrixByArr target)
        //{
        //    int size = source.ColSize;
        //    HTLib.DoubleVector3[] p = new HTLib.DoubleVector3[size];
        //    HTLib.DoubleVector3[] x = new HTLib.DoubleVector3[size];
        //    HDebug.Assert(source.RowSize == 3);
        //    HDebug.Assert(target.RowSize == 3);
        //    for(int i=0; i<size; i++)
        //    {
        //        p[i] = new HTLib.DoubleVector3(source[i,0], source[i,1], source[i,2]);
        //        x[i] = new HTLib.DoubleVector3(target[i,0], target[i,1], target[i,2]);
        //    }
        //
        //    HTLib.Trans3 trans = HTLib.ICP3.OptimalTransform(p, x);
        //    return new Trans3(trans);
        //}
    }
}
