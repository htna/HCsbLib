using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
//using System.Runtime.Serialization;

namespace HTLib2
{
    /// Implement
    /// * void IBinarySerializable.Serialize(HBinaryWriter writer);
    /// * constructor of ctor(HBinaryReader reader);
    ///   which corresponds to 
    ///   1. creating an object and
    ///   2. calling obj.Deserialize(HBinaryReader reader);
    public interface IBinarySerializable
    {
        //IBinarySerializable(HBinaryReader reader);
        void Serialize(HBinaryWriter writer);
        //[Obsolete] void Deserialize(HBinaryReader reader);
    }
}
