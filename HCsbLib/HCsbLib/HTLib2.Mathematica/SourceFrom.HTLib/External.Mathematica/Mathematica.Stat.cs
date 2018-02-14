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
		public static double[] ChiCdfs(IEnumerable<double> xs, double df)
		{
			string evaluate = "";
			evaluate += "cdf[x_]:=N[CDF[ChiSquareDistribution["+df.ToString("0.00000000")+"]][x]];";
			evaluate += "Table[cdf[x],{x," + vals2str(xs).ToString() + "}]";
			double[] result = EvaluateDoubles(evaluate);
			return result;
		}
		public static double[] ChiPdfs(IEnumerable<double> xs, double df)
		{
			string evaluate = "Table[PDF[ChiSquareDistribution[" + df.ToString("0.00000000") + "],x]+0."
							+ ",{x," + vals2str(xs).ToString() + "}]";

			double[] result = EvaluateDoubles(evaluate);
			return result;
		}
		public static double[] NormalPdfs(IEnumerable<double> xs, double mean, double var)
		{
			string evaluate = "N[Table[PDF[NormalDistribution[mean, var]][x], {x, xs}]]";
			evaluate = evaluate.Replace("mean", mean.ToString("0.00000000"));
			evaluate = evaluate.Replace("var", var.ToString("0.00000000"));
			evaluate = evaluate.Replace("xs", vals2str(xs).ToString());

			double[] result = EvaluateDoubles(evaluate);
			return result;
		}
	}
}
*/