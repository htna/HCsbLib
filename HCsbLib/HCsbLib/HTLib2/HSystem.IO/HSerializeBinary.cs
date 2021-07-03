using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HTLib2
{
	public static partial class HSerialize
	{
        public static void SerializeBinary<T0                                    >(string filename, T0 obj0                                                                                 ) { _SerializeBinary(filename, obj0                                                       ); }
        public static void SerializeBinary<T0, T1                                >(string filename, T0 obj0, T1 obj1                                                                        ) { _SerializeBinary(filename, obj0, obj1                                                 ); }
        public static void SerializeBinary<T0, T1, T2                            >(string filename, T0 obj0, T1 obj1, T2 obj2                                                               ) { _SerializeBinary(filename, obj0, obj1, obj2                                           ); }
        public static void SerializeBinary<T0, T1, T2, T3                        >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3                                                      ) { _SerializeBinary(filename, obj0, obj1, obj2, obj3                                     ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4                    >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4                                             ) { _SerializeBinary(filename, obj0, obj1, obj2, obj3, obj4                               ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5                >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5                                    ) { _SerializeBinary(filename, obj0, obj1, obj2, obj3, obj4, obj5                         ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6            >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6                           ) { _SerializeBinary(filename, obj0, obj1, obj2, obj3, obj4, obj5, obj6                   ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6, T7        >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7                  ) { _SerializeBinary(filename, obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7             ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8    >(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8         ) { _SerializeBinary(filename, obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8       ); }
        public static void SerializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string filename, T0 obj0, T1 obj1, T2 obj2, T3 obj3, T4 obj4, T5 obj5, T6 obj6, T7 obj7, T8 obj8, T9 obj9) { _SerializeBinary(filename, obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9 ); }

        public static bool DeserializeBinary<T0                                    >(string filename, out T0 obj0                                                                                                                     ) { Type[] types = new Type[] { typeof(T0)                                                                                                             }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0);                                                                                                                                                                                     return false; } HDebug.Assert(objs.Length ==  1); obj0 = (T0)objs[0];                                                                                                                                                                                     return true; }
        public static bool DeserializeBinary<T0, T1                                >(string filename, out T0 obj0, out T1 obj1                                                                                                        ) { Type[] types = new Type[] { typeof(T0), typeof(T1)                                                                                                 }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1);                                                                                                                                                                 return false; } HDebug.Assert(objs.Length ==  2); obj0 = (T0)objs[0]; obj1 = (T1)objs[1];                                                                                                                                                                 return true; }
        public static bool DeserializeBinary<T0, T1, T2                            >(string filename, out T0 obj0, out T1 obj1, out T2 obj2                                                                                           ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2)                                                                                     }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2);                                                                                                                                             return false; } HDebug.Assert(objs.Length ==  3); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2];                                                                                                                                             return true; }
        public static bool DeserializeBinary<T0, T1, T2, T3                        >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3                                                                              ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3)                                                                         }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3);                                                                                                                         return false; } HDebug.Assert(objs.Length ==  4); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3];                                                                                                                         return true; }
        public static bool DeserializeBinary<T0, T1, T2, T3, T4                    >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4                                                                 ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4)                                                             }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4);                                                                                                     return false; } HDebug.Assert(objs.Length ==  5); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4];                                                                                                     return true; }
        public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5                >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5                                                    ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)                                                 }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5);                                                                                 return false; } HDebug.Assert(objs.Length ==  6); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5];                                                                                 return true; }
        public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5, T6            >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6                                       ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)                                     }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6);                                                             return false; } HDebug.Assert(objs.Length ==  7); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6];                                                             return true; }
        public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5, T6, T7        >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7                          ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)                         }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6); obj7 = default(T7);                                         return false; } HDebug.Assert(objs.Length ==  8); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7];                                         return true; }
        public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8    >(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7, out T8 obj8             ) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)             }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6); obj7 = default(T7); obj8 = default(T8);                     return false; } HDebug.Assert(objs.Length ==  9); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7]; obj8 = (T8)objs[8];                     return true; }
        public static bool DeserializeBinary<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string filename, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7, out T8 obj8, out T9 obj9) { Type[] types = new Type[] { typeof(T0), typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9) }; object[] objs = _DeserializeBinary(filename, types); if(objs == null) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6); obj7 = default(T7); obj8 = default(T8); obj9 = default(T9); return false; } HDebug.Assert(objs.Length == 10); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7]; obj8 = (T8)objs[8]; obj9 = (T9)objs[9]; return true; }

        public static void _SerializeBinary(string filename, params object[] objs)
		{
            string lockname = "Serializer: "+filename.Replace("\\", "@");
            using(new NamedLock(lockname))
            {
                Stream stream = HFile.Open(filename, FileMode.Create);
                HBinaryWriter writer = new BinaryWriter(stream);
                writer.Write("SerializeBinaryBegin");
                {
                    int count = objs.Length;
                    writer.HWrite(count);
                    for(int i=0; i<count; i++)
                    {
                        ((IBinarySerializable)objs[i]).Serialize(writer);
                    }
                }
                writer.Write("SerializeBinaryEnd");
                stream.Flush();
                stream.Close();
            }
		}
        public static object[] _DeserializeBinary(string filename, params Type[] types)
        {
            string lockname = "Serializer: "+filename.Replace("\\", "@");
            using(new NamedLock(lockname))
            {
                Stream stream = HFile.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                HBinaryReader reader = new BinaryReader(stream);
                object[] objs;
                if(reader.ReadString() != "SerializeBinaryBegin") return null;
                {
                    int count;
                    reader.HRead(out count);
                    if(count != types.Length)
                        return null;
                    objs = new object[count];
                    for(int i = 0; i < count; i++)
                    {
                        objs[i] = Activator.CreateInstance(types[i], reader);
                        // T(GBubartReader reader) constructor
                    }
                }
                if(reader.ReadString() != "SerializeBinaryEnd") return null;
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

                HBinaryWriter writer = new BinaryWriter(stream);
                {
                    if(ver != null)
                        writer.HWrite(new Ver(ver.Value));
                }
                {
                    System.Int32 count = objs.Length;
                    writer.HWrite(count);
                    for(int i = 0; i < count; i++)
                    {
                        Type   type = objs[i].type;
                        object obj  = objs[i].obj ;
                        writer.HWrite(obj);
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
                HBinaryReader reader = new BinaryReader(stream);
                {
                    if(ver != null)
                    {
                        try
                        {
                            Ver sver;
                            reader.HRead(out sver);
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
                    reader.HRead(out count);
                    if(count != types.Length)
                        return null;
                    objs = new object[count];
                    for(int i = 0; i < count; i++)
                    {
                        reader.HRead(out objs[i], types[i]);
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
