using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        static bool GetHess4PwIntrAct_doselftest = HDebug.IsDebuggerAttached;
        public bool GetHess4PwIntrAct_selftest(List<ForceField.IForceField> frcflds, double frcfactor)
        {
            Vector[] coords = GetCoords();
            Vector[] forces  = GetVectorsZero();
            MatrixByArr[,] hessian = new MatrixByArr[size,size];
            {
                for(int c=0; c<size; c++)
                    for(int r=0; r<size; r++)
                        hessian[c, r] = new double[3, 3];
            }
            Dictionary<string, object> cache = new Dictionary<string,object>();
            cache.Add("all nonbondeds",null);
            PwForceDecomposer forceij = null;
            double energy = GetPotentialNonbondeds(frcflds, coords, ref forces, ref hessian, cache, forceij);

            List<ForceField.IForceField> frcflds_nonbondeds = new List<ForceField.IForceField>();
            foreach(ForceField.INonbonded frcfld_nonbonded in SelectInFrcflds(frcflds, new List<ForceField.INonbonded>()))
                frcflds_nonbondeds.Add(frcfld_nonbonded);

            MatrixByArr[,] hess = GetHess4PwIntrAct(frcflds_nonbondeds, frcfactor);
            if(hess.GetLength(0) != hessian.GetLength(0)) return false;
            if(hess.GetLength(1) != hessian.GetLength(1)) return false;
            for(int i=0; i<hess.GetLength(0); i++)
                for(int j=0; j<hess.GetLength(1); j++)
                {
                    if(i==j)
                        continue;
                    if(hess[i,j].ColSize != hessian[i,j].ColSize) return false;
                    if(hess[i,j].RowSize != hessian[i,j].RowSize) return false;
                    if(HDebug.CheckTolerance(0.00000001, hess[i,j] - hessian[i,j]) == false)
                        return false;
                }
            return true;
        }
        public MatrixByArr[,] GetHess4PwIntrAct(List<ForceField.IForceField> frcflds, double frcfactor)
        {
            if(GetHess4PwIntrAct_doselftest)
            {
                GetHess4PwIntrAct_doselftest = false;
                HDebug.Assert(GetHess4PwIntrAct_selftest(frcflds, frcfactor));
            }

            Vector[] coords = GetCoords();
            Dictionary<Pair<int,int>,ForceField.PwIntrActInfo> pwintractinfos = new Dictionary<Pair<int,int>,ForceField.PwIntrActInfo>();
            for(int i=0; i<size-1; i++)
                for(int j=i+1; j<size; j++)
                    pwintractinfos.Add(new Pair<int, int>(i, j), new ForceField.PwIntrActInfo());
            GetHess4PwIntrActBonds(frcflds, coords, pwintractinfos);
            GetHess4PwIntrActAngles(frcflds, coords, pwintractinfos);
            GetHess4PwIntrActImpropers(frcflds, coords, pwintractinfos);
            GetHess4PwIntrActNonbondeds(frcflds, coords, pwintractinfos);

            MatrixByArr[,] hessblk = new MatrixByArr[size, size];
            Vector[] frcs = new Vector[size];
            for(int i=0; i<size; i++)
            {
                hessblk[i, i] = new double[3, 3];
                frcs[i] = new double[3];
            }
            foreach(Pair<int,int> key in pwintractinfos.Keys)
            {
                int i = key.Item1;
                int j = key.Item2;
                ForceField.PwIntrActInfo pwintractinfo = pwintractinfos[key];
                double fij = pwintractinfo.Fij * frcfactor;
                double kij = pwintractinfo.Kij;
                hessblk[i, j] = ForceField.GetHessianBlock(coords[i], coords[j], kij, fij);
                hessblk[j, i] = ForceField.GetHessianBlock(coords[i], coords[j], kij, fij);
                hessblk[i, i] -= hessblk[i, j];
                hessblk[j, j] -= hessblk[j, i];
                /////////////////////////////////////////
                Vector uvec_ij = (coords[j] - coords[i]).UnitVector();
                frcs[i] +=  uvec_ij * fij;
                frcs[j] += -uvec_ij * fij;
            }

            double[] frcmags = new double[size];
            for(int i=0; i<size; i++)
                frcmags[i] = frcs[i].Dist;
            double frcmag_max = frcmags.Max();
            double frcmag_avg = frcmags.Average();
            double frcmag_med = frcmags.Median();

            return hessblk;
        }
        public void GetHess4PwIntrActBonds(List<ForceField.IForceField> frcflds, Vector[] coords, Dictionary<Pair<int,int>,ForceField.PwIntrActInfo> pwintractinfos)
        {
            List<ForceField.IBond> frcfld_bonds = SelectInFrcflds(frcflds, new List<ForceField.IBond>());

            Vector[] lcoords = new Vector[2];
            Vector[] lforces = new Vector[2];
            for(int i=0; i<bonds.Count; i++)
            {
                int id0 = bonds[i].atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = bonds[i].atoms[1].ID; lcoords[1] = coords[id1];
                int[] ids = new int[] { id0, id1 };
                foreach(ForceField.IHessBuilder4PwIntrAct frcfld in frcfld_bonds)
                {
                    Pair<int, int>[] lpwidxs;
                    ForceField.PwIntrActInfo[] lpwintractinfos;
                    frcfld.BuildHess4PwIntrAct(bonds[i], lcoords, out lpwidxs, out lpwintractinfos);
                    // rearrange index as {(min1,max1), (min2,max2), ... }
                    for(int j=0; j<lpwidxs.Length; j++)
                    {
                        int idxmin = Math.Min(ids[lpwidxs[j].Item1], ids[lpwidxs[j].Item2]);
                        int idxmax = Math.Max(ids[lpwidxs[j].Item1], ids[lpwidxs[j].Item2]);
                        Pair<int, int> key = new Pair<int, int>(idxmin, idxmax);
                        pwintractinfos[key] += lpwintractinfos[j];
                    }
                }
            }
        }
        public void GetHess4PwIntrActAngles(List<ForceField.IForceField> frcflds, Vector[] coords, Dictionary<Pair<int, int>, ForceField.PwIntrActInfo> pwintractinfos)
        {
            List<ForceField.IAngle> frcfld_angles = SelectInFrcflds(frcflds, new List<ForceField.IAngle>());

            Vector[] lcoords = new Vector[3];
            Vector[] lforces = new Vector[3];
            for(int i=0; i<angles.Count; i++)
            {
                int id0 = angles[i].atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = angles[i].atoms[1].ID; lcoords[1] = coords[id1];
                int id2 = angles[i].atoms[2].ID; lcoords[2] = coords[id2];
                int[] ids = new int[] { id0, id1, id2 };
                foreach(ForceField.IHessBuilder4PwIntrAct frcfld in frcfld_angles)
                {
                    Pair<int, int>[] lpwidxs;
                    ForceField.PwIntrActInfo[] lpwintractinfos;
                    frcfld.BuildHess4PwIntrAct(angles[i], lcoords, out lpwidxs, out lpwintractinfos);
                    // rearrange index as {(min1,max1), (min2,max2), ... }
                    for(int j=0; j<lpwidxs.Length; j++)
                    {
                        int idxmin = Math.Min(ids[lpwidxs[j].Item1], ids[lpwidxs[j].Item2]);
                        int idxmax = Math.Max(ids[lpwidxs[j].Item1], ids[lpwidxs[j].Item2]);
                        Pair<int, int> key = new Pair<int, int>(idxmin, idxmax);
                        pwintractinfos[key] += lpwintractinfos[j];
                    }
                }
            }
        }
        public void GetHess4PwIntrActImpropers(List<ForceField.IForceField> frcflds, Vector[] coords, Dictionary<Pair<int, int>, ForceField.PwIntrActInfo> pwintractinfos)
        {
            List<ForceField.IImproper> frcfld_impropers = SelectInFrcflds(frcflds, new List<ForceField.IImproper>());

            Vector[] lcoords = new Vector[4];
            Vector[] lforces = new Vector[4];
            for(int i=0; i<impropers.Count; i++)
            {
                int id0 = impropers[i].atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = impropers[i].atoms[1].ID; lcoords[1] = coords[id1];
                int id2 = impropers[i].atoms[2].ID; lcoords[2] = coords[id2];
                int id3 = impropers[i].atoms[3].ID; lcoords[3] = coords[id3];
                int[] ids = new int[] { id0, id1, id2, id3 };
                foreach(ForceField.IHessBuilder4PwIntrAct frcfld in frcfld_impropers)
                {
                    Pair<int, int>[] lpwidxs;
                    ForceField.PwIntrActInfo[] lpwintractinfos;
                    frcfld.BuildHess4PwIntrAct(impropers[i], lcoords, out lpwidxs, out lpwintractinfos);
                    // rearrange index as {(min1,max1), (min2,max2), ... }
                    for(int j=0; j<lpwidxs.Length; j++)
                    {
                        int idxmin = Math.Min(ids[lpwidxs[j].Item1], ids[lpwidxs[j].Item2]);
                        int idxmax = Math.Max(ids[lpwidxs[j].Item1], ids[lpwidxs[j].Item2]);
                        Pair<int, int> key = new Pair<int, int>(idxmin, idxmax);
                        pwintractinfos[key] += lpwintractinfos[j];
                    }
                }
            }
        }
        public void GetHess4PwIntrActNonbondeds(List<ForceField.IForceField> frcflds, Vector[] coords, Dictionary<Pair<int, int>, ForceField.PwIntrActInfo> pwintractinfos)
        {
            List<ForceField.INonbonded> frcfld_nonbondeds = SelectInFrcflds(frcflds, new List<ForceField.INonbonded>());

            Nonbondeds_v1 nonbondeds = null;
            {
                // compute all non-bondeds
                double nonbondeds_maxdist = double.PositiveInfinity;
                nonbondeds = new Nonbondeds_v1(atoms, size, nonbondeds_maxdist);
                nonbondeds.UpdateNonbondeds(coords, 0);
            }

            Vector[] lcoords = new Vector[2];
            Vector[] lforces = new Vector[2];
            foreach(Nonbonded nonbond in nonbondeds)
            {
                int id0 = nonbond.atoms[0].ID; lcoords[0] = coords[id0];
                int id1 = nonbond.atoms[1].ID; lcoords[1] = coords[id1];
                int[] ids = new int[] { id0, id1 };
                foreach(ForceField.IHessBuilder4PwIntrAct frcfld in frcfld_nonbondeds)
                {
                    Pair<int, int>[] lpwidxs;
                    ForceField.PwIntrActInfo[] lpwintractinfos;
                    frcfld.BuildHess4PwIntrAct(nonbond, lcoords, out lpwidxs, out lpwintractinfos);
                    // rearrange index as {(min1,max1), (min2,max2), ... }
                    for(int j=0; j<lpwidxs.Length; j++)
                    {
                        int idxmin = Math.Min(ids[lpwidxs[j].Item1], ids[lpwidxs[j].Item2]);
                        int idxmax = Math.Max(ids[lpwidxs[j].Item1], ids[lpwidxs[j].Item2]);
                        Pair<int, int> key = new Pair<int, int>(idxmin, idxmax);
                        pwintractinfos[key] += lpwintractinfos[j];
                    }
                }
            }
        }
    }
}
