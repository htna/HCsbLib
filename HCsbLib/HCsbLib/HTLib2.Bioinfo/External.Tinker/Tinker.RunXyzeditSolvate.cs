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
            public class CXyzedit
            {
                public Tinker.Xyz xyz;
                public string[]   capture;
            };

            public static CXyzedit XyzeditSolvate
                ( string xyzeditpath
                , Tinker.Xyz xyz
                , Tinker.Prm prm
                , Tinker.Xyz solv_box_xyz
                , string tempbase //=null
                , string[] keys //=null
                , string[] parameters
                , int num_Soak_Current_Molecule_in_Box_of_Solvent
                )
            {
                var tmpdir = HDirectory.CreateTempDirectory(tempbase);
                string currpath = HEnvironment.CurrentDirectory;
                Tinker.Xyz xyz_2;
                string[] capture;
                {
                    HEnvironment.CurrentDirectory = tmpdir.FullName;
                    xyz.ToFile("prot.xyz", false);
                    prm.ToFile("prot.prm");
                    solv_box_xyz.ToFile("solv_box.xyz", false);
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
                        command += xyzeditpath;
                        command += " prot.xyz";
                        command += " prot.prm";
                        if(keypath != null) command += " -k "+keypath;
                        if(parameters != null)
                            foreach(string parameter in parameters)
                                command += " " + parameter;
                        command += " " + num_Soak_Current_Molecule_in_Box_of_Solvent;
                        command += " solv_box.xyz";
                        command += " > output.txt";
                        List<string> errors = new List<string>();
                        int exitcode = HProcess.StartAsBatchSilent(null, null, errors, command);
                        capture = HFile.ReadAllLines("output.txt");
                        capture = capture.HAddRange(errors.ToArray());
                        xyz_2 = Tinker.Xyz.FromFile("prot.xyz_2", false);
                    }
                }
                HEnvironment.CurrentDirectory = currpath;
                try{ tmpdir.Delete(true); } catch {}

                return new CXyzedit
                {
                    xyz = xyz_2,
                    capture = capture,
                };
            }
        }
    }
}
