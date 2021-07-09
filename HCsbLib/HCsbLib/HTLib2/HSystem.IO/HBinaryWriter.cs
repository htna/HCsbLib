using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    public partial struct HBinaryWriter : IDisposable
    {
        public BinaryWriter      writer;
        public ObjectIDGenerator obj2id;
        public HBinaryWriter(Stream stream)
        {
            writer = new BinaryWriter(stream);
            obj2id = new ObjectIDGenerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public long GetObjId(object obj, out bool firstTime) { return obj2id.GetId(obj, out firstTime); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public long HasObjId(object obj, out bool firstTime) { return obj2id.HasId(obj, out firstTime); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator BinaryWriter(HBinaryWriter bwriter) { return bwriter.writer; }
      //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator HBinaryWriter(BinaryWriter  writer) { return new HBinaryWriter { writer = writer }; }

        public Stream BaseStream                         { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return writer.BaseStream                 ; } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Close()                                    {        writer.Close()                    ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Dispose()                                  {        writer.Dispose()                  ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Flush()                                    {        writer.Flush()                    ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public long Seek(int offset, SeekOrigin origin)        { return writer.Seek(offset, origin)       ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(char[] chars, int index, int count)  {        writer.Write(chars, index, count) ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(string value)                        {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(float value)                         {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(ulong value)                         {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(long value)                          {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(uint value)                          {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(int value)                           {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(ushort value)                        {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(short value)                         {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(byte[] buffer, int index, int count) {        writer.Write(buffer, index, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(double value)                        {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(char[] chars)                        {        writer.Write(chars)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(char ch)                             {        writer.Write(ch)                  ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(byte[] buffer)                       {        writer.Write(buffer)              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(sbyte value)                         {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(byte value)                          {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(bool value)                          {        writer.Write(value)               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void Write(decimal value)                       {        writer.Write(value)               ; }
        //protected virtual void Dispose(bool disposing);
        //protected void Write7BitEncodedInt(int value);
    }
}
