using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
	[Serializable]
	public partial class Vector : IVector<double>, ICloneable
	{
		public double[] _data;

		public Vector(int size)
		{
			HDebug.Assert(size >= 0);
			this._data = new double[size];
		}
		public Vector(int size, double initval)
		{
			HDebug.Assert(size >= 0);
			this._data = new double[size];
			for(int i=0; i<size; i++)
				_data[i] = initval;
		}
        //static char[] separator { get { return " ,\t".ToCharArray(); } }
        //public Vector(string vec)
        //{
        //    try
        //    {
        //        string[] data = vec.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        //        _data = new double[data.Length];
        //        for(int i=0; i<data.Length; i++)
        //            _data[i] = double.Parse(data[i]);
        //    }
        //    catch(Exception)
        //    {
        //        Debug.Break();
        //        _data = null;
        //    }
        //}
		public Vector(Vector vec)
		{
			this._data = new double[vec.Size];
			for(int i=0; i<Size; i++)
				_data[i] = vec._data[i];
		}
		public Vector(params double[] data)
		{
			this._data = data;
		}
        public Vector(params double[][] valuess)
        {
            _data = new double[valuess.HCount().Sum()];
            int i=0;
            foreach(double[] values in valuess)
                foreach(double value in values)
                {
                    this[i] = value;
                    i++;
                }
        }

        //public Vector(int size, params double[] data)
        //{
        //    Debug.Assert(false);
        //    Debug.Assert(size >= 0);
        //    this._data = new double[size];
        //    for(int i=0; i<data.Length; i++)
        //        this[i] = data[i];
        //}
        public long SizeLong
        {
            get
            {
                return _data.Length;
            }
        }
        public int Size
		{
			get
			{
				return _data.Length;
			}
		}
        public double this[long i]
		{
			get
			{
				HDebug.Assert(i >= 0 && i < Size);
				return _data[i];
			}
			set
			{
				HDebug.Assert(i >= 0 && i < Size);
				_data[i] = value;
			}
		}

		public bool IsNaN              { get { for(int i=0; i<Size; i++) if(double.IsNaN             (this[i])) return true; return false; } }
		public bool IsInfinity         { get { for(int i=0; i<Size; i++) if(double.IsInfinity        (this[i])) return true; return false; } }
		public bool IsPositiveInfinity { get { for(int i=0; i<Size; i++) if(double.IsPositiveInfinity(this[i])) return true; return false; } }
		public bool IsNegativeInfinity { get { for(int i=0; i<Size; i++) if(double.IsNegativeInfinity(this[i])) return true; return false; } }
		public bool IsComputable       { get { return ((IsNaN == false) && (IsInfinity == false)); } }

        public void SetZero()              { for(int i=0; i<Size; i++) this[i] = 0; }
        public void SetOne ()              { for(int i=0; i<Size; i++) this[i] = 1; }
        public void SetValue(double value) { for(int i=0; i<Size; i++) this[i] = value; }

		////////////////////////////////////////////////////////////////////////////////////
		// ToMatrix
		public static implicit operator double[](Vector vec)
		{
			return vec._data;
		}
		public static implicit operator Vector(double[] vec)
		{
			return new Vector(vec);
		}
		public static explicit operator MatrixByArr(Vector vec)
		{
			return vec.ToColMatrix();
		}
        static bool operator_Equal_SelfTest = HDebug.IsDebuggerAttached;
		public static bool operator==(Vector lvec, Vector rvec)
		{
            if(operator_Equal_SelfTest)
                #region self test
            {
                operator_Equal_SelfTest = false;
                Vector v0 = new double[] { 1, 2, 3, 4 };
                Vector v1 = new double[] { 1, 2, 3, 4 };
                Vector v2 = new double[] { 0, 2, 3, 4 };
                Vector v3 = new double[] { 1, 2, 3 };
                bool valid = true;
                if((v0 == v1) == false) valid = false;
                if((v0 != v2) == false) valid = false;
                if((v0 != v3) == false) valid = false;
                HDebug.Assert(valid);
            };
                #endregion
            object lobj = lvec;
            object robj = rvec;

            if(object.ReferenceEquals(lvec, rvec)) return true;
            if((lobj != null && robj == null) || (lobj == null && robj != null)) return false;
			if(lvec._data == rvec._data) return true;
			if(lvec._data==null ||rvec._data==null) return false;
			if(lvec.Size != rvec.Size) return false;
			for(int i=0; i<lvec.Size; i++)
				if(lvec[i] != rvec[i])
					return false;
			return true;
		}
		public static bool operator!=(Vector lvec, Vector rvec)
		{
			return !(lvec == rvec);
		}
		public static Vector operator+(Vector l, Vector r)
		{
			HDebug.Assert(l.Size == r.Size);
			int size = l.Size;
			Vector result = new Vector(size);
			for(int i=0; i<size; i++)
				result[i] = l[i] + r[i];
			return result;
		}
		public static Vector operator-(Vector l, Vector r)
        {
            return Sub(l._data, r._data);
        }
		public static Vector Sub(double[] l, double[] r)
		{
            HDebug.Assert(l.Length == r.Length);
            int size = l.Length;
			Vector result = new Vector(size);
			for(int i=0; i<size; i++)
				result[i] = l[i] - r[i];
			return result;
		}
		public static Vector operator -(Vector v)
		{
			int size = v.Size;
			Vector result = new Vector(size);
			for(int i=0; i<size; i++)
				result[i] = -1 * v[i];
			return result;
		}
		public static Vector operator*(double l, Vector r)
		{
			int size = r.Size;
			Vector result = new Vector(size);
			for(int i=0; i<size; i++)
				result[i] = l * r[i];
			return result;
		}
		public static Vector operator*(Vector l, double r)
		{
			return r*l;
		}
		public static Vector operator/(Vector l, double r)
		{
			int size = l.Size;
			Vector result = new Vector(size);
			for(int i=0; i<size; i++)
				result[i] = l[i] / r;
			return result;
		}

		public static bool IsNull(Vector vec)
		{
			return (vec._data == null);
		}
		public double Sum()
		{
			double sum = 0;
			for(int i=0; i<Size; i++)
				sum += this[i];
			return sum;
		}
		public double SumSquared()
		{
			double sum = 0;
			for(int i=0; i<Size; i++)
				sum += (this[i] * this[i]);
			return sum;
		}
		////////////////////////////////////////////////////////////////////////////////////
		// Functions
		public double Dist2
		{
			get
			{
				double result = 0;
				for(int i=0; i<Size; i++)
					result += this[i] * this[i];
                //HDebug.Assert(double.IsNaN(result) == false, double.IsInfinity(result) == false);
				return result;
			}
		}
		public double Dist
		{
			get
			{
                double dist = Math.Sqrt(Dist2);
				return dist;
			}
		}
		public Vector UnitVector()
		{
			double length = 1.0 / Math.Sqrt(_VtV(this, this));
			return this * length;
		}
		public static void AddTo(IList<Vector> dest, IList<Vector> add)
		{
			HDebug.Assert(dest.Count == add.Count);
			for(int i=0; i<dest.Count; i++)
				dest[i] += add[i];
		}

		////////////////////////////////////////////////////////////////////////////////////
		// for Collection
		public static Vector Average(IList<Vector> list)
		{
			if(list.Count <= 0)
				return null;
			Vector avg = list[0];
			for(int i=1; i<list.Count; i++)
			{
				if(list[i].Size != avg.Size)
					return null;
				avg += list[i];
			}
			avg /= list.Count;
			return avg;
		}
		////////////////////////////////////////////////////////////////////////////////////
		// ToArray
		public double[] ToArray()
		{
			return _data;
		}
		////////////////////////////////////////////////////////////////////////////////////
		// ToString
		public override string ToString()
		{
            StringBuilder str = new StringBuilder();
            str.Append("Vector ["+Size+"] ");
            str.Append(HToString("0.00000", null, "{", "}", ", "));
            return str.ToString();
		}
		public string ToString(string format)
		{
            StringBuilder str = new StringBuilder();
            str.Append("Vector ["+Size+"] ");
            str.Append(HToString(format, null, "{", "}", ", "));
            return str.ToString();
		}
        public string HToString(string format="0.00000"
                               , IFormatProvider formatProvider=null
                               , string begindelim  = "{"
                               , string enddelim    = "}"
                               , string delim       = ", "
                               )
		{
			StringBuilder str = new StringBuilder();
			str.Append(begindelim);

			for(int i=0; i<Size; i++)
			{
				if(i != 0) str.Append(delim);
				str.Append(this[i].ToString(format));
			}

			str.Append(enddelim);
			return str.ToString();
		}

		////////////////////////////////////////////////////////////////////////////////////
		// Serializable
		public Vector(SerializationInfo info, StreamingContext ctxt)
		{
			this._data = (double[])info.GetValue("data", typeof(double[]));
		}
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("data", this._data);
		}

		////////////////////////////////////////////////////////////////////////////////////
		// ICloneable
		object ICloneable.Clone()
		{
			return Clone();
		}
		public Vector Clone()
		{
			double[] data = null;
			if(_data != null)
				data = (double[])_data.Clone();
			return new Vector(data);
		}

		////////////////////////////////////////////////////////////////////////////////////
		// Object
		override public int GetHashCode()
		{
			return _data.GetHashCode();
		}
		public override bool Equals(object obj)
		{
			if(typeof(Vector).IsInstanceOfType(obj) == false)
				return false;
			return (this == ((Vector)obj));
		}


        static double _VtV(Vector l, Vector r)
        {
            // same to LinAlg.VtV
            HDebug.Assert(l.Size == r.Size);
            int size = l.Size;
            double result = 0;
            for(int i = 0; i < size; i++)
                result += l[i] * r[i];
            return result;
        }
    }
}
