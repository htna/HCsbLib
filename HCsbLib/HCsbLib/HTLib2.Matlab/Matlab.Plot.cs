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
            , IEnumerable<ValueTuple<double, double, double>> RGB = null
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
            string script = "scatter" +
                "( htlib2_matlab_PlotScatter.x" +
                ", htlib2_matlab_PlotScatter.y" +
                ((size != null) ? ", htlib2_matlab_PlotScatter.sz" : ", []") +
                ((RGB  != null) ? ", htlib2_matlab_PlotScatter.RGB" : "") +
                (filled ? "'filled'" : "") +
                ");";
            Matlab.Execute(script);
            if(xlabel != null) Matlab.Execute("xlabel('" + xlabel + "');");
            if(ylabel != null) Matlab.Execute("ylabel('" + ylabel + "');");
            if(title  != null) Matlab.Execute("title('"  + title  + "');");
            if(last_holdoff  ) Matlab.Execute("hold off;");
            Matlab.Execute("clear htlib2_matlab_PlotScatter;");
        }
    }
}
