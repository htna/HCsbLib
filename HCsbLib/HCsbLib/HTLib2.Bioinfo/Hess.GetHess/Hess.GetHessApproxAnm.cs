using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static HessMatrix GetHessApproxAnm(Vector[] coords, HessMatrix hess, string option, ILinAlg ila)
        {
            int nresi = coords.Length;
            HDebug.Assert(nresi*3 == hess.ColSize);
            HDebug.Assert(nresi*3 == hess.RowSize);

            double[,] Kij = new double[nresi, nresi];

            for(int c=0; c<nresi; c++)
            {
                for(int r=c+1; r<nresi; r++)
                {
                    HessMatrix  anmCR = Hess.GetHessAnm(coords.HSelectByIndex(new int[] { c, r }));
                    //Matrix  anmCR = anm.SubMatrix(new int[] { 0, 1, 2 }, new int[] { 3, 4, 5 });
                    int[] idxs = new int[] { c*3+0, c*3+1, c*3+2, r*3+0, r*3+1, r*3+2 };
                    HessMatrix hessCR = hess.SubMatrix(idxs, idxs).ToHessMatrix();
                    hessCR = Hess.CorrectHessDiag(hessCR);

                    Vector vecHessCR = hessCR.GetColVectorList().ToVector();
                    Vector  vecAnmCR =  anmCR.GetColVectorList().ToVector();

                    double Kcr;
                    switch(option)
                    {
                        case "match magnitude"  : Kcr = vecHessCR.Dist / vecAnmCR.Dist; break;
                        case "least-mean square": Kcr = LinAlg.VtV(vecAnmCR, vecHessCR) / LinAlg.VtV(vecAnmCR, vecAnmCR); break;
                        default: throw new NotImplementedException();
                    }

                    if(Kcr < 0)
                        HDebug.Assert(false);
                    HDebug.Assert(Kcr >= 0);
                    double mag2cahessCR =  vecAnmCR.Dist;
                    double mag2caAnmCR  = vecHessCR.Dist;
                    double mag2caAnmCRx = (Kcr*anmCR).GetColVectorList().ToVector().Dist;
                    Kij[c, r] = Kij[r, c] = Kcr;
                }
            }

            //return Kij;
            return Hess.GetHessAnm(coords, Kij);
            throw new NotImplementedException();
        }
    }
}
