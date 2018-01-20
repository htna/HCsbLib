using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Universe
    {
        public static void SelfTest(string rootpath, string[] args)
        {
            //{
            //    Pdb pdb = Pdb.FromFile(rootpath + @"\Sample\1a6g.pdb");
            //    HTLib2.Bioinfo.Universe.Build(pdb);
            //}
            GetPotential_SelfTest(rootpath, args);

            Universe univ = null;

            string sample = null;
            //sample = "1a6g_autopsf.unfolded";
            //sample = "1a6g_autopsf";
            switch(sample)
            {
                case "alanin":
                    {
                        List<ForceField.IForceField> frcflds = ForceField.GetMindyForceFields();
                        double k = 0.0001;
                        double threshold = 0.001;
                        //int iter = univ.Minimize_ConjugateGradient_v0(frcflds, k, threshold);
                        int randomPurturb = 0; // no random purturbation
                        bool[] atomsMovable = null; // update all atoms
                        int iter_conjgrad;
                        {
                            Namd.Psf psf = Namd.Psf.FromFile(rootpath + @"\Sample\alanin.psf");
                            Pdb pdb = Pdb.FromFile(rootpath + @"\Sample\alanin.pdb");
                            Namd.Prm prm = Namd.Prm.FromFileXPlor(rootpath + @"\Sample\alanin.params", new TextLogger());
                            univ = Universe.Build(psf, prm, pdb, false);

                            //frcflds = new List<ForceField.IForceField>();
                            //frcflds.Add(new ForceField.MindyBond());
                            //frcflds.Add(new ForceField.MindyAngle());
                            //frcflds.Add(new ForceField.MindyDihedral());
                            //frcflds.Add(new ForceField.MindyImproper());
                            //frcflds.Add(new ForceField.MindyNonbondedLennardJones());
                            //frcflds.Add(new ForceField.MindyNonbondedElectrostatic());

                            //iter_conjgrad = univ.Minimize_ConjugateGradient_v1(0, frcflds, k, 0.1, null, threshold, randomPurturb, atomsMovable, new MinimizeLogger_PrintEnergyForceMag(), null, null);

                            //frcflds = new List<ForceField.IForceField>();
                            //frcflds.Add(new ForceField.MindyBond());
                            //frcflds.Add(new ForceField.MindyAngle());
                            //frcflds.Add(new ForceField.MindyImproper());
                            //frcflds.Add(new ForceField.MindyNonbondedLennardJones());
                            //frcflds.Add(new ForceField.MindyNonbondedElectrostatic());

                            //iter_conjgrad = univ.Minimize_ConjugateGradient_v1(0, frcflds, k*0.1, 0.1, null, threshold, randomPurturb, atomsMovable, new MinimizeLogger_PrintEnergyForceMag(), null, null);

                            frcflds = new List<ForceField.IForceField>();
                            if(HDebug.False) frcflds.Add(new ForceField.MindyBond());                   else frcflds.Add(new ForceField.PwBond());
                            if(HDebug.False) frcflds.Add(new ForceField.MindyAngle());                  else frcflds.Add(new ForceField.PwAngle());
                            frcflds.Add(new ForceField.MindyDihedral());
                            if(HDebug.False) frcflds.Add(new ForceField.MindyImproper());               else frcflds.Add(new ForceField.PwImproper());
                            if(HDebug.False) frcflds.Add(new ForceField.MindyNonbondedElectrostatic()); else frcflds.Add(new ForceField.PwElec());
                            if(HDebug.False) frcflds.Add(new ForceField.MindyNonbondedLennardJones());  else frcflds.Add(new ForceField.PwVdw());

                            univ.LoadCoords(@"D:\xxxx.coords");
                            iter_conjgrad = univ.Minimize_ConjugateGradient_v1(0, frcflds, k, 0.01, null, threshold, randomPurturb, atomsMovable, new MinimizeLogger_PrintEnergyForceMag(), null, null);
                            univ.SaveCoords(@"D:\xxxx.coords");
                            System.Console.WriteLine("=======================================================================");
                            System.Console.WriteLine("=======================================================================");
                            System.Console.WriteLine("=======================================================================");
                        }
                        {
                            //Psf psf = Psf.FromFile(rootpath + @"\Sample\alanin.psf");
                            //Pdb pdb = Pdb.FromFile(rootpath + @"\Sample\alanin.pdb");
                            //Prm prm = Prm.FromFileXPlor(rootpath + @"\Sample\alanin.params", new TextLogger());
                            //univ = Universe.Build(psf, prm, pdb);

                            //frcflds = new List<ForceField.IForceField>();
                            //if(true) frcflds.Add(new ForceField.MindyBond()); else frcflds.Add(new ForceField.PwBond());
                            //if(true) frcflds.Add(new ForceField.MindyAngle()); else frcflds.Add(new ForceField.PwAngle());
                            //frcflds.Add(new ForceField.MindyDihedral());
                            //if(true) frcflds.Add(new ForceField.MindyImproper()); else frcflds.Add(new ForceField.PwImproper());
                            //if(true) frcflds.Add(new ForceField.MindyNonbondedElectrostatic()); else frcflds.Add(new ForceField.PwElec());
                            //if(true) frcflds.Add(new ForceField.MindyNonbondedLennardJones()); else frcflds.Add(new ForceField.PwVdw());
                            univ.atoms[0].Coord += new Vector(0.1,0.1,0.1);
                            //iter_conjgrad = univ.Minimize_ConjugateGradient_v1(0, frcflds, k, 0.001, null, threshold, randomPurturb, atomsMovable, new MinimizeLogger_PrintEnergyForceMag(), null, null);
                            iter_conjgrad = univ.Minimize_ConjugateGradient_AtomwiseUpdate(frcflds,
                                                                                    threshold: threshold,
                                                                                    k: k,
                                                                                    max_atom_movement: 0.001,
                                                                                    max_iteration: null,
                                                                                    atomsMovable: atomsMovable,
                                                                                    logger: new MinimizeLogger_PrintEnergyForceMag()
                                                                                    );
                        }
                        k = 0.0005;
                        HDebug.Assert(false);
                        //int iter_stepdsnt = univ.Minimize_SteepestDescent(frcflds, k, threshold, System.Console.Error);
                    }
                    break;
                case "1a6g_autopsf":
                    {
                        Namd.Psf psf = Namd.Psf.FromFile(rootpath + @"\Sample\1a6g_autopsf.psf");
                        Pdb pdb = Pdb.FromFile(rootpath + @"\Sample\1a6g_autopsf.pdb");
                        Namd.Prm prm = Namd.Prm.FromFile(rootpath + @"\Sample\1a6g_autopsf.prm", new TextLogger());

                        univ = Universe.Build(psf, prm, pdb, false);

                        List<ForceField.IForceField> frcflds = ForceField.GetMindyForceFields();
                        double threshold = 0.001;
                        string minimize_ver = "v1";
                        switch(minimize_ver)
                        {
                            //case "v2":
                            //    {
                            //        double atom_max_move = 0.03;
                            //        int iter_conjgrad = univ.Minimize_ConjugateGradient_v2(frcflds, atom_max_move, threshold, System.Console.Error);
                            //        break;
                            //    }
                            case "v1":
                                {
                                    double k = 0.0001;
                                    int randomPurturb = 0; // no random purturbation
                                    bool[] atomsMovable = null; // update all atoms
                                    int iter_conjgrad = univ.Minimize_ConjugateGradient_v1(0, frcflds, k, 0.1, null, threshold, randomPurturb, atomsMovable, new MinimizeLogger_PrintEnergyForceMag(), null, null);
                                    break;
                                }
                            default:
                                goto case "v1";
                        }
                        //atom_max_move = 0.03;
                        //int iter_stepdsnt = univ.Minimize_SteepestDescent(frcflds, atom_max_move, threshold, System.Console.Error);
                    }
                    break;
                case "1a6g_autopsf.unfolded":
                    {
                        Namd.Psf psf = Namd.Psf.FromFile(rootpath + @"\Sample\1a6g_autopsf.psf");
                        Pdb pdb = Pdb.FromFile(rootpath + @"\Sample\1a6g_autopsf.pdb");
                        {
                            Random rand = new Random(1);
                            List<Vector> coords = pdb.atoms.ListCoord();

                            for(int i=0; i<coords.Count; i++)
                            {
                                double x = i*0.1+rand.NextDouble()*0.01;
                                double y = i*0.1+rand.NextDouble()*0.01;
                                double z = i*0.1+rand.NextDouble()*0.01;
                                coords[i] = new Vector(x, y, z);
                            }
                            pdb.ToFile(rootpath+@"\Sample\1a6g_autopsf.unfolded.pdb", coords);
                            pdb = Pdb.FromFile(rootpath + @"\Sample\1a6g_autopsf.unfolded.pdb");
                        }
                        Namd.Prm prm = Namd.Prm.FromFile(rootpath + @"\Sample\1a6g_autopsf.prm", new TextLogger());

                        univ = Universe.Build(psf, prm, pdb, false);

                        List<ForceField.IForceField> frcflds = ForceField.GetMindyForceFields();
                        double threshold = 0.01;
                        string minimize_ver = "v1";
                        switch(minimize_ver)
                        {
                            //case "v2":
                            //    {
                            //        double atom_max_move = 0.1;
                            //        int iter_conjgrad = univ.Minimize_ConjugateGradient_v2(frcflds, atom_max_move, threshold, System.Console.Error);
                            //        break;
                            //    }
                            case "v1":
                                {
                                    double k = 0.0001;
                                    int randomPurturb = 100;
                                    bool[] atomsMovable = null; // updates all atoms
                                    int iter_conjgrad = univ.Minimize_ConjugateGradient_v1(0, frcflds, k, 0.1, null, threshold, randomPurturb, atomsMovable, new MinimizeLogger_PrintEnergyForceMag(), null, null);
                                    break;
                                }
                            default:
                                goto case "v1";
                        }
                        //atom_max_move = 0.03;
                        //int iter_stepdsnt = univ.Minimize_SteepestDescent(frcflds, atom_max_move, threshold, System.Console.Error);
                    }
                    break;
                default:
                    goto case "alanin";
            }
        }
    }
}
