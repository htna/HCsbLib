rem #
rem #
rem #  ###############################################################
rem #  ##                                                           ##
rem #  ##  link.make  --  link each of the TINKER package programs  ##
rem #  ##                  (g95 for Linux Version)                  ##
rem #  ##                                                           ##
rem #  ###############################################################
rem #
rem #
rem -static-libgcc -static-libstdc++ -static-libgfortran -Wl,--export-all-symbols

mkdir x
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/alchemy.exe   ./o/alchemy.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/analyze.exe   ./o/analyze.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/anneal.exe    ./o/anneal.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/archive.exe   ./o/archive.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/correlate.exe ./o/correlate.o ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/crystal.exe   ./o/crystal.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/diffuse.exe   ./o/diffuse.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/distgeom.exe  ./o/distgeom.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/document.exe  ./o/document.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/dynamic.exe   ./o/dynamic.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/gda.exe       ./o/gda.o       ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/intedit.exe   ./o/intedit.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/intxyz.exe    ./o/intxyz.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/minimize.exe  ./o/minimize.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/minirot.exe   ./o/minirot.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/minrigid.exe  ./o/minrigid.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/molxyz.exe    ./o/molxyz.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/monte.exe     ./o/monte.o     ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/newton.exe    ./o/newton.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/newtrot.exe   ./o/newtrot.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/nucleic.exe   ./o/nucleic.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/optimize.exe  ./o/optimize.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/optirot.exe   ./o/optirot.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/optrigid.exe  ./o/optrigid.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/path.exe      ./o/path.o      ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/pdbxyz.exe    ./o/pdbxyz.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/polarize.exe  ./o/polarize.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/poledit.exe   ./o/poledit.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/potential.exe ./o/potential.o ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/prmedit.exe   ./o/prmedit.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/protein.exe   ./o/protein.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/pss.exe       ./o/pss.o       ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/pssrigid.exe  ./o/pssrigid.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/pssrot.exe    ./o/pssrot.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/radial.exe    ./o/radial.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/saddle.exe    ./o/saddle.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/scan.exe      ./o/scan.o      ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/sniffer.exe   ./o/sniffer.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/spacefill.exe ./o/spacefill.o ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/spectrum.exe  ./o/spectrum.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/superpose.exe ./o/superpose.o ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/sybylxyz.exe  ./o/sybylxyz.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/testgrad.exe  ./o/testgrad.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/testhess.exe  ./o/testhess.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/testpair.exe  ./o/testpair.o  ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/testpol.exe   ./o/testpol.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/testrot.exe   ./o/testrot.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/timer.exe     ./o/timer.o     ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/timerot.exe   ./o/timerot.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/torsfit.exe   ./o/torsfit.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/valence.exe   ./o/valence.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/vibbig.exe    ./o/vibbig.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/vibrate.exe   ./o/vibrate.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/vibrot.exe    ./o/vibrot.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/xtalfit.exe   ./o/xtalfit.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/xtalmin.exe   ./o/xtalmin.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/xyzedit.exe   ./o/xyzedit.o   ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/xyzint.exe    ./o/xyzint.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/xyzpdb.exe    ./o/xyzpdb.o    ./a/libtinker.a
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/xyzsybyl.exe  ./o/xyzsybyl.o  ./a/libtinker.a
