using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        [Serializable]
        public class Hetatm : IAtom, IBinarySerializable
		{
            /// http://www.wwpdb.org/documentation/format32/sect9.html#HETATM
            /// 
            /// COLUMNS       DATA  TYPE     FIELD         DEFINITION
            /// -----------------------------------------------------------------------
            ///  1 - 6        Record name    "HETATM"
            ///  7 - 11       Integer        serial        Atom serial number.
            /// 13 - 16       Atom           name          Atom name.
            /// 17            Character      altLoc        Alternate location indicator.
            /// 18 - 20       Residue name   resName       Residue name.
            /// 22            Character      chainID       Chain identifier.
            /// 23 - 26       Integer        resSeq        Residue sequence number.
            /// 27            AChar          iCode         Code for insertion of residues.
            /// 31 - 38       Real(8.3)      x             Orthogonal coordinates for X.
            /// 39 - 46       Real(8.3)      y             Orthogonal coordinates for Y.
            /// 47 - 54       Real(8.3)      z             Orthogonal coordinates for Z.
            /// 55 - 60       Real(6.2)      occupancy     Occupancy.
            /// 61 - 66       Real(6.2)      tempFactor    Temperature factor.
            /// 77 - 78       LString(2)     element       Element symbol; right-justified.
            /// 79 - 80       LString(2)     charge        Charge on the atom.
            /// 
            /// Example
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// HETATM 8237 MG    MG A1001      13.872  -2.555 -29.045  1.00 27.36          MG 
            ///  
            /// HETATM 3835 FE   HEM A   1      17.140   3.115  15.066  1.00 14.14          FE
            /// HETATM 8238  S   SO4 A2001      10.885 -15.746 -14.404  1.00 47.84           S  
            /// HETATM 8239  O1  SO4 A2001      11.191 -14.833 -15.531  1.00 50.12           O  
            /// HETATM 8240  O2  SO4 A2001       9.576 -16.338 -14.706  1.00 48.55           O  
            /// HETATM 8241  O3  SO4 A2001      11.995 -16.703 -14.431  1.00 49.88           O  
            /// HETATM 8242  O4  SO4 A2001      10.932 -15.073 -13.100  1.00 49.91           O  



			public static string RecordName { get { return "HETATM"; } }

            Hetatm(string line)
                : base(line)
			{
			}
			public static Hetatm FromString(string line)
			{
				HDebug.Assert(IsHetatm(line));
				return new Hetatm(line);
			}
            public static Hetatm FromData(int serial, string name, string resName, char chainID, int resSeq, double x, double y, double z, char? altLoc=null, char? iCode=null, double? occupancy=null, double? tempFactor=null, string element=null, string charge=null, string segment=null)
            {
                Hetatm hetatm = Hetatm.FromString(LineFromData(RecordName, serial, name, resName, chainID, resSeq, x, y, z
                    , altLoc    : altLoc
                    , iCode     : iCode
                    , occupancy : occupancy
                    , tempFactor: tempFactor
                    , element   : element
                    , charge    : charge
                    , segment   : segment
                    ));
                return hetatm;
            }
            public static bool IsHetatm(string line) { return (line.Substring(0, 6) == "HETATM"); }

            public override string ToString()
            {
                return base.ToString("hetatm");
            }

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Hetatm(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Hetatm(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) {}
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
		}
	}
}
