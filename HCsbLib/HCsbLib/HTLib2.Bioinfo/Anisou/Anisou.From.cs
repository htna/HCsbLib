using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Anisou
    {
        public static Anisou FromValues(MatrixByArr U, Vector[] eigvecs, double[] eigvals)
        {
            Anisou anisou = new Anisou();
            anisou.U = U.CloneT();
            anisou.eigvecs = eigvecs.HClone<Vector>(); HDebug.Assert(eigvecs.Length == 3, eigvecs[0].Size == 3, eigvecs[1].Size == 3, eigvecs[2].Size == 3);
            anisou.eigvals = eigvals.HClone<double>(); HDebug.Assert(eigvals.Length == 3);
            return anisou;
        }
        public static Anisou FromValues(double[] values)
        {
            HDebug.Assert(values.Length == 21);
            MatrixByArr      U = new double[3,3]{ { values[0], values[1], values[2] },
                                             { values[3], values[4], values[5] },
                                             { values[6], values[7], values[8] },
                                           };
            Vector[] eigvecs = new Vector[3];
            eigvecs[0]       = new double[3] { values[ 9], values[10], values[11] };
            eigvecs[1]       = new double[3] { values[12], values[13], values[14] };
            eigvecs[2]       = new double[3] { values[15], values[16], values[17] };
            double[] eigvals = new double[3] { values[18], values[19], values[20] };
            return FromValues(U, eigvecs, eigvals);
        }
        public double[] ToValues()
        {
            double[] values = new double[21] {
                U[0,0], U[0,1], U[0,2],
                U[1,0], U[1,1], U[1,2],
                U[2,0], U[2,1], U[2,2],
                eigvecs[0][0], eigvecs[0][1], eigvecs[0][2],
                eigvecs[1][0], eigvecs[1][1], eigvecs[1][2],
                eigvecs[2][0], eigvecs[2][1], eigvecs[2][2],
                eigvals[0], eigvals[1], eigvals[2],
            };
            return values;
        }

        public static Anisou FromMatrix(MatrixByArr U, double eigvalthres=double.NegativeInfinity)
        {
            //Func<Matrix,Tuple<Vector[],double[]>> Eig = delegate(Matrix A)
            //{
            //    Vector[] eigvec;
            //    double[] eigval;
            //    HDebug.Verify(NumericSolver.Eig(A, out eigvec, out eigval));
            //    return new Tuple<Vector[], double[]>(eigvec, eigval);
            //};
            Func<MatrixByArr,Tuple<Vector[],double[]>> Eig = delegate(MatrixByArr A)
            {
                Tuple<MatrixByArr,Vector> VD = LinAlg.Eig(A);
                Vector[] eigvec = VD.Item1.GetColVectorList();
                double[] eigval = VD.Item2.ToArray();
                return new Tuple<Vector[], double[]>(eigvec, eigval);
            };
            return FromMatrix(U, Eig, eigvalthres);
        }
        public static Anisou FromMatrix(MatrixByArr U, Func<MatrixByArr,Tuple<Vector[],double[]>> Eig, double eigvalthres=double.NegativeInfinity)
        {
            Anisou anisou = new Anisou();

            anisou.U = U.CloneT();

            anisou.eigvecs = new Vector[3];
            anisou.eigvals = new double[3];
            {
                var eigvecval = Eig(anisou.U);
                Vector[] eigvec = eigvecval.Item1;
                double[] eigval = eigvecval.Item2;

                if(eigval[0] < eigvalthres) eigval[0] = 0;
                if(eigval[1] < eigvalthres) eigval[1] = 0;
                if(eigval[2] < eigvalthres) eigval[2] = 0;
                HDebug.Assert((eigval[0] == 0 && eigval[1] == 0  && eigval[2] == 0) == false);

                {   // normalize eigval and eigvec
                    double l;
                    l = eigvec[0].Dist; anisou.eigvecs[0] = eigvec[0] / l; anisou.eigvals[0] = eigval[0] * (l*l);
                    l = eigvec[1].Dist; anisou.eigvecs[1] = eigvec[1] / l; anisou.eigvals[1] = eigval[1] * (l*l);
                    l = eigvec[2].Dist; anisou.eigvecs[2] = eigvec[2] / l; anisou.eigvals[2] = eigval[2] * (l*l);
                }
            }
            return anisou;
        }
        public static Anisou[] FromMatrix(IList<MatrixByArr> Us)
        {
            Anisou[] anisous = new Anisou[Us.Count];
            for(int i=0; i<anisous.Length; i++)
                anisous[i] = FromMatrix(Us[i]);
            return anisous;
        }
        public static Anisou[] FromPdbAnisou(IList<Pdb.Anisou> anisous)
        {
            return FromMatrix(anisous.ListU());
        }
        public static Anisou[] FromBFactor(double[] bfactor, double scale=10000*1000)
        {
            Anisou[] anisou = new Anisou[bfactor.Length];
            for(int i=0; i<anisou.Length; i++)
            {
                double rad2 = bfactor[i];
                MatrixByArr U = new double[3, 3] { { rad2, 0, 0 }, { 0, rad2, 0 }, { 0, 0, rad2 } };
                anisou[i] = Anisou.FromMatrix(U*scale);
            }
            return anisou;
        }
        public static Anisou FromCoords(IList<Vector> coords, Vector meancoord=null, double eigvalthres=double.NegativeInfinity)
        {
            if(meancoord == null)
                meancoord = coords.Mean();

            MatrixByArr cov = new double[3, 3];
            {
                for(int i=0; i<coords.Count; i++)
                {
                    Vector vec = coords[i] - meancoord;
                    cov += LinAlg.VVt(vec, vec);
                }
            }
            Anisou anisou = Anisou.FromMatrix(cov, eigvalthres);
            return anisou;
        }
        public static Anisou[] FromCoords(List<Vector[]> ensemble, Vector[] meanconf=null, double eigvalthres=double.NegativeInfinity)
        {
            int size = ensemble[0].Length;
            Anisou[] anisous = new Anisou[size];

            HDebug.AssertNotNull(ensemble);
            System.Threading.Tasks.Parallel.For(0, size, delegate(int ai)
            //for(int ai=0; ai<size; ai++)
            {
                Vector mean = meanconf[ai];
                MatrixByArr cov = new double[3, 3];
                {
                    for(int ei=0; ei<ensemble.Count; ei++)
                    {
                        Vector vec = ensemble[ei][ai] - mean;
                        cov += LinAlg.VVt(vec, vec);
                    }
                }
                anisous[ai] = Anisou.FromMatrix(cov, eigvalthres:eigvalthres);
            }
            );

            return anisous;
        }
    }
}
