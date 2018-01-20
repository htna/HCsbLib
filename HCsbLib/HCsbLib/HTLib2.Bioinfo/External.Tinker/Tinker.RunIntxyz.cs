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
            public static Xyz Intxyz(string intpath, string tempbase=null)
            {
                string[] intlines = HFile.ReadAllLines(intpath);
                Xyz xyz;
                var tmpdir = HDirectory.CreateTempDirectory(tempbase);
                string currpath = HEnvironment.CurrentDirectory;
                {
                    HEnvironment.CurrentDirectory = tmpdir.FullName;
                    HFile.WriteAllLines("test.int", intlines);
                    Intxyz("test.int");
                    xyz = Xyz.FromFile("test.xyz", true);
                }
                HEnvironment.CurrentDirectory = currpath;
                try{ tmpdir.Delete(true); } catch {}
                return xyz;
            }
            public static void Intxyz(string intpath)
            {
                string command = "";
                command += GetProgramPath("intxyz.exe");
                command += " " + intpath;
                int exitcode = HProcess.StartAsBatchSilent(null, null, null, command);
            }
        }
    }
}
