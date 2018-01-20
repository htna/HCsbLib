using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Universe
    {
        public partial class MinimizeTNMImpl
        {
            private void GetPotentialAndTorForce( List<ForceField.IForceField> frcflds, Vector[] coords
                                                , out double energy, out Vector[] forces, out MatrixByArr hessian, out double[] dtor
                                                , Dictionary<string, object> cache)
            {
                MatrixByArr J;
                {
                    Graph<Universe.Atom[], Universe.Bond> univ_flexgraph = univ.BuildFlexibilityGraph();
                    List<Universe.RotableInfo> univ_rotinfos  = univ.GetRotableInfo(univ_flexgraph);
                    J = Paper.TNM.GetJ(univ, coords, univ_rotinfos);
                }

                Vector[] forces0 = univ.GetVectorsZero();
                hessian = new double[size*3, size*3];
                energy = univ.GetPotential(frcflds, coords, ref forces0, ref hessian, cache);
                forces = univ.GetVectorsZero();
                dtor = Paper.TNM.GetRotAngles(univ, coords, hessian, forces0, J: J, forceProjectedByTorsional: forces);
            }
            private void GetPotentialAndTorForce( List<ForceField.IForceField> frcflds, Vector[] coords
                                                , out double energy, out Vector[] forces
                                                , Dictionary<string, object> cache)
            {
                MatrixByArr J;
                {
                    Graph<Universe.Atom[], Universe.Bond> univ_flexgraph = univ.BuildFlexibilityGraph();
                    List<Universe.RotableInfo> univ_rotinfos  = univ.GetRotableInfo(univ_flexgraph);
                    J = Paper.TNM.GetJ(univ, coords, univ_rotinfos);
                }

                Vector[] forces0 = univ.GetVectorsZero();
                MatrixByArr hessian = null;
                energy = univ.GetPotential(frcflds, coords, ref forces0, ref hessian, cache);
                HPack<Vector[]> lforces = new HPack<Vector[]>();
                double[] dtor = Paper.TNM.GetRotAngles(univ, coords, forces0, 1, J: J, forcesProjectedByTorsional: lforces);
                forces = lforces.value;
            }
            private void GetPotentialAndTorForce(List<ForceField.IForceField> frcflds, Vector[] coords
                                                , out double energy, out Vector[] forces, out MatrixByArr hessian
                                                , Dictionary<string, object> cache)
            {
                double[] dtor;
                GetPotentialAndTorForce(frcflds, coords, out energy, out forces, out hessian, out dtor, cache);
            }
        }
    }
}
