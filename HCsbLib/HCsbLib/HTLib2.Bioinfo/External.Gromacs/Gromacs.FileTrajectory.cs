using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public class FileTrajectory
        {
            public class Frame
            {
                public string srcfile;
                public int frameidx;
                public int natoms;
                public int step;
                public double time;
                public int lambda;
                public Vector[] box;
                public Vector[] x;
                public Vector[] v;
                public Vector[] f;

                public static Frame FromLines(string[] lines)
                {
                    Frame frame = new Frame();

                    {
                        //eigenvec.trr frame 0:
                        string line = lines[0];
                        string[] tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        HDebug.Assert(tokens.Length == 3);
                        frame.srcfile = tokens[0];                  // eigenvec.trr
                        HDebug.Assert(tokens[1] == "frame");         // frame
                        HDebug.Assert(tokens[2].Last() == ':');      // 0:
                        frame.frameidx = int.Parse(tokens[2].Replace(":", ""));
                    }
                    {
                        //   natoms=       920  step=         0  time=0.0000000e+00  lambda=         1
                        string line = lines[1];
                        string[] tokens = line.Split(new char[] { ' ', '\t', '=' }, StringSplitOptions.RemoveEmptyEntries);
                        HDebug.Assert(             tokens[0] == "natoms");
                        frame.natoms = int.Parse( tokens[1]);
                        HDebug.Assert(             tokens[2] == "step");
                        frame.step = int.Parse(   tokens[3]);
                        HDebug.Assert(             tokens[4] == "time");
                        frame.time = double.Parse(tokens[5]);
                        HDebug.Assert(             tokens[6] == "lambda");
                        frame.lambda = int.Parse( tokens[7]);
                    }
                    Dictionary<string, Tuple<int, int, Vector[]>> vars = new Dictionary<string, Tuple<int, int, Vector[]>>();
                    for(int iline=2; iline<lines.Length; iline++)
                    {
                        string line = lines[iline];
                        line = line.Trim();

                        if(line.EndsWith("):"))
                        {
                            // "VALNAME (COLSIZExROWSIZE):"
                            string[] tokens;
                            tokens = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                            // "VALNEMS", "(COLSIZExROWSIZE):"
                            HDebug.Assert(tokens.Length == 2);
                            string var_name = tokens[0];
                            HDebug.Assert(tokens[1].StartsWith("(") && tokens[1].EndsWith("):"));
                            tokens = tokens[1].Split(new char[] { ' ', '\t', '(', 'x', ')', ':' }, StringSplitOptions.RemoveEmptyEntries);
                            // "COLSIZE","ROWSIZE"
                            HDebug.Assert(tokens.Length == 2);
                            int var_colsize = int.Parse(tokens[0]);
                            int var_rowsize = int.Parse(tokens[1]);
                            vars.Add(var_name, new Tuple<int, int, Vector[]>(var_colsize, var_rowsize, new Vector[var_colsize]));
                            continue;
                        }
                        if(line.Contains("]={"))
                        {
                            // "VARNAME[ INDEX]={VAL0, VAL1, VAL2, ...}
                            string[] tokens;
                            tokens = line.Split(new char[] { '[', ']', '=', '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                            string var_name  = tokens[0].Trim();
                            int    var_idx   = int.Parse(tokens[1]);
                            Vector var_value = Vector.FromString(tokens[2]);
                            Tuple<int, int, Vector[]> var = vars[var_name];
                            int var_rowsize = var.Item2;
                            HDebug.Assert(var_rowsize == var_value.Size);
                            HDebug.Assert(var.Item3[var_idx] == null);
                            var.Item3[var_idx] = var_value;
                            continue;
                        }
                        HDebug.Assert(false);
                    }
                    {
                        // check if there is any null vector in vars
                        foreach(string var_name in vars.Keys)
                        {
                            Tuple<int, int, Vector[]> var = vars[var_name];
                            int      var_colsize = var.Item1;
                            int      var_rowsize = var.Item2;
                            Vector[] var_values  = var.Item3;
                            HDebug.Assert(var_colsize == var_values.Length);
                            foreach(Vector var_value in var_values)
                                HDebug.Assert(var_value._data != null && var_value.Size == var_rowsize);
                        }
                    }

                    if(vars.ContainsKey("box")) frame.box = vars["box"].Item3;
                    if(vars.ContainsKey(  "x")) frame.x   = vars[  "x"].Item3;
                    if(vars.ContainsKey(  "v")) frame.v   = vars[  "v"].Item3;
                    if(vars.ContainsKey(  "f")) frame.f   = vars[  "f"].Item3;

                    return frame;
                }
            };

            public List<Frame> frames;

            static bool selftest = HDebug.IsDebuggerAttached;
            public static void SelfTest(string rootpath, string[] args)
            {
                if(selftest == false)
                    return;
                selftest = false;

                string filepath = rootpath + @"\Bioinfo\External.Gromacs\Selftest\FileTrajectory.traj.txt";
                FileTrajectory eigvec = FromFile(filepath);
            }
            public static FileTrajectory FromFile(string filepath)
            {
                string[] lines = HFile.ReadAllLines(filepath);
                return FromLines(lines);
            }
            public static FileTrajectory FromLines(string[] lines)
            {
                List<List<string>> frameliness = GroupByFrames(lines);
                FileTrajectory trajs = new FileTrajectory();
                trajs.frames = new List<Frame>();
                foreach(List<string> framelines in frameliness)
                {
                    Frame frame = Frame.FromLines(framelines.ToArray());
                    trajs.frames.Add(frame);
                }

                return trajs;
            }
            public static List<List<string>> GroupByFrames(string[] lines)
            {
                List<List<string>> groups = new List<List<string>>();
                foreach(string line in lines)
                {
                    if(line.Contains(" frame "))
                    {
                        groups.Add(new List<string>());
                    }
                    if(groups.Count >= 1)
                    {
                        groups.Last().Add(line);
                    }
                }
                return groups;
            }
        }
    }
}
