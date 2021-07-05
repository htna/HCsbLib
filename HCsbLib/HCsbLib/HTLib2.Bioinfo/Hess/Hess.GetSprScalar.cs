using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static double GetSprScalar123(Vector[] coords, HessMatrix hess, int atm1, int atm2, int atm3, IList<int> atms, ILinAlg ila)
        {
            throw new NotImplementedException();
        }
        public static double GetSprScalar(Vector[] coords, HessMatrix hess, int atm1, int atm2, IList<int> atms, ILinAlg ila)
        {
            HDebug.Assert(atms.Count == atms.HUnion().Length);

            int[] idxs = atms.HUnion();
            idxs = idxs.HRemoveAll(atm1);
            idxs = idxs.HRemoveAll(atm2);
            idxs = idxs.HInsert(0, atm2);
            idxs = idxs.HInsert(0, atm1);
            HDebug.Assert(idxs.HExcept(atms).HExcept(atm1, atm2).Length == 0);
            HDebug.Assert(idxs[0] == atm1);
            HDebug.Assert(idxs[1] == atm2);

            HessMatrix H = hess.SubMatrixByAtoms(idxs);
            H = Hess.GetHessFixDiag(H);
            HDebug.Assert(H.ColSize == H.RowSize, H.RowSize == idxs.Length*3);
            // H should have 6 zero eigenvalues !!
            // since it is the original hessian matrix.

            // return "0" spring if there is no connect between atm1 and atm2
            {
                Graph<int, object> g = new Graph<int, object>();
                for(int i=0; i<H.ColBlockSize; i++)
                    g.AddNode(i);
                foreach(var bc_br_bval in H.EnumBlocks())
                {
                    int bc = bc_br_bval.Item1;
                    int br = bc_br_bval.Item2;
                    if(bc > br) continue;
                    g.AddEdge(bc, br, new object());
                    //g.AddEdge(br, bc, new object()); // g is bi-directional graph
                }
                var paths = g.FindPathShortest(g.GetNode(0), new Graph<int,object>.Node[] { g.GetNode(1) } );
                if(paths == null)
                    // if there is no path between 0 <-> 1, then there is no spring
                    return 0;
            }

            Vector u12 = (coords[atm2] - coords[atm1]).UnitVector();
            Matrix S12 = new double[idxs.Length*3,1];
            S12[0,0] =  u12[0];    S12[1,0] =  u12[1];    S12[2,0] =  u12[2];
            S12[3,0] = -u12[0];    S12[4,0] = -u12[1];    S12[5,0] = -u12[2];

            Matrix invkij = GetInvSprTensorSymm(H.ToMatrix(), S12, ila);
            HDebug.Assert(invkij.ColSize == 1, invkij.RowSize == 1);
            HDebug.Assert(invkij[0, 0] > 0);

            double kij = 1/invkij[0, 0];
            return kij;
        }
        public static double GetSprTensor(Vector[] coords, HessMatrix hess, int atm1, int atm2, IList<int> atms, ILinAlg ila)
        {
            HDebug.Assert(atms.Count == atms.HUnion().Length);

            int[] idxs = atms.HUnion();
            idxs = idxs.HRemoveAll(atm1);
            idxs = idxs.HRemoveAll(atm2);
            idxs = idxs.HInsert(0, atm2);
            idxs = idxs.HInsert(0, atm1);
            HDebug.Assert(idxs.HExcept(atms).HExcept(atm1, atm2).Length == 0);
            HDebug.Assert(idxs[0] == atm1);
            HDebug.Assert(idxs[1] == atm2);

            HessMatrix H = hess.SubMatrixByAtoms(idxs);
            H = Hess.GetHessFixDiag(H);
            HDebug.Assert(H.ColSize == H.RowSize, H.RowSize == idxs.Length*3);
            // H should have 6 zero eigenvalues !!
            // since it is the original hessian matrix.

            Vector u12 = (coords[atm2] - coords[atm1]).UnitVector();
            Matrix S12 = Matrix.Zeros(idxs.Length*3,6);
            for(int i=0; i<6; i++)
                S12[i, i] = 1;

            Matrix invkij = GetInvSprTensorSymm(H.ToMatrix(), S12, ila);
            HDebug.Assert(invkij.ColSize == 1, invkij.RowSize == 1);
            HDebug.Assert(invkij[0, 0] > 0);

            double kij = 1/invkij[0, 0];
            return kij;
        }
        public static Matrix GetInvSprTensorSymm(Matrix H, Matrix S, ILinAlg ila)
        {
            // check H be symmetric
            HDebug.AssertToleranceMatrix(0.00000001, H - H.Tr());

            Matrix invK = GetInvSprTensor(H, S, ila);

            if(HDebug.IsDebuggerAttached && ila != null)
            {
                HDebug.AssertToleranceMatrix(0.00000001, invK - invK.Tr()); // check symmetric
                var invKK = ila.ToILMat(invK);
                var VVDD = ila.EigSymm(invKK);
                foreach(double d in VVDD.Item2)
                    HDebug.Assert(d > -0.00000001);
            }
            return invK;
        }
        public static Matrix GetInvSprTensor(Matrix H, Matrix S, ILinAlg ila)
        {
            Matrix  invH;
            string optInvH = "EigSymmTol";
            optInvH += ((ila == null) ? "-matlab" : "-ilnum");
            switch(optInvH)
            {
                case "InvSymm-ilnum":
                    HDebug.Assert(false);
                    invH = ila.InvSymm(H);
                    break;
                case "PInv-ilnum":
                    invH = ila.PInv(H);
                    break;
                case "EigSymm-ilnum":
                    {
                        var HH   = ila.ToILMat(H);
                        var VVDD = ila.EigSymm(HH);
                        var VV = VVDD.Item1;
                        for(int i=0; i<VVDD.Item2.Length; i++) VVDD.Item2[i] = 1 / VVDD.Item2[i];
                        for(int i=0; i<6                ; i++) VVDD.Item2[i] = 0;
                        var DD = ila.ToILMat(VVDD.Item2).Diag();
                        var invHH = ila.Mul(VV, DD, VV.Tr);
                        invH = invHH.ToArray();
                        //var check = (H * invH).ToArray();
                        GC.Collect();
                    }
                    break;
                case "EigSymmTol-matlab":
                    {
                        using(new Matlab.NamedLock(""))
                        {
                            Matlab.PutMatrix("invHH.HH", H);
                            Matlab.Execute("invHH.HH = (invHH.HH + invHH.HH')/2;");
                            Matlab.Execute("[invHH.VV, invHH.DD] = eig(invHH.HH);");
                            Matlab.Execute("invHH.DD = diag(invHH.DD);");
                            Matlab.Execute("invHH.DD(abs(invHH.DD)<0.00001) = 0;");
                            Matlab.Execute("invHH.DD = pinv(diag(invHH.DD));");
                            Matlab.Execute("invHH = invHH.VV * invHH.DD * invHH.VV';");
                            invH = Matlab.GetMatrix("invHH");
                            Matlab.Execute("clear invHH;");
                        }
                        GC.Collect();
                    }
                    break;
                case "EigSymmTol-ilnum":
                    {
                        var HH   = ila.ToILMat(H);
                        var VVDD = ila.EigSymm(HH);
                        var VV = VVDD.Item1;
                        for(int i=0; i<VVDD.Item2.Length; i++)
                        {
                            if(Math.Abs(VVDD.Item2[i]) < 0.00001)
                                VVDD.Item2[i] = 0;
                            else
                                VVDD.Item2[i] = 1 / VVDD.Item2[i];
                        }
                        var DD = ila.ToILMat(VVDD.Item2).Diag();
                        var invHH = ila.Mul(VV, DD, VV.Tr);
                        invH = invHH.ToArray();
                        //var check = (H * invH).ToArray();
                        GC.Collect();
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }

            Matrix invkij = 0.5 * (S.Tr() * invH * S);
            //HDebug.Assert(invkij >= 0);

            return invkij;
        }
    }
}
