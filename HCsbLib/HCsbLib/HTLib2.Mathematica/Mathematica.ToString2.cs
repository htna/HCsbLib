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
        public static StringBuilder ToStringBuilder2(object obj)
        {
            StringBuilder text = new StringBuilder();
            _ToString2(text, obj, null, null, null);
            return text;
        }
        public static StringBuilder ToStringBuilder2
            ( object obj
            , string formatInt      // null (default) or "{0,3}"
            , string formatDouble   // null (default) or "{0,8:0.0000}"
            , string formatString   // null (default) or "{0,10}"
            )
        {
            StringBuilder text = new StringBuilder();
            _ToString2(text, obj, formatInt, formatDouble, formatString);
            return text;
        }

        public static string ToString2(object obj)
        {
            StringBuilder text = new StringBuilder();
            _ToString2(text, obj, null, null, null);
            return text.ToString();
        }
        public static void ToString2
            ( object obj
            , StringBuilder text
            , string formatInt      // null (default) or "{0,3}"
            , string formatDouble   // null (default) or "{0,8:0.0000}"
            , string formatString   // null (default) or "{0,10}"
            )
        {
            _ToString2(text, obj, formatInt, formatDouble, formatString);
        }
        public static string ToString2
            ( string formatInt      // null (default) or "{0,3}"
            , string formatDouble   // null (default) or "{0,8:0.0000}"
            , string formatString   // null (default) or "{0,10}"
            , object obj
            )
        {
            StringBuilder text = new StringBuilder();
            _ToString2(text, obj, formatInt, formatDouble, formatString);
            return text.ToString();
        }
        public static string ToString2
            ( string formatInt      // null (default) or "{0,3}"
            , string formatDouble   // null (default) or "{0,8:0.0000}"
            , string formatString   // null (default) or "{0,10}"
            , object obj, params object[] objs
            )
        {
            StringBuilder text = new StringBuilder();
            List<object> list = new List<object>(objs.Length+1);
            list.Add(obj);
            list.AddRange(objs);
            _ToString2(text, list, formatInt, formatDouble, formatString);
            return text.ToString();
        }
        //public static string ToString2<T>(T[] objs)
        //{
        //    return _ToString2(null, objs);
        //}
        //public static string ToString2<T>(IEnumerable<T> objs)
        //{
        //    return _ToString2(null, objs);
        //}
        //public static string ToString2<T,U>(IDictionary<T,U> objs)
        //{
        //    return _ToString2(null, (object)objs);
        //}
        //protected static string _ToString2(string format, params object[] objs)
        //{
        //    return _ToString2(format, (IEnumerable<object>)objs);
        //}
        //protected static string _ToString2<OBJECT>(string format, OBJECT[,] objs)
        //{
        //    StringBuilder text = new StringBuilder();
        //    text.Append("{");
        //    for(int i=0; i<objs.GetLength(0); i++)
        //    {
        //        if(i != 0) text.Append(", ");
        //        text.Append("{");
        //        for(int j=0; j<objs.GetLength(1); j++)
        //        {
        //            if(j != 0) text.Append(", ");
        //            text.Append(_ToString2(format, objs[i, j]));
        //        }
        //        text.Append("}");
        //    }
        //    text.Append("}");
        //    return text.ToString();
        //}
        //protected static string _ToString2<OBJECT>(string format, OBJECT[,,] objs)
        //{
        //    StringBuilder text = new StringBuilder();
        //    text.Append("{");
        //    for(int i=0; i<objs.GetLength(0); i++)
        //    {
        //        if(i != 0) text.Append(", ");
        //        text.Append("{");
        //        for(int j=0; j<objs.GetLength(1); j++)
        //        {
        //            if(j != 0) text.Append(", ");
        //            text.Append("{");
        //            for(int k=0; k<objs.GetLength(2); k++)
        //            {
        //                if(k != 0) text.Append(", ");
        //                text.Append(_ToString2(format, objs[i, j, k]));
        //            }
        //            text.Append("}");
        //        }
        //        text.Append("}");
        //    }
        //    text.Append("}");
        //    return text.ToString();
        //}
        protected static void _ToString2(StringBuilder text, System.Array arr, int[] idxs, int dim, int rank, string formatInt, string formatDouble, string formatString)
        {
            if(dim == rank)
            {
                object obj = arr.GetValue(idxs);
                _ToString2(text, obj, formatInt, formatDouble, formatString);
                return;
            }

            text.Append("{");
            int leng = arr.GetLength(dim);
            string delim = (dim == 0) ? ", " : ",";
            for(int i=0; i<leng; i++)
            {
                if(i != 0)
                    text.Append(delim);
                idxs[dim] = i;
                _ToString2(text, arr, idxs, dim+1, rank, formatInt, formatDouble, formatString);
            }
            text.Append("}");
        }
        protected static void _ToString2(StringBuilder text, System.Array arr, string formatInt, string formatDouble, string formatString)
        {
            if(arr == null)
            {
                text.Append("{}");
                return;
            }

            int rank  = arr.Rank;
            int[] idxs = new int[rank];
            _ToString2(text, arr, idxs, 0, rank, formatInt, formatDouble, formatString);
        }
        protected static void _ToString2(StringBuilder text, System.Collections.IEnumerable objs, string formatInt, string formatDouble, string formatString)
        {
            text.Append("{");
            if(objs != null)
            {
                bool addcomma = false;
                foreach(object obj in objs)
                {
                    if(addcomma) text.Append(", ");
                    else         addcomma = true;
                    _ToString2(text, obj, formatInt, formatDouble, formatString);
                }
            }
            text.Append("}");
        }
        protected static void _ToString2(StringBuilder text, System.Collections.IDictionary obj, string formatInt, string formatDouble, string formatString)
        {
            text.Append("<| ");
            bool addcomma = false;
            foreach (System.Collections.DictionaryEntry key_value in obj)
            {
                if(addcomma) text.Append(", ");
                else         addcomma = true;
                _ToString2(text, key_value.Key  , formatInt, formatDouble, formatString);
                text.Append("->");
                _ToString2(text, key_value.Value, formatInt, formatDouble, formatString);
            }
            text.Append(" |>");
        }
        //protected static string _ToString2<T, U>(string format, KeyValuePair<T, U> obj, string[] delims)
        //{
        //    HDebug.Assert(delims.Length == 3);
        //    StringBuilder text = new StringBuilder();
        //    text.Append(delims[0]);
        //    text.Append(_ToString2(format, obj.Key));
        //    text.Append(delims[1]);
        //    text.Append(_ToString2(format, obj.Value));
        //    text.Append(delims[2]);
        //    return text.ToString();
        //}
        protected static void _ToString2(StringBuilder text, System.Runtime.CompilerServices.ITuple obj, string formatInt, string formatDouble, string formatString)
        {
            int leng = obj.Length;
            object[] objs = new object[leng];
            for(int i=0; i<leng; i++)
                objs[i] = obj[i];

            _ToString2(text, objs, formatInt, formatDouble, formatString);
        }
        protected static void _ToString2<T>(StringBuilder text, IMatrix<T> obj, string formatInt, string formatDouble, string formatString)
        //protected static void _ToString2(StringBuilder text, string format, IMatrix<double> obj)
        {
            text.Append("{");
            for(int c=0; c<obj.ColSize; c++)
            {
                if(c != 0) text.Append(", ");

                text.Append("{");
                for(int r=0; r<obj.RowSize; r++)
                {
                    if(r != 0) text.Append(",");
                    _ToString2(text, obj[c,r], formatInt, formatDouble, formatString);
                }
                text.Append("}");
            }
            text.Append("}");
        }
        protected static void _ToString2<T>(StringBuilder text, IVector<T> obj, string formatInt, string formatDouble, string formatString)
        //protected static void _ToString2(StringBuilder text, string format, IMatrix<double> obj)
        {
            text.Append("{");
            for(int i=0; i<obj.Size; i++)
            {
                if(i != 0) text.Append(",");
                _ToString2(text, obj[i], formatInt, formatDouble, formatString);
            }
            text.Append("}");
        }
        protected static void _ToString2(StringBuilder text, object obj, string formatInt, string formatDouble, string formatString)
        {
            try
            {
                if(obj == null)
                    return;
                string name = obj.GetType().FullName;

                if(name == typeof(Vector).FullName)
                {
                    Vector data = (Vector)obj;
                    _ToString2(text, data._data, formatInt, formatDouble, formatString);
                    return;
                }
                if(name == typeof(SVector3).FullName)
                {
                    SVector3 data = (SVector3)obj;
                    _ToString2(text, data, formatInt, formatDouble, formatString);
                    return;
                }
                if(name == typeof(int).FullName)
                {
                    if(formatInt == null) { text.Append(obj.ToString()                    ); return; }
                    else                  { text.Append(string.Format(formatInt, (int)obj)); return; }
                }
                if(name == typeof(double).FullName)
                {
                    string str;
                    if(formatDouble == null) str = ((double)obj).ToString();
                    else                     str = string.Format(formatDouble, (double)obj);

                    if(str.Contains("E") || str.Contains("e"))
                    {
                        str = str.Replace("e", "*10^");
                        str = str.Replace("E", "*10^");
                    }

                    text.Append(str);
                    return;
                }
                if(name == typeof(string).FullName)
                {
                    string str = obj.ToString();
                    if(str.StartsWith("(* ") && str.EndsWith(" *)"))
                    {
                        // add comment as it is
                        text.Append(str);
                    }
                    else
                    {
                        if(formatString == null) str = obj.ToString();
                        else                     str = string.Format(formatString, obj);
                        str = str.Replace("\\", "\\\\");
                        str = "\"" + str + "\"";

                        text.Append(str);
                    }
                    return;
                }
                if(name == typeof(char).FullName)
                {
                    string str = "\'" + (char)obj + "\'";

                    text.Append(str);
                    return;
                }

                if(obj is System.Runtime.CompilerServices.ITuple    ) { _ToString2(text, obj as System.Runtime.CompilerServices.ITuple    , formatInt, formatDouble, formatString); return; }
                if(obj is System.Collections.IDictionary            ) { _ToString2(text, obj as System.Collections.IDictionary            , formatInt, formatDouble, formatString); return; }
                if(obj is IMatrix<double>                           ) { _ToString2(text, obj as IMatrix<double>                           , formatInt, formatDouble, formatString); return; }
                if(obj is System.Array                              ) { _ToString2(text, obj as System.Array                              , formatInt, formatDouble, formatString); return; }
                if(obj is System.Collections.IList                  )
                {
                    //HDebug.ToDo();
                    //var objs = obj as System.Collections.IList;
                    //objs.getleng
                }
                if(obj is System.Collections.IEnumerable            ) { _ToString2(text, obj as System.Collections.IEnumerable            , formatInt, formatDouble, formatString); return; }

                //if(name==typeof(List<Tuple<double,int>>                              ).FullName) return _ToString2                           (format, (List<Tuple<double,int>>                              )obj);
                //if(name==typeof(List<Tuple<int,double>>                              ).FullName) return _ToString2                           (format, (List<Tuple<int,double>>                              )obj);
                //if(name==typeof(List<Tuple<double,double>>                           ).FullName) return _ToString2                           (format, (List<Tuple<double,double>>                           )obj);
                //if(name==typeof(List<double>                                         ).FullName) return _ToString2                           (format, (List<double>                                         )obj);
                //if(name==typeof(List<Vector>                                         ).FullName) return _ToString2                           (format, (List<Vector>                                         )obj);
              //if(name==typeof(List<DoubleVector3>                                  ).FullName) return _ToString2                           (format, (List<DoubleVector3>                                  )obj);
                //if(name==typeof(List<double[]>                                       ).FullName) return _ToString2                           (format, (List<double[]>                                       )obj);
                //if(name==typeof(List<double[,]>                                      ).FullName) return _ToString2                           (format, (List<double[,]>                                      )obj);
                //if(name==typeof(MatrixByArr[]                                        ).FullName) return _ToString2<MatrixByArr>              (format, (MatrixByArr[]                                        )obj);
                //if(name==typeof(Vector[]                                             ).FullName) return _ToString2<Vector>                   (format, (Vector[]                                             )obj);
                //if(name==typeof(string[]                                             ).FullName) return _ToString2<string>                   (format, (string[]                                             )obj);
                //if(name==typeof(string[,]                                            ).FullName) return _ToString2                           (format, (string[,]                                            )obj);
                //if(name==typeof(double[]                                             ).FullName) return _ToString2                           (format, (double[]                                             )obj);
                //if(name==typeof(int[]                                                ).FullName) return _ToString2                           (format, (int[]                                                )obj);
                //if(name==typeof(double[,]                                            ).FullName) return _ToString2                           (format, (double[,]                                            )obj);
                //if(name==typeof(double[,,]                                           ).FullName) return _ToString2                           (format, (double[,,]                                           )obj);
              //if(name==typeof(DoubleVector3[]                                      ).FullName) return _ToString2                           (format, (DoubleVector3[]                                      )obj);
                //if(name==typeof(double[][]                                           ).FullName) return _ToString2<double[]>                 (format, (double[][]                                           )obj);
                //if(name==typeof(double[][,]                                          ).FullName) return _ToString2                           (format, new List<double[,]>((double[][,]                      )obj));
                //if(name==typeof(double[,][]                                          ).FullName) return _ToString2                           (format, (double[,][]                                          )obj);
                //if(name==typeof(List<int>                                            ).FullName) return _ToString2<int>                      (format, (IEnumerable<int>                                     )obj);
                //if(name==typeof(List<string>                                         ).FullName) return _ToString2<string>                   (format, (IEnumerable<string>                                  )obj);
                //if(name==typeof(List<string[]>                                       ).FullName) return _ToString2<string[]>                 (format, (IEnumerable<string[]>                                )obj);
                //if(name==typeof(List<List<int>>                                      ).FullName) return _ToString2<List<int>>                (format, (IEnumerable<List<int>>                               )obj);
                //if(name==typeof(List<List<double>>                                   ).FullName) return _ToString2<List<double>>             (format, (IEnumerable<List<double>>                            )obj);
                //if(name==typeof(List<List<string>>                                   ).FullName) return _ToString2<List<string>>             (format, (IEnumerable<List<string>>                            )obj);
                //if(name==typeof(List<List<Vector>>                                   ).FullName) return _ToString2<List<Vector>>             (format, (IEnumerable<List<Vector>>                            )obj);
              //if(name==typeof(Dictionary<double,double>                            ).FullName) return _ToString2<double,double>            (format, (Dictionary<double,double>                            )obj);

              //if(name==typeof(KeyValuePair<double,Vector[]>                        ).FullName) return _ToString2<double,Vector[]>          (format, (KeyValuePair<double,Vector[]>                        )obj);
              //if(name==typeof(KeyValuePair<double,double[]>                        ).FullName) return _ToString2<double,double[]>          (format, (KeyValuePair<double,double[]>                        )obj);
              //if(name==typeof(KeyValuePair<double,string>                          ).FullName) return _ToString2<double,string>            (format, (KeyValuePair<double,string>                          )obj);
              //if(name==typeof(KeyValuePair<int, Tuple<Vector, Vector>>             ).FullName) return _ToString2<int, Tuple<Vector,Vector>>(format, (KeyValuePair<int, Tuple<Vector, Vector>>             )obj);
              //if(name==typeof(Dictionary<double,double[]>                          ).FullName) return _ToString2<double,double[]>          (format, (Dictionary<double,double[]>                          )obj);

              //if(name==typeof(Tuple<int   , int           >).FullName) { var val=(Tuple<int   , int           >)obj; return _ToString2(format, val.Item1, val.Item2); }
              //if(name==typeof(Tuple<int   , double        >).FullName) { var val=(Tuple<int   , double        >)obj; return _ToString2(format, val.Item1, val.Item2); }
              //if(name==typeof(Tuple<int   , double, double>).FullName) { var val=(Tuple<int   , double, double>)obj; return _ToString2(format, val.Item1, val.Item2, val.Item3); }
              //if(name==typeof(Tuple<double, double        >).FullName) { var val=(Tuple<double, double        >)obj; return _ToString2(format, val.Item1, val.Item2); }
              //if(name==typeof(Tuple<double, double, double>).FullName) { var val=(Tuple<double, double, double>)obj; return _ToString2(format, val.Item1, val.Item2, val.Item3); }
              //if(name==typeof(Tuple<double, int, int, int >).FullName) { var val=(Tuple<double, int, int, int >)obj; return _ToString2(format, val.Item1, val.Item2, val.Item3, val.Item4); }
              //if(name==typeof(Tuple<int   ,Vector         >).FullName) { var val=(Tuple<int   ,Vector         >)obj; return _ToString2(format, val.Item1, val.Item2); }
              //if(name==typeof(Tuple<string,string         >).FullName) { var val=(Tuple<string,string         >)obj; return _ToString2(format, val.Item1, val.Item2); }
              //if(name==typeof(Tuple<string,int            >).FullName) { var val=(Tuple<string,int            >)obj; return _ToString2(format, val.Item1, val.Item2); }

                //if(name==typeof(KeyValuePair<double, double>).FullName)
                //{
                //    KeyValuePair<double, double> kvp = (KeyValuePair<double, double>)obj;
                //    return _ToString2(format, kvp.Key, kvp.Value);
                //}
                //if(name == typeof(MatrixDouble).FullName)
                //{
                //    MatrixDouble data = (MatrixDouble)obj;
                //    return _ToString2(data.ToArray());
                //}
                //if(name == typeof(MatrixByArr).FullName)
                //{
                //    MatrixByArr data = (MatrixByArr)obj;
                //    return _ToString2(format, data.ToArray());
                //}
                //if(name == typeof(DoubleVector3).FullName)
                //{
                //    DoubleVector3 data = (DoubleVector3)obj;
                //    return _ToString2(data.v0, data.v1, data.v2);
                //}
                //if(name == typeof(DoubleMatrix3).FullName)
                //{
                //    DoubleMatrix3 data = (DoubleMatrix3)obj;
                //    return _ToString2(data.ToArray());
                //}
                if(name == typeof(object[]).FullName)
                {
                    _ToString2(text, (object[])obj, formatInt, formatDouble, formatString);
                    return;
                }
                HDebug.Assert(false);
                HDebug.Break();
            }
            catch(Exception)
            {
                HDebug.Break();
            }
        }
    }
}
