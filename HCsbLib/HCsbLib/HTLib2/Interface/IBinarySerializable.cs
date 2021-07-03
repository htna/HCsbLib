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
        //IBinarySerializable(HBinaryReader reader);
        void Serialize(HBinaryWriter writer);
        [Obsolete] void Deserialize(HBinaryReader reader);
    }
}
