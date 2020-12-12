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

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T>(this BinaryWriter writer,     T value) where T : IBinarySerializable
        {
            string typeT = typeof(T).FullName;
            writer.Write(typeT);
            value.Serialize(writer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T>(this BinaryWriter writer,     T[]     values) where T : IBinarySerializable { writer.Write(values.Length); for(int i=0; i<values.Length; i++) writer.HWrite(values[i]); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HWrite<T>(this BinaryWriter writer,     List<T> values) where T : IBinarySerializable { writer.Write(values.Count ); for(int i=0; i<values.Count ; i++) writer.HWrite(values[i]); }
    }
}
