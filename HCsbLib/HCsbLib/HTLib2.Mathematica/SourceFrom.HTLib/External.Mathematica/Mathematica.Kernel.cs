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
		static Dictionary<string,object> _EvaluateCache = new Dictionary<string, object>();
		public static double[] EvaluateDoubles(string evaluate)
		{
			double[] result = null;
			if(_EvaluateCache.ContainsKey(evaluate))
			{
				result = (double[])_EvaluateCache[evaluate];
			}
			else
			{
				IKernelLink ml = MathLinkFactory.CreateKernelLink();
				ml.WaitAndDiscardAnswer();
				ml.Evaluate(evaluate);
				ml.WaitForAnswer();
				try
				{
					result = ml.GetDoubleArray();
				}
				catch
				{
					result = null;
				}
				ml.Close();
				_EvaluateCache.Add(evaluate, result);
			}
			return (double[])(result.Clone());
		}
		public static object EvaluateObject(string evaluate)
		{
			object result = null;
			if(_EvaluateCache.ContainsKey(evaluate))
			{
				result = _EvaluateCache[evaluate];
			}
			else
			{
				IKernelLink ml = MathLinkFactory.CreateKernelLink();
				ml.WaitAndDiscardAnswer();
				ml.Evaluate(evaluate);
				ml.WaitForAnswer();
				try
				{
					result = ml.GetObject();
				}
				catch
				{
					result = null;
				}
				ml.Close();
				_EvaluateCache.Add(evaluate, result);
			}
			return result;
		}
	}
}
*/