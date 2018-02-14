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
		static StringBuilder vals2str(IEnumerable<double> points)
		{
			StringBuilder str = new StringBuilder();
			str.Append("{");
			bool addcomma = false;
			foreach(double point in points)
			{
				if(addcomma) str.Append(",");
				else addcomma = true;
				str.Append(point.ToString("0.00000000"));
			}
			str.Append("}");
			return str;
		}
		static StringBuilder vals2str(IEnumerable<DoubleVector2> points)
		{
			StringBuilder str = new StringBuilder();
			str.Append("{");
			bool addcomma = false;
			foreach(DoubleVector2 point in points)
			{
				if(addcomma) str.Append(",");
				else addcomma = true;
				str.Append("{" + point.x.ToString("0.00000000") + "," + point.y.ToString("0.00000000") + "}");
			}
			str.Append("}");
			return str;
		}
		static StringBuilder valss2str(IEnumerable<IEnumerable<DoubleVector2>> pointss)
		{
			StringBuilder str = new StringBuilder();
			str.Append("{");
			bool addcomma = false;
			foreach(IEnumerable<DoubleVector2> points in pointss)
			{
				if(addcomma) str.Append(",");
				else addcomma = true;
				str.Append(vals2str(points));
			}
			str.Append("}");
			return str;
		}
	}
}
*/