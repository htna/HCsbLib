using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    using IList       = System.Collections.IList;
    using IDictionary = System.Collections.IDictionary;
    public partial struct HBinaryWriter
    {
        //  public class Data : IBinarySerializable
        //  {
        //      public int   value ;
        //      public int[] values;
        //      public void Serialize(HBinaryWriter writer)
        //      {
        //          writer.HWrite(value );
        //          writer.HWrite(values);
        //      }
        //      public void Deserialize(HBinaryWriter writer)
        //      {
        //          writer.HRead(out value );
        //          writer.HRead(out values);
        //      }
        //  }


        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(double[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(int   [] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(long  [] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(string[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(bool  [] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<double> values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<int   > values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<long  > values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<string> values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(List<bool  > values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<  T>(T               value ) where T : IBinarySerializable { value.Serialize(this); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<  T>(List<T>         values) where T : IBinarySerializable { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) values[i].Serialize(this); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<  T>(T[]             values) where T : IBinarySerializable { writer.Write(values.Length); for(int i=0; i<values.Length; i++) values[i].Serialize(this); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write<K,T>(Dictionary<K,T> dict  ) where T : IBinarySerializable {
            writer.Write(dict.Count);
            int cnt = 0;
            foreach(var key_val in dict)
            {
                cnt ++;
                K key = key_val.Key;
                T val = key_val.Value;
                _HWrite<K>(key);
                _HWrite<T>(val);
            }
            HDebug.Assert(cnt == dict.Count);
        }

        void _HWriteDouble(object value) { writer.Write((double)value); }
        void _HWriteInt   (object value) { writer.Write((int   )value); }
        void _HWriteLong  (object value) { writer.Write((long  )value); }
        void _HWriteString(object value) { writer.Write((string)value); }
        void _HWriteBool  (object value) { writer.Write((bool  )value); }
        void _HWriteBinarySerializable<T>(object obj)
        {
            if(typeof(T).IsClass)
            {
                bool firstTime;
                long objid = GetObjId(obj, out firstTime);
                writer.Write(objid);
                if(firstTime)
                {
                    // string type_name = value.GetType().AssemblyQualifiedName;
                    // writer.Write(type_name);
                    ((IBinarySerializable)obj).Serialize(this);
                }
            }
            else
            {
                // string type_name = value.GetType().AssemblyQualifiedName;
                // writer.Write(type_name);
                ((IBinarySerializable)obj).Serialize(this);
            }
        }
        //static void _HWriteBinarySerializable(HBinaryWriter writer, object value)
        //{
        //    if((value is IBinarySerializable) == false)
        //        throw new HException();
        //    string type_name = value.GetType().AssemblyQualifiedName;
        //    writer.Write(type_name);
        //    ((IBinarySerializable)value).Serialize(writer);
        //}
        //static void _HWriteList(HBinaryWriter writer, object value)
        //{
        //    if((value is IList) == false)
        //        throw new HException();
        //    IList values = (IList)value;
        //    writer.Write(values.Count);
        //    for(int i=0; i<values.Count; i++)
        //        _HWrite(writer, values[i]);
        //}
        //static void _HWriteArray(HBinaryWriter writer, object value)
        //{
        //    if((value is Array) == false)
        //        throw new HException();
        //    Array values = (Array)value;
        //    writer.Write(values.Length);
        //    for(int i=0; i<values.Length; i++)
        //        _HWrite(writer, values.GetValue(i));
        //}
        //public static void _HWriteDictionary(this HBinaryWriter writer, object value)
        //{
        //    if((value is IDictionary) == false)
        //        throw new HException();
        //    IDictionary dict = (IDictionary)value;
        //    writer.Write(dict.Count);
        //    var dict_enum = dict.GetEnumerator();
        //    //foreach(var key_val in dictenum)
        //    int cnt = 0;
        //    while(dict_enum.MoveNext())
        //    {
        //        cnt ++;
        //        _HWrite(writer, dict_enum.Key  );
        //        _HWrite(writer, dict_enum.Value);
        //    }
        //    HDebug.Assert(cnt == dict.Count);
        //}
        void _HWrite<T>(T value)
        {
            //string type_name = value.GetType()AssemblyQualifiedName;
            Type type = typeof(T);
            if(type.IsSubclassOf(typeof(double             ))) { _HWriteDouble               (value); return; }
            if(type.IsSubclassOf(typeof(int                ))) { _HWriteInt                  (value); return; }
            if(type.IsSubclassOf(typeof(long               ))) { _HWriteLong                 (value); return; }
            if(type.IsSubclassOf(typeof(string             ))) { _HWriteString               (value); return; }
            if(type.IsSubclassOf(typeof(bool               ))) { _HWriteBool                 (value); return; }
            if(type.IsSubclassOf(typeof(IBinarySerializable))) { _HWriteBinarySerializable<T>(value); return; }
            //if(value is IList              ) { _HWriteList              (writer, value); return; }
            //if(value is Array              ) { _HWriteArray             (writer, value); return; }
            //if(value is IDictionary        ) { _HWriteDictionary        (writer, value); return; }
            throw new Exception();
        }
    }
}
