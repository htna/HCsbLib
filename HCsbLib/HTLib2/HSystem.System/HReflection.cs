using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.ConstrainedExecution;
using System.Linq;

namespace HTLib2
{
    using MemoryStream    = System.IO.MemoryStream;
    using BinaryFormatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;
    public static class HReflection
    {
        public static string AssemblyQualifiedName<T>()
        {
            return typeof(T).AssemblyQualifiedName;
        }
        public static object CreateInstance(string AssemblyQualifiedName)
        {
            Type   type = Type.GetType(AssemblyQualifiedName);
            object obj  = Activator.CreateInstance(type);
            return obj;
        }

        // http://stackoverflow.com/questions/6979718/c-sharp-object-to-string-and-back
        public static string ObjectToString(object obj)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return System.Convert.ToBase64String(ms.ToArray());
            }
        }
        public static object StringToObject(string base64String)
        {
            byte[] bytes = System.Convert.FromBase64String(base64String);
            using(MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return new BinaryFormatter().Deserialize(ms);
            }
        }
    }
}
