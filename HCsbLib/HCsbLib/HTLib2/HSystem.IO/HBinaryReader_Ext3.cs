﻿using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
using System.Runtime.CompilerServices;
using System.IO;

namespace HTLib2
{
    public partial struct HBinaryReader
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Read(IBinarySerializeDeserialize data)
        {
            data.BinaryDeserialize(this);
        }
    }
}
