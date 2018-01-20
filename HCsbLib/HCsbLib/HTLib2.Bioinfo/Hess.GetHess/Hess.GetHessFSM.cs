using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public class PFSM
        {
            public Matrix KHess; // kij * ANM_ij
            public Matrix FHess; // fij/rij * (ANM_ij - GNM_ij)
            public Matrix hess { get { return (KHess + FHess); } }
        }
        public static PFSM GetHessFSM(IList<Vector> coords, Matrix pwspring, Matrix pwforce, ILinAlg la)
        {
            HDebug.ToDo("check!!");
            int size = coords.Count;

            Matrix khess = Hess.GetHessAnm(coords, pwspring.ToArray());
            Matrix invkhess;
            {
                var HH = la.ToILMat(khess);
                HDebug.Assert(false); // try to use eig, instead of pinv
                var invHH = la.PInv(HH);
                HH.Dispose();
                //var VVDD = la.EigSymm(HH);
                invkhess = invHH.ToArray();
                invHH.Dispose();
            }

            Matrix npwforce = Hess.GetHessFSM_GetEqlibForc(coords, khess, invkhess, pwforce);
            Matrix  pwdist  = coords.Pwdist();

            Matrix anm = Hess.GetHessAnm(coords, double.PositiveInfinity);

            MatrixBySparseMatrix fhess = MatrixBySparseMatrix.Zeros(size*3, size*3, 3);
            for(int i=0; i<size; i++)
                for(int j=0; j<size; j++)
                {
                    if(i == j) continue;
                    int[] idxi = new int[]{ i*3+0, i*3+1, i*3+2 };
                    int[] idxj = new int[]{ j*3+0, j*3+1, j*3+2 };

                    // ANM_ij
                    MatrixByArr fhess_ij = anm.SubMatrix(idxi, idxj).ToArray();
                    // ANM_ij - GNM_ij = ANM_ij - I_33
                    fhess_ij[0,0] += 1;
                    fhess_ij[1,1] += 1;
                    fhess_ij[2,2] += 1;
                    // fij/rij * (ANM_ij - GNM_ij)
                    fhess_ij *= (npwforce[i,j] / pwdist[i,j]);

                    fhess.SetBlock(i, j, fhess_ij);
                    fhess.SetBlock(i, i, fhess.GetBlock(i, i) - fhess_ij);
                }

            return new PFSM
            {
                KHess = khess,
                FHess = fhess,
            };
        }
        public static Matrix GetHessFSM_GetEqlibForc(IList<Vector> coords, Matrix hess, Matrix invhess, Matrix pwfrc)
        {
            int size = coords.Count;
            HDebug.Assert(hess.ColSize == size*3, hess.RowSize == size*3);
            HDebug.Assert(pwfrc.ColSize == size , pwfrc.RowSize == size );

            Vector[,] uij = new Vector[size,size];
            Vector[]  fi  = new Vector[size];
            for(int i=0; i<size; i++)
            {
                fi[i] = new double[3];
                for(int j=0; j<size; j++)
                {
                    if(i == j) continue;
                    uij[i,j] = (coords[j] - coords[i]).UnitVector();
                    fi[i] += uij[i,j] * pwfrc[i,j];
                }
            }

            Vector[] di = new Vector[size];
            for(int i=0; i<size; i++)
            {
                di[i] = new double[3];
                for(int j=0; j<size; j++)
                {
                    Matrix invHij = invhess.SubMatrix( new int[] { i*3+0, i*3+1, i*3+2 }
                                                     , new int[] { j*3+0, j*3+1, j*3+2 }
                                                     );
                    di[i] += LinAlg.MV(invHij, fi[i]);
                }
            }

            Matrix F = Matrix.Zeros(size, size);
            for(int i=0; i<size; i++)
                for(int j=0; j<size; j++)
                {
                    Matrix Hij = hess.SubMatrix( new int[] { i*3+0, i*3+1, i*3+2 }
                                               , new int[] { j*3+0, j*3+1, j*3+2 }
                                               );
                    double gij = LinAlg.VtV(LinAlg.MV(Hij, di[j] - di[i]), uij[i, j]);
                    F[i,j] = pwfrc[i,j] + gij;
                }

            return F;
        }
    }
}
