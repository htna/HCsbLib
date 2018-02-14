using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Charmm
    {
        /// https://www.charmmtutorial.org/index.php/Full_example

        public static string[] MinimizeCrd
            ( IEnumerable<string> toplines
            , IEnumerable<string> parlines
            , IEnumerable<string> psflines
            , IEnumerable<string> crdlines
            , string conf_inp_mini
            )
        {
            string tempbase = @"C:\temp\";

            if(conf_inp_mini == null)
                conf_inp_mini = @"
mini sd nstep 100
mini abnr nstep 1000 nprint 10000 tolg 0.0001
! mini abnr nstep 1000 nprint 100 tolg 0.01
! mini abnr nstep 10000000 nprint 100 tolg 0.00
! mini abnr nstep   1000000  nprint 100 tolg 10.00    ! target-min-step1-tolg-10_00.crd
! mini abnr nstep   1000000  nprint 100 tolg 1.00     ! target-min-step2-tolg-1_00.crd
! mini abnr nstep   1000000  nprint 100 tolg 0.10
! mini sd nstep 10000                                 ! target-min-step3-sd10000.crd
! mini abnr nstep   1000000  nprint 100 tolg 0.01     ! target-min-step4-tolg-0_01.crd
! mini abnr nstep   10000  nprint 100 tolg 0.0001     ! target-min-step5-tolg-0_0001.crd
! mini abnr nstep   10000  nprint 100 tolg 0.000001   ! target-min-step6-tolg-0_000001.crd
! mini abnr nstep   10000  nprint 100 tolg 0.000000     ! target-min-step7-tolg-0_000000.crd
";

            string[] mincrdlines;

            using(var temp = new HTempDirectory(tempbase, null))
            {
                temp.EnterTemp();
                string topname = "top.rtf"    ; HFile.WriteAllLines(topname, toplines);
                string parname = "par.prm"    ; HFile.WriteAllLines(parname, parlines);
                string psfname = "target.psf" ; HFile.WriteAllLines(psfname, psflines);
                string crdname = "target.crd" ; HFile.WriteAllLines(crdname, crdlines);
                string mincrdname = "target-minimized.crd";

                string conf_inp = @"* Minimize PDB
*

! read topology and parameter file
read rtf   card name $$topname$$
read param card name $$parname$$
! read the psf and coordinate file
read psf  card name $$psfname$$
read coor card name $$crdname$$

! set up shake
shake bonh param sele all end

! set up electrostatics, since we're not using PME, set up switching
! electrostatics
nbond inbfrq -1 elec fswitch vdw vswitch cutnb 16. ctofnb 12. ctonnb 10.

energy

coor copy comp

" + conf_inp_mini + @"

coor rms

ioform extended

write coor card name $$mincrdname$$
* Initial minimization, no PME.
*

stop


";
                conf_inp = conf_inp.Replace("$$topname$$", topname)
                                   .Replace("$$parname$$", parname)
                                   .Replace("$$psfname$$", psfname)
                                   .Replace("$$crdname$$", crdname)
                                   .Replace("$$mincrdname$$", mincrdname)
                                   ;
                HFile.WriteAllText("conf.inp", conf_inp);

                System.Console.WriteLine("Run the following command at " + temp + " :");
                System.Console.WriteLine("      $ charmm < conf.inp");
                System.Console.WriteLine("   or $ mpd&");
                System.Console.WriteLine("      $ mpirun -n 38 charmm_M < conf.inp");
                System.Console.WriteLine("Then, copy target-minimized.crd to " + temp);

                while(true)
                {
                    bool next = HConsole.ReadValue<bool>("Ready for next? ", false, null, false, true);
                    if(next)
                    {
                        if(HFile.ExistsAll(mincrdname))
                        {
                            mincrdlines = HFile.ReadAllLines(mincrdname);
                            break;
                        }
                        System.Console.WriteLine("DO, copy target.psf and target.crd to " + temp);
                    }
                }

                temp.QuitTemp();
            }
            return mincrdlines;
        }
    }
}
