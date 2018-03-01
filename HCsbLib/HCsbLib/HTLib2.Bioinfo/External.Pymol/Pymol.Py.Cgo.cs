using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using StreamWriter = System.IO.StreamWriter;
    
    public partial class Pymol
	{
        public static partial class Script
        {
            public static class Cgo
            {
                //  public abstract class CgoElement
                //  {
                //      public override abstract string ToString();
                //  };
                //  public class CgoAlpha : CgoElement
                //  {
                //      public double alpha;
                //      public override string ToString()
                //      {
                //          return string.Format("ALPHA, {0}, ", alpha);
                //      }
                //  };
                //  public class CgoCylinder : CgoElement
                //  {
                //      public Vector pt1;
                //      public Vector pt2;
                //      public double radius;
                //      public double red1, green1, blue1;
                //      public double red2, green2, blue2;
                //      public double red   { set { red1   = red2   = value; } }
                //      public double green { set { green1 = green2 = value; } }
                //      public double blue  { set { blue1  = blue2   = value; } }
                //      public override string ToString()
                //      {
                //          return string.Format("CYLINDER, {0}, {1}, {2},    {3},    {4}, {5}, {6},    {7}, {8}, {9}, "
                //              , pt1[0], pt1[1], pt1[2]
                //              , pt2[0], pt2[1], pt2[2]
                //              , radius
                //              , red1, green1, blue1
                //              , red2, green2, blue2
                //              );
                //      }
                //  }
                public class CgoElements
                {
                    protected internal List<string> lines = new List<string>();
                    public void AddAlpha(double alpha)
                    {
                        string line = string.Format("ALPHA, {0}, ", alpha);
                        lines.Add(line);
                    }
                    public void AddCylinder(Vector pt1, Vector pt2, double radius, double red, double green, double blue)
                    {
                        AddCylinder(pt1, pt2, radius, red, green, blue, red, green, blue);
                    }
                    public void AddCylinder(Vector pt1, Vector pt2, double radius, double red1, double green1, double blue1, double red2, double green2, double blue2)
                    {
                        string line = string.Format("CYLINDER, {0},{1},{2},    {3},{4},{5},    {6},    {7},{8},{9},    {10},{11},{12}, "
                            , pt1[0], pt1[1], pt1[2]
                            , pt2[0], pt2[1], pt2[2]
                            , radius
                            , red1, green1, blue1
                            , red2, green2, blue2
                            );
                        lines.Add(line);
                    }
                }

                public static CgoElements NewCgoElements()
                {
                    return new CgoElements();
                }
                public static void WriteImport(string pypath, bool append=false)
                {
                    StreamWriter file;
                    if(append) file = HFile.AppendText(pypath);
                    else       file = HFile.CreateText(pypath);
                    file.WriteLine("from pymol.cgo import *");
                    file.WriteLine("from pymol import cmd");
                    file.Close();
                }
                public static void WriteCgoObject
                    ( string pypath
                    , string objname
                    , CgoElements cgoelems
                    )
                {
                    List<string> lines = new List<string>();

                    lines.Add("obj = [");
                    foreach(var cgoline in cgoelems.lines)
                        lines.Add("    " + cgoline);
                    lines.Add("]");
                    lines.Add("cmd.load_cgo(obj,\"" + objname + "\")");

                    HFile.AppendAllLines(pypath, lines);
                }

                [Obsolete]
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
                [Obsolete]
                public static void WriteCylinder
                    ( string pypath
                    , string objname
                    , IList<Tuple<double?, Vector, Vector, double, Vector, Vector>> lstAlphaPt1Pt2RadRgb1Rgb2
                    )
                {
                    var cgoelems = NewCgoElements();
                    foreach(var item in lstAlphaPt1Pt2RadRgb1Rgb2)
                    {
                        double? alpha = item.Item1;
                        Vector  pt1   = item.Item2;
                        Vector  pt2   = item.Item3;
                        double  rad   = item.Item4;
                        Vector  rgb1  = item.Item5;
                        Vector  rgb2  = item.Item6;

                        if(alpha != null) cgoelems.AddAlpha(alpha.Value);
                        cgoelems.AddCylinder
                            ( pt1, pt2
                            , rad
                            , rgb1[0], rgb1[1], rgb1[2]
                            , rgb2[0], rgb2[1], rgb2[2]
                            );
                    }
                    WriteCgoObject(pypath, objname, cgoelems);
                }
            }
        }
    }
}
