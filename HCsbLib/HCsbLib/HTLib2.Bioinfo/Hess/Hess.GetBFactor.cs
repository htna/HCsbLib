using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        static bool GetBFactorSelfTest_DoTest = true;
        public static bool GetBFactorSelfTest()
        {
            if(GetBFactorSelfTest_DoTest == false)
                return true;
            GetBFactorSelfTest_DoTest = false;

            string pathroot = @"K:\PdbUnzippeds\";

            if(GetBFactorSelfTest(pathroot+"1MJC.pdb", 13, 0.67, 2, null) == false) return false;
            return true;
        }
        public static bool GetBFactorSelfTest(string pdbpath, double cutoff, double corr, int round, InfoPack extra)
        {
            if(HFile.Exists(pdbpath) == false)
                return false;

            Pdb pdb = Pdb.FromFile(pdbpath);
            for(int i=0; i<pdb.atoms.Length; i++)
            {
                HDebug.Assert(pdb.atoms[0].altLoc  == pdb.atoms[i].altLoc );
                HDebug.Assert(pdb.atoms[0].chainID == pdb.atoms[i].chainID);
            }
            List<Pdb.Atom> atoms = new List<Pdb.Atom>();
            for(int i=0; i<pdb.atoms.Length; i++)
            {
                Pdb.Atom atom = pdb.atoms[i];
                if(atom.name.Trim().ToUpper() == "CA")
                    atoms.Add(atom);
            }
            List<Vector> coords = atoms.ListCoord();
            Matrix hess = Hess.GetHessAnm(coords, cutoff).ToMatrix();
            InfoPack lextra = new InfoPack();
            Vector bfactor = GetBFactor(hess, 0.00000001, null, lextra);
            if(extra != null)
            {
                extra["num_zero_eigvals"] = lextra["num_zero_eigvals"];
                extra["eigenvalues"] = lextra["eigenvalues"];
            }

            double corr_comp = pdb.CorrBFactor(atoms.ListName(), atoms.ListResSeq(), bfactor.ToArray());
            corr_comp = Math.Round(corr_comp, round);
            HDebug.Assert(corr == corr_comp);
            return (corr == corr_comp);
        }
        public static Vector GetBFactor(Matrix hess, double? thresEigval, int? numZeroEigval, InfoPack extra=null)
        {
            HDebug.Assert(GetBFactorSelfTest());

            int n = hess.ColSize/3;
            HDebug.Assert(n*3 == hess.ColSize);

            HDebug.Assert(hess.RowSize == hess.ColSize);
            InfoPack lextra = new InfoPack();
            Matrix invhess = NumericSolver.InvEig(hess, thresEigval, numZeroEigval, lextra);
            //Matrix invhess = NumericSolver.InvEig(hess, 0.00000001, null, lextra);

            Vector bfactor = new double[n];
            for(int i=0; i<n; i++)
                bfactor[i] = invhess[i*3+0, i*3+0] + invhess[i*3+1, i*3+1] + invhess[i*3+2, i*3+2];

            if(extra != null)
            {
                extra["num_zero_eigvals"] = lextra["num_zero_eigvals"];
                extra["eigenvalues"     ] = lextra["eigenvalues"     ];
            }

            return bfactor;
        }
        public static Vector GetBFactor(Matrix hess
                                               , int  numIgnoreSmallEigval=6 // the number to ignore the eigenvalues
                                               , bool zeroForNegEigval=true  // set zero for the negative eigen values
                                               )
        {
            HDebug.Assert(GetBFactorSelfTest());

            int n = hess.ColSize/3;
            HDebug.Assert(n*3 == hess.ColSize);

            HDebug.Assert(hess.RowSize == hess.ColSize);
            InfoPack lextra = new InfoPack();
            Vector[] eigvec;
            double[] eigval;
            NumericSolver.Eig(hess.ToArray(), out eigvec, out eigval);

            double[] abs_eigval = eigval.HAbs();
            List<int> idx_sorted_abs_eigval = new List<int>(abs_eigval.HIdxSorted());
            for(int i=0; i<numIgnoreSmallEigval; i++)
                idx_sorted_abs_eigval.RemoveAt(0);

            Vector bfactor = new double[n];
            foreach(int idx in idx_sorted_abs_eigval)
            {
                if(eigval[idx] < 0)
                    continue;
                for(int i=0; i<n; i++)
                {
                    double dx = eigvec[idx][i*3+0];
                    double dy = eigvec[idx][i*3+1];
                    double dz = eigvec[idx][i*3+2];
                    bfactor[i] += (dx*dx + dy*dy + dz*dz)/eigval[idx];
                }
            }

            return bfactor;
        }
        public static Vector GetBFactor(Matrix hess, Vector mass
                                               , int numIgnoreSmallEigval=6 // the number to ignore the eigenvalues
                                               , bool zeroForNegEigval=true  // set zero for the negative eigen values
                                               )
        {
            HDebug.Assert(GetBFactorSelfTest());
            HDebug.Assert(hess.ColSize == hess.RowSize);
            HDebug.Assert(hess.ColSize % 3 == 0);
            HDebug.Assert(hess.ColSize / 3 == mass.Size);

            Matrix mhess = GetMassWeightedHess(hess, mass);

            // ei  : mass weighted eigenvector
            // wi2 : mass weighted eigenvalue
            // Ui' : ei*ei/wi2 = diag(eigenvec * 1/eigenval * eigenvec')
            Vector bfactor = GetBFactor(mhess
                                                , numIgnoreSmallEigval : numIgnoreSmallEigval
                                                , zeroForNegEigval : zeroForNegEigval
                                                );

            // vi : mass free eigenvector
            // Ui := (Kb T) * (vi * vi / wi2)
            //     = (Kb T / mi) * ((vi*mi^-2) * (vi*mi^-2) / wi2)
            //     = (Kb T / mi) * (ei * ei / wi2)
            //     = (Kb T / mi) * Ui'
            int n = mass.Size;
            for(int i=0; i<n; i++)
                // assume Kb, T are constant (=1)
                bfactor[i] = bfactor[i] / mass[i];

            return bfactor;
        }
        public static Vector GetBFactor(Matrix hess, Vector mass, double? thresEigval, int? numZeroEigval, InfoPack extra=null)
        {
            HDebug.Assert(GetBFactorSelfTest());
            HDebug.Assert(hess.ColSize == hess.RowSize);
            HDebug.Assert(hess.ColSize % 3 == 0);
            HDebug.Assert(hess.ColSize / 3 == mass.Size);

            Matrix mhess = GetMassWeightedHess(hess, mass);

            // ei  : mass weighted eigenvector
            // wi2 : mass weighted eigenvalue
            // Ui' : ei*ei/wi2 = diag(eigenvec * 1/eigenval * eigenvec')
            Vector bfactor = GetBFactor(mhess, thresEigval, numZeroEigval, extra);

            // vi : mass free eigenvector
            // Ui := (Kb T) * (vi * vi / wi2)
            //     = (Kb T / mi) * ((vi*mi^-2) * (vi*mi^-2) / wi2)
            //     = (Kb T / mi) * (ei * ei / wi2)
            //     = (Kb T / mi) * Ui'
            int n = mass.Size;
            for(int i=0; i<n; i++)
                // assume Kb, T are constant (=1)
                bfactor[i] = bfactor[i] / mass[i];

            return bfactor;
        }
        //static bool GetBFactor_selftest = true;
        //public static Vector GetBFactor(MatrixByArr[,] hess, Vector mass, double? thresEigval, int? numZeroEigval, InfoPack extra=null)
        //{
        //    HDebug.Assert(GetBFactorSelfTest());
        //
        //    HDebug.Assert(hess.GetLength(0) == hess.GetLength(1));
        //    int n = hess.GetLength(0);
        //
        //    if(GetBFactor_selftest)
        //    {
        //        GetBFactor_selftest = false;
        //        // mass weighted hessian
        //        // MH = M^(-1/2) * H * M^(-1/2)
        //        // MH_ij = H_IJ * sqrt(M[i] * M[j])
        //        {
        //            // mass weighted block hessian
        //            MatrixByArr[,] mbhess = new MatrixByArr[n, n];
        //            for(int i=0; i<n; i++)
        //            {
        //                //mbhess[i, i] = new double[3, 3];
        //                for(int j=0; j<n; j++)
        //                {
        //                    //if(i == j) continue;
        //                    HDebug.Assert(hess[i, j].ColSize == 3, hess[i, j].RowSize == 3);
        //                    mbhess[i, j] = hess[i, j] / Math.Sqrt(mass[i] * mass[j]);
        //                    //mbhess[i, i] -= mbhess[i, j];
        //                }
        //            }
        //            MatrixByArr  _mhess = MatrixByArr.FromMatrixArray(mbhess);
        //            MatrixByArr __mhess = MatrixByArr.FromMatrixArray(GetMassWeightedHess(hess, mass));
        //            HDebug.AssertTolerance(0.00000001, _mhess - __mhess);
        //        }
        //    }
        //
        //    MatrixByArr mhess = MatrixByArr.FromMatrixArray(GetMassWeightedHess(hess, mass));
        //
        //    // ei  : mass weighted eigenvector
        //    // wi2 : mass weighted eigenvalue
        //    // Ui' : ei*ei/wi2 = diag(eigenvec * 1/eigenval * eigenvec')
        //    Vector bfactor = GetBFactor(mhess, thresEigval, numZeroEigval, extra);
        //    
        //    // vi : mass free eigenvector
        //    // Ui := (Kb T) * (vi * vi / wi2)
        //    //     = (Kb T / mi) * ((vi*mi^-2) * (vi*mi^-2) / wi2)
        //    //     = (Kb T / mi) * (ei * ei / wi2)
        //    //     = (Kb T / mi) * Ui'
        //    for(int i=0; i<n; i++)
        //        // assume Kb, T are constant (=1)
        //        bfactor[i] = bfactor[i] / mass[i];
        //
        //    return bfactor;
        //}
    }
}
