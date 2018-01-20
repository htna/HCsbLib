using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public double GetPotentialParallel(List<ForceField.IForceField> frcflds
                                            , Vector[] coords
                                            , Vector[] forces = null
                                            , Dictionary<string, object> cache = null
                                            , Vector[,] forceij = null
                                            )
        {
            MatrixByArr[,] hess = null;
            string[] pottypes = {"energy_bonds     "   // 0
                                ,"energy_angles    "   // 1
                                ,"energy_dihedrals "   // 2
                                ,"energy_impropers "   // 3
                                ,"energy_nonbondeds"   // 4
                                ,"energy_customs   "}; // 5

            if(cache != null)
                // make temporary cache
                cache = new Dictionary<string, object>();

            double[] lenergy = new double[pottypes.Length];
            Vector[][] lforces = new Vector[pottypes.Length][];
            if(forces != null)
                for(int i=0; i<lforces.Length; i++)
                    lforces[i] = GetVectorsZero();

            Parallel.Invoke(
                delegate() { int i=4; lenergy[i] = GetPotentialNonbondeds(frcflds, coords, ref lforces[i], ref hess, cache, forceij); },
                delegate() { int i=0; lenergy[i] = GetPotentialBonds     (frcflds, coords, ref lforces[i], ref hess, null , forceij); },
                delegate() { int i=1; lenergy[i] = GetPotentialAngles    (frcflds, coords, ref lforces[i], ref hess, null , forceij); },
                delegate() { int i=2; lenergy[i] = GetPotentialDihedrals (frcflds, coords, ref lforces[i], ref hess, null , forceij); },
                delegate() { int i=3; lenergy[i] = GetPotentialImpropers (frcflds, coords, ref lforces[i], ref hess, null , forceij); },
                delegate() { int i=5; lenergy[i] = GetPotentialCustoms   (frcflds, coords, ref lforces[i], ref hess, null , forceij); }
                );
            //Parallel.For(0, 6, delegate(int i)
            //{
            //    switch(i)
            //    {
            //        case 0: { lenergy[0] = GetPotentialBonds     (frcflds, coords, ref lforces[0], ref hess, null , forceij); } break;
            //        case 1: { lenergy[1] = GetPotentialAngles    (frcflds, coords, ref lforces[1], ref hess, null , forceij); } break;
            //        case 2: { lenergy[2] = GetPotentialDihedrals (frcflds, coords, ref lforces[2], ref hess, null , forceij); } break;
            //        case 3: { lenergy[3] = GetPotentialImpropers (frcflds, coords, ref lforces[3], ref hess, null , forceij); } break;
            //        case 4: { lenergy[4] = GetPotentialNonbondeds(frcflds, coords, ref lforces[4], ref hess, cache, forceij); } break;
            //        case 5: { lenergy[5] = GetPotentialCustoms   (frcflds, coords, ref lforces[5], ref hess, null , forceij); } break;
            //    }
            //});
            { int i=0; if(cache != null) cache.Add("energy_bonds     ", 0); cache["energy_bonds     "] = lenergy[i]; if(forces != null) Vector.AddTo(forces, lforces[i]); }
            { int i=1; if(cache != null) cache.Add("energy_angles    ", 0); cache["energy_angles    "] = lenergy[i]; if(forces != null) Vector.AddTo(forces, lforces[i]); }
            { int i=2; if(cache != null) cache.Add("energy_dihedrals ", 0); cache["energy_dihedrals "] = lenergy[i]; if(forces != null) Vector.AddTo(forces, lforces[i]); }
            { int i=3; if(cache != null) cache.Add("energy_impropers ", 0); cache["energy_impropers "] = lenergy[i]; if(forces != null) Vector.AddTo(forces, lforces[i]); }
            { int i=4; if(cache != null) cache.Add("energy_nonbondeds", 0); cache["energy_nonbondeds"] = lenergy[i]; if(forces != null) Vector.AddTo(forces, lforces[i]); }
            { int i=5; if(cache != null) cache.Add("energy_customs   ", 0); cache["energy_customs   "] = lenergy[i]; if(forces != null) Vector.AddTo(forces, lforces[i]); }

            double energy  = 0;
            for(int i=0; i<pottypes.Length; i++)
                energy += lenergy[i];

            if(hess != null)
                GetPotentialParallel_MakeSumZero(hess);

            return energy;
        }
        public void GetPotentialParallel_MakeSumZero(MatrixByArr[,] hess)
        {
            HDebug.Assert(false);
            //for(int c=0; c<size*3; c++)
            //    for(int r=0; r<size*3; r++)
            //        hessian[c, r] = hess[c/3, r/3][c%3, r%3];

            //hessian = (hessian + hessian.Transpose())/2;

            //for(int i=0; i<size; i++)
            //{
            //    hessian[i*3+0, i*3+0] = 0; hessian[i*3+0, i*3+1] = 0; hessian[i*3+0, i*3+2] = 0;
            //    hessian[i*3+1, i*3+0] = 0; hessian[i*3+1, i*3+1] = 0; hessian[i*3+1, i*3+2] = 0;
            //    hessian[i*3+2, i*3+0] = 0; hessian[i*3+2, i*3+1] = 0; hessian[i*3+2, i*3+2] = 0;

            //    int c = i;
            //    for(int r=0; r<size; r++)
            //    {
            //        hessian[i*3+0, i*3+0] -= hessian[c*3+0, r*3+0]; hessian[i*3+0, i*3+1] -= hessian[c*3+0, r*3+1]; hessian[i*3+0, i*3+2] -= hessian[c*3+0, r*3+2];
            //        hessian[i*3+1, i*3+0] -= hessian[c*3+1, r*3+0]; hessian[i*3+1, i*3+1] -= hessian[c*3+1, r*3+1]; hessian[i*3+1, i*3+2] -= hessian[c*3+1, r*3+2];
            //        hessian[i*3+2, i*3+0] -= hessian[c*3+2, r*3+0]; hessian[i*3+2, i*3+1] -= hessian[c*3+2, r*3+1]; hessian[i*3+2, i*3+2] -= hessian[c*3+2, r*3+2];
            //    }
            //}
        }
    }
}
