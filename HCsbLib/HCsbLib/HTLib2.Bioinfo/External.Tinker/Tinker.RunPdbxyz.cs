using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public partial class Run
        {
            public class CPdbxyz
            {
                public Tinker.Xyz xyz;
                public string[]   seq;
                public string[]   capture;
            };

            public static CPdbxyz Pdbxyz
                ( Pdb pdb
                , Tinker.Prm prm
                , string tempbase //=null
                , string parameters //=null
                , string tinkerversion //="6.2.1"
                , params string[] keys
                )
            {
                var tmpdir = HDirectory.CreateTempDirectory(tempbase);
                string currpath = HEnvironment.CurrentDirectory;
                Tinker.Xyz xyz;
                string[] seq;
                string[] capture;
                {
                    HEnvironment.CurrentDirectory = tmpdir.FullName;
                    {
                        foreach(var respath_filename in GetResourcePaths("6.2.1", "pdbxyz"))
                        {
                            string respath  = respath_filename.Item1;
                            string filename = respath_filename.Item2;
                            HResource.CopyResourceTo<Tinker>(respath, filename);
                        }
                    }
                    pdb.ToFile("prot.pdb");
                    prm.ToFile("prot.prm");
                    string keypath = null;
                    if((keys != null) && (keys.Length > 0))
                    {
                        keypath = "prot.key";
                        HFile.WriteAllLines(keypath, keys);
                    }

                    {
                      //bool ComputeAnalyticalGradientVector    = true;
                      //bool ComputeNumericalGradientVector     = false;
                      //bool OutputBreakdownByGradientComponent = false;
                        string command = "";
                        command += "pdbxyz.exe";
                        command += " prot.pdb";
                        command += " prot.prm";
                        if(keypath != null) command += " -k "+keypath;
                        command += " > output.txt";
                        List<string> errors = new List<string>();
                        int exitcode = HProcess.StartAsBatchSilent(null, null, errors, command);
                        capture = HFile.ReadAllLines("output.txt");
                        capture = capture.HAddRange(errors.ToArray());
                        xyz = Tinker.Xyz.FromFile("prot.xyz", false);

                        if(HFile.Exists("prot.seq")) seq = HFile.ReadAllLines("prot.seq");
                        else                         seq = null;
                    }
                }
                HEnvironment.CurrentDirectory = currpath;
                try{ tmpdir.Delete(true); } catch {}

                return new CPdbxyz
                {
                    xyz = xyz,
                    seq = seq,
                    capture = capture,
                };
            }
        }
    }
}
