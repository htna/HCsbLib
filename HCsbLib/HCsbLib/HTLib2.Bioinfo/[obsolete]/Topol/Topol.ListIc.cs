/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Topol
	{
		public class ListIc
		{
			/// Finally in the residue definition are the internal coordinate IC statements. For each
			/// set of four atoms 1 2 3 4, the IC specifies in order the bond length 1-2, the angle 1-2-3,
			/// the dihedral 1-2-3-4, the angle 2-3-4, and the bond length 3-4. With this set of data,
			/// the position of atom 1 may be determined based on the positions of atoms 2-4, and the position
			/// of atom 4 may be determined from the positions of atoms 1-3, allowing the recursive generation
			/// of coordinates for all atoms in the structure based on a three-atom seed. Improper IC statements
			/// are indicated by a * preceding the third atom, the atom to which the other three are bonded,
			/// as in 1 2 *3 4. The order of atoms in an IC statement is different from that of an IMPR statement,
			/// and values provide the length 1-3, the angle 1-3-2, the dihedral 1-2-3-4, the angle 2-3-4, and
			/// the length 3-4.
			///
			/// IC -C   CA   *N   HN    1.3551 126.4900  180.0000 115.4200  0.9996
			/// IC -C   N    CA   C     1.3551 126.4900  180.0000 114.4400  1.5390
			/// IC N    CA   C    +N    1.4592 114.4400  180.0000 116.8400  1.3558
			/// IC +N   CA   *C   O     1.3558 116.8400  180.0000 122.5200  1.2297
			/// IC CA   C    +N   +CA   1.5390 116.8400  180.0000 126.7700  1.4613
			/// IC N    C    *CA  CB    1.4592 114.4400  123.2300 111.0900  1.5461
			/// IC N    C    *CA  HA    1.4592 114.4400 -120.4500 106.3900  1.0840
			/// IC C    CA   CB   HB1   1.5390 111.0900  177.2500 109.6000  1.1109
			/// IC HB1  CA   *CB  HB2   1.1109 109.6000  119.1300 111.0500  1.1119
			/// IC HB1  CA   *CB  HB3   1.1109 109.6000 -119.5800 111.6100  1.1114

			readonly List<string> _lines;

			// bond length
			readonly List<double> LengthValue = new List<double>();
			readonly List<string> LengthType1 = new List<string>();
			readonly List<string> LengthType2 = new List<string>();
			readonly Dictionary<string,double> Length = new Dictionary<string, double>();
			// bond angle
			readonly List<double> AngleValue = new List<double>();
			readonly List<string> AngleType1 = new List<string>();
			readonly List<string> AngleType2 = new List<string>();
			readonly List<string> AngleType3 = new List<string>();
			readonly Dictionary<string,double> Angle = new Dictionary<string, double>();
			// dihedral
			readonly List<double> DihedralValue = new List<double>();
			readonly List<string> DihedralType1 = new List<string>();
			readonly List<string> DihedralType2 = new List<string>();
			readonly List<string> DihedralType3 = new List<string>();
			readonly List<string> DihedralType4 = new List<string>();
			readonly List<bool>   DihedralImproper = new List<bool>();
			readonly Dictionary<string,double> Dihedral = new Dictionary<string, double>();
			
			ListIc(List<string> lines)
			{
				this._lines = lines;

				foreach(string line in lines)
				{
					string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					HDebug.Assert(tokens[0] == "IC");
					HDebug.Assert(tokens.Length == 10);
					// bond length
					string atm1 = GetTypeClear(tokens[1]);
					string atm2 = GetTypeClear(tokens[2]);
					string atm3 = GetTypeClear(tokens[3]);
					string atm4 = GetTypeClear(tokens[4]);
					double val1 = double.Parse(tokens[5]); UpdateLengthFromToken  (val1, atm1, atm2);
					double val2 = double.Parse(tokens[6]); UpdateAngleFromToken   (val2, atm1, atm2, atm3);
					double val3 = double.Parse(tokens[7]); UpdateDihedralFromToken(val3, atm1, atm2, atm3, atm4, tokens[3][0] == '*');
					double val4 = double.Parse(tokens[8]); UpdateAngleFromToken   (val4, atm2, atm3, atm4);
					double val5 = double.Parse(tokens[9]); UpdateLengthFromToken  (val5, atm3, atm4);
				}
			}
			public static ListIc FromLines(List<string> lines)
			{
				return new ListIc(lines);
			}
			public string GetTypeClear(string atom_type)
			{
				if(atom_type[0] != '*')
					return atom_type;
				return atom_type.Substring(1);
			}
			public void UpdateLengthFromToken(double val, string atm1, string atm2)
			{
				LengthValue.Add(val);
				LengthType1.Add(atm1);
				LengthType2.Add(atm2);
				Length.Add(GetKeyForLength(atm1, atm2), val);
			}
			public void UpdateAngleFromToken(double val, string atm1, string atm2, string atm3)
			{
				AngleValue.Add(val);
				AngleType1.Add(atm1);
				AngleType2.Add(atm2);
				AngleType3.Add(atm3);
				Angle.Add(GetKeyForAngle(atm1, atm2, atm3), val);
			}
			public void UpdateDihedralFromToken(double val, string atm1, string atm2, string atm3, string atm4, bool improper)
			{
				DihedralValue.Add(val);
				DihedralType1.Add(atm1);
				DihedralType2.Add(atm2);
				DihedralType3.Add(atm3);
				DihedralType4.Add(atm4);
				DihedralImproper.Add(improper);
				Dihedral.Add(GetKeyForDihedral(atm1, atm2, atm3, atm4), val);
			}
			public string GetKeyForLength(string atm1, string atm2)
			{
				if(string.Compare(atm1, atm2, true) < 0)
					return atm1+","+atm2;
				return atm2+","+atm1;
			}
			public double GetBondLength(string atom1_type, string atom2_type)
			{
				string key = GetKeyForLength(atom1_type, atom2_type);
				if(Length.ContainsKey(key) == false)
					return double.NaN;
				return Length[key];
			}
			public string GetKeyForAngle(string atm1, string atm2, string atm3)
			{
				if(string.Compare(atm1, atm3, true) < 0)
					return atm1+","+atm2+","+atm3;
				return atm3+","+atm2+","+atm1;
			}
			public double GetBondAngle(string atom1_type, string atom2_type, string atom3_type)
			{
				string key = GetKeyForAngle(atom1_type, atom2_type, atom3_type);
				if(Angle.ContainsKey(key) == false)
					return double.NaN;
				return Angle[key];
			}
			public string GetKeyForDihedral(string atm1, string atm2, string atm3, string atm4)
			{
				if(string.Compare(atm1, atm4, true) < 0)
					return atm1+","+atm2+","+atm3+","+atm4;
				return atm4+","+atm3+","+atm2+","+atm1;
			}
		}
	}
}
*/