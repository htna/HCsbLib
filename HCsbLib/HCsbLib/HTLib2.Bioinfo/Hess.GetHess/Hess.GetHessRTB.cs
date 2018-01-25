﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        /// http://onlinelibrary.wiley.com/doi/10.1002/1097-0134(20001001)41:1%3C1::AID-PROT10%3E3.0.CO;2-P/abstract
        /// http://onlinelibrary.wiley.com/doi/10.1002/1097-0134(20001001)41:1%3C1::AID-PROT10%3E3.0.CO;2-P/pdf
        /// http://www.ncbi.nlm.nih.gov/pubmed/10944387?dopt=Abstract
        /// Proteins. 2000 Oct 1;41(1):1-7.
        /// Building-block approach for determining low-frequency normal modes of macromolecules.
        /// Tama F, Gadea FX, Marques O, Sanejouand YH.
        /// 
        ///     In the RTB approach, H is first expressed in a basis defined by the rotations and translations
        ///     of nb blocks, Hb, the projected Hessian, being given by:
        ///         Hb = P' H P
        ///     where P is an orthogonal 3N 3 6nb matrix built with the vectors associated with the local rotations
        ///     and translations of each block. Approximate low-frequency normal modes of the system, thus, are
        ///     obtained by diagonalizing Hb, a matrix of size $6 nb x 6 nb$, the corresponding(3N) atomic
        ///     displacements being obtained as:
        ///         Ap = P Ab
        ///     where Ab is the matrix of the eigenvectors of Hb.
        ///     ...
        ///     The atomic fluctuations are obtained from a normal mode analysis as:
        ///         〈x_i^2〉 = (kb T / mi) \sum(j=1..nv) aij^2 / wj^2 (2)
        ///     where xi is the atomic coordinate i, mi, its mass, nv, the number of modes considered,
        ///     vj = wj /2pi the frequency of normal mode j, and aij the corresponding coordinate displacement.
        /// 
        ///     1a. determine P as translational and rotational (from center of mass of each block)
        ///     1b. adjust P to make each block P as orthonormal (use SVD)
        ///     2.  Hb = P' H P
        ///     3.  [V,D] = eig(Hb)
        ///     4a. modei = vi / sqrt(mi)               ← from (2)
        ///     4b. eigi  = di
        /// 
        /// http://www.ncbi.nlm.nih.gov/pubmed/12414680?dopt=Abstract
        /// http://www.ncbi.nlm.nih.gov/pmc/articles/PMC1302332/pdf/12414680.pdf
        /// Biophys J. 2002 Nov;83(5):2457-74.
        /// A coarse-grained normal mode approach for macromolecules: an efficient implementation and application to Ca(2+)-ATPase.
        /// Li G, Cui Q.
        public class HessRTB
        {
            public HessMatrix   hess;
            public Vector[]     coords;
            public double[]     masses;
            public IList<int[]> blocks;
            public Matrix       P;
            public Matrix       PHP;
            public Matrix       PMP;

            Mode[] _allmodes = null;
            public Mode[] GetAllModes()
            {
                if(_allmodes == null)
                {
                    Mode[] rtbmodes = GetRtbModes();

                    Matrix M = rtbmodes.ListEigvecAsMatrix();
                    if(HDebug.IsDebuggerAttached)
                        #region check if each colume of M is same to its corresponding eigvector
                {
                    for(int r=0; r<M.RowSize; r++)
                    {
                        Vector colvec_r = M.GetColVector(r);
                        double err = (colvec_r - rtbmodes[r].eigvec).ToArray().HAbs().Max();
                        HDebug.Assert(err == 0);
                    }
                }
                    #endregion

                    Matrix PM = null;
                    using(new Matlab.NamedLock(""))
                    {
                        Matlab.PutMatrix("P", P);
                        Matlab.PutMatrix("M", M);
                        PM = Matlab.GetMatrix("P*M", true);
                    }

                    _allmodes = new Mode[rtbmodes.Length];
                    for(int i=0; i<rtbmodes.Length; i++)
                    {
                        _allmodes[i] = new Mode
                        {
                            th     = rtbmodes[i].th,
                            eigval = rtbmodes[i].eigval,
                            eigvec = PM.GetColVector(i),
                        };
                        // mass-weighted, mass-reduced is already handled in GetRtbModes() using "PMP" and "PMPih".
                        // HDebug.ToDo("check if each mode should be devided by masses (or not)");
                        // //for(int j=0; j<_allmodes[i].eigvec.Size; j++)
                        // //    _allmodes[i].eigvec[j] = _allmodes[i].eigvec[j] / masses[j/3];
                    }
                }
                return _allmodes;
            }

            Mode[] _rtbmodes = null;
            public Mode[] GetRtbModes()
            {
                if(_rtbmodes == null)
                {
                    Matrix   eigvec;
                    double[] eigval;
                    using(new Matlab.NamedLock(""))
                    {   // solve [eigvec, eigval] = eig(PHP, PMP)

                        {   // PMPih = 1/sqrt(PMP)        where "ih" stands for "Inverse Half" -1/2
                            Matlab.PutMatrix("PMP", PMP);
                            Matlab.Execute("PMP = (PMP + PMP')/2;");
                            Matlab.Execute("[PMPih.V, PMPih.D] = eig(PMP); PMPih.D = diag(PMPih.D);");
                            Matlab.Execute("PMPih.Dih = 1.0 ./ sqrt(PMPih.D);");    // PMPih.Dih = PMPih.D ^ -1/2
                            if(HDebug.IsDebuggerAttached)
                            {
                                double err = Matlab.GetValue("max(abs(1 - PMPih.D .* PMPih.Dih .* PMPih.Dih))");
                                HDebug.AssertTolerance(0.00000001, err);
                            }
                            Matlab.Execute("PMPih = PMPih.V * diag(PMPih.Dih) * PMPih.V';");
                            if(HDebug.IsDebuggerAttached)
                            {
                                double err = Matlab.GetValue("max(max(abs(eye(size(PMP)) - (PMP * PMPih * PMPih))))");
                                HDebug.AssertTolerance(0.00000001, err);
                            }
                            Matlab.Execute("clear PMP;");
                        }

                        {   // to solve [eigvec, eigval] = eig(PHP, PMP)
                            // 1. H = PMP^-1/2 * PHP * PMP^-1/2
                            // 2. [V,D] = eig(H)
                            // 3. V = PMP^-1/2 * V
                            Matlab.PutMatrix("PHP", PHP);                                                       // put RTB Hess
                            Matlab.Execute("PHP = (PHP + PHP')/2;");
                            Matlab.Execute("PHP = PMPih * PHP * PMPih; PHP = (PHP + PHP')/2;");                 // mass-weighted Hess
                            Matlab.Execute("[V,D] = eig(PHP); D=diag(D);                        clear PHP;");   // mass-weighted modes, eigenvalues
                            Matlab.Execute("V = PMPih * V;                                      clear PMPih;"); // mass-reduced modes
                        }

                        eigvec = Matlab.GetMatrix("V");
                        eigval = Matlab.GetVector("D");
                        Matlab.Execute("clear;");
                    }

                    List<Mode> modes;
                    {   // sort by eigenvalues
                        int[] idx = eigval.HIdxSorted();
                        modes = new List<Mode>(idx.Length);
                        for(int i=0; i<eigval.Length; i++)
                        {
                            Mode mode = new Mode
                            {
                                th     = i+1,
                                eigval = eigval[idx[i]],
                                eigvec = eigvec.GetColVector(idx[i]),
                            };
                            modes.Add(mode);
                        }
                    }

                    _rtbmodes = modes.ToArray();
                }
                return _rtbmodes;
            }
        }
        public static HessRTB GetHessRTB(HessMatrix hess, Vector[] coords, double[] masses, IList<int[]> blocks)
        {
            return BuilderHessRTB.GetHessRTB(hess, coords, masses, blocks);
        }
        public static HessRTB GetHessRTBByBlockAsResidue(HessMatrix hess, Vector[] coords, double[] masses, Universe.Atom[] atoms)
        {
            int leng = coords.Length;
            HDebug.Exception
                ( leng == hess.ColBlockSize
                , leng == hess.RowBlockSize
                , leng == coords.Length
                , leng == masses.Length
                , leng == atoms.Length
                , "length does not match"
                );

            List<int[]> blocks = new List<int[]>();
            foreach(var group in atoms.GroupByResidue())
            {
                List<int> block = new List<int>();
                foreach(var atom in group) block.Add(atom.ID);
                blocks.Add(block.ToArray());
            }

            return BuilderHessRTB.GetHessRTB(hess, coords, masses, blocks);
        }
        public static class BuilderHessRTB
        {
            public static HessRTB GetHessRTB(HessMatrix hess, Vector[] coords, double[] masses, IList<int[]> blocks)
            {
                #region check pre-condition
                {
                    HDebug.Assert(coords.Length == hess.ColBlockSize);                      // check hess matrix
                    HDebug.Assert(coords.Length == hess.RowBlockSize);                      // check hess matrix
                    HDebug.Assert(coords.Length == blocks.HMerge().HToHashSet().Count);     // check hess contains all blocks
                    HDebug.Assert(coords.Length == blocks.HMerge().Count             );     // no duplicated index in blocks
                }
                #endregion

                List<Vector> Ps = new List<Vector>();
                foreach(int[] block in blocks)
                {
                    List<Vector> PBlk = new List<Vector>();
                    PBlk.AddRange(GetTrans (coords, masses, block));
                    PBlk.AddRange(GetRotate(coords, masses, block));
                    {
                        // PBlk = ToOrthonormal   (coords, masses, block, PBlk.ToArray()).ToList();
                        ///
                        ///     Effect of making orthonormal is not significant as below table, but consumes time by calling SVD
                        ///     Therefore, skip making orthonormal
                        ///     =========================================================================================================================================================
                        ///     model   | making orthonormal?|             | MSF corr   , check sparsity              , overlap weighted by eigval : overlap of mode 1-1, 2-2, 3-3, ...
                        ///     =========================================================================================================================================================
                        ///     NMA     | orthonormal by SVD | RTB         | corr 0.9234, spcty(all    NaN, ca    NaN), wovlp 0.5911 : 0.82,0.79,0.74,0.69,0.66,0.63,0.60,0.59,0.56,0.54)
                        ///             | orthogonal         | RTB         | corr 0.9230, spcty(all    NaN, ca    NaN), wovlp 0.5973 : 0.83,0.80,0.75,0.70,0.67,0.64,0.60,0.59,0.58,0.55)
                        ///     ---------------------------------------------------------------------------------------------------------------------------------------------------------
                        ///     scrnNMA | orthonormal by SVD | RTB         | corr 0.9245, spcty(all    NaN, ca    NaN), wovlp 0.5794 : 0.83,0.78,0.73,0.68,0.65,0.62,0.60,0.58,0.55,0.55)
                        ///             | orthogonal         | RTB         | corr 0.9243, spcty(all    NaN, ca    NaN), wovlp 0.5844 : 0.83,0.78,0.73,0.68,0.66,0.62,0.60,0.58,0.55,0.55)
                        ///     ---------------------------------------------------------------------------------------------------------------------------------------------------------
                        ///     sbNMA   | orthonormal by SVD | RTB         | corr 0.9777, spcty(all    NaN, ca    NaN), wovlp 0.6065 : 0.93,0.89,0.86,0.81,0.75,0.71,0.69,0.66,0.63,0.62)
                        ///             | orthogonal         | RTB         | corr 0.9776, spcty(all    NaN, ca    NaN), wovlp 0.6175 : 0.94,0.90,0.87,0.82,0.76,0.73,0.71,0.69,0.66,0.63)
                        ///     ---------------------------------------------------------------------------------------------------------------------------------------------------------
                        ///     ssNMA   | orthonormal by SVD | RTB         | corr 0.9677, spcty(all    NaN, ca    NaN), wovlp 0.5993 : 0.92,0.87,0.83,0.77,0.72,0.69,0.66,0.63,0.60,0.59)
                        ///             | orthogonal         | RTB         | corr 0.9675, spcty(all    NaN, ca    NaN), wovlp 0.6076 : 0.92,0.88,0.84,0.78,0.73,0.70,0.67,0.64,0.62,0.60)
                        ///     ---------------------------------------------------------------------------------------------------------------------------------------------------------
                        ///     eANM    | orthonormal by SVD | RTB         | corr 0.9870, spcty(all    NaN, ca    NaN), wovlp 0.5906 : 0.95,0.91,0.87,0.83,0.77,0.73,0.71,0.68,0.66,0.61)
                        ///             | orthogonal         | RTB         | corr 0.9869, spcty(all    NaN, ca    NaN), wovlp 0.6014 : 0.95,0.92,0.88,0.84,0.78,0.74,0.73,0.70,0.67,0.65)
                        ///     ---------------------------------------------------------------------------------------------------------------------------------------------------------
                        ///     AA-ANM  | orthonormal by SVD | RTB         | corr 0.9593, spcty(all    NaN, ca    NaN), wovlp 0.4140 : 0.94,0.90,0.85,0.78,0.74,0.72,0.66,0.64,0.61,0.61)
                        ///             | orthogonal         | RTB         | corr 0.9589, spcty(all    NaN, ca    NaN), wovlp 0.4204 : 0.94,0.91,0.85,0.80,0.76,0.73,0.68,0.66,0.63,0.61)
                    }
                    Ps.AddRange(PBlk);
                }

                Matrix P = Matrix.FromColVectorList(Ps);

                Matrix PHP;
                Matrix PMP;
                using(new Matlab.NamedLock(""))
                {
                    if     (hess is HessMatrixSparse) Matlab.PutSparseMatrix("H", hess.GetMatrixSparse(), 3, 3);
                    else if(hess is HessMatrixDense ) Matlab.PutMatrix("H", hess, true);
                    else HDebug.Exception();
                    Matlab.PutMatrix("P", P);
                    Matlab.PutVector("M", masses);
                    Matlab.Execute("M=diag(reshape([M,M,M]',length(M)*3,1));");
                    Matlab.Execute("PHP = P'*H*P; PHP = (PHP + PHP')/2;");
                    Matlab.Execute("PMP = P'*M*P; PMP = (PMP + PMP')/2;");
                    PHP = Matlab.GetMatrix("PHP");
                    PMP = Matlab.GetMatrix("PMP");
                }

                return new HessRTB
                {
                    hess   = hess,
                    coords = coords,
                    masses = masses,
                    blocks = blocks,
                    P      = P,
                    PHP    = PHP,
                    PMP    = PMP,
                };
            }
            public static Vector[] ToOrthonormal(Vector[] coords, double[] masses, int[] block, Vector[] PBlk)
            {
                if(HDebug.IsDebuggerAttached)
                    #region check if elements in non-block are zeros.
                {
                    int leng = coords.Length;
                    foreach(int i in HEnum.HEnumCount(leng).HEnumExcept(block.HToHashSet()))
                        for(int r=0; r<PBlk.Length; r++)
                        {
                            int c0 = i*3;
                            HDebug.Assert(PBlk[r][c0+0] == 0);
                            HDebug.Assert(PBlk[r][c0+1] == 0);
                            HDebug.Assert(PBlk[r][c0+2] == 0);
                        }
                }
                    #endregion

                Matrix Pmat = new double[block.Length*3, PBlk.Length];
                for(int r=0; r<PBlk.Length; r++)
                    for(int i=0; i<block.Length; i++)
                    {
                        int i0 = i        * 3;
                        int c0 = block[i] * 3;
                        Pmat[i0+0, r] = PBlk[r][c0+0];
                        Pmat[i0+1, r] = PBlk[r][c0+1];
                        Pmat[i0+2, r] = PBlk[r][c0+2];
                    }

                using(new Matlab.NamedLock(""))
                {
                    Matlab.PutValue("n", PBlk.Length);
                    Matlab.PutMatrix("P", Pmat);
                    Matlab.Execute("[U,S,V] = svd(P);");
                    Matlab.Execute("U = U(:,1:n);");
                    if(HDebug.IsDebuggerAttached)
                    {
                        Matlab.Execute("SV = S(1:n,1:n)*V';");
                        double err = Matlab.GetValue("max(max(abs(P - U*SV)))");
                        HDebug.Assert(Math.Abs(err) < 0.00000001);
                    }
                    Pmat = Matlab.GetMatrix("U");
                }

                Vector[] PBlkOrth = new Vector[PBlk.Length];
                for(int r=0; r<PBlk.Length; r++)
                {
                    Vector PBlkOrth_r = new double[PBlk[r].Size];
                    for(int i=0; i<block.Length; i++)
                    {
                        int i0 = i        * 3;
                        int c0 = block[i] * 3;
                        PBlkOrth_r[c0+0] = Pmat[i0+0, r];
                        PBlkOrth_r[c0+1] = Pmat[i0+1, r];
                        PBlkOrth_r[c0+2] = Pmat[i0+2, r];
                    }
                    PBlkOrth[r] = PBlkOrth_r;
                }

                if(HDebug.IsDebuggerAttached)
                    #region checi the orthonormal condition, and rot/trans condition (using ANM)
                {
                    {   // check if all trans/rot modes are orthonormal
                        for(int i=0; i<PBlkOrth.Length; i++)
                        {
                            HDebug.Exception(Math.Abs(PBlkOrth[i].Dist - 1) < 0.00000001);
                            for(int j=i+1; j<PBlkOrth.Length; j++)
                            {
                                double dot = LinAlg.VtV(PBlkOrth[i], PBlkOrth[j]);
                                HDebug.Exception(Math.Abs(dot) < 0.00000001);
                            }
                        }
                    }
                    {   // check if this is true rot/trans modes using ANM
                        Vector[] anmcoords = coords.HClone();
                        int leng = coords.Length;
                        foreach(int i in HEnum.HEnumCount(leng).HEnumExcept(block.HToHashSet()))
                            anmcoords[i] = null;
                        HessMatrix H = GetHessAnm(anmcoords, 100);
                        Matrix PHP;
                        using(new Matlab.NamedLock(""))
                        {
                            Matlab.PutSparseMatrix("H", H.GetMatrixSparse(), 3, 3);
                            Matlab.PutMatrix("P", PBlkOrth.ToMatrix(true));
                            PHP = Matlab.GetMatrix("P'*H*P");
                        }
                        double maxerr = PHP.HAbsMax();
                        HDebug.Exception(Math.Abs(maxerr) < 0.00000001);
                    }
                }
                    #endregion

                return PBlkOrth;
            }
            public static Vector[] GetRotate(Vector[] coords, double[] masses, int[] block)
            {
                Vector cent = Geometry.CenterOfMass(coords, masses, block);
                return GetRotate(coords, cent, block);
            }
            public static Vector[] GetRotate(Vector[] coords, Vector cent, int[] block)
            {
                int leng = coords.Length;
                Vector[] rotbyx = new Vector[leng];
                Vector[] rotbyy = new Vector[leng];
                Vector[] rotbyz = new Vector[leng];
                
                Vector zeros = new double[3];
                for(int i = 0; i<leng; i++) rotbyx[i] = rotbyy[i] = rotbyz[i] = zeros;

                Vector rx = new double[3] { 1, 0, 0 };
                Vector ry = new double[3] { 0, 1, 0 };
                Vector rz = new double[3] { 0, 0, 1 };

                Func<Vector, Vector, Vector> GetTangent = delegate(Vector pt, Vector axisdirect)
                {
                    /// Magnitude of rotation tangent is proportional to the distance from the point to the axis.
                    /// Ex) when a point is in x-axis (r,0), rotating along z-axis by θ is: (r*sin(θ), 0)
                    /// 
                    ///  |
                    ///  |                 ^ sin(θ)
                    ///  |                 |
                    /// -+-----------------r----------
                    /// 
                    Vector rot1 = cent;
                    Vector rot2 = cent + axisdirect;
                    double dist = Geometry.DistancePointLine(pt, rot1, rot2);
                    Vector tan  = Geometry.RotateTangentUnit(pt, rot1, rot2) * dist;
                    return tan;
                };

                IEnumerable<int> enumblock = block;
                if(block != null) enumblock = block;
                else              enumblock = HEnum.HEnumCount(leng);
                foreach(int i in enumblock)
                {
                    Vector pt = coords[i];
                    rotbyx[i] = GetTangent(pt, rx);
                    rotbyy[i] = GetTangent(pt, ry);
                    rotbyz[i] = GetTangent(pt, rz);
                }

                Vector[] rots = new Vector[3]
                    {
                        rotbyx.ToVector().UnitVector(),
                        rotbyy.ToVector().UnitVector(),
                        rotbyz.ToVector().UnitVector(),
                    };
                return rots;
            }
            public static Vector[] GetTrans(Vector[] coords, double[] masses, int[] block)
            {
                Vector cent = Geometry.CenterOfMass(coords, masses, block);
                return GetTrans(coords, cent, block);
            }
            public static Vector[] GetTrans(Vector[] coords, Vector cent, int[] block)
            {
                int leng = coords.Length;
                Vector[] transx = new Vector[leng];
                Vector[] transy = new Vector[leng];
                Vector[] transz = new Vector[leng];

                Vector zeros = new double[3];
                for(int i = 0; i<leng; i++) transx[i] = transy[i] = transz[i] = zeros;

                Vector dx = new double[3] { 1, 0, 0 };
                Vector dy = new double[3] { 0, 1, 0 };
                Vector dz = new double[3] { 0, 0, 1 };

                IEnumerable<int> enumblock = block;
                if(block != null) enumblock = block;
                else              enumblock = HEnum.HEnumCount(leng);
                foreach(int i in enumblock)
                {
                    transx[i] = dx;
                    transy[i] = dy;
                    transz[i] = dz;
                }

                Vector[] trans = new Vector[3]
                    {
                        transx.ToVector().UnitVector(),
                        transy.ToVector().UnitVector(),
                        transz.ToVector().UnitVector(),
                    };
                return trans;
            }
        }
    }
}