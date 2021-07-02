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
		public static implicit operator BinaryReader(HBinarySerializableReader breader)
		{
			return breader.reader;
		}
    }
    public struct HBinarySerializableWriter
    {
        public BinaryWriter writer;
		public static implicit operator BinaryWriter(HBinarySerializableWriter bwriter)
		{
			return bwriter.writer;
		}
    }
}
