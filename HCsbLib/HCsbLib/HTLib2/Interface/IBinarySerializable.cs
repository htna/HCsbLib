using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
//using System.Runtime.Serialization;

namespace HTLib2
{
    // Implement
    // * void IBinarySerializable.Serialize(HBinaryWriter writer);
    // * constructor of ctor(HBinaryReader reader);
    //   which corresponds to 
    //   1. creating an object and
    //   2. calling obj.Deserialize(HBinaryReader reader);
    //   
    // Example:
    //      class Data : IBinarySerializable
    //      {
    //          int value;
    //          ///////////////////////////////////////////////////
    //          // IBinarySerializable
    //          public void BinarySerialize(HBinaryWriter writer)
    //          {
    //              writer.HWrite(value);
    //          }
    //          public Data(HBinaryReader reader)
    //          {
    //              reader.HRead(out value);
    //          }
    //      }

    public interface IBinarySerializable
    {
        //IBinarySerializable(HBinaryReader reader);
        void BinarySerialize(HBinaryWriter writer);
        //[Obsolete] void BinaryDeserialize(HBinaryReader reader);
    }
}
