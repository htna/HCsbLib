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

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out double value) { value = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out int    value) { value = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out string value) { value = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out bool   value) { value = reader.ReadBoolean(); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static int HReadLeng(this BinaryReader reader) { return reader.ReadInt32();  }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out double[] values) { int length = reader.ReadInt32(); values = new double[length]; for(int i=0; i<length; i++) values[i] = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out int   [] values) { int length = reader.ReadInt32(); values = new int   [length]; for(int i=0; i<length; i++) values[i] = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out string[] values) { int length = reader.ReadInt32(); values = new string[length]; for(int i=0; i<length; i++) values[i] = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out bool  [] values) { int length = reader.ReadInt32(); values = new bool  [length]; for(int i=0; i<length; i++) values[i] = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out List<double> values) { int length = reader.ReadInt32(); values = new List<double>(length); for(int i=0; i<length; i++) values.Add(reader.ReadDouble ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out List<int   > values) { int length = reader.ReadInt32(); values = new List<int   >(length); for(int i=0; i<length; i++) values.Add(reader.ReadInt32  ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out List<string> values) { int length = reader.ReadInt32(); values = new List<string>(length); for(int i=0; i<length; i++) values.Add(reader.ReadString ()); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out List<bool  > values) { int length = reader.ReadInt32(); values = new List<bool  >(length); for(int i=0; i<length; i++) values.Add(reader.ReadBoolean()); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T>(this BinaryWriter writer,     T value) where T : IBinarySerializable
        {
            string typeT = typeof(T).FullName;
            writer.Write(typeT);
            value.Serialize(writer);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead <T>(this BinaryReader reader, out T value) where T : IBinarySerializable
        {
            string typeT = reader.ReadString();
            Type   t = Type.GetType(typeT);
            object o = Activator.CreateInstance(t);
            if((o is T) == false)
                throw new HException("type-mismatch in HRead<T>(reader,out T value)\nAssign "+t.Name+" type into "+typeof(T).Name+" type.");
            value = (T)o;
            value.Deserialize(reader);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static T HRead <T>(this BinaryReader reader) where T : IBinarySerializable
        {
            T value;
            reader.HRead<T>(out value);
            return value;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T>(this BinaryWriter writer,     T[]     values) where T : IBinarySerializable { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.HWrite(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T>(this BinaryWriter writer,     List<T> values) where T : IBinarySerializable { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) writer.HWrite(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead <T>(this BinaryReader reader, out T[]     values) where T : IBinarySerializable { int length = reader.ReadInt32(); values = new T      [length]; for(int i=0; i<length; i++) values[i] =reader.HRead<T>() ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead <T>(this BinaryReader reader, out List<T> values) where T : IBinarySerializable { int length = reader.ReadInt32(); values = new List<T>(length); for(int i=0; i<length; i++) values.Add(reader.HRead<T>()); }

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
    }
}
