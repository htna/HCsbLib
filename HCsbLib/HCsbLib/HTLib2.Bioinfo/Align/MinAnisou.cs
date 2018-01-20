using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Align
    {
        public class MinAnisou : IMinAlign
        {
            //public static Trans3 GetTrans(IList<Vector> C1, Matrix[] anisou1, IList<Vector> C2)
            //{
            //    int size = C1.Count;
            //    Tuple<Vector[], double[]>[] eigs1 = new Tuple<Vector[], double[]>[size];
            //    {
            //        for(int i=0; i<size; i++)
            //        {
            //            Vector[] eigvec;
            //            double[] eigval;
            //            Debug.Verify(NumericSolver.Eig(anisou1[i], out eigvec, out eigval));
            //            {   // normalize eigval and eigvec
            //                double l;
            //                l = eigvec[0].Dist; eigvec[0] /= l; eigval[0] *= (l*l); eigval[0] = Math.Pow(eigval[0], 2); Debug.Assert(eigval[0] >= 0);
            //                l = eigvec[1].Dist; eigvec[1] /= l; eigval[1] *= (l*l); eigval[1] = Math.Pow(eigval[1], 2); Debug.Assert(eigval[1] >= 0);
            //                l = eigvec[2].Dist; eigvec[2] /= l; eigval[2] *= (l*l); eigval[2] = Math.Pow(eigval[2], 2); Debug.Assert(eigval[2] >= 0);
            //            }
            //            {
            //                eigval[0] = 1 / eigval[0];
            //                eigval[1] = 1 / eigval[1];
            //                eigval[2] = 1 / eigval[2];
            //            }
            //            eigs1[i] = new Tuple<Vector[], double[]>(eigvec, eigval);
            //        }
            //    }
            //    return GetTrans(C1, eigs1, C2);
            //}

            public static void GetEnergies(AlignData data1, IList<Vector> coords2
                                              , out double pot_rmsd, out double pot_enrg, out double pot_enrg_full, out double pot_enrg_anisou
                                              , Pdb pdb2 = null
                                              , string pdb2outpath = null
                                              )
            {
                Anisou[] anisous1 = data1.GetAnisous();
                Trans3 trans = GetTrans(data1.coords, anisous1, coords2);
                List<Vector> coords2trans = new List<Vector>(trans.GetTransformed(coords2));
                if(pdb2 != null && pdb2outpath != null)
                    pdb2.ToFile(pdb2outpath, coords2trans);
                //data1.pdb.ToFile(pdb2outpath, coords2trans, anisous: anisous1.GetUs());

                pot_rmsd = data1.GetRmsdFrom(coords2trans);
                pot_enrg = data1.GetEnergyFromDiag(coords2trans);
                pot_enrg_full = data1.GetEnergyFromFull(coords2trans);
                pot_enrg_anisou = data1.GetEnergyFromAnisou(coords2trans);
            }

            public static void Align( IList<Vector> C1
                                    , IList<Anisou> anisou1
                                    , ref List<Vector> C2
                                    , HPack<List<Vector>> optMoveC2=null
                                    , HPack<Trans3> outTrans=null
                                    , HPack<double> optEnergy=null
                                    )
            {
                Trans3 trans = GetTrans(C1, anisou1, C2, optEnergy:optEnergy);
                Vector[] nC2 = trans.GetTransformed(C2).ToArray();
                if(optMoveC2 != null)
                {
                    optMoveC2.value = new List<Vector>(nC2.Length);
                    for(int i=0; i<nC2.Length; i++)
                        optMoveC2.value.Add(nC2[i] - C2[i]);
                }
                if(outTrans != null)
                    outTrans.value = trans;
                C2 = new List<Vector>(nC2);
            }

            public static Trans3 GetTrans(IList<Vector> C1, IList<Anisou> anisou1, IList<Vector> C2, HPack<double> optEnergy=null)
            {
                int size = C1.Count;
                HDebug.Assert(size==C1.Count, size==C2.Count);

                Vector[] C1s = C1.ToArray().Clone(3);
                Vector[] C2s = C2.ToArray().Clone(3);
                double[] W1s = new double[size*3];
                double[] enrgs = new double[size*3];
                for(int i=0; i<size; i++)
                {
                    HDebug.Assert(anisou1[i].eigvals[0] >= 0); W1s[i*3+0] = (anisou1[i].eigvals[0] <= 0) ? 0 : 1 / anisou1[i].eigvals[0];
                    HDebug.Assert(anisou1[i].eigvals[1] >= 0); W1s[i*3+1] = (anisou1[i].eigvals[1] <= 0) ? 0 : 1 / anisou1[i].eigvals[1];
                    HDebug.Assert(anisou1[i].eigvals[2] >= 0); W1s[i*3+2] = (anisou1[i].eigvals[2] <= 0) ? 0 : 1 / anisou1[i].eigvals[2];
                    enrgs[i*3+0] = enrgs[i*3+1] = enrgs[i*3+2] = double.NaN;
                }

                //Trans3 trans = ICP3.OptimalTransform(C2, C1);
                Trans3 trans = new Trans3(new double[] { 0, 0, 0 }, 1, Quaternion.UnitRotation);// = transICP3.OptimalTransformWeighted(C2, C1, W1s);
                int iter = 0;
                int iter_max = 1000;
                //double enrg = double.NaN;
                while(iter < iter_max)
                {
                    iter++;
                    Vector[] C2sUpdated = trans.GetTransformed(C2s);

                    //for(int i=0; i<size; i++)
                    System.Threading.Tasks.Parallel.For(0, size, delegate(int i)
                    {
                        for(int j=0; j<3; j++)
                        {
                            Vector planeNormal = anisou1[i].axes[j];
                            Vector planeBase   = C1[i];
                            Vector query       = C2sUpdated[i*3+j];
                            Vector closest     = query - LinAlg.DotProd(planeNormal, query-planeBase) * planeNormal;
                            HDebug.AssertTolerance(0.00001, LinAlg.DotProd(closest-planeBase, planeNormal));
                            C1s[i*3+j] = closest;
                            enrgs[i*3+j] = W1s[i*3+j] * (query-closest).Dist2;
                        }
                    });
                    Trans3 dtrans = ICP3.OptimalTransformWeighted(C2sUpdated, C1s, W1s);
                    trans = Trans3.AppendTrans(trans, dtrans);
                    double max_dtrans_matrix = (dtrans.TransformMatrix - LinAlg.Eye(4)).ToArray().HAbs().HMax();
                    if(max_dtrans_matrix < 0.0000001)
                        break;
                }

                if(optEnergy != null)
                    optEnergy.value = enrgs.Sum() / size;

                return trans;
            }

            public static void Align(IList<Vector> C1
                                    , IList<Anisou> anisou1
                                    , ref IList<Vector> C2
                                    , ref IList<Anisou> anisou2
                                    , HPack<Trans3> outTrans=null
                                    , HPack<double> optEnergy=null
                                    )
            {
                Trans3 trans = GetTrans(C1, anisou1, C2, anisou2, optEnergy: optEnergy);
                C2      = new List<Vector>(C2);
                anisou2 = new List<Anisou>(anisou2);
                
                Vector[] nC2 = trans.GetTransformed(C2).ToArray();
                for(int i=0; i<C2.Count; i++)
                {
                    Vector p2i = C2[i];
                    Anisou a2i = anisou2[i];
                    Vector np2i = trans.DoTransform(p2i);
                    Anisou na2i = a2i.Clone();
                    na2i.eigvecs[0] = (trans.DoTransform(p2i+a2i.eigvecs[0])-np2i).UnitVector();
                    na2i.eigvecs[1] = (trans.DoTransform(p2i+a2i.eigvecs[1])-np2i).UnitVector();
                    na2i.eigvecs[2] = (trans.DoTransform(p2i+a2i.eigvecs[2])-np2i).UnitVector();
                    C2[i]      = np2i;
                    anisou2[i] = na2i;
                }
                if(outTrans != null)
                    outTrans.value = trans;
            }
            public static Trans3 GetTrans(IList<Vector> C1, IList<Anisou> anisou1, IList<Vector> C2, IList<Anisou> anisou2, HPack<double> optEnergy=null)
            {
                int size = C1.Count;
                HDebug.Assert(size==C1.Count, size==C2.Count);

                Vector[] srcs = new Vector[size*3*2]; // sources
                Vector[] tars = new Vector[size*3*2]; // targets
                double[] weis = new double[size*3*2]; // weights
                double[] engs = new double[size*3*2]; // energies

                //for(int i=0; i<size; i++)
                //{
                //    Debug.Assert(anisou1[i].eigvals[0] >= 0); W1s[i*3+0] = (anisou1[i].eigvals[0] <= 0) ? 0 : 1 / anisou1[i].eigvals[0];
                //    Debug.Assert(anisou1[i].eigvals[1] >= 0); W1s[i*3+1] = (anisou1[i].eigvals[1] <= 0) ? 0 : 1 / anisou1[i].eigvals[1];
                //    Debug.Assert(anisou1[i].eigvals[2] >= 0); W1s[i*3+2] = (anisou1[i].eigvals[2] <= 0) ? 0 : 1 / anisou1[i].eigvals[2];
                //    enrgs[i*3+0] = enrgs[i*3+1] = enrgs[i*3+2] = double.NaN;
                //}

                //Trans3 trans = ICP3.OptimalTransform(C2, C1);
                Trans3 trans = new Trans3(new double[] { 0, 0, 0 }, 1, Quaternion.UnitRotation);// = transICP3.OptimalTransformWeighted(C2, C1, W1s);
                int iter = 0;
                int iter_max = 1000;
                //double enrg = double.NaN;
                while(iter < iter_max)
                {
                    iter++;

                    //for(int i=0; i<size; i++)
                    System.Threading.Tasks.Parallel.For(0, size, delegate(int i)
                    {
                        for(int j=0; j<3; j++)
                        {
                            Vector p1 = C1[i];
                            Vector n1 = anisou1[i].axes[j];
                            double w1 = anisou1[i].eigvals[j]; w1 = (w1 <= 0) ? 0 : 1/w1;

                            Vector p2 = trans.DoTransform(C2[i]);
                            Vector n2 = (trans.DoTransform(C2[i] + anisou2[i].axes[j]) - p2).UnitVector();
                            double w2 = anisou2[i].eigvals[j]; w2 = (w2 <= 0) ? 0 : 1/w2;

                            Vector clo12 = p2 - LinAlg.DotProd(n1, p2-p1) * n1; // closest point from p1 to plane2
                            Vector clo21 = p1 - LinAlg.DotProd(n2, p1-p2) * n2; // closest point from p1 to plane2
                            HDebug.AssertTolerance(0.00001, LinAlg.DotProd(clo12-p1, n1));
                            HDebug.AssertTolerance(0.00001, LinAlg.DotProd(clo21-p2, n2));

                            // p2 -> (pt closest to p2 on plane1 with w1)
                            srcs[(i*3+j)*2+0] = p2;
                            tars[(i*3+j)*2+0] = clo12;
                            weis[(i*3+j)*2+0] = w1;
                            engs[(i*3+j)*2+0] = w1 * (p2-clo12).Dist2;
                            // inverse of {p1 -> (pt closest to p1 on plane2 with w2)}
                            // = (pt closest to p1 on plane2 with w2) -> p1
                            srcs[(i*3+j)*2+1] = clo21;
                            tars[(i*3+j)*2+1] = p1;
                            weis[(i*3+j)*2+1] = w2;
                            engs[(i*3+j)*2+1] = w2 * (clo21-p1).Dist2;
                        }
                    });
                    Trans3 dtrans = ICP3.OptimalTransformWeighted(srcs, tars, weis);
                    trans = Trans3.AppendTrans(trans, dtrans);
                    double max_dtrans_matrix = (dtrans.TransformMatrix - LinAlg.Eye(4)).ToArray().HAbs().HMax();
                    if(max_dtrans_matrix < 0.0000001)
                        break;
                }

                if(optEnergy != null)
                    optEnergy.value = engs.Sum() / size;

                return trans;
            }
        }
    }
}

