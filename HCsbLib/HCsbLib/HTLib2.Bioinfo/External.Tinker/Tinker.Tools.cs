using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public partial class Tools
        {
            public static List<string> Convert_PdbWaterBox_To_XyzWaterBox
                ( string path_solvbox_pdb //= @"K:\Temp\solvate\solvbox100.pdb";
                )
            {
                Pdb pdb = Pdb.FromFile(path_solvbox_pdb);
                var atoms = pdb.atoms;
                HDebug.Assert(atoms.Length == 783_198);
                string format_id    = " {0,6}";
                string format_coord = " {0,11:0.000000}";

                List<string> lines = new List<string>();
                lines.Add(string.Format(format_id, atoms.Length));
                for(int i=0; i<atoms.Length; i++)
                {
                    var atom = atoms[i];
                    int id   = i+1;

                    //Tinker.Xyz.Atom
                    string sId       = string.Format(format_id, id);
                    string sAtomType = null;
                    string sCoord    = string.Format(format_coord, atom.x)
                                        + string.Format(format_coord, atom.y)
                                        + string.Format(format_coord, atom.z);
                    string sAtomId   = null;
                    string sBondedId = null;

                    switch(i%3)
                    {
                        case 0:
                            {
                                HDebug.Assert(atom.name == " OH2");
                                sAtomType = "  OT ";
                                sAtomId   = "   101";
                                sBondedId = string.Format(format_id, id+1)
                                            + string.Format(format_id, id+2);
                                //ATOM      1  OH2 TIP3W   1     -39.757 -43.987 -44.549  1.00  0.00      WT1  O
                                //ATOM      2  H1  TIP3W   1     -38.939 -43.594 -44.737  1.00  0.00      WT1  H
                                //ATOM      3  H2  TIP3W   1     -39.487 -44.929 -44.474  1.00  0.00      WT1  H
                                // 36944  OT    26.427752   -2.676966  -11.003556   101 36945 36946
                                // 36945  HT    25.722690   -1.988586  -10.960428    88 36944
                                // 36946  HT    25.928766   -3.441458  -11.365251    88 36944
                            }
                            break;
                        case 1:
                            {
                                HDebug.Assert(atom.name == " H1 ");
                                sAtomType = "  HT ";
                                sAtomId   = "    88";
                                sBondedId = string.Format(format_id, id-1);
                            }
                            break;
                        case 2:
                            {
                                HDebug.Assert(atom.name == " H2 ");
                                sAtomType = "  HT ";
                                sAtomId   = "    88";
                                sBondedId = string.Format(format_id, id-2);
                            }
                            break;
                    }

                    HDebug.Assert(sId       != null);
                    HDebug.Assert(sAtomType != null);
                    HDebug.Assert(sCoord    != null);
                    HDebug.Assert(sAtomId   != null);
                    HDebug.Assert(sBondedId != null);
                    string line = sId + sAtomType + sCoord + sAtomId + sBondedId;
                    lines.Add(line);
                }

                //HFile.WriteAllLines(path_solvbox_pdb.Replace(".pdb",".xyz"), lines);
                return lines;
            }
        }
    }
}
