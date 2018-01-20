using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public class Eigenvec
        {
            public class Frame
            {
                public string srcfile;
                public int frameidx;
                public int natoms;
                public int step;
                public double time; // eigenvalue
                public int lambda;
                //public Matrix box;
                public Vector[] x;  // eigenvector

                public static Frame FromLines(string[] lines)
                {
                    Frame frame = new Frame();
                    int lineidx = 0;
                    {
                        //eigenvec.trr frame 0:
                        string[] tokens = lines[lineidx++].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        HDebug.Assert(tokens.Length == 3);
                        frame.srcfile = tokens[0];                  // eigenvec.trr
                        HDebug.Assert(tokens[1] == "frame");         // frame
                        HDebug.Assert(tokens[2].Last() == ':');      // 0:
                        frame.frameidx = int.Parse(tokens[2].Replace(":", ""));
                    }
                    {
                        //   natoms=       920  step=         0  time=0.0000000e+00  lambda=         1
                        string[] tokens = lines[lineidx++].Split(new char[] { ' ', '\t','=' }, StringSplitOptions.RemoveEmptyEntries);
                        HDebug.Assert(             tokens[0] == "natoms");
                        frame.natoms = int.Parse( tokens[1]);
                        HDebug.Assert(             tokens[2] == "step");
                        frame.step = int.Parse(   tokens[3]);
                        HDebug.Assert(             tokens[4] == "time");
                        frame.time = double.Parse(tokens[5]);
                        HDebug.Assert(             tokens[6] == "lambda");
                        frame.lambda = int.Parse( tokens[7]);
                    }
                    {
                        //box (3x3):
                        //   box[    0]={ 0.00000e+00,  0.00000e+00,  0.00000e+00}
                        //   box[    1]={ 0.00000e+00,  0.00000e+00,  0.00000e+00}
                        //   box[    2]={ 0.00000e+00,  0.00000e+00,  0.00000e+00}
                        lineidx++; HDebug.Assert(lines[lineidx-1] == "   box (3x3):");
                        lineidx++; //Debug.Assert(lines[lineidx-1] == "      box[    0]={ 0.00000e+00,  0.00000e+00,  0.00000e+00}");
                        lineidx++; //Debug.Assert(lines[lineidx-1] == "      box[    1]={ 0.00000e+00,  0.00000e+00,  0.00000e+00}");
                        lineidx++; //Debug.Assert(lines[lineidx-1] == "      box[    2]={ 0.00000e+00,  0.00000e+00,  0.00000e+00}");
                    }
                    {
                        //   x (920x3):
                        //      x[    0]={ 1.03414e+00,  8.38227e-01,  9.45561e-01}
                        //      x[    1]={ 9.80454e-01,  9.12340e-01,  8.96750e-01}
                        //      x[    2]={ 1.02299e+00,  7.46792e-01,  8.98052e-01}
                        //      x[    3]={ 9.96949e-01,  8.30545e-01,  1.04402e+00}
                        //      ...
                        string line = lines[lineidx++]; //x (920x3):
                        HDebug.Assert(line.StartsWith("   x ("), line.EndsWith("):"));
                        List<Vector> x = new List<Vector>();
                        for(; lineidx<lines.Length; lineidx++)
                        {
                            line = lines[lineidx];
                            string valname;
                            int validx;
                            Vector val;
                            HDebug.Verify(ReadVector(line, out valname, out validx, out val));
                            HDebug.Assert(valname == "x");
                            HDebug.Assert(validx == x.Count);
                            HDebug.Assert(val.Size == 3);
                            x.Add(val);
                        }
                        frame.x = x.ToArray();
                    }
                    return frame;
                }
                public static bool ReadVector(string line, out string valname, out int validx, out Vector val)
                {
                    string[] tokens;
                    string token0, token1;
                    {
                        tokens = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                        HDebug.Assert(tokens.Length == 2);
                        token0 = tokens[0];
                        token1 = tokens[1];
                    }
                    {
                        tokens = token0.Split(new char[] { ' ', '\t', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                        HDebug.Assert(tokens.Length == 2);
                        valname = tokens[0];
                        validx  = int.Parse(tokens[1]);
                    }
                    {
                        HDebug.Assert(token1.StartsWith("{"), token1.EndsWith("}"));
                        string token10 = token1;
                        token10 = token10.Replace("{", "");
                        token10 = token10.Replace("}", "");
                        val = Vector.FromString(token10);
                    }
                    return true;
                }
            };

            public List<Frame> frames;

            static bool selftest = HDebug.IsDebuggerAttached;
            public static void SelfTest(string rootpath, string[] args)
            {
                if(selftest == false)
                    return;
                selftest = false;

                string filepath = rootpath + @"\Bioinfo\External.Gromacs\Selftest\eigenvec.txt";
                Eigenvec eigvec = FromFile(filepath);
            }
            public static Eigenvec FromFile(string filepath)
            {
                List<List<string>> frameliness;
                {
                    string[] lines = HFile.ReadAllLines(filepath);
                    frameliness = GroupByFrames(lines);
                }
                Eigenvec eigvec = new Eigenvec();
                eigvec.frames = new List<Frame>();
                foreach(List<string> framelines in frameliness)
                {
                    Frame frame = Frame.FromLines(framelines.ToArray());
                    eigvec.frames.Add(frame);
                }

                return eigvec;
            }
            public List<Mode> ToModes()
            {
                List<Mode> modes = new List<Mode>();
                for(int i=0; i<frames.Count; i++)
                {
                    Frame frame = frames[i];
                    if(frame.lambda != 0)
                        continue;
                    Mode mode;
                    {
                        mode = new Mode();
                        mode.th     = frame.step;
                        mode.eigval = frame.time;
                        mode.eigvec = new double[frame.x.Length*3];
                        for(int j=0; j<frame.x.Length; j++)
                        {
                            HDebug.Assert(frame.x[j].Size == 3);
                            mode.eigvec[j*3+0] = frame.x[j][0];
                            mode.eigvec[j*3+1] = frame.x[j][1];
                            mode.eigvec[j*3+2] = frame.x[j][2];
                        }
                    }
                    modes.Add(mode);
                }
                return modes;
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
