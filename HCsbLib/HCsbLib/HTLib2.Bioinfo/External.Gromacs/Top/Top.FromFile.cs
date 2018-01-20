using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public partial class Top
        {
            public static Top FromFile(string path)
            {
                List<string> defines = new List<string>();
                return FromFile(path, defines);
            }
            public static Top FromFile(string path, List<string> defines)
            {
                List<string> lines = new List<string>(HFile.ReadAllLines(path));

                List<LineElement> elements = new List<LineElement>();
                //List<Tuple<string,List<LineElement>>> elementgroup = new List<Tuple<string, List<LineElement>>>();
                //Dictionary<int,Atom> atoms = new Dictionary<int, Atom>();
                //List<Bond> bonds = new List<Bond>();
                //List<Pair> pairs = new List<Pair>();
                //List<Angle> angles = new List<Angle>();
                string type = null;
                while(lines.Count > 0)
                //for(int i=0; i<lines.Length; i++)
                {
                    string line = lines[0];
                    lines.RemoveAt(0);
                    //LineElement element = new LineElement(lines[i]);
                    string typei = LineElement.GetType(line);
                    if(typei != null)
                    {
                        type = typei;
                        //elementgroup.Add(new Tuple<string, List<LineElement>>(type, new List<LineElement>()));
                        continue;
                    }

                    {
                        line = line.Trim();
                        if(line.EndsWith("\\"))
                        {
                            while(lines.Count > 0)
                            {
                                line = line + "\n" + lines[0].Trim();
                                lines.RemoveAt(0);
                                if(line.EndsWith("\\") == false)
                                    break;
                            }
                            //line = "";
                            //for(; i<lines.Length; i++)
                            //{
                            //    string lline = lines[i].Trim();
                            //    line = line + lline + "\n";
                            //    if(lline.EndsWith("\\") == false)
                            //        break;
                            //}
                        }
                        line = (line.IndexOf(';') == -1) ? line.Trim() : line.Substring(0, line.IndexOf(';')).Trim();
                        if(line.Length == 0)
                            continue;
                        if(line[0] == '*')
                            continue;
                        if(line.StartsWith("#define"))
                        {
                            string define = line.Replace("#define","").Trim();
                            defines.Add(define);
                            continue;
                        }
                        if(line.StartsWith("#include"))
                        {
                            string includepath;
                            {
                                includepath = line.Replace("#include", "").Trim().Replace("\"", "").Trim();
                                if(HFile.Exists(includepath) == false)
                                {
                                    includepath = @"C:\Program Files (x86)\Gromacs\share\gromacs\top\"
                                                + line.Replace("#include", "").Trim().Replace("\"", "").Trim();
                                }
                                if(HFile.Exists(includepath) == false)
                                {
                                    includepath = HDirectory.GetParent(path).FullName + "\\" //path.Substring(0, path.LastIndexOf('/')+1)
                                                + line.Replace("#include", "").Trim().Replace("\"", "").Trim();
                                }
                                HDebug.Assert(HFile.Exists(includepath));
                            }
                            Top    includetop  = FromFile(includepath, defines);
                            elements.AddRange(includetop.elements);
                            type = null;
                            continue;
                        }
                        if(line.StartsWith("#ifdef"))
                        {
                            FromFile_ifdef(defines, lines, line);
                            continue;
                        }
                        if(line.StartsWith("#"))
                        {
                            HDebug.Assert(false);
                        }
                        if(type == "moleculetype"           ) { LineElement element=new Moleculetype           (line,path); elements.Add(element); continue; }
                        if(type == "atoms"                  ) { LineElement element=new Atom                   (line,path); elements.Add(element); continue; }
                        if(type == "bonds"                  ) { LineElement element=new Bond                   (line,path); elements.Add(element); continue; }
                        if(type == "pairs"                  ) { LineElement element=new Pair                   (line,path); elements.Add(element); continue; }
                        if(type == "angles"                 ) { LineElement element=new Angle                  (line,path); elements.Add(element); continue; }
                        if(type == "dihedrals"              ) { LineElement element=new Dihedral               (line,path); elements.Add(element); continue; }
                        if(type == "cmap"                   ) { LineElement element=new Cmap                   (line,path); elements.Add(element); continue; }
                        if(type == "position_restraints"    ) { LineElement element=new Position_restraints    (line,path); elements.Add(element); continue; }
                        if(type == "system"                 ) { LineElement element=new System                 (line,path); elements.Add(element); continue; }
                        if(type == "molecules"              ) { LineElement element=new Molecules              (line,path); elements.Add(element); continue; }

                        if(type == "defaults"               ) { LineElement element=new Defaults               (line,path); elements.Add(element); continue; }
                        if(type == "atomtypes"              ) { LineElement element=new Atomtypes              (line,path); elements.Add(element); continue; }
                        if(type == "pairtypes"              ) { LineElement element=new Pairtypes              (line,path); elements.Add(element); continue; }
                        if(type == "bondtypes"              ) { LineElement element=new Bondtypes              (line,path); elements.Add(element); continue; }
                        if(type == "constrainttypes"        ) { LineElement element=new Constrainttypes        (line,path); elements.Add(element); continue; }
                        if(type == "angletypes"             ) { LineElement element=new Angletypes             (line,path); elements.Add(element); continue; }
                        if(type == "dihedraltypes"          ) { LineElement element=new Dihedraltypes          (line,path); elements.Add(element); continue; }
                        if(type == "implicit_genborn_params") { LineElement element=new Implicit_genborn_params(line,path); elements.Add(element); continue; }
                        if(type == "cmaptypes"              ) { LineElement element=new Cmaptypes              (line,path); elements.Add(element); continue; }
                        if(type == "settles"                ) { LineElement element=new Settles                (line,path); elements.Add(element); continue; }
                        if(type == "exclusions"             ) { LineElement element=new Exclusions             (line,path); elements.Add(element); continue; }

                        HDebug.Assert(false);
                    }
                }

                Top top = new Top();
                top.elements = elements.ToArray();

                return top;
            }

            private static void FromFile_ifdef(List<string> defines, List<string> lines, string line)
            {
                string define = line.Replace("#ifdef", "").Trim();

                List<string> lines_true  = new List<string>();
                List<string> lines_false = new List<string>();
                List<string> lines_addto = lines_true;
                while(lines.Count > 0)
                {
                    line = lines[0];
                    lines.RemoveAt(0);

                    if(line.StartsWith("#ifdef")) FromFile_ifdef(defines, lines, line);
                    else if(line.StartsWith("#else")) lines_addto = lines_false;
                    else if(line.StartsWith("#endif")) break;
                    else lines_addto.Add(line);
                }

                if(defines.Contains(define)) lines.InsertRange(0, lines_true);
                else                         lines.InsertRange(0, lines_false);
            }
        }
    }
}
