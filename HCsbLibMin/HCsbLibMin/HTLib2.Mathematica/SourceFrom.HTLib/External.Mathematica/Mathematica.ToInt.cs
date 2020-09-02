/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wolfram.NETLink;

// C:\Program Files\Wolfram Research\Mathematica\7.0\SystemFiles\Links\NETLink

namespace HTLib
{
	public partial class Mathematica
	{
		public static int ToScalarInt(object value)
		{
			Debug.Assert(value is string);
			return int.Parse((string)value);
		}
		public static double ToScalarDouble(object value)
		{
			Debug.Assert(value is string);
			string val = (string)value;
			val = val.Replace("*10^", "e");
			return double.Parse(val);
		}


		public delegate T ToScalar<T>(object value);
		public static T[] ToVector<T>(object values, ToScalar<T> toscalar)
		{
			object[] objs = (object[])values;
			T[] ts = new T[objs.Length];
			for(int i=0; i<ts.Length; i++)
			{
				ts[i] = toscalar(objs[i]);
			}
			return ts;
		}
		public static T[][] ToVector2<T>(object values, ToScalar<T> toscalar)
		{
			object[] objs = (object[])values;
			T[][] ts = new T[objs.Length][];
			for(int i=0; i<ts.Length; i++)
				ts[i] = ToVector<T>(objs[i], toscalar);
			return ts;
		}
		public static T[][][] ToVector3<T>(object values, ToScalar<T> toscalar)
		{
			object[] objs = (object[])values;
			T[][][] ts = new T[objs.Length][][];
			for(int i=0; i<ts.Length; i++)
				ts[i] = ToVector2<T>(objs[i], toscalar);
			return ts;
		}
		public static T[,] ToMatrix<T>(object values, ToScalar<T> toscalar)
		{
			object[,] objs = (object[,])values;
			T[,] ts = new T[objs.GetLength(0), objs.GetLength(1)];
			for(int i=0; i<ts.GetLength(0); i++)
				for(int j=0; j<ts.GetLength(1); j++)
					ts[i, j] = toscalar(objs[i, j]);
			return ts;
		}
		public static T[,] ToMatrix<T>(T[][] values)
		{
			Debug.AssertAnd(values.Length >= 1, values[0].Length >= 1);
			T[,] ts = new T[values.Length, values[0].Length];
			for(int i=0; i<values.Length; i++)
			{
				Debug.Assert(values[i].Length == ts.GetLength(1));
				for(int j=0; j<values[i].Length; j++)
					ts[i, j] = values[i][j];
			}
			return ts;
		}
	}
}
*/