using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Charmm
    {
        public partial class Tool
        {
            public static List<string> GenSolvCrd(IEnumerable<Tuple<Vector,Vector,Vector>> enumCoordOHH)
            {
                /// * WATER LAYER
                /// *  DATE:     9/23/ 4     11:59:41      CREATED BY USER: klauda
                /// *
                ///     139968  EXT
                ///          1         1  TIP3      OH2           -44.7800154321      -46.4680221193      -49.7340000000  BLK1      1               0.0000000000
                ///     ...
                ///     139968     46656  TIP3      H2             53.8779845679       54.8549778807       54.7330000000  BLK1      46656           0.0000000000
                /// 01234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
                /// 0         1         2         3         4         5         6         7         8         9         10        11        12        13        

                ////////////////          1         1  TIP3      OH2           -44.7800154321      -46.4680221193      -49.7340000000  BLK1      1               0.0000000000
                ////////////////     139968     46656  TIP3      H2             53.8779845679       54.8549778807       54.7330000000  BLK1      46656           0.0000000000
                string format = "    ______    ______  TIP3      ___      ________.__________ ________.__________ ________.__________  BLK1      _____           0.0000000000";
                //////////////// 012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890
                //////////////// 0         1         2         3         4         5         6         7         8         9         10        11        12        13        14

                List<string> lines = new List<string>();
                int resSeq = 0;
                foreach(var coordOHH in enumCoordOHH)
                {
                    resSeq++;
                    string resseq0 = string.Format("{0,6}", resSeq);
                    string resseq1 = string.Format("{0,-6}", resSeq);

                    string OH2_ser = string.Format("{0,6}", lines.Count + 1);
                    string OH2_x   = string.Format(" {0,19:0.0000000000}", coordOHH.Item1[0]);
                    string OH2_y   = string.Format(" {0,19:0.0000000000}", coordOHH.Item1[1]);
                    string OH2_z   = string.Format(" {0,19:0.0000000000}", coordOHH.Item1[2]);

                    string  H1_ser = string.Format("{0,6}", lines.Count + 2);
                    string  H1_x   = string.Format(" {0,19:0.0000000000}", coordOHH.Item2[0]);
                    string  H1_y   = string.Format(" {0,19:0.0000000000}", coordOHH.Item2[1]);
                    string  H1_z   = string.Format(" {0,19:0.0000000000}", coordOHH.Item2[2]);

                    string  H2_ser = string.Format("{0,6}", lines.Count + 3);
                    string  H2_x   = string.Format(" {0,19:0.0000000000}", coordOHH.Item3[0]);
                    string  H2_y   = string.Format(" {0,19:0.0000000000}", coordOHH.Item3[1]);
                    string  H2_z   = string.Format(" {0,19:0.0000000000}", coordOHH.Item3[2]);

                    string OH2_str = "    "+OH2_ser+"    "+resseq0+"  TIP3      OH2     "+OH2_x+OH2_y+OH2_z+"  BLK1      "+resseq1+"          0.0000000000";
                    string  H1_str = "    "+ H1_ser+"    "+resseq0+"  TIP3      H1      "+ H1_x+ H1_y+ H1_z+"  BLK1      "+resseq1+"          0.0000000000";
                    string  H2_str = "    "+ H2_ser+"    "+resseq0+"  TIP3      H2      "+ H2_x+ H2_y+ H2_z+"  BLK1      "+resseq1+"          0.0000000000";

                    // format  = "    ______    ______  TIP3      ___      ________.__________ ________.__________ ________.__________  BLK1      _____           0.0000000000"
                    // OH2_str = "         1         1  TIP3      OH2           -79.8000000000      -20.4000000000       -4.2000000000  BLK1          1           0.0000000000"
                    //  H1_str = "         2         1  TIP3      OH2           -79.8170000000      -20.3060000000       -3.2480000000  BLK1          1           0.0000000000"
                    //  H2_str = "         3         1  TIP3      OH2           -78.8810000000      -20.4560000000       -3.9370000000  BLK1          1           0.0000000000"
                    lines.Add(OH2_str);
                    lines.Add( H1_str);
                    lines.Add( H2_str);
                }

                //                 Chemistry at HARvard Macromolecular Mechanics
                //                (CHARMM) - Free Version 42b1     August 15, 2017
                //                                      652
                //       Copyright(c) 1984-2014  President and Fellows of Harvard College
                //                              All Rights Reserved
                //  Current operating system: CYGWIN_NT-10.0-2.9.0(0.318/5/3)(x86_64)@DESKTOP-I
                //                 Created on  9/16/17 at  1:50:52 by user: htna
                //
                //            Maximum number of ATOMS:    360720, and RESidues:      120240

                int numatoms = lines.Count;
                HDebug.Assert(numatoms < 360720); int diff_atm = 360720 - numatoms;
                HDebug.Assert(resSeq   < 120240); int diff_res = 120240 - resSeq  ;

                string line = string.Format("    {0,6}  EXT", numatoms);
                lines.Insert(0, line);  // "    ______  EXT"
                lines.Insert(0, "*");
                lines.Insert(0, "*  DATE:     0/00/00     00:00:00      CREATED BY USER: htna");
                lines.Insert(0, "* WATER LAYER");

                return lines;
            }
        }
    }
}
