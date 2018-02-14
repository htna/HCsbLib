using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class PdbTools
    {
        public static IEnumerable<IList<Pdb.IAtom>> SoakMoleculeISolventBox(IList<Pdb.IAtom> mole, IList<IList<Pdb.IAtom>> solv_atoms, Namd.Prm prm)
        {
            yield return mole;

            KDTree.KDTree<Pdb.IAtom> kdtree_mole = new KDTree.KDTree<Pdb.IAtom>(3);
            foreach(var atom in mole)
                kdtree_mole.insert(atom.coord, atom);

            //List<Pdb.IAtom> socked = new List<Pdb.IAtom>();
            //socked.AddRange(mole);

            Dictionary<string, double> elem_radius = new Dictionary<string, double>
            {
                #region {"C", 1.90}, ...
                //  public static NbndInfo[] nbndinfo_set_SSTeMcs6c = new NbndInfo[]
                //  {
                {"C", 1.90},    //      new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="C", rmin2=1.90, charge=0.0, epsilon=-0.1},
                {"H", 1.20},    //      new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="H", rmin2=1.20, charge=0.0, epsilon=-0.1},
                {"O", 1.70},    //      new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="O", rmin2=1.70, charge=0.0, epsilon=-0.1},
                {"N", 1.85},    //      new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="N", rmin2=1.85, charge=0.0, epsilon=-0.1},
                {"S", 2.00},    //      new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="S", rmin2=2.00, charge=0.0, epsilon=-0.1},
                {"F", 1.47},    //      new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="F", rmin2=1.47, charge=0.0, epsilon=-0.1}, /// same to unif0
                {"P", 1.80},    //      new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="P", rmin2=1.80, charge=0.0, epsilon=-0.1}, /// same to unif0
                                //      new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="C", rmin2=1.90, charge=0.0, epsilon=-0.1},
                                //      new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="H", rmin2=1.20, charge=0.0, epsilon=-0.1},
                                //      new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="O", rmin2=1.40, charge=0.0, epsilon=-0.1}, /// special treatment: 1.70 -> 1.40
                                //      new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="N", rmin2=1.55, charge=0.0, epsilon=-0.1}, /// special treatment: 1.85 -> 1.55
                                //      new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="S", rmin2=2.00, charge=0.0, epsilon=-0.1},
                                //      new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="F", rmin2=1.47, charge=0.0, epsilon=-0.1}, /// same to unif0
                                //      new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="P", rmin2=1.80, charge=0.0, epsilon=-0.1}, /// same to unif0
                                //      ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                //      //new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="", rmin2=0.00, charge=0.0, epsilon=-0.1},
                                //      // http://en.wikipedia.org/wiki/Van_der_Waals_radius
                {"CL", 1.75},   //      new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="CL", rmin2=1.75, charge=0.0, epsilon=-0.1},
                {"CU", 1.40},   //      new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="CU", rmin2=1.40, charge=0.0, epsilon=-0.1},
                                //      // http://en.wikipedia.org/wiki/Atomic_radii_of_the_elements_(data_page)
                {"MG" , 1.73},  //      new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="MG", rmin2=1.73, charge=0.0, epsilon=-0.1}, // magnesium
                {"K"  , 2.75},  //      new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="K" , rmin2=2.75, charge=0.0, epsilon=-0.1}, // potassium
                {"POT", 2.75},  //      new NbndInfo{ nbndtype=NbndType.nbnd, atomelem="POT",rmin2=2.75, charge=0.0, epsilon=-0.1}, // potassium
                                //      ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                                //      // par_all27_prot_na.prm
                                //      // FE     0.010000   0.000000     0.650000 ! ALLOW HEM
                                //      new NbndInfo{ nbndtype=NbndType.nbnd  , atomelem="FE" ,rmin2=0.65, charge=0.0, epsilon=-0.1},
                                //      new NbndInfo{ nbndtype=NbndType.nbnd14, atomelem="FE" ,rmin2=0.65, charge=0.0, epsilon=-0.1},
                                //  };
                #endregion
            };

            bool noclash_with_mole = false;

            foreach(var atoms in solv_atoms)
            {
                List<Pdb.IAtom> noclash_atoms = new List<Pdb.IAtom>();
                foreach(var atom in atoms)
                {
                    Vector atom_coord = atom.coord;
                    double atom_rmin  = elem_radius[atom.element.Trim()];

                    Pdb.IAtom near      = kdtree_mole.nearest(atom_coord);
                    double    near_rmin = elem_radius[near.element.Trim()];

                    double dist = (atom_coord - near.coord).Dist;
                    if(atom.name == "OH2 ")
                    {
                        if((atom_rmin + near_rmin) < dist)
                            noclash_with_mole = true;
                        else
                            noclash_with_mole = false;
                    }

                    if(noclash_with_mole)
                    {
                        noclash_atoms.Add(atom);
                        //yield return atom;
                        //socked.Add(atom);
                    }
                }
                if(noclash_atoms.Count != 0)
                    yield return noclash_atoms;
            }
        }
    }
}
