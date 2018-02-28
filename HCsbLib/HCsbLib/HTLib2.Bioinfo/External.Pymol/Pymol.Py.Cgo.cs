using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using StreamWriter = System.IO.StreamWriter;
    
    public partial class Pymol
	{
        public static partial class Py
        {
            public static class Cgo
            {
                public static void Init(string pypath, bool append=false)
                {
                    StreamWriter file;
                    if(append) file = HFile.AppendText(pypath);
                    else       file = HFile.CreateText(pypath);
                    file.WriteLine("from pymol.cgo import *");
                    file.WriteLine("from pymol import cmd");
                    file.Close();
                }
                public static void WriteCylinder
                    ( string pypath
                    , string objname
                    , double? alpha
                    , Vector pt1
                    , Vector pt2
                    , double rad
                    , Vector col1
                    , Vector col2
                    )
                {
                    WriteCylinder(pypath, objname, new Tuple<double?, Vector, Vector, double, Vector, Vector>[]
                        {
                            new Tuple<double?, Vector, Vector, double, Vector, Vector>(alpha, pt1, pt2, rad, col1, col2),
                        }
                        );
                }
                public static void WriteCylinder
                    ( string pypath
                    , string objname
                    , IList<Tuple<double?, Vector, Vector, double, Vector, Vector>> lstAlphaPt1Pt2RadRgb1Rgb2
                    )
                {
                    StreamWriter file = HFile.AppendText(pypath);
                    file.WriteLine("obj = [");
                    foreach(var item in lstAlphaPt1Pt2RadRgb1Rgb2)
                    {
                        double? alpha = item.Item1;
                        Vector  pt1   = item.Item2;
                        Vector  pt2   = item.Item3;
                        double  rad   = item.Item4;
                        Vector  rgb1  = item.Item5;
                        Vector  rgb2  = item.Item6;

                        string line = "    ";
                        if(alpha != null) line += "ALPHA, " + alpha.Value + ", ";
                        line += pt1[0] + ", " + pt1[1] + ", " + pt1[2] + ", ";
                        line += pt2[0] + ", " + pt2[1] + ", " + pt2[2] + ", ";
                        line += rad    + ", ";
                        line += rgb1[0] + ", " + rgb1[0] + ", " + rgb1[0] + ", ";
                        line += rgb2[0] + ", " + rgb2[0] + ", " + rgb2[0] + ", ";
                        file.WriteLine(line);
                    }
                    file.WriteLine("]");
                    file.WriteLine("cmd.load_cgo(obj,\"" + objname + "\")");
                    file.Close();
                }
            }
        }
    }
}
