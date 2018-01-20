using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
	{
        /// Mixed-resolution ANM 
        /// * Focused Functional Dynamics of Supramolecules by Use of a Mixed-Resolution Elastic Network Model
        ///   Biophys J: http://www.cell.com/biophysj/abstract/S0006-3495(09)01111-4
        ///              http://www.ncbi.nlm.nih.gov/pmc/articles/PMC2726304/
        /// * Collective Dynamics of Large Proteins from Mixed Coarse-Grained Elastic Network Model
        ///   http://onlinelibrary.wiley.com/doi/10.1002/qsar.200430922/abstract

        public class CMixAnm
        {
            public Matrix     hess   = null;
            public Pdb.Atom[] atoms  = null;
            public Vector[]   coords = null;
            public Matrix     kij    = null;

            public Mode[]    _modes  = null;
            public Mode[] GetModes(ILinAlg la)
            {
                if(_modes == null)
                {
                    _modes = Hess.GetModesFromHess(hess, la);
                }
                return _modes;
            }
        }
        public static CMixAnm GetHessMixAnm(IList<Pdb.Atom> atoms, IList<Vector> coords, IList<int> resiAllAtom)
        {
            double AaCutoff =  6.0; double AaSprcst = 0.64;
            double CaCutoff = 13.0; double CaSprcst = 3.10;
            List<Tuple<char?,int>> lresiAllAtom = new List<Tuple<char?,int>>();
            foreach(int resi in resiAllAtom)
                lresiAllAtom.Add(new Tuple<char?, int>(null, resi)); // null apply to all chains
            return GetHessMixAnm(atoms, coords, lresiAllAtom
                                ,CaCutoff, CaSprcst
                                ,AaCutoff, AaSprcst
                                );
        }
        public static CMixAnm GetHessMixAnm(IList<Pdb.Atom> atoms, IList<Vector> coords, IList<Tuple<char?,int>> resiAllAtom)
        {
            double AaCutoff =  6.0; double AaSprcst = 0.64;
            double CaCutoff = 13.0; double CaSprcst = 3.10;
            return GetHessMixAnm(atoms, coords, resiAllAtom
                                ,CaCutoff, CaSprcst
                                ,AaCutoff, AaSprcst
                                );
        }
        public static CMixAnm GetHessMixAnm(IList<Pdb.Atom> atoms
                                           ,IList<Vector>   coords
                                           ,IList<Tuple<char?,int>> resiAllAtom // list of (chain ID, residue seq). null chain ID is the wild card
                                           ,double CaCutoff, double CaSprcst
                                           ,double AaCutoff, double AaSprcst
                                           )
        {
            HDebug.Assert(atoms.Count == coords.Count);

            List<Tuple<Pdb.Atom, Vector, double, double>> lstAtomLocCtofSprk = new List<Tuple<Pdb.Atom, Vector, double, double>>();
            {
                HashSet<Tuple<char?,int>> setResiAllAtom = resiAllAtom.HToHashSet();
                for(int i=0; i<atoms.Count; i++)
                {
                    var atom = atoms[i];
                    var pos  = coords[i];
                    Tuple<char?,int> resi0 = new Tuple<char?,int>(atom.chainID,atom.resSeq);
                    Tuple<char?,int> resi1 = new Tuple<char?,int>(null        ,atom.resSeq); // wild card
                    if(setResiAllAtom.Contains(resi0) || setResiAllAtom.Contains(resi1))
                    {
                        lstAtomLocCtofSprk.Add(new Tuple<Pdb.Atom, Vector, double, double>(atom, pos, AaCutoff, AaSprcst));
                    }
                    else
                    {
                        string name = atom.name.Trim();
                        if(name == "CA")
                        {
                            lstAtomLocCtofSprk.Add(new Tuple<Pdb.Atom, Vector, double, double>(atom, pos, CaCutoff, CaSprcst));
                        }
                    }
                }
            }

            Vector[]  hesscoords = lstAtomLocCtofSprk.HListItem2().ToArray();
            double[,] hesskij = new double[hesscoords.Length, hesscoords.Length];
            {
                for(int i=0; i<hesscoords.Length; i++)
                {
                    Vector coordi  = hesscoords[i];
                    double cutoffi = lstAtomLocCtofSprk[i].Item3;
                    double sprcsti = lstAtomLocCtofSprk[i].Item4;
                    double cutoffi3 = cutoffi*cutoffi*cutoffi;
                    for(int j=i; j<hesscoords.Length; j++)
                    {
                        if(i == j)
                        {
                            hesskij[i, j] = 0;
                            continue;
                        }

                        Vector coordj = hesscoords[j];
                        double cutoffj = lstAtomLocCtofSprk[j].Item3;
                        double sprcstj = lstAtomLocCtofSprk[j].Item4;
                        double cutoffj3 = cutoffj*cutoffj*cutoffj;

                        double distij = (coordi - coordj).Dist;

                        double cutoffij;
                        if(cutoffi == cutoffj) cutoffij = cutoffi;
                        else                   cutoffij = Math.Pow((cutoffi3+cutoffj3)/2, 1/3.0);

                        double sprcstij = 0;
                        if(distij < cutoffij)
                            sprcstij = (sprcsti + sprcstj)/2;

                        hesskij[i, j] = sprcstij;
                        hesskij[j, i] = sprcstij;
                    }
                }
            }

            Matrix hess = GetHessAnm(hesscoords, hesskij);

            return new CMixAnm
            {
                atoms  = lstAtomLocCtofSprk.HListItem1().ToArray(),
                coords = hesscoords,
                kij    = hesskij,
                hess   = hess
            };
        }

        public static HessInfo GetHessMixAnm(IList<Vector>   coords
                                            ,IList<int>      lstIdxCa
                                            ,IList<int>      lstIdxAll
                                            )
        {
            double AaCutoff =  6.0; double AaSprcst = 0.64;
            double CaCutoff = 13.0; double CaSprcst = 3.10;
            return GetHessMixAnm(coords, lstIdxCa, lstIdxAll
                                ,CaCutoff, CaSprcst
                                ,AaCutoff, AaSprcst
                                );
        }
        public static HessInfo GetHessMixAnm(IList<Vector>   coords
                                            ,IList<int>      lstIdxCa
                                            ,IList<int>      lstIdxAll
                                            ,double CaCutoff, double CaSprcst
                                            ,double AaCutoff, double AaSprcst
                                            )
        {
            int size = coords.Count;

            Vector[] mixCoords = new Vector[size];
            double[] cutoffs = new double[size]; cutoffs = cutoffs.HUpdateValueAll(double.NaN);
            double[] sprcsts = new double[size]; sprcsts = sprcsts.HUpdateValueAll(double.NaN);

            foreach(int idx in lstIdxCa ) { mixCoords[idx] = coords[idx].Clone(); cutoffs[idx] = CaCutoff; sprcsts[idx] = CaSprcst; }
            foreach(int idx in lstIdxAll) { mixCoords[idx] = coords[idx].Clone(); cutoffs[idx] = AaCutoff; sprcsts[idx] = AaSprcst; }

            double[,] hesskij = new double[size, size]; hesskij = hesskij.HUpdateValueAll(double.NaN);
            for(int i=0; i<size; i++)
            {
                Vector coordi  = mixCoords[i];
                if(coordi == null)
                    continue;
                double cutoffi = cutoffs[i];
                double sprcsti = sprcsts[i];
                double cutoffi3 = cutoffi*cutoffi*cutoffi;
                for(int j=i; j<size; j++)
                {
                    if(i == j)
                    {
                        hesskij[i, j] = 0;
                        continue;
                    }

                    Vector coordj = mixCoords[j];
                    if(coordj == null)
                        continue;
                    double cutoffj = cutoffs[j];
                    double sprcstj = sprcsts[j];
                    double cutoffj3 = cutoffj*cutoffj*cutoffj;

                    double distij = (coordi - coordj).Dist;

                    double cutoffij;
                    if(cutoffi == cutoffj) cutoffij = cutoffi;
                    else                   cutoffij = Math.Pow((cutoffi3+cutoffj3)/2, 1/3.0);

                    double sprcstij = 0;
                    if(distij < cutoffij)
                        sprcstij = (sprcsti + sprcstj)/2;

                    hesskij[i, j] = sprcstij;
                    hesskij[j, i] = sprcstij;
                }
            }

            HessMatrix hess = GetHessAnm(mixCoords, hesskij);

            Vector mass = new double[size]; mass.SetValue(1);
            var hessinfo = new Hess.HessInfo
            {
                hess = hess,
                mass = mass,
                coords = mixCoords,
                atoms = null,
                numZeroEigval = 6,
            };

            return hessinfo;
        }
        public static Mode[] GetModeMixAnm(IList<Vector> coords
                                            , IList<int>      lstIdxCa
                                            , IList<int>      lstIdxAll
                                            , ILinAlg la
                                            )
        {
            double AaCutoff =  6.0; double AaSprcst = 0.64;
            double CaCutoff = 13.0; double CaSprcst = 3.10;
            return GetModeMixAnm(coords, lstIdxCa, lstIdxAll
                                ,CaCutoff, CaSprcst
                                ,AaCutoff, AaSprcst
                                ,la
                                );
        }
        public static Mode[] GetModeMixAnm(IList<Vector> coords
                                            , IList<int> lstIdxCa
                                            , IList<int> lstIdxAll
                                            , double CaCutoff, double CaSprcst
                                            , double AaCutoff, double AaSprcst
                                            , ILinAlg la
                                            )
        {
            HessInfo hessinfo = GetHessMixAnm(coords, lstIdxCa, lstIdxAll
                                ,CaCutoff, CaSprcst
                                ,AaCutoff, AaSprcst
                                );

            int[] idxMixed = lstIdxCa.HUnionWith(lstIdxAll);

            var hessinfoMixed = hessinfo.GetSubHessInfo(idxMixed);
            Mode[] modesMixed = hessinfoMixed.GetModesMassReduced(true, la);
            Mode[] modes      = modesMixed.ToModeEigvecResized(coords.Count, idxMixed);
            return modes;
        }
	}
}
