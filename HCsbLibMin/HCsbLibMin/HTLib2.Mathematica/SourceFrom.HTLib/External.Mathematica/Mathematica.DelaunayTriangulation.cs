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
		public static void DelaunayTriangulation(IEnumerable<Vector> points, out string command)
		{
			StringBuilder evaluate = new StringBuilder();
			evaluate.Append("Needs[\"ComputationalGeometry`\"];");
			evaluate.Append("points="); evaluate.Append(Mathematica.ToString(points)); evaluate.Append(";");
			evaluate.Append("DelaunayTriangulation[points]-1");
			command = evaluate.ToString();
		}
		public static void DelaunayTriangulation(IEnumerable<Vector> points, out object result)
		{
			string command;
			DelaunayTriangulation(points, out command);
			result = EvaluateObject(command);
		}
	}
}
*/