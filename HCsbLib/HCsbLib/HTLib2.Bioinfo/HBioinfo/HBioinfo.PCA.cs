using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        public static Mode[] PCA(IList<Vector[]> confs, Vector[] meanconf, Vector masses, Func<Matrix, Tuple<Matrix, Vector>> fnEig = null)
        {
            int size = meanconf.Length;
            int size3 = size * 3;
            Matrix cov = Matrix.Zeros(size3, size3);

            Vector vmeanconf = Vector.FromBlockvector(meanconf);
            foreach(Vector[] conf in confs)
            {
                Vector vconf = Vector.FromBlockvector(conf);
                Vector dvconf = vconf - vmeanconf;
                LinAlg.AddToM.VVt(cov, dvconf); // hess += dvconf * dvcond'
            }
            cov /= confs.Count;

            Vector invmasses = new double[masses.Size]; // inverse mass
            for(int i=0; i < masses.Size; i++) invmasses[i] = 1 / masses[i];

            Matrix mwcov = Hess.GetMassWeightedHess(cov, invmasses);        // just reuse Hess.GetMassWeightedHess(...)
            Mode[] mwmodes = PCA(mwcov, confs.Count, fnEig);
            Mode[] modes = mwmodes.GetMassReduced(masses.ToArray()).ToArray();  // just reuse ListMode.GetMassDeweighted(...)

            return modes;
        }
        public static Mode[] PCA(IList<Vector[]> confs, ref Vector[] meanconf, ILinAlg ila)
        {
            if(meanconf == null)
            {
                meanconf = new Vector[confs[0].Length];
                for(int i=0; i<meanconf.Length; i++)
                {
                    Vector meancoord = new double[3];
                    foreach(Vector[] conf in confs)
                        meancoord += conf[i];
                    meancoord /= confs.Count;
                    meanconf[i] = meancoord;
                }
            }

            int size = meanconf.Length;
            int size3 = size * 3;
            int num  = confs.Count;
            Matrix mconfs = Matrix.Zeros(size3, num);
            Vector vmeanconf = Vector.FromBlockvector(meanconf);
            for(int r=0; r<num; r++)
            {
                Vector vconf = Vector.FromBlockvector(confs[r]);
                Vector dvconf = vconf - vmeanconf;
                for(int c=0; c<size3; c++)
                    mconfs[c, r] = dvconf[c];
            }

            Matrix cov = null;
            {
                var CONFS = ila.ToILMat(mconfs);
                var COV = CONFS * CONFS.Tr;
                cov = COV.ToArray();
                CONFS.Dispose();
                COV.Dispose();
            }

            Func<Matrix, Tuple<Matrix, Vector>> fnEig = delegate(Matrix A)
            {
                var AA = ila.ToILMat(A);
                var VVDD = ila.EigSymm(AA);
                var VV = VVDD.Item1;
                Vector D = VVDD.Item2;
                Matrix V = VV.ToArray();
                AA.Dispose();
                VV.Dispose();
                return new Tuple<Matrix, Vector>(V, D);
            };
            return PCA(cov, confs.Count, fnEig);
        }
        public static Mode[] PCA(IList<Vector[]> confs, Vector[] meanconf, Func<Matrix, Tuple<Matrix, Vector>> fnEig = null)
        {
            int size = meanconf.Length;
            int size3 = size * 3;
            Matrix cov = Matrix.Zeros(size3, size3);

            Vector vmeanconf = Vector.FromBlockvector(meanconf);
            foreach(Vector[] conf in confs)
            {
                Vector vconf = Vector.FromBlockvector(conf);
                Vector dvconf = vconf - vmeanconf;
                LinAlg.AddToM.VVt(cov, dvconf); // hess += dvconf * dvcond'
            }
            cov /= confs.Count;

            return PCA(cov, confs.Count, fnEig);
        }
        public static Mode[] PCA(Matrix cov, int numconfs, Func<Matrix, Tuple<Matrix, Vector>> fnEig = null)
        {
            HDebug.Assert(cov.RowSize == cov.ColSize);
            int size3 = cov.ColSize;

            if(fnEig == null)
                fnEig = delegate(Matrix A)
                {
                    using(new Matlab.NamedLock("TEST"))
                    {
                        Matlab.Clear("TEST");
                        Matlab.PutMatrix("TEST.H", A);
                        Matlab.Execute("TEST.H = (TEST.H + TEST.H')/2;");
                        Matlab.Execute("[TEST.V, TEST.D] = eig(TEST.H);");
                        Matlab.Execute("TEST.D = diag(TEST.D);");
                        //Matlab.Execute("TEST.idx = find(TEST.D>0.00000001);");
                        //Matrix leigvecs = Matlab.GetMatrix("TEST.V(:,TEST.idx)");
                        //Vector leigvals = Matlab.GetVector("TEST.D(TEST.idx)");
                        Matrix leigvecs = Matlab.GetMatrix("TEST.V");
                        Vector leigvals = Matlab.GetVector("TEST.D");
                        Matlab.Clear("TEST");
                        return new Tuple<Matrix, Vector>(leigvecs, leigvals);
                    }
                };

            Tuple<Matrix,Vector> eigs = fnEig(cov);
            Matrix eigvecs = eigs.Item1;
            Vector eigvals = eigs.Item2;
            HDebug.Assert(eigvecs.ColSize == size3, eigvals.Size == eigvecs.RowSize);

            Mode[] modes = new Mode[eigvals.Size];
            for(int im=0; im < modes.Length; im++)
                modes[im] = new Mode
                {
                    eigvec = eigvecs.GetColVector(im),
                    eigval = 1.0 / eigvals[im],
                    th = im
                };

            int maxNumEigval = Math.Min(numconfs - 1, size3);
            HDebug.Assert(maxNumEigval >= 0);
            HDebug.Assert(eigvals.Size == size3);
            if(maxNumEigval < size3)
            {
                modes = modes.SortByEigvalAbs().ToArray();
                modes = modes.Take(maxNumEigval).ToArray();
                foreach(var mode in modes)
                    HDebug.Assert(mode.eigval >= 0);
                //Tuple<Mode[], Mode[]> nzmodes_zeromodes = modes.SeparateTolerants();
                //Mode[] modesNonzero = nzmodes_zeromodes.Item1;
                //Mode[] modesZero    = nzmodes_zeromodes.Item2;
                //modes = modesZero;
            }
            HDebug.Assert(modes.Length == maxNumEigval);

            return modes;
        }
        public static Mode[] PCA(IList<Vector[]> confs, out Vector[] meanconf)
        {
            //HDebug.ToDo();

            int size = confs[0].Length;
            meanconf = new Vector[size];
            for(int i=0; i<size; i++)
            {
                Vector meancoord = new double[3];
                foreach(Vector[] conf in confs)
                    meancoord += conf[i];
                meancoord /= confs.Count;
                meanconf[i] = meancoord;
            }
            return PCA(confs, meanconf);
        }
        public static void PCA(string pdbpath, out Pdb.Atom[] meanconf, out Matrix modes, out Vector freq)
        {
            HDebug.ToDo();

            meanconf = null;
            modes = null;
            freq = null;
            //Pdb[] pdbs;
            //Pdb.FromFile(pdbpath, out pdbs);
            //
            //List<Vector>[] confs = new List<Vector>[pdbs.Length];
            //for(int i=0; i<confs.Length; i++)
            //    confs[i] = pdbs[i].atoms.ListCoord();
            //
            //List<Vector> lmeanconf;
            //Do(confs, out lmeanconf, out modes, out freq);
            //meanconf = pdbs[0].atoms.UpdateCoord(lmeanconf).ToArray();
        }
        public static void PCA(string pdbpath, string outpathroot, double scaleAnimation)
        {
            HDebug.ToDo();

            Pdb.Atom[] meanconf;
            Matrix modes;
            Vector freq;
            PCA(pdbpath, out meanconf, out modes, out freq);

            for(int i=0; i<freq.Size; i++)
            {
                string outpath = outpathroot + i.ToString("00") + ".pdb";
                Vector[] mode = (modes.GetColVector(i) * scaleAnimation / freq[i]).ToVectors(3);
                Pdb.ToFileAnimated(outpath, meanconf, mode);
            }
        }
    }
}
