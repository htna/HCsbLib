using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2
{
	[Serializable]
	public struct Quaternion : IEquatable<Quaternion>, ISerializable
	{
		public enum EnumToString { ToAxisAngle, ToVector };
		public static EnumToString typeToString = EnumToString.ToAxisAngle;
		public static Quaternion UnitRotation
		{
			get { return new Quaternion(new Vector(1, 1, 1), 0); }
		}

		public readonly Vector _data;
		public double r { get{ return _data[0]; } } // real value
		public double i { get{ return _data[1]; } } // first imaginery value
		public double j { get{ return _data[2]; } } // second imaginery value
		public double k { get{ return _data[3]; } } // third imaginery value

        public Quaternion Clone()
        {
            return new Quaternion(_data.Clone());
        }

		public Quaternion(double[] axis, double angle)
		{
			double _sin = Math.Sin((double)angle/2);
			double _cos = Math.Cos((double)angle/2);
			Vector axisNormalized = Vector.From(axis.Clone());
			axisNormalized = axisNormalized / axisNormalized.Dist;
			double _r = _cos;
			double _i = axisNormalized[0] * _sin;
			double _j = axisNormalized[1] * _sin;
			double _k = axisNormalized[2] * _sin;
			this._data = new Vector(_r, _i, _j, _k);
		}
		public Quaternion(double r, double i, double j, double k)
		{
			this._data = new Vector(r, i, j, k);
		}
		public Quaternion(double[] q)
		{
			HDebug.Assert(q.Length == 4);
			this._data = Vector.From(q.Clone());
		}
		//public Quaternion(double r)
		//{
		//    this._data = new Vector(r, 0, 0, 0);
		//}
		public Quaternion(Quaternion q)
		{
			this._data = Vector.From(q._data.Clone());
		}

		public static bool operator!=(Quaternion left, Quaternion right)
		{
			return (left._data != right._data);
		}
		public static bool operator==(Quaternion left, Quaternion right)
		{
			return (left._data == right._data);
		}
		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;

			if(obj.GetType() != typeof(Quaternion))
				return false;

			return (this == (Quaternion)obj);
		}
		public override int GetHashCode()
		{
			return _data.GetHashCode();
		}
		public override string ToString()
		{
			string str  = "";
			switch(typeToString)
			{
				case EnumToString.ToVector:
					str = ""
						+ r.ToString("0.000") + " + "
						+ i.ToString("0.000") + " i + "
						+ j.ToString("0.000") + " j + "
						+ k.ToString("0.000") + " k";
					break;
				case EnumToString.ToAxisAngle:
					str = ""
						+ "{" + RotationAxis.ToString("0.000") + "} & "
						+ RotationAngle.ToString("0.000");
					break;
			}
			return str;
		}
		////////////////////////////////////////////////////////////////////////////////////
		// IEquatable<PointF>
		public bool Equals(Quaternion other)
		{
			return (this == other);
		}

		public double[] ToArray()
		{
			return new double[] { r, i, j, k };
		}
		public Quaternion Conjugate
		{
			get
			{
				return new Quaternion(r, -i, -j, -k);
			}
		}
		public Quaternion Inverse
		{
			get
			{
				double leng = r*r + i*i + j*j + k*k;
				return new Quaternion(r/leng, -i/leng, -j/leng, -k/leng);
			}
		}

		//////////////////////////////////////////////////////////////////////////
		// Quaternion rotation
		public double RotationAngle
		{
			get
			{
				double angle = Math.Acos(r) * 2;
				return angle;
			}
		}
		public Vector RotationAxis
		{
			get
			{
				double len = Math.Sqrt(i*i + j*j + k*k);
				return new Vector( i/len, j/len, k/len );
			}
		}
		public MatrixByArr RotationMatrix
		{
			get
			{
				if(RotationAngle == 0)
                    return LinAlg.Eye(3);
				MatrixByArr mat = new MatrixByArr(3,3);
				double q0 = r;
				double q1 = i;
				double q2 = j;
				double q3 = k;
				mat[0,0] = q0*q0 + q1*q1 - q2*q2 - q3*q3;
				mat[0,1] = 2*(q1*q2 - q0*q3);
				mat[0,2] = 2*(q1*q3 + q0*q2);
				mat[1,0] = 2*(q1*q2 + q0*q3);
				mat[1,1] = q0*q0 + q2*q2 - q1*q1 - q3*q3;
				mat[1,2] = 2*(q2*q3 - q0*q1);
				mat[2,0] = 2*(q1*q3 - q0*q2);
				mat[2,1] = 2*(q2*q3 + q0*q1);
				mat[2,2] = q0*q0 + q3*q3 - q1*q1 - q2*q2;
				return mat;
			}
		}

		//////////////////////////////////////////////////////////////////////////
		// operators of (Quaternion vs Quaternion)
		public static Quaternion operator+(Quaternion left, Quaternion right)
		{
			return new Quaternion(
						left.r+right.r,
						left.i+right.i,
						left.j+right.j,
						left.k+right.k
						);
		}
		public static Quaternion operator-(Quaternion left, Quaternion right)
		{
			return new Quaternion(
						left.r-right.r,
						left.i-right.i,
						left.j-right.j,
						left.k-right.k
						);
		}
		public static Quaternion operator*(Quaternion left, Quaternion right)
		{
			Quaternion p = left;
			Quaternion q = right;
			return new Quaternion(
						p.r*q.r - (p.i*q.i + p.j*q.j + p.k*q.k),
						p.r*q.i + q.r*p.i + (p.j*q.k - p.k*q.j),
						p.r*q.j + q.r*p.j + (p.k*q.i - p.i*q.k),
						p.r*q.k + q.r*p.k + (p.i*q.j - p.j*q.i)
						);
		}

		//////////////////////////////////////////////////////////////////////////
		// operators of (Quaternion vs double)
		public static Quaternion operator+(Quaternion left, double right)
		{
			return new Quaternion(
						left.r+right,
						left.i,
						left.j,
						left.k
						);
		}
		public static Quaternion operator-(Quaternion left, double right)
		{
			return new Quaternion(
						left.r-right,
						left.i,
						left.j,
						left.k
						);
		}
		public static Quaternion operator*(Quaternion left, double right)
		{
			return new Quaternion(
						left.r * right,
						left.i * right,
						left.j * right,
						left.k * right
						);
		}
		public static Quaternion operator/(Quaternion left, double right)
		{
			return (left * (1/right));
		}

		//////////////////////////////////////////////////////////////////////////
		// operators of (double vs Quaternion)
		public static Quaternion operator+(double left, Quaternion right)
		{
			return new Quaternion(
						left+right.r,
						right.i,
						right.j,
						right.k
						);
		}
		public static Quaternion operator-(double left, Quaternion right)
		{
			return new Quaternion(
						left-right.r,
						right.i,
						right.j,
						right.k
						);
		}
		public static Quaternion operator*(double left, Quaternion right)
		{
			return new Quaternion(
						left * right.r,
						left * right.i,
						left * right.j,
						left * right.k
						);
		}

		public static Quaternion GetQuaternion(MatrixByArr mat, double scale)
		{
			//   [a b c]   [q0*q0 + q1*q1 - q2*q2 - q3*q3,	2*(q1*q2 - q0*q3),				2*(q1*q3 + q0*q2)            ]
			// s*[d e f] = [2*(q1*q2 + q0*q3),				q0*q0 + q2*q2 - q1*q1 - q3*q3,	2*(q2*q3 - q0*q1)            ]
			//   [g h i]   [2*(q1*q3 - q0*q2),				2*(q2*q3 + q0*q1),				q0*q0 + q3*q3 - q1*q1 - q2*q2]
			//
			// q0*q0 + q1*q1 + q2*q2 + q3*q3 = 1
			// s(a+e) = 2*(q0*q0 - q3*q3)
			// s(a+i) = 2*(q0*q0 - q2*q2)
			// s(e+i) = 2*(q0*q0 - q1*q1)
			// s(d+b) = 4 * q1*q2
			// s(d-b) = 4 * q0*q3
			// s(c+g) = 4 * q1*q3
			// s(c-g) = 4 * q0*q2
			// s(h+f) = 4 * q2*q3
			// s(h-f) = 4 * q0*q1
			//
			// if we assume 's == 1',
			//    then (a+e+i) = 3*q0*q0 - (q1*q1 + q2*q2 + q3*q3) = 3*q0*q0 - (1-q0*q0) = 4*q0*q0 - 1
			//         q0 = sqrt((a + e + i + 4)/4)
			//         q1 = (h - f)/(4 * q0)
			//         q2 = (c - g)/(4 * q0)
			//         q3 = (d - b)/(4 * q0)
			HDebug.Assert(scale == 1);
			HDebug.Assert(mat.ColSize == 3, mat.RowSize == 3);
			double q0 = Math.Sqrt((mat[0,0] + mat[1,1] + mat[2,2] + 4)/4);
			HDebug.Assert(double.IsNaN(q0) == false);
			HDebug.Assert(q0 != 0);
			double q1 = (mat[2,1] - mat[1,2]) / (4*q0);
			double q2 = (mat[0,2] - mat[2,0]) / (4*q0);
			double q3 = (mat[1,0] - mat[0,1]) / (4*q0);
			HDebug.AssertSimilar((mat[0,0]+mat[1,1]), 2*(q0*q0-q3*q3), 0.001);
			HDebug.AssertSimilar((mat[0,0]+mat[2,2]), 2*(q0*q0-q2*q2), 0.001);
			HDebug.AssertSimilar((mat[1,1]+mat[2,2]), 2*(q0*q0-q1*q1), 0.001);
			HDebug.AssertSimilar((mat[1,0]+mat[0,1]), (4*q1*q2), 0.001);
			HDebug.AssertSimilar((mat[1,0]-mat[0,1]), (4*q0*q3), 0.001);
			HDebug.AssertSimilar((mat[0,2]+mat[2,0]), (4*q1*q3), 0.001);
			HDebug.AssertSimilar((mat[0,2]-mat[2,0]), (4*q0*q2), 0.001);
			HDebug.AssertSimilar((mat[2,1]+mat[1,2]), (4*q2*q3), 0.001);
			HDebug.AssertSimilar((mat[2,1]-mat[1,2]), (4*q0*q1), 0.001);
			Quaternion quater = new Quaternion(q0, q1, q2, q3);
			if(HDebug.IsDebuggerAttached)
			{
				MatrixByArr rotmat = quater.RotationMatrix;
				const double tolerance = 0.001;
				HDebug.Assert(Math.Abs(rotmat[0,0] - mat[0,0]) < tolerance);
				HDebug.Assert(Math.Abs(rotmat[0,1] - mat[0,1]) < tolerance);
				HDebug.Assert(Math.Abs(rotmat[0,2] - mat[0,2]) < tolerance);
				HDebug.Assert(Math.Abs(rotmat[1,0] - mat[1,0]) < tolerance);
				HDebug.Assert(Math.Abs(rotmat[1,1] - mat[1,1]) < tolerance);
				HDebug.Assert(Math.Abs(rotmat[1,2] - mat[1,2]) < tolerance);
				HDebug.Assert(Math.Abs(rotmat[2,0] - mat[2,0]) < tolerance);
				HDebug.Assert(Math.Abs(rotmat[2,1] - mat[2,1]) < tolerance);
				HDebug.Assert(Math.Abs(rotmat[2,2] - mat[2,2]) < tolerance);
			}
			return quater;
			//////////////////////////////////////////////////////////////////////////
			// (sqrt(3)*q0 + q1 + q2 + q3) * (sqrt(3)*q0 - q1 - q2 - q3)
			//   = (3*q0*q0 - q1*q1 - q2*q2 - q3*q3) - 2*(q1*q2 + q2*q3 + q1*q3)
			//   = (a+e+i)                           - (d+b + c+g + h+f)/2
			//
			// (q0 - q1 + q2 - q3) * (a0 - q1 - q2 + q3)
			//   = (q0*q0 + q1*q1 - q2*q2 - q3*q3) + 2*(q0*q1 + q2*q3)
			//   = a + h/2
		}
		////////////////////////////////////////////////////////////////////////////////////
		// Parse
		public static Quaternion Parse(string s)
		{
			// parse "0.2+1.0i+2.0j+3.0k"
			List<string> rijk = new List<string>(s.Split('+'));
			while(rijk.Remove("") == true) ;
			HDebug.Assert(rijk.Count == 4);
			for(int i_=0; i_<4; i_++) rijk[i_] = rijk[i_].Trim();
			HDebug.Assert(rijk[1].EndsWith("i"));
			HDebug.Assert(rijk[2].EndsWith("j"));
			HDebug.Assert(rijk[3].EndsWith("k"));
			double r = double.Parse(rijk[0]);
			double i = double.Parse(rijk[1].TrimEnd('i'));
			double j = double.Parse(rijk[2].TrimEnd('j'));
			double k = double.Parse(rijk[3].TrimEnd('k'));
			return new Quaternion(r, i, j, k);
		}
		////////////////////////////////////////////////////////////////////////////////////
		// Serializable
		public Quaternion(SerializationInfo info, StreamingContext context)
		{
			_data = (double[])info.GetValue("data", typeof(double[]));
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("data", _data);
		}
	}
}
