using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
public partial class Namd
{
    public partial class Psf
    {
        [Serializable]
        public class Atom
        {
            //    1231 !NATOM
            //       1 U    1    MET  N    NH3   -0.300000       14.0070           0
            //       2 U    1    MET  HT1  HC     0.330000        1.0080           0
            //        15 GPUAA    5        URA      H4'      HN7      0.090000        1.0080           0
            //      3822 GPUA     129      URA      O3'      ON2     -0.570000       15.9994           0
            //      3823 GPUA     129A     GUA      P        P        1.500000       30.9740           0
            //     32441 GPUA     1028A    CYT      P        P        1.500000       30.9740           0
            //       ....
            //   atom ID, segment name, residue ID, residue name, atom name, atom type, charge, mass, and an unused 0
            readonly string line;
            public int    AtomId     ;
            public string SegmentName;
            public Tuple<int,char> ResidueId  ; // residue ID is the combination of (residue ID + insert code)
            public string ResidueName;
            public string AtomName   ;
            public string AtomType   ;
            public double Charge     ;
            public double Mass       ;
            Atom(string line)
            {
                this.line = line;
            }
            public static Atom FromLine(string line)
            {
                ///0123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
                ///0         1         2         3         4         5         6         7         8         9
                ///==========================================================================================
                ///       0-9 11-18 19-22 23    29-31    38-46    47-50  54-63      65-77                   89
                ///==========================================================================================
                ///        15 GPUAA    5        URA      H4'      HN7      0.090000        1.0080           0
                ///         1 GPUA     5        URA      N        NH3     -0.300000       14.0070           0
                ///       472 GPUA     19       CYT      C5       CN3     -0.130000       12.0110           0
                ///      3822 GPUA     129      URA      O3'      ON2     -0.570000       15.9994           0
                ///      3823 GPUA     129A     GUA      P        P        1.500000       30.9740           0
                ///     10152 GPUA     324      GUA      O1P      ON3     -0.780000       15.9994           0
                ///    103784 GQUA     209      CYT      O2P      ON3     -0.780000       15.9994           0
                ///    103786 GQUA     209      CYT      C5'      CN8B    -0.080000       12.0110           0
                ///     32441 GPUA     1028A    CYT      P        P        1.500000       30.9740           0

                string[] tokens = line.Split(separator, StringSplitOptions.RemoveEmptyEntries);
                Atom atom = new Atom(line);
                atom.AtomId      = int.Parse(tokens[0]);
                atom.SegmentName =           tokens[1] ;
                {
                    int resi;
                    if(int.TryParse(tokens[2], out resi))
                    {
                        atom.ResidueId = new Tuple<int,char>(int.Parse(tokens[2]), ' ');
                    }
                    else
                    {
                        int    leng  = tokens[2].Length;
                        string sresi = tokens[2].Substring(0, leng-1);
                        char   iCode = tokens[2][leng-1];
                        atom.ResidueId = new Tuple<int, char>(int.Parse(sresi), iCode);
                    }
                }
                atom.ResidueName =           tokens[3] ;
                atom.AtomName    =           tokens[4] ;
                atom.AtomType    =           tokens[5] ;
                atom.Charge      = double.Parse(tokens[6]);
                atom.Mass        = double.Parse(tokens[7]);
                return atom;
            }
            public override string ToString()
            {
                string str = SegmentName + " : " + AtomId + " " + AtomName + " : " + ResidueId + " " + ResidueName;
                return str;
            }
            ////////////////////////////////////////////////////////////////////////////////////
            // Serializable
            public Atom(SerializationInfo info, StreamingContext ctxt)
            {
                AtomId      = (int   )info.GetValue("AtomId"     , typeof(int   ));
                SegmentName = (string)info.GetValue("SegmentName", typeof(string));
                var resid_1 = (int   )info.GetValue("ResidueId_1", typeof(int   ));
                var resid_2 = (char  )info.GetValue("ResidueId_2", typeof(char  ));
                ResidueId   = new Tuple<int, char>(resid_1, resid_2);
                ResidueName = (string)info.GetValue("ResidueName", typeof(string));
                AtomName    = (string)info.GetValue("AtomName"   , typeof(string));
                AtomType    = (string)info.GetValue("AtomType"   , typeof(string));
                Charge      = (double)info.GetValue("Charge"     , typeof(double));
                Mass        = (double)info.GetValue("Mass"       , typeof(double));
            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("AtomId"     , AtomId     );
                info.AddValue("SegmentName", SegmentName);
                info.AddValue("ResidueId_1", ResidueId.Item1);
                info.AddValue("ResidueId_2", ResidueId.Item2);
                info.AddValue("ResidueName", ResidueName);
                info.AddValue("AtomName"   , AtomName   );
                info.AddValue("AtomType"   , AtomType   );
                info.AddValue("Charge"     , Charge     );
                info.AddValue("Mass"       , Mass       );
            }
        }
    }
}
}
