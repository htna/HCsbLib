using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Wolfram.NETLink;


// C:\Program Files\Wolfram Research\Mathematica\7.0\SystemFiles\Links\NETLink

namespace HTLib2
{
    using ITuple            = System.Runtime.CompilerServices.ITuple;
    using IDictionary       = System.Collections.IDictionary        ;
    using DictionaryEntry   = System.Collections.DictionaryEntry    ;
    using IList             = System.Collections.IList              ;
    using IEnumerable       = System.Collections.IEnumerable        ;

    public partial class Mathematica
    {
        public static IEnumerable<string> EnumString(object obj)
        {
            foreach(var str in _EnumString(null, obj))
                yield return str;
        }
        public static IEnumerable<string> EnumString(string format, object obj)
        {
            foreach(var str in _EnumString(format, obj))
                yield return str;
        }
        protected static IEnumerable<string> _EnumString(string format, IEnumerable objs)
        {
            yield return "{";

            if(objs != null)
            {
                bool addcomma = false;
                foreach(object obj in objs)
                {
                    if(addcomma) yield return ", ";
                    else         addcomma = true;

                    foreach(var str in _EnumString(format, obj))
                        yield return str;
                }
            }

            yield return "}";
        }
        protected static IEnumerable<string> _EnumString(string format, IDictionary obj)
        {
            yield return "<| ";

            bool addcomma = false;
            foreach (DictionaryEntry key_value in obj)
            {
                if(addcomma) yield return ", ";
                else         addcomma = true;
                
                foreach(var str in _EnumString(format, key_value.Key))
                    yield return str;

                yield return "->";

                foreach(var str in _EnumString(format, key_value.Value))
                    yield return str;
            }

            yield return " |>";
        }
        protected static IEnumerable<string> _EnumString(string format, ITuple obj)
        {
            int leng = obj.Length;
            object[] objs = new object[leng];
            for(int i=0; i<leng; i++)
                objs[i] = obj[i];

            foreach(var str in _EnumString(format, objs))
                yield return str;
        }
        protected static IEnumerable<string> _EnumString(string format, object obj)
        {
            if(obj == null)
                yield break;

            switch(obj)
            {
                case Vector data:
                    {
                        foreach(var str in _EnumString(format, data._data))
                            yield return str;
                    }
                    yield break;

                case int data:
                    {
                        if(format == null)  yield return data.ToString();
                        else                yield return string.Format(format, data);
                    }
                    yield break;

                case double data:
                    {
                        string str;
                        if(format == null) str = data.ToString();
                        else               str = string.Format(format, (double)obj);

                        if(str.Contains("E") || str.Contains("e"))
                        {
                            str = str.Replace("e", "*10^");
                            str = str.Replace("E", "*10^");
                        }

                        yield return str;
                    }
                    yield break;

                case string data:
                    {
                        string str = data;
                        if(str.StartsWith("(* ") && str.EndsWith(" *)"))
                        {
                            // add comment as it is
                            yield return str;
                        }
                        else
                        {
                            str = str.Replace("\\", "\\\\");
                            str = "\"" + str + "\"";

                            yield return str;
                        }
                    }
                    yield break;

                case char data:
                    {
                        string str = "\'" + data + "\'";
                        yield return str;
                    }
                    yield break;

                case ITuple         data: foreach(var str in _EnumString(format, data)) yield return str; yield break;
                case IDictionary    data: foreach(var str in _EnumString(format, data)) yield return str; yield break;
                case IEnumerable    data: foreach(var str in _EnumString(format, data)) yield return str; yield break;
                //case IList

                default:
                    {
                        if(obj is object[])
                        {
                            foreach(var str in _EnumString(format, (object[])obj))
                                yield return str;
                        }
                    }
                    yield break;
            }

            throw new Exception();
        }
    }
}
