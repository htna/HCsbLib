using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static bool GetHessAnmSelfTest()
        {
            if(HDebug.Selftest() == false)
                return true;

            string pdbpath = @"C:\Users\htna\svn\htnasvn_htna\VisualStudioSolutions\Library2\HTLib2.Bioinfo\Bioinfo.Data\pdb\1MJC.pdb";
            if(HFile.Exists(pdbpath) == false)
                return false;

            Pdb pdb = Pdb.FromFile(pdbpath);
            for(int i=0; i<pdb.atoms.Length; i++)
            {
                HDebug.Assert(pdb.atoms[0].altLoc  == pdb.atoms[i].altLoc );
                HDebug.Assert(pdb.atoms[0].chainID == pdb.atoms[i].chainID);
            }
            List<Vector> coords = pdb.atoms.ListCoord();
            double cutoff = 13;
            Matlab.Execute("clear");
            Matlab.PutMatrix("x", Matrix.FromRowVectorList(coords).ToArray());
            Matlab.PutValue("cutoffR", cutoff);
            Matlab.Execute(@"%  function cx = contactsNew(x, cutoffR)
                                % Contact matrix within cutoff distance.
                                % Author: Guang Song
                                % New: 10/25/2006
                                %

                                %n = size(x,1); 
                                % Method 1: slow
                                %for i=1:n
                                %  center = x(i,:);
                                %  distSqr(:,i) = sum((x-center(ones(n,1),:)).^2,2);
                                %end
                                %cx = sparse(distSqr<=cutoffR^2);

                                % Method 2: fast! about 28 times faster when array size is 659x3
                                %tot = zeros(n,n);
                                %for i=1:3
                                %  xi = x(:,ones(n,1)*i);
                                %  %tmp = (xi - xi.').^2;
                                %  %tot = tot + tmp;
                                %  tot = tot +  (xi - xi.').^2;
                                %end
                                %cx = sparse(tot<=cutoffR^2);

                                % Method 3: this implementation is the shortest! but sligtly slower than 
                                % method 2
                                %xn = x(:,:,ones(n,1)); % create n copy x
                                %xnp = permute(xn,[3,2,1]);
                                %tot = sum((xn-xnp).^2,2); % sum along x, y, z
                                %cx = sparse(permute(tot,[1,3,2])<=cutoffR^2);
                                % put it into one line like below actually slows it down. Don't do that.
                                %cx =  sparse(permute(sum((xn-permute(xn,[3,2,1])).^2,2),[1,3,2])<=cutoffR^2);

                                %Method 4: using function pdist, which I just know
                                % this one line implementation is even faster. 2 times than method 2.
                                cx = sparse(squareform(pdist(x)<=cutoffR));
                            ");
            Matlab.Execute(@"%  function [anm,xij,normxij] = baseHess(x,cx)
                                % Basic Hessian Matrix
                                % Author: Guang Song
                                % Created: Feb 23, 2005
                                % Rev: 11/09/06
                                %
                                % cx is the contact map. Also with gama info (new! 02/23/05)
                                dim = size(x,1);
                                nx = x(:,:,ones(1,dim));
                                xij = permute(nx,[3,1,2]) - permute(nx,[1,3,2]); % xj - xi for any i j
                                normxij = squareform(pdist(x)) + diag(ones(1,dim)); % + diag part added to avoid divided by zero.
                                anm = zeros(3*dim,3*dim);
                                for i=1:3
                                  for j=1:3
                                     tmp = xij(:,:,i).*xij(:,:,j).*cx./normxij.^2;
                                     tmp = diag(sum(tmp)) - tmp;
                                     anm(i:3:3*dim,j:3:3*dim) = tmp;
                                  end
                                end

                                % if dR is scalar, then dR = 1, back to GNM.
                                %if abs(i-j) == 1 % virtual bonds. should stay around 3.81 A
                                %   K33 = K33*100;
                                %end 
                                anm = (anm+anm')/2; % make sure return matrix is symmetric (fix numeric error)
                            ");
            Matrix anm_gsong = Matlab.GetMatrix("anm");
            Matlab.Execute("clear;");

            HessMatrix anm = GetHessAnm(coords.ToArray(), cutoff);

            if(anm_gsong.RowSize != anm.RowSize) { HDebug.Assert(false); return false; }
            if(anm_gsong.ColSize != anm.ColSize) { HDebug.Assert(false); return false; }

            for(int c=0; c<anm.ColSize; c++)
                for(int r=0; r<anm.RowSize; r++)
                    if(Math.Abs(anm_gsong[c, r] - anm[c, r]) >= 0.00000001)
                        { HDebug.Assert(false); return false; }

            return true;
        }
        public static IEnumerable<Tuple<int, int, double>> EnumHessAnmSpr(IList<Vector> coords, double cutoff, double sprcst)
        {
            if(HDebug.Selftest())
            {
                Vector[] _coords = Pdb.FromLines(SelftestData.lines_1L2Y_pdb).atoms.SelectByName("CA").ListCoord().ToArray().HSelectCount(10);
                HashSet<Tuple<int, int, double>> sprs0 = //EnumHessAnmSpr_obsolete(_coords, 7, 1).HToHashSet();
                    new HashSet<Tuple<int, int, double>>
                {
                    new Tuple<int,int,double>(0, 1, 1),   new Tuple<int,int,double>(1, 0, 1),   new Tuple<int,int,double>(0, 2, 1),   new Tuple<int,int,double>(2, 0, 1),   new Tuple<int,int,double>(0, 3, 1), 
                    new Tuple<int,int,double>(3, 0, 1),   new Tuple<int,int,double>(0, 4, 1),   new Tuple<int,int,double>(4, 0, 1),   new Tuple<int,int,double>(1, 2, 1),   new Tuple<int,int,double>(2, 1, 1),   
                    new Tuple<int,int,double>(1, 3, 1),   new Tuple<int,int,double>(3, 1, 1),   new Tuple<int,int,double>(1, 4, 1),   new Tuple<int,int,double>(4, 1, 1),   new Tuple<int,int,double>(1, 5, 1),   
                    new Tuple<int,int,double>(5, 1, 1),   new Tuple<int,int,double>(2, 3, 1),   new Tuple<int,int,double>(3, 2, 1),   new Tuple<int,int,double>(2, 4, 1),   new Tuple<int,int,double>(4, 2, 1),   
                    new Tuple<int,int,double>(2, 5, 1),   new Tuple<int,int,double>(5, 2, 1),   new Tuple<int,int,double>(2, 6, 1),   new Tuple<int,int,double>(6, 2, 1),   new Tuple<int,int,double>(3, 4, 1),   
                    new Tuple<int,int,double>(4, 3, 1),   new Tuple<int,int,double>(3, 5, 1),   new Tuple<int,int,double>(5, 3, 1),   new Tuple<int,int,double>(3, 6, 1),   new Tuple<int,int,double>(6, 3, 1),   
                    new Tuple<int,int,double>(3, 7, 1),   new Tuple<int,int,double>(7, 3, 1),   new Tuple<int,int,double>(4, 5, 1),   new Tuple<int,int,double>(5, 4, 1),   new Tuple<int,int,double>(4, 6, 1),   
                    new Tuple<int,int,double>(6, 4, 1),   new Tuple<int,int,double>(4, 7, 1),   new Tuple<int,int,double>(7, 4, 1),   new Tuple<int,int,double>(4, 8, 1),   new Tuple<int,int,double>(8, 4, 1),   
                    new Tuple<int,int,double>(5, 6, 1),   new Tuple<int,int,double>(6, 5, 1),   new Tuple<int,int,double>(5, 7, 1),   new Tuple<int,int,double>(7, 5, 1),   new Tuple<int,int,double>(5, 8, 1),   
                    new Tuple<int,int,double>(8, 5, 1),   new Tuple<int,int,double>(6, 7, 1),   new Tuple<int,int,double>(7, 6, 1),   new Tuple<int,int,double>(6, 8, 1),   new Tuple<int,int,double>(8, 6, 1),   
                    new Tuple<int,int,double>(6, 9, 1),   new Tuple<int,int,double>(9, 6, 1),   new Tuple<int,int,double>(7, 8, 1),   new Tuple<int,int,double>(8, 7, 1),   new Tuple<int,int,double>(7, 9, 1),   
                    new Tuple<int,int,double>(9, 7, 1),   new Tuple<int,int,double>(8, 9, 1),   new Tuple<int,int,double>(9, 8, 1),   
                };
                HashSet<Tuple<int, int, double>> sprs1 = EnumHessAnmSpr         (_coords, 7, 1).HToHashSet();
                HDebug.Exception(sprs0.Count == sprs1.Count);
                foreach(var spr in sprs0)
                {
                    HDebug.Exception(sprs1.Contains(spr));
                }
            }

            KDTreeDLL.KDTree<object> kdtree = new KDTreeDLL.KDTree<object>(3);
            for(int i=0; i<coords.Count; i++)
                kdtree.insert(coords[i], i);

            int size = coords.Count;
            double cutoff2 = cutoff*cutoff;
            int num_springs = 0;
            for(int c=0; c<coords.Count; c++)
            {
                Vector lowk = coords[c] - (new double[] {cutoff, cutoff, cutoff});
                Vector uppk = coords[c] + (new double[] {cutoff, cutoff, cutoff});
                foreach(int r in kdtree.range(lowk, uppk))
                {
                    if(c >= r)
                        continue;
                    double dist2 = (coords[c] - coords[r]).Dist2;
                    if(dist2 < cutoff2)
                    {
                        yield return new Tuple<int, int, double>(c, r, sprcst);
                        yield return new Tuple<int, int, double>(r, c, sprcst);
                        num_springs += 2;
                    }
                }
            }
            double ratio_springs = ((double)num_springs) / (size*size);
        }
        //public static IEnumerable<Tuple<int, int, double>> EnumHessAnmSpr_obsolete(IList<Vector> coords, double cutoff, double sprcst)
        //{
        //    int size = coords.Count;
        //    double cutoff2 = cutoff*cutoff;
        //    //Matrix Kij = Matrix.Zeros(size, size);
        //    int num_springs = 0;
        //    for(int c=0; c<size; c++)
        //    {
        //        if(coords[c] == null) continue;
        //        for(int r=c+1; r<size; r++)
        //        {
        //            if(coords[r] == null) continue;
        //            double dist2 = (coords[c] - coords[r]).Dist2;
        //            if(dist2 <= cutoff2)
        //            {
        //                yield return new Tuple<int, int, double>(c, r, sprcst);  //Kij[c, r] = sprcst;
        //                yield return new Tuple<int, int, double>(r, c, sprcst);  //Kij[r, c] = sprcst;
        //                num_springs += 2;
        //            }
        //        }
        //    }
        //    double ratio_springs = ((double)num_springs) / (size*size);
        //    //return Kij;
        //}
        public static MatrixSparse<double> GetHessAnmBmat(IList<Vector> coords, double cutoff)
        {
            if(HDebug.Selftest())
            {
                var    _coords = Pdb._smallest_protein_cacoords;
                Matrix _Bmat = Hess.GetHessAnmBmat(_coords, 13).ToArray();
                Matrix _BB = _Bmat * _Bmat.Tr();
                Matrix _ANM = Hess.GetHessAnm(_coords, 13).ToMatrix();
                double _err = (_BB - _ANM).HAbsMax();
                HDebug.Assert(_err < 0.00000001);
            }

            List<Tuple<int, int, double>> sprs = EnumHessAnmSpr(coords, cutoff, 1).ToList();
            MatrixSparse<double> Bmat = new MatrixSparse<double>(3*coords.Count, sprs.Count);
            int spr_count = 0;
            for(int ij=0; ij<sprs.Count; ij++)
            {
                var spr = sprs[ij];
                int    ai  = spr.Item1;
                int    aj  = spr.Item2;
                if(ai >= aj)
                    continue;
                double kij = spr.Item3;
                Vector coordi = coords[ai];
                Vector coordj = coords[aj];
                double sij = (coordi - coordj).Dist;
                double xij = (coordj[0] - coordi[0])/sij;
                double yij = (coordj[1] - coordi[1])/sij;
                double zij = (coordj[2] - coordi[2])/sij;
                Bmat[(ai*3+0), spr_count] = xij;
                Bmat[(ai*3+1), spr_count] = yij;
                Bmat[(ai*3+2), spr_count] = zij;
                Bmat[(aj*3+0), spr_count] = -xij;
                Bmat[(aj*3+1), spr_count] = -yij;
                Bmat[(aj*3+2), spr_count] = -zij;
                spr_count++;
            }
            return Bmat.GetSubMatrix(3*coords.Count, spr_count);
        }
        public static HessInfo GetHessAnm(Universe univ, IList<Vector> coords, double cutoff)
        {
            // cutoff: 4.5    for all atomic model
            //         7 or 8 for CA model (Atilgan01-BiophysJ - Anisotropy of Fluctuation Dynamics of Proteins with an Elastic Network Model)
            HessMatrix hess = GetHessAnm(coords, cutoff);

            Vector   lmass   = null;
            object[] latoms  = null;
            Vector[] lcoords = null;

            if(univ == null)
            {
                lmass   = Vector.Ones(coords.Count);
                latoms  = null;
                lcoords = coords.HCloneVectors().ToArray();
            }
            else
            {
                lmass   = univ.GetMasses();
                latoms  = univ.atoms.ToArray();
                lcoords = coords.HCloneVectors().ToArray();
            }

            return new HessInfo
            {
                hess   = hess,
                mass   = lmass,
                atoms  = latoms,
                coords = lcoords,
                numZeroEigval = 6,
            };
        }
        public static HessMatrix GetHessAnm(IList<Vector> coords, double cutoff, string options="")
        {
            double[] cutoffs = new double[coords.Count];
            for(int i=0; i<cutoffs.Length; i++)
                cutoffs[i] = cutoff;
            return GetHessAnm(coords, cutoffs, options);
        }
        public static HessMatrix GetHessAnm(IList<Vector> coords, IList<double> cutoffs, string options="")
        {
            int n = coords.Count;
            HessMatrix hess = null;
            hess = HessMatrix.ZerosHessMatrix(n*3, n*3);
            GetHessAnm(coords, cutoffs, hess, options);
            return hess;
        }
        public static void GetHessAnm(IList<Vector> coords, IList<double> cutoffs, HessMatrix hess, string options="")
        {
            if(coords.Count > 10000)
                HDebug.ToDo("System size is too big. Use EnumHessAnmSpr() and other GetHessAnm()");

            //Debug.Assert(AnmHessianSelfTest());

            HDebug.Assert(coords.Count == cutoffs.Count);

            double[] cutoffs2 = new double[cutoffs.Count];
            for(int i=0; i<cutoffs.Count; i++)
                cutoffs2[i] = cutoffs[i] * cutoffs[i];

            double Epsilon = 0;// double.Epsilon;

            int n = coords.Count;
            if(hess.RowSize != 3*n || hess.ColSize != 3*n)
                throw new Exception();

            Action<int> comp_hess_i = delegate(int i)
            {
                if(coords[i] == null)
                    return;
                //continue;
                for(int j=0; j<n; j++)
                {
                    if(i == j)
                        continue;
                    if(coords[j] == null)
                        continue;
                    Vector vec_ij = coords[j] - coords[i];
                    double dist2 = vec_ij.Dist2;
                    double cutoff2 = Math.Max(cutoffs2[i], cutoffs2[j]);
                    if(dist2 > cutoff2)
                    {
                        if(Epsilon == 0)
                            continue;
                        HDebug.Assert(hess[i*3+0, j*3+0] == 0); hess[i*3+0, j*3+0] = Epsilon;
                        HDebug.Assert(hess[i*3+0, j*3+1] == 0); hess[i*3+0, j*3+1] = Epsilon;
                        HDebug.Assert(hess[i*3+0, j*3+2] == 0); hess[i*3+0, j*3+2] = Epsilon;
                        HDebug.Assert(hess[i*3+1, j*3+0] == 0); hess[i*3+1, j*3+0] = Epsilon;
                        HDebug.Assert(hess[i*3+1, j*3+1] == 0); hess[i*3+1, j*3+1] = Epsilon;
                        HDebug.Assert(hess[i*3+1, j*3+2] == 0); hess[i*3+1, j*3+2] = Epsilon;
                        HDebug.Assert(hess[i*3+2, j*3+0] == 0); hess[i*3+2, j*3+0] = Epsilon;
                        HDebug.Assert(hess[i*3+2, j*3+1] == 0); hess[i*3+2, j*3+1] = Epsilon;
                        HDebug.Assert(hess[i*3+2, j*3+2] == 0); hess[i*3+2, j*3+2] = Epsilon;
                        continue;
                    }
                    Vector unit_ij = vec_ij.UnitVector();
                    double k_ij = 1; double val;
                    val = k_ij * unit_ij[0]*unit_ij[0]; hess[i*3+0, j*3+0] = Epsilon - val; hess[i*3+0, i*3+0] += val;  // hess[i*3+0, j*3+0] = Epsilon - k_ij * unit_ij[0]*unit_ij[0]; hess[i*3+0, i*3+0] += k_ij * unit_ij[0]*unit_ij[0];
                    val = k_ij * unit_ij[0]*unit_ij[1]; hess[i*3+0, j*3+1] = Epsilon - val; hess[i*3+0, i*3+1] += val;  // hess[i*3+0, j*3+1] = Epsilon - k_ij * unit_ij[0]*unit_ij[1]; hess[i*3+0, i*3+1] += k_ij * unit_ij[0]*unit_ij[1];
                    val = k_ij * unit_ij[0]*unit_ij[2]; hess[i*3+0, j*3+2] = Epsilon - val; hess[i*3+0, i*3+2] += val;  // hess[i*3+0, j*3+2] = Epsilon - k_ij * unit_ij[0]*unit_ij[2]; hess[i*3+0, i*3+2] += k_ij * unit_ij[0]*unit_ij[2];
                    val = k_ij * unit_ij[1]*unit_ij[0]; hess[i*3+1, j*3+0] = Epsilon - val; hess[i*3+1, i*3+0] += val;  // hess[i*3+1, j*3+0] = Epsilon - k_ij * unit_ij[1]*unit_ij[0]; hess[i*3+1, i*3+0] += k_ij * unit_ij[1]*unit_ij[0];
                    val = k_ij * unit_ij[1]*unit_ij[1]; hess[i*3+1, j*3+1] = Epsilon - val; hess[i*3+1, i*3+1] += val;  // hess[i*3+1, j*3+1] = Epsilon - k_ij * unit_ij[1]*unit_ij[1]; hess[i*3+1, i*3+1] += k_ij * unit_ij[1]*unit_ij[1];
                    val = k_ij * unit_ij[1]*unit_ij[2]; hess[i*3+1, j*3+2] = Epsilon - val; hess[i*3+1, i*3+2] += val;  // hess[i*3+1, j*3+2] = Epsilon - k_ij * unit_ij[1]*unit_ij[2]; hess[i*3+1, i*3+2] += k_ij * unit_ij[1]*unit_ij[2];
                    val = k_ij * unit_ij[2]*unit_ij[0]; hess[i*3+2, j*3+0] = Epsilon - val; hess[i*3+2, i*3+0] += val;  // hess[i*3+2, j*3+0] = Epsilon - k_ij * unit_ij[2]*unit_ij[0]; hess[i*3+2, i*3+0] += k_ij * unit_ij[2]*unit_ij[0];
                    val = k_ij * unit_ij[2]*unit_ij[1]; hess[i*3+2, j*3+1] = Epsilon - val; hess[i*3+2, i*3+1] += val;  // hess[i*3+2, j*3+1] = Epsilon - k_ij * unit_ij[2]*unit_ij[1]; hess[i*3+2, i*3+1] += k_ij * unit_ij[2]*unit_ij[1];
                    val = k_ij * unit_ij[2]*unit_ij[2]; hess[i*3+2, j*3+2] = Epsilon - val; hess[i*3+2, i*3+2] += val;  // hess[i*3+2, j*3+2] = Epsilon - k_ij * unit_ij[2]*unit_ij[2]; hess[i*3+2, i*3+2] += k_ij * unit_ij[2]*unit_ij[2];
                }
            };

            if(options.Split(';').Contains("parallel"))
            {
                System.Threading.Tasks.Parallel.For(0, n, comp_hess_i);
            }
            else
            {
                for(int i=0; i<n; i++)
                    comp_hess_i(i);
            }
        }
        //public static void GetHessAnm(IList<Vector> coords, double cutoff, MatrixSparse<MatrixByArr> hess)
        //{
        //    int n = coords.Count;
        //
        //    MatrixSparse<MatrixByArr> lhess = null;
        //    if(HDebug.Selftest())
        //        lhess = new MatrixSparse<MatrixByArr>(n, n, hess.GetDefault);
        //
        //    double cutoff2 = cutoff * cutoff;
        //    for(int i=0; i<n; i++)
        //    {
        //        if(coords[i] == null)
        //            continue;
        //        for(int j=0; j<n; j++)
        //        {
        //            if(i == j)
        //                continue;
        //            if(coords[j] == null)
        //                continue;
        //            Vector vec_ij = coords[j] - coords[i];
        //            double dist2 = vec_ij.Dist2;
        //            if(dist2 > cutoff2)
        //                continue;
        //
        //            Vector unit_ij = vec_ij.UnitVector();
        //            double k_ij = 1;
        //            MatrixByArr hessij = new double[3, 3];
        //            hessij[0, 0] = k_ij * unit_ij[0]*unit_ij[0];
        //            hessij[0, 1] = k_ij * unit_ij[0]*unit_ij[1];
        //            hessij[0, 2] = k_ij * unit_ij[0]*unit_ij[2];
        //            hessij[1, 0] = k_ij * unit_ij[1]*unit_ij[0];
        //            hessij[1, 1] = k_ij * unit_ij[1]*unit_ij[1];
        //            hessij[1, 2] = k_ij * unit_ij[1]*unit_ij[2];
        //            hessij[2, 0] = k_ij * unit_ij[2]*unit_ij[0];
        //            hessij[2, 1] = k_ij * unit_ij[2]*unit_ij[1];
        //            hessij[2, 2] = k_ij * unit_ij[2]*unit_ij[2];
        //            hess[i, j] -= hessij;
        //            hess[i, i] += hessij;
        //            if(lhess != null)
        //            {
        //                lhess[i, j] -= hessij;
        //                lhess[i, i] += hessij;
        //            }
        //        }
        //    }
        //
        //    if(lhess != null)
        //    {
        //        MatrixByArr lhess0 = GetHessAnm(coords, cutoff);
        //        MatrixByArr lhess1 = MatrixByArr.FromMatrixArray(lhess.ToArray());
        //        HDebug.AssertTolerance(0.00000000001, lhess0-lhess1);
        //    }
        //}
        static HessMatrix GetHessAnm_debug(IList<Vector> coords, Matrix Kij)
        {
            //Debug.Assert(AnmHessianSelfTest());

            int n = coords.Count;
            HessMatrix hess = HessMatrix.ZerosHessMatrix(n*3, n*3);
            for(int i=0; i<n; i++)
            {
                if(coords[i] == null)
                    continue;
                for(int j=0; j<n; j++)
                {
                    if(i == j)
                    {
                        HDebug.Assert(Kij[i, j] == 0);
                        continue;
                    }
                    if(coords[j] == null)
                        continue;
                    Vector vec_ij = coords[j] - coords[i];
                    Vector unit_ij = vec_ij.UnitVector();
                    double k_ij = Kij[i, j];
                    HDebug.Assert(double.IsNaN(k_ij) == false);
                    if(k_ij == 0)
                        continue;
                    hess[i*3+0, j*3+0] = -k_ij * unit_ij[0]*unit_ij[0];    hess[i*3+0, i*3+0] += k_ij * unit_ij[0]*unit_ij[0];
                    hess[i*3+0, j*3+1] = -k_ij * unit_ij[0]*unit_ij[1];    hess[i*3+0, i*3+1] += k_ij * unit_ij[0]*unit_ij[1];
                    hess[i*3+0, j*3+2] = -k_ij * unit_ij[0]*unit_ij[2];    hess[i*3+0, i*3+2] += k_ij * unit_ij[0]*unit_ij[2];
                    hess[i*3+1, j*3+0] = -k_ij * unit_ij[1]*unit_ij[0];    hess[i*3+1, i*3+0] += k_ij * unit_ij[1]*unit_ij[0];
                    hess[i*3+1, j*3+1] = -k_ij * unit_ij[1]*unit_ij[1];    hess[i*3+1, i*3+1] += k_ij * unit_ij[1]*unit_ij[1];
                    hess[i*3+1, j*3+2] = -k_ij * unit_ij[1]*unit_ij[2];    hess[i*3+1, i*3+2] += k_ij * unit_ij[1]*unit_ij[2];
                    hess[i*3+2, j*3+0] = -k_ij * unit_ij[2]*unit_ij[0];    hess[i*3+2, i*3+0] += k_ij * unit_ij[2]*unit_ij[0];
                    hess[i*3+2, j*3+1] = -k_ij * unit_ij[2]*unit_ij[1];    hess[i*3+2, i*3+1] += k_ij * unit_ij[2]*unit_ij[1];
                    hess[i*3+2, j*3+2] = -k_ij * unit_ij[2]*unit_ij[2];    hess[i*3+2, i*3+2] += k_ij * unit_ij[2]*unit_ij[2];
                }
            }
            return hess;
        }
        static bool              GetHessAnm_selftest = HDebug.IsDebuggerAttached;
        public static HessMatrix GetHessAnm(IList<Vector> coords, Matrix Kij)
        {
            IEnumerable<Tuple<int, int, double>> enumKij = Kij.HEnumNonZeros();
            return GetHessAnm(coords, enumKij);
        }
        public static HessMatrix GetHessAnm(IList<Vector> coords, IEnumerable<Tuple<int, int, double>> enumKij)
        {
            if(GetHessAnm_selftest)
            {
                GetHessAnm_selftest = false;
                Vector[] tcoords = new Vector[]
                {
                    new Vector( 0, 0, 0),
                    new Vector( 1, 0, 0),
                    new Vector( 0, 2, 0),
                    new Vector( 0, 0, 3),
                    new Vector( 4, 5, 0),
                    new Vector( 0, 6, 7),
                    new Vector( 8, 0, 9),
                    new Vector(-1,-1,-1),
                };
                Matrix tKij = Matrix.Ones(8, 8);
                for(int i=0; i<8; i++) tKij[i, i] = 0;
                HessMatrix hess0 = GetHessAnm_debug(tcoords, tKij);
                HessMatrix hess1 = GetHessAnm      (tcoords, tKij.HEnumNonZeros());
                HDebug.Assert((hess0 - hess1).HAbsMax() == 0);
            }
            //Debug.Assert(AnmHessianSelfTest());

            int n = coords.Count;
            HessMatrix hess = null;
            hess = HessMatrix.ZerosHessMatrix(n*3, n*3);
            int numset = 0;
            foreach(var kij in enumKij)
            {
                int i = kij.Item1;
                int j = kij.Item2;
                double k_ij = kij.Item3;
                {
                    if(i == j)
                    {
                        HDebug.Assert(k_ij == 0);
                        continue;
                    }
                    if(coords[j] == null)
                        continue;
                    Vector vec_ij = coords[j] - coords[i];
                    Vector unit_ij = vec_ij.UnitVector();
                    HDebug.Assert(double.IsNaN(k_ij) == false);
                    if(k_ij == 0)
                        continue;
                    MatrixByArr hessij = new double[3, 3];
                    hessij[0, 0] = -k_ij * unit_ij[0]*unit_ij[0];
                    hessij[0, 1] = -k_ij * unit_ij[0]*unit_ij[1];
                    hessij[0, 2] = -k_ij * unit_ij[0]*unit_ij[2];
                    hessij[1, 0] = -k_ij * unit_ij[1]*unit_ij[0];
                    hessij[1, 1] = -k_ij * unit_ij[1]*unit_ij[1];
                    hessij[1, 2] = -k_ij * unit_ij[1]*unit_ij[2];
                    hessij[2, 0] = -k_ij * unit_ij[2]*unit_ij[0];
                    hessij[2, 1] = -k_ij * unit_ij[2]*unit_ij[1];
                    hessij[2, 2] = -k_ij * unit_ij[2]*unit_ij[2];
                    hess.SetBlock(i, j, hessij);
                    MatrixByArr hessii = hess.GetBlock(i, i) - hessij;
                    hess.SetBlock(i, i, hessii);
                    numset++;
                }
            }

            return hess;
        }
        public static HessMatrix GetHessAnm(IList<Vector> coords)
        {
            //Debug.Assert(AnmHessianSelfTest());

            int n = coords.Count;
            HessMatrix hess = HessMatrix.ZerosHessMatrix(n*3, n*3);
            for(int i=0; i<n; i++)
            {
                if(coords[i] == null)
                    continue;
                for(int j=0; j<n; j++)
                {
                    if(i == j)
                        continue;
                    if(coords[j] == null)
                        continue;
                    Vector vec_ij = coords[j] - coords[i];
                    double dist2 = vec_ij.Dist2;

                    Vector unit_ij = vec_ij.UnitVector();
                    double k_ij = 1.0 / dist2;
                    hess[i*3+0, j*3+0] = double.Epsilon - k_ij * unit_ij[0]*unit_ij[0]; hess[i*3+0, i*3+0] += k_ij * unit_ij[0]*unit_ij[0];
                    hess[i*3+0, j*3+1] = double.Epsilon - k_ij * unit_ij[0]*unit_ij[1]; hess[i*3+0, i*3+1] += k_ij * unit_ij[0]*unit_ij[1];
                    hess[i*3+0, j*3+2] = double.Epsilon - k_ij * unit_ij[0]*unit_ij[2]; hess[i*3+0, i*3+2] += k_ij * unit_ij[0]*unit_ij[2];
                    hess[i*3+1, j*3+0] = double.Epsilon - k_ij * unit_ij[1]*unit_ij[0]; hess[i*3+1, i*3+0] += k_ij * unit_ij[1]*unit_ij[0];
                    hess[i*3+1, j*3+1] = double.Epsilon - k_ij * unit_ij[1]*unit_ij[1]; hess[i*3+1, i*3+1] += k_ij * unit_ij[1]*unit_ij[1];
                    hess[i*3+1, j*3+2] = double.Epsilon - k_ij * unit_ij[1]*unit_ij[2]; hess[i*3+1, i*3+2] += k_ij * unit_ij[1]*unit_ij[2];
                    hess[i*3+2, j*3+0] = double.Epsilon - k_ij * unit_ij[2]*unit_ij[0]; hess[i*3+2, i*3+0] += k_ij * unit_ij[2]*unit_ij[0];
                    hess[i*3+2, j*3+1] = double.Epsilon - k_ij * unit_ij[2]*unit_ij[1]; hess[i*3+2, i*3+1] += k_ij * unit_ij[2]*unit_ij[1];
                    hess[i*3+2, j*3+2] = double.Epsilon - k_ij * unit_ij[2]*unit_ij[2]; hess[i*3+2, i*3+2] += k_ij * unit_ij[2]*unit_ij[2];
                }
            }
            return hess;
        }
    }
}
