using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public static void GetBFactor(string topolpath, string eigenvecpath, out string[] name, out int[] resSeq, out double[] BFactor
                                     ,bool ignoreNegativeEigval=true
                                      )
        {
            Top topol = Top.FromFile(topolpath);
            Eigenvec eigenvec = Eigenvec.FromFile(eigenvecpath);

            GetBFactor(topol, eigenvec, out name, out resSeq, out BFactor, ignoreNegativeEigval);
        }
        public static void GetBFactor(Top topol, Eigenvec eigenvec, out string[] name, out int[] resSeq, out double[] BFactor
                                     ,bool ignoreNegativeEigval=true
                                     )
        {
            List<Gromacs.Top.Atom> atoms = topol.GetAtoms();

            name = new string[atoms.Count];
            resSeq = new int[atoms.Count];
            BFactor = new double[atoms.Count];
            for(int i=0; i<atoms.Count; i++)
            {
                name[i] = atoms[i].atom;
                resSeq[i] = atoms[i].resnr;

                int count_lambda_0 = 0;
                foreach(var frame in eigenvec.frames)
                {
                    if(frame.lambda != 0)
                        continue;
                    if(frame.step <= 6)
                    {
                        HDebug.Assert(frame.step == frame.frameidx);
                        continue;
                    }
                    count_lambda_0++;
                    Vector[] eigvec = frame.x;
                    double eigval   = frame.time;
                    HDebug.AssertIf(eigval<0, Math.Abs(eigval) < 0.00001);
                    if(ignoreNegativeEigval)
                        if(eigval < 0)
                            continue;
                    HDebug.Assert(BFactor.Length == eigvec.Length);
                    BFactor[i] += LinAlg.VtV(eigvec[i], eigvec[i]) / eigval;
                }
            }
        }
    }
}
