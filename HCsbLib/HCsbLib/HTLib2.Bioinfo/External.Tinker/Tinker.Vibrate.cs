using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public partial class Vibrate
        {
            /// * original hess: H
            /// * mass matrix: M
            /// 
            /// 1. [~, eigval] = eig(H)
            /// 2. mwH = M^-0.5 H M^-0.5        ⇐ mass-weighted hessian matrix
            /// 3. [mweigvec, mweigval] = eig(mwH)
            /// 4. freq = sqrt(convert)/(2*pi*lightspeed) *sqrt(mweigval)
            ///           where convert = 418.4, lightspeed = 0.0299792458
            /// 5. mode = normalized (M^-0.5 mweigvec)

            /// * eigval: tinker eigenvalue
            /// * freq  : tinker frequency
            /// * mode  : tinker mode
            /// 
            /// mweigvec ← normalize (M^0.5 * mode)
            /// mweigval ← (2*pi*lightspeed)^2 / convert * freq^2
            /// 
            public Dictionary<int,double>                             idx2eigval;
            public Dictionary<int,double>                             idx2freq;
            public Dictionary<int,Tuple<double, Tuple<int,Vector>[]>> idx2freq_mode;

            public Mode[] ToModes(IList<double> masses)
            {
                /// mweigvec ← normalize (M^0.5 * mode)
                /// mweigval ← (2*pi*lightspeed)^2 / convert * freq^2
                ///          = scale * freq^2
                ///    where convert = 418.4, lightspeed = 0.0299792458
                ///          scale = (2*pi*lightspeed)^2 / convert
                /// 
                /// mode.eigval =             mweigval
                /// mode.eigvec = mass^-0.5 * mweigvec
                double convert = 418.4;
                double lightspeed = 0.0299792458;
                double scale = (2*Math.PI*lightspeed)*(2*Math.PI*lightspeed) / convert;

                double[] mass05 = new double[masses.Count];
                for(int i=0; i<masses.Count; i++)
                    mass05[i] = Math.Sqrt(masses[i]);

                List<Mode> modes = new List<Mode>();
                foreach(int idx in idx2freq_mode.Keys.ToArray().HSort())
                {
                    double               freq        = idx2freq_mode[idx].Item1;
                    Tuple<int, Vector>[] lst_atm_vec = idx2freq_mode[idx].Item2;
                    double               eigval      = idx2eigval   [idx];

                    double mweigval = scale * freq*freq;

                    Vector mweigvec;
                    {
                        HDebug.Assert(masses.Count == lst_atm_vec.Length);
                        mweigvec = new double[masses.Count*3];
                        for(int i=0; i<mweigvec.Size; i++) mweigvec[i] = double.NaN;
                        foreach(Tuple<int, Vector> atm_vec in lst_atm_vec)
                        {
                            int    atm = atm_vec.Item1;
                            Vector vec = atm_vec.Item2;
                            HDebug.Assert(vec.Size == 3);
                            int lidx = atm-1;
                            HDebug.Assert(double.IsNaN(mweigvec[lidx*3+0])); mweigvec[lidx*3+0] = mass05[lidx]*vec[0];
                            HDebug.Assert(double.IsNaN(mweigvec[lidx*3+1])); mweigvec[lidx*3+1] = mass05[lidx]*vec[1];
                            HDebug.Assert(double.IsNaN(mweigvec[lidx*3+2])); mweigvec[lidx*3+2] = mass05[lidx]*vec[2];
                        }
                        for(int i=0; i<mweigvec.Size; i++) HDebug.Assert(double.IsNaN(mweigvec[i]) == false);
                        mweigvec = mweigvec.UnitVector();
                    }
                    Vector modevec;
                    {
                        modevec = new double[mweigvec.Size];
                        for(int i=0; i<modevec.Size; i++)
                            modevec[i] = mweigvec[i] / mass05[i/3];
                    }

                    Mode mode = new Mode();
                    mode.eigval = mweigval;
                    mode.eigvec = modevec;
                    mode.th     = idx;
                    if(eigval == 0)
                        /// special treatment
                        mode.eigval = 0;

                    modes.Add(mode);
                }

                return modes.ToArray();
            }
            public Mode[] ToModes_wrong()
            {
                HDebug.ToDo("call ToModes() instead since this is incorrect");

                List<Mode> modes = new List<Mode>();
                foreach(int idx in idx2freq_mode.Keys)
                {
                    double eigval = idx2eigval[idx];
                    Tuple<int, Vector>[] lst_atm_vec = idx2freq_mode[idx].Item2;
                    Vector eigvec = new double[lst_atm_vec.Length*3];
                    for(int i=0; i<eigvec.Size; i++) eigvec[i] = double.NaN;
                    foreach(Tuple<int, Vector> atm_vec in lst_atm_vec)
                    {
                        int    atm = atm_vec.Item1;
                        Vector vec = atm_vec.Item2;
                        HDebug.Assert(vec.Size == 3);
                        HDebug.Assert(double.IsNaN(eigvec[(atm-1)*3+0])); eigvec[(atm-1)*3+0] = vec[0];
                        HDebug.Assert(double.IsNaN(eigvec[(atm-1)*3+1])); eigvec[(atm-1)*3+1] = vec[1];
                        HDebug.Assert(double.IsNaN(eigvec[(atm-1)*3+2])); eigvec[(atm-1)*3+2] = vec[2];
                    }
                    for(int i=0; i<eigvec.Size; i++) HDebug.Assert(double.IsNaN(eigvec[i]) == false);

                    Mode mode = new Mode();
                    mode.eigval = eigval;
                    mode.eigvec = eigvec;
                    modes.Add(mode);
                }

                return modes.ToArray();
            }
            //public Mode[] ToModesUnnormalized_wrong(double[] masses)
            //{
            //    Mode[] modes   = ToModes_wrong();
            //    Mode[] unmodes = modes.ToModesUnnormalized(masses);
            //    return unmodes;
            //}

            public static Vibrate FromFile(string filepath)
            {
                Vibrate vib;
                {
                    List<string> lines = HFile.ReadLines(filepath).ToList();
                    vib = FromLines(lines);
                }
                System.GC.Collect();
                return vib;
            }
            public static Vibrate FromLines(IList<string> lines)
            {
                if(HDebug.Selftest())
                    #region MyRegion
                {
                    Vibrate tvib = FromLines(selftest);
                    Vector tmass = new double[tvib.idx2freq_mode.First().Value.Item2.Length];
                    tmass.SetValue(1);
                    tvib.ToModes(tmass.ToArray());
                }
                    #endregion
                if(lines == null)
                    return null;

                try
                {
                    List<string> llines = new List<string>(lines);
                    for(int i=0; i<llines.Count; i++)
                    {
                        string line = llines[i];
                        int idx = line.IndexOf('#');
                        if(idx >= 0)
                            line = line.Substring(0, idx);
                        line = line.TrimEnd(' ');
                        llines[i] = line;
                    }
                    llines = llines.HRemoveAll("").ToList();
                    List<List<string>> groups = FromLines_CollectGroup(llines);

                    Dictionary<int, double>                            idx2eigval    = null;
                    Dictionary<int, double>                            idx2freq      = null;
                    Dictionary<int,Tuple<double, Tuple<int,Vector>[]>> idx2freq_mode = new Dictionary<int,Tuple<double,Tuple<int,Vector>[]>>();
                    foreach(List<string> group in groups)
                    {
                        string header = group[0];

                        if(header.Contains("Eigenvalues"))
                        {
                            idx2eigval = FromLines_GetKeyValue(group);
                            continue;
                        }
                        if(header.Contains("Frequencies"))
                        {
                            idx2freq = FromLines_GetKeyValue(group);
                            foreach(int idx in idx2freq.Keys)
                                idx2freq_mode.Add(idx, null);
                            continue;
                        }
                        if(header.Contains("Mode"       ))
                        {
                            Tuple<int,Tuple<double,Tuple<int,Vector>[]>> idx_freq_mode = FromLines_GetMode(group);
                            int                  idx      = idx_freq_mode.Item1;
                            double               freq     = idx_freq_mode.Item2.Item1;
                            Tuple<int, Vector>[] modevecs = idx_freq_mode.Item2.Item2;
                            if(modevecs.Length*3 != idx2freq.Count)
                                throw new Exception("mode vector size is not matching");
                            HDebug.Assert(idx2freq != null);
                            HDebug.Assert(idx2freq[idx] == freq);
                            if(idx2freq_mode.ContainsKey(idx) == false)
                                throw new Exception("idx2freq_mode.ContainsKey(idx) == false");
                            if(idx2freq_mode[idx] != null)
                                throw new Exception("double assign idx2freq_mode[idx]");
                            HDebug.Assert(idx2freq_mode[idx] == null);
                            idx2freq_mode[idx] = idx_freq_mode.Item2;
                            continue;
                        }
                    }

                    foreach(int idx in idx2freq_mode.Keys)
                    {
                        if(idx2freq_mode[idx] == null)
                            throw new Exception("(idx2freq_mode[idx] == null) ==> exist not-assigned mode vector");
                    }

                    Vibrate vibrate = new Vibrate();
                    vibrate.idx2eigval    = idx2eigval;
                    vibrate.idx2freq      = idx2freq;
                    vibrate.idx2freq_mode = idx2freq_mode;

                    return vibrate;
                }
                catch(Exception)
                {
                    HDebug.Assert(false);
                    return null;
                }
            }
        }
        public static List<List<string>> FromLines_CollectGroup(IList<string> lines)
        {
            List<List<string>> groups = new List<List<string>>();
            for(int i=0; i<lines.Count; i++)
            {
                string line = lines[i];
                HDebug.Assert(line[0] == ' ');
                if(line[1] != ' ' && "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(line[2]) != -1)
                    groups.Add(new List<string>());
                groups.Last().Add(line);
            }
            return groups;
        }
        public static Dictionary<int, double> FromLines_GetKeyValue(IList<string> lines)
        {
            Dictionary<int, double> keyvals = new Dictionary<int,double>();
            for(int idx=1; idx<lines.Count; idx++)
            {
                string line = lines[idx];
                string[] tokens = line.Split().HRemoveAll("");
                HDebug.Assert(tokens.Length % 2 == 0);
                for(int i=0; i<tokens.Length; i+=2)
                {
                    int    key = int   .Parse(tokens[i+0]);
                    double val = double.Parse(tokens[i+1]);
                    keyvals.Add(key, val);
                }
            }
            return keyvals;
        }
        public static Tuple<int, Tuple<double, Tuple<int,Vector>[]>> FromLines_GetMode(IList<string> lines)
        {
            int    modeidx ;
            double modefreq;
            {
                string header = lines[0];
                string[] tokens = header.Split(new string[] {" ", "Vibrational", "Normal", "Mode", "with", "Frequency", "cm-1"}, StringSplitOptions.RemoveEmptyEntries);
                HDebug.Assert(tokens.Length == 2);
                modeidx  = int   .Parse(tokens[0]);
                modefreq = double.Parse(tokens[1]);
            }
            List<Tuple<int, Vector>> modevecs = new List<Tuple<int, Vector>>();
            for(int i=1; i<lines.Count; i++)
            {
                string line = lines[i];
                string[] tokens = line.Split(new string[] {" ","Atom","Delta X","Delta Y","Delta Z"}, StringSplitOptions.RemoveEmptyEntries);
                if(tokens.Length < 4)
                    continue;
                int    atom   = int   .Parse(tokens[0]);
                double deltax = double.Parse(tokens[1]);
                double deltay = double.Parse(tokens[2]);
                double deltaz = double.Parse(tokens[3]);
                Vector delta = new double[3] {deltax, deltay, deltaz};
                modevecs.Add(new Tuple<int, Vector>(atom, delta));
            }
            return new Tuple<int, Tuple<double, Tuple<int, Vector>[]>>(modeidx, new Tuple<double, Tuple<int, Vector>[]>(modefreq, modevecs.ToArray()));
        }
        //public static List<string> FromLines_ExtractLinesFrequencies(ref List<string> lines)
        //{
        //    return null;
        //}
        //public static List<string> FromLines_ExtractLinesModes(ref List<string> lines)
        //{
        //    return null;
        //}
    }
}
