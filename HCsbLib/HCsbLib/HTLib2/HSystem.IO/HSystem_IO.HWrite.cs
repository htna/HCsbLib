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
        //      public void Deserialize(System.IO.BinaryWriter writer)
        //      {
        //          writer.HRead(out value );
        //          writer.HRead(out values);
        //      }
        //  }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, double value) { writer.Write(value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, int    value) { writer.Write(value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, string value) { writer.Write(value); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, bool   value) { writer.Write(value); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, double[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, int   [] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, string[] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, bool  [] values) { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, List<double> values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, List<int   > values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, List<string> values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite(this BinaryWriter writer, List<bool  > values) { writer.Write(values.Count); for(int i=0; i<values.Count; i++) writer.Write(values[i]); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T  >(this BinaryWriter writer, T               value ) { _HWrite          <T  >(writer, value ); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T  >(this BinaryWriter writer, List<T>         values) { _HWriteList      <T  >(writer, values); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T  >(this BinaryWriter writer, T[]             values) { _HWriteArray     <T  >(writer, values); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T,U>(this BinaryWriter writer, Dictionary<T,U> dict  ) { _HWriteDictionary<T,U>(writer, dict  ); }

        static void _HWriteDouble(BinaryWriter writer, object value) { writer.Write((double)value); }
        static void _HWriteInt   (BinaryWriter writer, object value) { writer.Write((int   )value); }
        static void _HWriteString(BinaryWriter writer, object value) { writer.Write((string)value); }
        static void _HWriteBool  (BinaryWriter writer, object value) { writer.Write((bool  )value); }
        static void _HWriteBinarySerializable<T>(BinaryWriter writer, object value)
        {
            if((value is IBinarySerializable) == false)
                throw new HException();
            string typeT = typeof(T).FullName;
            writer.Write(typeT);
            ((IBinarySerializable)value).Serialize(writer);
        }
        static void _HWriteList<T>(BinaryWriter writer, object value)
        {
            List<T> values = (List<T>)value;
            writer.Write(values.Count);
            for(int i=0; i<values.Count; i++)
                _HWrite(writer, values[i]);
        }
        static void _HWriteArray<T>(BinaryWriter writer, object value)
        {
            T[] values = (T[])value;
            writer.Write(values.Length);
            for(int i=0; i<values.Length; i++)
                _HWrite(writer, values[i]);
        }
        static void _HWriteDictionary<T,U>(BinaryWriter writer, object value)
        {
            Dictionary<T,U> dict = (Dictionary<T,U>)value;
            writer.Write(dict.Count);
            foreach(var key_val in dict)
            {
                _HWrite(writer, key_val.Key  );
                _HWrite(writer, key_val.Value);
            }
        }
        static void _HWrite<T>(BinaryWriter writer, T value)
        {
            Type type = typeof(T);
            if(value is IBinarySerializable) { _HWriteBinarySerializable<T>(writer, value); return; }
            string type_name = type.FullName;
            if(type_name == typeof(double ).FullName) { _HWrite        (writer, value); return; }
            if(type_name == typeof(int    ).FullName) { _HWrite        (writer, value); return; }
            if(type_name == typeof(string ).FullName) { _HWrite        (writer, value); return; }
            if(type_name == typeof(bool   ).FullName) { _HWrite        (writer, value); return; }
            if(type_name == typeof(List<T>).FullName) { _HWriteList <T>(writer, value); return; }
            if(type_name == typeof(    T[]).FullName) { _HWriteArray<T>(writer, value); return; }
            throw new Exception();
        }
    }
}
