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
        public static HessMatrix ThirdTerm(IList<Vector> caArray, double K_phi1, double K_phi3, HessMatrix hessian
                                      , bool useArnaud96=false)
        {
            if(hessian == null)
                hessian = new double[caArray.Count*3, caArray.Count*3];

            for(int i=0; i<caArray.Count-3; i++)
                ThirdTerm(caArray, K_phi1, K_phi3, hessian, i, i+1, i+2, i+3, useArnaud96:useArnaud96);

            return hessian;
        }
        private static void ThirdTerm(IList<Vector> caArray, double K_phi1, double K_phi3, HessMatrix hessian, int i, int j, int k, int l
                                     , bool useArnaud96=false)
        {
            int[] idx = new int[] { i, j, k, l };
            Vector[] lcaArray = new Vector[] { caArray[idx[0]], caArray[idx[1]], caArray[idx[2]], caArray[idx[3]] };
            Matrix lhess = ThirdTerm(lcaArray, K_phi1, K_phi3, useArnaud96: useArnaud96);
            for(int c=0; c<4; c++) for(int dc=0; dc<3; dc++)
            for(int r=0; r<4; r++) for(int dr=0; dr<3; dr++)
                hessian[idx[c]*3+dc, idx[r]*3+dr] += lhess[c*3+dc, r*3+dr];
        }
        private static void ThirdTermN(IList<Vector> caArray, double K_phin, double n, HessMatrix hessian, int i, int j, int k, int l
                                      , bool useArnaud96=false)
        {
            int[] idx = new int[] { i, j, k, l };
            Vector[] lcaArray = new Vector[] { caArray[idx[0]], caArray[idx[1]], caArray[idx[2]], caArray[idx[3]] };
            Matrix lhess = ThirdTermN(lcaArray, K_phin, n, useArnaud96: useArnaud96);
            double maxabs_lhess = lhess.ToArray().HAbs().HMax();
            HDebug.Assert(maxabs_lhess < 10000);

            for(int c=0; c<4; c++) for(int dc=0; dc<3; dc++)
            for(int r=0; r<4; r++) for(int dr=0; dr<3; dr++)
                hessian[idx[c]*3+dc, idx[r]*3+dr] += lhess[c*3+dc, r*3+dr];
        }
        //private static void ThirdTermN(IList<Vector> caArray, double K_phin, double n, MatrixSparse<MatrixByArr> hessian, int i, int j, int k, int l
        //                              , bool useArnaud96=true)
        //{
        //    int[] idx = new int[] { i, j, k, l };
        //    Vector[] lcaArray = new Vector[] { caArray[idx[0]], caArray[idx[1]], caArray[idx[2]], caArray[idx[3]] };
        //    MatrixByArr lhess = ThirdTermN(lcaArray, K_phin, n, useArnaud96: useArnaud96);
        //    double maxabs_lhess = lhess.ToArray().HAbs().HMax();
        //    HDebug.Assert(maxabs_lhess < 10000);
        //
        //    for(int c=0; c<4; c++)
        //    for(int r=0; r<4; r++)
        //    {
        //        MatrixByArr lhess_cr = lhess.SubMatrixByFromCount(c*3, 3, r*3, 3);
        //        hessian[idx[c], idx[r]] += lhess_cr;
        //    }
        //}
        /// <summary>
        /// Dihedral Term
        /// V3 = Kchi(1 + cos(n*chi - delta))
        ///    = Kchi(1 - cos(n*chi - delta + pi))
        ///    ~ | Kchi*(  1/2) (chi - delta + pi)^2       if n == 1
        ///      | Kchi*(  4/2) (chi - delta + pi)^2       if n == 2
        ///      | Kchi*(  9/2) (chi - delta + pi)^2       if n == 3
        ///      | Kchi*( 16/2) (chi - delta + pi)^2       if n == 4
        ///      | Kchi*( 25/2) (chi - delta + pi)^2       if n == 5
        ///      | Kchi*( 36/2) (chi - delta + pi)^2       if n == 6
        ///      | Kchi*(n^2/2) (chi - delta + pi)^2       otherwise
        ///    = Kchin(Kchi,n) * (chi - delta + pi)^2
        ///      where  Kchin(Kchi,n) = Kchi * n^2 / 2
        ///      
        /// </summary>
        /// <param name="caArray_"></param>
        /// <param name="K_phi_n"></param>
        /// <returns></returns>
        public static Matrix ThirdTermN(IList<Vector> caArray_, double K_phi, double n
                                       , bool useArnaud96=false)
        {
            double K_phin = K_phi * 0.5 * (n*n);
            Matrix hess = ThirdTerm(caArray_, K_phin, useArnaud96:useArnaud96);
            return hess;
        }
        /// <summary>
        /// Dihedral Term
        /// V3 = K_phi1 ( 1 - cos(phi - phi0) )
        ///    + K_phi3 ( 1 - cos 3(phi - phi0) )
        /// </summary>
        /// <param name="caArray_"></param>
        /// <param name="K_phi1"></param>
        /// <param name="K_phi3"></param>
        /// <returns></returns>
        public static Matrix ThirdTerm(IList<Vector> caArray_, double K_phi1, double K_phi3
                                      , bool useArnaud96=false)
        {
            double K_phi=K_phi1/2+K_phi3*9/2;
            return ThirdTerm(caArray_, K_phi, useArnaud96: useArnaud96);
        }
        /// <summary>
        /// Dihedral Term
        /// V3 = K_phi ( phi - phi0 )^2
        /// </summary>
        /// <param name="caArray_"></param>
        /// <param name="K_phi1"></param>
        /// <param name="K_phi3"></param>
        /// <returns></returns>
        static bool ThirdTerm_selftest = HDebug.IsDebuggerAttached;
        public static Matrix ThirdTerm(IList<Vector> caArray_, double K_phi
                                      , bool useArnaud96=false
                                      , bool checkComputable=true)
        {
            //HDebug.Assert(useArnaud96 == true); // convert to use Arnaud96
            if(useArnaud96)
            {
                HessMatrix hess3 = Paper.Arnaud96.HessSpring(caArray_, 2*K_phi).ToArray();
                //if(HDebug.IsDebuggerAttached)
                if(ThirdTerm_selftest)
                {
                    ThirdTerm_selftest = false;
                    Matrix hess3_ = ThirdTerm(caArray_, K_phi, useArnaud96:false, checkComputable:false);
                    Matrix dhess3 = hess3 - hess3_;
                    if(hess3_.IsComputable())
                    {
                        Vector v0 = caArray_[0];
                        Vector v1 = caArray_[1];
                        Vector v2 = caArray_[2];
                        Vector v3 = caArray_[3];
                        Vector A012 = LinAlg.CrossProd(v0-v1, v1-v2).UnitVector();
                        Vector B123 = LinAlg.CrossProd(v1-v2, v2-v3).UnitVector();
                        double AB = LinAlg.VtV(A012, B123);
                        double acosAB = Math.Acos(AB);
                
                        double hess3max = hess3_.ToArray().HAbs().HMax();
                        if(Math.Abs(AB)<0.999999)
                            HDebug.AssertToleranceMatrix(hess3max*0.0001, dhess3);
                    }
                }
                return hess3;
            }

            VECTORS caArray = new VECTORS(caArray_);
            MATRIX hessian = new MATRIX(new double[12,12]);

            {
                int i=1;
                int j=2;
                int k=3;
                int l=4;

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
            if(checkComputable)
                HDebug.Assert(hessian.matrix.IsComputable());
            return hessian.matrix;
        }
    }
}
}
