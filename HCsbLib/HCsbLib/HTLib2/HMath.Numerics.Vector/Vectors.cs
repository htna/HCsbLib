using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HTLib2
{
    public partial class Vectors : ICloneable
	{
		public Vector[] _vecs;

		public Vectors(int length)
		{
			HDebug.Assert(length >= 0);
            _vecs = new Vector[length];
		}
		public Vectors(Vector[] vecs)
		{
            _vecs = new Vector[vecs.Length];
            for(int i=0; i<Length; i++)
                _vecs[i] = vecs[i];
		}
		public Vectors(double[][] vecs)
		{
            _vecs = new Vector[vecs.Length];
            for(int i=0; i<Length; i++)
                _vecs[i] = vecs[i];
        }
		
        //public static Vector From(object data)
        //{
        //    Type dataType = data.GetType();
        //    if(dataType.IsSubclassOf(typeof(double[]))) { return new Vector((double[])data); }
        //    Debug.Assert(false);
        //    return null;
        //}

        public int Length         { get { return _vecs.Length; } }
		public Vector this[int i] { get { HDebug.Assert(i >= 0 && i < Length); return _vecs[i]; }
			                        set { HDebug.Assert(i >= 0 && i < Length); _vecs[i] = value; }
                                  }

        public double[] GetDist()
        {
            double[] dist = new double[Length];
            for(int i=0; i<Length; i++)
                dist[i] = _vecs[i].Dist;
            return dist;
        }
        public double[] GetDist2()
        {
            double[] dist2 = new double[Length];
            for(int i=0; i<Length; i++)
                dist2[i] = _vecs[i].Dist2;
            return dist2;
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // ToMatrix
        public static implicit operator Vector[](Vectors vecs)
        {
            return vecs._vecs;
        }
        //public static implicit operator double[][](Vectors vecs)
        //{
        //    double[][] result = new double[vecs.Length][];
        //    for(int i=0; i<vecs.Length; i++)
        //        result[i] = vecs[i];
        //    return result;
        //}
        //public static implicit operator Vectors(double[][] vecs)
        //{
        //    return new Vectors(vecs);
        //}
        public static implicit operator Vectors(Vector[] vecs)
        {
            return new Vectors(vecs);
        }
        //public static explicit operator Matrix(Vector vec)
        //{
        //    return vec.ToColMatrix();
        //}

		public static Vectors operator+(Vectors l, Vectors r)
		{
			HDebug.Assert(l.Length == r.Length);
			int length = l.Length;
			Vectors result = new Vectors(length);
			for(int i=0; i<length; i++)
				result[i] = l[i] + r[i];
			return result;
		}
		public static Vectors operator-(Vectors l, Vectors r)
		{
			HDebug.Assert(l.Length == r.Length);
			int length = l.Length;
			Vectors result = new Vectors(length);
			for(int i=0; i<length; i++)
				result[i] = l[i] - r[i];
			return result;
		}
		public static Vectors operator -(Vectors v)
		{
			int length = v.Length;
			Vectors result = new Vectors(length);
			for(int i=0; i<length; i++)
				result[i] = -1 * v[i];
			return result;
		}
		public static Vectors operator*(double l, Vectors r)
		{
			int length = r.Length;
			Vectors result = new Vectors(length);
			for(int i=0; i<length; i++)
				result[i] = l * r[i];
			return result;
		}
		public static Vectors operator*(Vectors l, double r)
		{
			return r*l;
		}
		public static Vectors operator/(Vectors l, double r)
		{
            return l * (1/r);
		}

		public static bool IsNull(Vectors v)
		{
			return (v._vecs == null);
		}
		////////////////////////////////////////////////////////////////////////////////////
		// Functions
		public static Vector VtV(Vectors l, Vectors r)
		{
			HDebug.Assert(l.Length == r.Length);
			int length = l.Length;
            Vector result = new double[length];
            for(int i=0; i<length; i++)
                result[i] = LinAlg.VtV(l[i], r[i]);
            return result;
		}
		public Vectors UnitVectors()
		{
			Vectors result = new Vectors(Length);
            for(int i=0; i<Length; i++)
                result[i] = this[i].UnitVector();
            return result;
		}
        public Vector Norms(int p)
        {
            Vector norms = new double[Length];
            for(int i=0; i<Length; i++)
                norms[i] = this[i].Norm(p);
            return norms;
        }
        public Vector NormsInf()
        {
            Vector norms = new double[Length];
            for(int i=0; i<Length; i++)
                norms[i] = this[i].NormInf();
            return norms;
        }

		public bool IsNaN              { get { foreach(Vector vec in _vecs) if(vec.IsNaN             ) return true; return false; } }
		public bool IsInfinity         { get { foreach(Vector vec in _vecs) if(vec.IsInfinity        ) return true; return false; } }
		public bool IsPositiveInfinity { get { foreach(Vector vec in _vecs) if(vec.IsPositiveInfinity) return true; return false; } }
		public bool IsNegativeInfinity { get { foreach(Vector vec in _vecs) if(vec.IsNegativeInfinity) return true; return false; } }
		public bool IsComputable       { get { return ((IsNaN == false) && (IsInfinity == false)); } }

        ////////////////////////////////////////////////////////////////////////////////////
        // ToArray
        public Vector[] ToArray()
        {
            return _vecs;
        }
        ////////////////////////////////////////////////////////////////////////////////////
		// ToString
		public override string ToString()
		{
            return "" + Length + " vectors";
		}

		////////////////////////////////////////////////////////////////////////////////////
		// Serializable
        //public Vector(SerializationInfo info, StreamingContext ctxt)
        //{
        //    this._data = (double[])info.GetValue("data", typeof(double[]));
        //}
        //public void GetObjectData(SerializationInfo info, StreamingContext context)
        //{
        //    info.AddValue("data", this._data);
        //}

		////////////////////////////////////////////////////////////////////////////////////
		// ICloneable
		object ICloneable.Clone()
		{
			return Clone();
		}
		public Vectors Clone()
		{
			Vectors result = new Vectors(Length);
			for(int i=0; i<Length; i++)
				result[i] = this[i].Clone();
			return result;
		}

		////////////////////////////////////////////////////////////////////////////////////
		// Object
        //override public int GetHashCode()
        //{
        //    return _data.GetHashCode();
        //}
        //public override bool Equals(object obj)
        //{
        //    if(typeof(Vector).IsInstanceOfType(obj) == false)
        //        return false;
        //    return (this == ((Vector)obj));
        //}
	}
}
