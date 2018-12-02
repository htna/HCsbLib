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
	    [Serializable]
        class Ver : ISerializable
        {
            public readonly int ver;
		    public Ver(int ver)
		    {
                this.ver = ver;
		    }
            public Ver(SerializationInfo info, StreamingContext context)
		    {
                ver = (int)info.GetValue("ver", typeof(int));
		    }
		    public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("ver", ver);
            }
    		public static implicit operator int(Ver ver)
            {
                return ver.ver;
            }
        }
        public class SerializedVersionException : Exception
        {
            public    SerializedVersionException() { }
            public    SerializedVersionException(string message) : base(message) { }
            public    SerializedVersionException(string message, Exception innerException) : base (message,innerException) { }
        }

        public static void Serialize(string filename, int? ver, object obj0                                                                                                                     ) { _Serialize(filename, ver, new object[] { obj0                                                       }); }
        public static void Serialize(string filename, int? ver, object obj0, object obj1                                                                                                        ) { _Serialize(filename, ver, new object[] { obj0, obj1                                                 }); }
        public static void Serialize(string filename, int? ver, object obj0, object obj1, object obj2                                                                                           ) { _Serialize(filename, ver, new object[] { obj0, obj1, obj2                                           }); }
        public static void Serialize(string filename, int? ver, object obj0, object obj1, object obj2, object obj3                                                                              ) { _Serialize(filename, ver, new object[] { obj0, obj1, obj2, obj3                                     }); }
        public static void Serialize(string filename, int? ver, object obj0, object obj1, object obj2, object obj3, object obj4                                                                 ) { _Serialize(filename, ver, new object[] { obj0, obj1, obj2, obj3, obj4                               }); }
        public static void Serialize(string filename, int? ver, object obj0, object obj1, object obj2, object obj3, object obj4, object obj5                                                    ) { _Serialize(filename, ver, new object[] { obj0, obj1, obj2, obj3, obj4, obj5                         }); }
        public static void Serialize(string filename, int? ver, object obj0, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6                                       ) { _Serialize(filename, ver, new object[] { obj0, obj1, obj2, obj3, obj4, obj5, obj6                   }); }
        public static void Serialize(string filename, int? ver, object obj0, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7                          ) { _Serialize(filename, ver, new object[] { obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7             }); }
        public static void Serialize(string filename, int? ver, object obj0, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8             ) { _Serialize(filename, ver, new object[] { obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8       }); }
        public static void Serialize(string filename, int? ver, object obj0, object obj1, object obj2, object obj3, object obj4, object obj5, object obj6, object obj7, object obj8, object obj9) { _Serialize(filename, ver, new object[] { obj0, obj1, obj2, obj3, obj4, obj5, obj6, obj7, obj8, obj9 }); }

        public static void SerializeDepreciated(string filename, int? ver, object obj1, params object[] obj234)
        {
            List<object> objs = new List<object>();
            objs.Add(obj1);
            foreach(object obj in obj234)
                objs.Add(obj);
            _Serialize(filename, ver, objs.ToArray());
        }
        public static void _Serialize(string filename, int? ver, object[] objs)
		{
            string lockname = "Serializer: "+filename.Replace("\\", "@");
            using(new NamedLock(lockname))
            {
                Stream stream = HFile.Open(filename, FileMode.Create);
                BinaryFormatter bFormatter = new BinaryFormatter();
                {
                    if(ver != null)
                        bFormatter.Serialize(stream, new Ver(ver.Value));
                }
                {
                    System.Int32 count = objs.Length;
                    bFormatter.Serialize(stream, count);
                    for(int i = 0; i < count; i++)
                    {
                        bFormatter.Serialize(stream, objs[i]);
                    }
                }
                stream.Flush();
                stream.Close();
            }
		}
        public static bool _Deserialize(string filename, int? ver, out object[] objs)
        {
            string lockname = "Serializer: "+filename.Replace("\\", "@");
            using(new NamedLock(lockname))
            {
                Stream stream = HFile.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                BinaryFormatter bFormatter = new BinaryFormatter();
                {
                    if(ver != null)
                    {
                        try
                        {
                            Ver sver = (Ver)bFormatter.Deserialize(stream);
                            if(sver.ver != ver.Value)
                            {
                                stream.Close();
                                objs = null;
                                return false;
                            }
                        }
                        catch(Exception)
                        {
                            stream.Close();
                            objs = null;
                            return false;
                        }
                    }
                }
                {
                    System.Int32 count = (System.Int32)bFormatter.Deserialize(stream);
                    objs = new object[count];
                    for(int i = 0; i < count; i++)
                    {
                        objs[i] = bFormatter.Deserialize(stream);
                    }
                }
                stream.Close();
            }
            return true;
        }
        public static T Deserialize<T>(string filename, int? ver)
		{
            object[] objs;
            HDebug.Verify(_Deserialize(filename, ver, out objs));
			HDebug.Assert(objs.Length == 1);
			return (T)objs[0];
		}
        //public static bool DeserializeIfExist<T>(string filename, int? ver, out T obj)
        //{
        //    if(HFile.Exists(filename) == false)
        //    {
        //        obj = default(T);
        //        return false;
        //    }
        //    return Deserialize(filename, ver, out obj);
        //}
        //public static bool Deserialize<T>(string filename, int? ver, out T obj)
        //{
        //    object[] objs;
        //    if(Deserialize(filename, ver, out objs) == false)
        //    {
        //        obj = default(T);
        //        return false;
        //    }
        //    HDebug.Assert(objs.Length == 1);
        //    obj = (T)objs[0];
        //    return true;
        //}
        //public static bool Deserialize<T1, T2>(string filename, int? ver, out T1 obj1, out T2 obj2)
        //{
        //    object[] objs;
        //    if(Deserialize(filename, ver, out objs) == false)
        //    {
        //        obj1 = default(T1);
        //        obj2 = default(T2);
        //        return false;
        //    }
        //    HDebug.Assert(objs.Length == 2);
        //    obj1 = (T1)objs[0];
        //    obj2 = (T2)objs[1];
        //    return true;
        //}
        //public static bool Deserialize<T1, T2, T3>(string filename, int? ver, out T1 obj1, out T2 obj2, out T3 obj3)
        //{
        //    object[] objs;
        //    if(Deserialize(filename, ver, out objs) == false)
        //    {
        //        obj1 = default(T1);
        //        obj2 = default(T2);
        //        obj3 = default(T3);
        //        return false;
        //    }
        //    HDebug.Assert(objs.Length == 3);
        //    obj1 = (T1)objs[0];
        //    obj2 = (T2)objs[1];
        //    obj3 = (T3)objs[2];
        //    return true;
        //}
        //public static bool Deserialize<T1, T2, T3, T4>(string filename, int? ver, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4)
        //{
        //    object[] objs;
        //    if(Deserialize(filename, ver, out objs) == false)
        //    {
        //        obj1 = default(T1);
        //        obj2 = default(T2);
        //        obj3 = default(T3);
        //        obj4 = default(T4);
        //        return false;
        //    }
        //    HDebug.Assert(objs.Length == 4);
        //    obj1 = (T1)objs[0];
        //    obj2 = (T2)objs[1];
        //    obj3 = (T3)objs[2];
        //    obj4 = (T4)objs[3];
        //    return true;
        //}
        //public static bool Deserialize<T1, T2, T3, T4, T5>(string filename, int? ver, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5)
        //{
        //    object[] objs;
        //    if(Deserialize(filename, ver, out objs) == false)
        //    {
        //        obj1 = default(T1);
        //        obj2 = default(T2);
        //        obj3 = default(T3);
        //        obj4 = default(T4);
        //        obj5 = default(T5);
        //        return false;
        //    }
        //    HDebug.Assert(objs.Length == 5);
        //    obj1 = (T1)objs[0];
        //    obj2 = (T2)objs[1];
        //    obj3 = (T3)objs[2];
        //    obj4 = (T4)objs[3];
        //    obj5 = (T5)objs[4];
        //    return true;
        //}

        public static bool Deserialize<T0                                    >(string filename, int? ver, out T0 obj0                                                                                                                     ) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0);                                                                                                                                                                                     return false; } HDebug.Assert(objs.Length ==  1); obj0 = (T0)objs[0];                                                                                                                                                                                     return true; }
        public static bool Deserialize<T0, T1                                >(string filename, int? ver, out T0 obj0, out T1 obj1                                                                                                        ) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0); obj1 = default(T1);                                                                                                                                                                 return false; } HDebug.Assert(objs.Length ==  2); obj0 = (T0)objs[0]; obj1 = (T1)objs[1];                                                                                                                                                                 return true; }
        public static bool Deserialize<T0, T1, T2                            >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2                                                                                           ) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2);                                                                                                                                             return false; } HDebug.Assert(objs.Length ==  3); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2];                                                                                                                                             return true; }
        public static bool Deserialize<T0, T1, T2, T3                        >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3                                                                              ) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3);                                                                                                                         return false; } HDebug.Assert(objs.Length ==  4); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3];                                                                                                                         return true; }
        public static bool Deserialize<T0, T1, T2, T3, T4                    >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4                                                                 ) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4);                                                                                                     return false; } HDebug.Assert(objs.Length ==  5); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4];                                                                                                     return true; }
        public static bool Deserialize<T0, T1, T2, T3, T4, T5                >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5                                                    ) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5);                                                                                 return false; } HDebug.Assert(objs.Length ==  6); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5];                                                                                 return true; }
        public static bool Deserialize<T0, T1, T2, T3, T4, T5, T6            >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6                                       ) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6);                                                             return false; } HDebug.Assert(objs.Length ==  7); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6];                                                             return true; }
        public static bool Deserialize<T0, T1, T2, T3, T4, T5, T6, T7        >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7                          ) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6); obj7 = default(T7);                                         return false; } HDebug.Assert(objs.Length ==  8); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7];                                         return true; }
        public static bool Deserialize<T0, T1, T2, T3, T4, T5, T6, T7, T8    >(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7, out T8 obj8             ) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6); obj7 = default(T7); obj8 = default(T8);                     return false; } HDebug.Assert(objs.Length ==  9); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7]; obj8 = (T8)objs[8];                     return true; }
        public static bool Deserialize<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(string filename, int? ver, out T0 obj0, out T1 obj1, out T2 obj2, out T3 obj3, out T4 obj4, out T5 obj5, out T6 obj6, out T7 obj7, out T8 obj8, out T9 obj9) { object[] objs; if(_Deserialize(filename, ver, out objs) == false) { obj0 = default(T0); obj1 = default(T1); obj2 = default(T2); obj3 = default(T3); obj4 = default(T4); obj5 = default(T5); obj6 = default(T6); obj7 = default(T7); obj8 = default(T8); obj9 = default(T9); return false; } HDebug.Assert(objs.Length == 10); obj0 = (T0)objs[0]; obj1 = (T1)objs[1]; obj2 = (T2)objs[2]; obj3 = (T3)objs[3]; obj4 = (T4)objs[4]; obj5 = (T5)objs[5]; obj6 = (T6)objs[6]; obj7 = (T7)objs[7]; obj8 = (T8)objs[8]; obj9 = (T9)objs[9]; return true; }
    }
}
