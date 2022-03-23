using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Acid = HBioinfo.Acid;
    public static partial class HStaticBioinfo
    {
        public static partial class HBioinfo
        {
            public partial class Acid
            {
                public static object ReadStructShape2D(string resi)
                {
                    return CReadStructShape2D.ReadStructShape2D(resi);
                }
                public class CReadStructShape2D
                {
                    public static object ReadStructShape2D(string resn)
                    {
                        string[] lines = GetStructShape2DDataLines(resn);
                        //ColumnToPtX(lines[0]);
                        char[,] map = LinesToMap(lines);
                        List<(int x, int y, string name)> verts = FindVerts(lines);
                        List<(string edgetype, int v1, string v1name, int v2, string v2name)> edges = FindEdges(lines, verts);
                        return null;
                    }

                    public static Dictionary<string, List<string>> _resn_StructShape2DDataLines = null;
                    public static string[] GetStructShape2DDataLines(string resn)
                    {
                        if(_resn_StructShape2DDataLines == null)
                        {
                            _resn_StructShape2DDataLines = new Dictionary<string, List<string>>();
                            List<string> lines = null;
                            foreach(var line in struct_shape_2d_data)
                            {
                                if(line.StartsWith("RESI "))
                                {
                                    string[] tokens = line.HSplit(' ');
                                    HDebug.Assert(tokens[0] == "RESI");
                                    lines = new List<string>();
                                    _resn_StructShape2DDataLines.Add(tokens[1].ToUpper(), lines);
                                }
                                else if(line[0] == '!')
                                {
                                    if(lines != null)
                                        lines.Add(line);
                                }
                                else
                                {
                                    /// skip
                                }
                            }
                        }

                        resn = resn.ToUpper();
                        if(_resn_StructShape2DDataLines.ContainsKey(resn) == false)
                            return null;
                        return _resn_StructShape2DDataLines[resn].ToArray();
                    }

                    public static char[,] LinesToMap(string[] lines)
                    {
                        int max_line_length = -1;
                        foreach(var line in lines)
                            max_line_length = Math.Max(max_line_length, line.Length);
                    
                        char[,] map = new char[max_line_length, lines.Length];
                        for(int x=0; x<map.GetLength(0); x++)
                            for(int y=0; y<map.GetLength(1); y++)
                                map[x,y] = ' ';
                    
                        for(int y=0; y<lines.Length; y++)
                        {
                            string line = lines[y];
                            for(int x=0; x<line.Length; x++)
                            {
                                char ch = line[x];
                                if(ch == '!') ch = ' ';
                                map[x,y] = ch;
                            }
                        }
                    
                        return map;
                    }

                    public static List<(int x, int y, string name)> FindVerts(string[] lines)
                    {
                        List<(int x, int y, string name)> verts = new List<(int x, int y, string name)>();
                        for(int y=0; y<lines.Length; y++)
                        {
                            string line = lines[y];
                            for(int x=0; x<line.Length; x++)
                            {
                                char ch = line[x];
                                if(ch == '*')
                                {
                                    string name = "";
                                    for(int xx=x+1; xx<line.Length; xx++)
                                    {
                                        ch = line[xx];
                                        if(('a'<=ch && ch<='z') || ('A'<=ch && ch<='Z') || ('0'<=ch && ch<='9'))
                                        {
                                            name = name + ch;
                                            continue;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }

                                    verts.Add((x, y, name));
                                }
                            }
                        }
                        return verts;
                    }

                    public static List<(string edgetype, int v1, string v1name, int v2, string v2name)> FindEdges(string[] lines, List<(int x, int y, string name)> verts)
                    {
                        HashSet<(int v1, int v2, string type)> edgeset = new HashSet<(int v1, int v2, string type)>();
                        for(int y=0; y<lines.Length; y++)
                        {
                            string line = lines[y];
                            for(int x=0; x<line.Length; x++)
                            {
                                char ch = line[x];
                                int v1, v2;
                                switch(ch)
                                {
                                    case '!':
                                    case ' ':
                                        break;
                                    case '│': { (v1,v2) = FindEdge_Up_Down   (lines, verts, x, y); edgeset.Add((v1,v2,"│")); } break;
                                    case '║': { (v1,v2) = FindEdge_Up_Down   (lines, verts, x, y); edgeset.Add((v1,v2,"║")); } break;
                                    case '─': { (v1,v2) = FindEdge_Left_Right(lines, verts, x, y); edgeset.Add((v1,v2,"─")); } break;
                                    case '═': { (v1,v2) = FindEdge_Left_Right(lines, verts, x, y); edgeset.Add((v1,v2,"═")); }  break;
                                    case '╲':
                                        (v1,v2) = FindEdge_UpLeft_DownRight(lines, verts, x, y);
                                        if(line[x+1] == '╲') { edgeset.Add((v1,v2,"╲╲")); x++; }
                                        else                 { edgeset.Add((v1,v2,"╲" ));      }
                                        break;
                                    case '╱':
                                        (v1,v2) = FindEdge_UpRight_DownLeft(lines, verts, x, y);
                                        if(line[x+1] == '╱') { edgeset.Add((v1,v2,"╱╱")); x++; }
                                        else                 { edgeset.Add((v1,v2,"╱" ));      }
                                        break;
                                }
                            }
                        }

                        List<(string edgetype, int v1, string v1name, int v2, string v2name)> edges = new List<(string edgetype, int v1, string v1name, int v2, string v2name)>();
                        foreach(var edge in edgeset)
                        {
                            string v1name = verts[edge.v1].name;
                            string v2name = verts[edge.v2].name;
                            edges.Add((edge.type, edge.v1, v1name, edge.v2, v2name));
                        }

                        return edges;
                    }
                    public static int FindVert(List<(int x, int y, string name)> verts, int x, int y)
                    {
                        for(int i=0; i<verts.Count; i++)
                        {
                            var vert = verts[i];
                            if((x == vert.x) && (y == vert.y))
                                return i;
                        }
                        return -1;
                    }
                    public static (int v1, int v2) FindEdge_Up_Down(string[] lines, List<(int x, int y, string name)> verts, int x, int y)
                    {
                        int vert_y1 = -1; for(int ny=y; ; ny--) if(lines[ny][x] == '*') { vert_y1=ny; break; }
                        int vert_y2 = -1; for(int ny=y; ; ny++) if(lines[ny][x] == '*') { vert_y2=ny; break; }
                        int v1 = FindVert(verts, x, vert_y1); HDebug.Assert(v1 != -1);
                        int v2 = FindVert(verts, x, vert_y2); HDebug.Assert(v2 != -1);
                        return (v1, v2).HSort();
                    }
                    public static (int v1, int v2) FindEdge_Left_Right(string[] lines, List<(int x, int y, string name)> verts, int x, int y)
                    {
                        int vert_x1 = -1; for(int nx=x; ; nx--) if(lines[y][nx] == '*') { vert_x1=nx; break; }
                        int vert_x2 = -1; for(int nx=x; ; nx++) if(lines[y][nx] == '*') { vert_x2=nx; break; }
                        int v1 = FindVert(verts, vert_x1, y); HDebug.Assert(v1 != -1);
                        int v2 = FindVert(verts, vert_x2, y); HDebug.Assert(v2 != -1);
                        return (v1, v2).HSort();
                    }
                    public static (int v1, int v2) FindEdge_UpLeft_DownRight(string[] lines, List<(int x, int y, string name)> verts, int x, int y)
                    {
                        int vert_y1 = y-1; int vert_x1 = -1; for(int nx=x; ; nx--) if(lines[vert_y1][nx] == '*') { vert_x1=nx; break; }
                        int vert_y2 = y+1; int vert_x2 = -1; for(int nx=x; ; nx++) if(lines[vert_y2][nx] == '*') { vert_x2=nx; break; }
                        int v1 = FindVert(verts, vert_x1, vert_y1); HDebug.Assert(v1 != -1);
                        int v2 = FindVert(verts, vert_x2, vert_y2); HDebug.Assert(v2 != -1);
                        return (v1, v2).HSort();
                    }
                    public static (int v1, int v2) FindEdge_UpRight_DownLeft(string[] lines, List<(int x, int y, string name)> verts, int x, int y)
                    {
                        int vert_y1 = y-1; int vert_x1 = -1; for(int nx=x; ; nx++) if(lines[vert_y1][nx] == '*') { vert_x1=nx; break; }
                        int vert_y2 = y+1; int vert_x2 = -1; for(int nx=x; ; nx--) if(lines[vert_y2][nx] == '*') { vert_x2=nx; break; }
                        int v1 = FindVert(verts, vert_x1, vert_y1); HDebug.Assert(v1 != -1);
                        int v2 = FindVert(verts, vert_x2, vert_y2); HDebug.Assert(v2 != -1);
                        return (v1, v2).HSort();
                    }

                    public static double[] ColumnToPtX(string lines)
                    {
                        HDebug.Assert(lines.StartsWith("!!"));

                        List<int> colstars = new List<int>();
                        for(int i=0; i<lines.Length; i++)
                            if(lines[i] == '*')
                                colstars.Add(i);

                        List<double> col2ptx = new List<double>();
                        //for(int i=0; i<lines.Length; i++)

                        return null;
                    }
                }
            }
        }
    }
}
