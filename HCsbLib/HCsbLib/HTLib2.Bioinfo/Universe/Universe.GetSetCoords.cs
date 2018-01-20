using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public Vector[] GetCoords()
        {
            Vector[] coords;
            GetCoords(out coords);
            return coords;
        }
        public void GetCoords(out Vector[] coords)
        {
            coords = new Vector[size];
            foreach(Atom atom in atoms)
            {
                HDebug.Assert(coords[atom.ID] == null); // verify that the coord is not assigned yet
                if(atom.Coord == null)
                    continue;
                coords[atom.ID] = atom.Coord.Clone();
            }
        }
        public List<Vector> GetCoords(IList<int> idxs)
        {
            List<Vector> coords = new List<Vector>(idxs.Count);
            for(int i=0; i<idxs.Count; i++)
            {
                int idx = idxs[i];
                coords.Add(atoms[idx].Coord.Clone());
            }
            return coords;
        }
        public List<Vector> GetCoords(params int[] idxs)
        {
            return GetCoords((IList<int>)idxs);
        }
        public void SetCoords(IList<Vector> coords)
        {
            if(coords.Count != size)
                throw new HException();
            bool[] assigned = new bool[size];
            foreach(Atom atom in atoms)
            {
                HDebug.Assert(assigned[atom.ID] == false);
                assigned[atom.ID] = true;
                atom.Coord = coords[atom.ID].Clone();
            }
        }
        public void SaveCoords(string path)
        {
            Vector[] coords = GetCoords();
            SaveCoords(path, coords);
        }
        public void SaveCoords(string path, Vector[] coords)
        {
            HSerialize.Serialize(path, null, coords);
        }
        public void LoadCoords(string path)
        {
            Vector[] coords;
            HSerialize.Deserialize(path, null, out coords);
            SetCoords(coords);
        }
        public void SaveCoordsToPdb(string path, Pdb pdb, Vector[] coords)
        {
            Pdb.Atom[] pdbatoms = pdb.atoms;
            Vector[]   pdbcoords = new Vector[pdbatoms.Length];
            Dictionary<int,int> serial2idx = pdbatoms.ToDictionaryAsSerialToIndex();
            for(int ia=0; ia<size; ia++)
            {
                Universe.Atom uatom = atoms[ia];
                List<Pdb.Atom> patoms = uatom.sources.HSelectByType((Pdb.Atom)null).ToList();
                if(patoms.Count >= 1)
                {
                    HDebug.Assert(patoms.Count == 1);
                    int serial = patoms[0].serial;
                    int idx    = serial2idx[serial];
                    HDebug.Assert(pdbatoms[idx].serial == serial);
                    HDebug.Assert(pdbcoords[idx] == null);
                    pdbcoords[idx] = coords[ia].Clone();
                }
                else
                {
                    HDebug.Assert();
                }
            }

            for(int i=0; i<pdbcoords.Length; i++)
            {
                if(pdbcoords[i] == null)
                {
                    HDebug.Assert(false);
                    pdbcoords[i] = new double[3] { double.NaN, double.NaN, double.NaN };
                }
            }

            pdb.ToFile(path, pdbcoords);
        }
        public void SaveCoordsToPdb(string path)
        {
            SaveCoordsToPdb(path, GetCoords());
        }
        public void SaveCoordsToPdb(string path, Vector[] coords)
        {
            SaveCoordsToPdb(path, pdb, coords);
        }
        public void SaveCoordsToPdb(string path, Pdb pdb)
        {
            SaveCoordsToPdb(path, pdb, GetCoords());
        }
        public void _SaveCoordsToPdb(string path, Vector[] coords)
        {
            System.Console.Error.WriteLine("Check if 'Universe._SaveCoordsToPdb(string path, Vector[] coords)' is correct");
            HDebug.ToDo();
            pdb.ToFile(path, coords);
        }
        public void _SaveCoordsToPdb(string path)
        {
            _SaveCoordsToPdb(path, GetCoords());
        }
        public bool LoadCoordsFromPdb(string path)
        {
            Pdb pdb = Pdb.FromFile(path);

            HDebug.Assert(this.pdb.atoms.Length == pdb.atoms.Length);
            HDebug.Assert(this.size == pdb.atoms.Length);
            Vector[] coords = new Vector[size];
            for(int i=0; i<size; i++)
            {
                if(this.pdb.atoms[i].name    != pdb.atoms[i].name   ) return false;
                if(this.pdb.atoms[i].resName != pdb.atoms[i].resName) return false;
                coords[i] = pdb.atoms[i].coord;
            }
            SetCoords(coords);
            return true;
        }
    }
}
