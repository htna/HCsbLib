/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib
{
	public partial class Mathematica
	{
		public static void ListPlot(IEnumerable<double> points)
		{
			MathematicaPlotForm dlg = new MathematicaPlotForm("ListPlot[" + vals2str(points) + "]");
			dlg.ShowDialog();
		}
		public static void ListPlot(IEnumerable<DoubleVector2> points)
		{
			MathematicaPlotForm dlg = new MathematicaPlotForm("ListPlot[" + vals2str(points) + "]");
			dlg.ShowDialog();
		}
		public static void ListLinePlot(IEnumerable<DoubleVector2> points)
		{
			MathematicaPlotForm dlg = new MathematicaPlotForm("ListLinePlot[" + vals2str(points) + "]");
			dlg.ShowDialog();
		}
		public static void ListLinePlot(IEnumerable<double> xs, IEnumerable<double> ys)
		{
			MathematicaPlotForm dlg = new MathematicaPlotForm("ListLinePlot[" + vals2str((new DoubleVector2s(xs, ys)).ToArray()) + "]");
			dlg.ShowDialog();
		}
		public static void ListLinePlot(params IEnumerable<DoubleVector2>[] pointss)
		{
			MathematicaPlotForm dlg = new MathematicaPlotForm("ListLinePlot[" + valss2str(pointss) + ",PlotRange->All]");
			dlg.ShowDialog();
		}
	}
}
*/