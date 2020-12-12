using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
//using System.Runtime.Serialization;

namespace HTLib2
{
    public interface IBinarySerializable
    {
        void Serialize(BinaryWriter toStream);
        void Deserialize(BinaryReader fromStream);
    }
}
