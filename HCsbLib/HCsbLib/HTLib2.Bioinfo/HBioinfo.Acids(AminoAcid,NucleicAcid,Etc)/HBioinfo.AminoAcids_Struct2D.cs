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
                    var graph = Struct2D.GetStruct2D(resi);

                    var lines = graph.GetMathematicaString_Graph
                        ( style:"Yellow"
                        , size:"0.5"
                        );

                    return null;
                }
                public partial class Struct2D
                {
                    public Dictionary<string, int>           name_ivert;
                    public List<(int x, int y, string name)> verts;
                    public List<(string edgetype, int v1, string v1name, int v2, string v2name)> edges;

                    public static Struct2D GetStruct2D(string resn)
                    {
                        List<(int x, int y, string name)> verts;
                        List<(string edgetype, int v1, string v1name, int v2, string v2name)> edges;

                        string[] lines = GetStructShape2DDataLines(resn);
                        verts = FindVerts(lines);
                        edges = FindEdges(lines, verts);

                        Dictionary<string, int> name_ivert = new Dictionary<string, int>(verts.Count);
                        for(int i=0; i<verts.Count; i++)
                            if(verts[i].name != "")
                                name_ivert.Add(verts[i].name, i);

                        return new Struct2D
                        {
                            verts = verts,
                            edges = edges,
                            name_ivert = name_ivert,
                        };
                    }

                    public string[] GetMathematicaString_Graph
                        ( object style=null
                        , object size=null
                        )
                    {
                        List<string> lines = new List<string>();
                        lines.Add("Graph[");
                        lines.Add(                            GetMathematicaString_Edges()                  );
                        lines.Add(", VertexCoordinates -> " + GetMathematicaString_VertexCoordinates(0.4, 1));
                        lines.Add(", VertexLabels -> "      + GetMathematicaString_VertexLabels()           );
                        lines.Add(", VertexSize -> "        + GetMathematicaString_VertexSize(size)         );
                        lines.Add(", VertexStyle -> "       + GetMathematicaString_VertexStyle(style)       );
                        lines.Add(", EdgeStyle -> "         + GetMathematicaString_EdgeStyle("Directive[Darker[Gray],Thickness[0.008]]", "Directive[Darker[Gray],Thickness[0.016],Black]"));
                        lines.Add("]");
                        return lines.ToArray();
                    }

                    public string GetMathematicaString_EdgeStyle
                        ( string singlebond //"Directive[Darker[Gray],Thickness[0.008]]"
                        , string doublebond //"Directive[Darker[Gray],Thickness[0.016]]"
                        )
                    {
                        Dictionary<(int,int),string> edge_style = new Dictionary<(int,int),string>();
                        foreach(var edge in edges)
                        {
                            switch(edge.edgetype)
                            {
                                case "║": case "═": case "╲╲": case "╱╱":
                                    edge_style.Add((edge.v1,edge.v2), doublebond);
                                    break;
                                default:
                                    break;
                            }
                        }
                        return GetMathematicaString_EdgeValue
                            ( default_value: singlebond
                            ,    edge_value: edge_style
                            );
                    }
                    public string GetMathematicaString_EdgeValue
                        ( string                    default_value = null
                        , Dictionary<(int,int),string> edge_value = null
                        )
                    {
                        if((default_value == null) && (edge_value == null))
                            throw new Exception();

                        if(edge_value == null)
                           edge_value = new Dictionary<(int,int),string>();

                        string str = "";
                        foreach(var edge in edges)
                        {
                            (int,int) v12 = (edge.v1, edge.v2); string sval12 = null; if(edge_value.ContainsKey(v12)) sval12 = edge_value[v12];
                            (int,int) v21 = (edge.v2, edge.v1); string sval21 = null; if(edge_value.ContainsKey(v21)) sval21 = edge_value[v21];
                            
                            if((sval12 == null) && (sval21 == null)) continue;
                            if((sval12 != null) && (sval21 != null) && (sval12 != sval21)) throw new Exception();

                            string sval = null;
                            if(sval12 != null) sval = sval12;
                            if(sval21 != null) sval = sval21;

                            v12 = v12.HSort();
                            str += "," + (v12.Item1+1) + "<->" + (v12.Item2+1) + "->" + sval;
                        }

                        str = str.Substring(1).Replace(" ","");
                        if(default_value != null) str = "{" + default_value + "," + str + "}";
                        else                      str = "{" +                       str + "}";
                        return str;
                    }

                    public string GetMathematicaString_VertexStyle(object style)                                { return GetMathematicaString_VertexValue(style); }
                    public string GetMathematicaString_VertexStyle(string style = "Yellow")                     { return GetMathematicaString_VertexValue(style); }
                    public string GetMathematicaString_VertexStyle(Dictionary<string,string> name_style = null) { return GetMathematicaString_VertexValue(name_style); }
                    
                    public string GetMathematicaString_VertexSize(object size)                                  { return GetMathematicaString_VertexValue(size); }
                    public string GetMathematicaString_VertexSize(string size = "0.5")                          { return GetMathematicaString_VertexValue(size); }
                    public string GetMathematicaString_VertexSize(Dictionary<string,string> name_size = null)   { return GetMathematicaString_VertexValue(name_size); }

                    public string GetMathematicaString_VertexValue(object value)
                    {
                        switch(value)
                        {
                            case string strvalue:
                                return GetMathematicaString_VertexValue(strvalue);
                            case Dictionary<string, string> name_value:
                                return GetMathematicaString_VertexValue(name_value);
                            default:
                                throw new Exception();
                        }
                    }
                    public string GetMathematicaString_VertexValue(string value)
                    {
                        Dictionary<string,string> name_size = new Dictionary<string, string>(verts.Count);
                        for(int i=0; i<verts.Count; i++)
                        {
                            string name = verts[i].name;
                            if(name == "")
                                continue;
                            name_size.Add(name, value);
                        }
                        return GetMathematicaString_VertexValue(name_size);
                    }
                    public string GetMathematicaString_VertexValue(Dictionary<string,string> name_value)
                    {
                        if(name_value == null)
                            name_value = new Dictionary<string,string>();
                        string str = "";
                        for(int i=0; i<verts.Count; i++)
                        {
                            string name = verts[i].name;
                            if(name_value.ContainsKey(name) == false)
                                continue;
                            string value = name_value[name];
                            str += "," + (i+1) + "->" + value;
                        }
                        str = str.Substring(1).Replace(" ","");
                        return "{"+str+"}";
                    }

                    public string GetMathematicaString_VertexCoordinates(double scale_x = 1, double scale_y = 1, int round_digit = 2)
                    {
                        string str = "";
                        for(int i=0; i<verts.Count; i++)
                        {
                            double x = Math.Round(verts[i].x * scale_x, round_digit); 
                            double y = Math.Round(verts[i].y * scale_y, round_digit);
                            str += "," + (i+1) + "->" + Mathematica.ToString2((x,y));
                        }
                        str = str.Substring(1).Replace(" ","");
                        return "{"+str+"}";
                    }

                    public string GetMathematicaString_VertexLabels(Dictionary<string,string> name_label = null)
                    {
                        if(name_label == null)
                        {
                            name_label = new Dictionary<string, string>(verts.Count);
                            foreach(var vert in verts)
                                if(vert.name != "")
                                    name_label.Add(vert.name, vert.name);
                        }
                        string str = "";
                        for(int i=0; i<verts.Count; i++)
                        {
                            string name  = verts[i].name;
                            if(name_label.ContainsKey(name) == false)
                                continue;

                            string label = name_label[name];
                            str += ",{" + (i+1) + ",\"" + label + "\"}";
                        }
                        str = "{" + str.Substring(1) + "}";
                        str = "Table[item[[1]]->Placed[item[[2]],Center], {item,"+str+"}]";
                        return str;
                    }

                    public string GetMathematicaString_Edges()
                    {
                        string str = "";
                        foreach(var edge in edges)
                        {
                            str += "," + (edge.v1+1) + "<->" + (edge.v2+1);
                        }
                        str = str.Substring(1);
                        return "{"+str+"}";
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

                    //  public static char[,] LinesToMap(string[] lines)
                    //  {
                    //      int max_line_length = -1;
                    //      foreach(var line in lines)
                    //          max_line_length = Math.Max(max_line_length, line.Length);
                    //  
                    //      char[,] map = new char[max_line_length, lines.Length];
                    //      for(int x=0; x<map.GetLength(0); x++)
                    //          for(int y=0; y<map.GetLength(1); y++)
                    //              map[x,y] = ' ';
                    //  
                    //      for(int y=0; y<lines.Length; y++)
                    //      {
                    //          string line = lines[y];
                    //          for(int x=0; x<line.Length; x++)
                    //          {
                    //              char ch = line[x];
                    //              if(ch == '!') ch = ' ';
                    //              map[x,y] = ch;
                    //          }
                    //      }
                    //  
                    //      return map;
                    //  }

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
