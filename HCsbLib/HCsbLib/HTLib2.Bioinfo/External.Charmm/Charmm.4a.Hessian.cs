using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Charmm
    {
        /// https://www.charmmtutorial.org/index.php/Full_example

        public static string[] Hessian
            ( IEnumerable<string> toplines
            , IEnumerable<string> parlines
            , IEnumerable<string> psflines
            , IEnumerable<string> crdlines
            )
        {
            string tempbase = @"C:\temp\";

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
* Minimize PDB
*

! read topology and parameter file
read rtf   card name top.rtf
read param card name par.prm
! read the psf and coordinate file
read psf  card name target.psf
read coor card name target.crd

! set up shake
shake bonh param sele all end

! set up electrostatics, since we're not using PME, set up switching
! electrostatics
nbond inbfrq -1 elec fswitch vdw vswitch cutnb 16. ctofnb 12. ctonnb 10.

energy

coor copy comp


! mini sd nstep 100
! mini abnr nstep 1000 nprint 10000 tolg 0.0000
! ! mini abnr nstep 1000 nprint 100 tolg 0.01
! ! mini abnr nstep 10000000 nprint 100 tolg 0.00
! ! mini abnr nstep   1000000  nprint 100 tolg 10.00    ! target-min-step1-tolg-10_00.crd
! ! mini abnr nstep   1000000  nprint 100 tolg 1.00     ! target-min-step2-tolg-1_00.crd
! ! mini abnr nstep   1000000  nprint 100 tolg 0.10
! ! mini sd nstep 10000                                 ! target-min-step3-sd10000.crd
! ! mini abnr nstep   1000000  nprint 100 tolg 0.01     ! target-min-step4-tolg-0_01.crd
! ! mini abnr nstep   10000  nprint 100 tolg 0.0001     ! target-min-step5-tolg-0_0001.crd
! ! mini abnr nstep   10000  nprint 100 tolg 0.000001   ! target-min-step6-tolg-0_000001.crd
! ! mini abnr nstep   10000  nprint 100 tolg 0.000000     ! target-min-step7-tolg-0_000000.crd
! 
! 
! coor rms
! 
! ioform extended
! 
! write coor card name target-minimized.crd
! * Initial minimization, no PME.
! *

! VIBRAN NMOD 20 ATOM FSWITCH rdie eps 4.0 VDW VSHIFT cutnb 13.0 ctofnb 12.0 CTONNB 8.0
! 
! DIMB ITERations 500 TOLErance 0.04 PARDim 200 IUNMode 21 DWIN
! 
! WRITe SECOnd-derivatives card

! BOMLEV -2
! VIBRAN
! DIAG ENTRopy

! VIBRan NMODes 500
! DIAG
! print norm vector dipole

!mini abnr nstep   100  nprint 100 tolg 0.000000     ! target-min-step7-tolg-0_000000.crd
BOMLEV -2
open unit 1 write form name " +" \"second.dat\" " +@" 
REDUce CMPAct
vibran
!diag
write second card unit 1
close unit 1

!calc natom3 ?NATOM *3
!vibran nmode @natom3
!diag
!print norm

!bomlev - 2
!VIBRan NMOD 300
!DIAGonalize


!https://www.charmm.org/charmm/documentation/by-version/c42b1/params/doc/vibran/

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
                System.Console.WriteLine("Then, copy second.dat to " + temp);

                while(true)
                {
                    bool next = HConsole.ReadValue<bool>("Ready for next? ", false, null, false, true);
                    if(next)
                    {
                        if(HFile.ExistsAll(mincrdname))
                        {
                            mincrdlines = HFile.ReadAllLines(mincrdname);
                            /// second.dat has the following format
                            /// 
                            /// num-atoms
                            /// pot-energy
                            /// atom1-xforce  atom1-yforce atom1-zforce
                            /// atom2-xforce  atom2-yforce atom2-zforce
                            ///         ...
                            /// atomn-xforce  atomn-yforce atomn-zforce
                            /// upper- or lower-diag elem 1
                            /// upper- or lower-diag elem 2
                            ///   ....
                            /// upper- or lower-diag elem 3n*3n/2
                            /// atom1-xcoord  atom1-ycoord atom1-zcoord
                            /// atom2-xcoord  atom2-ycoord atom2-zcoord
                            ///         ...
                            /// atomn-xcoord  atomn-ycoord atomn-zcoord
                            /// 
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
