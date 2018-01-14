/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	[Serializable]
	public struct Trans3 : ISerializable
	{
		HTLib1.Trans3 trans;

        public static Trans3 UnitTrans { get { return new Trans3(HTLib1.Trans3.UnitTrans); } }

        public static Trans3 FromMove(Vector dt)
        {
            HDebug.Assert(dt.Size == 3);
            return new Trans3(new HTLib1.Trans3(dt[0], dt[1], dt[2]));
        }
        public static Trans3 FromScale(double ds)
        {
            return new Trans3(new HTLib1.Trans3(ds));
        }
        public static Trans3 FromRotate(Vector drAxis, double drAngler)
        {
throw new NotImplementedException();
//            HDebug.Assert(drAxis.Size == 3);
//            HTLib1.Quaternion dr = new HTLib1.Quaternion(new HTLib1.DoubleVector3(drAxis), drAngler);
//            return new Trans3(new HTLib1.Trans3(dr));
        }

        public Trans3(Vector dt, double ds, Quaternion dr)
        {
throw new NotImplementedException();
//            HDebug.Assert(dt.Size == 3);
//            this.trans = new HTLib1.Trans3(dt[0], dt[1], dt[2], ds, new HTLib1.Quaternion(dr.r, dr.i, dr.j, dr.k));
        }
        public Trans3(Vector dt, double ds, Vector drAxis, double drAngler)
        {
throw new NotImplementedException();
//            HDebug.Assert(dt.Size == 3);
//            HDebug.Assert(drAxis.Size == 3);
//            HTLib1.Quaternion dr = new HTLib1.Quaternion(new HTLib1.DoubleVector3(drAxis), drAngler);
//            this.trans = new HTLib1.Trans3(dt[0], dt[1], dt[2], ds, dr);
        }
		public Trans3(HTLib1.Trans3 trans)
		{
			this.trans = trans;
		}
        public Trans3 Clone()
        {
            Vector dt = this.dt;
            double ds = this.ds;
            Quaternion dr = this.dr;
            return new Trans3(dt, ds, dr);
        }

		public Vector     dt { get { return trans.dt.ToArray();                                             } }  // move (x,y,z)
		public double     ds { get { return trans.ds;                                                       } }  // scale
		public Quaternion dr { get { return new Quaternion(trans.dr.r, trans.dr.i, trans.dr.j, trans.dr.k); } }  // rotate

		public MatrixByArr TransformMatrix
		{
			get
			{
throw new NotImplementedException();
//				return trans.Double4x4;
			}
		}
		public double[,] AffineMatrix
		{
			get
			{
throw new NotImplementedException();
//				return trans.AffineMatrix;
			}
		}

        public static Trans3 AppendTrans(Trans3 trans, Trans3 dtrans)
        {
            Trans3 newtrans = new Trans3(HTLib1.Trans3.GetTransform(dtrans.trans, trans.trans));
            if(HDebug.IsDebuggerAttached)
            {
                MatrixByArr mat = trans.TransformMatrix;
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
			return new Trans3(trans.Inverse);
		}
		public override string ToString()
		{
			return trans.ToString(null, null);
		}
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return trans.ToString(format, formatProvider);
		}

        public static Trans3 GetTransformNoScale(Vector from1, Vector from2, Vector from3, Vector to1, Vector to2, Vector to3)
        {
throw new NotImplementedException();
//            HTLib1.DoubleVector3 lfrom1 = new HTLib1.DoubleVector3(from1);
//            HTLib1.DoubleVector3 lfrom2 = new HTLib1.DoubleVector3(from2);
//            HTLib1.DoubleVector3 lfrom3 = new HTLib1.DoubleVector3(from3);
//            HTLib1.DoubleVector3 lto1   = new HTLib1.DoubleVector3(to1);
//            HTLib1.DoubleVector3 lto2   = new HTLib1.DoubleVector3(to2);
//            HTLib1.DoubleVector3 lto3   = new HTLib1.DoubleVector3(to3);
//            HTLib1.Trans3        ltrans = HTLib1.Trans3.GetTransformNoScale(lto1, lto2, lto3, lfrom1, lfrom2, lfrom3);
//            if(HDebug.IsDebuggerAttached)
//            {
//                var nto1 = ltrans.DoTransform(lfrom1); var err1=(nto1 - lto1).Length;
//                var nto2 = ltrans.DoTransform(lfrom2); var err2=(nto2 - lto2).Length;
//                var nto3 = ltrans.DoTransform(lfrom3); var err3=(nto3 - lto3).Length;
//                HDebug.AssertTolerance(0.00000001, err1, err2, err3);
//            }
//            return new Trans3(ltrans);
        }
        public static Trans3 GetTransformNoScale(Vector from1, Vector from2, Vector to1, Vector to2)
        {
throw new NotImplementedException();
//            HTLib1.DoubleVector3 lfrom1 = new HTLib1.DoubleVector3(from1);
//            HTLib1.DoubleVector3 lfrom2 = new HTLib1.DoubleVector3(from2);
//            HTLib1.DoubleVector3 lto1   = new HTLib1.DoubleVector3(to1  );
//            HTLib1.DoubleVector3 lto2   = new HTLib1.DoubleVector3(to2  );
//            HTLib1.Trans3        ltrans = HTLib1.Trans3.GetTransformNoScale(lfrom1, lfrom2, lto1, lto2);
//            return new Trans3(ltrans);
        }

		////////////////////////////////////////////////////////////////////////////////////
		// Parse
		public static Trans3 Parse(string s, params char[] separator)
		{
			return new Trans3(HTLib1.Trans3.Parse(s, separator));
		}

		////////////////////////////////////////////////////////////////////////////////////
		// Serializable
		public Trans3(SerializationInfo info, StreamingContext context)
		{
			trans = new HTLib1.Trans3(info, context);
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			trans.GetObjectData(info, context);
		}
		public void ToFile(string path)
		{
			trans.ToFile(path);
		}
		public static Trans3 FromFile(string path)
		{
			return new Trans3(HTLib1.Trans3.FromFile(path));
		}
	}
}
*/