using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    public struct HBinarySerializableReader
    {
        public BinaryReader reader;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator BinaryReader(HBinarySerializableReader breader) { return breader.reader; }

        public Stream BaseStream                           { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return reader.BaseStream         ; } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void    Close()                                   {        Close()                   ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void    Dispose()                                 {        Dispose()                 ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     PeekChar()                                { return PeekChar()                ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     Read()                                    { return Read()                    ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     Read(byte[] buffer, int index, int count) { return Read(buffer, index, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     Read(char[] buffer, int index, int count) { return Read(buffer, index, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool    ReadBoolean()                             { return ReadBoolean()             ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte    ReadByte()                                { return ReadByte()                ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte[]  ReadBytes(int count)                      { return ReadBytes(count)          ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public char    ReadChar()                                { return ReadChar()                ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public char[]  ReadChars(int count)                      { return ReadChars(count)          ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public decimal ReadDecimal()                             { return ReadDecimal()             ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public double  ReadDouble()                              { return ReadDouble()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public short   ReadInt16()                               { return ReadInt16()               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     ReadInt32()                               { return ReadInt32()               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public long    ReadInt64()                               { return ReadInt64()               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public sbyte   ReadSByte()                               { return ReadSByte()               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public float   ReadSingle()                              { return ReadSingle()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string  ReadString()                              { return ReadString()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ushort  ReadUInt16()                              { return ReadUInt16()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public uint    ReadUInt32()                              { return ReadUInt32()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ulong   ReadUInt64()                              { return ReadUInt64()              ; }
        //protected virtual void Dispose(bool disposing);
        //protected virtual void FillBuffer(int numBytes);
        //protected internal int Read7BitEncodedInt();
    }
    public struct HBinarySerializableWriter
    {
        public BinaryWriter writer;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator BinaryWriter(HBinarySerializableWriter bwriter) { return bwriter.writer; }

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
