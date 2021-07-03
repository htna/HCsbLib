using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Hess
    {
        public static HessMatrix GetHessByKijFij(IList<Vector> coords, double[,] K, double[,] F, HessMatrix hess=null)
        {
            int size = coords.Count;
            if(hess == null)
                hess = HessMatrix.ZerosHessMatrix(size*3, size*3);
            if(K == null) K = new double[size, size];
            if(F == null) F = new double[size, size];
        /// Parallel.For(0, size, delegate(int i)
            for(int i=0; i<size; i++)
            {
                for(int j=i; j<size; j++)
                {
                    if(i == j)
                        continue;
                    if(K[i, j] == 0 && F[i, j] == 0)
                        continue;

                    MatrixByArr hessij = GetHessByKijFij(coords[i], coords[j], K[i,j], F[i,j]);
                    HDebug.AssertTolerance(0.00000001, K[i,j] - K[j,i]);
                    HDebug.AssertTolerance(0.00000001, F[i,j] - F[j,i]);
                    if(HDebug.IsDebuggerAttached)
                        HDebug.AssertTolerance(0.00000001, hessij - GetHessByKijFij(coords[j], coords[i], K[j,i], F[j,i]));

                    int n0=i*3;
                    int n1=j*3;
                    for(int di=0; di<3; di++)
                        for(int dj=0; dj<3; dj++)
                        {
                            hess[n0+di,n0+dj] -= hessij[di,dj];
                            hess[n0+di,n1+dj] += hessij[di,dj];
                            hess[n1+di,n0+dj] += hessij[di,dj];
                            hess[n1+di,n1+dj] -= hessij[di,dj];
                        }
                }
            }
        /// );
            return hess;
        }
        public static MatrixByArr GetHessByKijFij(Vector coord1, Vector coord2, double Kij, double Fij)
        {
            double dx = coord2[0] - coord1[0];
            double dy = coord2[1] - coord1[1];
            double dz = coord2[2] - coord1[2];
            double dist2 = (dx*dx + dy*dy + dz*dz);
            double dist  = Math.Sqrt(dist2);
            MatrixByArr Htilde = new double[3, 3]{{ dx*-dx, dx*-dy, dx*-dz },
                                             { dy*-dx, dy*-dy, dy*-dz },
                                             { dz*-dx, dz*-dy, dz*-dz }};
            Htilde = Htilde / dist2;
            MatrixByArr I3 = LinAlg.Eye(3);
            // hess_ij = Kij * HTilde[i, j] - c * f_r * (I3 + HTilde[i, j]);
            MatrixByArr Hij  = new double[3, 3];
            Hij += Kij * Htilde;
            Hij -= (Fij/dist) * (I3 + Htilde);
            return Hij;
        }
    }
}
