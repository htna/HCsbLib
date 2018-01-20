using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public int MinimizeHydrogens(List<ForceField.IForceField> frcflds
                                    , string logpath = ""
                                    , InfoPack extra = null
                                    )
        {
            bool[] atomsMovable = new bool[size];
            for(int i=0; i<size; i++)
                atomsMovable[i] = atoms[i].IsHydrogen();

            double k = 0.001;
            double threshold = 0.001;
            double max_atom_movement = 0.1;
            IMinimizeLogger logger = new MinimizeLogger_PrintEnergyForceMag(logpath); // null
            int randomPurturb = 0; // no random purturbation

            return Minimize_ConjugateGradient_v1(0, frcflds, k, max_atom_movement, null, threshold, randomPurturb, atomsMovable, logger, extra, null);
        }
        public int MinimizeHydrogens(List<ForceField.IForceField> frcflds, double? k, double threshold, InfoPack extra)
        {
            bool[] atomsMovable = new bool[size];
            for(int i=0; i<size; i++)
                atomsMovable[i] = atoms[i].IsHydrogen();

            IMinimizeLogger logger = null;
            double max_atom_movement = 0.1;
            int randomPurturb = 0; // no random purturbation

            return Minimize_ConjugateGradient_v1(0, frcflds, k, max_atom_movement, null, threshold, randomPurturb, atomsMovable, logger, extra, null);
        }
        public int MinimizeHydrogens(List<ForceField.IForceField> frcflds, double? k, double threshold, int randomPurturb, InfoPack extra)
        {
            bool[] atomsMovable = new bool[size];
            for(int i=0; i<size; i++)
                atomsMovable[i] = atoms[i].IsHydrogen();

            IMinimizeLogger logger = null;
            double max_atom_movement = 0.1;

            return Minimize_ConjugateGradient_v1(0, frcflds, k, max_atom_movement, null, threshold, randomPurturb, atomsMovable, logger, extra, null);
        }
    }
}
