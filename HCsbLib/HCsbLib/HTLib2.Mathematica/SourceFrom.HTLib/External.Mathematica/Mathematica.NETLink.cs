/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib
{
	public partial class Mathematica
	{
		public class NETLink
		{
			//public static double[,] ToDouble(int[,] src)
			//{
			//    double[,] dest = new double[src.GetLength(0), src.GetLength(1)];
			//    for(int i=0; i<src.GetLength(0); i++)
			//        for(int j=0; j<src.GetLength(1); j++)
			//            dest[i, j] = src[i, j];
			//    return dest;
			//}
			//public static double[] ToDouble(int[] src)
			//{
			//    double[] dest = new double[src.Length];
			//    for(int i=0; i<src.Length; i++)
			//        dest[i] = src[i];
			//    return dest;
			//}
			public static void Throw(string exception_message)
			{
				throw new Exception(exception_message);
			}
			public static T Throw<T>(string exception_message)
			{
				throw new Exception(exception_message);
			}
			public static double ToDouble(object value, string exception_message)
			{
				if(value is double) return (double)value;
				if(value is int) return (int)value;
				return Throw<int>(exception_message);
			}
			public static double[] ToDoubleArr(object value, string exception_message)
			{
				if(value is double[]) return (double[])value;
				if(value is int[])
				{
					int[] value_ = (int[])value;
					double[] _value = new double[value_.Length];
					for(int i=0; i<_value.Length; i++) _value[i] = value_[i];
					return _value;
				}
				return Throw<double[]>(exception_message);
			}
			public static double[,] ToDoubleArr2(object value, string exception_message)
			{
				if(value is double[,]) return (double[,])value;
				if(value is int[,])
				{
					int[,] value_ = (int[,])value;
					double[,] _value = new double[value_.GetLength(0), value_.GetLength(1)];
					for(int i=0; i<_value.GetLength(0); i++)
						for(int j=0; j<_value.GetLength(1); j++)
							_value[i, j] = value_[i, j];
					return _value;
				}
				return Throw<double[,]>(exception_message);
			}
			public static int ToInt(object value, string exception_message)
			{
				if(value is int) return (int)value;
				return Throw<int>(exception_message);
			}
			public static int[] ToIntArr(object value, string exception_message)
			{
				if(value is int[]) return (int[])value;
				return Throw<int[]>(exception_message);
			}
			public static int[,] ToIntArr2(object value, string exception_message)
			{
				if(value is int[,]) return (int[,])value;
				return Throw<int[,]>(exception_message);
			}
			public static Vector[] ToVectorArr(object value, string exception_message)
			{
				double[,] mat = ToDoubleArr2(value, exception_message);
				Vector[] vectors = new Vector[mat.GetLength(0)];
				for(int i=0; i<vectors.Length; i++)
				{
					List<double> vector = new List<double>();
					for(int j=0; j<mat.GetLength(1); j++)
						vector.Add(mat[i, j]);
					vectors[i] = vector.ToArray();
				}
				return vectors;
			}

			public static double[,] TestMathBridge(object nodes, object triags, object thick, object E, object v, object groundids, object forceids, object forcevalues)
			{
				try
				{
					Vector[]  _nodes       = ToVectorArr(nodes, "nodes type does not match");
					double    _thick       = ToDouble(thick, "thick type does not match");
					double    _E           = ToDouble(E, "E type does not match");
					double    _v           = ToDouble(v, "v type does not match");
					int[]     _groundids   = ToIntArr(groundids, "groundids type does not match");
					int[]     _forceids    = ToIntArr(forceids, "forceids type does not match");
					double[]  _forcevalues = ToDoubleArr(forcevalues, "forcevalues type does not match");
					if(_forceids.Length == _forcevalues.Length) Throw("the size of forceids is not same to that of forcevalues");


					Vector[] dnodes_ = new Vector[_nodes.Length/2];
					for(int i=0; i<_nodes.Length; i+= 2)
					{
						dnodes_[i] = _nodes[i*2];
						if(i*2+1 < _nodes.Length)
						{
							dnodes_[i] += _nodes[i*2+1];
							dnodes_[i] /= 2;
						}
					}

					Debug.Assert(dnodes_[0].Size == 2);
					Debug.Assert(dnodes_.Length==_nodes.Count());
					double[,] dnodes = new double[dnodes_.Length, 2];
					for(int i=0; i<_nodes.Count(); i++)
					{
						Debug.Assert(dnodes_[i].Size == 2);
						dnodes[i, 0] = dnodes_[i][0];
						dnodes[i, 1] = dnodes_[i][1];
					}
					return dnodes;
				}
				catch(Exception e)
				{
					throw e;
				}
			}
		}
	}
}
*/