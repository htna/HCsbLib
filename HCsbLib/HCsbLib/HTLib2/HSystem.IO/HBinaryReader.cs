using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    public partial struct HBinaryReader : IDisposable
    {
        public BinaryReader            reader;
        public Dictionary<long,object> id2obj;
        public HBinaryReader(Stream stream)
        {
            reader = new BinaryReader(stream);
            id2obj = new Dictionary<long, object>();
        }
        public HBinaryReader(string path)
        {
            Stream stream = HFile.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            reader = new BinaryReader(stream);
            id2obj = new Dictionary<long, object>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public void   AddIdObj(long objid, object obj) { id2obj.Add(objid, obj); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public object GetIdObj(long objid            ) { return id2obj[objid];   }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator BinaryReader(HBinaryReader breader) { return breader.reader; }
      //[MethodImpl(MethodImplOptions.AggressiveInlining)] public static implicit operator HBinaryReader(BinaryReader  reader) { return new HBinaryReader { reader = reader }; }

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
}
