gfortran -c -O -o ./o/htnaWriteHess.o   ../htnaWriteHess.f
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/htnaWriteHess.exe  ./o/htnaWriteHess.o  ./a/libtinker.a
copy /y ".\x\htnaWriteHess.exe" "C:\Users\htna\svn\htnasvn_htna\Research.bioinfo.prog.TINKER\1A6G\htnaWriteHess.exe"

gfortran -c -O -o ./o/htnaWriteGrad.o   ../htnaWriteGrad.f
gfortran -Wl,--export-all-symbols -static-libgcc -static-libgfortran -Wl,--export-all-symbols -o ./x/htnaWriteGrad.exe  ./o/htnaWriteGrad.o  ./a/libtinker.a
copy /y ".\x\htnaWriteGrad.exe" "C:\Users\htna\svn\htnasvn_htna\Research.bioinfo.prog.TINKER\1A6G\htnaWriteGrad.exe"
