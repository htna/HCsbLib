using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Charmm
    {
        /// https://www.charmmtutorial.org/index.php/Full_example

        /// The first input script to be used is setup.inp (http://www.charmmtutorial.org/images/7/75/Setup.inp),
        /// which reads the sequence and coordinates from the PDB file and creates the other data structures
        /// needed by CHARMM. Most of this script should be reasonably straightforward, however there are a couple
        /// of commands that have not been seen previously:
        /// 
        /// * READ SEQUence tells CHARMM to read the sequence for the macromolecule from the file. For example,
        ///   the sequence "ALA ALA" would refer to an alanine dipeptide.
        /// * The GENErate command is used to create a new segment in the protein structure file. In this script,
        ///   we do not read in a PSF, so the GENErate command effectively creates the PSF.
        ///     * The SETUp option to GENErate tells CHARMM to create the internal coordinate (IC) tables.
        ///     * a-pro is the identifier we give to the segment that we are generating (this is called the SEGID
        ///       and is limited to 5 characters).
        ///     * first nter last cter determine the terminal group patchings for the segments. NTER and CTER are
        ///       the defaults for proteins and refer to the capping at each end of the peptide chain.
        /// * The REWInd command tells CHARMM to begin reading a file again from the beginning (handy when you want
        ///   to read two types of data from a file as we do here)!
        /// * The IC commands in this file tell CHARMM to fill in missing data in the internal coordinate table with
        ///   information from the parameter sets (bonds, angles, etc. being placed at their minimum). This data can
        ///   then be used to build cartesian coordinates. However, since the PDB file already has cartesian
        ///   coordinates for all of the atoms except the hydrogens, it is not really needed here (but it does not
        ///   hurt to have it).
        /// * The HBUIld command places the hydrogens relative to the heavy atom positions. Most PDB files do not
        ///   have hydrogen positions, so this command is used within CHARMM to build them. Note that the hydrogen
        ///   positions may not be optimal, so it is best to minimize these following the set-up.
        /// 
        /// Once we have all of the coordinates in place, we go ahead and save out our newly created PSF and coordinate files.

        public static SPsfCrd BuildPsfCrd
            ( IEnumerable<string> toplines
            , IEnumerable<string> parlines
            , IEnumerable<Pdb.IAtom> pdbatoms
            )
        {
            List<string> pdblines = new List<string>(pdbatoms.Count());
            foreach(var atom in pdbatoms)
            {
                string pdbline;
                if(atom.resName == "HIS")
                    pdbline = atom.GetUpdatedLineResName("HSD");
                else
                    pdbline = atom.line;
                pdblines.Add(pdbline);
            }
            return BuildPsfCrd(toplines, parlines, pdblines);
        }

        public static SPsfCrd BuildPsfCrd
            ( IEnumerable<string> toplines
            , IEnumerable<string> parlines
            , IEnumerable<string> pdblines
            )
        {
            string tempbase = @"C:\temp\";

            SPsfCrd psfcrd;

            using(var temp = new HTempDirectory(tempbase, null))
            {
                temp.EnterTemp();
                string topname = "top.rtf"; HFile.WriteAllLines(topname, toplines);
                string parname = "par.prm"; HFile.WriteAllLines(parname, parlines);

                string tgtname = "target" ; HFile.WriteAllLines(tgtname+".pdb", pdblines);

                string Setup_inp = @"* Run Segment Through CHARMM
*

! read topology and parameter files

read rtf  card name $$topname$$
read para card name $$parname$$

! Read sequence from the PDB coordinate file
open unit 1 card read name $$tgtname$$.pdb
read sequ pdb unit 1

! now generate the PSF and also the IC table (SETU keyword)
generate setu a-pro first NTER last CTER

rewind unit 1

! set bomlev to -1 to avois sying on lack of hydrogen coordinates
bomlev -1
read coor pdb unit 1
! them put bomlev back up to 0
bomlev 0

close unit 1

! prints out number of atoms that still have undefined coordinates.
define test select segid a-pro .and. ( .not. hydrogen ) .and. ( .not. init ) show end

ic para
ic fill preserve
ic build
hbuild sele all end

! write out the protein structure file (psf) and
! the coordinate file in pdb and crd format.

write psf card name $$tgtname$$.psf
* PSF
*

write coor card name $$tgtname$$.crd
* Coords
*

stop

"
                    .Replace("$$topname$$",  topname)
                    .Replace("$$parname$$",  parname)
                    .Replace("$$tgtname$$",  tgtname);
                HFile.WriteAllText("Setup.inp", Setup_inp);

                System.Console.WriteLine("Run the following command at " + temp + " :");
                System.Console.WriteLine("      $ charmm < Setup.inp");
                System.Console.WriteLine("   or $ mpd&");
                System.Console.WriteLine("      $ mpirun -n 38 charmm_M < Setup.inp");
                System.Console.WriteLine("Then, copy target.psf and target.crd to " + temp);

                while(true)
                {
                    bool next = HConsole.ReadValue<bool>("Ready for next? ", false, null, false, true);
                    if(next)
                    {
                        if(HFile.ExistsAll("target.crd", "target.psf"))
                        {
                            string[] crdlines = HFile.ReadAllLines("target.crd"); 
                            string[] psflines = HFile.ReadAllLines("target.psf");
                            psfcrd = new SPsfCrd
                            {
                                crdlines = crdlines,
                                psflines = psflines,
                            };
                            break;
                        }
                        System.Console.WriteLine("DO, copy target.psf and target.crd to " + temp);
                    }
                }

                temp.QuitTemp();
            }

            return psfcrd;
        }
    }
}
