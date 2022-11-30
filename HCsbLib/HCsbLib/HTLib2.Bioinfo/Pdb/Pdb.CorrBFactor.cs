using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
	{
        public double CorrBFactor(IList<string> name, IList<int> resSeq, IList<double> BFactor, char selAltLoc='A', InfoPack extre=null, bool ignoreHydrogen=true)
        {
            List<double> bfactor_pdb   = new List<double>();
            List<double> bfactor_input = new List<double>();
            HDebug.Assert(name.Count == resSeq.Count);
            HDebug.Assert(name.Count == BFactor.Count);
            int count = name.Count;
            for(int i=0; i<count; i++)
            {
                List<Atom> found = atoms.FindAtoms(name[i], resSeq[i]);
                if(found.Count == 0)
                    continue;

                int idx=-1;
                for(int j=0; j<found.Count; j++) if(found[j].altLoc==' '      ) idx=j;
                for(int j=0; j<found.Count; j++) if(found[j].altLoc==selAltLoc) idx=j;
                
                HDebug.Assert(idx != -1); // only one atom should be found
                                         // handle altLoc later
                if(ignoreHydrogen)
                    if(found[idx].IsHydrogen())
                        continue;

                bfactor_pdb.Add(found[idx].tempFactor);
                bfactor_input.Add(BFactor[i]);
            }

            double corr = NumericSolver.Corr(bfactor_pdb.ToArray(), bfactor_input.ToArray());

            if(extre != null)
            {
                MatrixByArr A = new double[bfactor_input.Count, 2];
                for(int i=0; i<bfactor_input.Count; i++)
                {
                    A[i, 0] = bfactor_input[i];
                    A[i, 1] = 1;
                }
                Vector b = bfactor_pdb.ToArray();
                Matrix pinvA = NumericSolver.Pinv(A);
                Vector x = LinAlg.MV(pinvA, b);

                extre["bfactor_pdb"  ] = bfactor_pdb;
                extre["bfactor_input"] = bfactor_input;
                extre["info1"] = "approx (bfactor_input * scale1 + shift1 = bfactor_pdb)";
                extre["scale1"] = x[0];
                extre["shift1"] = x[1];
                extre["info2"] = "approx (bfactor_input * scale2 = bfactor_pdb)";
                extre["scale2"] = LinAlg.VtV(bfactor_input.HToVectorT(), bfactor_pdb.HToVectorT())    // x = (A' * A)^-1 * (A' * b)  <= a,b: column vector
                                / LinAlg.VtV(bfactor_input.HToVectorT(), bfactor_input.HToVectorT()); //     (a' * a) / (a' * a)     <= A  : matrix
            }
            return corr;
        }
        //public double CorrBFactor(IList<string> name, IList<int> resSeq, IList<double> mass, IList<double> BFactor, InfoPack extre=null)
        //{
        //    List<double> bfactor_pdb   = new List<double>();
        //    List<double> bfactor_input = new List<double>();
        //    Debug.Assert(name.Count == resSeq.Count);
        //    Debug.Assert(name.Count == BFactor.Count);
        //    Debug.Assert(name.Count == mass.Count);
        //    int count = name.Count;
        //    for(int i=0; i<count; i++)
        //    {
        //        List<Atom> found = Atom.FindAtoms(atoms, name[i], resSeq[i]);
        //        if(found.Count == 0)
        //            continue;
        //        Debug.Assert(found.Count == 1); // only one atom should be found
        //        // handle altLoc later
        //        bfactor_pdb.Add(found[0].tempFactor);
        //        bfactor_input.Add(BFactor[i]/mass[i]);
        //    }
        //
        //    double corr = NumericSolver.Corr(bfactor_pdb.ToArray(), bfactor_input.ToArray());
        //
        //    if(extre != null)
        //    {
        //        Matrix A = new double[bfactor_input.Count, 2];
        //        for(int i=0; i<bfactor_input.Count; i++)
        //        {
        //            A[i, 0] = bfactor_input[i];
        //            A[i, 1] = 1;
        //        }
        //        Vector b = bfactor_pdb.ToArray();
        //        Matrix pinvA = NumericSolver.Pinv(A);
        //        Vector x = Vector.MV(pinvA, b);
        //
        //        extre["bfactor_pdb"] = bfactor_pdb;
        //        extre["bfactor_input"] = bfactor_input;
        //        extre["info1"] = "approx (bfactor_input * scale1 + shift1 = bfactor_pdb)";
        //        extre["scale1"] = x[0];
        //        extre["shift1"] = x[1];
        //        extre["info2"] = "approx (bfactor_input * scale2 = bfactor_pdb)";
        //        extre["scale2"] = Vector.VtV(bfactor_input.ToArray(), bfactor_pdb.ToArray())    // x = (A' * A)^-1 * (A' * b)  <= a,b: column vector
        //                        / Vector.VtV(bfactor_input.ToArray(), bfactor_input.ToArray()); //     (a' * a) / (a' * a)     <= A  : matrix
        //    }
        //    return corr;
        //}
    }
}
