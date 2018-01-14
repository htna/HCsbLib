using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
    [Serializable]
    public struct Trans3 : ISerializable
    {
        //HTLib.Trans3 trans;
        public readonly Vector     dt; // move (x,y,z)
        public readonly double     ds;  // scale
        public readonly Quaternion dr;  // rotate

        public static Trans3 UnitTrans
        {
            get
            {
                //return new Trans3(HTLib.Trans3.UnitTrans);
                return new Trans3
                (
                    new double[3],
                    0,
                    Quaternion.UnitRotation
                );
            }
        }

        public static Trans3 FromMove(Vector dt)
        {
            HDebug.Assert(dt.Size == 3);
            //return new Trans3(new HTLib.Trans3(dt[0], dt[1], dt[2]));
            return new Trans3
            (
                dt.Clone(),
                1,
                Quaternion.UnitRotation
            );
        }
        public static Trans3 FromScale(double ds)
        {
            //return new Trans3(new HTLib.Trans3(ds));
            return new Trans3
            (
                new double[3],
                ds,
                Quaternion.UnitRotation
            );

        }
        public static Trans3 FromRotate(Vector drAxis, double drAngler)
        {
            HDebug.Assert(drAxis.Size == 3);
            //HTLib.Quaternion dr = new HTLib.Quaternion(new HTLib.DoubleVector3(drAxis), drAngler);
            //return new Trans3(new HTLib.Trans3(dr));
            return new Trans3
            (
                new double[3],
                1,
                new Quaternion(drAxis.Clone(), drAngler)
            );
        }

        public Trans3(Vector dt, double ds, Quaternion dr)
        {
            HDebug.Assert(dt.Size == 3);
            //this.trans = new HTLib.Trans3(dt[0], dt[1], dt[2], ds, new HTLib.Quaternion(dr.r, dr.i, dr.j, dr.k));
            this.dt = dt.Clone();
            this.ds = ds;
            this.dr = dr.Clone();
        }
        public Trans3(Vector dt, double ds, Vector drAxis, double drAngler)
        {
            HDebug.Assert(dt.Size == 3);
            HDebug.Assert(drAxis.Size == 3);
            //HTLib.Quaternion dr = new HTLib.Quaternion(new HTLib.DoubleVector3(drAxis), drAngler);
            //this.trans = new HTLib.Trans3(dt[0], dt[1], dt[2], ds, dr);
            this.dt = dt.Clone();
            this.ds = ds;
            this.dr = new Quaternion(drAxis.Clone(), drAngler);
        }
        //public Trans3(HTLib.Trans3 trans)
        //{
        //    this.trans = trans;
        //}
        public Trans3 Clone()
        {
            Vector dt = this.dt;
            double ds = this.ds;
            Quaternion dr = this.dr;
            return new Trans3(dt, ds, dr);
        }

        //public Vector     dt { get { return trans.dt.ToArray();                                             } }  // move (x,y,z)
        //public double     ds { get { return trans.ds;                                                       } }  // scale
        //public Quaternion dr { get { return new Quaternion(trans.dr.r, trans.dr.i, trans.dr.j, trans.dr.k); } }  // rotate

        public MatrixByArr TransformMatrix
        {
            get
            {
                //return trans.Double4x4;
                MatrixByArr rotmat = dr.RotationMatrix;
                return new double[,]{
                    {ds*rotmat[0,0], ds*rotmat[0,1], ds*rotmat[0,2], dt[0]},
                    {ds*rotmat[1,0], ds*rotmat[1,1], ds*rotmat[1,2], dt[1]},
                    {ds*rotmat[2,0], ds*rotmat[2,1], ds*rotmat[2,2], dt[2]},
                    {             0,              0,              0,     1},
                };
            }
        }
        public double[,] AffineMatrix
        {
            get
            {
                //return trans.AffineMatrix;
                Matrix rotmat = dr.RotationMatrix;
                return new double[,] {
                    {ds*rotmat[0,0], ds*rotmat[0,1], ds*rotmat[0,2], dt[0]},
                    {ds*rotmat[1,0], ds*rotmat[1,1], ds*rotmat[1,2], dt[1]},
                    {ds*rotmat[2,0], ds*rotmat[2,1], ds*rotmat[2,2], dt[2]},
                };
            }
        }

        public static Trans3 AppendTrans(Trans3 _trans, Trans3 dtrans)
        {
            //Trans3 newtrans = new Trans3(HTLib.Trans3.GetTransform(dtrans.trans, trans.trans));
            Trans3 newtrans;
            {   //public static Trans3 GetTransform(Trans3 trans2, Trans3 trans1)
                Trans3 trans2 = dtrans;
                Trans3 trans1 = _trans;
                // trans2 * trans2
    //          DoubleMatrix4 mat2 = trans2.TransformMatrix;
    //          DoubleMatrix4 mat1 = trans1.TransformMatrix;
    //          DoubleMatrix4 trans_mat = mat2*mat1;
    //          DoubleVector3 dt = new DoubleVector3(trans_mat.m03, trans_mat.m13, trans_mat.m23);
                Vector        dt = trans2.DoTransform(trans1.dt);
                double        ds = trans2.ds * trans1.ds;
                Quaternion    dr = trans2.dr * trans1.dr; //Quaternion.GetQuaternion(trans_mat, ds);

                Trans3 trans = new Trans3(dt, ds, dr);

                if(HDebug.IsDebuggerAttached)
                {
                    Vector pt1, pt2, pt12;
                    pt1 = new double[] { 10, 0, 0 };
                    pt2 = trans2.DoTransform(trans1.DoTransform(pt1));
                    pt12 = trans.DoTransform(pt1);
                    HDebug.Assert((pt12-pt2).Dist < 0.001);
                    pt1 = new double[] { 0, 10, 0 };
                    pt2 = trans2.DoTransform(trans1.DoTransform(pt1));
                    pt12 = trans.DoTransform(pt1);
                    HDebug.Assert((pt12-pt2).Dist < 0.001);
                    pt1 = new double[] { 0, 0, 10 };
                    pt2 = trans2.DoTransform(trans1.DoTransform(pt1));
                    pt12 = trans.DoTransform(pt1);
                    HDebug.Assert((pt12-pt2).Dist < 0.001);
                }

                //return trans;
                newtrans = trans;
            }

            if(HDebug.IsDebuggerAttached)
            {
                MatrixByArr mat = _trans.TransformMatrix;
                MatrixByArr dmat = dtrans.TransformMatrix;
                MatrixByArr newmat = newtrans.TransformMatrix;
                MatrixByArr diff = newmat - dmat * mat;
                HDebug.AssertTolerance(0.00001, diff);
            }
            return newtrans;
        }

        public double[] DoTransform(double[] pt)
        {
            HDebug.Assert(pt.Length == 3 || pt.Length == 4);
            double[] moved = null;
            if(pt.Length == 3) moved = new double[3];
            if(pt.Length == 4) moved = new double[4];
            double[,] affine = this.AffineMatrix;
            moved[0] = affine[0, 0]*pt[0] + affine[0, 1]*pt[1] + affine[0, 2]*pt[2] + affine[0, 3];
            moved[1] = affine[1, 0]*pt[0] + affine[1, 1]*pt[1] + affine[1, 2]*pt[2] + affine[1, 3];
            moved[2] = affine[2, 0]*pt[0] + affine[2, 1]*pt[1] + affine[2, 2]*pt[2] + affine[2, 3];
            if(pt.Length == 4) moved[3] = pt[3];
            return moved;
        }
        public void DoTransform(IList<double[]> pts)
        {
            for(int i=0; i<pts.Count; i++)
            {
                pts[i] = DoTransform(pts[i]);
            }
        }
        public void DoTransform(IList<Vector> pts)
        {
            for(int i=0; i<pts.Count; i++)
            {
                pts[i] = DoTransform(pts[i]);
            }
        }
        public Vector[] GetTransformed(Vector[] pts) { return GetTransformed(new List<Vector>(pts)).ToArray(); }
        public List<Vector> GetTransformed(IList<Vector> pts)
        {
            List<Vector> transformed = new List<Vector>(pts.Count);
            for(int i=0; i<pts.Count; i++)
            {
                transformed.Add(DoTransform(pts[i]));
            }
            return transformed;
        }
        public void DoTransform(MatrixByArr pts)
        {
            HDebug.Assert(pts.RowSize == 3);
            for(int i=0; i<pts.ColSize; i++)
            {
                Vector pt = pts.GetRowVector(i);
                pt = DoTransform(pt);
                pts[i, 0] = pt[0];
                pts[i, 1] = pt[1];
                pts[i, 2] = pt[2];
            }
        }

        public Trans3 GetInverse()
        {
            throw new NotImplementedException();
            //return new Trans3(trans.Inverse);
            //{
            //    DoubleVector3[] from = new DoubleVector3[3] {
            //        new DoubleVector3(1, 0, 0),
            //        new DoubleVector3(0, 1, 0),
            //        new DoubleVector3(0, 0, 1)
            //    };
            //    DoubleVector3[] to = new DoubleVector3[3]{
            //        this.DoTransform(from[0]),
            //        this.DoTransform(from[1]),
            //        this.DoTransform(from[2])
            //    };
            //    ICP3Points icp = new ICP3Points(to, from, Trans3.UnitTrans, 0);
            //    Trans3 inv = icp.trans;
            //    {
            //        Quaternion invR = new Quaternion(dr.RotationAxis, -1*dr.RotationAngle);
            //        double     invS = 1 / ds;
            //        DoubleVector3 invT = (-1.0 * (invR.RotationMatrix * invS)) * dt;
            //        inv = new Trans3(invT, invS, invR);
            //    }
            //
            //    if(Debug.IsDebuggerAttached)
            //    {
            //        DoubleVector3 from0 = inv.DoTransform(to[0]);
            //        DoubleVector3 from1 = inv.DoTransform(to[1]);
            //        DoubleVector3 from2 = inv.DoTransform(to[2]);
            //        Debug.Assert((from0-from[0]).Length < 0.00000001);
            //        Debug.Assert((from1-from[1]).Length < 0.00000001);
            //        Debug.Assert((from2-from[2]).Length < 0.00000001);
            //    }
            //    return inv;
            //}
        }
        public override string ToString()
        {
            return ToString(null, null);
        }
        public string ToString(string format, IFormatProvider formatProvider)
        {
            //return trans.ToString(format, formatProvider);
            string str
                = "mv(" + dt[0].ToString("0.000", formatProvider) + "," + dt[1].ToString("0.000", formatProvider) + "," + dt[2].ToString("0.000", formatProvider) + "), "
                + "s(" + ds.ToString("0.000", formatProvider) + "), "
                + "r(" + dr.ToString() + ")";
            return str;
        }

        public static Trans3 GetTransform(Vector dt, double ds, Quaternion dr)
        {
            return new Trans3(dt.Clone(), ds, dr.Clone());
        }
        public static Trans3 GetTransformNoScale(Vector from1, Vector from2, Vector from3, Vector to1, Vector to2, Vector to3)
        {
            //HTLib.DoubleVector3 lfrom1 = new HTLib.DoubleVector3(from1);
            //HTLib.DoubleVector3 lfrom2 = new HTLib.DoubleVector3(from2);
            //HTLib.DoubleVector3 lfrom3 = new HTLib.DoubleVector3(from3);
            //HTLib.DoubleVector3 lto1   = new HTLib.DoubleVector3(to1);
            //HTLib.DoubleVector3 lto2   = new HTLib.DoubleVector3(to2);
            //HTLib.DoubleVector3 lto3   = new HTLib.DoubleVector3(to3);
            //HTLib.Trans3        ltrans = HTLib.Trans3.GetTransformNoScale(lto1, lto2, lto3, lfrom1, lfrom2, lfrom3);
            Trans3 ltrans;
            {
                Vector[] p = new Vector[] { to1, to2, to3 };
                Vector[] x = new Vector[] { from1, from2, from3 };
                Trans3 optTrans = ICP3.OptimalTransform(p, x);
                ltrans = optTrans;
            }

            if(HDebug.IsDebuggerAttached)
            {
                var nto1 = ltrans.DoTransform(from1); var err1=(nto1 - to1).Dist;
                var nto2 = ltrans.DoTransform(from2); var err2=(nto2 - to2).Dist;
                var nto3 = ltrans.DoTransform(from3); var err3=(nto3 - to3).Dist;
                HDebug.AssertTolerance(0.00000001, err1, err2, err3);
            }
            return ltrans;
        }
        public static Trans3 GetTransformNoScale(Vector from1, Vector from2, Vector to1, Vector to2)
        {
            //HTLib.DoubleVector3 lfrom1 = new HTLib.DoubleVector3(from1);
            //HTLib.DoubleVector3 lfrom2 = new HTLib.DoubleVector3(from2);
            //HTLib.DoubleVector3 lto1   = new HTLib.DoubleVector3(to1  );
            //HTLib.DoubleVector3 lto2   = new HTLib.DoubleVector3(to2  );
            //HTLib.Trans3        ltrans = HTLib.Trans3.GetTransformNoScale(lfrom1, lfrom2, lto1, lto2);
            //return new Trans3(ltrans);
            {
                double ds = 1;
                //double dx = to2.x - from2.x;     // dx + from2.x = to2.x
                //double dy = to2.y - from2.y;     // dx + from2.y = to2.y
                //double dz = to2.z - from2.z;     // dx + from2.z = to2.z
                Vector from = (from2 - from1).UnitVector();
                Vector to   = (to2   - to1  ).UnitVector();
                Quaternion dr;
                double innerprod = LinAlg.DotProd(from, to);    // InnerProduct(from,to);
                if(innerprod >= 1)
                {
                    HDebug.Assert(innerprod == 1);
                    dr = Quaternion.UnitRotation;
                }
                else
                {
                    Vector axis = LinAlg.CrossProd(from, to);   // CrossProduct(from, to);
                    if(axis.Dist2 == 0)
                    {
                        // to avoid degenerate cases
                        double mag = to.Dist / 100000000;
                        axis = LinAlg.CrossProd(from, to+(new double[] { mag, mag*2, mag*3 }));     //Vector.CrossProduct(from, to+new DoubleVector3(mag, mag*2, mag*3));
                    }
                    double angle = Math.Acos(innerprod);
                    dr = new Quaternion(axis, angle);
                    HDebug.Assert(LinAlg.DotProd((dr.RotationMatrix * from), to) > 0.99999);
                }
                Trans3 trans  =                   Trans3.GetTransform(       -from1,  1, Quaternion.UnitRotation);
                trans = Trans3.AppendTrans(trans, Trans3.GetTransform(new double[3], ds, dr                     ));
                trans = Trans3.AppendTrans(trans, Trans3.GetTransform(          to1,  1, Quaternion.UnitRotation));
                //new Trans3(dx, dy, dz, ds, dr);
                if(HDebug.IsDebuggerAttached)
                {
                    Vector fromto1 = trans.DoTransform(from1);
                    HDebug.Assert((fromto1-to1).Dist < 0.000001);
                    Vector fromto2 = trans.DoTransform(from2);
                    HDebug.Assert(((fromto2-fromto1).UnitVector()-(to2-to1).UnitVector()).Dist < 0.000001);
                }
                return trans;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Parse
        public static Trans3 Parse(string s, params char[] separator)
        {
            throw new NotImplementedException();
            //return new Trans3(HTLib.Trans3.Parse(s, separator));
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Serializable
        public Trans3(SerializationInfo info, StreamingContext context)
        {
            //trans = new HTLib.Trans3(info, context);
            {
                double dx = (double)info.GetValue("dx", typeof(double));
                double dy = (double)info.GetValue("dy", typeof(double));
                double dz = (double)info.GetValue("dz", typeof(double));
                dt = new Vector(dx, dy, dz);
                ds = (double)info.GetValue("ds", typeof(double));
                dr = (Quaternion)info.GetValue("dr", typeof(Quaternion));
            }
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //trans.GetObjectData(info, context);
            {
                info.AddValue("dx", dt[0]);
                info.AddValue("dy", dt[1]);
                info.AddValue("dz", dt[2]);
                info.AddValue("ds", ds);
                info.AddValue("dr", dr);
            }
        }
        public void ToFile(string path)
        {
            //trans.ToFile(path);
            {
                System.IO.StreamWriter writer = new System.IO.StreamWriter(path);
                writer.Write("{0},{1},{2}", dt[0], dt[1], dt[2]);
                writer.Write(",{0}", dr);
                writer.Write(",{0}+{1}i+{2}j+{3}k", dr.r, dr.i, dr.j, dr.k);
                writer.Close();
                writer.Dispose();
            }
        }
        public static Trans3 FromFile(string path)
        {
            //return new Trans3(HTLib.Trans3.FromFile(path));
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(path);
                string str = reader.ReadLine();
                reader.Close();
                reader.Dispose();
                return Trans3.Parse(str);
            }
        }
    }
}
