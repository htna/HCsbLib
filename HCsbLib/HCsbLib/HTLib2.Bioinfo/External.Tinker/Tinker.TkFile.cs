using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public class TkFile
        {
            public static string[] LinesFromFile(string path, bool loadLatest)
            {
                string loadpath = path;
                if(loadLatest)
                {
                    System.IO.FileInfo   fileinfo  = HFile.GetFileInfo(path);
                    System.IO.FileInfo[] fileinfos = fileinfo.Directory.GetFiles(fileinfo.Name+"*");
                    if(fileinfos.Length != 0)
                    {
                        List<string> filepaths = new List<string>();
                        foreach(System.IO.FileInfo lfileinfo in fileinfos)
                            filepaths.Add(lfileinfo.FullName);
                        filepaths.Sort();
                        loadpath = filepaths.Last();
                    }
                }
                if(HFile.Exists(loadpath) == false)
                    return null;
                string[] lines = HFile.ReadAllLines(loadpath);
                return lines;
            }
            public static void ElementsToFile(string path, bool saveAsNext, IList<Element> elements)
            {
                string writepath = path;
                if(saveAsNext)
                {
                    int idx=2;
                    while(HFile.Exists(writepath))
                    {
                        writepath = string.Format("{0}_{1}", path, idx);
                        idx++;
                    }
                }

                List<string> lines = new List<string>();
                foreach(Element element in elements)
                    lines.Add(element.line);
                HFile.WriteAllLines(writepath, lines);
            }

            public class Element
            {
                public readonly string line;
                public Element(string line) { this.line = line; }
                public virtual string type { get { throw new NotImplementedException(); } }

                string[] tokens = null;
                public override string ToString()
                {
                    return line;
                }

                public string[] GetTokens()
                {
                    if(tokens != null)
                        return tokens;
                    tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    return tokens;
                }
                public string  GetTokenString(int idx) { string[] tokens = GetTokens(); if(idx >= tokens.Length) return null;return              tokens[idx];  }
                public int?    GetTokenInt   (int idx) { string[] tokens = GetTokens(); if(idx >= tokens.Length) return null;return    int.Parse(tokens[idx]); }
                public double? GetTokenDouble(int idx) { string[] tokens = GetTokens(); if(idx >= tokens.Length) return null;return double.Parse(tokens[idx]); }

                public string  GetString(int[] idx)
                {
                    if(line.Length <= idx[0]) return null;
                    if(line.Length <= idx[1]) return line.Substring(idx[0]);
                    int leng=idx[1]-idx[0]+1;
                    return line.Substring(idx[0], leng);
                }
                public int?    GetInt   (params int[] idx) { string str=GetString(idx); if(str == null) return null; int    val; if(int   .TryParse(str, out val) == false) return null; return val; }
                public double? GetDouble(params int[] idx) { string str=GetString(idx); if(str == null) return null; double val; if(double.TryParse(str, out val) == false) return null; return val; }

                public static string UpdateLine<T>(string line, T value, string format, params int[] idx)
                {
                    string str = string.Format(format, value).HSubEndStringCount(idx[1]-idx[0]+1);
                    char[] nline = line.ToArray();
                    for(int i=0; i<str.Length; i++)
                        nline[i+idx[0]] = str[i];
                    return nline.HToString();
                }
            }
        }
    }
}
