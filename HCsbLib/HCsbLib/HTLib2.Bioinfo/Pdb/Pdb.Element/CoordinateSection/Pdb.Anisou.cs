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
        public class Anisou : Element, IComparable<Anisou>, IBinarySerializable
		{
			/// http://www.wwpdb.org/documentation/format32/sect9.html#ANISOU
			///
            /// ANISOU
            /// 
            /// The ANISOU records present the anisotropic temperature factors.
            /// 
            ///  COLUMNS       DATA  TYPE    FIELD          DEFINITION
            /// -----------------------------------------------------------------
            ///  1 - 6        Record name   "ANISOU"
            ///  7 - 11       Integer       serial         Atom serial number.
            /// 13 - 16       Atom          name           Atom name.
            /// 17            Character     altLoc         Alternate location indicator
            /// 18 - 20       Residue name  resName        Residue name.
            /// 22            Character     chainID        Chain identifier.
            /// 23 - 26       Integer       resSeq         Residue sequence number.
            /// 27            AChar         iCode          Insertion code.
            /// 29 - 35       Integer       u[0][0]        U(1,1)
            /// 36 - 42       Integer       u[1][1]        U(2,2)
            /// 43 - 49       Integer       u[2][2]        U(3,3)
            /// 50 - 56       Integer       u[0][1]        U(1,2)
            /// 57 - 63       Integer       u[0][2]        U(1,3)
            /// 64 - 70       Integer       u[1][2]        U(2,3)
            /// 77 - 78       LString(2)    element        Element symbol, right-justified.
            /// 79 - 80       LString(2)    charge         Charge on the atom.
            ///  
            /// Example
            ///           1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// ATOM    107  N   GLY A  13      12.681  37.302 -25.211 1.000 15.56           N
            /// ANISOU  107  N   GLY A  13     2406   1892   1614    198    519   -328       N
            /// ATOM    108  CA  GLY A  13      11.982  37.996 -26.241 1.000 16.92           C
            /// ANISOU  108  CA  GLY A  13     2748   2004   1679    -21    155   -419       C
            /// ATOM    109  C   GLY A  13      11.678  39.447 -26.008 1.000 15.73           C
            /// ANISOU  109  C   GLY A  13     2555   1955   1468     87    357   -109       C
            /// ATOM    110  O   GLY A  13      11.444  40.201 -26.971 1.000 20.93           O
            /// ANISOU  110  O   GLY A  13     3837   2505   1611    164   -121    189       O
            /// ATOM    111  N   ASN A  14      11.608  39.863 -24.755 1.000 13.68           N
            /// ANISOU  111  N   ASN A  14     2059   1674   1462     27    244    -96       N

			public static string RecordName { get { return "ANISOU"; } }

            Anisou(string line)
                : base(line)
			{
			}
			public static Anisou FromString(string line)
			{
				HDebug.Assert(IsAnisou(line));
				return new Anisou(line);
			}
            public static Anisou FromAtom(Atom atom, int[,] U)
            {
                string line = atom.line;
                line = line.Replace("ATOM  ", "ANISOU");
                line = GetUpdatedU(line, U);
                return FromString(line);
            }
            public static Anisou FromAtom(Atom atom, int U012)
            {
                int U00 = U012;
                int U11 = U012;
                int U22 = U012;
                int[,] U = new int[3, 3] { { U00, 0, 0 }, { 0, U11, 0 }, { 0, 0, U22 } };
                return FromAtom(atom, U);
            }
            public static Anisou FromData
                ( string line
                , int? serial=null, string name=null, char? altLoc=null, string resName=null, char? chainID=null, int? resSeq=null, char? iCode=null
                , int[] u11_u23 = null
                , string element=null, string charge=null
                )
            {
                string nline = line;
                if(nline == null)
                  //nline = "ANISOU  107  N   GLY A  13     2406   1892   1614    198    519   -328       N  ";
                    nline = "ANISOU__________________________________________________________________________";

                if(serial  != null) nline = UpdateLineInteger(nline, serial .Value, idxs_serial );
                if(name    != null) nline = UpdateLineString (nline, name         , idxs_name   );
                if(altLoc  != null) nline = UpdateLineChar   (nline, altLoc .Value, idxs_altLoc );
                if(resName != null) nline = UpdateLineString (nline, resName      , idxs_resName);
                if(chainID != null) nline = UpdateLineChar   (nline, chainID.Value, idxs_chainID);
                if(resSeq  != null) nline = UpdateLineInteger(nline, resSeq .Value, idxs_resSeq );
                if(iCode   != null) nline = UpdateLineChar   (nline, iCode  .Value, idxs_iCode  );
                if(element != null) nline = UpdateLineString (nline, element      , idxs_element);
                if(charge  != null) nline = UpdateLineString (nline, charge       , idxs_charge );
                if(u11_u23 != null)
                {
                    int u11 = u11_u23[0]; nline = UpdateLineInteger(nline, u11, idxs_u11);
                    int u22 = u11_u23[1]; nline = UpdateLineInteger(nline, u22, idxs_u22);
                    int u33 = u11_u23[2]; nline = UpdateLineInteger(nline, u33, idxs_u33);
                    int u12 = u11_u23[3]; nline = UpdateLineInteger(nline, u12, idxs_u12);
                    int u13 = u11_u23[4]; nline = UpdateLineInteger(nline, u13, idxs_u13);
                    int u23 = u11_u23[5]; nline = UpdateLineInteger(nline, u23, idxs_u23);
                }

                if(line.Contains('_'))
                    throw new ArgumentException("still contains '_' in new PDB anisou line");

                return Anisou.FromString(nline);
            }
            public static bool IsAnisou(string line) { return (line.Substring(0, 6) == "ANISOU"); }
            public    int serial    { get { return Integer(idxs_serial    ).Value; } } static readonly int[] idxs_serial     = new int[]{ 7,11}; //  7 - 11       Integer       serial         Atom serial number.
            public string name      { get { return String (idxs_name      );       } } static readonly int[] idxs_name       = new int[]{13,16}; // 13 - 16       Atom          name           Atom name.
            public   char altLoc    { get { return Char   (idxs_altLoc    );       } } static readonly int[] idxs_altLoc     = new int[]{17,17}; // 17            Character     altLoc         Alternate location indicator
            public string resName   { get { return String (idxs_resName   );       } } static readonly int[] idxs_resName    = new int[]{18,20}; // 18 - 20       Residue name  resName        Residue name.
            public   char chainID   { get { return Char   (idxs_chainID   );       } } static readonly int[] idxs_chainID    = new int[]{22,22}; // 22            Character     chainID        Chain identifier.
            public    int resSeq    { get { return Integer(idxs_resSeq    ).Value; } } static readonly int[] idxs_resSeq     = new int[]{23,26}; // 23 - 26       Integer       resSeq         Residue sequence number.
            public   char iCode     { get { return Char   (idxs_iCode     );       } } static readonly int[] idxs_iCode      = new int[]{27,27}; // 27            AChar         iCode          Insertion code.
            public    int u11       { get { return Integer(idxs_u11       ).Value; } } static readonly int[] idxs_u11        = new int[]{29,35}; // 29 - 35       Integer       u[0][0]        U(1,1)
            public    int u22       { get { return Integer(idxs_u22       ).Value; } } static readonly int[] idxs_u22        = new int[]{36,42}; // 36 - 42       Integer       u[1][1]        U(2,2)
            public    int u33       { get { return Integer(idxs_u33       ).Value; } } static readonly int[] idxs_u33        = new int[]{43,49}; // 43 - 49       Integer       u[2][2]        U(3,3)
            public    int u12       { get { return Integer(idxs_u12       ).Value; } } static readonly int[] idxs_u12        = new int[]{50,56}; // 50 - 56       Integer       u[0][1]        U(1,2)
            public    int u13       { get { return Integer(idxs_u13       ).Value; } } static readonly int[] idxs_u13        = new int[]{57,63}; // 57 - 63       Integer       u[0][2]        U(1,3)
            public    int u23       { get { return Integer(idxs_u23       ).Value; } } static readonly int[] idxs_u23        = new int[]{64,70}; // 64 - 70       Integer       u[1][2]        U(2,3)
            public string element   { get { return String (idxs_element   );       } } static readonly int[] idxs_element    = new int[]{77,78}; // 77 - 78       LString(2)    element        Element symbol, right-justified.
            public string charge    { get { return String (idxs_charge    );       } } static readonly int[] idxs_charge     = new int[]{79,80}; // 79 - 80       LString(2)    charge         Charge on the atom.

            public double[,] U       { get { return new double[3,3]{{u11,u12,u13}
                                                                   ,{u12,u22,u23}
                                                                   ,{u13,u23,u33}}; } }
            public bool IsHydrogen() { return ((name[0] == 'H') || (name[0] == 'h')); }

			// IComparable<Anisou>
			int IComparable<Anisou>.CompareTo(Anisou other)
			{
				return serial.CompareTo(other.serial);
			}

            public string GetUpdatedU(int[,] U)
            {
                return GetUpdatedU(this.line, U);
            }
            public static string GetUpdatedU(string this_line, int[,] U)
            {
                string stru11 = string.Format("{0}       ", U[1-1, 1-1]);
                string stru22 = string.Format("{0}       ", U[2-1, 2-1]);
                string stru33 = string.Format("{0}       ", U[3-1, 3-1]);
                string stru12 = string.Format("{0}       ", U[1-1, 2-1]);
                string stru13 = string.Format("{0}       ", U[1-1, 3-1]);
                string stru23 = string.Format("{0}       ", U[2-1, 3-1]);
                char[] line = this_line.ToArray();
                for(int i=0; i<(idxs_u11[1]-idxs_u11[0]+1); i++) line[i+idxs_u11[0]] = stru11[i];
                for(int i=0; i<(idxs_u22[1]-idxs_u22[0]+1); i++) line[i+idxs_u22[0]] = stru22[i];
                for(int i=0; i<(idxs_u33[1]-idxs_u33[0]+1); i++) line[i+idxs_u33[0]] = stru33[i];
                for(int i=0; i<(idxs_u12[1]-idxs_u12[0]+1); i++) line[i+idxs_u12[0]] = stru12[i];
                for(int i=0; i<(idxs_u13[1]-idxs_u13[0]+1); i++) line[i+idxs_u13[0]] = stru13[i];
                for(int i=0; i<(idxs_u23[1]-idxs_u23[0]+1); i++) line[i+idxs_u23[0]] = stru23[i];
                string result = new string(line);
                return result;
            }

            public override string ToString()
            {
                string str  = string.Format("anisou {0:D} {1}, {2:d} {3}", serial, name.Trim(), resSeq, resName.Trim());
                       //str += string.Format(", pos({0:G4},{1:G4},{2:G4})", x, y, z);
                       //str += string.Format(", alt({0}), chain({1})", altLoc, chainID);
                return str;
            }

            ////////////////////////////////////////////////////////////////////////////////////
            // IBinarySerializable
            public new void BinarySerialize(HBinaryWriter writer)
            {
            }
            public Anisou(HBinaryReader reader) : base(reader)
            {
            }
            // IBinarySerializable
		    ////////////////////////////////////////////////////////////////////////////////////
		    // Serializable
            public Anisou(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }


			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
	}
}
