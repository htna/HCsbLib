using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace HTLib2
{
    public partial class Matlab
    {
        public static void PlotScatter1
            ( params object[] infos
            )
        {
        }

        public static void PlotScatter
            ( IEnumerable<double> x
            , IEnumerable<double> y
            , IEnumerable<double> size = null
            , double? sizeall = null
            , IEnumerable<ValueTuple<double, double, double>> RGB = null
            , ValueTuple<double, double, double>? RGBall = null
            , string xlabel = null
            , string ylabel = null
            , string title  = null
            , bool filled       = false
            , bool init_figure  = true
            , bool init_holdon  = true
            , bool last_holdoff = true
            )
        {
            Matlab.PutVector("htlib2_matlab_PlotScatter.x" , x   .ToArray());
            Matlab.PutVector("htlib2_matlab_PlotScatter.y" , y   .ToArray());
            if(size != null) Matlab.PutVector("htlib2_matlab_PlotScatter.sz", size.ToArray());
            if(RGB  != null) Matlab.PutVector("htlib2_matlab_PlotScatter.R" , RGB.HListItem1().ToArray() );
            if(RGB  != null) Matlab.PutVector("htlib2_matlab_PlotScatter.G" , RGB.HListItem2().ToArray() );
            if(RGB  != null) Matlab.PutVector("htlib2_matlab_PlotScatter.B" , RGB.HListItem3().ToArray());
            if(RGB  != null) Matlab.Execute  ("htlib2_matlab_PlotScatter.RGB = [htlib2_matlab_PlotScatter.R; htlib2_matlab_PlotScatter.G; htlib2_matlab_PlotScatter.B];");
            if(init_figure ) Matlab.Execute("figure;" );
            if(init_holdon ) Matlab.Execute("hold on;");
            string script;
            {
                script = "scatter" +
                         "( htlib2_matlab_PlotScatter.x" +
                         ", htlib2_matlab_PlotScatter.y";
                // size
                if  (sizeall != null) script += ", " + sizeall.Value;
                else if(size != null) script += ", htlib2_matlab_PlotScatter.sz";
                else                  script += ", []";
                // color
                if  (RGBall != null) script += string.Format(", [{0},{1},{2}]", RGBall.Value.Item1, RGBall.Value.Item2, RGBall.Value.Item3);
                else if(RGB != null) script += ", htlib2_matlab_PlotScatter.RGB";
                // filled
                if(filled) script += ", 'filled'";
                // close
                script += ");";
            }
            Matlab.Execute(script);
            if(xlabel != null) Matlab.Execute("xlabel('" + xlabel + "');");
            if(ylabel != null) Matlab.Execute("ylabel('" + ylabel + "');");
            if(title  != null) Matlab.Execute("title('"  + title  + "');");
            if(last_holdoff  ) Matlab.Execute("hold off;");
            Matlab.Execute("clear htlib2_matlab_PlotScatter;");
        }
    }
}
