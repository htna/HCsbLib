using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static double GetHessHydroBond_defThresEnergy = 0.01;
        public static HessMatrix GetHessHydroBond(Universe univ, IList<Vector> coords, bool ignNegSpr)
        {
            return GetHessHydroBond(univ, coords, ignNegSpr, null, GetHessHydroBond_defThresEnergy);
        }
        public static HessMatrix GetHessHydroBond(Universe univ, IList<Vector> coords, bool ignNegSpr, double thres_energy)
        {
            return GetHessHydroBond(univ, coords, ignNegSpr, null, thres_energy);
        }
        public static HessMatrix GetHessHydroBond(Universe univ, IList<Vector> coords, double constSpr)
        {
            return GetHessHydroBond(univ, coords, false, constSpr, GetHessHydroBond_defThresEnergy);
        }
        public static HessMatrix GetHessHydroBond(Universe univ, IList<Vector> coords, double constSpr, double thres_energy)
        {
            return GetHessHydroBond(univ, coords, false, constSpr, thres_energy);
        }
        public static HessMatrix GetHessHydroBond(Universe univ, IList<Vector> coords, bool ignNegSpr, double? constSpr, double thres_energy)
        {
            int size = coords.Count;
            double[,] hbond_kij = new double[size, size];
            foreach(var hbond in univ.EnumHydroBondIrback09(coords))
            {
                Universe.Atom Ni = hbond.Ni;
                Universe.Atom Hi = hbond.Hi;
                Universe.Atom Oj = hbond.Oj;
                Universe.Atom Cj = hbond.Cj;
                double hbond_energy = hbond.energy;
                double hbond_spring = hbond.spring; // spring between Hi..Oj

                if(Math.Abs(hbond_energy) < thres_energy)
                    continue;
                if(ignNegSpr && (hbond_spring < 0))
                    continue;
                if(constSpr != null)
                    hbond_spring = constSpr.Value;

                int i = Hi.ID;
                int j = Oj.ID;
                hbond_kij[i, j] = hbond_spring;
                hbond_kij[j, i] = hbond_spring;
            }
            HessMatrix hess_hbond = GetHessAnm(coords, hbond_kij);
            return hess_hbond;
        }
    }
}
