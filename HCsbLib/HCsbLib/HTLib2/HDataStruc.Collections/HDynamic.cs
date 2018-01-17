using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HTLib2
{
    public static partial class HStatic
    {
        public static void Add(this Dictionary<string, HDynamic> dict, string key, object value)
        {
            dict.Add(key, new HDynamic(value));
        }
    }
    [Serializable]
    public class HDynamic
    {
        object value;
        public HDynamic(object value)
        {
            this.value = value;
        }

        public void Get<TYPE>(out TYPE value) { value = (TYPE)this.value; }
        public TYPE Get<TYPE>() { return (TYPE)value; }
        public void Set(object value) { this.value = value; }

        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public object   Object  { get { return (object  )value; }       set { this.value = value; } }

        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public int      Int     { get { return (int     )value; }       set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public double   Double  { get { return (double  )value; }       set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public string   String  { get { return (string  )value; }       set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public char     Char    { get { return (char    )value; }       set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public bool     Bool    { get { return (bool    )value; }       set { this.value = value; } }

        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public Vector       Vector      { get { return (Vector      )value; } set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public MatrixByArr  MatrixByArr { get { return (MatrixByArr )value; } set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public HDictionary  Dictionary  { get { return (HDictionary )value; } set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public double[]     DoubleArr   { get { return (double[]    )value; } set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public double[,]    DoubleArr2D { get { return (double[,]   )value; } set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public double[][]   DoubleArrArr{ get { return (double[][]  )value; } set { this.value = value; } }

        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public List<int>       ListInt         { get { return (List<int>      )value; }       set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public List<double>    ListDouble      { get { return (List<double>   )value; }       set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public List<Vector>    ListVector      { get { return (List<Vector>   )value; }       set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public List<Vector[]>  ListVectorArray { get { return (List<Vector[]> )value; }       set { this.value = value; } }

        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public List<int[]>     ListArrayInt    { get { return (List<int[]>    )value; }       set { this.value = value; } }
        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public Vector[]        ArrayVector     { get { return (Vector[]       )value; }       set { this.value = value; } }

        [DebuggerBrowsableAttribute(DebuggerBrowsableState.Never)] public dynamic        Dynamic     { get { return (dynamic)value; }       set { this.value = value; } }
        public int    DynamicLength   { get { return Dynamic.Length;     } }
        public string DynamicToString { get { return Dynamic.ToString(); } }
        public object DynamicObjectAt(int idx) { return Dynamic[idx]; }

        public override string ToString()
        {
            { var val=value as IList<int   >; if(val != null) return    "int["+val.Count()+"] "+val.HToVectorT().ToString(20); }
            { var val=value as IList<double>; if(val != null) return "double["+val.Count()+"] "+val.HToVectorT().ToString(20); }
            { var val=value as IList<Vector>; if(val != null) return "Vector["+val.Count()+"] "+val.HToVectorT().ToString(20); }
            { var val=value as IList<Tuple<int,int        >>; if(val != null) return         "Tuple<int,int>["+val.Count()+"] "+val.HToVectorT().ToString(20); }
            { var val=value as IList<Tuple<int,int,int    >>; if(val != null) return     "Tuple<int,int,int>["+val.Count()+"] "+val.HToVectorT().ToString(20); }
            { var val=value as IList<Tuple<int,int,int,int>>; if(val != null) return "Tuple<int,int,int,int>["+val.Count()+"] "+val.HToVectorT().ToString(20); }
            { var val=value as double[,]                    ; if(val != null) return (new MatrixByArr(value as double[,])).ToString().Replace("Matrix","double"); }
            return value.ToString();
        }
    }
}
