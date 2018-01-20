using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Namd
    {
        public static Run.CNamd2 RunNamd2
            ( IList<string> pdb_lines
            , IList<string> psf_lines
            , IList<string> prm_lines
            , string tempbase           // = null
            , string namd2version       // = "2.8"
            , string option             // = "+p3"
            , IList<string> infiles     = null
            , IList<string> outfiles    = null
            , IList<string> conf_lines  = null
            )
        {
            return Run.Namd2
            ( pdb_lines
            , psf_lines
            , prm_lines
            , tempbase
            , namd2version
            , option
            , infiles
            , outfiles
            , conf_lines
            );
        }
        public partial class Run
        {
            public class CNamd2
            {
                public string[] coor_lines; public Pdb coor { get { return Pdb.FromLines(coor_lines); } }
                    /// coor is the same to pdb
                public double coor_rmsd;    // rmsd between initial conformation and minimized conformation
            };

            public static CNamd2 Namd2
                ( IList<string> pdb_lines
                , IList<string> psf_lines
                , IList<string> prm_lines
                , string tempbase           // = null
                , string namd2version       // = "2.8"
                , string option             // = "+p3"
                , IList<string> infiles     = null
                , IList<string> outfiles    = null
                , IList<string> conf_lines  = null
                )
            {
                string[] lines = null;
                if((conf_lines != null) && (conf_lines.Count > 0))
                {
                    lines = conf_lines.ToArray();
                }
                else
                {
                    // http://www.msg.ucsf.edu/local/programs/Vega/pages/tu_namdmin.htm
                    lines = new string[]
                    { "numsteps                10000              " // minimization steps
                    //{ "numsteps                1000               " // minimization steps
                    , "minimization            on                 "
                    , "dielectric              1.0                "
                    , "coordinates             prot.pdb           " // coordinate file
                    , "outputname              output             " // output file: prot.coor
                    , "outputEnergies          1000               "
                    , "binaryoutput            no                 "
                    , "DCDFreq                 1000               "
                    , "restartFreq             1000               "
                    , "structure               prot.psf           " // psf file
                    , "paraTypeCharmm          on                 "
                    , "parameters              prot.prm           " // parameter file
                    //, "parameters              par_all22_vega.inp "
                    , "exclude                 scaled1-4          "
                    , "1-4scaling              1.0                "
                    , "switching               on                 "
                    , "switchdist              8.0                "
                    , "cutoff                  22.0               " //, "cutoff                  12.0               "
                    , "pairlistdist            23.5               " //, "pairlistdist            13.5               "
                    , "margin                  0.0                "
                    , "stepspercycle           20                 "
                    , "fixedAtoms              on                 " // fix atoms
                    , "fixedAtomsCol           O                  " // select fixing atoms from O
                    };
                }

                var tmpdir = HDirectory.CreateTempDirectory(tempbase);
                string currpath = HEnvironment.CurrentDirectory;
                HEnvironment.CurrentDirectory = tmpdir.FullName;
                string[] coor_lines = null;
                double   coor_rmsd = double.NaN;
                {
                    {
                        //foreach(var respath_filename in GetResourcePaths("2.8", "psfgen"))
                        foreach(var respath_filename in GetResourcePaths(namd2version, "namd2"))
                        {
                            string respath  = respath_filename.Item1;
                            string filename = respath_filename.Item2;
                            HResource.CopyResourceTo<Tinker>(respath, filename);
                        }
                    }

                    Vector[] coords0 = Pdb.FromLines(pdb_lines).atoms.ListCoord().ToArray();
                    System.IO.File.WriteAllLines("prot.pdb", pdb_lines);
                    System.IO.File.WriteAllLines("prot.psf", psf_lines);
                    System.IO.File.WriteAllLines("prot.prm", prm_lines);
                    System.IO.File.WriteAllLines("prot.conf", lines);

                    if(option == null) option = "";
                    string command = string.Format("namd2 {0} prot.conf", option);
                    HProcess.StartAsBatchInConsole(null, false, command);

                    if(HFile.Exists("output.coor"))
                    {
                        coor_lines = HFile.ReadAllLines("output.coor");
                        Vector[] coords1  = Pdb.FromLines(coor_lines).atoms.ListCoord().ToArray();
                        Vector[] coords1x = Align.MinRMSD.Align(coords0, coords1);
                        coor_rmsd         = Align.MinRMSD.GetRMSD(coords0, coords1x);
                    }
                }
                HEnvironment.CurrentDirectory = currpath;
                try{ tmpdir.Delete(true); } catch {}

                if(coor_lines == null)
                    return null;

                return new CNamd2
                {
                    coor_lines = coor_lines,
                    coor_rmsd  = coor_rmsd,
                };
            }
        }
    }
}
