using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HTLib2
{
	public partial class HSerialize
	{
        public static void SerializeText(string filename, Func<int, string> i_tostring, int length)
        {
            StreamWriter file = new StreamWriter(filename);
            for(int i=0; i<length; i++)
            {
                if(i == 0)
                    file.Write("{ ");
                else
                    file.Write(", ");
                file.Write(i_tostring(i));
            }
            file.Write(" }");
            file.Close();
        }
        public static void SerializeText<T>(string filename, T[] values, Func<T, string> tostring) { Func<int, string> i_tostring = delegate(int i) { return tostring(values[i]);  }; SerializeText(filename, i_tostring, values.Length); }
        public static void SerializeText   (string filename, dynamic[] values)                     { Func<int, string> i_tostring = delegate(int i) { return values[i].ToString(); }; SerializeText(filename, i_tostring, values.Length); }
        public static void SerializeText   (string filename, string[] values)                      { Func<int, string> i_tostring = delegate(int i) { return values[i].ToString(); }; SerializeText(filename, i_tostring, values.Length); }
        public static void SerializeText   (string filename, double[] values)                      { Func<int, string> i_tostring = delegate(int i) { return values[i].ToString(); }; SerializeText(filename, i_tostring, values.Length); }
        public static void SerializeText   (string filename, float [] values)                      { Func<int, string> i_tostring = delegate(int i) { return values[i].ToString(); }; SerializeText(filename, i_tostring, values.Length); }
        public static void SerializeText   (string filename, int   [] values)                      { Func<int, string> i_tostring = delegate(int i) { return values[i].ToString(); }; SerializeText(filename, i_tostring, values.Length); }
        public static void SerializeText   (string filename, bool  [] values)                      { Func<int, string> i_tostring = delegate(int i) { return values[i].ToString(); }; SerializeText(filename, i_tostring, values.Length); }
        public static void SerializeText   (string filename, char  [] values)                      { Func<int, string> i_tostring = delegate(int i) { return values[i].ToString(); }; SerializeText(filename, i_tostring, values.Length); }
        public static void DeserializeText<T>(string filename, Func<string,T> parse, out T[] values)
        {
            string text = HFile.ReadAllText(filename).Trim();
            HDebug.Assert(text.StartsWith("{")); text = text.Substring(1);
            HDebug.Assert(text.EndsWith("}"));   text = text.Substring(0, text.Length-1);
            text = text.Trim();
            if(text.Length == 0)
            {
                values = new T[0];
                return;
            }
            string[] token = text.Split(',');
            values = new T[token.Length];
            for(int i=0; i<token.Length; i++)
            {
                T value = parse(token[i]);
                values[i] = value;
            }
        }
        public static string[] DeserializeTextString(string filename) { string[] values; DeserializeText(filename, out values); return values; }
        public static double[] DeserializeTextDouble(string filename) { double[] values; DeserializeText(filename, out values); return values; }
        public static float [] DeserializeTextFloat (string filename) { float [] values; DeserializeText(filename, out values); return values; }
        public static int   [] DeserializeTextInt   (string filename) { int   [] values; DeserializeText(filename, out values); return values; }
        public static bool  [] DeserializeTextBool  (string filename) { bool  [] values; DeserializeText(filename, out values); return values; }
        public static char  [] DeserializeTextChar  (string filename) { char  [] values; DeserializeText(filename, out values); return values; }
        public static void DeserializeText(string filename, out string[] values) { Func<string, string> parse = delegate(string value) { return              value ; }; DeserializeText(filename, parse, out values); }
        public static void DeserializeText(string filename, out double[] values) { Func<string, double> parse = delegate(string value) { return double.Parse(value); }; DeserializeText(filename, parse, out values); }
        public static void DeserializeText(string filename, out float [] values) { Func<string, float > parse = delegate(string value) { return float .Parse(value); }; DeserializeText(filename, parse, out values); }
        public static void DeserializeText(string filename, out int   [] values) { Func<string, int   > parse = delegate(string value) { return int   .Parse(value); }; DeserializeText(filename, parse, out values); }
        public static void DeserializeText(string filename, out bool  [] values) { Func<string, bool  > parse = delegate(string value) { return bool  .Parse(value); }; DeserializeText(filename, parse, out values); }
        public static void DeserializeText(string filename, out char  [] values) { Func<string, char  > parse = delegate(string value) { return char  .Parse(value); }; DeserializeText(filename, parse, out values); }
    }
}
