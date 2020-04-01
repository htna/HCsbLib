#pragma warning disable CS0162

using System;
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
                        Matlab.PutMatrix("P", P, true);
                        Matlab.PutMatrix("M", M, true);
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
        public static HessRTB GetHessRTB(HessMatrix hess, Vector[] coords, double[] masses, IList<int[]> blocks, string opt)
        {
            return BuilderHessRTB.GetHessRTB(hess, coords, masses, blocks, opt);
        }
        public static HessRTB GetHessRTBByBlockAsResidue(HessMatrix hess, Vector[] coords, double[] masses, Universe.Atom[] atoms, string opt)
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

            return BuilderHessRTB.GetHessRTB(hess, coords, masses, blocks, opt);
        }
        public static class BuilderHessRTB
        {
            public static HessRTB GetHessRTB(HessMatrix hess, Vector[] coords, double[] masses, IList<int[]> blocks, string opt)
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
                    switch(opt)
                    {
                        case "v1":
                            // GetRotate is incorrect
                            PBlk.AddRange(GetTrans (coords, masses, block));
                            PBlk.AddRange(GetRotate(coords, masses, block));
                            break;
                        case "v2":
                            PBlk.AddRange(GetRotTran(coords, masses, block));
                            break;
                        case null:
                            goto case "v2";
                    }
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
                    Matlab.PutMatrix("P", P, true);
                    Matlab.PutVector("M", masses);
                    Matlab.Execute("M=diag(reshape([M,M,M]',length(M)*3,1));");
                    Matlab.Execute("PHP = P'*H*P; PHP = (PHP + PHP')/2;");
                    Matlab.Execute("PMP = P'*M*P; PMP = (PMP + PMP')/2;");
                    PHP = Matlab.GetMatrix("PHP", true);
                    PMP = Matlab.GetMatrix("PMP", true);
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
            public static Vector[] GetRotTran(Vector[] coords, double[] masses, int[] block)
            {
                Vector[] blkcoords = coords.HSelectByIndex(block);
                double[] blkmasses = masses.HSelectByIndex(block);
                Vector[] blkrottran = GetRotTran(blkcoords, blkmasses);
                Vector[] rottran = new Vector[blkrottran.Length];
                for(int i=0; i<rottran.Length; i++)
                {
                    HDebug.Assert(blkrottran[i].Size == block.Length*3);
                    rottran[i] = new double[coords.Length*3];
                    for(int j=0; j<block.Length; j++)
                    {
                        rottran[i][block[j]*3+0] = blkrottran[i][j*3+0];
                        rottran[i][block[j]*3+1] = blkrottran[i][j*3+1];
                        rottran[i][block[j]*3+2] = blkrottran[i][j*3+2];
                    }
                }
                return rottran;
            }
            public static Vector[] GetRotTran(Vector[] coords, double[] masses)
            {
                #region source rtbProjection.m
                /// rtbProjection.m
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// function [P, xyz] = rtbProjection(xyz, mass)
                /// % the approach is to find the inertia. compute the principal axes. and then use them to determine directly translation or rotation. 
                /// 
                /// n = size(xyz, 1); % n: the number of atoms
                /// if nargin == 1
                ///     mass = ones(n,1);
                /// end
                /// 
                /// M = sum(mass);
                /// % find the mass center.
                /// m3 = repmat(mass, 1, 3);
                /// center = sum (xyz.*m3)/M;
                /// xyz = xyz - center(ones(n, 1), :);
                /// 
                /// mwX = sqrt (m3).*xyz;
                /// inertia = sum(sum(mwX.^2))*eye(3) - mwX'*mwX;
                /// [V,D] = eig(inertia);
                /// tV = V'; % tV: transpose of V. Columns of V are principal axes. 
                /// for i=1:3
                ///     trans{i} = tV(ones(n,1)*i, :); % the 3 translations are along principal axes 
                /// end
                /// P = zeros(n*3, 6);
                /// for i=1:3
                ///     rotate{i} = cross(trans{i}, xyz);
                ///     temp = mat2vec(trans{i});
                ///     P(:,i) = temp/norm(temp);
                ///     temp = mat2vec(rotate{i});
                ///     P(:,i+3) = temp/norm(temp);
                /// end
                /// m3 = mat2vec(sqrt(m3));
                /// P = repmat (m3(:),1,size(P,2)).*P;
                /// % now normalize columns of P
                /// P = P*diag(1./normMat(P,1));
                /// 
                /// function vec = mat2vec(mat)
                /// % convert a matrix to a vector, extracting data *row-wise*.
                /// vec = reshape(mat',1,prod(size(mat)));
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                #endregion

                if(HDebug.Selftest())
                    #region selftest
                {
                    // get test coords and masses
                    Vector[] tcoords = Pdb.FromLines(SelftestData.lines_1EVC_pdb).atoms.ListCoord().ToArray();
                    double[] tmasses = new double[tcoords.Length];
                    for(int i=0; i<tmasses.Length; i++)
                        tmasses[i] = 1;
                    // get test rot/trans RTB vectors
                    Vector[] trottra = GetRotTran(tcoords, tmasses);
                    HDebug.Assert(trottra.Length == 6);
                    // get test ANM
                    var      tanm = Hess.GetHessAnm(tcoords);
                    // size of vec_i == 1
                    for(int i=0; i<trottra.Length; i++)
                    {
                        double dist = trottra[i].Dist;
                        HDebug.Assert(Math.Abs(dist - 1) < 0.00000001);
                    }
                    // vec_i and vec_j must be orthogonal
                    for(int i=0; i<trottra.Length; i++)
                        for(int j=i+1; j<trottra.Length; j++)
                        {
                            double dot = LinAlg.VtV(trottra[i], trottra[j]);
                            HDebug.Assert(Math.Abs(dot) < 0.00000001);
                        }
                    // vec_i' * ANM * vec_i == 0
                    for(int i=0; i<trottra.Length; i++)
                    {
                        double eigi = LinAlg.VtMV(trottra[i], tanm, trottra[i]);
                        HDebug.Assert(Math.Abs(eigi) < 0.00000001);
                        Vector tvecx = trottra[i].Clone();
                        tvecx[1] += (1.0 / tvecx.Size) * Math.Sign(tvecx[1]);
                        tvecx = tvecx.UnitVector();
                        double eigix = LinAlg.VtMV(tvecx, tanm, tvecx);
                        HDebug.Assert(Math.Abs(eigix) > 0.00000001);
                    }
                }
                    #endregion

                Vector[] rottran;
                using(new Matlab.NamedLock(""))
                {
                    Matlab.PutMatrix("xyz", coords.ToMatrix(), true);
                    Matlab.Execute  ("xyz = xyz';");
                    Matlab.PutVector("mass", masses);
                    //Matlab.Execute("function [P, xyz] = rtbProjection(xyz, mass)                                                                                        ");
                    //Matlab.Execute("% the approach is to find the inertia. compute the principal axes. and then use them to determine directly translation or rotation. ");
                    Matlab.Execute("                                                                                 ");
                    Matlab.Execute("n = size(xyz, 1); % n: the number of atoms                                       ");
                    //Matlab.Execute("if nargin == 1;                                                                  ");
                    //Matlab.Execute("    mass = ones(n,1);                                                            ");
                    //Matlab.Execute("end                                                                              ");
                    Matlab.Execute("                                                                                 ");
                    Matlab.Execute("M = sum(mass);                                                                   ");
                    Matlab.Execute("% find the mass center.                                                          ");
                    Matlab.Execute("m3 = repmat(mass, 1, 3);                                                         ");
                    Matlab.Execute("center = sum (xyz.*m3)/M;                                                        ");
                    Matlab.Execute("xyz = xyz - center(ones(n, 1), :);                                               ");
                    Matlab.Execute("                                                                                 ");
                    Matlab.Execute("mwX = sqrt (m3).*xyz;                                                            ");
                    Matlab.Execute("inertia = sum(sum(mwX.^2))*eye(3) - mwX'*mwX;                                    ");
                    Matlab.Execute("[V,D] = eig(inertia);                                                            ");
                    Matlab.Execute("tV = V'; % tV: transpose of V. Columns of V are principal axes.                  ");
                    Matlab.Execute("for i=1:3                                                                        \n"
                                  +"    trans{i} = tV(ones(n,1)*i, :); % the 3 translations are along principal axes \n"
                                  +"end                                                                              \n");
                    Matlab.Execute("P = zeros(n*3, 6);                                                               ");
                    Matlab.Execute("mat2vec = @(mat) reshape(mat',1,prod(size(mat)));                                ");
                    Matlab.Execute("for i=1:3                                                                        \n"
                                  +"    rotate{i} = cross(trans{i}, xyz);                                            \n"
                                  +"    temp = mat2vec(trans{i});                                                    \n"
                                  +"    P(:,i) = temp/norm(temp);                                                    \n"
                                  +"    temp = mat2vec(rotate{i});                                                   \n"
                                  +"    P(:,i+3) = temp/norm(temp);                                                  \n"
                                  +"end                                                                              ");
                    Matlab.Execute("m3 = mat2vec(sqrt(m3));                                                          ");
                    Matlab.Execute("P = repmat (m3(:),1,size(P,2)).*P;                                               ");
                    //Matlab.Execute("% now normalize columns of P                                                     "); // already normalized
                    //Matlab.Execute("normMat = @(x) sqrt(sum(x.^2,2));                                                "); // already normalized
                    //Matlab.Execute("P = P*diag(1./normMat(P,1));                                                     "); // already normalized
                    //Matlab.Execute("                                                                                 "); // already normalized
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    //Matlab.Execute("function vec = mat2vec(mat)                                                      ");
                    //Matlab.Execute("% convert a matrix to a vector, extracting data *row-wise*.                      ");
                    //Matlab.Execute("vec = reshape(mat',1,prod(size(mat)));                                           ");
                    //////////////////////////////////////////////////////////////////////////////////////////////////////
                    //Matlab.Execute("function amp = normMat(x)                                                        ");
                    //Matlab.Execute("amp = sqrt(sum(x.^2,2));                                                         ");

                    Matrix xyz = Matlab.GetMatrix("xyz", true);
                    Matrix P   = Matlab.GetMatrix("P", true);
                    rottran = P.GetColVectorList();
                }
                return rottran;
            }
            public static Vector[] GetRotate(Vector[] coords, double[] masses, int[] block)
            {
                Vector cent = Geometry.CenterOfMass(coords, masses, block);
                return GetRotate(coords, cent, block);
            }
            //[System.Diagnostics.CodeAnalysis.SuppressMessage
            public static Vector[] GetRotate(Vector[] coords, Vector cent, int[] block)
            {
                throw new Exception("this implementation is wrong. Use the following algorithm to get rotation modes for RTB.");

                double[] io_mass = null;
                if(HDebug.IsDebuggerAttached)
                {
                    using(var temp = new HTempDirectory(@"K:\temp\", null))
                    {
                        temp.EnterTemp();
                        HFile.WriteAllText("rtbProjection.m", @"
function [P, xyz] = rtbProjection(xyz, mass)
% the approach is to find the inertia. compute the principal axes. and then use them to determine directly translation or rotation. 

n = size(xyz, 1); % n: the number of atoms
if nargin == 1
    mass = ones(n,1);
end

M = sum(mass);
% find the mass center.
m3 = repmat(mass, 1, 3);
center = sum (xyz.*m3)/M;
xyz = xyz - center(ones(n, 1), :);

mwX = sqrt (m3).*xyz;
inertia = sum(sum(mwX.^2))*eye(3) - mwX'*mwX;
[V,D] = eig(inertia);
tV = V'; % tV: transpose of V. Columns of V are principal axes. 
for i=1:3
    trans{i} = tV(ones(n,1)*i, :); % the 3 translations are along principal axes 
end
P = zeros(n*3, 6);
for i=1:3
    rotate{i} = cross(trans{i}, xyz);
    temp = mat2vec(trans{i});
    P(:,i) = temp/norm(temp);
    temp = mat2vec(rotate{i});
    P(:,i+3) = temp/norm(temp);
end
m3 = mat2vec(sqrt(m3));
P = repmat (m3(:),1,size(P,2)).*P;
% now normalize columns of P
P = P*diag(1./normMat(P,1));

function vec = mat2vec(mat)
% convert a matrix to a vector, extracting data *row-wise*.
vec = reshape(mat',1,prod(size(mat)));
");
                        Matlab.Execute("cd \'"+ temp .dirinfo.FullName+ "\'");
                        Matlab.PutMatrix("xyz", coords.ToMatrix(false));
                        Matlab.PutVector("mass", io_mass);
                        temp.QuitTemp();
                    }
                }

                HDebug.Assert(coords.Length == io_mass.Length);

                Vector mwcenter = new double[3];
                for(int i = 0; i < coords.Length; i++)
                    mwcenter += (coords[i] * io_mass[i]);
                mwcenter /= io_mass.Sum();

                Vector[] mwcoords = new Vector[coords.Length];
                for(int i = 0; i < coords.Length; i++)
                    mwcoords[i] = (coords[i] - mwcenter) * io_mass[i];

                Matrix mwPCA = new double[3,3];
                for(int i = 0; i < coords.Length; i++)
                    mwPCA += LinAlg.VVt(mwcoords[i], mwcoords[i]);

                var V_D = LinAlg.Eig(mwPCA.ToArray());
                var V   = V_D.Item1;

                Vector[] rotvecs = new Vector[3];
                for(int i=0; i<3; i++)
                {
                    Vector rotaxis = new double[] { V[0,i], V[1,i], V[2,i] };
                    Vector[] rotveci = new Vector[coords.Length];
                    for(int j = 0; j < coords.Length; j++)
                        rotveci[j] = LinAlg.CrossProd(rotaxis, mwcoords[i]);
                    rotvecs[i] = rotveci.ToVector().UnitVector();
                }

                if(HDebug.IsDebuggerAttached)
                {
                    double dot01 = LinAlg.VtV(rotvecs[0], rotvecs[1]);
                    double dot02 = LinAlg.VtV(rotvecs[0], rotvecs[2]);
                    double dot12 = LinAlg.VtV(rotvecs[1], rotvecs[2]);
                    HDebug.Assert(Math.Abs(dot01) < 0.0000001);
                    HDebug.Assert(Math.Abs(dot02) < 0.0000001);
                    HDebug.Assert(Math.Abs(dot12) < 0.0000001);
                }

                return rotvecs;






                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// from song: rtbProjection.m
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// function [P, xyz] = rtbProjection(xyz, mass)
                /// % the approach is to find the inertia. compute the principal axes. and then use them to determine directly translation or rotation. 
                /// 
                /// n = size(xyz, 1); % n: the number of atoms
                /// if nargin == 1
                ///     mass = ones(n,1);
                /// end
                /// 
                /// M = sum(mass);
                /// % find the mass center.
                /// m3 = repmat(mass, 1, 3);
                /// center = sum (xyz.*m3)/M;
                /// xyz = xyz - center(ones(n, 1), :);
                /// 
                /// mwX = sqrt (m3).*xyz;
                /// inertia = sum(sum(mwX.^2))*eye(3) - mwX'*mwX;
                /// [V,D] = eig(inertia);
                /// tV = V'; % tV: transpose of V. Columns of V are principal axes. 
                /// for i=1:3
                ///     trans{i} = tV(ones(n,1)*i, :); % the 3 translations are along principal axes 
                /// end
                /// P = zeros(n*3, 6);
                /// for i=1:3
                ///     rotate{i} = cross(trans{i}, xyz);
                ///     temp = mat2vec(trans{i});
                ///     P(:,i) = temp/norm(temp);
                ///     temp = mat2vec(rotate{i});
                ///     P(:,i+3) = temp/norm(temp);
                /// end
                /// m3 = mat2vec(sqrt(m3));
                /// P = repmat (m3(:),1,size(P,2)).*P;
                /// % now normalize columns of P
                /// P = P*diag(1./normMat(P,1));
                /// 
                /// function vec = mat2vec(mat)
                /// % convert a matrix to a vector, extracting data *row-wise*.
                /// vec = reshape(mat',1,prod(size(mat)));
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                /// rotx[t_           ] := { { 1,0,0,0},{ 0,Cos[t],-Sin[t],0},{ 0,Sin[t],Cos[t],0},{ 0,0,0,1} };
                /// roty[t_           ] := { { Cos[t],0,Sin[t],0},{ 0,1,0,0},{ -Sin[t],0,Cos[t],0},{ 0,0,0,1} };
                /// rotz[t_           ] := { { Cos[t],-Sin[t],0,0},{ Sin[t],Cos[t],0,0},{ 0,0,1,0},{ 0,0,0,1} };
                /// tran[tx_, ty_, tz_] := { { 1,0,0,tx},{ 0,1,0,ty},{ 0,0,1,tz},{ 0,0,0,1} };
                /// pt                   = Transpose[{{px,py,pz,1}}];
                /// 
                /// point  : (px,py,pz)
                /// center : (cx,cy,cz)
                /// angle  : t
                /// 
                /// tran[cx, cy, cz].rotx[t].tran[-cx, -cy, -cz].pt           =    {{px}, {cy - cy Cos[t] + py Cos[t] + cz Sin[t] - pz Sin[t]}, {cz - cz Cos[t] + pz Cos[t] - cy Sin[t] + py Sin[t]}, {1}}
                /// tran[cx, cy, cz].roty[t].tran[-cx, -cy, -cz].pt           =    {{cx - cx Cos[t] + px Cos[t] - cz Sin[t] + pz Sin[t]}, {py}, {cz - cz Cos[t] + pz Cos[t] + cx Sin[t] - px Sin[t]}, {1}}
                /// tran[cx, cy, cz].rotz[t].tran[-cx, -cy, -cz].pt           =    {{cx - cx Cos[t] + px Cos[t] + cy Sin[t] - py Sin[t]}, {cy - cy Cos[t] + py Cos[t] - cx Sin[t] + px Sin[t]}, {pz}, {1}}
                /// 
                /// D[tran[cx, cy, cz].rotx[t].tran[-cx, -cy, -cz].pt, t]     =    {{0}, {cz Cos[t] - pz Cos[t] + cy Sin[t] - py Sin[t]}, {-cy Cos[t] + py Cos[t] + cz Sin[t] - pz Sin[t]}, {0}}
                /// D[tran[cx, cy, cz].roty[t].tran[-cx, -cy, -cz].pt, t]     =    {{-cz Cos[t] + pz Cos[t] + cx Sin[t] - px Sin[t]}, {0}, {cx Cos[t] - px Cos[t] + cz Sin[t] - pz Sin[t]}, {0}}
                /// D[tran[cx, cy, cz].rotz[t].tran[-cx, -cy, -cz].pt, t]     =    {{cy Cos[t] - py Cos[t] + cx Sin[t] - px Sin[t]}, {-cx Cos[t] + px Cos[t] + cy Sin[t] - py Sin[t]}, {0}, {0}}
                /// 
                /// D[tran[cx,cy,cz].rotx[a].tran[-cx,-cy,-cz].pt,a]/.a->0    =    {{0}, {cz - pz}, {-cy + py}, {0}}
                /// D[tran[cx,cy,cz].roty[a].tran[-cx,-cy,-cz].pt,a]/.a->0    =    {{-cz + pz}, {0}, {cx - px}, {0}}
                /// D[tran[cx,cy,cz].rotz[a].tran[-cx,-cy,-cz].pt,a]/.a->0    =    {{cy - py}, {-cx + px}, {0}, {0}}
                ///     rotx of atom (px,py,pz) with center (cx,cy,cz): {{0}, {cz - pz}, {-cy + py}, {0}}    =    { 0, cz - pz, -cy + py, 0 }    =>    {        0,  cz - pz, -cy + py }
                ///     rotx of atom (px,py,pz) with center (cx,cy,cz): {{-cz + pz}, {0}, {cx - px}, {0}}    =    { -cz + pz, 0, cx - px, 0 }    =>    { -cz + pz,        0,  cx - px }
                ///     rotx of atom (px,py,pz) with center (cx,cy,cz): {{cy - py}, {-cx + px}, {0}, {0}}    =    { cy - py, -cx + px, 0, 0 }    =>    {  cy - py, -cx + px,        0 }
                ///     

                int leng = coords.Length;

                Vector[] rots;
                {
                    Vector[] rotbyx = new Vector[leng];
                    Vector[] rotbyy = new Vector[leng];
                    Vector[] rotbyz = new Vector[leng];

                    double cx = cent[0];
                    double cy = cent[1];
                    double cz = cent[2];
                    for(int i=0; i<leng; i++)
                    {
                        double px = coords[i][0];
                        double py = coords[i][1];
                        double pz = coords[i][2];

                        rotbyx[i] = new double[] {        0,  cz - pz, -cy + py };
                        rotbyy[i] = new double[] { -cz + pz,        0,  cx - px };
                        rotbyz[i] = new double[] {  cy - py, -cx + px,        0 };
                    }

                    rots = new Vector[]
                        {
                            rotbyx.ToVector().UnitVector(),
                            rotbyy.ToVector().UnitVector(),
                            rotbyz.ToVector().UnitVector(),
                        };
                }

                if(HDebug.IsDebuggerAttached)
                {
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

                    Vector[] trots = new Vector[3]
                        {
                            rotbyx.ToVector().UnitVector(),
                            rotbyy.ToVector().UnitVector(),
                            rotbyz.ToVector().UnitVector(),
                        };

                    double test0 = LinAlg.VtV(rots[0], trots[0]);
                    double test1 = LinAlg.VtV(rots[1], trots[1]);
                    double test2 = LinAlg.VtV(rots[2], trots[2]);
                    HDebug.Assert(Math.Abs(test0 - 1) < 0.00000001);
                    HDebug.Assert(Math.Abs(test1 - 1) < 0.00000001);
                    HDebug.Assert(Math.Abs(test2 - 1) < 0.00000001);
                }
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
