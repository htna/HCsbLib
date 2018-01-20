using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static Matrix[] GetAnisou(Matrix hessMassWeighted, double[] mass, double scale=10000*1000)
        {
            /// Estimation of "anisotropic temperature factors" (ANISOU)
            /// 
            /// delta = hess^-1 * force
            ///       = (0 + V7*V7'/L7 + V8*V8'/L8 + V9*V9'/L9 + ...) * force    (* assume that 1-6 eigvecs/eigvals are ignored, because rot,trans *)
            /// 
            /// Assume that force[i] follows gaussian distributions N(0,1). Here, if there are 1000 samples, let denote i-th force as fi, and its j-th element as fi[j]
            /// Then, $V7' * fi = si7, V8' * fi = si8, ...$ follows gaussian distribution N(0,1), too.
            /// Its moved position by k-th eigen component is determined then, as
            ///     dik = (Vk * Vk' / Lk) * Fi
            ///         = Vk / Lk * (Vk' * Fi)
            ///         = Vk / Lk * Sik.
            /// Additionally, the moved position j-th atom is:
            ///     dik[j] = Vk[j] / Lk[j] * Sik.
            /// and its correlation matrix is written as (because its mean position is 0 !!!):
            ///     Cik[j] = dik[j] * dik[j]'
            ///            = [dik[j]_x * dik[j]_x    dik[j]_x * dik[j]_y    dik[j]_x * dik[j]_z]
            ///              [dik[j]_y * dik[j]_x    dik[j]_y * dik[j]_y    dik[j]_y * dik[j]_z]
            ///              [dik[j]_z * dik[j]_x    dik[j]_z * dik[j]_y    dik[j]_z * dik[j]_z]
            ///            = (Vk[j] * Vk[j]') / (Lk[j]*Lk[j]) * (Sik*Sik).
            /// 
            /// Note that Sik*Sik follows the chi-square distribution, because Sik follows the gaussian distribution N(0,1).
            /// Additionally, note that the thermal fluctuation is (not one projection toward k-th eigen component with only i-th force, but) the results of 1..i.. forced movements and 1..k.. eigen components.
            /// Therefore, for j-th atom, the accumulation of the correlation over all forces (1..i..) with all eigen components (1..k..) is:
            ///     C[j] = sum_{i,k} {(Vk[j] * Vk[j]') / (Lk[j]*Lk[j]) * (Sik*Sik)}.
            /// 
            /// Here, Sik is normal distribution independent to i and k. Therefore, the mean of C[j] is
            ///     E(C[j]) = E( sum_{i,k} {(Vk[j] * Vk[j]') / (Lk[j]*Lk[j]) * (Sik*Sik)} )
            ///             = sum_{i,k} E( (Vk[j] * Vk[j]') / (Lk[j]*Lk[j]) * (Sik*Sik) )
            ///             = sum_{i,k} { (Vk[j] * Vk[j]') / (Lk[j]*Lk[j]) * E(Sik*Sik) }
            ///             = sum_{i,k} { (Vk[j] * Vk[j]') / (Lk[j]*Lk[j]) * 1          }          (* because mean of E(x*x)=1 where x~N(0,1) *)
            ///             = sum_{k} { (Vk[j] * Vk[j]') / (Lk[j]*Lk[j]) }
            /// 
            /// Note that E(C[j]) is same to the j-th diagonal component of inverse hessian matrix (except, the eigenvalues are squared).
            /// 
            /// Fixation: Gromacx generate the ensemble X by
            ///               X[j] = sum_{k} {Vk / sqrt(Lk[j]) / sqrt(mass[j]) * x_k},
            ///           where x~N(0,1). However, the above is assumed as 
            ///               X[j] = sum_{k} {Vk / Lk[j] * x_k}.
            ///           In order to apply the assumption by the Gromacs ensemble, The equation should be fixed as
            ///               E(C[j]) = sum_{k} { (Vk[j] * Vk[j]') / (sqrt(Lk[j])*sqrt(Lk[j])) }
            ///                       = sum_{k} { (Vk[j] * Vk[j]') / Lk[j] / mass[j] }
            /// 



            // anisotropic temperature factors
            int size = mass.Length;
            HDebug.Assert(hessMassWeighted.RowSize==size*3, hessMassWeighted.ColSize==size*3);
            Matrix[] anisous = new Matrix[size];
            using(new Matlab.NamedLock("ANISOU"))
            {
                Matlab.Clear("ANISOU");
                Matlab.PutMatrix("ANISOU.H", hessMassWeighted);
                Matlab.Execute("[ANISOU.V,ANISOU.D] = eig(ANISOU.H);");
                Matlab.Execute("ANISOU.D = diag(ANISOU.D);");
                Matlab.Execute("ANISOU.D = ANISOU.D(7:end);");
                //Matlab.Execute("ANISOU.D = ANISOU.D .^ 2;"); // assume the gromacs ensemble condition
                Matlab.Execute("ANISOU.V = ANISOU.V(:,7:end);");
                Matlab.Execute("ANISOU.invH = ANISOU.V * pinv(diag(ANISOU.D)) * ANISOU.V';");
                for(int i=0; i<size; i++)
                {
                    string idx = string.Format("{0}:{1}", i*3+1, i*3+3);
                    anisous[i] = Matlab.GetMatrix("ANISOU.invH("+idx+","+idx+")");
                }
                for(int i=0; i<size; i++)
                    anisous[i] *= (scale/mass[i]);
                Matlab.Clear("ANISOU");
            }
            return anisous;
        }
    }
}
