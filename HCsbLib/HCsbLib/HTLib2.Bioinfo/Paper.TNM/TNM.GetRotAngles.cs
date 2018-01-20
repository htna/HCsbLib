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
        public static double[] GetRotAngles(  Universe univ
                                            , Vector[] coords
                                            , MatrixByArr hessian
                                            , Vector[] forces
                                            , MatrixByArr J = null
                                            , Graph<Universe.Atom[], Universe.Bond> univ_flexgraph = null
                                            , List<Universe.RotableInfo> univ_rotinfos = null
                                            , Vector[] forceProjectedByTorsional = null
                                            , HPack<Vector> optEigvalOfTorHessian = null
                                            )
        {
            Vector mass = univ.GetMasses();
            //Vector[] dcoords = new Vector[univ.size];
            //double t2 = t*t;
            //for(int i=0; i<univ.size; i++)
            //    dcoords[i] = forces[i] * (0.5*t2/mass[i]);

            if(J == null)
            {
                if(univ_rotinfos  == null)
                {
                    if(univ_flexgraph == null) univ_flexgraph = univ.BuildFlexibilityGraph();
                    univ_rotinfos  = univ.GetRotableInfo(univ_flexgraph);
                }
                J = TNM.GetJ(univ, coords, univ_rotinfos);
            }

            double[] dangles;
            using(new Matlab.NamedLock("TEST"))
            {
                Matlab.Clear("TEST");
                Matlab.PutVector("TEST.F", Vector.FromBlockvector(forces));
                Matlab.PutMatrix("TEST.J", J);
                Matlab.PutMatrix("TEST.H", hessian);
                Matlab.PutVector("TEST.M", univ.GetMasses(3));
                Matlab.Execute("TEST.M = diag(TEST.M);");

                Matlab.Execute("TEST.JHJ = TEST.J' * TEST.H * TEST.J;");
                Matlab.Execute("TEST.JMJ = TEST.J' * TEST.M * TEST.J;");
                // (J' H J) tor = J' F
                // (V' D V) tor = J' F  <= (V,D) are (eigvec,eigval) of generalized eigenvalue problem with (A = JHJ, B = JMJ)
                // tor = inv(V' D V) J' F
                Matlab.Execute("[TEST.V, TEST.D] = eig(TEST.JHJ, TEST.JMJ);");
                if(optEigvalOfTorHessian != null) optEigvalOfTorHessian.value = Matlab.GetVector("diag(TEST.D)");
                {
                    Matlab.Execute("TEST.D = diag(TEST.D);");
                    Matlab.Execute("TEST.D(abs(TEST.D)<1) = 0;"); // remove "eigenvalue < 1" because they will increase
                                                                  // the magnitude of force term too big !!!
                    Matlab.Execute("TEST.D = diag(TEST.D);");
                }
                Matlab.Execute("TEST.invJHJ  = TEST.V * pinv(TEST.D) * TEST.V';");
                Matlab.Execute("TEST.dtor  = TEST.invJHJ * TEST.J' * TEST.F;");
                /// f = m a
                /// d = 1/2 a t^2
                ///   = 0.5 a        : assuming t=1
                ///   = 0.5 f/m
                /// f = m a
                ///   = 2 m d t^-2
                ///   = 2 m d        : assuming t=1
                /// 
                /// coord change
                /// dr = 0.5 a t^2
                ///    = 0.5 f/m     : assuming t=1
                ///    = 0.5 M^-1 F  : M is mass matrix, F is the net force of each atom
                /// 
                /// torsional angle change
                /// dtor = (J' M J)^-1 J' M * dr                  : (6) of TNM paper
                ///      = (J' M J)^-1 J' M * 0.5 M^-1 F
                ///      = 0.5 (J' M J)^-1 J' F
                ///
                /// force filtered by torsional ...
                /// F_tor = ma
                ///       = 2 M (J dtor)
                ///       = 2 M J 0.5 (J' M J)^-1 J' F
                ///       = M J (J' M J)^-1 J' F
                ///
                /// H J dtor = F
                ///          = F_tor                            : update force as the torsional filtered force
                ///          = M J (J' M J)^-1 J' F
                /// (J' H J) dtor = (J' M J) (J' M J)^-1 J' F
                /// (V D V') dtor = (J' M J) (J' M J)^-1 J' F               : eigen decomposition of (J' H J) using
                ///                                                           generalized eigenvalue problem with (J' M J)
                ///          dtor = (V D^-1 V') (J' M J) (J' M J)^-1 J' F   : (J' M J) (J' M J)^-1 = I. However, it has
                ///                                                           the projection effect of J'F into (J' M J)
                ///                                                           vector space(?). The projection will be taken
                ///                                                           care by (V D^-1 V')
                ///               = (V D^-1 V') J' F
                /// 
                dangles = Matlab.GetVector("TEST.dtor");
                if(forceProjectedByTorsional != null)
                {
                    HDebug.Assert(forceProjectedByTorsional.Length == forces.Length);
                    Matlab.Execute("TEST.F_tor = TEST.M * TEST.J * pinv(TEST.JMJ) * TEST.J' * TEST.F;");
                    Vector lforceProjectedByTorsional = Matlab.GetVector("TEST.F_tor");
                    HDebug.Assert(lforceProjectedByTorsional.Size == forceProjectedByTorsional.Length*3);
                    for(int i=0; i<forceProjectedByTorsional.Length; i++)
                    {
                        int i3 = i*3;
                        forceProjectedByTorsional[i] = new double[] { lforceProjectedByTorsional[i3+0],
                                                                      lforceProjectedByTorsional[i3+1],
                                                                      lforceProjectedByTorsional[i3+2],
                                                                    };
                    }
                }
                Matlab.Clear("TEST");
            }

            return dangles;
        }
        public static double[] GetRotAngles(Universe univ
                                            , Vector[] coords
                                            , Vector[] forces
                                            , double t // 0.1
                                            , MatrixByArr J = null
                                            , Graph<Universe.Atom[], Universe.Bond> univ_flexgraph = null
                                            , List<Universe.RotableInfo> univ_rotinfos = null
                                            , HPack<Vector[]> forcesProjectedByTorsional = null
                                            , HPack<Vector[]> dcoordsProjectedByTorsional = null
                                            )
        {
            Vector mass = univ.GetMasses();
            //Vector[] dcoords = new Vector[univ.size];
            double t2 = t*t;
            //for(int i=0; i<univ.size; i++)
            //    dcoords[i] = forces[i] * (0.5*t2/mass[i]);

            if(J == null)
            {
                if(univ_rotinfos  == null)
                {
                    if(univ_flexgraph == null) univ_flexgraph = univ.BuildFlexibilityGraph();
                    univ_rotinfos  = univ.GetRotableInfo(univ_flexgraph);
                }
                J = TNM.GetJ(univ, coords, univ_rotinfos);
            }

            double[] dangles;
            using(new Matlab.NamedLock("TEST"))
            {
                Matlab.Clear("TEST");
                Matlab.PutVector("TEST.F", Vector.FromBlockvector(forces));
                Matlab.PutValue ("TEST.t2", t2);
                //Matlab.PutVector("TEST.R", Vector.FromBlockvector(dcoords));
                Matlab.PutMatrix("TEST.J", J);
                Matlab.PutVector("TEST.M", univ.GetMasses(3));
                Matlab.Execute("TEST.M = diag(TEST.M);");
                /// f = m a
                /// d = 1/2 a t^2
                ///   = 0.5 f/m t^2
                /// f = m a
                ///   = 2 m d t^-2
                /// 
                /// coord change
                /// dcoord = 0.5 a t^2
                ///        = (0.5 t^2) f/m
                ///        = (0.5 t^2) M^-1 F  : M is mass matrix, F is the net force of each atom
                /// 
                /// torsional angle change
                /// dtor =           (J' M J)^-1 J' M * dcoord                  : (6) of TNM paper
                ///      =           (J' M J)^-1 J' M * (0.5 t^2) M^-1 F
                ///      = (0.5 t^2) (J' M J)^-1 J'                    F
                ///      = (0.5 t^2) (J' M J)^-1 J' F
                ///      = (0.5 t2)  invJMJ      JF
                ///
                /// force filtered by torsional ...
                /// F_tor = m a
                ///       = 2 m d t^-2
                ///       = 2 M (J * dtor) t^-2
                ///       = 2 M (J * (0.5 t^2) (J' M J)^-1 J' F) t^-2
                ///       = M J (J' M J)^-1 J' F
                ///       = MJ  invJMJ      JF
                ///
                /// coord change filtered by torsional
                /// R_tor = (0.5 t^2) M^-1 * F_tor
                ///       = (0.5 t^2) J (J' M J)^-1 J' F
                ///       = (0.5 t2)  J invJMJ      JF
                Matlab.Execute("TEST.JMJ    = TEST.J' * TEST.M * TEST.J;");
                Matlab.Execute("TEST.invJMJ = inv(TEST.JMJ);");
                Matlab.Execute("TEST.MJ     = TEST.M * TEST.J;");
                Matlab.Execute("TEST.JF     = TEST.J' * TEST.F;");
                Matlab.Execute("TEST.dtor   = (0.5 * TEST.t2) * TEST.invJMJ * TEST.JF;");  // (6) of TNM paper
                Matlab.Execute("TEST.F_tor  = TEST.MJ * TEST.invJMJ * TEST.JF;");
                Matlab.Execute("TEST.R_tor  = (0.5 * TEST.t2) * TEST.J * TEST.invJMJ * TEST.JF;");

                dangles = Matlab.GetVector("TEST.dtor");
                if(forcesProjectedByTorsional != null)
                {
                    Vector F_tor = Matlab.GetVector("TEST.F_tor");
                    HDebug.Assert(F_tor.Size == forces.Length*3);
                    forcesProjectedByTorsional.value = new Vector[forces.Length];
                    for(int i=0; i<forces.Length; i++)
                    {
                        int i3 = i*3;
                        forcesProjectedByTorsional.value[i] = new double[] { F_tor[i3+0], F_tor[i3+1], F_tor[i3+2] };
                    }
                }
                if(dcoordsProjectedByTorsional != null)
                {
                    Vector R_tor = Matlab.GetVector("TEST.R_tor");
                    HDebug.Assert(R_tor.Size == coords.Length*3);
                    dcoordsProjectedByTorsional.value = new Vector[coords.Length];
                    for(int i=0; i<coords.Length; i++)
                    {
                        int i3 = i*3;
                        dcoordsProjectedByTorsional.value[i] = new double[] { R_tor[i3+0], R_tor[i3+1], R_tor[i3+2] };
                    }
                }
                Matlab.Clear("TEST");
            }

            return dangles;
        }
        public static double[] GetRotAngles(  Universe univ
                                            , Vector[] coords
                                            , Vector[] dcoords
                                            , MatrixByArr J = null
                                            , Graph<Universe.Atom[], Universe.Bond> univ_flexgraph = null
                                            , List<Universe.RotableInfo> univ_rotinfos = null
                                            , Vector[] dcoordsRotated = null
                                            )
        {
            if(J == null)
            {
                if(univ_rotinfos  == null)
                {
                    if(univ_flexgraph == null) univ_flexgraph = univ.BuildFlexibilityGraph();
                    univ_rotinfos  = univ.GetRotableInfo(univ_flexgraph);
                }
                J = TNM.GetJ(univ, coords, univ_rotinfos);
            }

            double[] dangles;
            using(new Matlab.NamedLock("TEST"))
            {
                Matlab.Clear("TEST");
                Matlab.PutVector("TEST.R", Vector.FromBlockvector(dcoords));
                Matlab.PutMatrix("TEST.J", J.ToArray(), true);
                Matlab.PutVector("TEST.M", univ.GetMasses(3));
                Matlab.Execute("TEST.M = diag(TEST.M);");
                Matlab.Execute("TEST.invJMJ = inv(TEST.J' * TEST.M * TEST.J);");
                Matlab.Execute("TEST.A = TEST.invJMJ * TEST.J' * TEST.M * TEST.R;");  // (6) of TNM paper
                dangles = Matlab.GetVector("TEST.A");
                if(dcoordsRotated != null)
                {
                    HDebug.Assert(dcoordsRotated.Length == dcoords.Length);
                    Matlab.Execute("TEST.dR = TEST.J * TEST.A;");
                    Vector ldcoordsRotated = Matlab.GetVector("TEST.dR");
                    HDebug.Assert(ldcoordsRotated.Size == dcoordsRotated.Length*3);
                    for(int i=0; i<dcoordsRotated.Length; i++)
                    {
                        int i3 = i*3;
                        dcoordsRotated[i] = new double[] { ldcoordsRotated[i3+0], ldcoordsRotated[i3+1], ldcoordsRotated[i3+2] };
                    }
                }
                Matlab.Clear("TEST");
            }

            return dangles;
        }
        public static double[] GetRotAngles( Universe univ
                                           , Vector[] coords
                                           , Vector[] dcoords
                                           , Func<Vector, MatrixByArr>   Diag
                                           , Func<MatrixByArr, MatrixByArr>   InvSymm
                                           , Func<MatrixByArr, MatrixByArr, MatrixByArr, MatrixByArr> Mul
                                           , MatrixByArr J = null
                                           , Graph<Universe.Atom[], Universe.Bond> univ_flexgraph = null
                                           , List<Universe.RotableInfo> univ_rotinfos = null
                                           , Vector[] dcoordsRotated = null
                                           )
        {
            if(J == null)
            {
                if(univ_rotinfos  == null)
                {
                    if(univ_flexgraph == null) univ_flexgraph = univ.BuildFlexibilityGraph();
                    univ_rotinfos  = univ.GetRotableInfo(univ_flexgraph);
                }
                J = TNM.GetJ(univ, coords, univ_rotinfos);
            }

            Vector dangles;
            {
                Vector R = Vector.FromBlockvector(dcoords);
                MatrixByArr M = Diag(univ.GetMasses(3));
                MatrixByArr Jt = J.Tr();
                MatrixByArr invJMJ = InvSymm(Mul(Jt, M, J));
                Vector A = LinAlg.MV(Mul(invJMJ, Jt, M), R);  // (6) of TNM paper
                dangles = A;
                if(dcoordsRotated != null)
                {
                    HDebug.Assert(dcoordsRotated.Length == dcoords.Length);
                    Vector dR = LinAlg.MV(J, A);
                    Vector ldcoordsRotated = dR;
                    HDebug.Assert(ldcoordsRotated.Size == dcoordsRotated.Length*3);
                    for(int i=0; i<dcoordsRotated.Length; i++)
                    {
                        int i3 = i*3;
                        dcoordsRotated[i] = new double[] { ldcoordsRotated[i3+0], ldcoordsRotated[i3+1], ldcoordsRotated[i3+2] };
                    }
                }
            }

            if(HDebug.IsDebuggerAttached)
            {
                Vector tdangles = GetRotAngles(univ, coords, dcoords, J, univ_flexgraph, univ_rotinfos);

                HDebug.Assert(0.9999 < (tdangles.Dist/dangles.Dist), (tdangles.Dist/dangles.Dist) < 1.0001);
                HDebug.Assert(LinAlg.DotProd(tdangles, dangles)/(tdangles.Dist*dangles.Dist) > 0.9999);
                HDebug.Assert((tdangles-dangles).Dist / tdangles.Dist < 0.0001);
            }

            return dangles;
        }
        public static double[] GetRotAngles( Universe univ
                                           , Vector[] coords
                                           , Vector[] dcoords
                                           , MatrixByArr J
                                           , ILinAlg ila
                                           )
        {
            Vector dangles;
            using(ila.NewDisposables())
            {
                Vector R = Vector.FromBlockvector(dcoords);
                Vector M = univ.GetMasses(3);
                var RR = ila.ToILMat(R).AddDisposable();
                var MM = ila.ToILMat(M).Diag().AddDisposable();
                var JJ = ila.ToILMat(J).AddDisposable();
                var invJMJ = ila.Inv(JJ.Tr * MM * JJ).AddDisposable();
                var AA = invJMJ * JJ.Tr * MM * RR;
                dangles = AA.ToArray().HToArray1D();
            }

            if(HDebug.False && HDebug.IsDebuggerAttached)
            {
                Vector tdangles = GetRotAngles(univ, coords, dcoords, J);

                HDebug.Assert(0.9999 < (tdangles.Dist/dangles.Dist), (tdangles.Dist/dangles.Dist) < 1.0001);
                HDebug.Assert(LinAlg.DotProd(tdangles, dangles)/(tdangles.Dist*dangles.Dist) > 0.9999);
                HDebug.Assert((tdangles-dangles).Dist / tdangles.Dist < 0.0001);
            }

            return dangles;
        }
    }
}
}
