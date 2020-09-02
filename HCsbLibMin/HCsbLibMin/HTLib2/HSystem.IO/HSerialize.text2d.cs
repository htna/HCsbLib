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
        public static void SerializeText2D<T>(string filename, Func<T,string> tostring, T[,] values)
        {
            string text = values.HToStringSeparated(tostring
                                                    , "{ ", "\n, ", "\n}"
                                                    , "{", ", ", "}"
                                                    );
            HFile.WriteAllText(filename, text);
        }
        public static void SerializeText2D(string filename, string[,] values)
        {
            Func<string, string> tostring = delegate(string value)
            {
                return value.ToString();
            };
            SerializeText2D(filename, tostring, values);
        }
        public static void SerializeText2D(string filename, double[,] values, string format=null)
        {
            Func<double, string> tostring = delegate(double value)
            {
                if(format == null)
                    return value.ToString();
                return value.ToString(format);
            };
            SerializeText2D(filename, tostring, values);
        }
        public static void SerializeText2D(string filename, int[,] values)
        {
            Func<int, string> tostring = delegate(int value)
            {
                return value.ToString();
            };
            SerializeText2D(filename, tostring, values);
        }
        public static void DeSerializeText2D<T>(string filename, Func<string, T> parse, out T[,] values)
        {
            throw new NotImplementedException();
            //string text = HFile.ReadAllText(filename).Trim();
            //HDebug.Assert(text.StartsWith("{")); text = text.Substring(1);
            //HDebug.Assert(text.EndsWith("}"));   text = text.Substring(0, text.Length-1);
            //text = text.Trim();
            //if(text.Length == 0)
            //{
            //    values = new T[0];
            //    return;
            //}
            //string[,] token = text.Split(',');
            //values = new T[token.Length];
            //for(int i=0; i<token.Length; i++)
            //{
            //    T value = parse(token[i]);
            //    values[i] = value;
            //}
        }
        public static string[,] DeSerializeText2DString(string filename)
        {
            string[,] values;
            DeSerializeText2D(filename, out values);
            return values;
        }
        public static void DeSerializeText2D(string filename, out string[,] values)
        {
            Func<string, string> parse = delegate(string value)
            {
                return value;
            };
            DeSerializeText2D(filename, parse, out values);
        }
        public static double[,] DeSerializeText2DDouble(string filename)
        {
            double[,] values;
            DeSerializeText2D(filename, out values);
            return values;
        }
        public static void DeSerializeText2D(string filename, out double[,] values)
        {
            Func<string, double> parse = delegate(string value)
            {
                return double.Parse(value);
            };
            DeSerializeText2D(filename, parse, out values);
        }
        public static int[,] DeSerializeText2DInt(string filename)
        {
            int[,] values;
            DeSerializeText2D(filename, out values);
            return values;
        }
        public static void DeSerializeText2D(string filename, out int[,] values)
        {
            Func<string, int> parse = delegate(string value)
            {
                return int.Parse(value);
            };
            DeSerializeText2D(filename, parse, out values);
        }
    }
}
