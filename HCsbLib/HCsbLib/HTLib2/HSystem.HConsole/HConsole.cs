using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class HConsole
	{
        public static string ParsePath(string value)
        {
            if(value.EndsWith("\\") == false)
                value += "\\";
            if(HDirectory.Exists(value))
                return value;
            throw new Exception();
        }
        public static T ReadValue<T>(string message, T defaultValue
                                    , Func<string,T> parse // [=null] or int.Parse, double.Parse, bool.Parse, ...
                                    , bool bEchoSelection // [=false]
                                    , bool bRepeatParsing // [=false]
                                    )
        {
            if(parse == null)
            {
                parse = delegate(string str)
                {
                    if(typeof(T) == typeof(bool   )) return (dynamic)(bool   .Parse(str));
                    if(typeof(T) == typeof(byte   )) return (dynamic)(byte   .Parse(str));
                    if(typeof(T) == typeof(sbyte  )) return (dynamic)(sbyte  .Parse(str));
                    if(typeof(T) == typeof(char   )) return (dynamic)(char   .Parse(str));
                    if(typeof(T) == typeof(decimal)) return (dynamic)(decimal.Parse(str));
                    if(typeof(T) == typeof(double )) return (dynamic)(double .Parse(str));
                    if(typeof(T) == typeof(float  )) return (dynamic)(float  .Parse(str));
                    if(typeof(T) == typeof(int    )) return (dynamic)(int    .Parse(str));
                    if(typeof(T) == typeof(uint   )) return (dynamic)(uint   .Parse(str));
                    if(typeof(T) == typeof(long   )) return (dynamic)(long   .Parse(str));
                    if(typeof(T) == typeof(ulong  )) return (dynamic)(ulong  .Parse(str));
                  //if(typeof(T) == typeof(object )) return (dynamic)(object .Parse(str));
                    if(typeof(T) == typeof(short  )) return (dynamic)(short  .Parse(str));
                    if(typeof(T) == typeof(ushort )) return (dynamic)(ushort .Parse(str));
                  //if(typeof(T) == typeof(string )) return (dynamic)(string .Parse(str));

                    return (dynamic)str;
                };
            }

            T value = defaultValue;
            int maxiter = 10;
            while(maxiter > 0)
            {
                maxiter--;

                Console.Error.Write(message+string.Format("[{0}]: ",defaultValue));
                string line = Console.ReadLine();

                if(line.Trim().Length == 0)
                {
                    value = defaultValue;
                    break;
                }

                try
                {
                    value = parse(line);
                    break;
                }
                catch(Exception)
                {
                    if(bRepeatParsing == false)
                    {
                        value = defaultValue;
                        break;
                    }
                }
            }
            if(bEchoSelection)
            {
                Console.Error.WriteLine("  Selection: "+value);
                Console.WriteLine("  Selection: "+value);
            }
            return value;
        }
	}
}
