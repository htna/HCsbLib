using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HTLib2.Bioinfo
{
    using FileInfo = System.IO.FileInfo;
    using DirectoryInfo = System.IO.DirectoryInfo;
    public partial class Gromacs
    {
        public partial class Top
        {
            public static Top FromPdb( string pdbpath
                                     , string ff    = "charmm27"
                                     , string water = "none"
                                     , HPack<Pdb> optout_confout=null
                                     )
            {
                Top top;

                FileInfo pdbinfo = new FileInfo(pdbpath);
                string currdir = HEnvironment.CurrentDirectory;
                DirectoryInfo temproot = HDirectory.CreateTempDirectory();
                HEnvironment.CurrentDirectory = temproot.FullName;
                {
                    HFile.Copy(pdbinfo.FullName, "protein.pdb");

                    RunPdb2gmx( f: "protein.pdb"
                              , o: "confout.pdb"
                              , p: "topol.top"
                              , q: "clean.pdb"
                              , ff: ff
                              , water: water
                              , merge: "all"
                              , lineStderr:null
                              , lineStdout:null
                              );

                    top = Top.FromFile("topol.top");
                    if(optout_confout != null)
                        optout_confout.value = Pdb.FromFile("confout.pdb");
                }
                HEnvironment.CurrentDirectory = currdir;
                Thread.Sleep(100);
                try{ temproot.Delete(true); } catch(Exception) {}

                return top;
            }
        }
    }
}
