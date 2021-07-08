using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HTLib2
{
    using IBS = IBinarySerializable;
	public static partial class HSerialize
	{ 
        public static void SerializeBinary<T0                                    >(string filename, T0 obj0                                                                                 ) where T0:IBS                                                                                                                      { _SerializeBinary(filename, GetOBS(obj0)                                                                                                                               ); }
        public static void SerializeBinary<T0,T1                                 >(string filename, T0 obj0, T1 obj1                                                                        ) where T0:IBS where T1:IBS                                                                                                         { _SerializeBinary(filename, GetOBS(obj0), GetOBS(obj1)                                                                                                                 ); }
        public static void SerializeBinary<T0, T1, T2                            >(string filename, T0 obj0, T1 obj1, T2 obj2                                                               ) where T0:IBS where T1:IBS where T2:IBS                                                                                            { _SerializeBinary(filename, GetOBS(obj0), GetOBS(obj1), GetOBS(obj2)                                                                                                   ); }
        public static void SerializeBinary<T0, T1, T2, T3                        >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3                                                      ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS                                                                               { _SerializeBinary(filename, GetOBS(obj0), GetOBS(obj1), GetOBS(obj2), GetOBS(obj3)                                                                                     ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4                    >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4                                             ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS                                                                  { _SerializeBinary(filename, GetOBS(obj0), GetOBS(obj1), GetOBS(obj2), GetOBS(obj3), GetOBS(obj4)                                                                       ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5                >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5                                    ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS                                                     { _SerializeBinary(filename, GetOBS(obj0), GetOBS(obj1), GetOBS(obj2), GetOBS(obj3), GetOBS(obj4), GetOBS(obj5)                                                         ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6            >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6                           ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS where T6:IBS                                        { _SerializeBinary(filename, GetOBS(obj0), GetOBS(obj1), GetOBS(obj2), GetOBS(obj3), GetOBS(obj4), GetOBS(obj5), GetOBS(obj6)                                           ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6, T7        >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7                  ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS where T6:IBS where T7:IBS                           { _SerializeBinary(filename, GetOBS(obj0), GetOBS(obj1), GetOBS(obj2), GetOBS(obj3), GetOBS(obj4), GetOBS(obj5), GetOBS(obj6), GetOBS(obj7)                             ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8    >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8         ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS where T6:IBS where T7:IBS where T8:IBS              { _SerializeBinary(filename, GetOBS(obj0), GetOBS(obj1), GetOBS(obj2), GetOBS(obj3), GetOBS(obj4), GetOBS(obj5), GetOBS(obj6), GetOBS(obj7), GetOBS(obj8)               ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS where T6:IBS where T7:IBS where T8:IBS where T9:IBS { _SerializeBinary(filename, GetOBS(obj0), GetOBS(obj1), GetOBS(obj2), GetOBS(obj3), GetOBS(obj4), GetOBS(obj5), GetOBS(obj6), GetOBS(obj7), GetOBS(obj8), GetOBS(obj9) ); }

        public static void DeserializeBinary<T0                                    >(string filename, out T0 obj0                                                                                                                     ) where T0:IBS                                                                                                                      { object[] objs = _DeserializeBinary(filename, GetOBD<T0>()                                                                                                                               ); HDebug.Assert(objs.Length ==  1); obj0 = (T0)objs[0];                                                                                                                                                                                     }
        public static void DeserializeBinary<T0, T1                                >(string filename, out T0 obj0, out T1 obj1                                                                                                        ) where T0:IBS where T1:IBS                                                                                                         { object[] objs = _DeserializeBinary(filename, GetOBD<T0>(), GetOBD<T1>()                                                                                                                 ); HDebug.Assert(objs.Length ==  2); obj0 = (T0)objs[0]; obj1 = (T1)objs[1];                                                                                                                                                                 }
        public static void DeserializeBinary<T0, T1, T2                            >(string filename, out T0 obj0, out T1 obj1, out T2 obj2                                                                                           ) where T0:IBS where T1:IBS where T2:IBS                                                                                            { object[] objs = _DeserializeBinary(filename, GetOBD<T0>(), GetOBD<T1>(), GetOBD<T2>()                                                                                                   ); HDebug.Assert(objs.Length ==  3); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2];                                                                                                                                             }
        public static void DeserializeBinary<T0, T1, T2, T3                        >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3                                                                              ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS                                                                               { object[] objs = _DeserializeBinary(filename, GetOBD<T0>(), GetOBD<T1>(), GetOBD<T2>(), GetOBD<T3>()                                                                                     ); HDebug.Assert(objs.Length ==  4); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3];                                                                                                                         }
        public static void DeserializeBinary<T0, T1, T2, T3, T4                    >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4                                                                 ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS                                                                  { object[] objs = _DeserializeBinary(filename, GetOBD<T0>(), GetOBD<T1>(), GetOBD<T2>(), GetOBD<T3>(), GetOBD<T4>()                                                                       ); HDebug.Assert(objs.Length ==  5); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4];                                                                                                     }
        public static void DeserializeBinary<T0, T1, T2, T3, T4, T5                >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5                                                    ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS                                                     { object[] objs = _DeserializeBinary(filename, GetOBD<T0>(), GetOBD<T1>(), GetOBD<T2>(), GetOBD<T3>(), GetOBD<T4>(), GetOBD<T5>()                                                         ); HDebug.Assert(objs.Length ==  6); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5];                                                                                 }
        public static void DeserializeBinary<T0, T1, T2, T3, T4, T5, T6            >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6                                       ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS where T6:IBS                                        { object[] objs = _DeserializeBinary(filename, GetOBD<T0>(), GetOBD<T1>(), GetOBD<T2>(), GetOBD<T3>(), GetOBD<T4>(), GetOBD<T5>(), GetOBD<T6>()                                           ); HDebug.Assert(objs.Length ==  7); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6];                                                             }
        public static void DeserializeBinary<T0, T1, T2, T3, T4, T5, T6, T7        >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7                          ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS where T6:IBS where T7:IBS                           { object[] objs = _DeserializeBinary(filename, GetOBD<T0>(), GetOBD<T1>(), GetOBD<T2>(), GetOBD<T3>(), GetOBD<T4>(), GetOBD<T5>(), GetOBD<T6>(), GetOBD<T7>()                             ); HDebug.Assert(objs.Length ==  8); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7];                                         }
        public static void DeserializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8    >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7, out T8 obj8             ) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS where T6:IBS where T7:IBS where T8:IBS              { object[] objs = _DeserializeBinary(filename, GetOBD<T0>(), GetOBD<T1>(), GetOBD<T2>(), GetOBD<T3>(), GetOBD<T4>(), GetOBD<T5>(), GetOBD<T6>(), GetOBD<T7>(), GetOBD<T8>()               ); HDebug.Assert(objs.Length ==  9); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7]; obj8 = (T8)objs[8];                     }
        public static void DeserializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7, out T8 obj8, out T9 obj9) where T0:IBS where T1:IBS where T2:IBS where T3:IBS where T4:IBS where T5:IBS where T6:IBS where T7:IBS where T8:IBS where T9:IBS { object[] objs = _DeserializeBinary(filename, GetOBD<T0>(), GetOBD<T1>(), GetOBD<T2>(), GetOBD<T3>(), GetOBD<T4>(), GetOBD<T5>(), GetOBD<T6>(), GetOBD<T7>(), GetOBD<T8>(), GetOBD<T9>() ); HDebug.Assert(objs.Length == 10); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7]; obj8 = (T8)objs[8]; obj9 = (T9)objs[9]; }

        // OBS : Object Binary Serializer
        static IOBS GetOBS<T>(T obj)
            where T  : IBS
        {
            return new OBS<T>{ obj = obj };
        }
        interface IOBS
        {
            void SerializeBinary(HBinaryWriter writer);
        }
        class OBS<T> : IOBS
            where T  : IBS
        {
            public T obj;
            public void SerializeBinary(HBinaryWriter writer)
            {
                obj.BinarySerialize(writer);
            }
        }
        static void _SerializeBinary(string filename, params IOBS[] objBinSerializers)
		{
            string lockname = "Serializer: "+filename.Replace("\\", "@");
            using(new NamedLock(lockname))
            {
                Stream stream = HFile.Open(filename, FileMode.Create);
                HBinaryWriter writer = new HBinaryWriter(stream);
                writer.Write("SerializeBinaryBegin");
                {
                    int count = objBinSerializers.Length;
                    writer.Write(count);
                    for(int i=0; i<count; i++)
                        objBinSerializers[i].SerializeBinary(writer);
                }
                writer.Write("SerializeBinaryEnd");
                stream.Flush();
                stream.Close();
            }
		}
        // OBD : Object Binary Deserializer
        static IOBD GetOBD<T>()
            where T  : IBS
        {
            return new OBD<T>();
        }
        interface IOBD
        {
            object DeserializeBinary(HBinaryReader reader);
        }
        class OBD<T> : IOBD
            where T  : IBS
        {
            public object DeserializeBinary(HBinaryReader reader)
            {
                Type type = typeof(T);
                HDebug.Assert(type != null);
                object obj = Activator.CreateInstance(type, reader); // create object using reader by calling constructor Class(HBinaryReader reader)
                return obj;
            }
        }
        static object[] _DeserializeBinary(string filename, params IOBD[] objBinDeserializers)
        {
            string lockname = "Serializer: "+filename.Replace("\\", "@");
            using(new NamedLock(lockname))
            {
                Stream stream = HFile.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                HBinaryReader reader = new HBinaryReader(stream);
                object[] objs;
                if(reader.ReadString() != "SerializeBinaryBegin") return null;
                {
                    int count;
                    reader.Read(out count);
                    if(count != objBinDeserializers.Length)
                        return null;
                    objs = new object[count];
                    for(int i = 0; i < count; i++)
                        objs[i] = objBinDeserializers[i].DeserializeBinary(reader);
                }
                if(reader.ReadString() != "SerializeBinaryEnd")
                    throw new FileFormatException();
                stream.Close();
                return objs;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Obsolete functions

        [Obsolete] public static void SerializeBinary<T0                                    >(string filename, int? ver, T0 obj0                                                                                 ) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0)                                                                                                                                                                                     }); }
        [Obsolete] public static void SerializeBinary<T0, T1                                >(string filename, int? ver, T0 obj0, T1 obj1                                                                        ) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0), (typeof(T1), obj1)                                                                                                                                                                 }); }
        [Obsolete] public static void SerializeBinary<T0, T1, T2                            >(string filename, int? ver, T0 obj0, T1 obj1, T2 obj2                                                               ) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0), (typeof(T1), obj1), (typeof(T2), obj2)                                                                                                                                             }); }
        [Obsolete] public static void SerializeBinary<T0, T1, T2, T3                        >(string filename, int? ver, T0 obj0, T1 obj1, T2 obj2, T3 obj3                                                      ) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0), (typeof(T1), obj1), (typeof(T2), obj2), (typeof(T3), obj3)                                                                                                                         }); }
        [Obsolete] public static void SerializeBinary<T0, T1, T2, T3, T4                    >(string filename, int? ver, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4                                             ) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0), (typeof(T1), obj1), (typeof(T2), obj2), (typeof(T3), obj3), (typeof(T4), obj4)                                                                                                     }); }
        [Obsolete] public static void SerializeBinary<T0, T1, T2, T3, T4, T5                >(string filename, int? ver, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5                                    ) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0), (typeof(T1), obj1), (typeof(T2), obj2), (typeof(T3), obj3), (typeof(T4), obj4), (typeof(T5), obj5)                                                                                 }); }
        [Obsolete] public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6            >(string filename, int? ver, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6                           ) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0), (typeof(T1), obj1), (typeof(T2), obj2), (typeof(T3), obj3), (typeof(T4), obj4), (typeof(T5), obj5), (typeof(T6), obj6)                                                             }); }
        [Obsolete] public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6, T7        >(string filename, int? ver, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7                  ) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0), (typeof(T1), obj1), (typeof(T2), obj2), (typeof(T3), obj3), (typeof(T4), obj4), (typeof(T5), obj5), (typeof(T6), obj6), (typeof(T7), obj7)                                         }); }
        [Obsolete] public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8    >(string filename, int? ver, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8         ) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0), (typeof(T1), obj1), (typeof(T2), obj2), (typeof(T3), obj3), (typeof(T4), obj4), (typeof(T5), obj5), (typeof(T6), obj6), (typeof(T7), obj7), (typeof(T8), obj8)                     }); }
        [Obsolete] public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string filename, int? ver, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9) { _SerializeBinary(filename, ver, new (Type, object)[] { (typeof(T0), obj0), (typeof(T1), obj1), (typeof(T2), obj2), (typeof(T3), obj3), (typeof(T4), obj4), (typeof(T5), obj5), (typeof(T6), obj6), (typeof(T7), obj7), (typeof(T8), obj8), (typeof(T9), obj9) }); }
        [Obsolete] public static void _SerializeBinary(string filename, int? ver, (Type type, object obj)[] objs)
		{
            string lockname = "Serializer: "+filename.Replace("\\", "@");
            using(new NamedLock(lockname))
            {
                Stream stream = HFile.Open(filename, FileMode.Create);

                HBinaryWriter hwriter = new HBinaryWriter(stream);
                {
                    if(ver != null)
                        hwriter.Write(new Ver(ver.Value));
                }
                {
                    System.Int32 count = objs.Length;
                    hwriter.Write(count);
                    for(int i = 0; i < count; i++)
                    {
                        Type   type = objs[i].type;
                        object obj  = objs[i].obj ;
/////////////////////////////////////////////////////////////////////////hwriter.HWrite(obj);
                    }
                }
                stream.Flush();
                stream.Close();
            }
		}
        [Obsolete] public static object[] _DeserializeBinary(string filename, int? ver, Type[] types)
        {
            string lockname = "Serializer: "+filename.Replace("\\", "@");
            using(new NamedLock(lockname))
            {
                Stream stream = HFile.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                HBinaryReader reader = new HBinaryReader(stream);
                {
                    if(ver != null)
                    {
                        try
                        {
                            Ver sver;
                            reader.Read(out sver);
                            if(sver.ver != ver.Value)
                            {
                                stream.Close();
                                return null;
                            }
                        }
                        catch(Exception)
                        {
                            stream.Close();
                            return null;
                        }
                    }
                }
                object[] objs;
                {
                    System.Int32 count;
                    reader.Read(out count);
                    if(count != types.Length)
                        return null;
                    objs = new object[count];
                    for(int i = 0; i < count; i++)
                    {
                        reader.Read(types[i], out objs[i]);
                    }
                }
                stream.Close();
                return objs;
            }
        }
        //public static T Deserialize<T>(string filename, int? ver)
		//{
        //    object[] objs;
        //    HDebug.Verify(_Deserialize(filename, ver, out objs));
		//	HDebug.Assert(objs.Length == 1);
		//	return (T)objs[0];
		//}
        [Obsolete] public static bool DeserializeBinary<T0                                    >(string filename, int? ver, out T0 obj0                                                                                                                     ) { Type[] types = new Type[] { typeof(T0)                                                                                                             }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0);                                                                                                                                                                                     return false; } HDebug.Assert(objs.Length ==  1); obj0 = (T0)objs[0];                                                                                                                                                                                     return true; }
        [Obsolete] public static bool DeserializeBinary<T0, T1                                >(string filename, int? ver, out T0 obj0, out T1 obj1                                                                                                        ) { Type[] types = new Type[] { typeof(T0), typeof(T1)                                                                                                 }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1);                                                                                                                                                                 return false; } HDebug.Assert(objs.Length ==  2); obj0 = (T0)objs[0]; obj1 = (T1)objs[1];                                                                                                                                                                 return true; }
        [Obsolete] public static bool DeserializeBinary<T0, T1, T2                            >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2                                                                                           ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2)                                                                                     }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2);                                                                                                                                             return false; } HDebug.Assert(objs.Length ==  3); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2];                                                                                                                                             return true; }
        [Obsolete] public static bool DeserializeBinary<T0, T1, T2, T3                        >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3                                                                              ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3)                                                                         }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3);                                                                                                                         return false; } HDebug.Assert(objs.Length ==  4); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3];                                                                                                                         return true; }
        [Obsolete] public static bool DeserializeBinary<T0, T1, T2, T3, T4                    >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4                                                                 ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4)                                                             }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4);                                                                                                     return false; } HDebug.Assert(objs.Length ==  5); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4];                                                                                                     return true; }
        [Obsolete] public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5                >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5                                                    ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)                                                 }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5);                                                                                 return false; } HDebug.Assert(objs.Length ==  6); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5];                                                                                 return true; }
        [Obsolete] public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5, T6            >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6                                       ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)                                     }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6);                                                             return false; } HDebug.Assert(objs.Length ==  7); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6];                                                             return true; }
        [Obsolete] public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5, T6, T7        >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7                          ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)                         }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6); obj7 = default(T7);                                         return false; } HDebug.Assert(objs.Length ==  8); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7];                                         return true; }
        [Obsolete] public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8    >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7, out T8 obj8             ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)             }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6); obj7 = default(T7); obj8 = default(T8);                     return false; } HDebug.Assert(objs.Length ==  9); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7]; obj8 = (T8)objs[8];                     return true; }
        [Obsolete] public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7, out T8 obj8, out T9 obj9) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }; object[] objs = _DeserializeBinary(filename, ver, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6); obj7 = default(T7); obj8 = default(T8); obj9 = default(T9); return false; } HDebug.Assert(objs.Length == 10); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7]; obj8 = (T8)objs[8]; obj9 = (T9)objs[9]; return true; }
    }
}
