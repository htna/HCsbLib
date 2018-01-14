using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public static partial class HStaticBioinfo
    {
        public static string[] ListLine<T>(this IList<T> elements)
            where T : Pdb.Element
        {
            int count = elements.Count;
            string[] lines = new string[count];
            for(int i=0; i<count; i++)
                lines[i] = elements[i].line;
            return lines;
        }
    }
    public partial class Pdb
    {
        [Serializable]
        public partial class Element : ISerializable
        {
            public readonly string line;
            protected string  String (int begin, int end) { string substr = line.Substring(begin-1, end-begin+1); return substr; }
            protected char    Char   (int index         ) { return line[index-1]; }
            protected int?    Integer(int begin, int end) { string substr = line.Substring(begin-1, end-begin+1); int    val; if(int   .TryParse(substr,out val)) return val; return null; }
            protected double? Double (int begin, int end) { string substr = line.Substring(begin-1, end-begin+1); double val; if(double.TryParse(substr,out val)) return val; return null; }

            protected int    Integer(int begin, int end, int defvalue)
            {
                string substr = line.Substring(begin-1, end-begin+1);
                int value;
                if(int.TryParse(substr, out value) == false)
                    value = defvalue;
                return value;
            }

            protected string  String (int[] idxs) { HDebug.Assert(idxs.Length==2);                     return String (idxs[0], idxs[1]); }
            protected char    Char   (int[] idxs) { HDebug.Assert(idxs.Length==2, idxs[0] == idxs[1]); return Char   (idxs[0]         ); }
            protected int?    Integer(int[] idxs) { HDebug.Assert(idxs.Length==2);                     return Integer(idxs[0], idxs[1]); }
            protected double? Double (int[] idxs) { HDebug.Assert(idxs.Length==2);                     return Double (idxs[0], idxs[1]); }

            protected static string UpdateLineString (string line, string value, int[] idxs) { HDebug.Assert(idxs.Length==2);                     return UpdateLineString (line, value, idxs[0], idxs[1]); }
            protected static string UpdateLineChar   (string line, char   value, int[] idxs) { HDebug.Assert(idxs.Length==2, idxs[0] == idxs[1]); return UpdateLineChar   (line, value, idxs[0]         ); }
            protected static string UpdateLineInteger(string line, int    value, int[] idxs) { HDebug.Assert(idxs.Length==2);                     return UpdateLineInteger(line, value, idxs[0], idxs[1]); }
            protected static string UpdateLineDouble (string line, double value, int[] idxs) { HDebug.Assert(idxs.Length==2);                     return UpdateLineDouble (line, value, idxs[0], idxs[1]); }

            protected static string UpdateLineString (string line, string value, int begin, int end)
            {
                int leng = end-begin+1;
                if(value.Length > leng) throw new ArgumentException("line.length > (end-begin+1)");
                char[] chs = line.ToCharArray();
                string lvalue = "                                "+value;
                lvalue = lvalue.HSubEndStringCount(leng);
                for(int i=0; i<leng; i++)
                    chs[begin+i-1] = lvalue[i];
                return new String(chs);
            }
            protected static string UpdateLineChar   (string line, char   value, int index         ) { return UpdateLineString(line, ""+value              , index, index); }
            protected static string UpdateLineInteger(string line, int    value, int begin, int end) { return UpdateLineString(line, value.ToString()      , begin, end  ); }
            protected static string UpdateLineDouble (string line, double value, int begin, int end) { return UpdateLineString(line, value.ToString("0.00"), begin, end  ); }

            public Element(string line)
            {
                this.line = line;
            }
            public override string ToString() { return line; }
            public virtual Element Clone() { HDebug.Assert(false); return new Element(line); }

            public string record
            {
                get
                {
                    int[] idxs_record = new int[] { 1, 6 };
                    return String(idxs_record);
                }
            }

		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Element(SerializationInfo info, StreamingContext ctxt)
		    {
                this.line = (string)info.GetValue("line", typeof(string));
		    }
            public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("line", line);
            }
//            public abstract string RecordName { get; }
        }
        //public class Elements<ELEM>
        //    where ELEM : Element
        //{
        //    abstract Elements<ELEM>
        //    public Elements<ELEM> FromLines(string[] lines)
        //    {
        //        Elements<ELEM> elements = new 
        //    }
        //}
        static int[] Collect(Element[] elements, string RecordName)
        {
            List<int> collection = new List<int>();
            for(int idx=0; idx<elements.Length; idx++)
            {
                string line = elements[idx].line;
                if(line.StartsWith(RecordName))
                    collection.Add(idx);
            }
            return collection.ToArray();
        }
    }
}
