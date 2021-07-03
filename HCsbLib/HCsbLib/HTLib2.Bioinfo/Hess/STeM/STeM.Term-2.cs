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
        public static HessMatrix SecondTerm(IList<Vector> caArray, double K_theta, HessMatrix hessian)
        {
            if(hessian == null)
                hessian = HessMatrix.FromMatrix(new double[caArray.Count*3, caArray.Count*3]);

            for(int i=0; i<caArray.Count-2; i++)
                SecondTerm(caArray, K_theta, hessian, i, i+1, i+2);

            return hessian;
        }
        public static void SecondTerm(IList<Vector> caArray, double K_theta, HessMatrix hessian, int i, int j, int k)
        {
            int[] idx = new int[] { i, j, k };
            Vector[] lcaArray = new Vector[] { caArray[idx[0]], caArray[idx[1]], caArray[idx[2]] };
            Matrix lhess = SecondTerm(lcaArray, K_theta);
            double maxabs_lhess = lhess.ToArray().HAbs().HMax();
            HDebug.Assert(maxabs_lhess < 10000);

            for(int c=0; c<3; c++) for(int dc=0; dc<3; dc++)
            for(int r=0; r<3; r++) for(int dr=0; dr<3; dr++)
                hessian[idx[c]*3+dc, idx[r]*3+dr] += lhess[c*3+dc, r*3+dr];
        }
        //private static void SecondTerm(IList<Vector> caArray, double K_theta, MatrixSparse<MatrixByArr> hessian, int i, int j, int k)
        //{
        //    int[] idx = new int[] { i, j, k };
        //    Vector[] lcaArray = new Vector[] { caArray[idx[0]], caArray[idx[1]], caArray[idx[2]] };
        //    MatrixByArr lhess = SecondTerm(lcaArray, K_theta);
        //    double maxabs_lhess = lhess.ToArray().HAbs().HMax();
        //    HDebug.Assert(maxabs_lhess < 10000);
        //
        //    for(int c=0; c<3; c++)
        //    for(int r=0; r<3; r++)
        //    {
        //        MatrixByArr lhess_cr = lhess.SubMatrixByFromCount(c*3, 3, r*3, 3);
        //        hessian[idx[c], idx[r]] += lhess_cr;
        //    }
        //}
        /// <summary>
        /// Angular Term
        /// V2 = K_theta (theta - theta_0)^2
        /// </summary>
        /// <param name="caArray_"></param>
        /// <param name="K_theta"></param>
        /// <returns></returns>
        public static Matrix SecondTerm(IList<Vector> caArray_, double K_theta)
        {
            VECTORS caArray = new VECTORS(caArray_);
            MATRIX hessian = new MATRIX(new double[9,9]);

            // derive the hessian of the second term
            {
                int i=1;
                int j=2;
                int k=3;

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
                double lpl=p.Dist;
                Vector q=caArray.GetRow(k)-caArray.GetRow(j);
                double lql=q.Dist;
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
            HDebug.Assert(hessian.matrix.IsComputable());
            return hessian.matrix;
        }
    }
}
}
