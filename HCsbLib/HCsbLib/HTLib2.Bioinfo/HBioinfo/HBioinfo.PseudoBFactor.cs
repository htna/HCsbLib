using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        public static double[] PseudoBFactor(IList<Vector[]> ensemble, Vector[] meanconf)
        {
            /// http://onlinelibrary.wiley.com/doi/10.1002/prot.22328/pdf
            ///   Yang, L., Song, G. and Jernigan, R. L. (2009), Comparisons of
            ///   experimental and computed protein anisotropic temperature
            ///   factors. Proteins, 76: 164–175. doi: 10.1002/prot.22328
            /// 
            /// Similarly to the isotropic B-factors, B^aniso relates to the
            /// fluctuation 〈ΔRi,ΔRi〉 of atom i as:
            /// 
            ///     B^anisou = 8π^2〈ΔRi,ΔRi〉                            (3)
            ///     
            /// where
            /// 
            ///     〈ΔRi,ΔRi〉                                           (4)
            ///         = [ Δxi^2       Δxi.Δyi     Δxi.Δzi ]
            ///           [ Δxi.Δyi     Δyi^2       Δyi.Δzi ]
            ///           [ Δxi.Δzi     Δyi.Δzi     Δzi^2   ]
            ///           
            /// From the anisotropic B-factors, we can obtain the corre-
            /// sponding isotropic B-factors, because they are related by:
            /// 
            ///     B_i = 1/3 trace(B_i^anisou)                         (5)
            ///     
            /// For NMR ensembles, the pseudo anisotropic B-factors
            /// are calculated by averaging the residue fluctuations
            /// between all conformer pairs in the ensemble, using Eq. (3).
            int size = meanconf.Length;
            double[] bfactor = new double[size];
            for(int i=0; i<size; i++)
            {
                double mean_dxi2 = 0;
                double mean_dyi2 = 0;
                double mean_dzi2 = 0;
                foreach(Vector[] conf in ensemble)
                {
                    HDebug.Assert(meanconf.Length == conf.Length);
                    Vector dRi = conf[i]-meanconf[i];
                    mean_dxi2 += dRi[0]*dRi[0];
                    mean_dyi2 += dRi[1]*dRi[1];
                    mean_dzi2 += dRi[2]*dRi[2];
                }
                mean_dxi2 = mean_dxi2 / ensemble.Count;
                mean_dyi2 = mean_dyi2 / ensemble.Count;
                mean_dzi2 = mean_dzi2 / ensemble.Count;
                double trace_Bi_anisou = (mean_dxi2 + mean_dyi2 + mean_dzi2);
                double Bi = (1.0/3) * trace_Bi_anisou;
                bfactor[i] = Bi;
            }
            return bfactor;
        }
        public static double[] PseudoBFactor(IList<Pdb.Atom[]> ensemble)
        {
            /// 1. get conformations
            /// 2. align conformations to the first conformation
            /// 3. determine mean conformation
            /// 4. reassign the mean conformation as the conformation whose RMSD is the smallest to meanconf
            /// 5. return pseudo b-factor

            // get conformations
            List<Vector[]> lstconf;
            {
                lstconf = new List<Vector[]>();
                Pdb.Atom[] atoms0 = ensemble[0];
                for(int i=0; i<ensemble.Count; i++)
                {
                    Pdb.Atom[] atoms = ensemble[i];
                    Vector[] conf = atoms.ListCoord().ToArray();
                    lstconf.Add(conf);

                    // check if atom types are the same
                    HDebug.Assert(atoms0.Length == atoms.Length);
                    for(int ia=0; ia<atoms0.Length; ia++)
                    {
                        HDebug.Assert(atoms0[ia].serial     == atoms[ia].serial    );
                        HDebug.Assert(atoms0[ia].name       == atoms[ia].name      );
                        HDebug.Assert(atoms0[ia].altLoc     == atoms[ia].altLoc    );
                        HDebug.Assert(atoms0[ia].resName    == atoms[ia].resName   );
                        HDebug.Assert(atoms0[ia].chainID    == atoms[ia].chainID   );
                        HDebug.Assert(atoms0[ia].resSeq     == atoms[ia].resSeq    );
                        HDebug.Assert(atoms0[ia].iCode      == atoms[ia].iCode     );
                        //Debug.Assert(atoms0[ia].x          == atoms[ia].x         );
                        //Debug.Assert(atoms0[ia].y          == atoms[ia].y         );
                        //Debug.Assert(atoms0[ia].z          == atoms[ia].z         );
                        HDebug.Assert(atoms0[ia].occupancy  == atoms[ia].occupancy );
                        HDebug.Assert(atoms0[ia].tempFactor == atoms[ia].tempFactor);
                        HDebug.Assert(atoms0[ia].element    == atoms[ia].element   );
                        HDebug.Assert(atoms0[ia].charge     == atoms[ia].charge    );
                    }
                }
            }
            // align conformations to the first conformation
            {
                for(int i=1; i<lstconf.Count; i++)
                {
                    Vector[] conf = lstconf[i];
                    conf = Align.MinRMSD.Align(lstconf[0], conf);
                    lstconf[i] = conf;
                }
            }
            // determine mean conformation
            Vector[] meanconf;
            {
                meanconf = new Vector[lstconf[0].Length];
                for(int ia=0; ia<meanconf.Length; ia++)
                {
                    List<Vector> coordi = new List<Vector>();
                    foreach(Vector[] conf in lstconf)
                        coordi.Add(conf[ia]);
                    meanconf[ia] = coordi.Mean();
                }
            }
            // reassign the mean conformation as the conformation whose RMSD is the smallest to meanconf
            {
                List<double> lstrmsd = new List<double>();
                for(int ic=0; ic<lstconf.Count; ic++)
                {
                    double rmsd = Align.MinRMSD.GetRMSD(meanconf.ToList(), lstconf[ic].ToList());
                    lstrmsd.Add(rmsd);
                }
                int idxMinRmsd = lstrmsd.HIdxSorted().First();
                meanconf = lstconf[idxMinRmsd].HCloneDeep();
            }
            // return pseudo b-factor
            double[] pseudoBFactor = PseudoBFactor(lstconf, meanconf);
            return pseudoBFactor;
        }
    }
}
