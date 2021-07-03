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
    public static partial class HSystem_IO
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
        //      public void Deserialize(HBinaryReader reader)
        //      {
        //          reader.HRead(out value );
        //          reader.HRead(out values);
        //      }
        //  }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out double value) { value = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out int    value) { value = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out string value) { value = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out bool   value) { value = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out double[] values) { int length = reader.ReadInt32(); values = new double[length]; for(int i=0; i<length; i++) values[i] = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out int   [] values) { int length = reader.ReadInt32(); values = new int   [length]; for(int i=0; i<length; i++) values[i] = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out string[] values) { int length = reader.ReadInt32(); values = new string[length]; for(int i=0; i<length; i++) values[i] = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out bool  [] values) { int length = reader.ReadInt32(); values = new bool  [length]; for(int i=0; i<length; i++) values[i] = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out List<double> values) { int length = reader.ReadInt32(); values = new List<double>(length); for(int i=0; i<length; i++) values.Add(reader.ReadDouble ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out List<int   > values) { int length = reader.ReadInt32(); values = new List<int   >(length); for(int i=0; i<length; i++) values.Add(reader.ReadInt32  ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out List<string> values) { int length = reader.ReadInt32(); values = new List<string>(length); for(int i=0; i<length; i++) values.Add(reader.ReadString ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this HBinaryReader reader, out List<bool  > values) { int length = reader.ReadInt32(); values = new List<bool  >(length); for(int i=0; i<length; i++) values.Add(reader.ReadBoolean()); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead     (this HBinaryReader reader, out object value, Type type){ value  =                   _HRead          (reader, type                   ) ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T  >(this HBinaryReader reader, out T               value ) { value  = (T              )(_HRead          (reader, typeof(T              ))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T  >(this HBinaryReader reader, out List<T>         values) { values = (List<T>        )(_HReadList      (reader, typeof(List<T>        ))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T  >(this HBinaryReader reader, out T[]             values) { values = (T[]            )(_HReadArray     (reader, typeof(T[]            ))); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead<T,U>(this HBinaryReader reader, out Dictionary<T,U> dict  ) { dict   = (Dictionary<T,U>)(_HReadDictionary(reader, typeof(Dictionary<T,U>))); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] static object _HReadDouble(HBinaryReader reader) { return reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static object _HReadInt   (HBinaryReader reader) { return reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static object _HReadString(HBinaryReader reader) { return reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] static object _HReadBool  (HBinaryReader reader) { return reader.ReadBoolean(); }
        static object _HReadBinarySerializable(HBinaryReader reader)
        {
            string type_name = reader.ReadString();
            Type   type = Type.GetType(type_name);
            HDebug.Assert(type != null);
            object obj = Activator.CreateInstance(type, reader); // create object using reader by calling constructor Class(HBinaryReader reader)
            if((obj is IBinarySerializable) == false)
                throw new Exception("type-mismatch in HRead<T>(reader,out T value)\nAssign "+type.Name+" type into "+typeof(IBinarySerializable).Name+" type.");
            //((IBinarySerializable)obj).Deserialize(reader);
            return obj;
        }
        static object _HReadList(HBinaryReader reader, Type type)
        {
            IList values = (IList)Activator.CreateInstance(type);
            int leng = reader.ReadInt32();
            Type typeT = type.GenericTypeArguments[0];
            for(int i=0; i<leng; i++)
            {
                object value = _HRead(reader, typeT);
                values.Add(value);
            }
            return values;
        }
        static object _HReadArray(HBinaryReader reader, Type type)
        {
            HDebug.ToDo("check");
            Type typeT = type.GenericTypeArguments[0];
            int leng = reader.ReadInt32();
            Array values = Array.CreateInstance(typeT, leng);
            for(int i=0; i<leng; i++)
            {
                object value = _HRead(reader, typeT);
                values.SetValue(value, i);
            }
            return values;
        }
        static object _HReadDictionary(HBinaryReader reader, Type type)
        {
            IDictionary dict = (IDictionary)Activator.CreateInstance(type);
            int leng = reader.ReadInt32();
            Type typeT = type.GenericTypeArguments[0];
            Type typeU = type.GenericTypeArguments[1];
            for(int i=0; i<leng; i++)
            {
                object key = _HRead(reader, typeT);
                object val = _HRead(reader, typeU);
                dict.Add(key, val);
            }
            return dict;
        }
        static void _HRead<T>(HBinaryReader reader, out T value)
        {
            value = (T)_HRead(reader, typeof(T));
        }
        static object _HRead(HBinaryReader reader, Type type)
        {
            string type_name = type.FullName;
            if(typeof(IBinarySerializable).IsAssignableFrom(type)) return _HReadBinarySerializable(reader);
            if(type_name == typeof(double              ).FullName) return _HReadDouble            (reader);
            if(type_name == typeof(int                 ).FullName) return _HReadInt               (reader);
            if(type_name == typeof(string              ).FullName) return _HReadString            (reader);
            if(type_name == typeof(bool                ).FullName) return _HReadBool              (reader);
            if(typeof(IList              ).IsAssignableFrom(type)) return _HReadList              (reader, type);
            if(typeof(Array              ).IsAssignableFrom(type)) return _HReadArray             (reader, type);
            if(typeof(IDictionary        ).IsAssignableFrom(type)) return _HReadDictionary        (reader, type);
            throw new Exception();
        }
    }
}
