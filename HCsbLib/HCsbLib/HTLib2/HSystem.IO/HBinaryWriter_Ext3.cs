using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    public partial struct HBinaryWriter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(IBinarySerializeDeserialize data)
        {
            data.BinarySerialize(this);
        }
    }
}
