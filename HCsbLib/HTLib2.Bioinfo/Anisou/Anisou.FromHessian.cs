using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Anisou
    {
        public static Anisou[] FromHessian(MatrixByArr hessMassWeighted, double[] mass, double scale=10000*1000
                                                , string cachepath = null
                                                )
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

            int size = mass.Length;
            HDebug.Assert(hessMassWeighted.RowSize==size*3, hessMassWeighted.ColSize==size*3);
            Anisou[] anisous = new Anisou[size];

            if(cachepath != null && HFile.Exists(cachepath))
            {
                List<Anisou> lstanisou;
                HDebug.Verify(HSerialize.Deserialize<List<Anisou>>(cachepath, null, out lstanisou));
                anisous = lstanisou.ToArray();
                return anisous;
            }

            // anisotropic temperature factors
            using(new Matlab.NamedLock("ANISOU"))
            {
                Matlab.Clear("ANISOU");
                Matlab.PutMatrix("ANISOU.H", hessMassWeighted);
                Matlab.Execute("[ANISOU.V,ANISOU.D] = eig(ANISOU.H);");
                Matlab.Execute("ANISOU.D = diag(ANISOU.D);");                               // get diagonal
                {
                    Matlab.Execute("[ANISOU.sortD, ANISOU.sortIdxD] = sort(abs(ANISOU.D));");   // sorted index of abs(D)
                    Matlab.Execute("ANISOU.D(ANISOU.sortIdxD(1:6)) = 0;");                      // set the 6 smallest eigenvalues as zero
                    //Matlab.Execute("ANISOU.D(ANISOU.D < 0) = 0;");                              // set negative eigenvalues as zero
                }
                //{
                //    Matlab.Execute("ANISOU.D(1:6) = 0;");
                //}
                Matlab.Execute("ANISOU.invD = 1 ./ ANISOU.D;");                             // set invD
                Matlab.Execute("ANISOU.invD(ANISOU.D == 0) = 0;");                          // set Inf (by divided by zero) as zero
                //Matlab.Execute("ANISOU.D = ANISOU.D .^ 2;"); // assume the gromacs ensemble condition
                Matlab.Execute("ANISOU.invH = ANISOU.V * diag(ANISOU.invD) * ANISOU.V';");
                for(int i=0; i<size; i++)
                {
                    string idx = string.Format("{0}:{1}", i*3+1, i*3+3);
                    MatrixByArr U = Matlab.GetMatrix("ANISOU.invH("+idx+","+idx+")");
                    U *= (scale/mass[i]);

                    anisous[i] = Anisou.FromMatrix(U);
                }
                Matlab.Clear("ANISOU");
            }

            if(cachepath != null)
                HSerialize.Serialize(cachepath, null, new List<Anisou>(anisous));

            return anisous;
        }
        public static Anisou[] FromHessian(Mode[] modesMassWeighted, double[] mass, double scale=10000*1000
                                                , string cachepath = null
                                                )
        {
            if(cachepath != null && HFile.Exists(cachepath))
            {
                List<Anisou> lstanisou;
                HDebug.Verify(HSerialize.Deserialize(cachepath, null, out lstanisou));
                return lstanisou.ToArray();
            }

            int size = modesMassWeighted.Size();
            HDebug.Assert(size == mass.Length);
            Anisou[] anisous = new Anisou[size];

            for(int i=0; i<size; i++)
            {
                MatrixByArr invHii = new double[3, 3];
                foreach(Mode mode in modesMassWeighted)
                {
                    Vector modei = mode.GetEigvecOfAtom(i);
                    invHii = invHii + LinAlg.VVt(modei, modei)*mode.eigval;
                }

                MatrixByArr Ui = invHii * scale / mass[i];

                anisous[i] = Anisou.FromMatrix(Ui);
            }

            if(cachepath != null)
                HSerialize.Serialize(cachepath, null, new List<Anisou>(anisous));

            return anisous;
        }
    }
}
