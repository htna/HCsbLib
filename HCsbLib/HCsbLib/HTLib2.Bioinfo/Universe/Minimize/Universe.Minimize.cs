using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public void Minimize(List<ForceField.IForceField> frcflds
                            , string   logpath           = ""
                            , int      iterInitial       = 0
                            , double?  k                 = null
                            , double   max_atom_movement = 0.1
                            , int?     max_iteration     = null
                            , double   threshold         = 0.001
                            , int      randomPurturb     = 0
                            , bool[]   atomsMovable      = null
                            , InfoPack extra             = null
                            )
        {
            Minimize_ConjugateGradient(frcflds
                                      ,logpath           : logpath          
                                      ,iterInitial       : iterInitial      
                                      ,k                 : k                
                                      ,max_atom_movement : max_atom_movement
                                      ,max_iteration     : max_iteration
                                      ,threshold         : threshold        
                                      ,randomPurturb     : randomPurturb    
                                      ,atomsMovable      : atomsMovable     
                                      ,extra             : extra            
                                      );
        }
        public void Minimize_Parallel(List<ForceField.IForceField> frcflds
                            , string logpath           = ""
                            , int iterInitial       = 0
                            , double? k                 = null
                            , double max_atom_movement = 0.1
                            , int?     max_iteration     = null
                            , double threshold         = 0.001
                            , int randomPurturb     = 0
                            , bool[] atomsMovable      = null
                            , InfoPack extra             = null
                            )
        {
            Minimize_ConjugateGradient_Parallel(frcflds,
                                                k: k,
                                                max_atom_movement: max_atom_movement,
                                                max_iteration: max_iteration,
                                                threshold: threshold,
                                                randomPurturb: randomPurturb,
                                                atomsMovable: atomsMovable,
                                                logger: new MinimizeLogger_PrintEnergyForceMag(logpath),
                                                extra: extra,
                                                doSteepDeescent: null
                                                );
        }
    }
}
