using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
    public partial class Matlab
    {
        public static void PlotScatter
            ( double[] xs
            , double[] ys
            , string xlabel=null
            , string ylabel=null
            , string title=null
            )
        {
            Matlab.PutVector("htlib2_matlab_PlotScatter.xs", xs);
            Matlab.PutVector("htlib2_matlab_PlotScatter.ys", ys);
            Matlab.Execute("figure;");
            Matlab.Execute("hold on;");
            Matlab.Execute("scatter(htlib2_matlab_PlotScatter.xs, htlib2_matlab_PlotScatter.ys);");
            if(xlabel != null) Matlab.Execute("xlabel('" + xlabel + "');");
            if(ylabel != null) Matlab.Execute("ylabel('" + ylabel + "');");
            if(title  != null) Matlab.Execute("title('"  + title  + "');");
            Matlab.Execute("hold off;");
            Matlab.Execute("clear htlib2_matlab_PlotScatter;");
        }
    }
}
