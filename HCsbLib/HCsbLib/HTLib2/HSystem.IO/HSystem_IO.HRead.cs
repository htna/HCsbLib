using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    public static partial class HSystem_IO
    {
        //  public class Data : IBinarySerializable
        //  {
        //      public int   value ;
        //      public int[] values;
        //      public void Serialize(System.IO.BinaryWriter writer)
        //      {
        //          writer.HWrite(value );
        //          writer.HWrite(values);
        //      }
        //      public void Deserialize(System.IO.BinaryReader reader)
        //      {
        //          reader.HRead(out value );
        //          reader.HRead(out values);
        //      }
        //  }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out double value) { value = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out int    value) { value = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out string value) { value = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out bool   value) { value = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out double[] values) { int length = reader.ReadInt32(); values = new double[length]; for(int i=0; i<length; i++) values[i] = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out int   [] values) { int length = reader.ReadInt32(); values = new int   [length]; for(int i=0; i<length; i++) values[i] = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out string[] values) { int length = reader.ReadInt32(); values = new string[length]; for(int i=0; i<length; i++) values[i] = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out bool  [] values) { int length = reader.ReadInt32(); values = new bool  [length]; for(int i=0; i<length; i++) values[i] = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out List<double> values) { int length = reader.ReadInt32(); values = new List<double>(length); for(int i=0; i<length; i++) values.Add(reader.ReadDouble ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out List<int   > values) { int length = reader.ReadInt32(); values = new List<int   >(length); for(int i=0; i<length; i++) values.Add(reader.ReadInt32  ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out List<string> values) { int length = reader.ReadInt32(); values = new List<string>(length); for(int i=0; i<length; i++) values.Add(reader.ReadString ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out List<bool  > values) { int length = reader.ReadInt32(); values = new List<bool  >(length); for(int i=0; i<length; i++) values.Add(reader.ReadBoolean()); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T  >(this BinaryReader reader, out T               value ) { value  = (T               )(_HRead          <T  >(reader)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T  >(this BinaryReader reader, out List<T>         values) { values = (List<T>         )(_HReadList      <T  >(reader)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T  >(this BinaryReader reader, out T[]             values) { values = (T[]             )(_HReadArray     <T  >(reader)); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T,U>(this BinaryReader reader, out Dictionary<T,U> dict  ) { dict   = (Dictionary<T,U> )(_HReadDictionary<T,U>(reader)); }

        static object _HReadDouble(BinaryReader reader) { return reader.ReadDouble (); }
        static object _HReadInt   (BinaryReader reader) { return reader.ReadInt32  (); }
        static object _HReadString(BinaryReader reader) { return reader.ReadString (); }
        static object _HReadBool  (BinaryReader reader) { return reader.ReadBoolean(); }
        static object _HReadBinarySerializable<T>(BinaryReader reader)
        {
            HDebug.Assert(typeof(T).IsSubclassOf(typeof(IBinarySerializable)));
            string type_name = reader.ReadString();
            Type   type = Type.GetType(type_name);
            object obj = Activator.CreateInstance(type);
            if((obj is T) == false)
                throw new HException("type-mismatch in HRead<T>(reader,out T value)\nAssign "+type.Name+" type into "+typeof(T).Name+" type.");
            ((IBinarySerializable)obj).Deserialize(reader);
            return obj;
        }
        static object _HReadList<T>(BinaryReader reader)
        {
            int leng = reader.ReadInt32();
            List<T> values = new List<T>(leng);
            for(int i=0; i<leng; i++)
            {
                object value = _HRead<T>(reader);
                values.Add((T)value);
            }
            return values;
        }
        static object _HReadArray<T>(BinaryReader reader)
        {
            int leng = reader.ReadInt32();
            T[] values = new T[leng];
            for(int i=0; i<leng; i++)
            {
                object value = _HRead<T>(reader);
                values[i] = (T)value;
            }
            return values;
        }
        static object _HReadDictionary<T,U>(BinaryReader reader)
        {
            int leng = reader.ReadInt32();
            Dictionary<T,U> dict = new Dictionary<T,U>(leng);
            for(int i=0; i<leng; i++)
            {
                T key = (T)_HRead<T>(reader);
                U val = (U)_HRead<U>(reader);
                dict.Add(key, val);
            }
            return dict;
        }
        static object _HRead<T>(BinaryReader reader)
        {
            Type type = typeof(T);
            if(type.IsSubclassOf(typeof(IBinarySerializable))) return _HReadBinarySerializable<T>(reader);
            string type_name = type.FullName;
            if(type_name == typeof(double ).FullName) return _HReadDouble  (reader);
            if(type_name == typeof(int    ).FullName) return _HReadInt     (reader);
            if(type_name == typeof(string ).FullName) return _HReadString  (reader);
            if(type_name == typeof(bool   ).FullName) return _HReadBool    (reader);
            if(type_name == typeof(List<T>).FullName) return _HReadList <T>(reader);
            if(type_name == typeof(    T[]).FullName) return _HReadArray<T>(reader);
            throw new Exception();
        }
    }
}
