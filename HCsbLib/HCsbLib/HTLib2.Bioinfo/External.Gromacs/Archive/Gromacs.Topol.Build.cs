/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo.External
{
    public partial class Gromacs
    {
        public partial class Topol
        {
            public static Topol FromFile(string filepath)
            {
                string[] lines = File.ReadAllLines(filepath);
                lines = RemoveComments(lines);
                lines = RemoveDirectives(lines);
                List<List<string>> linegroups = GroupsByContents(lines);

                List<List<Element>> groups = new List<List<Element>>();
                foreach(List<string> linegroup in linegroups)
                {
                    switch(linegroup[0])
                    {
                        case "[ atoms ]"    : groups.Add(new List<Element>()); for(int i=1; i<linegroup.Count; i++) groups.Last().Add(Atom    .FromString(linegroup[i])); break;
                        case "[ bonds ]"    : groups.Add(new List<Element>()); for(int i=1; i<linegroup.Count; i++) groups.Last().Add(Bond    .FromString(linegroup[i])); break;
                        case "[ pairs ]"    : groups.Add(new List<Element>()); for(int i=1; i<linegroup.Count; i++) groups.Last().Add(Pair    .FromString(linegroup[i])); break;
                        case "[ angles ]"   : groups.Add(new List<Element>()); for(int i=1; i<linegroup.Count; i++) groups.Last().Add(Angle   .FromString(linegroup[i])); break;
                        case "[ dihedrals ]": groups.Add(new List<Element>()); for(int i=1; i<linegroup.Count; i++) groups.Last().Add(Dihedral.FromString(linegroup[i])); break;
                        case "[ cmap ]"     : groups.Add(new List<Element>()); for(int i=1; i<linegroup.Count; i++) groups.Last().Add(Cmap    .FromString(linegroup[i])); break;
                    }
                }

                Topol topol = new Topol();
                topol.linegroups = linegroups;
                topol.groups     = groups;

                return topol;
            }
            static string[] RemoveComments(string[] lines)
            {
                List<string> result = new List<string>();
                foreach(string line in lines)
                {
                    string lline = line;

                    // remove comments
                    int idx = lline.IndexOf(';');
                    if(idx >= 0)
                        lline = lline.Substring(0, idx);
                    // trim
                    lline = lline.Trim();
                    // append to the result if != ""
                    if(lline.Length > 0)
                        result.Add(lline);
                }
                return result.ToArray();
            }
            static string[] RemoveDirectives(string[] lines)
            {
                List<string> result = new List<string>();
                foreach(string line in lines)
                {
                    if(line.StartsWith("#include ")) continue;
                    if(line.StartsWith("#ifdef"   )) continue;
                    if(line.StartsWith("#endif"   )) continue;
                    if(line.StartsWith("#"        )) { Debug.Assert(false); continue; }
                    result.Add(line);
                }
                return result.ToArray();
            }
            static List<List<string>> GroupsByContents(string[] lines)
            {
                List<List<string>> groups = new List<List<string>>();
                foreach(string line in lines)
                {
                    if(line.StartsWith("[ ") && line.EndsWith(" ]"))
                        groups.Add(new List<string>());
                    if((groups.Count > 0) && (groups.Last() != null))
                        groups.Last().Add(line);
                }
                return groups;
            }
        }
    }
}
*/