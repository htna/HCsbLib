using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    //  Cij = <ri.rj> / (<ri.ri> <rj.rj>)^0.5
    //  Cab = <ra.rb> / (<ra.ra> <rb.rb>)^0.5
    //      where <x> is mean of x
    //            a.b is dot product of a and b
    //
    //  Dij = Table[0, {i,Length[idxs]}, {j,Length[idxs]}];
    //  Dii = Table[0, {i,Length[idxs]}];
    //  For[i=1, i<=nmodes, i++,
    //      modei = HMode[i];
    //      eigvi = McorrEigval[i];
    //      Dij = Dij + modei.Transpose[modei]/eigvi;
    //      Dii = Dii + Table[Dot[vec,vec],{vec,modei}]/eigvi;
    //      ];
    //  Dij = Dij / nmodes;
    //  Dii = Dii / nmodes;
    //  Cij = Dij / Sqrt[Transpose[{Dii}].{Dii}];
    //  corr = Cij;

    public static partial class ModeStatic
    {
        public static bool GetCorrMatrix_SelfTest(IList<Mode> modes, Func<IList<Mode>, Matrix> fnGetCorrMatrix)
        {
            //  nmodes=3;
            //  idxs={1,2,3,4};
            //  HMode[1] = {{1,2,3},{2,3,4},{3,4,5},{4,5,6}}; McorrEigval[1] = 2;
            //  HMode[2] = {{2,3,4},{3,4,5},{4,5,6},{5,6,7}}; McorrEigval[2] = 3;
            //  HMode[3] = {{3,4,5},{4,5,6},{5,6,7},{6,7,8}}; McorrEigval[3] = 5;
            //  Dij = Table[0, {i,Length[idxs]}, {j,Length[idxs]}];
            //  Dii = Table[0, {i,Length[idxs]}];
            //  For[i=1, i<=nmodes, i++,
            //      modei = HMode[i];
            //      eigvi = McorrEigval[i];
            //      Dij = Dij + modei.Transpose[modei]/eigvi;
            //      Dii = Dii + Table[Dot[vec,vec],{vec,modei}]/eigvi;
            //      ];
            //  Dij = Dij / nmodes;
            //  Dii = Dii / nmodes;
            //  Cij = Dij / Sqrt[Transpose[{Dii}].{Dii}];
            //  corr = Cij;
            //
            //  corr    : {{1, 263/(5 Sqrt[2794]), 163/(5 Sqrt[1090]), 389/(5 Sqrt[6298])}, {263/(5 Sqrt[2794]), 1, 871/Sqrt[761365], 2087/Sqrt[4399153]}, {163/(5 Sqrt[1090]), 871/Sqrt[761365], 1, 1309/Sqrt[1716205]}, {389/(5 Sqrt[6298]), 2087/Sqrt[4399153], 1309/Sqrt[1716205], 1}}
            //  N[corr] : {{1., 0.995113, 0.987426, 0.980343}, {0.995113, 1., 0.99821, 0.995034}, {0.987426, 0.99821, 1., 0.999206}, {0.980343, 0.995034, 0.999206, 1.}}
            Mode[] tmodes = new Mode[]
            {
                new Mode { eigvec=new double[] {1,2,3, 2,3,4, 3,4,5, 4,5,6}, eigval=2 },
                new Mode { eigvec=new double[] {2,3,4, 3,4,5, 4,5,6, 5,6,7}, eigval=3 },
                new Mode { eigvec=new double[] {3,4,5, 4,5,6, 5,6,7, 6,7,8}, eigval=5 },
            };
            Matrix tcorr0 = new double[,]
            { {1.000000, 0.995113, 0.987426, 0.980343}
            , {0.995113, 1.000000, 0.99821 , 0.995034}
            , {0.987426, 0.99821 , 1.000000, 0.999206}
            , {0.980343, 0.995034, 0.999206, 1.000000}
            };
            tcorr0 = new double[,]
            { {1.0                    ,  263/(5*Math.Sqrt(2794)),  163/(5*Math.Sqrt(1090)), 389/(5*Math.Sqrt(6298))}
            , {263/(5*Math.Sqrt(2794)), 1.0                     ,  871/Math.Sqrt( 761365 ), 2087/Math.Sqrt(4399153)}
            , {163/(5*Math.Sqrt(1090)),  871/Math.Sqrt( 761365 ), 1.0                     , 1309/Math.Sqrt(1716205)}
            , {389/(5*Math.Sqrt(6298)), 2087/Math.Sqrt(4399153 ), 1309/Math.Sqrt(1716205 ), 1.0                    }
            };

            Matrix tcorrx = fnGetCorrMatrix(tmodes);
            Matrix dtcorr = tcorrx - tcorr0;
            if(dtcorr.HAbsMax() < 0.00001)
                return true;
            return false;
        }
        public static Matrix GetCorrMatrix(this IList<Mode> modes)
        {
            return GetCorrMatrix(modes, false);
        }
        public static Matrix GetCorrMatrix(this IList<Mode> modes, bool useAbsEigval)
        {
            if(HDebug.Selftest())
                HDebug.Assert(GetCorrMatrix_SelfTest(modes, GetCorrMatrix));
            
            Vector[][] eigvecs = new Vector[modes.Count][];
            double[]   eigvals = new double[modes.Count];
            for(int i=0; i<modes.Count; i++)
            {
                eigvals[i] = modes[i].eigval;
                eigvecs[i] = modes[i].GetEigvecsOfAtoms();
                if(useAbsEigval)
                    eigvals[i] = Math.Abs(eigvals[i]);
            }

            int size = modes[0].size;
            Matrix Dij = Matrix.Zeros(size, size);
            Vector Dii = new double[size];

            for(int i=0; i<eigvals.Length; i++)
            {
                Vector[] eigvec = eigvecs[i];
                double   eigval = eigvals[i];
                for(int c=0; c<size; c++)
                {
                    {
                        /// for(int r=0; r<size; r++)
                        ///     Dij[c, r] += (LinAlg.VtV(eigvec[c], eigvec[r]) / eigval);
                        Dij[c, c] += (LinAlg.VtV(eigvec[c], eigvec[c]) / eigval);
                        for(int r=c+1; r<size; r++)
                        {
                            double dDij_cr = (LinAlg.VtV(eigvec[c], eigvec[r]) / eigval);;
                            Dij[c, r] += dDij_cr;
                            Dij[r, c] += dDij_cr;
                        }
                    }
                    Dii[c] += (LinAlg.VtV(eigvec[c], eigvec[c]) / eigval);
                }
            }
            Dij = Dij / modes.Count;
            Dii = Dii / modes.Count;

            double[] sqrt_Dii = Dii.ToArray().HSqrt();
            // Cij
            for(int c=0; c<size; c++)
                for(int r=0; r<size; r++)
                    //Dij[c, r] /= Math.Sqrt(Dii[c] * Dii[r]);
                    Dij[c, r] /= (sqrt_Dii[c] * sqrt_Dii[r]);

            return Dij;
        }
        public static MatrixSparse<double> GetCorrMatrix(this IList<Mode> modes, IEnumerable<Tuple<int, int>> enumCorrCR)
        {
            Vector[][] eigvecs = new Vector[modes.Count][];
            double[]   eigvals = new double[modes.Count];
            for(int i=0; i<modes.Count; i++)
            {
                eigvals[i] = modes[i].eigval;
                eigvecs[i] = modes[i].GetEigvecsOfAtoms();
            }

            int size = modes[0].size;
            MatrixSparse<double> Dij = new MatrixSparse<double>(size, size);
            Vector               Dii = new double[size];

            for(int i=0; i<eigvals.Length; i++)
            {
                Vector[] eigvec = eigvecs[i];
                double   eigval = eigvals[i];

                for(int c=0; c<size; c++)
                    Dii[c] += (LinAlg.VtV(eigvec[c], eigvec[c]) / eigval);
                foreach(var cr in enumCorrCR)
                {
                    int c = cr.Item1;
                    int r = cr.Item2;
                    Dij[c, r] += (LinAlg.VtV(eigvec[c], eigvec[r]) / eigval);
                }
            }
            foreach(var cr in enumCorrCR)
                Dij[cr] = Dij[cr] / modes.Count;
            Dii = Dii / modes.Count;

            // Cij
            foreach(var cr in enumCorrCR)
            {
                int c = cr.Item1;
                int r = cr.Item2;
                Dij[c, r] /= Math.Sqrt(Dii[c] * Dii[r]);
            }

            if(HDebug.IsDebuggerAttached)
            {
                Matrix tDij = GetCorrMatrix(modes);
                foreach(var cr in enumCorrCR)
                {
                    int c = cr.Item1;
                    int r = cr.Item2;
                    double err = Dij[c, r] - tDij[c, r];
                    HDebug.Exception(Math.Abs(err) < 0.00000001);
                }
            }

            return Dij;
        }
        public static Matrix GetCorrMatrixMatlab(this IList<Mode> modes)
        {
            return GetCorrMatrixMatlab(modes, false);
        }
        public static Matrix GetCorrMatrixMatlab(this IList<Mode> modes, bool useAbsEigval)
        {
            if(HDebug.Selftest())
                HDebug.Assert(GetCorrMatrix_SelfTest(modes, GetCorrMatrixMatlab));

            //  Dii = zeros(n, 1);
            //  For[i=1, i<=nmodes, i++,
            //      Dii = Dii + Table[Dot[vec,vec],{vec,modei}]/eigvi;
            //      ];
            //  Dij = zeros(n, n);
            //  For[i=1, i<=nmodes, i++,
            //      Dijx = Dijx + modei_x.Transpose[modei_x] / eigvi;
            //      Dijy = Dijy + modei_y.Transpose[modei_y] / eigvi;
            //      Dijz = Dijz + modei_z.Transpose[modei_z] / eigvi;
            //      Dii = Dii + Table[Dot[vec,vec],{vec,modei}]/eigvi;
            //      ];
            //  Dij = Dij / nmodes;
            //  Dii = Dii / nmodes;
            //  Cij = Dij / Sqrt[Transpose[{Dii}].{Dii}];
            //  corr = Cij;

            Matrix MD = modes.ListEigvec().ToMatrix();
            Vector EV = modes.ListEigval().ToArray();
            Matrix corrmat;
            using(new Matlab.NamedLock(""))
            {
                Matlab.Clear();
                Matlab.PutMatrix("MD", MD, true);
                Matlab.PutVector("EV", EV);
                if(useAbsEigval)
                    Matlab.Execute("D = abs(D);");
                Matlab.Execute("nmodes = length(EV);");
                Matlab.Execute("iEV   = diag(1 ./ EV);");
                Matlab.Execute("Dijx = MD(1:3:end,:); Dijx = Dijx*iEV*Dijx'; Dij=    Dijx; clear Dijx;");
                Matlab.Execute("Dijy = MD(2:3:end,:); Dijy = Dijy*iEV*Dijy'; Dij=Dij+Dijy; clear Dijy;");
                Matlab.Execute("Dijz = MD(3:3:end,:); Dijz = Dijz*iEV*Dijz'; Dij=Dij+Dijz; clear Dijz;");
                Matlab.Execute("clear MD; clear EV; clear iEV;");
                Matlab.Execute("Dij = Dij / nmodes;");
                Matlab.Execute("Dii = diag(Dij);");
                Matlab.Execute("Dij = Dij ./ sqrt(Dii*Dii');");
                corrmat = Matlab.GetMatrix("Dij", true);
            }
            return corrmat;
        }
    }
}
