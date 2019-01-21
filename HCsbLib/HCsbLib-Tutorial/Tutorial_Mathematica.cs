using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace Tutorial
{
    partial class Program
    {
        public class Tutorial_Mathematica
        {
            public static void Main(string pathbase, string[] args)
            {
                List<double> xs = new List<double>();
                List<double> ys = new List<double>();
                for(double x=0; x<=Math.PI*2; x+=0.1)
                {
                    xs.Add(x);
                    ys.Add(Math.Sin(x));
                }

                string str_xs = Mathematica.ToString2(xs);
                string str_ys = Mathematica.ToString2(ys);
                string str_plot = "ListLinePlot[Transpose[{"+str_xs+","+str_ys+"}], PlotRange->All]";
                Mathematica.EvaluatePng(str_plot, 300, pathbase+"test_mathematica.png");


            }
        }
    }
}
