using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        /////////////////////////////////////////////////////////////////////////////////
        //                                                                             //
        //  http://en.wikipedia.org/wiki/Energy_minimization                           //
        //                                                                             //
        /////////////////////////////////////////////////////////////////////////////////
        //                                                                             //
        //  Algorithm                                                                  //
        //    coord[i]_(t) = coord[i]_(t-1) + k * h[i]_(t)                             //
        //    h[i]_(t) = F[i]_(t) + r[i]_(t-1) * h[i]_(t-1)                            //
        //    F[i]_(t) = force with atom i at coord[i]_(t)                             //
        //    r[i]_(t-1) = F[i]_(t) . F[i]_(t) / F[i]_(t-1) . F[i]_(t-1)               //
        //    where coord[i] is the coordinate of atom i,                              //
        //          F.F is inner product, and                                          //
        //          h[i]_0 = 0 (same to the steepest descent)                          //
        //                                                                             //
        //    Initial (step 1)                                                         //
        //      h = 0                                                                  //
        //      forces0 = forces                                                       //
        //    Iteration (step n>1)                                                     //
        //      a. get forces                                                          //
        //      b. r = forces . forces / forces0 . forces0                             //
        //      c. h = forces + r * h                                                  //
        //      d. coords = coords + k * h                                             //
        //                                                                             //
        /////////////////////////////////////////////////////////////////////////////////
        //                                                                             //
        //  Procedure                                                                  //
        //    0. Initial configuration of atoms                                        //
        //    1. Save the position of atoms                                            //
        //    2. Calculate the potential energy of system and the net forces on atoms  //
        //    3. Check if every force reaches to zero                                  //
        //       A. if yes, END                                                        //
        //    4. Move atoms with conjugated gradient                                   //
        //    5. Predict energy or forces on atoms                                     //
        //    6. Check if the predicted forces or energy will exceed over the limit    //
        //       A. if no, goto 1                                                      //
        //    7. Back to saved configuration                                           //
        //    8. Move atoms with simple gradient                                       //
        //    9. goto 1                                                                //
        /////////////////////////////////////////////////////////////////////////////////

        static Vector[] AddConditional(Vector[] coords, bool[] movables, Vector[] moves, Vector[] movesAdd, double leastMove)
        {
            HDebug.Assert(coords.Length == moves.Length, moves.Length == movables.Length);
            int countUpdated = 0;
            int size = coords.Length;
            Vector[] coordsUpdated = new Vector[size];
            double leastMove2 = leastMove*leastMove;

            double[] dist2s = new double[size];
            for(int i=0; i<size; i++)
            {
                moves[i] = moves[i]*0 + movesAdd[i];
                dist2s[i] = moves[i].Dist2;
            }
            {
                double[] sorteds = dist2s.HSort().ToArray();
                leastMove2 = Math.Min(leastMove2, sorteds[sorteds.Length/3]);
            }
            for(int i=0; i<size; i++)
            {
                if(movables[i] && (dist2s[i] > leastMove2))
                {
                    countUpdated++;
                    coordsUpdated[i] = coords[i] + moves[i];
                    moves[i].SetZero();
                }
                else
                    coordsUpdated[i] = coords[i];
            }
            return coordsUpdated;
        }
        static Vector[] AddConditional(Vector[] coords, Vector[] movements, bool[] movables)
        {
            int countupdated = 0;
            HDebug.Assert(coords.Length == movements.Length, movements.Length == movables.Length);
            int size = coords.Length;
            Vector[] coordsUpdated = new Vector[size];
            for(int i=0; i<size; i++)
            {
                if(movables[i])
                {
                    countupdated++;
                    coordsUpdated[i] = coords[i] + movements[i];
                }
                else
                    coordsUpdated[i] = coords[i];
            }
            return coordsUpdated;
        }
        static double NormInf(Vector[] forces, bool[] movables)
        {
            HDebug.Assert(forces.Length == movables.Length);
            int size = forces.Length;
            List<double> norms = new List<double>();
            for(int i=0; i<size; i++)
                if(movables[i])
                    norms.Add(forces[i].NormInf());
            return (new Vector(norms.ToArray())).NormInf();
        }
        static double Norm(int p, Vector[] forces, bool[] movables)
        {
            HDebug.Assert(forces.Length == movables.Length);
            int size = forces.Length;
            List<double> norms = new List<double>();
            for(int i=0; i<size; i++)
                if(movables[i])
                    norms.Add(forces[i].Norm(p));
            return (new Vector(norms.ToArray())).Norm(p);
        }

        public int Minimize_ConjugateGradient(List<ForceField.IForceField> frcflds
                                             ,string logpath           = ""
                                             ,int    iterInitial       = 0
                                             ,double? k                = null
                                             ,int? max_iteration       = null
                                             ,double max_atom_movement = 0.1
                                             ,double threshold         = 0.001
                                             ,int    randomPurturb     = 0
                                             ,bool[] atomsMovable      = null
                                             ,InfoPack extra           = null
                                             )
        {
            IMinimizeLogger logger = new MinimizeLogger_PrintEnergyForceMag(logpath);

            return Minimize_ConjugateGradient_v1(iterInitial, frcflds, k, max_atom_movement, max_iteration, threshold, randomPurturb, atomsMovable, logger, extra, null);
        }
        public int Minimize_ConjugateGradient(List<ForceField.IForceField> frcflds
                                             ,IMinimizeLogger logger
                                             ,int iterInitial          = 0
                                             ,double? k                = null
                                             ,double max_atom_movement = 0.1
                                             ,double threshold         = 0.001
                                             ,int randomPurturb        = 0
                                             ,bool[] atomsMovable      = null
                                             ,InfoPack extra           = null
                                             )
        {
            return Minimize_ConjugateGradient_v1(iterInitial, frcflds, k, max_atom_movement, null, threshold, randomPurturb, atomsMovable, logger, extra, null);
        }
        //public int Minimize_ConjugateGradient(List<ForceField.IForceField> frcflds)
        //{
        //    double k = 0.0001;
        //    double threshold = 0.001;
        //    double max_atom_movement = 0.1;
        //    IMinimizeLogger logger = null;
        //    int randomPurturb = 0; // no random purturbation
        //    bool[] atomsMovable = null;
        //    return Minimize_ConjugateGradient_v1(0, frcflds, k, max_atom_movement, threshold, randomPurturb, atomsMovable, logger);
        //}
        //public int Minimize_ConjugateGradient(List<ForceField.IForceField> frcflds, double k, double threshold)
        //{
        //    IMinimizeLogger logger = null;
        //    double max_atom_movement = 0.1;
        //    int randomPurturb = 0; // no random purturbation
        //    bool[] atomsMovable = null;
        //    return Minimize_ConjugateGradient_v1(0, frcflds, k, max_atom_movement, threshold, randomPurturb, atomsMovable, logger);
        //}
        //public int Minimize_ConjugateGradient(List<ForceField.IForceField> frcflds, double k, double threshold, int randomPurturb)
        //{
        //    IMinimizeLogger logger = null;
        //    double max_atom_movement = 0.1;
        //    bool[] atomsMovable = null;
        //    return Minimize_ConjugateGradient_v1(0, frcflds, k, max_atom_movement, threshold, randomPurturb, atomsMovable, logger);
        //}
        public static void Minimize_ConjugateGradient_WriteLog(System.IO.TextWriter logwriter, long iter, double energy, Vectors forces, bool[] movables, string message="")
        {
            if(logwriter == null)
                return;
            double forces_NormInf = NormInf(forces, movables);
            double forces_Norm1   = Norm(1, forces, movables);
            double forces_Norm2   = Norm(2, forces, movables);
            //logwriter.Write("" + iter + "-iter: energy(" + energy + ")");
            //logwriter.Write(                 ", force-norm-1(" + forces_Norm1 + ")");
            //logwriter.Write(                 ", force-norm-2(" + forces_Norm2 + ")");
            //logwriter.Write(                 ", force-norm-inf(" + forces_NormInf + ")");
            logwriter.Write("{0,2:####}-iter: energy({1:0.00000000000000})", iter, energy); 
            logwriter.Write(         ", force-norm-1({0:0.00000000000000})",forces_Norm1);
            logwriter.Write(         ", force-norm-2({0:0.00000000000000})",forces_Norm2);
            logwriter.Write(       ", force-norm-inf({0:0.00000000000000})",forces_NormInf);

            if(message.Length != 0)
                logwriter.Write(" - " + message);
            logwriter.WriteLine();
        }
        public interface IMinimizeLogger
        {
            void log(long iter, Vectors coords, double energy, Vectors forces, bool[] movables, string message="");
            void logTrajectory(Universe univ, long iter, Vectors coords);
        }
        public class MinimizeLogger : IMinimizeLogger
        {
            public void log(long iter, Vectors coords, double energy, Vectors forces, bool[] movables, string message="")
            {
            }
            public void logTrajectory(Universe univ, long iter, Vectors coords)
            {
            }
        }
        public class MinimizeLogger_PrintEnergyForceMag : IMinimizeLogger
        {
            string logpath = null;
            public int logTrajectoryFrequency = 10;
            public MinimizeLogger_PrintEnergyForceMag(string logpath = null)
            {
                this.logpath = logpath;
            }
            public void log(long iter, Vectors coords, double energy, Vectors forces, string message="")
            {
                Minimize_ConjugateGradient_WriteLog(System.Console.Error, iter, energy, forces, null, message);
            }
            public void log(long iter, Vectors coords, double energy, Vectors forces, bool[] movables, string message="")
            {
                Minimize_ConjugateGradient_WriteLog(System.Console.Error, iter, energy, forces, movables, message);
            }
            public void logTrajectory(Universe univ, long iter, Vectors coords)
            {
                if(logpath == null)
                    return;

                if(iter % logTrajectoryFrequency != 0)
                    return;

                string pdbid = "";
                if(univ.refs.ContainsKey("pdbid"))
                    pdbid = univ.refs["pdbid"].String;
                string dir = logpath;
                if(dir == "")
                    dir = "output-"+pdbid;
                System.IO.Directory.CreateDirectory(dir);
                string pdbname = string.Format("mini.conju.{0:D5}.pdb", iter);
                univ.pdb.ToFile(dir+"\\"+pdbname, coords.ToArray());
                if(iter %100 == 0)
                    univ.SaveCoords(dir+"\\conformation.coords", coords);

                string pmlloadname = (pdbid != "") ? pdbid : "eqlib";

                try
                {
                    System.IO.File.AppendAllLines(dir+"\\mini.conju.[animation].pml", new string[] { "load "+pdbname+", "+pmlloadname });
                }
                catch(Exception)
                {
                }
            }
        }
    }
}
