using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Hess
{
    public partial class STeM
    {
        public delegate double FuncIJ  (int i, int j);
        public delegate double FuncIJK (int i, int j, int k);
        public delegate double FuncIJKL(int i, int j, int k, int l);

        public static HessMatrix GetHessCa_v2(IList<Vector> caArray)
        {
            double Epsilon=0.36;
            double K_r=100*Epsilon;
            double K_theta=20*Epsilon;
            double K_phi1=1*Epsilon;
            double K_phi3=0.5*Epsilon;
            int numOfResidues=caArray.Count;
            double[,] distance = caArray.Pwdist();

            HessMatrix hessian = HessMatrix.ZerosHessMatrix(caArray.Count*3, caArray.Count*3);
            hessian = FirstTerm (caArray, K_r           , hessian: hessian);
            hessian = SecondTerm(caArray, K_theta       , hessian: hessian);
            hessian = ThirdTerm (caArray, K_phi1, K_phi3, hessian: hessian);
            hessian = FourthTerm(caArray, Epsilon       , hessian: hessian);

            return hessian;
        }
        public static HessMatrix GetHessCa(IList<Vector> caArray)
        {
            double Epsilon = 0.36;
            double K_r     = 100 * Epsilon;
            double K_theta = 20  * Epsilon;
            double K_phi1  = 1   * Epsilon;
            double K_phi3  = 0.5 * Epsilon;

            return GetHessCa
                ( caArray
                , K_r     : K_r    
                , K_theta : K_theta
                , K_phi1  : K_phi1 
                , K_phi3  : K_phi3 
                , Epsilon : Epsilon
                );
        }
        public static HessMatrix GetHessCa
            ( IList<Vector> caArray
            , double? K_r
            , double? K_theta
            , double? K_phi1
            , double? K_phi3
            , double? Epsilon
            )
        {
            // written by TuLiang
            // converted to C# by htna

            //double Epsilon=0.36;
            //double K_r=100*Epsilon;
            //double K_theta=20*Epsilon;
            //double K_phi1=1*Epsilon;
            //double K_phi3=0.5*Epsilon;

            FuncIJ   func_K_r     = null; if(K_r     != null) func_K_r     = delegate(int i, int j              ) { HDebug.Assert(0<=i, i<caArray.Count); HDebug.Assert(0<=j, j<caArray.Count);                                                                             return K_r.Value    ; };
            FuncIJK  func_K_theta = null; if(K_theta != null) func_K_theta = delegate(int i, int j, int k       ) { HDebug.Assert(0<=i, i<caArray.Count); HDebug.Assert(0<=j, j<caArray.Count); HDebug.Assert(0<=k, k<caArray.Count);                                       return K_theta.Value; };
            FuncIJKL func_K_phi1  = null; if(K_phi1  != null) func_K_phi1  = delegate(int i, int j, int k, int l) { HDebug.Assert(0<=i, i<caArray.Count); HDebug.Assert(0<=j, j<caArray.Count); HDebug.Assert(0<=k, k<caArray.Count); HDebug.Assert(0<=l, l<caArray.Count); return K_phi1.Value ; };
            FuncIJKL func_K_phi3  = null; if(K_phi3  != null) func_K_phi3  = delegate(int i, int j, int k, int l) { HDebug.Assert(0<=i, i<caArray.Count); HDebug.Assert(0<=j, j<caArray.Count); HDebug.Assert(0<=k, k<caArray.Count); HDebug.Assert(0<=l, l<caArray.Count); return K_phi3.Value ; };
            FuncIJ   func_Epsilon = null; if(Epsilon != null) func_Epsilon = delegate(int i, int j              ) { HDebug.Assert(0<=i, i<caArray.Count); HDebug.Assert(0<=j, j<caArray.Count);                                                                             return Epsilon.Value; };

            return GetHessCa
            ( caArray
            , func_K_r
            , func_K_theta
            , func_K_phi1
            , func_K_phi3
            , func_Epsilon
            );
        }

        public static HessMatrix GetHessCa
            ( IList<Vector> caArray
            , FuncIJ   func_K_r
            , FuncIJK  func_K_theta
            , FuncIJKL func_K_phi1
            , FuncIJKL func_K_phi3
            , FuncIJ   func_Epsilon
            )
        {
            int numOfResidues=caArray.Count;
            double[,] distance = caArray.Pwdist();

            HessMatrix hessian = HessMatrix .ZerosHessMatrix(caArray.Count*3, caArray.Count*3);

            if(func_K_r     != null                       ) hessian = firstTerm (caArray, distance, numOfResidues, func_K_r                , hessian_:hessian);
            if(func_K_theta != null                       ) hessian = secondTerm(caArray, distance, numOfResidues, func_K_theta            , hessian_:hessian);
            if(func_K_phi1  != null && func_K_phi3 != null) hessian = thirdTerm (caArray, distance, numOfResidues, func_K_phi1, func_K_phi3, hessian_:hessian);
            if(func_Epsilon != null                       ) hessian = fourthTerm(caArray, distance, numOfResidues, func_Epsilon            , hessian_:hessian);
            if(HDebug.Selftest())
            {
                HessMatrix hess0 = GetHessCa_matlab(caArray);
                HessMatrix dhess = hessian - hess0;
                HDebug.AssertToleranceMatrix(0.00000001, dhess);
            }
            return hessian;
        }
        public static HessMatrix GetHessCa_matlab(IList<Vector> caArray)
        {
            string randomname = HFile.GetTempPath("Fn",".m");
            randomname = randomname.Substring(0, randomname.Length-2);

            HessMatrix hess0;
            using(new Matlab.NamedLock("STEM"))
            {
                HFile.WriteAllText(randomname+".m", matlab_source);

                Matlab.Clear("STEM");
                string currpath = Matlab.Pwd();
                Matlab.Cd(HEnvironment.CurrentDirectory);

                Matlab.PutMatrix("STEM.caArray", caArray.ToMatrix(false));
                Matlab.Execute("[STEM.hv1, STEM.hv2, STEM.hv3, STEM.hv4] = "+randomname+"(STEM.caArray);");
                HessMatrix hv1 = HessMatrix.FromMatrix(Matlab.GetMatrix("STEM.hv1"));
                HessMatrix hv2 = HessMatrix.FromMatrix(Matlab.GetMatrix("STEM.hv2"));
                HessMatrix hv3 = HessMatrix.FromMatrix(Matlab.GetMatrix("STEM.hv3"));
                HessMatrix hv4 = HessMatrix.FromMatrix(Matlab.GetMatrix("STEM.hv4"));
                hess0 = hv1 + hv2 + hv3 + hv4;

                Matlab.Cd(currpath);

                HFile.Delete(randomname+".m");
            }
            return hess0;
        }

        public static HessMatrix firstTerm(IList<Vector> caArray_, MatrixByArr distance_, int numOfResidues, FuncIJ func_K_r, HessMatrix hessian_=null)
        {
            VECTORS caArray  = new VECTORS(caArray_);
            MATRIX  distance = new MATRIX(distance_);
            if(hessian_ == null)
                hessian_ = HessMatrix.ZerosHessMatrix(caArray_.Count*3, caArray_.Count*3);
            MATRIX hessian = new MATRIX(hessian_);

            // derive the hessian of the first term (off diagonal)
            for(int m=2; m<=numOfResidues; m++)
            {
                int i=m-1;
                int j=m;

                double K_r = func_K_r(i-1, j-1);

                double bx=caArray[i,1] - caArray[j,1];
                double by=caArray[i,2] - caArray[j,2];
                double bz=caArray[i,3] - caArray[j,3];
                double distijsqr=distance[i,j] * distance[i,j];

                //Hij
                // diagonals of off-diagonal super elements (1st term)

                hessian[3*i-2,3*j-2]       += -2*K_r*bx*bx/distijsqr;
                hessian[3*i-1,3*j-1]       += -2*K_r*by*by/distijsqr;
                hessian[3*i  ,3*j  ]       += -2*K_r*bz*bz/distijsqr;

                // off-diagonals of off-diagonal super elements (1st term)

                hessian[3*i-2,3*j-1]       += -2*K_r*bx*by/distijsqr;
                hessian[3*i-2,3*j  ]       += -2*K_r*bx*bz/distijsqr;
                hessian[3*i-1,3*j-2]       += -2*K_r*by*bx/distijsqr;
                hessian[3*i-1,3*j  ]       += -2*K_r*by*bz/distijsqr;
                hessian[3*i  ,3*j-2]       += -2*K_r*bz*bx/distijsqr;
                hessian[3*i  ,3*j-1]       += -2*K_r*bz*by/distijsqr;

                //Hji
                // diagonals of off-diagonal super elements (1st term)

                hessian[3*j-2,3*i-2]       += -2*K_r*bx*bx/distijsqr;
                hessian[3*j-1,3*i-1]       += -2*K_r*by*by/distijsqr;
                hessian[3*j  ,3*i  ]       += -2*K_r*bz*bz/distijsqr;

                // off-diagonals of off-diagonal super elements (1st term)

                hessian[3*j-2,3*i-1]       += -2*K_r*bx*by/distijsqr;
                hessian[3*j-2,3*i  ]       += -2*K_r*bx*bz/distijsqr;
                hessian[3*j-1,3*i-2]       += -2*K_r*by*bx/distijsqr;
                hessian[3*j-1,3*i  ]       += -2*K_r*by*bz/distijsqr;
                hessian[3*j  ,3*i-2]       += -2*K_r*bz*bx/distijsqr;
                hessian[3*j  ,3*i-1]       += -2*K_r*bz*by/distijsqr;

                //Hii
                //update the diagonals of diagonal super elements

                hessian[3*i-2,3*i-2]       += +2*K_r*bx*bx/distijsqr;
                hessian[3*i-1,3*i-1]       += +2*K_r*by*by/distijsqr;
                hessian[3*i  ,3*i  ]       += +2*K_r*bz*bz/distijsqr;
            
                // update the off-diagonals of diagonal super elements
                hessian[3*i-2,3*i-1]       += +2*K_r*bx*by/distijsqr;
                hessian[3*i-2,3*i  ]       += +2*K_r*bx*bz/distijsqr;
                hessian[3*i-1,3*i-2]       += +2*K_r*by*bx/distijsqr;
                hessian[3*i-1,3*i  ]       += +2*K_r*by*bz/distijsqr;
                hessian[3*i  ,3*i-2]       += +2*K_r*bz*bx/distijsqr;
                hessian[3*i  ,3*i-1]       += +2*K_r*bz*by/distijsqr;

                //Hjj
                //update the diagonals of diagonal super elements

                hessian[3*j-2,3*j-2]       += +2*K_r*bx*bx/distijsqr;
                hessian[3*j-1,3*j-1]       += +2*K_r*by*by/distijsqr;
                hessian[3*j  ,3*j  ]       += +2*K_r*bz*bz/distijsqr;

                // update the off-diagonals of diagonal super elements
                hessian[3*j-2,3*j-1]       += +2*K_r*bx*by/distijsqr;
                hessian[3*j-2,3*j  ]       += +2*K_r*bx*bz/distijsqr;
                hessian[3*j-1,3*j-2]       += +2*K_r*by*bx/distijsqr;
                hessian[3*j-1,3*j  ]       += +2*K_r*by*bz/distijsqr;
                hessian[3*j  ,3*j-2]       += +2*K_r*bz*bx/distijsqr;
                hessian[3*j  ,3*j-1]       += +2*K_r*bz*by/distijsqr;
            }

            if(hessian.matrix is HessMatrix)
                return (hessian.matrix as HessMatrix);
            return HessMatrix.FromMatrix( hessian.matrix );
        }

        public static HessMatrix secondTerm(IList<Vector> caArray_, MatrixByArr distance_, int numOfResidues, FuncIJK func_K_theta, HessMatrix hessian_=null)
        {
            VECTORS caArray = new VECTORS(caArray_);
            MATRIX distance = new MATRIX(distance_);
            if(hessian_ == null)
                hessian_ = HessMatrix.ZerosHessMatrix( caArray_.Count*3, caArray_.Count*3 );
            MATRIX hessian = new MATRIX(hessian_);

            // derive the hessian of the second term
            for(int m=2; m<=numOfResidues-1; m++)
            {
                int i=m-1;
                int j=m;
                int k=m+1;

                double K_theta = func_K_theta(i-1, j-1, k-1);

                double Xi=caArray[i,1];
                double Yi=caArray[i,2];
                double Zi=caArray[i,3];
                double Xj=caArray[j,1];
                double Yj=caArray[j,2];
                double Zj=caArray[j,3];
                double Xk=caArray[k,1];
                double Yk=caArray[k,2];
                double Zk=caArray[k,3];

                Vector p=caArray.GetRow(i)-caArray.GetRow(j);
                double lpl=distance[i,j];
                Vector q=caArray.GetRow(k)-caArray.GetRow(j);
                double lql=distance[k,j];
                double G=dot(p,q)/(lpl*lql);

                /* dG/dXi */ double dGdXi=((Xk-Xj)*lpl*lql-dot(p,q)*(lql/lpl)*(Xi-Xj))/pow(lpl*lql);
                /* dG/dYi */ double dGdYi=((Yk-Yj)*lpl*lql-dot(p,q)*(lql/lpl)*(Yi-Yj))/pow(lpl*lql);
                /* dG/dZi */ double dGdZi=((Zk-Zj)*lpl*lql-dot(p,q)*(lql/lpl)*(Zi-Zj))/pow(lpl*lql);

                /* dG/dXj */ double dGdXj=((2*Xj-Xi-Xk)*lpl*lql-dot(p,q)*(lql/lpl)*(Xj-Xi)-dot(p,q)*(lpl/lql)*(Xj-Xk))/pow(lpl*lql);
                /* dG/dYj */ double dGdYj=((2*Yj-Yi-Yk)*lpl*lql-dot(p,q)*(lql/lpl)*(Yj-Yi)-dot(p,q)*(lpl/lql)*(Yj-Yk))/pow(lpl*lql);
                /* dG/dZj */ double dGdZj=((2*Zj-Zi-Zk)*lpl*lql-dot(p,q)*(lql/lpl)*(Zj-Zi)-dot(p,q)*(lpl/lql)*(Zj-Zk))/pow(lpl*lql);

                /* dG/dXk */ double dGdXk=((Xi-Xj)*lpl*lql-dot(p,q)*(lpl/lql)*(Xk-Xj))/pow(lpl*lql);
                /* dG/dYk */ double dGdYk=((Yi-Yj)*lpl*lql-dot(p,q)*(lpl/lql)*(Yk-Yj))/pow(lpl*lql);
                /* dG/dZk */ double dGdZk=((Zi-Zj)*lpl*lql-dot(p,q)*(lpl/lql)*(Zk-Zj))/pow(lpl*lql);

                //Hij
                //d^2V/dXidXj  d^2V/dXidYj  d^2V/dXidZj
                //d^2V/dYidXj  d^2V/dYidYj  d^2V/dYidZj
                //d^2V/dZidXj  d^2V/dZidYj  d^2V/dZidZj             

                // diagonals of off-diagonal super elements             

                hessian[3*i-2,3*j-2]       += +2*K_theta/(1-G*G)*dGdXi*dGdXj;
                hessian[3*i-1,3*j-1]       += +2*K_theta/(1-G*G)*dGdYi*dGdYj;
                hessian[3*i  ,3*j  ]       += +2*K_theta/(1-G*G)*dGdZi*dGdZj;

                // off-diagonals of off-diagonal super elements

                hessian[3*i-2,3*j-1]       += +2*K_theta/(1-G*G)*dGdXi*dGdYj;
                hessian[3*i-2,3*j  ]       += +2*K_theta/(1-G*G)*dGdXi*dGdZj;
                hessian[3*i-1,3*j-2]       += +2*K_theta/(1-G*G)*dGdYi*dGdXj;
                hessian[3*i-1,3*j  ]       += +2*K_theta/(1-G*G)*dGdYi*dGdZj;
                hessian[3*i  ,3*j-2]       += +2*K_theta/(1-G*G)*dGdZi*dGdXj;
                hessian[3*i  ,3*j-1]       += +2*K_theta/(1-G*G)*dGdZi*dGdYj;

                //Hji
                // diagonals of off-diagonal super elements            
                hessian[3*j-2,3*i-2]       += +2*K_theta/(1-G*G)*dGdXj*dGdXi;
                hessian[3*j-1,3*i-1]       += +2*K_theta/(1-G*G)*dGdYj*dGdYi;
                hessian[3*j  ,3*i  ]       += +2*K_theta/(1-G*G)*dGdZj*dGdZi;

                // off-diagonals of off-diagonal super elements

                hessian[3*j-2,3*i-1]       += +2*K_theta/(1-G*G)*dGdXj*dGdYi;
                hessian[3*j-2,3*i  ]       += +2*K_theta/(1-G*G)*dGdXj*dGdZi;
                hessian[3*j-1,3*i-2]       += +2*K_theta/(1-G*G)*dGdYj*dGdXi;
                hessian[3*j-1,3*i  ]       += +2*K_theta/(1-G*G)*dGdYj*dGdZi;
                hessian[3*j  ,3*i-2]       += +2*K_theta/(1-G*G)*dGdZj*dGdXi;
                hessian[3*j  ,3*i-1]       += +2*K_theta/(1-G*G)*dGdZj*dGdYi;

                //disp(G);
                //disp([i,j]);
                //disp(hessian(3*i-2:3*i,3*j-2:3*j));
                //disp(hessian(3*j-2:3*j,3*i-2:3*i));
                
                
                //Hjk
                //d^2V/dXjdXk  d^2V/dXjdYk  d^2V/dXjdZk
                //d^2V/dYjdXk  d^2V/dYjdYk  d^2V/dYjdZk
                //d^2V/dZjdXk  d^2V/dZjdYk  d^2V/dZjdZk
            
                // diagonals of off-diagonal super elements

                hessian[3*j-2,3*k-2]       += +2*K_theta/(1-G*G)*dGdXj*dGdXk;
                hessian[3*j-1,3*k-1]       += +2*K_theta/(1-G*G)*dGdYj*dGdYk;
                hessian[3*j  ,3*k  ]       += +2*K_theta/(1-G*G)*dGdZj*dGdZk;
            
                // off-diagonals of off-diagonal super elements

                hessian[3*j-2,3*k-1]       += +2*K_theta/(1-G*G)*dGdXj*dGdYk;
                hessian[3*j-2,3*k  ]       += +2*K_theta/(1-G*G)*dGdXj*dGdZk;
                hessian[3*j-1,3*k-2]       += +2*K_theta/(1-G*G)*dGdYj*dGdXk;
                hessian[3*j-1,3*k  ]       += +2*K_theta/(1-G*G)*dGdYj*dGdZk;
                hessian[3*j  ,3*k-2]       += +2*K_theta/(1-G*G)*dGdZj*dGdXk;
                hessian[3*j  ,3*k-1]       += +2*K_theta/(1-G*G)*dGdZj*dGdYk;                                                                       
            
                //Hkj

                // diagonals of off-diagonal super elements

                hessian[3*k-2,3*j-2]       += +2*K_theta/(1-G*G)*dGdXk*dGdXj;
                hessian[3*k-1,3*j-1]       += +2*K_theta/(1-G*G)*dGdYk*dGdYj;
                hessian[3*k  ,3*j  ]       += +2*K_theta/(1-G*G)*dGdZk*dGdZj;
            
                // off-diagonals of off-diagonal super elements
    
                hessian[3*k-2,3*j-1]       += +2*K_theta/(1-G*G)*dGdXk*dGdYj;
                hessian[3*k-2,3*j  ]       += +2*K_theta/(1-G*G)*dGdXk*dGdZj;
                hessian[3*k-1,3*j-2]       += +2*K_theta/(1-G*G)*dGdYk*dGdXj;
                hessian[3*k-1,3*j  ]       += +2*K_theta/(1-G*G)*dGdYk*dGdZj;
                hessian[3*k  ,3*j-2]       += +2*K_theta/(1-G*G)*dGdZk*dGdXj;
                hessian[3*k  ,3*j-1]       += +2*K_theta/(1-G*G)*dGdZk*dGdYj;                                                                       
            
                //Hik
                //d^2V/dXidXk  d^2V/dXidYk  d^2V/dXidZk
                //d^2V/dYidXk  d^2V/dYidYk  d^2V/dYidZk
                //d^2V/dZidXk  d^2V/dZidYk  d^2V/dZidZk
                                    
                // diagonals of off-diagonal super elements

                hessian[3*i-2,3*k-2]       += +2*K_theta/(1-G*G)*dGdXi*dGdXk;
                hessian[3*i-1,3*k-1]       += +2*K_theta/(1-G*G)*dGdYi*dGdYk;
                hessian[3*i  ,3*k  ]       += +2*K_theta/(1-G*G)*dGdZi*dGdZk;
            
                // off-diagonals of off-diagonal super elements

                hessian[3*i-2,3*k-1]       += +2*K_theta/(1-G*G)*dGdXi*dGdYk;
                hessian[3*i-2,3*k  ]       += +2*K_theta/(1-G*G)*dGdXi*dGdZk;
                hessian[3*i-1,3*k-2]       += +2*K_theta/(1-G*G)*dGdYi*dGdXk;
                hessian[3*i-1,3*k  ]       += +2*K_theta/(1-G*G)*dGdYi*dGdZk;
                hessian[3*i  ,3*k-2]       += +2*K_theta/(1-G*G)*dGdZi*dGdXk;
                hessian[3*i  ,3*k-1]       += +2*K_theta/(1-G*G)*dGdZi*dGdYk;                                                                       

                //Hki
            
                // diagonals of off-diagonal super elements

                hessian[3*k-2,3*i-2]       += +2*K_theta/(1-G*G)*dGdXk*dGdXi;
                hessian[3*k-1,3*i-1]       += +2*K_theta/(1-G*G)*dGdYk*dGdYi;
                hessian[3*k  ,3*i  ]       += +2*K_theta/(1-G*G)*dGdZk*dGdZi;
            
                // off-diagonals of off-diagonal super elements

                hessian[3*k-2,3*i-1]       += +2*K_theta/(1-G*G)*dGdXk*dGdYi;
                hessian[3*k-2,3*i  ]       += +2*K_theta/(1-G*G)*dGdXk*dGdZi;
                hessian[3*k-1,3*i-2]       += +2*K_theta/(1-G*G)*dGdYk*dGdXi;
                hessian[3*k-1,3*i  ]       += +2*K_theta/(1-G*G)*dGdYk*dGdZi;
                hessian[3*k  ,3*i-2]       += +2*K_theta/(1-G*G)*dGdZk*dGdXi;
                hessian[3*k  ,3*i-1]       += +2*K_theta/(1-G*G)*dGdZk*dGdYi;                                                                       
                
                //Hii
                //update the diagonals of diagonal super elements

                hessian[3*i-2,3*i-2]       += +2*K_theta/(1-G*G)*dGdXi*dGdXi;
                hessian[3*i-1,3*i-1]       += +2*K_theta/(1-G*G)*dGdYi*dGdYi;
                hessian[3*i  ,3*i  ]       += +2*K_theta/(1-G*G)*dGdZi*dGdZi;                        

                // update the off-diagonals of diagonal super elements            
                hessian[3*i-2,3*i-1]       += +2*K_theta/(1-G*G)*dGdXi*dGdYi;
                hessian[3*i-2,3*i  ]       += +2*K_theta/(1-G*G)*dGdXi*dGdZi;
                hessian[3*i-1,3*i-2]       += +2*K_theta/(1-G*G)*dGdYi*dGdXi;
                hessian[3*i-1,3*i  ]       += +2*K_theta/(1-G*G)*dGdYi*dGdZi;
                hessian[3*i  ,3*i-2]       += +2*K_theta/(1-G*G)*dGdZi*dGdXi;
                hessian[3*i  ,3*i-1]       += +2*K_theta/(1-G*G)*dGdZi*dGdYi;                        

                //Hjj
                //update the diagonals of diagonal super elements

                hessian[3*j-2,3*j-2]       += +2*K_theta/(1-G*G)*dGdXj*dGdXj;
                hessian[3*j-1,3*j-1]       += +2*K_theta/(1-G*G)*dGdYj*dGdYj;
                hessian[3*j  ,3*j  ]       += +2*K_theta/(1-G*G)*dGdZj*dGdZj;                        

                // update the off-diagonals of diagonal super elements            
                hessian[3*j-2,3*j-1]       += +2*K_theta/(1-G*G)*dGdXj*dGdYj;
                hessian[3*j-2,3*j  ]       += +2*K_theta/(1-G*G)*dGdXj*dGdZj;
                hessian[3*j-1,3*j-2]       += +2*K_theta/(1-G*G)*dGdYj*dGdXj;
                hessian[3*j-1,3*j  ]       += +2*K_theta/(1-G*G)*dGdYj*dGdZj;
                hessian[3*j  ,3*j-2]       += +2*K_theta/(1-G*G)*dGdZj*dGdXj;
                hessian[3*j  ,3*j-1]       += +2*K_theta/(1-G*G)*dGdZj*dGdYj;                                                                                        

                //Hkk
                //update the diagonals of diagonal super elements

                hessian[3*k-2,3*k-2]       += +2*K_theta/(1-G*G)*dGdXk*dGdXk;
                hessian[3*k-1,3*k-1]       += +2*K_theta/(1-G*G)*dGdYk*dGdYk;
                hessian[3*k  ,3*k  ]       += +2*K_theta/(1-G*G)*dGdZk*dGdZk;                        

                // update the off-diagonals of diagonal super elements            
                hessian[3*k-2,3*k-1]       += +2*K_theta/(1-G*G)*dGdXk*dGdYk;
                hessian[3*k-2,3*k  ]       += +2*K_theta/(1-G*G)*dGdXk*dGdZk;
                hessian[3*k-1,3*k-2]       += +2*K_theta/(1-G*G)*dGdYk*dGdXk;
                hessian[3*k-1,3*k  ]       += +2*K_theta/(1-G*G)*dGdYk*dGdZk;
                hessian[3*k  ,3*k-2]       += +2*K_theta/(1-G*G)*dGdZk*dGdXk;
                hessian[3*k  ,3*k-1]       += +2*K_theta/(1-G*G)*dGdZk*dGdYk;
            }

            if(hessian.matrix is HessMatrix)
                return (hessian.matrix as HessMatrix);
            return HessMatrix.FromMatrix( hessian.matrix );
        }

        public static HessMatrix thirdTerm(IList<Vector> caArray_, MatrixByArr distance_, int numOfResidues, FuncIJKL func_K_phi1, FuncIJKL func_K_phi3, HessMatrix hessian_=null)
        {
            VECTORS caArray = new VECTORS(caArray_);
            MATRIX distance = new MATRIX(distance_);
            if(hessian_ == null)
                hessian_ = HessMatrix.ZerosHessMatrix(caArray_.Count*3, caArray_.Count*3);
            MATRIX hessian = new MATRIX(hessian_);

            for(int m=3; m<=numOfResidues-1; m++)
            {
                int i=m-2;
                int j=m-1;
                int k=m;
                int l=m+1;

                double K_phi1 = func_K_phi1(i-1, j-1, k-1, l-1);
                double K_phi3 = func_K_phi3(i-1, j-1, k-1, l-1);
                double K_phi=K_phi1/2+K_phi3*9/2;

                double Xi=caArray[i,1];
                double Yi=caArray[i,2];
                double Zi=caArray[i,3];
                double Xj=caArray[j,1];
                double Yj=caArray[j,2];
                double Zj=caArray[j,3];
                double Xk=caArray[k,1];
                double Yk=caArray[k,2];
                double Zk=caArray[k,3];
                double Xl=caArray[l,1];
                double Yl=caArray[l,2];
                double Zl=caArray[l,3];
                Vector a=caArray.GetRow(j)-caArray.GetRow(i);
                Vector b=caArray.GetRow(k)-caArray.GetRow(j);
                Vector c=caArray.GetRow(l)-caArray.GetRow(k);
                Vector v1=cross(a,b);
                Vector v2=cross(b,c);
                double lv1l=glength(v1);
                double lv2l=glength(v2);
                double G=dot(v1,v2)/(lv1l*lv2l);

                /* dv1/dXi */ Vector dv1dXi=new double[]{0     ,Zk-Zj ,Yj-Yk};
                /* dv1/dYi */ Vector dv1dYi=new double[]{Zj-Zk ,0     ,Xk-Xj};
                /* dv1/dZi */ Vector dv1dZi=new double[]{Yk-Yj ,Xj-Xk ,0    };

                /* dv1/dXj */ Vector dv1dXj=new double[]{0     ,Zi-Zk ,Yk-Yi};
                /* dv1/dYj */ Vector dv1dYj=new double[]{Zk-Zi ,0     ,Xi-Xk};
                /* dv1/dZj */ Vector dv1dZj=new double[]{Yi-Yk ,Xk-Xi ,0    };

                /* dv1/dXk */ Vector dv1dXk=new double[]{0     ,Zj-Zi ,Yi-Yj};
                /* dv2/dYk */ Vector dv1dYk=new double[]{Zi-Zj ,0     ,Xj-Xi};
                /* dv2/dZk */ Vector dv1dZk=new double[]{Yj-Yi ,Xi-Xj ,0    };

                /* dv1/dXl */ Vector dv1dXl=new double[]{0     ,0     ,0    };
                /* dv1/dYl */ Vector dv1dYl=new double[]{0     ,0     ,0    };
                /* dv1/dZl */ Vector dv1dZl=new double[]{0     ,0     ,0    };

                /* dv2/dXi */ Vector dv2dXi=new double[]{0     ,0     ,0    };
                /* dv2/dYi */ Vector dv2dYi=new double[]{0     ,0     ,0    };
                /* dv2/dZi */ Vector dv2dZi=new double[]{0     ,0     ,0    };

                /* dv2/dXj */ Vector dv2dXj=new double[]{0     ,Zl-Zk ,Yk-Yl};
                /* dv2/dYj */ Vector dv2dYj=new double[]{Zk-Zl ,0     ,Xl-Xk};
                /* dv2/dZj */ Vector dv2dZj=new double[]{Yl-Yk ,Xk-Xl ,0    };

                /* dv2/dXk */ Vector dv2dXk=new double[]{0     ,Zj-Zl ,Yl-Yj};
                /* dv2/dYk */ Vector dv2dYk=new double[]{Zl-Zj ,0     ,Xj-Xl};
                /* dv2/dZk */ Vector dv2dZk=new double[]{Yj-Yl ,Xl-Xj ,0    };

                /* dv2/dXl */ Vector dv2dXl=new double[]{0     ,Zk-Zj ,Yj-Yk};
                /* dv2/dYl */ Vector dv2dYl=new double[]{Zj-Zk ,0     ,Xk-Xj};
                /* dv2/dZl */ Vector dv2dZl=new double[]{Yk-Yj ,Xj-Xk ,0    };
               
                double K1=(Yj-Yi)*(Zk-Zj)-(Yk-Yj)*(Zj-Zi); double K1_2 = K1*K1;
                double K2=(Xk-Xj)*(Zj-Zi)-(Xj-Xi)*(Zk-Zj); double K2_2 = K2*K2;
                double K3=(Xj-Xi)*(Yk-Yj)-(Xk-Xj)*(Yj-Yi); double K3_2 = K3*K3;

                /* dlv1l/dXi */ double dlv1ldXi=(2*K2*(Zk-Zj)+2*K3*(Yj-Yk))/(2*sqrt(K1_2+K2_2+K3_2));
                /* dlv1l/dYi */ double dlv1ldYi=(2*K1*(Zj-Zk)+2*K3*(Xk-Xj))/(2*sqrt(K1_2+K2_2+K3_2));
                /* dlv1l/dZi */ double dlv1ldZi=(2*K1*(Yk-Yj)+2*K2*(Xj-Xk))/(2*sqrt(K1_2+K2_2+K3_2));

                /* dlv1ldXj */ double dlv1ldXj=(2*K2*(Zi-Zk)+2*K3*(Yk-Yi))/(2*sqrt(K1_2+K2_2+K3_2));
                /* dlv1ldYj */ double dlv1ldYj=(2*K1*(Zk-Zi)+2*K3*(Xi-Xk))/(2*sqrt(K1_2+K2_2+K3_2));
                /* dlv1ldZj */ double dlv1ldZj=(2*K1*(Yi-Yk)+2*K2*(Xk-Xi))/(2*sqrt(K1_2+K2_2+K3_2));

                /* dlv1ldXk */ double dlv1ldXk=(2*K2*(Zj-Zi)+2*K3*(Yi-Yj))/(2*sqrt(K1_2+K2_2+K3_2));
                /* dlv1ldYk */ double dlv1ldYk=(2*K1*(Zi-Zj)+2*K3*(Xj-Xi))/(2*sqrt(K1_2+K2_2+K3_2));
                /* dlv1ldZk */ double dlv1ldZk=(2*K1*(Yj-Yi)+2*K2*(Xi-Xj))/(2*sqrt(K1_2+K2_2+K3_2));

                /* dlv1ldXl */ double dlv1ldXl=0;
                /* dlv1ldYl */ double dlv1ldYl=0;
                /* dlv1ldZl */ double dlv1ldZl=0;

                double L1=(Yk-Yj)*(Zl-Zk)-(Yl-Yk)*(Zk-Zj); double L1_2 = L1 * L1;
                double L2=(Xl-Xk)*(Zk-Zj)-(Xk-Xj)*(Zl-Zk); double L2_2 = L2 * L2;
                double L3=(Xk-Xj)*(Yl-Yk)-(Xl-Xk)*(Yk-Yj); double L3_2 = L3 * L3;

                /* dlv2l/dXi */ double dlv2ldXi=0;
                /* dlv2l/dYi */ double dlv2ldYi=0;
                /* dlv2l/dZi */ double dlv2ldZi=0;

                /* dlv2l/dXj */ double dlv2ldXj=(2*L2*(Zl-Zk)+2*L3*(Yk-Yl))/(2*sqrt(L1_2+L2_2+L3_2));
                /* dlv2l/dYj */ double dlv2ldYj=(2*L1*(Zk-Zl)+2*L3*(Xl-Xk))/(2*sqrt(L1_2+L2_2+L3_2));
                /* dlv2l/dZj */ double dlv2ldZj=(2*L1*(Yl-Yk)+2*L2*(Xk-Xl))/(2*sqrt(L1_2+L2_2+L3_2));

                /* dlv2l/dXk */ double dlv2ldXk=(2*L2*(Zj-Zl)+2*L3*(Yl-Yj))/(2*sqrt(L1_2+L2_2+L3_2));
                /* dlv2l/dYk */ double dlv2ldYk=(2*L1*(Zl-Zj)+2*L3*(Xj-Xl))/(2*sqrt(L1_2+L2_2+L3_2));
                /* dlv2l/dZk */ double dlv2ldZk=(2*L1*(Yj-Yl)+2*L2*(Xl-Xj))/(2*sqrt(L1_2+L2_2+L3_2));

                /* dlv2l/dXl */ double dlv2ldXl=(2*L2*(Zk-Zj)+2*L3*(Yj-Yk))/(2*sqrt(L1_2+L2_2+L3_2));
                /* dlv2l/dYl */ double dlv2ldYl=(2*L1*(Zj-Zk)+2*L3*(Xk-Xj))/(2*sqrt(L1_2+L2_2+L3_2));
                /* dlv2l/dZl */ double dlv2ldZl=(2*L1*(Yk-Yj)+2*L2*(Xj-Xk))/(2*sqrt(L1_2+L2_2+L3_2));

                /* dG/dXi */ double dGdXi=((dot(dv1dXi,v2)+dot(dv2dXi,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldXi*lv2l+dlv2ldXi*lv1l))/pow(lv1l*lv2l);
                /* dG/dYi */ double dGdYi=((dot(dv1dYi,v2)+dot(dv2dYi,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldYi*lv2l+dlv2ldYi*lv1l))/pow(lv1l*lv2l);
                /* dG/dZi */ double dGdZi=((dot(dv1dZi,v2)+dot(dv2dZi,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldZi*lv2l+dlv2ldZi*lv1l))/pow(lv1l*lv2l);

                /* dG/dXj */ double dGdXj=((dot(dv1dXj,v2)+dot(dv2dXj,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldXj*lv2l+dlv2ldXj*lv1l))/pow(lv1l*lv2l);     
                /* dG/dYj */ double dGdYj=((dot(dv1dYj,v2)+dot(dv2dYj,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldYj*lv2l+dlv2ldYj*lv1l))/pow(lv1l*lv2l);
                /* dG/dZj */ double dGdZj=((dot(dv1dZj,v2)+dot(dv2dZj,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldZj*lv2l+dlv2ldZj*lv1l))/pow(lv1l*lv2l);

                /* dG/dXk */ double dGdXk=((dot(dv1dXk,v2)+dot(dv2dXk,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldXk*lv2l+dlv2ldXk*lv1l))/pow(lv1l*lv2l);
                /* dG/dYk */ double dGdYk=((dot(dv1dYk,v2)+dot(dv2dYk,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldYk*lv2l+dlv2ldYk*lv1l))/pow(lv1l*lv2l);
                /* dG/dZk */ double dGdZk=((dot(dv1dZk,v2)+dot(dv2dZk,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldZk*lv2l+dlv2ldZk*lv1l))/pow(lv1l*lv2l);

                /* dG/dXl */ double dGdXl=((dot(dv1dXl,v2)+dot(dv2dXl,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldXl*lv2l+dlv2ldXl*lv1l))/pow(lv1l*lv2l);
                /* dG/dYl */ double dGdYl=((dot(dv1dYl,v2)+dot(dv2dYl,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldYl*lv2l+dlv2ldYl*lv1l))/pow(lv1l*lv2l);
                /* dG/dZl */ double dGdZl=((dot(dv1dZl,v2)+dot(dv2dZl,v1))*lv1l*lv2l-dot(v1,v2)*(dlv1ldZl*lv2l+dlv2ldZl*lv1l))/pow(lv1l*lv2l);                       

                //Hij
                //d^2V/dXidXj  d^2V/dXidYj  d^2V/dXidZj
                //d^2V/dYidXj  d^2V/dYidYj  d^2V/dYidZj
                //d^2V/dZidXj  d^2V/dZidYj  d^2V/dZidZj               

                // diagonals of off-diagonal super elements             

                hessian[3*i-2,3*j-2]       += +(2*K_phi)/(1-G*G)*dGdXi*dGdXj;
                hessian[3*i-1,3*j-1]       += +(2*K_phi)/(1-G*G)*dGdYi*dGdYj;
                hessian[3*i  ,3*j  ]       += +(2*K_phi)/(1-G*G)*dGdZi*dGdZj;

                // off-diagonals of off-diagonal super elements

                hessian[3*i-2,3*j-1]       += +(2*K_phi)/(1-G*G)*dGdXi*dGdYj;
                hessian[3*i-2,3*j  ]       += +(2*K_phi)/(1-G*G)*dGdXi*dGdZj;
                hessian[3*i-1,3*j-2]       += +(2*K_phi)/(1-G*G)*dGdYi*dGdXj;
                hessian[3*i-1,3*j  ]       += +(2*K_phi)/(1-G*G)*dGdYi*dGdZj;
                hessian[3*i  ,3*j-2]       += +(2*K_phi)/(1-G*G)*dGdZi*dGdXj;
                hessian[3*i  ,3*j-1]       += +(2*K_phi)/(1-G*G)*dGdZi*dGdYj;

                //Hji
                // diagonals of off-diagonal super elements
                hessian[3*j-2,3*i-2]       += +(2*K_phi)/(1-G*G)*dGdXj*dGdXi;
                hessian[3*j-1,3*i-1]       += +(2*K_phi)/(1-G*G)*dGdYj*dGdYi;
                hessian[3*j  ,3*i  ]       += +(2*K_phi)/(1-G*G)*dGdZj*dGdZi;

                // off-diagonals of off-diagonal super elements

                hessian[3*j-2,3*i-1]       += +(2*K_phi)/(1-G*G)*dGdXj*dGdYi;
                hessian[3*j-2,3*i  ]       += +(2*K_phi)/(1-G*G)*dGdXj*dGdZi;
                hessian[3*j-1,3*i-2]       += +(2*K_phi)/(1-G*G)*dGdYj*dGdXi;
                hessian[3*j-1,3*i  ]       += +(2*K_phi)/(1-G*G)*dGdYj*dGdZi;
                hessian[3*j  ,3*i-2]       += +(2*K_phi)/(1-G*G)*dGdZj*dGdXi;
                hessian[3*j  ,3*i-1]       += +(2*K_phi)/(1-G*G)*dGdZj*dGdYi;                                                                                                   

                //Hil

                // diagonals of off-diagonal super elements

                hessian[3*i-2,3*l-2]       += +(2*K_phi)/(1-G*G)*dGdXi*dGdXl;
                hessian[3*i-1,3*l-1]       += +(2*K_phi)/(1-G*G)*dGdYi*dGdYl;
                hessian[3*i  ,3*l  ]       += +(2*K_phi)/(1-G*G)*dGdZi*dGdZl;

                // off-diagonals of off-diagonal super elements

                hessian[3*i-2,3*l-1]       += +(2*K_phi)/(1-G*G)*dGdXi*dGdYl;
                hessian[3*i-2,3*l  ]       += +(2*K_phi)/(1-G*G)*dGdXi*dGdZl;
                hessian[3*i-1,3*l-2]       += +(2*K_phi)/(1-G*G)*dGdYi*dGdXl;
                hessian[3*i-1,3*l  ]       += +(2*K_phi)/(1-G*G)*dGdYi*dGdZl;
                hessian[3*i  ,3*l-2]       += +(2*K_phi)/(1-G*G)*dGdZi*dGdXl;
                hessian[3*i  ,3*l-1]       += +(2*K_phi)/(1-G*G)*dGdZi*dGdYl;

                //Hli
                // diagonals of off-diagonal super elements
                hessian[3*l-2,3*i-2]       += +(2*K_phi)/(1-G*G)*dGdXl*dGdXi;
                hessian[3*l-1,3*i-1]       += +(2*K_phi)/(1-G*G)*dGdYl*dGdYi;
                hessian[3*l  ,3*i  ]       += +(2*K_phi)/(1-G*G)*dGdZl*dGdZi;

                // off-diagonals of off-diagonal super elements

                hessian[3*l-2,3*i-1]       += +(2*K_phi)/(1-G*G)*dGdXl*dGdYi;
                hessian[3*l-2,3*i  ]       += +(2*K_phi)/(1-G*G)*dGdXl*dGdZi;
                hessian[3*l-1,3*i-2]       += +(2*K_phi)/(1-G*G)*dGdYl*dGdXi;
                hessian[3*l-1,3*i  ]       += +(2*K_phi)/(1-G*G)*dGdYl*dGdZi;
                hessian[3*l  ,3*i-2]       += +(2*K_phi)/(1-G*G)*dGdZl*dGdXi;
                hessian[3*l  ,3*i-1]       += +(2*K_phi)/(1-G*G)*dGdZl*dGdYi;

                //Hkj        
                // diagonals of off-diagonal super elements

                hessian[3*k-2,3*j-2]       += +(2*K_phi)/(1-G*G)*dGdXk*dGdXj;
                hessian[3*k-1,3*j-1]       += +(2*K_phi)/(1-G*G)*dGdYk*dGdYj;
                hessian[3*k  ,3*j  ]       += +(2*K_phi)/(1-G*G)*dGdZk*dGdZj;

                // off-diagonals of off-diagonal super elements

                hessian[3*k-2,3*j-1]       += +(2*K_phi)/(1-G*G)*dGdXk*dGdYj;
                hessian[3*k-2,3*j  ]       += +(2*K_phi)/(1-G*G)*dGdXk*dGdZj;
                hessian[3*k-1,3*j-2]       += +(2*K_phi)/(1-G*G)*dGdYk*dGdXj;
                hessian[3*k-1,3*j  ]       += +(2*K_phi)/(1-G*G)*dGdYk*dGdZj;
                hessian[3*k  ,3*j-2]       += +(2*K_phi)/(1-G*G)*dGdZk*dGdXj;
                hessian[3*k  ,3*j-1]       += +(2*K_phi)/(1-G*G)*dGdZk*dGdYj;

                //Hjk
                // diagonals of off-diagonal super elements
                hessian[3*j-2,3*k-2]       += +(2*K_phi)/(1-G*G)*dGdXj*dGdXk;
                hessian[3*j-1,3*k-1]       += +(2*K_phi)/(1-G*G)*dGdYj*dGdYk;
                hessian[3*j  ,3*k  ]       += +(2*K_phi)/(1-G*G)*dGdZj*dGdZk;

                // off-diagonals of off-diagonal super elements

                hessian[3*j-2,3*k-1]       += +(2*K_phi)/(1-G*G)*dGdXj*dGdYk;
                hessian[3*j-2,3*k  ]       += +(2*K_phi)/(1-G*G)*dGdXj*dGdZk;
                hessian[3*j-1,3*k-2]       += +(2*K_phi)/(1-G*G)*dGdYj*dGdXk;
                hessian[3*j-1,3*k  ]       += +(2*K_phi)/(1-G*G)*dGdYj*dGdZk;
                hessian[3*j  ,3*k-2]       += +(2*K_phi)/(1-G*G)*dGdZj*dGdXk;
                hessian[3*j  ,3*k-1]       += +(2*K_phi)/(1-G*G)*dGdZj*dGdYk;

                //Hik            
                // diagonals of off-diagonal super elements

                hessian[3*i-2,3*k-2]       += +(2*K_phi)/(1-G*G)*dGdXi*dGdXk;
                hessian[3*i-1,3*k-1]       += +(2*K_phi)/(1-G*G)*dGdYi*dGdYk;
                hessian[3*i  ,3*k  ]       += +(2*K_phi)/(1-G*G)*dGdZi*dGdZk;

                // off-diagonals of off-diagonal super elements

                hessian[3*i-2,3*k-1]       += +(2*K_phi)/(1-G*G)*dGdXi*dGdYk;
                hessian[3*i-2,3*k  ]       += +(2*K_phi)/(1-G*G)*dGdXi*dGdZk;
                hessian[3*i-1,3*k-2]       += +(2*K_phi)/(1-G*G)*dGdYi*dGdXk;
                hessian[3*i-1,3*k  ]       += +(2*K_phi)/(1-G*G)*dGdYi*dGdZk;
                hessian[3*i  ,3*k-2]       += +(2*K_phi)/(1-G*G)*dGdZi*dGdXk;
                hessian[3*i  ,3*k-1]       += +(2*K_phi)/(1-G*G)*dGdZi*dGdYk;

                //Hki
                // diagonals of off-diagonal super elements            
                hessian[3*k-2,3*i-2]       += +(2*K_phi)/(1-G*G)*dGdXk*dGdXi;
                hessian[3*k-1,3*i-1]       += +(2*K_phi)/(1-G*G)*dGdYk*dGdYi;
                hessian[3*k  ,3*i  ]       += +(2*K_phi)/(1-G*G)*dGdZk*dGdZi;

                // off-diagonals of off-diagonal super elements

                hessian[3*k-2,3*i-1]       += +(2*K_phi)/(1-G*G)*dGdXk*dGdYi;
                hessian[3*k-2,3*i  ]       += +(2*K_phi)/(1-G*G)*dGdXk*dGdZi;
                hessian[3*k-1,3*i-2]       += +(2*K_phi)/(1-G*G)*dGdYk*dGdXi;
                hessian[3*k-1,3*i  ]       += +(2*K_phi)/(1-G*G)*dGdYk*dGdZi;
                hessian[3*k  ,3*i-2]       += +(2*K_phi)/(1-G*G)*dGdZk*dGdXi;
                hessian[3*k  ,3*i-1]       += +(2*K_phi)/(1-G*G)*dGdZk*dGdYi;

                //Hlj
                // diagonals of off-diagonal super elements

                hessian[3*l-2,3*j-2]       += +(2*K_phi)/(1-G*G)*dGdXl*dGdXj;
                hessian[3*l-1,3*j-1]       += +(2*K_phi)/(1-G*G)*dGdYl*dGdYj;
                hessian[3*l  ,3*j  ]       += +(2*K_phi)/(1-G*G)*dGdZl*dGdZj;

                // off-diagonals of off-diagonal super elements

                hessian[3*l-2,3*j-1]       += +(2*K_phi)/(1-G*G)*dGdXl*dGdYj;
                hessian[3*l-2,3*j  ]       += +(2*K_phi)/(1-G*G)*dGdXl*dGdZj;
                hessian[3*l-1,3*j-2]       += +(2*K_phi)/(1-G*G)*dGdYl*dGdXj;
                hessian[3*l-1,3*j  ]       += +(2*K_phi)/(1-G*G)*dGdYl*dGdZj;
                hessian[3*l  ,3*j-2]       += +(2*K_phi)/(1-G*G)*dGdZl*dGdXj;
                hessian[3*l  ,3*j-1]       += +(2*K_phi)/(1-G*G)*dGdZl*dGdYj;

                //Hjl
                // diagonals of off-diagonal super elements
                hessian[3*j-2,3*l-2]       += +(2*K_phi)/(1-G*G)*dGdXj*dGdXl;
                hessian[3*j-1,3*l-1]       += +(2*K_phi)/(1-G*G)*dGdYj*dGdYl;
                hessian[3*j  ,3*l  ]       += +(2*K_phi)/(1-G*G)*dGdZj*dGdZl;

                // off-diagonals of off-diagonal super elements

                hessian[3*j-2,3*l-1]       += +(2*K_phi)/(1-G*G)*dGdXj*dGdYl;
                hessian[3*j-2,3*l  ]       += +(2*K_phi)/(1-G*G)*dGdXj*dGdZl;
                hessian[3*j-1,3*l-2]       += +(2*K_phi)/(1-G*G)*dGdYj*dGdXl;
                hessian[3*j-1,3*l  ]       += +(2*K_phi)/(1-G*G)*dGdYj*dGdZl;
                hessian[3*j  ,3*l-2]       += +(2*K_phi)/(1-G*G)*dGdZj*dGdXl;
                hessian[3*j  ,3*l-1]       += +(2*K_phi)/(1-G*G)*dGdZj*dGdYl;

                //Hlk
                // diagonals of off-diagonal super elements

                hessian[3*l-2,3*k-2]       += +(2*K_phi)/(1-G*G)*dGdXl*dGdXk;
                hessian[3*l-1,3*k-1]       += +(2*K_phi)/(1-G*G)*dGdYl*dGdYk;
                hessian[3*l  ,3*k  ]       += +(2*K_phi)/(1-G*G)*dGdZl*dGdZk;

                // off-diagonals of off-diagonal super elements

                hessian[3*l-2,3*k-1]       += +(2*K_phi)/(1-G*G)*dGdXl*dGdYk;
                hessian[3*l-2,3*k  ]       += +(2*K_phi)/(1-G*G)*dGdXl*dGdZk;
                hessian[3*l-1,3*k-2]       += +(2*K_phi)/(1-G*G)*dGdYl*dGdXk;
                hessian[3*l-1,3*k  ]       += +(2*K_phi)/(1-G*G)*dGdYl*dGdZk;
                hessian[3*l  ,3*k-2]       += +(2*K_phi)/(1-G*G)*dGdZl*dGdXk;
                hessian[3*l  ,3*k-1]       += +(2*K_phi)/(1-G*G)*dGdZl*dGdYk;

                //Hkl
                // diagonals of off-diagonal super elements
                hessian[3*k-2,3*l-2]       += +(2*K_phi)/(1-G*G)*dGdXk*dGdXl;
                hessian[3*k-1,3*l-1]       += +(2*K_phi)/(1-G*G)*dGdYk*dGdYl;
                hessian[3*k  ,3*l  ]       += +(2*K_phi)/(1-G*G)*dGdZk*dGdZl;

                // off-diagonals of off-diagonal super elements

                hessian[3*k-2,3*l-1]       += +(2*K_phi)/(1-G*G)*dGdXk*dGdYl;
                hessian[3*k-2,3*l  ]       += +(2*K_phi)/(1-G*G)*dGdXk*dGdZl;
                hessian[3*k-1,3*l-2]       += +(2*K_phi)/(1-G*G)*dGdYk*dGdXl;
                hessian[3*k-1,3*l  ]       += +(2*K_phi)/(1-G*G)*dGdYk*dGdZl;
                hessian[3*k  ,3*l-2]       += +(2*K_phi)/(1-G*G)*dGdZk*dGdXl;
                hessian[3*k  ,3*l-1]       += +(2*K_phi)/(1-G*G)*dGdZk*dGdYl;

                //Hii
                //update the diagonals of diagonal super elements

                hessian[3*i-2,3*i-2]       += +2*K_phi/(1-G*G)*dGdXi*dGdXi;
                hessian[3*i-1,3*i-1]       += +2*K_phi/(1-G*G)*dGdYi*dGdYi;
                hessian[3*i  ,3*i  ]       += +2*K_phi/(1-G*G)*dGdZi*dGdZi;

                // update the off-diagonals of diagonal super elements
                hessian[3*i-2,3*i-1]       += +2*K_phi/(1-G*G)*dGdXi*dGdYi;
                hessian[3*i-2,3*i  ]       += +2*K_phi/(1-G*G)*dGdXi*dGdZi;
                hessian[3*i-1,3*i-2]       += +2*K_phi/(1-G*G)*dGdYi*dGdXi;
                hessian[3*i-1,3*i  ]       += +2*K_phi/(1-G*G)*dGdYi*dGdZi;
                hessian[3*i  ,3*i-2]       += +2*K_phi/(1-G*G)*dGdZi*dGdXi;
                hessian[3*i  ,3*i-1]       += +2*K_phi/(1-G*G)*dGdZi*dGdYi;

                //Hjj
                //update the diagonals of diagonal super elements

                hessian[3*j-2,3*j-2]       += +2*K_phi/(1-G*G)*dGdXj*dGdXj;
                hessian[3*j-1,3*j-1]       += +2*K_phi/(1-G*G)*dGdYj*dGdYj;
                hessian[3*j  ,3*j  ]       += +2*K_phi/(1-G*G)*dGdZj*dGdZj;

                // update the off-diagonals of diagonal super elements
                hessian[3*j-2,3*j-1]       += +2*K_phi/(1-G*G)*dGdXj*dGdYj;
                hessian[3*j-2,3*j  ]       += +2*K_phi/(1-G*G)*dGdXj*dGdZj;
                hessian[3*j-1,3*j-2]       += +2*K_phi/(1-G*G)*dGdYj*dGdXj;
                hessian[3*j-1,3*j  ]       += +2*K_phi/(1-G*G)*dGdYj*dGdZj;
                hessian[3*j  ,3*j-2]       += +2*K_phi/(1-G*G)*dGdZj*dGdXj;
                hessian[3*j  ,3*j-1]       += +2*K_phi/(1-G*G)*dGdZj*dGdYj;

                //Hkk
                //update the diagonals of diagonal super elements

                hessian[3*k-2,3*k-2]       += +2*K_phi/(1-G*G)*dGdXk*dGdXk;
                hessian[3*k-1,3*k-1]       += +2*K_phi/(1-G*G)*dGdYk*dGdYk;
                hessian[3*k  ,3*k  ]       += +2*K_phi/(1-G*G)*dGdZk*dGdZk;

                // update the off-diagonals of diagonal super elements            
                hessian[3*k-2,3*k-1]       += +2*K_phi/(1-G*G)*dGdXk*dGdYk;
                hessian[3*k-2,3*k  ]       += +2*K_phi/(1-G*G)*dGdXk*dGdZk;
                hessian[3*k-1,3*k-2]       += +2*K_phi/(1-G*G)*dGdYk*dGdXk;
                hessian[3*k-1,3*k  ]       += +2*K_phi/(1-G*G)*dGdYk*dGdZk;
                hessian[3*k  ,3*k-2]       += +2*K_phi/(1-G*G)*dGdZk*dGdXk;
                hessian[3*k  ,3*k-1]       += +2*K_phi/(1-G*G)*dGdZk*dGdYk;

                //Hll
                //update the diagonals of diagonal super elements

                hessian[3*l-2,3*l-2]       += +2*K_phi/(1-G*G)*dGdXl*dGdXl;
                hessian[3*l-1,3*l-1]       += +2*K_phi/(1-G*G)*dGdYl*dGdYl;
                hessian[3*l  ,3*l  ]       += +2*K_phi/(1-G*G)*dGdZl*dGdZl;

                // update the off-diagonals of diagonal super elements
                hessian[3*l-2,3*l-1]       += +2*K_phi/(1-G*G)*dGdXl*dGdYl;
                hessian[3*l-2,3*l  ]       += +2*K_phi/(1-G*G)*dGdXl*dGdZl;
                hessian[3*l-1,3*l-2]       += +2*K_phi/(1-G*G)*dGdYl*dGdXl;
                hessian[3*l-1,3*l  ]       += +2*K_phi/(1-G*G)*dGdYl*dGdZl;
                hessian[3*l  ,3*l-2]       += +2*K_phi/(1-G*G)*dGdZl*dGdXl;
                hessian[3*l  ,3*l-1]       += +2*K_phi/(1-G*G)*dGdZl*dGdYl;
            }

            if(hessian.matrix is HessMatrix)
                return (hessian.matrix as HessMatrix);
            return HessMatrix.FromMatrix( hessian.matrix );
        }

        public static HessMatrix fourthTerm(IList<Vector> caArray_, MatrixByArr distance_, int numOfResidues, FuncIJ func_Epsilon, HessMatrix hessian_=null)
        {
            VECTORS caArray = new VECTORS(caArray_);
            MATRIX distance = new MATRIX(distance_);
            if(hessian_ == null)
                hessian_ = HessMatrix.ZerosHessMatrix(caArray_.Count*3, caArray_.Count*3);
            MATRIX hessian = new MATRIX(hessian_);

            // derive the hessian of the first term (off diagonal)
            for(int i=1; i<=numOfResidues; i++)
            {
                for(int j=1; j<=numOfResidues; j++)
                {
                    if(abs(i-j)>3)
                    {
                        double Epsilon = func_Epsilon(i-1, j-1);

                        double bx=caArray[i,1] - caArray[j,1];
                        double by=caArray[i,2] - caArray[j,2];
                        double bz=caArray[i,3] - caArray[j,3];
                        double distijsqr = pow(distance[i,j], 4);

                        // diagonals of off-diagonal super elements (1st term)
                        hessian[3*i-2,3*j-2]       += -120*Epsilon*bx*bx/distijsqr;
                        hessian[3*i-1,3*j-1]       += -120*Epsilon*by*by/distijsqr;
                        hessian[3*i  ,3*j  ]       += -120*Epsilon*bz*bz/distijsqr;

                        // off-diagonals of off-diagonal super elements (1st term)

                        hessian[3*i-2,3*j-1]       += -120*Epsilon*bx*by/distijsqr;
                        hessian[3*i-2,3*j  ]       += -120*Epsilon*bx*bz/distijsqr;
                        hessian[3*i-1,3*j-2]       += -120*Epsilon*by*bx/distijsqr;
                        hessian[3*i-1,3*j  ]       += -120*Epsilon*by*bz/distijsqr;
                        hessian[3*i  ,3*j-2]       += -120*Epsilon*bx*bz/distijsqr;
                        hessian[3*i  ,3*j-1]       += -120*Epsilon*by*bz/distijsqr;

                        //Hii
                        //update the diagonals of diagonal super elements

                        hessian[3*i-2,3*i-2]       += +120*Epsilon*bx*bx/distijsqr;
                        hessian[3*i-1,3*i-1]       += +120*Epsilon*by*by/distijsqr;
                        hessian[3*i  ,3*i  ]       += +120*Epsilon*bz*bz/distijsqr;

                        // update the off-diagonals of diagonal super elements
                        hessian[3*i-2,3*i-1]       += +120*Epsilon*bx*by/distijsqr;
                        hessian[3*i-2,3*i  ]       += +120*Epsilon*bx*bz/distijsqr;
                        hessian[3*i-1,3*i-2]       += +120*Epsilon*by*bx/distijsqr;
                        hessian[3*i-1,3*i  ]       += +120*Epsilon*by*bz/distijsqr;
                        hessian[3*i  ,3*i-2]       += +120*Epsilon*bz*bx/distijsqr;
                        hessian[3*i  ,3*i-1]       += +120*Epsilon*bz*by/distijsqr;
                    }
                }
            }

            if(hessian.matrix is HessMatrix)
                return (hessian.matrix as HessMatrix);
            return HessMatrix.FromMatrix( hessian.matrix );
        }
    }
}
}
