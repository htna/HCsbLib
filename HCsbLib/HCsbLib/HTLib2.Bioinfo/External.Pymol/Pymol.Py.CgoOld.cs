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
            public static class CgoOld
            {
                public static void WriteBlank( string pmlpath
                                             , bool append
                                             )
                {
                    StreamWriter file;
                    if(append) file = HFile.AppendText(pmlpath);
                    else       file = HFile.CreateText(pmlpath);
                    file.Close();
                }
                public static void WriteLines( string pmlpath
                                             , string objname
                                             , IList<Vector> pathcoords
                                             , double? linewidth = null
                                             , double alpha=1.0, double red=1.0, double green=1.0, double blue=1.0
                                             , bool append=false
                                             )
                {
                    #region python source
                    //def writeChannelPath(pypath, pathcoords):
                    //    file = open(pypath, 'w');
                    //    file.write('from pymol.cgo import *'+'\n');
                    //    file.write('from pymol import cmd'+'\n');
                    //    file.write('obj = ['+'\n');
                    //    file.write('BEGIN, LINES,'+'\n');
                    //    for idx in range(pathcoords.shape[0]-1):
                    //        vert1 = pathcoords[idx,:];
                    //        vert2 = pathcoords[idx+1,:];
                    //        file.write('ALPHA,    1.000, COLOR,   1.000,   0.000,   0.000, ');
                    //        file.write('VERTEX,   %6.3f,  %6.3f,   %6.3f, ' % (vert1[0], vert1[1], vert1[2]));
                    //        file.write('VERTEX,   %6.3f,  %6.3f,   %6.3f, ' % (vert2[0], vert2[1], vert2[2]));
                    //        file.write('\n');
                    //        pass
                    //    file.write('END'+'\n');
                    //    file.write(']'+'\n');
                    //    file.write('cmd.load_cgo(obj,"channel_path")'+'\n');
                    //    file.close();
                    //    return
                    #endregion

                    StreamWriter file;
                    if(append) file = HFile.AppendText(pmlpath);
                    else       file = HFile.CreateText(pmlpath);
                    {
                        //StreamWriter file = File.AppendText(pypath);
                        file.WriteLine("from pymol.cgo import *");
                        file.WriteLine("from pymol import cmd");
                        file.WriteLine("obj = [");
                        file.WriteLine("BEGIN, LINES,");
                        if(linewidth != null)
                            file.WriteLine("LINEWIDTH, {0:0.000},", linewidth);
                        for(int i=0; i<pathcoords.Count/2; i++)
                        {
                            Vector vert1 = pathcoords[i*2+0];
                            Vector vert2 = pathcoords[i*2+1];
                            file.Write("ALPHA,    {0:0.000}, COLOR,   {1:0.000},   {2:0.000},   {3:0.000}, ", alpha, red, green, blue);
                            file.Write("VERTEX,   {0:0.000},  {1:0.000},   {2:0.000}, ", vert1[0], vert1[1], vert1[2]);
                            file.Write("VERTEX,   {0:0.000},  {1:0.000},   {2:0.000}, ", vert2[0], vert2[1], vert2[2]);
                            file.WriteLine();
                        }
                        file.WriteLine("END");
                        file.WriteLine("]");
                        // file.WriteLine("cmd.load_cgo(obj,\"channel_path\")");
                        file.WriteLine("cmd.load_cgo(obj,\""+objname+"\")");
                        file.WriteLine();
                    }
                    file.Close();
                }
                public static void WriteLineStrip( string pmlpath
                                                 , string objname
                                                 , IList<Vector> pathcoords
                                                 , double? linewidth = null
                                                 , double alpha=1.0, double red=1.0, double green=1.0, double blue=1.0
                                                 , bool append=false
                                                 )
                {
                    StreamWriter file;
                    if(append) file = HFile.AppendText(pmlpath);
                    else file = HFile.CreateText(pmlpath);
                    {
                        //StreamWriter file = File.AppendText(pypath);
                        file.WriteLine("from pymol.cgo import *");
                        file.WriteLine("from pymol import cmd");
                        file.WriteLine("obj = [");
                        file.WriteLine("BEGIN, LINE_STRIP,");
                        if(linewidth != null)
                            file.WriteLine("LINEWIDTH, {0:0.000},", linewidth);
                        file.WriteLine("ALPHA,    {0:0.000}, COLOR,   {1:0.000},   {2:0.000},   {3:0.000}, ", alpha, red, green, blue);
                        for(int i=0; i<pathcoords.Count; i++)
                        {
                            Vector vert = pathcoords[i];
                            file.Write("VERTEX,   {0:0.000},  {1:0.000},   {2:0.000}, ", vert[0], vert[1], vert[2]);
                        }
                        file.WriteLine();
                        file.WriteLine("END");
                        file.WriteLine("]");
                        // file.WriteLine("cmd.load_cgo(obj,\"channel_path\")");
                        file.WriteLine("cmd.load_cgo(obj,\""+objname+"\")");
                        file.WriteLine();
                    }
                    file.Close();
                }
                public static void WriteLineStrips( string pmlpath
                                                  , string objname
                                                  , IList<LineStrip> linestrips
                                                  , bool append=false
                                                  )
                {
                    StreamWriter file;
                    if(append) file = HFile.AppendText(pmlpath);
                    else file = HFile.CreateText(pmlpath);
                    {
                        //StreamWriter file = File.AppendText(pypath);
                        file.WriteLine("from pymol.cgo import *");
                        file.WriteLine("from pymol import cmd");
                        file.WriteLine("obj = [");
                        foreach(var linestrip in linestrips)
                        {
                            file.WriteLine("BEGIN, LINE_STRIP,");
                            if(linestrip.linewidth != null)
                                file.WriteLine("       LINEWIDTH, {0:0.000},", linestrip.linewidth);
                            file.WriteLine("       ALPHA,    {0:0.000}, COLOR,   {1:0.000},   {2:0.000},   {3:0.000}, ", linestrip.alpha, linestrip.red, linestrip.green, linestrip.blue);
                            for(int i=0; i<linestrip.coords.Count; i++)
                            {
                                Vector vert = linestrip.coords[i];
                                file.Write("       VERTEX,   {0:0.000},  {1:0.000},   {2:0.000}, ", vert[0], vert[1], vert[2]);
                            }
                            file.WriteLine();
                            file.WriteLine("       END,");
                        }
                        file.WriteLine("]");
                        // file.WriteLine("cmd.load_cgo(obj,\"channel_path\")");
                        file.WriteLine("cmd.load_cgo(obj,\""+objname+"\")");
                        file.WriteLine();
                    }
                    file.Close();
                }
                public static void WriteCircles( string pmlpath
                                               , string objname
                                               , IList<Tuple<Vector,Vector,Vector>> lst_circle_verts
                                               , int numLinesInCircle = 50
                                               , double? linewidth = null
                                               , double alpha=1.0, double red=1.0, double green=1.0, double blue=1.0
                                               , bool append=false
                                               )
                {
                    List<Tuple<Vector, Vector, double>> lst_center_normal_radii = new List<Tuple<Vector, Vector, double>>();
                    foreach(var circle_verts in lst_circle_verts)
                    {
                        Vector pt1 = circle_verts.Item1;
                        Vector pt2 = circle_verts.Item2;
                        Vector pt3 = circle_verts.Item3;
                        double a, b, c, A, B, C, radii;
                        Vector center;
                        Geometry.Triangle.GetTriGeom(pt1, pt2, pt3, out a, out b, out c, out A, out B, out C, out radii, out center);
                        Vector normal = Geometry.PlaneNormal(pt1, pt2, pt3);
                        lst_center_normal_radii.Add(new Tuple<Vector, Vector, double>(center, normal, radii));
                    }
                    WriteCircles( pmlpath
                                , objname
                                , lst_center_normal_radii
                                , numLinesInCircle:numLinesInCircle
                                , linewidth:linewidth
                                , alpha:alpha, red:red, green:green, blue:blue
                                , append:append
                                );
                }
                public static void WriteCircles( string pmlpath
                                    , string objname
                                    , IList<Tuple<Vector,Vector,double>> lst_center_normal_radii
                                    , int numLinesInCircle = 50
                                    , double? linewidth = null
                                    , double alpha=1.0, double red=1.0, double green=1.0, double blue=1.0
                                    , bool append=false
                                    )
                {
                    List<Circle> circles = new List<Circle>();
                    foreach(var center_normal_radii in lst_center_normal_radii)
                    {
                        circles.Add(new Circle{
                            center = center_normal_radii.Item1,
                            normal = center_normal_radii.Item2,
                            radii  = center_normal_radii.Item3,
                            numLinesInCircle = numLinesInCircle,
                            alpha = alpha,
                            color = new Tuple<double,double,double>(red,green,blue),
                            linewidth = linewidth
                        });
                    }
                    WriteCircles(pmlpath, objname, circles, append);
                }
                public class LineStrip
                {
                    public IList<Vector> coords;
                    public double? linewidth = null;
                    public double alpha= 1.0;
                    public double red=1.0;
                    public double green=1.0;
                    public double blue=1.0;
                }
                public class Circle
                {
                    public Tuple<Vector,Vector,double> GetCenterNormalRadii(Vector pt1, Vector pt2, Vector pt3)
                    {
                        double a, b, c, A, B, C, radii;
                        Vector center;
                        Geometry.Triangle.GetTriGeom(pt1, pt2, pt3, out a, out b, out c, out A, out B, out C, out radii, out center);
                        Vector normal = Geometry.PlaneNormal(pt1, pt2, pt3);
                        return new Tuple<Vector, Vector, double>(center, normal, radii);
                    }
                    public Tuple<Vector,Vector,Vector> center_normal_radii
                    {
                        set
                        {
                            //Tuple<Vector,Vector,double> GetCenterNormalRadii(Vector pt1, Vector pt2, Vector pt3)
                            Vector pt1 = value.Item1;
                            Vector pt2 = value.Item2;
                            Vector pt3 = value.Item3;
                            double a, b, c, A, B, C;
                            Geometry.Triangle.GetTriGeom(pt1, pt2, pt3, out a, out b, out c, out A, out B, out C, out radii, out center);
                            normal = Geometry.PlaneNormal(pt1, pt2, pt3);
                        }
                    }
                    public Vector center       ;
                    public Vector normal       ;
                    public double radii        ;
                    public int numLinesInCircle              = 50;
                    public double? alpha                     = null;
                    public Tuple<double,double,double> color = null; // red, green, blue
                    public double? linewidth                 = null;
                };
                public static void WriteCircles( string pmlpath
                                               , string objname
                                               , IList<Circle> lst_circle
                                               //, int numLinesInCircle = 50
                                               //, double? linewidth = null
                                               //, double alpha=1.0, double red=1.0, double green=1.0, double blue=1.0
                                               , bool append=false
                                               )
                {
                    StreamWriter file;
                    if(append) file = HFile.AppendText(pmlpath);
                    else file = HFile.CreateText(pmlpath);
                    {
                        //StreamWriter file = File.AppendText(pypath);
                        file.WriteLine("from pymol.cgo import *");
                        file.WriteLine("from pymol import cmd");
                        file.WriteLine("obj = [");
                        foreach(var circle in lst_circle)
                        {
                            //Vector center        = center_normal_radii_numseg_color_width.Item1;
                            //Vector normal        = center_normal_radii_numseg_color_width.Item2;
                            //double radii         = center_normal_radii_numseg_color_width.Item3;
                            //int numLinesInCircle = center_normal_radii_numseg_color_width.Item4;
                            //double? alpha        = center_normal_radii_numseg_color_width.Item5.Item1;
                            //double? red          = center_normal_radii_numseg_color_width.Item5.Item2;
                            //double? green        = center_normal_radii_numseg_color_width.Item5.Item3;
                            //double? blue         = center_normal_radii_numseg_color_width.Item5.Item4;
                            //double? linewidth    = center_normal_radii_numseg_color_width.Item6;

                            Trans3 trans = Trans3.GetTransformNoScale( new double[3]{0,0,0}
                                                                     , new double[3]{0,0,1}
                                                                     , circle.center
                                                                     , circle.center + circle.normal.UnitVector()
                                                                     );
                            Vector[] pts = new Vector[circle.numLinesInCircle+1];
                            double di = (2*Math.PI)/circle.numLinesInCircle;
                            for(int i=0; i<circle.numLinesInCircle; i++)
                            {
                                Vector lpt = new double[3] { circle.radii*Math.Cos(di*i), circle.radii*Math.Sin(di*i), 0 };
                                pts[i] = trans.DoTransform(lpt);
                            }
                            pts[circle.numLinesInCircle] = pts[0];

                            file.Write("BEGIN, LINE_STRIP, ");
                            if(circle.linewidth != null) file.Write("LINEWIDTH, {0:0.000}, ", circle.linewidth);
                            if(circle.alpha     != null) file.Write("ALPHA, {0:0.000},", circle.alpha);
                            if(circle.color     != null) file.Write("COLOR, {0:0.000}, {1:0.000}, {2:0.000}, ", circle.color.Item1, circle.color.Item2, circle.color.Item3);
                            for(int i=0; i<pts.Length; i++)
                            {
                                if(i%10 == 0)
                                {
                                    file.WriteLine();
                                    file.Write("       ");
                                }
                                file.Write("VERTEX,   {0:0.000},  {1:0.000},   {2:0.000}, ", pts[i][0]+0.000001, pts[i][1]+0.000001, pts[i][2]+0.000001);
                            }
                            file.WriteLine();
                            file.WriteLine("       END, ");
                        }
                        file.WriteLine("]");
                        // file.WriteLine("cmd.load_cgo(obj,\"channel_path\")");
                        file.WriteLine("cmd.load_cgo(obj,\""+objname+"\")");
                        file.WriteLine();
                    }
                    file.Close();
                }
                public static void WriteSphere(string pmlpath
                                              , string objname
                                              , IList<Tuple<Vector, double>> lst_centercoord_radii
                                              , double? linewidth = null
                                              , double alpha=1.0, double red=1.0, double green=1.0, double blue=1.0
                                              , bool append=true
                                              )
                {
                    var lst_centercoord_radii_color = new List<Tuple<Vector, double, Tuple<double, double, double, double>>>();
                    foreach(var centercoord_radii in lst_centercoord_radii)
                        lst_centercoord_radii_color.Add
                            (new Tuple<Vector,double,Tuple<double,double,double,double>>(
                                centercoord_radii.Item1,
                                centercoord_radii.Item2,
                                new Tuple<double,double,double,double>(alpha, red, green, blue)
                            ));
                    WriteSphere(pmlpath, objname, lst_centercoord_radii_color, linewidth, append);
                }
                public static void WriteSphere(string pmlpath
                                              , string objname
                                              , IList<Vector> lst_centercoord
                                              , double radii
                                              , double? linewidth = null
                                              , double alpha=1.0, double red=1.0, double green=1.0, double blue=1.0
                                              , bool append=true
                                              )
                {
                    var lst_centercoord_radii_color = new List<Tuple<Vector, double, Tuple<double, double, double, double>>>();
                    foreach(var centercoord in lst_centercoord)
                        lst_centercoord_radii_color.Add
                            (new Tuple<Vector,double,Tuple<double,double,double,double>>(
                                centercoord,
                                radii,
                                new Tuple<double,double,double,double>(alpha, red, green, blue)
                            ));
                    WriteSphere(pmlpath, objname, lst_centercoord_radii_color, linewidth, append);
                }
                public static void WriteSphere(string pmlpath
                                              , string objname
                                              , IList<Tuple<Vector, double, Tuple<double,double,double,double>>> lst_centercoord_radii_color
                                              , double? linewidth = null
                                              , bool append=true
                                              )
                {
                    #region python source
                    //def writeChannelPathAndForcedAtoms(pypath, pathcoords, atomcoords, constfrc):
                    //    writeChannelPath(pypath, pathcoords);
                    //    file = open(pypath, 'a');
                    //    file.write('\n');
                    //    file.write('sphobj = ['+'\n');
                    //    for idx in range(constfrc['count']):
                    //        atm0 = constfrc['key0' ][idx]; atm0 = atomcoords[atm0, :];
                    //        atm1 = constfrc['key1' ][idx]; atm1 = atomcoords[atm1, :];
                    //        frc  = constfrc['value'][idx];
                    //        file.write('ALPHA,    0.500, COLOR,   1.000,   0.000,   0.000, ');
                    //        file.write('SPHERE,   %6.3f,  %6.3f,   %6.3f,  %6.3f,  ' % (atm0[0]*10, atm0[1]*10, atm0[2]*10, 0.3));
                    //        file.write('SPHERE,   %6.3f,  %6.3f,   %6.3f,  %6.3f,  ' % (atm1[0]*10, atm1[1]*10, atm1[2]*10, 0.3));
                    //        file.write('\n');
                    //        pass
                    //    file.write(']'+'\n');
                    //    file.write('cmd.load_cgo(sphobj,"forced_atoms")'+'\n');
                    //    file.close();
                    //    return
                    #endregion
                    StreamWriter file;
                    if(append) file = HFile.AppendText(pmlpath);
                    else file = HFile.CreateText(pmlpath);
                    {
                        file.WriteLine("from pymol.cgo import *");
                        file.WriteLine("from pymol import cmd");
                        file.WriteLine("obj = [");
                        file.Write("BEGIN, ");
                        if(linewidth != null)
                            file.WriteLine("LINEWIDTH, {0:0.000},", linewidth);
                        {
                            foreach(var centercoord_radii_color in lst_centercoord_radii_color)
                            {
                                Vector coord = centercoord_radii_color.Item1;
                                double radii = centercoord_radii_color.Item2;
                                double alpha = centercoord_radii_color.Item3.Item1;
                                double red   = centercoord_radii_color.Item3.Item2;
                                double green = centercoord_radii_color.Item3.Item3;
                                double blue  = centercoord_radii_color.Item3.Item4;

                                file.Write("ALPHA,    {0:0.000}, COLOR,   {1:0.000},   {2:0.000},   {3:0.000}, ", alpha, red, green, blue);
                                file.Write("SPHERE,   {0:0.000},  {1:0.000},   {2:0.000},  {3:0.000},  ", coord[0], coord[1], coord[2], radii);
                                file.WriteLine();
                            }
                        }
                        file.Write("END, ");
                        file.WriteLine("]");
                        // file.WriteLine("cmd.load_cgo(sphobj,\"forced_atoms\")");
                        file.WriteLine("cmd.load_cgo(obj,\""+objname+"\")");
                        file.WriteLine();
                    }
                    file.Close();
                }
                public static void WriteSphere( string pmlpath
                                              , string objname
                                              , Vector centercoord, double radii
                                              , double alpha=1.0, double red=1.0, double green=1.0, double blue=1.0
                                              , bool append=true
                                              )
                {
                    List<Tuple<Vector,double>> lst_centercoord_radii = new List<Tuple<Vector,double>>(1);
                    lst_centercoord_radii.Add(new Tuple<Vector, double>(centercoord, radii));
                    WriteSphere( pmlpath
                               , objname
                               , lst_centercoord_radii
                               , alpha: alpha, red: red, green: green, blue: blue, append: append);
                }
            }
        }
    }
}
