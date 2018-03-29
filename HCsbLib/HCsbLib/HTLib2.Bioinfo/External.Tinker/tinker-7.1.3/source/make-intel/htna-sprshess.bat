rem call "C:\Program Files (x86)\Intel\Composer XE 2015\bin\ifortvars.bat" intel64

rem ifort /c /O3 /QxHost /Qip- /Qprec-div- /w /Qopenmp ..\..\source\htna-sprshess.f
rem ifort /O3 /Qprec-div- /Qopenmp /recursive /libs:static htna-sprshess.obj    ..\tinker.lib   ..\..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib   /link /stack:8000000

rem ifort /c /O0 -g -traceback -check all -check bounds -debug all /QxHost /Qip- /Qprec-div- /w /Qopenmp ..\..\source\htna-sprshess.f
rem ifort    /O0 -g -traceback -check all -check bounds -debug all /Qprec-div- /Qopenmp /recursive /libs:static htna-sprshess.obj    ..\tinker.lib   ..\..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib   /link /stack:8000000

ifort /c /O0 -traceback -check -debug /QxHost /Qip- /Qprec-div- /w /Qopenmp ..\..\source\htna-sprshess.f
ifort    /O0 -traceback -check -debug /Qprec-div- /Qopenmp /recursive /libs:static htna-sprshess.obj    ..\tinker.lib   ..\..\..\fftw\fftw-3.3-libs\x64\Static-Release\libfftw-3.3.lib   /link /stack:8000000
