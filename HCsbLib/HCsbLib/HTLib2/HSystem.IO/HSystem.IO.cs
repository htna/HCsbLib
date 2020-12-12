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

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out double value) { value = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out int    value) { value = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out string value) { value = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out bool   value) { value = reader.ReadBoolean(); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out double[] values) { int length = reader.ReadInt32(); values = new double[length]; for(int i=0; i<length; i++) values[i] = reader.ReadDouble (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out int   [] values) { int length = reader.ReadInt32(); values = new int   [length]; for(int i=0; i<length; i++) values[i] = reader.ReadInt32  (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out string[] values) { int length = reader.ReadInt32(); values = new string[length]; for(int i=0; i<length; i++) values[i] = reader.ReadString (); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static void HRead (this BinaryReader reader, out bool  [] values) { int length = reader.ReadInt32(); values = new bool  [length]; for(int i=0; i<length; i++) values[i] = reader.ReadBoolean(); }
    }
}