/*
namespace ConfAlign
{
    public partial class Program
    {
        public class MinAnisou : IMinAlign
        {
            //public static Trans3 GetTrans(IList<Vector> C1, Matrix[] anisou1, IList<Vector> C2)
            //{
            //    int size = C1.Count;
            //    Tuple<Vector[], double[]>[] eigs1 = new Tuple<Vector[], double[]>[size];
            //    {
            //        for(int i=0; i<size; i++)
            //        {
            //            Vector[] eigvec;
            //            double[] eigval;
            //            Debug.Verify(NumericSolver.Eig(anisou1[i], out eigvec, out eigval));
            //            {   // normalize eigval and eigvec
            //                double l;
            //                l = eigvec[0].Dist; eigvec[0] /= l; eigval[0] *= (l*l); eigval[0] = Math.Pow(eigval[0], 2); Debug.Assert(eigval[0] >= 0);
            //                l = eigvec[1].Dist; eigvec[1] /= l; eigval[1] *= (l*l); eigval[1] = Math.Pow(eigval[1], 2); Debug.Assert(eigval[1] >= 0);
            //                l = eigvec[2].Dist; eigvec[2] /= l; eigval[2] *= (l*l); eigval[2] = Math.Pow(eigval[2], 2); Debug.Assert(eigval[2] >= 0);
            //            }
            //            {
            //                eigval[0] = 1 / eigval[0];
            //                eigval[1] = 1 / eigval[1];
            //                eigval[2] = 1 / eigval[2];
            //            }
            //            eigs1[i] = new Tuple<Vector[], double[]>(eigvec, eigval);
            //        }
            //    }
            //    return GetTrans(C1, eigs1, C2);
            //}

            public static void GetEnergies(AlignData data1, IList<Vector> coords2
                                              , out double pot_rmsd, out double pot_enrg, out double pot_enrg_full, out double pot_enrg_anisou
                                              , Pdb pdb2 = null
                                              , string pdb2outpath = null
                                              )
            {
                Bioinfo.Anisou[] anisous1 = data1.GetAnisous();
                Trans3 trans = GetTrans(data1.coords, anisous1, coords2);
                List<Vector> coords2trans = new List<Vector>(trans.GetTransformed(coords2));
                if(pdb2 != null && pdb2outpath != null)
                    pdb2.ToFile(pdb2outpath, coords2trans);
                //data1.pdb.ToFile(pdb2outpath, coords2trans, anisous: anisous1.GetUs());

                pot_rmsd = data1.GetRmsdFrom(coords2trans);
                pot_enrg = data1.GetEnergyFromDiag(coords2trans);
                pot_enrg_full = data1.GetEnergyFromFull(coords2trans);
                pot_enrg_anisou = data1.GetEnergyFromAnisou(coords2trans);
            }

            public static void Align(IList<Vector> C1, IList<Bioinfo.Anisou> anisou1, ref List<Vector> C2, Pack<List<Vector>> optMoveC2=null)
            {
                Trans3 trans = GetTrans(C1, anisou1, C2);
                Vector[] nC2 = trans.GetTransformed(C2);
                if(optMoveC2 != null)
                {
                    optMoveC2.value = new List<Vector>(nC2.Length);
                    for(int i=0; i<nC2.Length; i++)
                        optMoveC2.value.Add(nC2[i] - C2[i]);
                }
                C2 = new List<Vector>(nC2);
            }

            public static Trans3 GetTrans(IList<Vector> C1, IList<Bioinfo.Anisou> anisou1, IList<Vector> C2)
            {
                int size = C1.Count;
                Debug.Assert(size==C1.Count, size==C2.Count);

                Vector[] C1s = C1.ToArray().Clone(3);
                Vector[] C2s = C2.ToArray().Clone(3);
                double[] W1s = new double[size*3];
                for(int i=0; i<size; i++)
                {
                    W1s[i*3+0] = 1 / anisou1[i].eigvals[0];
                    W1s[i*3+1] = 1 / anisou1[i].eigvals[1];
                    W1s[i*3+2] = 1 / anisou1[i].eigvals[2];
                }

                // double[] W1sSorted = W1s.Sort();
                // int[]    W1sIndexs = W1s.IdxSorted();
                // double thres = W1sSorted[2700];
                // for(int i=0; i<n*3; i++)
                // {
                //     if(W1s[i] < thres)
                //         W1s[i] = 0;
                // }

                //Trans3 trans = ICP3.OptimalTransform(C2, C1);
                Trans3 trans = new Trans3(new double[] { 0, 0, 0 }, 1, Quaternion.UnitRotation);// = transICP3.OptimalTransformWeighted(C2, C1, W1s);
                int iter = 0;
                int iter_max = 1000;
                while(iter < iter_max)
                {
                    iter++;
                    Vector[] C2sUpdated = trans.GetTransformed(C2s);

                    //for(int i=0; i<size; i++)
                    Parallel.For(0, size, delegate(int i)
                    {
                        for(int j=0; j<3; j++)
                        {
                            Vector planeNormal = anisou1[i].axes[j];
                            Vector planeBase   = C1[i];
                            Vector query       = C2sUpdated[i*3+j];
                            Vector closest     = query - Vector.DotProd(planeNormal, query-planeBase) * planeNormal;
                            Debug.AssertTolerance(0.00001, Vector.DotProd(closest-planeBase, planeNormal));
                            C1s[i*3+j] = closest;
                        }
                    });
                    Trans3 dtrans = ICP3.OptimalTransformWeighted(C2sUpdated, C1s, W1s);
                    trans = Trans3.AppendTrans(trans, dtrans);
                    double max_dtrans_matrix = (dtrans.TransformMatrix - LinAlg.Eye(4)).ToArray().Abs().Max();
                    if(max_dtrans_matrix < 0.0000001)
                        break;
                }

                return trans;
            }
        }
    }
}
*/