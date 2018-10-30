using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Wolfram.NETLink;


// C:\Program Files\Wolfram Research\Mathematica\7.0\SystemFiles\Links\NETLink

namespace HTLib2
{
    public partial class Mathematica
    {
        public static string ToString(params object[] objs)
        {
            if(objs.Length == 1)
                return _ToString(null, objs[0]);
            return _ToString(null, (IEnumerable<object>)objs);
        }
        public static string ToString(string format, params object[] objs)
        {
            if(objs.Length == 1)
                return _ToString(format, objs[0]);
            return _ToString(format, (IEnumerable<object>)objs);
        }
        public static string ToString_Nearest<T, U>(IEnumerable<KeyValuePair<T,U>> objs, bool ignore_null)
        {
            StringBuilder text = new StringBuilder();
            Boolean addcomma = false;
            text.Append("{");
            foreach(KeyValuePair<T,U> obj in objs)
            {
                if(ignore_null && obj.Value == null)
                    continue;
                if(addcomma) text.Append(", ");
                else addcomma = true;
                text.Append(_ToString(null, obj, new string[]{"", "->", ""}));
            }
            text.Append("}");
            return text.ToString();
        }
        public static string ToString_Nearest<U>(IEnumerable<KeyValuePair<string, U>> objs, bool ignore_null)
        {
            StringBuilder text = new StringBuilder();
            Boolean addcomma = false;
            text.Append("{");
            foreach(KeyValuePair<string,U> obj in objs)
            {
                if(ignore_null && obj.Value == null)
                    continue;
                if(addcomma) text.Append(", ");
                else addcomma = true;
                text.Append(_ToString(null, obj, new string[] { "\"", "\"->", "" }));
            }
            text.Append("}");
            return text.ToString();
        }
        public static string ToString(object obj)
        {
            return _ToString(null, obj);
        }
        public static string ToString(string format, object obj)
        {
            return _ToString(format, obj);
        }
        public static string ToString<T>(T[] objs)
        {
            return _ToString(null, objs);
        }
        public static string ToString<T>(IEnumerable<T> objs)
        {
            return _ToString(null, objs);
        }
        public static string ToString<T,U>(IDictionary<T,U> objs)
        {
            return _ToString(null, (object)objs);
        }
        protected static string _ToString(string format, params object[] objs)
        {
            return _ToString(format, (IEnumerable<object>)objs);
        }
        protected static string _ToString<T>(string format, IEnumerable<T> objs)
        {
            StringBuilder text = new StringBuilder();
            Boolean addcomma = false;
            text.Append("{");
            if(objs != null)
                foreach(object obj in objs)
                {
                    if(addcomma) text.Append(", ");
                    else addcomma = true;
                    text.Append(_ToString(format, obj));
                }
            text.Append("}");
            return text.ToString();
        }
        protected static string _ToString<OBJECT>(string format, OBJECT[,] objs)
        {
            StringBuilder text = new StringBuilder();
            text.Append("{");
            for(int i=0; i<objs.GetLength(0); i++)
            {
                if(i != 0) text.Append(", ");
                text.Append("{");
                for(int j=0; j<objs.GetLength(1); j++)
                {
                    if(j != 0) text.Append(", ");
                    text.Append(_ToString(format, objs[i, j]));
                }
                text.Append("}");
            }
            text.Append("}");
            return text.ToString();
        }
        protected static string _ToString<OBJECT>(string format, OBJECT[,,] objs)
        {
            StringBuilder text = new StringBuilder();
            text.Append("{");
            for(int i=0; i<objs.GetLength(0); i++)
            {
                if(i != 0) text.Append(", ");
                text.Append("{");
                for(int j=0; j<objs.GetLength(1); j++)
                {
                    if(j != 0) text.Append(", ");
                    text.Append("{");
                    for(int k=0; k<objs.GetLength(2); k++)
                    {
                        if(k != 0) text.Append(", ");
                        text.Append(_ToString(format, objs[i, j, k]));
                    }
                    text.Append("}");
                }
                text.Append("}");
            }
            text.Append("}");
            return text.ToString();
        }
        protected static string _ToString(string format, System.Collections.IDictionary obj)
        {
            //  StringBuilder text = new StringBuilder();
            //  text.Append("{");
            //  int i = 0;
            //  foreach (System.Collections.DictionaryEntry key_value in obj)
            //  {
            //      if (i != 0) text.Append(", ");
            //      i++;
            //      text.Append(_ToString(format, key_value.Key, key_value.Value));
            //  }
            //  text.Append("}");
            //  return text.ToString();

            StringBuilder text = new StringBuilder();
            text.Append("<| ");
            int i = 0;
            foreach (System.Collections.DictionaryEntry key_value in obj)
            {
                if (i != 0) text.Append(", ");
                i++;
                text.Append(_ToString(format, key_value.Key));
                text.Append("->");
                text.Append(_ToString(format, key_value.Value));
            }
            text.Append(" |>");
            return text.ToString();

            throw new NotImplementedException();
        }
        protected static string _ToString<T, U>(string format, KeyValuePair<T, U> obj, string[] delims)
        {
            HDebug.Assert(delims.Length == 3);
            StringBuilder text = new StringBuilder();
            text.Append(delims[0]);
            text.Append(_ToString(format, obj.Key));
            text.Append(delims[1]);
            text.Append(_ToString(format, obj.Value));
            text.Append(delims[2]);
            return text.ToString();
        }
        protected static string _ToString(string format, System.Runtime.CompilerServices.ITuple obj)
        {
            int leng = obj.Length;
            object[] objs = new object[leng];
            for(int i=0; i<leng; i++)
                objs[i] = obj[i];

            return _ToString(format, objs);
        }
        protected static string _ToString(string format, object obj)
        {
        try{
            if(obj == null)
                return "";
            string name = obj.GetType().FullName;

            if(obj is System.Runtime.CompilerServices.ITuple    ) return _ToString(format, obj as System.Runtime.CompilerServices.ITuple    );
            if(obj is System.Collections.IDictionary            ) return _ToString(format, obj as System.Collections.IDictionary            );

          //if(name==typeof(Tuple<DoubleVector3,Tuple<double,DoubleVector3>[]>     ).FullName) return _ToString                           (format, (Tuple<DoubleVector3,Tuple<double,DoubleVector3>[]>     )obj);
          //if(name==typeof(Tuple<double,DoubleVector3>[]                          ).FullName) return _ToString                           (format, (Tuple<double,DoubleVector3>[]                          )obj);
          //if(name==typeof(Tuple<double,DoubleVector3>                            ).FullName) return _ToString                           (format, (Tuple<double,DoubleVector3>                            )obj);
          //if(name==typeof(Tuple<DoubleVector3,Tuple<DoubleVector3,DoubleVector3>>).FullName) return _ToString                           (format, (Tuple<DoubleVector3,Tuple<DoubleVector3,DoubleVector3>>)obj);
          //if(name==typeof(Tuple<DoubleVector3,DoubleVector3>                     ).FullName) return _ToString                           (format, (Tuple<DoubleVector3,DoubleVector3>                     )obj);
          //if(name==typeof(Tuple<double[],double[]>                             ).FullName) return _ToString                           (format, (Tuple<double[],double[]>                             )obj);
          //if(name==typeof(Tuple<double,double>                                 ).FullName) return _ToString                           (format, (Tuple<double,double>                                 )obj);
          //if(name==typeof(Tuple<double,int>                                    ).FullName) return _ToString                           (format, (Tuple<double,int>                                    )obj);
          //if(name==typeof(Tuple<int,double>                                    ).FullName) return _ToString                           (format, (Tuple<int,double>                                    )obj);
          //if(name==typeof(Tuple<double,Vector>                                 ).FullName) return _ToString                           (format, (Tuple<double,Vector>                                 )obj);
          //if(name==typeof(Tuple<Vector,Vector>                                 ).FullName) return _ToString                           (format, (Tuple<Vector,Vector>                                 )obj);
            if(name==typeof(List<Tuple<double,int>>                              ).FullName) return _ToString                           (format, (List<Tuple<double,int>>                              )obj);
            if(name==typeof(List<Tuple<int,double>>                              ).FullName) return _ToString                           (format, (List<Tuple<int,double>>                              )obj);
            if(name==typeof(List<Tuple<double,double>>                           ).FullName) return _ToString                           (format, (List<Tuple<double,double>>                           )obj);
            if(name==typeof(List<double>                                         ).FullName) return _ToString                           (format, (List<double>                                         )obj);
            if(name==typeof(List<Vector>                                         ).FullName) return _ToString                           (format, (List<Vector>                                         )obj);
          //if(name==typeof(List<DoubleVector3>                                  ).FullName) return _ToString                           (format, (List<DoubleVector3>                                  )obj);
            if(name==typeof(List<double[]>                                       ).FullName) return _ToString                           (format, (List<double[]>                                       )obj);
            if(name==typeof(List<double[,]>                                      ).FullName) return _ToString                           (format, (List<double[,]>                                      )obj);
            if(name==typeof(MatrixByArr[]                                        ).FullName) return _ToString<MatrixByArr>              (format, (MatrixByArr[]                                        )obj);
            if(name==typeof(Vector[]                                             ).FullName) return _ToString<Vector>                   (format, (Vector[]                                             )obj);
            if(name==typeof(string[]                                             ).FullName) return _ToString<string>                   (format, (string[]                                             )obj);
            if(name==typeof(string[,]                                            ).FullName) return _ToString                           (format, (string[,]                                            )obj);
            if(name==typeof(double[]                                             ).FullName) return _ToString                           (format, (double[]                                             )obj);
            if(name==typeof(int[]                                                ).FullName) return _ToString                           (format, (int[]                                                )obj);
            if(name==typeof(double[,]                                            ).FullName) return _ToString                           (format, (double[,]                                            )obj);
            if(name==typeof(double[,,]                                           ).FullName) return _ToString                           (format, (double[,,]                                           )obj);
          //if(name==typeof(DoubleVector3[]                                      ).FullName) return _ToString                           (format, (DoubleVector3[]                                      )obj);
            if(name==typeof(double[][]                                           ).FullName) return _ToString<double[]>                 (format, (double[][]                                           )obj);
            if(name==typeof(double[][,]                                          ).FullName) return _ToString                           (format, new List<double[,]>((double[][,]                      )obj));
            if(name==typeof(double[,][]                                          ).FullName) return _ToString                           (format, (double[,][]                                          )obj);
            if(name==typeof(List<int>                                            ).FullName) return _ToString<int>                      (format, (IEnumerable<int>                                     )obj);
            if(name==typeof(List<string>                                         ).FullName) return _ToString<string>                   (format, (IEnumerable<string>                                  )obj);
            if(name==typeof(List<string[]>                                       ).FullName) return _ToString<string[]>                 (format, (IEnumerable<string[]>                                )obj);
            if(name==typeof(List<List<int>>                                      ).FullName) return _ToString<List<int>>                (format, (IEnumerable<List<int>>                               )obj);
            if(name==typeof(List<List<double>>                                   ).FullName) return _ToString<List<double>>             (format, (IEnumerable<List<double>>                            )obj);
            if(name==typeof(List<List<string>>                                   ).FullName) return _ToString<List<string>>             (format, (IEnumerable<List<string>>                            )obj);
            if(name==typeof(List<List<Vector>>                                   ).FullName) return _ToString<List<Vector>>             (format, (IEnumerable<List<Vector>>                            )obj);
          //if(name==typeof(Dictionary<double,double>                            ).FullName) return _ToString<double,double>            (format, (Dictionary<double,double>                            )obj);

          //if(name==typeof(KeyValuePair<double,Vector[]>                        ).FullName) return _ToString<double,Vector[]>          (format, (KeyValuePair<double,Vector[]>                        )obj);
          //if(name==typeof(KeyValuePair<double,double[]>                        ).FullName) return _ToString<double,double[]>          (format, (KeyValuePair<double,double[]>                        )obj);
          //if(name==typeof(KeyValuePair<double,string>                          ).FullName) return _ToString<double,string>            (format, (KeyValuePair<double,string>                          )obj);
          //if(name==typeof(KeyValuePair<int, Tuple<Vector, Vector>>             ).FullName) return _ToString<int, Tuple<Vector,Vector>>(format, (KeyValuePair<int, Tuple<Vector, Vector>>             )obj);
          //if(name==typeof(Dictionary<double,double[]>                          ).FullName) return _ToString<double,double[]>          (format, (Dictionary<double,double[]>                          )obj);

          //if(name==typeof(Tuple<int   , int           >).FullName) { var val=(Tuple<int   , int           >)obj; return _ToString(format, val.Item1, val.Item2); }
          //if(name==typeof(Tuple<int   , double        >).FullName) { var val=(Tuple<int   , double        >)obj; return _ToString(format, val.Item1, val.Item2); }
          //if(name==typeof(Tuple<int   , double, double>).FullName) { var val=(Tuple<int   , double, double>)obj; return _ToString(format, val.Item1, val.Item2, val.Item3); }
          //if(name==typeof(Tuple<double, double        >).FullName) { var val=(Tuple<double, double        >)obj; return _ToString(format, val.Item1, val.Item2); }
          //if(name==typeof(Tuple<double, double, double>).FullName) { var val=(Tuple<double, double, double>)obj; return _ToString(format, val.Item1, val.Item2, val.Item3); }
          //if(name==typeof(Tuple<double, int, int, int >).FullName) { var val=(Tuple<double, int, int, int >)obj; return _ToString(format, val.Item1, val.Item2, val.Item3, val.Item4); }
          //if(name==typeof(Tuple<int   ,Vector         >).FullName) { var val=(Tuple<int   ,Vector         >)obj; return _ToString(format, val.Item1, val.Item2); }
          //if(name==typeof(Tuple<string,string         >).FullName) { var val=(Tuple<string,string         >)obj; return _ToString(format, val.Item1, val.Item2); }
          //if(name==typeof(Tuple<string,int            >).FullName) { var val=(Tuple<string,int            >)obj; return _ToString(format, val.Item1, val.Item2); }

            if(name==typeof(KeyValuePair<double, double>).FullName)
            {
                KeyValuePair<double, double> kvp = (KeyValuePair<double, double>)obj;
                return _ToString(format, kvp.Key, kvp.Value);
            }
            //if(name == typeof(MatrixDouble).FullName)
            //{
            //    MatrixDouble data = (MatrixDouble)obj;
            //    return _ToString(data.ToArray());
            //}
            if(name == typeof(MatrixByArr).FullName)
            {
                MatrixByArr data = (MatrixByArr)obj;
                return _ToString(format, data.ToArray());
            }
            if(name == typeof(Vector).FullName)
            {
                Vector data = (Vector)obj;
                return _ToString(format, data._data);
            }
            //if(name == typeof(DoubleVector3).FullName)
            //{
            //    DoubleVector3 data = (DoubleVector3)obj;
            //    return _ToString(data.v0, data.v1, data.v2);
            //}
            //if(name == typeof(DoubleMatrix3).FullName)
            //{
            //    DoubleMatrix3 data = (DoubleMatrix3)obj;
            //    return _ToString(data.ToArray());
            //}
            if(name == typeof(int).FullName)
            {
                if(format == null) return obj.ToString();
                else               return string.Format(format, (int)obj);
            }
            if(name == typeof(double).FullName)
            {
                string text;
                if(format == null) text = ((double)obj).ToString();
                else               text = string.Format(format, (double)obj);

                if(text.Contains("E") || text.Contains("e"))
                {
                    text = text.Replace("e", "*10^");
                    text = text.Replace("E", "*10^");
                }
                return text;
            }
            if(name == typeof(string).FullName)
            {
                string str = obj.ToString();
                str = str.Replace("\\", "\\\\");
                str = "\"" + str + "\"";
                return str;
            }
            if(name == typeof(object[]).FullName)
            {
                return _ToString(format, (object[])obj);
            }
            HDebug.Assert(false);
            HDebug.Break();
            return null;
        }
        catch(Exception)
        {
            HDebug.Break();
            return null;
        }
        }
        public static string ToMathematicaStringBase(object obj)
        {
            if(HDebug.True)
            {
                HDebug.Assert(false);
                return null;
            }
            HDebug.Exception("should not reach here");
            Type objtype = obj.GetType();
            //if(objtype.FullName == typeof(Tuple<DoubleVector3, Tuple<double, DoubleVector3>[]>).FullName)
            //{
            //    Tuple<DoubleVector3, Tuple<double, DoubleVector3>[]> data = (Tuple<DoubleVector3, Tuple<double, DoubleVector3>[]>)obj;
            //    return ToString(data);
            //}
            //if(objtype.FullName == typeof(Tuple<double, DoubleVector3>[]).FullName)
            //{
            //}
            //if(objtype.FullName == typeof(Tuple<double, DoubleVector3>).FullName)
            //{
            //    Tuple<double, DoubleVector3> data = (Tuple<double, DoubleVector3>)obj;
            //    StringBuilder text = new StringBuilder();
            //    //data.first;
            //}
            //if(objtype.FullName == typeof(DoubleVector3).FullName)
            //{
            //    StringBuilder text = new StringBuilder();
            //    DoubleVector3 data = (DoubleVector3)obj;
            //    text.Append("{");
            //    text.Append(ToString(data.v0).ToString());
            //    text.Append(",");
            //    text.Append(ToString(data.v1).ToString());
            //    text.Append(",");
            //    text.Append(ToString(data.v2).ToString());
            //    text.Append("}");
            //}
            if(objtype.FullName == typeof(double).FullName)
            {
                string text = ((double)obj).ToString();
                return text;
            }
            HDebug.Assert(false);
            return null;
        }
        //protected static string ToString(IEnumerable<DoubleVector3> points)
        //{
        //    return ToString(points, "0.00000000");
        //}
        //protected static string ToString(IEnumerable<DoubleVector3> points, string format)
        //{
        //    return ToString(points, Trans3.UnitTrans, 1, format);
        //}
        //protected static string ToString(IEnumerable<DoubleVector3> points, Trans3 trans, int skip, string format)
        //{
        //    int index = 0;
        //    StringBuilder text = new StringBuilder();
        //    text.Append("{");
        //    foreach(DoubleVector3 point in points)
        //    {
        //        if(index % skip == 0)
        //        {
        //            if(index != 0)
        //                text.Append(",");
        //            text.Append("{");
        //            text.Append(trans.DoTransform(point).ToString(format));
        //            text.Append("}");
        //        }
        //        index++;
        //    }
        //    text.Append("}");
        //    return text.ToString();
        //}
        //protected static string ToString(OFF.Mesh mesh, Trans3 trans,
        //                                        int skip/*=5*/,
        //                                        string format/*="0.00000000"*/)
        //{
        //    if(mesh == null)
        //        return "{}";
        //    DoubleVector3[] verts = mesh.VertsSparse(skip);
        //    if(verts.Length == 0)
        //        return "{}";
        //
        //    StringBuilder text = new StringBuilder();
        //    text.Append("{");
        //    text.Append("{"+trans.DoTransform(verts[0]).ToString(format)+"}");
        //    for(int i=1; i<verts.Length; i++)
        //        text.Append(",{"+trans.DoTransform(verts[i]).ToString(format)+"}");
        //    text.Append("}");
        //    return text.ToString();
        //}
        //protected static string ToString(Tuple<OFF.Vertex, double>[] map, Trans3 trans,
        //                                        int skip/*=5*/,
        //                                        string format/*="0.00000000"*/)
        //{
        //    if(map == null)
        //        return "{}";
        //    if(map.Length == 0)
        //        return "{}";
        //
        //    StringBuilder text = new StringBuilder();
        //    text.Append("{");
        //    bool comma = false;
        //    for(int i=0; i<map.Length; i+=skip)
        //    {
        //        // ","
        //        if(comma == false) comma=true;
        //        else text.Append(",");
        //        // "{x,y,z,dist}"
        //        text.Append("{"+trans.DoTransform(map[i].first.vec).ToString(format)+","+map[i].second.ToString(format)+"}");
        //    }
        //    text.Append("}");
        //    return text.ToString();
        //}
        //protected static string ToString(Tuple<DoubleVector3, double>[] map, Trans3 trans,
        //                                        int skip/*=5*/,
        //                                        string format/*="0.00000000"*/)
        //{
        //    if(map == null)
        //        return "{}";
        //    if(map.Length == 0)
        //        return "{}";
        //
        //    StringBuilder text = new StringBuilder();
        //    text.Append("{");
        //    bool comma = false;
        //    for(int i=0; i<map.Length; i+=skip)
        //    {
        //        // ","
        //        if(comma == false) comma=true;
        //        else text.Append(",");
        //        // "{x,y,z,dist}"
        //        text.Append("{"+trans.DoTransform(map[i].first).ToString(format)+","+map[i].second.ToString(format)+"}");
        //    }
        //    text.Append("}");
        //    return text.ToString();
        //}
        public static string ToString_ListSurfacePlot3D(string meshname)
        {
            StringBuilder text = new StringBuilder();
            text.Append("ListSurfacePlot3D[");
            text.Append(meshname);
            text.Append(",Mesh->None");
            text.Append(",MaxPlotPoints->30");
            text.Append(",PlotStyle -> Directive[Opacity[0.9]]");
            text.Append(", PerformanceGoal -> \"Speed\"");
            text.Append(",Axes -> False");
            text.Append(", Boxed -> False]");
            return text.ToString();
            //plots.Add("ListSurfacePlot3D[modelmesh,Mesh->None,MaxPlotPoints->50,PlotStyle -> Directive[Opacity[0.5]], PerformanceGoal -> \"Speed\",Axes -> False, Boxed -> False]");
        }
        //public static void ToString_Graphics3DPolygon(System.IO.StreamWriter writer, OFF.Mesh mesh, Tuple<OFF.Vertex, double>[] values, string name_prefix, Trans3 trans, string format/*=0.00000000*/)
        //{
        //    string name_verts  = name_prefix + "verts";
        //    string name_faces  = name_prefix + "faces";
        //    string name_values = name_prefix + "values";
        //
        //    Dictionary<OFF.Vertex, int> vert_index = new Dictionary<OFF.Vertex, int>();
        //    {
        //        writer.Write(name_verts + " = {");
        //        bool comma = false;
        //        for(int i=0; i<mesh.verts.Length; i++)
        //        {
        //            if(comma) writer.Write(",");
        //            else comma=true;
        //            vert_index.Add(mesh.verts[i], i);
        //            writer.Write("{"+mesh.verts[i].ToString(format)+"}");
        //        }
        //        writer.Write("};");
        //        writer.Write("\n");
        //    }
        //    {
        //        writer.Write(name_faces + " = {");
        //        bool comma = false;
        //        for(int i=0; i<mesh.verts.Length; i++)
        //        {
        //            if(comma) writer.Write(",");
        //            else comma=true;
        //            OFF.Face face = mesh.faces[i];
        //            Debug.Assert(face.verts.Length > 1);
        //            Debug.Assert(face.verts.Length == 3);
        //            writer.Write("{");
        //            writer.Write((vert_index[face.verts[0]]+1).ToString());
        //            for(int j=1; j<face.verts.Length; j++)
        //                writer.Write(","+(vert_index[face.verts[j]]+1).ToString());
        //            writer.Write("}");
        //        }
        //        writer.Write("};");
        //        writer.Write("\n");
        //    }
        //    {
        //        writer.Write(name_values + " = {");
        //        bool comma = false;
        //        for(int i=0; i<mesh.verts.Length; i++)
        //        {
        //            if(comma) writer.Write(",");
        //            else comma=true;
        //            Debug.Assert(values[i].first == mesh.verts[i]);
        //            Debug.Assert(vert_index.ContainsKey(values[i].first) && vert_index[values[i].first] == i);
        //            writer.Write(values[i].second.ToString(format));
        //        }
        //        writer.Write("};");
        //        writer.Write("\n");
        //    }
        //    writer.Write("\n");
        //    writer.Write("ColFunc[v_] := RGBColor[(1-Exp[-(v-1)^2*10]), Exp[-(v-0.5)^2*10]*0.9+0.1, (1-Exp[-(v-0.2)^2*10])];");
        //    writer.Write("\n");
        //    writer.Write("Graphics3D[\n");
        //    writer.Write("  EdgeForm[]\n");
        //    writer.Write("  ,Table[\n");
        //    writer.Write("     Polygon[\n");
        //    writer.Write("       {"+name_verts+"[["+name_faces+"[[i]][[1]]]],"+name_verts+"[["+name_faces+"[[i]][[2]]]],"+name_verts+"[["+name_faces+"[[i]][[3]]]]}\n");
        //    writer.Write("       ,VertexColors->{ColFunc["+name_values+"[["+name_faces+"[[i]][[1]]]]],ColFunc["+name_values+"[["+name_faces+"[[i]][[2]]]]],ColFunc["+name_values+"[["+name_faces+"[[i]][[3]]]]]}\n");
        //    writer.Write("     ]\n");
        //    writer.Write("     ,{i,1,Length["+name_verts+"],1}\n");
        //    writer.Write("  ]\n");
        //    writer.Write("]\n");
        //}



        protected static string _ToString_<T, U>(string format, Tuple<T, U> obj)
        {
            return _ToString(format, obj.Item1, obj.Item2);
        }
        protected static string _ToString_<TYPE1, TYPE2>(string format, IEnumerable<KeyValuePair<TYPE1, TYPE2>> obj)
        {
            StringBuilder text = new StringBuilder();
            text.Append("{");
            int i=0;
            foreach(var key_value in obj)
            {
                if(i != 0) text.Append(", ");
                i++;
                text.Append(_ToString(format, key_value));
            }
            text.Append("}");
            return text.ToString();
        }
        protected static string _ToString_<T, U>(string format, KeyValuePair<T, U> obj)
        {
            return _ToString<T, U>(format, obj, new string[]{"{", ",", "}"});
        }
    }
}
