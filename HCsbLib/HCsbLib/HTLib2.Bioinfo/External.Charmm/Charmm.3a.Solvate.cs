using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Charmm
    {
        /// https://www.charmmtutorial.org/index.php/Full_example

        public static SPsfCrd Solvate
            ( IEnumerable<string> toplines
            , IEnumerable<string> parlines
            , IEnumerable<string> psflines_prot
            , IEnumerable<string> crdlines_prot
            , IEnumerable<string> crdlines_water
            )
        {
            string tempbase = @"C:\temp\";

            SPsfCrd psfcrd;

            using(var temp = new HTempDirectory(tempbase, null))
            {
                temp.EnterTemp();
                string topname       = "top.rtf"  ; HFile.WriteAllLines(topname      , toplines      );
                string parname       = "par.prm"  ; HFile.WriteAllLines(parname      , parlines      );
                string psfname_prot  = "prot.psf" ; HFile.WriteAllLines(psfname_prot , psflines_prot );
                string crdname_prot  = "prot.crd" ; HFile.WriteAllLines(crdname_prot , crdlines_prot );
                string crdname_water = "water.crd"; HFile.WriteAllLines(crdname_water, crdlines_water);
                string psfname_protwater = "protwater.psf";
                string crdname_protwater = "protwater.crd";

                string conf_inp = @"* Run Segment Through CHARMM
*

! read topology and parameter files
read rtf  card name $$topname$$
read para card name $$parname$$

! read the psf and coordinate file
read psf  card name $$psfname_prot$$
read coor card name $$crdname_prot$$


! Read in water sequence
read sequence tip3 46656

! Generate new segment for the water
generate bwat noangle nodihedral

! Read the water PDB coordinates and append them to the protein
read coor card append name $$crdname_water$$

! Delete waters which overlap with protein
delete atom sort -
select .byres. (segid bwat .AND. type oh2 .and. -
((.not. (segid bwat .OR. hydrogen)) .around. 2.5)) end

! set headstr = rhdo with a crystal dimension of @greatervalue
set headstr = test xxx

! we want to do a quick-and-dirty minimization to remove bad contacts. Therefore, we should
! set up shake and the non-bond parameters again.
shake bonh param sele all end
nbond inbfrq -1 elec fswitch vdw vswitch cutnb 16. ctofnb 12. ctonnb 10.
mini sd nstep 100 nprint 1 tolgrd 100.0

! use Expanded I/O format
ioform extended

! since we've changed the structure by adding waters, we need to write out a new PSF
write psf card name $$psfname_protwater$$
* new_1cbn-18126-1-solv.psf
* solvation: @headstr
*

write coor card name $$crdname_protwater$$
* new_1cbn-18126-1-solv.crd
* solvation: @headstr
*

stop

"                   .Replace("$$topname$$", topname)
                    .Replace("$$parname$$", parname)
                    .Replace("$$psfname_prot$$", psfname_prot)
                    .Replace("$$crdname_prot$$", crdname_prot)
                    .Replace("$$crdname_water$$", crdname_water)
                    .Replace("$$psfname_protwater$$", psfname_protwater)
                    .Replace("$$crdname_protwater$$", crdname_protwater)
                    ;
                HFile.WriteAllText("conf.inp", conf_inp);

                System.Console.WriteLine("Run the following command at " + temp + " :");
                System.Console.WriteLine("      $ charmm < conf.inp");
                System.Console.WriteLine("   or $ mpd&");
                System.Console.WriteLine("      $ mpirun -n 38 charmm_M < conf.inp");
                System.Console.WriteLine("Then, copy " + psfname_protwater + " and " + crdname_protwater + " to " + temp);

                while(true)
                {
                    bool next = HConsole.ReadValue<bool>("Ready for next? ", false, null, false, true);
                    if(next)
                    {
                        if(HFile.ExistsAll(psfname_protwater, crdname_protwater))
                        {
                            string[] psflines_protwater = HFile.ReadAllLines(psfname_protwater);
                            string[] crdlines_protwater = HFile.ReadAllLines(crdname_protwater);
                            psfcrd = new SPsfCrd
                            {
                                psflines = psflines_protwater,
                                crdlines = crdlines_protwater,
                            };
                            break;
                        }
                        System.Console.WriteLine("DO, copy "+ psfname_protwater + " and "+ crdname_protwater + " to " + temp);
                    }
                }

                temp.QuitTemp();
            }
            return psfcrd;
        }
    }
}
