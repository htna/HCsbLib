using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Universe.Atom;
    using Bond = Universe.Bond;
    using RotableInfo = Universe.RotableInfo;
public static partial class Paper
{
    public partial class TNM
    {
        public static MatrixByArr GetJ(Universe univ, Vector[] coords, List<RotableInfo> rotInfos, string option=null)
        {
            switch(option)
            {
                case "mine":
                    {
                        MatrixByArr[,] J = TNM_mine.GetJ(univ, coords, rotInfos, fnInv3x3: null);
                        double tolerance=0.00001;
                        HDebug.Assert(CheckEckartConditions(univ, coords, rotInfos, J, tolerance: tolerance));
                        //if(Debug.IsDebuggerAttachedWithProb(0.1))
                        //{
                        //    Matrix J0 = Matrix.FromBlockmatrix(J);
                        //    Matrix J1 = Matrix.FromBlockmatrix(GetJ(univ, coords, rotInfos, option:"paper", useNamedLock:false));
                        //    Debug.AssertTolerance(0.00000001, J0 - J1);
                        //}
                        return MatrixByArr.FromMatrixArray(J);
                    }
                case "paper":
                    {
                        MatrixByArr[,] J = TNM_paper.GetJ(univ, coords, rotInfos);
                        HDebug.Assert(CheckEckartConditions(univ, coords, rotInfos, J, 0.0000001));
                        return MatrixByArr.FromMatrixArray(J);
                    }
                case "mineopt":
                    {
                        MatrixByArr J = TNM_mineopt.GetJ(univ, coords, rotInfos);
                        if(HDebug.IsDebuggerAttached && univ.GetMolecules().Length == 1)
                        {
                            MatrixByArr J0 = TNM.GetJ(univ, coords, rotInfos, option:"paper");
                            MatrixByArr dJ = J-J0;
                            //double tolerance = dJ.ToArray().HAbs().HToArray1D().Mean();
                            //double maxAbsDH  = dJ.ToArray().HAbs().HMax();
                            //if(tolerance == 0)
                            //    tolerance = 0.000001;
                            HDebug.AssertTolerance(0.00000001, dJ);
                        }
                        return J;
                    }
                case null:
                    // my implementation has smaller error while checking Eckart conditions
                    goto case "mineopt";
            }
            return null;
        }
        public static Vector EckartConditionTrans(Universe univ, MatrixByArr[,] J, int a)
        {
            int n = J.GetLength(0);
            // check Eckart condition
            // http://en.wikipedia.org/wiki/Eckart_conditions
            //////////////////////////////////////////////////////////////////////
            // 1. translational Eckart condition
            //    sum{i=1..n}{mi * Jia} = 0
            Vector mJ = new double[3];
            for(int i=0; i<n; i++)
            {
                double mi = univ.atoms[i].Mass;
                Vector Jia = J[i, a].HToVector();
                mJ += mi * Jia;
            }
            return mJ;
        }
        public static Vector EckartConditionRotat(Universe univ, Vector[] coords, MatrixByArr[,] J, int a)
        {
            int n = J.GetLength(0);
            // check Eckart condition
            // http://en.wikipedia.org/wiki/Eckart_conditions
            //////////////////////////////////////////////////////////////////////
            // 2. rotational Eckart condition
            //    sum{i=1..n}{mi * r0i x Jia} = 0,
            //    where 'x' is the cross product
            Vector mrJ = new double[3];
            for(int i=0; i<n; i++)
            {
                double mi = univ.atoms[i].Mass;
                Vector ri = coords[univ.atoms[i].ID];
                Vector Jia = J[i, a].HToVector();
                mrJ += mi * LinAlg.CrossProd3(ri, Jia);
            }
            return mrJ;
        }
        public static bool CheckEckartConditions(Universe univ, Vector[] coords, List<RotableInfo> rotInfos, MatrixByArr[,] J, double tolerance=0.00000001)
        {
            int n = univ.atoms.Count;
            int m = rotInfos.Count;
            // 
            for(int a=0; a<m; a++)
            {
                // check Eckart condition
                // http://en.wikipedia.org/wiki/Eckart_conditions
                //////////////////////////////////////////////////////////////////////
                // 1. translational Eckart condition
                //    sum{i=1..n}{mi * Jia} = 0
                Vector mJ  = EckartConditionTrans(univ, J, a);
                //Debug.AssertTolerance(tolerance, mJ);
                if((mJ[0]>tolerance) || (mJ[1]>tolerance) || (mJ[2]>tolerance))
                    return false;
                // 2. rotational Eckart condition
                //    sum{i=1..n}{mi * r0i x Jia} = 0,
                //    where 'x' is the cross product
                Vector mrJ = EckartConditionRotat(univ, coords, J, a);
                //Debug.AssertTolerance(tolerance, mrJ);
                if((mrJ[0]>tolerance) || (mrJ[1]>tolerance) || (mrJ[2]>tolerance))
                    return false;
            }

            return true;
        }
    }
}
}
