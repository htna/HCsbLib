using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    public struct HBinaryReader : IDisposable
    {
        public BinaryReader reader;
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator BinaryReader(HBinaryReader breader) { return breader.reader; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator HBinaryReader(BinaryReader  reader) { return new HBinaryReader { reader = reader }; }

        public Stream BaseStream                           { [MethodImpl(MethodImplOptions.AggressiveInlining)] get { return reader.BaseStream         ; } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void    Close()                                   {        reader.Close()                   ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void    Dispose()                                 {        reader.Dispose()                 ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     PeekChar()                                { return reader.PeekChar()                ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     Read()                                    { return reader.Read()                    ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     Read(byte[] buffer, int index, int count) { return reader.Read(buffer, index, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     Read(char[] buffer, int index, int count) { return reader.Read(buffer, index, count); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public bool    ReadBoolean()                             { return reader.ReadBoolean()             ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte    ReadByte()                                { return reader.ReadByte()                ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public byte[]  ReadBytes(int count)                      { return reader.ReadBytes(count)          ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public char    ReadChar()                                { return reader.ReadChar()                ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public char[]  ReadChars(int count)                      { return reader.ReadChars(count)          ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public decimal ReadDecimal()                             { return reader.ReadDecimal()             ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public double  ReadDouble()                              { return reader.ReadDouble()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public short   ReadInt16()                               { return reader.ReadInt16()               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public int     ReadInt32()                               { return reader.ReadInt32()               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public long    ReadInt64()                               { return reader.ReadInt64()               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public sbyte   ReadSByte()                               { return reader.ReadSByte()               ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public float   ReadSingle()                              { return reader.ReadSingle()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public string  ReadString()                              { return reader.ReadString()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ushort  ReadUInt16()                              { return reader.ReadUInt16()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public uint    ReadUInt32()                              { return reader.ReadUInt32()              ; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public ulong   ReadUInt64()                              { return reader.ReadUInt64()              ; }
        //protected virtual void Dispose(bool disposing);
        //protected virtual void FillBuffer(int numBytes);
        //protected internal int Read7BitEncodedInt();
    }
    public struct HBinaryWriter : IDisposable
    {
        public BinaryWriter writer;

		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator BinaryWriter(HBinaryWriter bwriter) { return bwriter.writer; }
		[MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator HBinaryWriter(BinaryWriter  writer) { return new HBinaryWriter { writer = writer }; }

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
