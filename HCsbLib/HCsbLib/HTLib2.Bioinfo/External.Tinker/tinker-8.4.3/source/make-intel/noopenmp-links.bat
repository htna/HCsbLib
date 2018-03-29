mkdir noopenmp-bin
cd noopenmp-bin

@echo off
rem
rem
rem  ###############################################################
rem  ##                                                           ##
rem  ##  links.bat  --  link each of the TINKER package programs  ##
rem  ##        (Intel Fortran Compiler for Windows Version)       ##
rem  ##                                                           ##
rem  ###############################################################
rem
rem
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/alchemy.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/analyze.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/anneal.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/archive.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/bar.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/correlate.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/crystal.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/diffuse.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/distgeom.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/document.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/dynamic.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/gda.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/intedit.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/intxyz.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/minimize.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/minirot.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/minrigid.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/molxyz.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/monte.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/newton.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/newtrot.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/nucleic.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/optimize.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/optirot.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/optrigid.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/path.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/pdbxyz.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/polarize.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/poledit.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/potential.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/prmedit.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/protein.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/pss.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/pssrigid.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/pssrot.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/radial.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/saddle.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/scan.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/sniffer.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/spacefill.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/spectrum.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/superpose.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/sybylxyz.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/testgrad.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/testhess.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/testpair.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/testpol.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/testrot.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/timer.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/timerot.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/torsfit.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/valence.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/vibbig.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/vibrate.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/vibrot.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/xtalfit.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/xtalmin.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/xyzedit.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/xyzint.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/xyzpdb.obj ../noopenmp-obj/tinker.lib
ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/xyzsybyl.obj ../noopenmp-obj/tinker.lib

ifort /O3 /Qprec-div- /recursive /libs:static ../noopenmp-obj/testhesssparse-htna.obj ../noopenmp-obj/tinker.lib

cd ..
