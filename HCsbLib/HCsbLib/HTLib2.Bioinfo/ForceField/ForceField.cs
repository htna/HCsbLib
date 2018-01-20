using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class ForceField
    {
        public interface IForceField
        {
            string[] FrcFldType { get; }
            double? GetDefaultMinimizeStep(); // { return 0.0001; }
            void EnvClear();
            bool EnvAdd(string key, object value); // return true if info is applied, false otherwise
        };
        public interface IBond      : IForceField { void Compute(Universe.Bond        bond     , Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null); }
        public interface IAngle     : IForceField { void Compute(Universe.Angle       angle    , Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null); }
        public interface IDihedral  : IForceField { void Compute(Universe.Dihedral    dihedral , Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null); }
        public interface IImproper  : IForceField { void Compute(Universe.Improper    improper , Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null); }
        public interface INonbonded : IForceField { void Compute(Universe.Nonbonded   nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null);
                                                    void Compute(Universe.Nonbonded14 nonbonded, Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null);
                                                  }
        public interface ICustom    : IForceField { void Compute(Universe.Atoms      atoms     , Vector[] coords, ref double energy, ref Vector[] forces, ref MatrixByArr[,] hessian, double[,] pwfrc=null, double[,] pwspr=null); }


        public static List<IForceField> GetMindyForceFields()
        {
            return _GetMindyForceFields(true);
        }
        public static List<IForceField> GetMindyForceFields(bool divideRadijByTwo)
        {
            HDebug.Assert(divideRadijByTwo == false);
            return _GetMindyForceFields(false);
        }
        public static List<IForceField> GetRigidbodyForceFields()
        {
            List<IForceField> frcflds = new List<IForceField>();
            frcflds.Add(new Rigidbody());
            frcflds.Add(new MindyNonbondedLennardJones(false));
            frcflds.Add(new MindyNonbondedElectrostatic());
            return frcflds;
        }
        public static List<IForceField> _GetMindyForceFields(bool divideRadijByTwo)
        {
            List<IForceField> frcflds = new List<IForceField>();
            frcflds.Add(new MindyBond                  ());
            frcflds.Add(new MindyAngle                 ());
            frcflds.Add(new MindyDihedral              ());
            frcflds.Add(new MindyImproper              ());
            if(divideRadijByTwo)    frcflds.Add(new MindyNonbondedLennardJones());
            else                    frcflds.Add(new MindyNonbondedLennardJones(divideRadijByTwo));
            frcflds.Add(new MindyNonbondedElectrostatic());
            //frcflds.Add(new NonbondedLennardJones ());
            //frcflds.Add(new NonbondedElectrostatic());
            return frcflds;
        }
    }
}
