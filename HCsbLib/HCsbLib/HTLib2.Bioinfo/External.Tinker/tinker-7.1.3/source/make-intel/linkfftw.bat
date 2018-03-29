@echo off
rem
rem
rem  ###############################################################
rem  ##                                                           ##
rem  ##  links.bat  --  link each of the TINKER package programs  ##
rem  ##            (Intel Fortran for Windows Version)            ##
rem  ##                                                           ##
rem  ###############################################################
rem
rem
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static alchemy.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static analyze.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static anneal.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static archive.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static bar.obj       tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static correlate.obj tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static crystal.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static diffuse.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static distgeom.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static document.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static dynamic.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static gda.obj       tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static intedit.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static intxyz.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static minimize.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static minirot.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static minrigid.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static molxyz.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static monte.obj     tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static newton.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static newtrot.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static nucleic.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static optimize.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static optirot.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static optrigid.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static path.obj      tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static pdbxyz.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static polarize.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static poledit.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static potential.obj tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static prmedit.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static protein.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static pss.obj       tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static pssrigid.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static pssrot.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static radial.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static saddle.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static scan.obj      tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static sniffer.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static spacefill.obj tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static spectrum.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static superpose.obj tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static sybylxyz.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static testgrad.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static testhess.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static testpair.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static testpol.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static testrot.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static timer.obj     tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static timerot.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static torsfit.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static valence.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static vibbig.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static vibrate.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static vibrot.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static xtalfit.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static xtalmin.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static xyzedit.obj   tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static xyzint.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static xyzpdb.obj    tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static xyzsybyl.obj  tinker.lib ..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib /link /stack:8000000
