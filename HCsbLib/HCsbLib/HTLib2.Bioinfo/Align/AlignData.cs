using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Align
    {
        public class AlignData
        {
            public string       cachebase = null;
            public Pdb          pdb;
            public List<Vector> coords;
            public double[]     masses;
            public MatrixByArr[,]    hess;
                   //Tuple<Vector[],double[]>[] _hess3x3_eigs = null;
                   Mode[]   _modes = null;
                   Anisou[] _anisous = null;
            public int          size { get { return coords.Count; } }

            public static AlignData FromFiles(string path_pdb, string path_hess, IList<double> masses, string cachebase=null)
            {
                AlignData data = new AlignData();
                data.pdb = Pdb.FromFile(path_pdb);
                data.coords = data.pdb.atoms.ListCoord();
                data.cachebase = cachebase;
                int n = data.coords.Count;

                {   // data.hess
                    List<string> lines = HFile.ReadLines(path_hess).ToList();
                    int row, col;
                    {
                        string[] tokens = lines[1].Split(new char[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        HDebug.Assert(tokens.Length == 2);
                        row = int.Parse(tokens[0]);
                        col = int.Parse(tokens[1]);
                        HDebug.Assert(row == col);
                    }
                    HDebug.Assert(row == n*3, col == n*3);
                    MatrixByArr[,] hess = new MatrixByArr[n, n];
                    for(int c=0; c<n; c++)
                        for(int r=0; r<n; r++)
                            hess[c, r] = new double[3, 3];
                    for(int c=0; c<col; c++)
                    {
                        string line = lines[c+2];
                        string[] tokens = line.Split(new char[] { ' ', ',', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        HDebug.Assert(tokens.Length == row);
                        for(int r=0; r<row; r++)
                        {
                            double val = double.Parse(tokens[r]);
                            hess[c/3, r/3][c%3, r%3] += val;
                            hess[r/3, c/3][r%3, c%3] += val;
                        }
                    }
                    for(int c=0; c<n; c++)
                        for(int r=0; r<n; r++)
                            hess[c, r] /= 2;
                    data.hess = hess;
                }

                data.masses = masses.ToArray();

                return data;
            }
            public Mode[] GetModes()
            {   // determine eigvals and eigvecs
                if(_modes != null)
                    return _modes.HClone<Mode>();
                MatrixByArr hess = MatrixByArr.FromMatrixArray(this.hess);
                _modes = HBioinfo.GetModes(hess, cachebase + "Modes.data");
                return _modes.HClone<Mode>();
            }
            public Anisou[] GetAnisous()
            {
                if(_anisous != null)
                    return _anisous;
                MatrixByArr hess = MatrixByArr.FromMatrixArray(this.hess);
                _anisous = Anisou.FromHessian(hess, masses, scale:10000000, cachepath:cachebase+"Anisous.data");
                //_anisous = Bioinfo.GetAnisou(GetModes(), masses, scale: 10000000);
                return _anisous;
            }
            public double[] GetBFactors()
            {
                Anisou[] anisous = GetAnisous();
                double[] bfactors = new double[size];
                for(int i=0; i<size; i++)
                    bfactors[i] = anisous[i].U.Trace();
                return bfactors;
            }
            //public Tuple<Vector[], double[]> GetAnmHessEigs(string cachepath, Vector mass, out MatrixByArr hess)
            //{
            //    Tuple<Vector[], double[]> hess_eigs;
            //    if(cachepath != null && HFile.Exists(cachepath))
            //    {
            //        HSerialize.Deserialize(cachepath, null, out hess, out hess_eigs);
            //        return hess_eigs;
            //    }
            //    else
            //    {
            //        Vector[] eigvec;
            //        double[] eigval;
            //        hess = Hess.GetHessAnm(coords);
            //        hess = Hess.GetMassWeightedHess(hess, mass);
            //        HDebug.Verify(NumericSolver.Eig(hess, out eigvec, out eigval));
            //        {   // sort by eigenvalues
            //            int[] idx = eigval.HIdxSorted();
            //            Vector[] _eigvec = eigvec.HClone<Vector>();
            //            double[] _eigval = eigval.HClone<double>();
            //            for(int i=0; i<eigval.Length; i++)
            //            {
            //                eigvec[i] = _eigvec[idx[i]];
            //                eigval[i] = _eigval[idx[i]];
            //            }
            //        }
            //        hess_eigs = new Tuple<Vector[], double[]>(eigvec, eigval);
            //
            //        HSerialize.Serialize(cachepath, null, hess, hess_eigs);
            //        return hess_eigs;
            //    }
            //}
            public double GetRmsdFrom(IList<Vector> coordsto)
            {
                HDebug.Assert(size == coordsto.Count);
                double rmsd = 0;
                for(int i=0; i<size; i++)
                    rmsd += (coords[i] - coordsto[i]).Dist2;
                rmsd = Math.Sqrt(rmsd / size);
                return rmsd;
            }
            public double GetEnergyFromDiag(IList<Vector> coordsto)
            {
                HDebug.Assert(size == coordsto.Count);
                Vector[] dcoords = new Vector[size];
                for(int i=0; i<size; i++)
                    dcoords[i] = coords[i] - coordsto[i];
                {
                    double energy = 0;
                    for(int i=0; i<size; i++)
                        energy += LinAlg.VtMV(dcoords[i], hess[i, i], dcoords[i]);
                    return energy;
                }
            }
            public double GetEnergyFromAnisou(IList<Vector> coordsto)
            {
                HDebug.Assert(size == coordsto.Count);
                Vector[] dcoords = new Vector[size];
                for(int i=0; i<size; i++)
                    dcoords[i] = coords[i] - coordsto[i];

                Anisou[] anisous = GetAnisous();
                {
                    double energy = 0;
                    for(int i=0; i<size; i++)
                    {
                        double energy0 = Math.Pow(LinAlg.VtV(dcoords[i], anisous[i].eigvecs[0]), 2) / anisous[i].eigvals[0];
                        double energy1 = Math.Pow(LinAlg.VtV(dcoords[i], anisous[i].eigvecs[1]), 2) / anisous[i].eigvals[1];
                        double energy2 = Math.Pow(LinAlg.VtV(dcoords[i], anisous[i].eigvecs[2]), 2) / anisous[i].eigvals[2];
                        energy += (energy0 + energy1 + energy2);
                    }
                    return energy;
                }
            }
            public double GetEnergyFromAnisous(IList<Vector> coordsto)
            {
                HDebug.Assert(size == coordsto.Count);
                Vector[] dcoords = new Vector[size];
                for(int i=0; i<size; i++)
                    dcoords[i] = coords[i] - coordsto[i];
                {
                    double energy = 0;
                    for(int i=0; i<size; i++)
                        energy += LinAlg.VtMV(dcoords[i], hess[i, i], dcoords[i]);
                    return energy;
                }
            }
            public double GetEnergyFromFull(IList<Vector> coordsto)
            {
                HDebug.Assert(size == coordsto.Count);
                Vector[] dcoords = new Vector[size];
                {
                    for(int i=0; i<size; i++)
                        dcoords[i] = coords[i] - coordsto[i];
                }

                Mode[] modes = GetModes();

                {
                    double energy = 0;
                    Vector ndcoords = new double[size*3];
                    double[] energies = new double[modes.Length];
                    for(int i=0; i<size*3; i++)
                        ndcoords[i] = dcoords[i/3][i%3];
                    for(int i=0; i<6; i++)
                        energies[i] = 0;
                    //for(int i=6; i<eigvecs.Length; i++)
                    System.Threading.Tasks.Parallel.For(6, modes.Length, delegate(int m)
                    {
                        double eigval = modes[m].eigval;
                        Vector eigvec = modes[m].eigvec;
                        energies[m] = Math.Pow(LinAlg.VtV(ndcoords, eigvec), 2)*eigval;
                    });
                    energy = energies.Mean();
                    energy = energies.Sum();
                    return energy;
                }
            }
        }
    }
}
