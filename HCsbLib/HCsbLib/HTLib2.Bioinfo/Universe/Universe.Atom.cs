using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Universe.Atom;
    using Bond = Universe.Bond;

	public partial class Universe
	{
        public partial class Atom
		{
            public const int MaxID = 46340; // 46340*46340 + 46340=2147441940 < 2147483647=int.MaxValue
                                            // upperID     lowerID
            public static int GetHashCode(IList<Atom> atoms)
            {
                //int hash = atoms[0].PdbAtom.serial;
                //for(int i=1; i<atoms.Count; i++)
                //    hash = hash * MaxID + atoms[i].PdbAtom.serial;
                //return hash;
                int hash = atoms[0].ID;
                for(int i=1; i<atoms.Count; i++)
                    hash = hash * MaxID + atoms[i].ID;
                return hash;
            }
            int _ID = -1;
            public readonly string _ResidueId = null;
            public int ID { get { return _ID; } }

            public int    AtomId      = -1        ;// { get { return PsfAtom.AtomId     ; } }
            public string AtomName    = ""        ;// { get { return PsfAtom.AtomName   ; } }
            public string AtomType    = ""        ;// { get { return PsfAtom.AtomType   ; } }
            public string AtomElem    = null      ;// { get { return PdbAtom.element    ; } }
            public string ResidueId      { get { if(_ResidueId==null) return ResiduePdbId.ToString(); return _ResidueId; } }
            public int    ResiduePdbId   { get { HDebug.Assert(); var pdbatom = sources_PdbAtom(); HDebug.Assert(pdbatom != null); return pdbatom.resSeq; } }
            public string ResidueName = ""        ;// { get { return PsfAtom.ResidueName; } }
            public double Charge      = double.NaN;// { get { return PsfAtom.Charge     ; } }
            public double Mass        = double.NaN;// { get { return PsfAtom.Mass       ; } }
			public double epsilon     = double.NaN;// { get { return PrmNonbonded.epsilon ; } }
			public double Rmin2       = double.NaN;// { get { return PrmNonbonded.Rmin2   ; } }
			public double eps_14      = double.NaN;// { get { return PrmNonbonded.eps_14  ; } }
            public double Rmin2_14    = double.NaN;// { get { return PrmNonbonded.Rmin2_14; } }

            public bool IsHydrogen()
            {
                HDebug.ToDo(); // check
                //return ((AtomType[0] == 'H') || (AtomType[0] == 'h'));
                return ((AtomElem == "H") || (AtomElem == "h"));
            }
            public bool IsWater()
            {
                if(ResidueName.Trim() == "TIP3") return true;

                var prm = sources.HFirstByType(null as Tinker.Prm.Atom);
                if(prm != null)
                {
                    bool water = (prm.Description.ToUpper().Contains("TIP3P") || prm.Description.ToUpper().Contains("WATER"));
                    return water;
                }
                return false;
            }

///         public bool IsVirtual = false;
            public Vector Coord;
///         public Vector _Coord;
///         public Vector Coord
///         {
///             get { if(IsVirtual) return null; return _Coord; }
///             set { if(IsVirtual) _Coord = null; else _Coord = value; }
///         }

            public readonly object[] sources;
            public T[] sources_ListType<T>()
                where T : class
            {
                return sources.HSelectByType(null as T).ToArray();
            }
            public Pdb.Atom[] sources_ListPdbAtom() { return sources_ListType<Pdb.Atom>(); }
            public Pdb.Atom   sources_PdbAtom()
            {
                var pdbatoms = sources.HSelectByType(null as Pdb.Atom).ToArray();
                if(pdbatoms.Length == 0) return null;
                HDebug.Assert(pdbatoms.Length == 1);
                return pdbatoms[0];
            }
            public Pdb.Atom   correspond_PdbAtom()
            {
                Pdb.Atom pdbatom = sources_PdbAtom();
                if(pdbatom != null)
                    return pdbatom;

                HashSet<Atom> visiteds = new HashSet<Atom>();
                visiteds.Add(this);
                List<Atom> tovisit = new List<Atom>();
                tovisit.AddRange(Inter12);
                tovisit.AddRange(Inter123);
                tovisit.AddRange(Inter14);

                foreach(var link in tovisit)
                {
                    if(visiteds.Contains(link))
                        continue;
                    pdbatom = link.sources_PdbAtom();
                    if(pdbatom != null)
                        return pdbatom;
                    visiteds.Add(link);
                }

                return null;
                //HDebug.Assert(false);
                throw new NotImplementedException();
            }
            //public readonly Pdb.Atom            PdbAtom;
            //public readonly Namd.Psf.Atom       PsfAtom;
            //public readonly Namd.Prm.Nonbonded  PrmNonbonded;
			public List<Bond     > Bonds      = new List<Bond     >();
			public List<Angle    > Angles     = new List<Angle    >();
			public List<Dihedral > Dihedrals  = new List<Dihedral >();
			public List<Improper > Impropers  = new List<Improper >();
            //public List<Nonbonded> Nonbondeds = new List<Nonbonded>();
            public List<Nonbonded14> Nonbonded14s = new List<Nonbonded14>();
			public HashSet<Atom>  Inter12   = new HashSet<Atom >();
			public HashSet<Atom>  Inter123  = new HashSet<Atom >();
			public HashSet<Atom>  Inter14   = new HashSet<Atom >();
                   HashSet<Atom>  _Inter01234 = null;
            public HashSet<Atom>  Inter01234
            {
                get
                {
                    if(_Inter01234 == null)
                    {
                        _Inter01234 = Enumerable.Union(Inter123, Inter14).HToHashSet();
                        _Inter01234.Add(this);
                    }
                    return _Inter01234;
                }
            }
            public List<Atom>[] ListInterAtom12()   { return ListInterAtomN(2); }
            public List<Atom>[] ListInterAtom123()  { return ListInterAtomN(3); }
            public List<Atom>[] ListInterAtom1234() { return ListInterAtomN(4); }
            public List<Atom>[] ListInterAtomN(int n)
            {
                if(n <= 0) throw new Exception();
                if(n == 1) return new List<Atom>[] { (new Atom[] { this }).ToList() };

                List<List<Atom>> list = new List<List<Atom>>();
                foreach(var bond in Bonds)
                {
                    Atom other = (bond.atoms[0] == this) ? bond.atoms[1] :  bond.atoms[0];
                    HDebug.Assert(object.ReferenceEquals(this, other) == false);

                    List<Atom>[] listsub = other.ListInterAtomN(n-1);
                    foreach(List<Atom> sub in listsub)
                    {
                        if(sub.Contains(this))
                            continue;
                        sub.Insert(0, this);
                        list.Add(sub);
                    }
                }
                return list.ToArray();
            }

            public Atom( int AtomId, string AtomName, string AtomType, string AtomElem  
                       , int ResidueId, string ResidueName
                       , double Charge, double Mass
                       , double epsilon, double Rmin2, double eps_14, double Rmin2_14
                       , params object[] sources
                       )
            {
                this.AtomId      = AtomId     ;     //this.AtomId      = PsfAtom.AtomId     ;
                this.AtomName    = AtomName   ;     //this.AtomName    = PsfAtom.AtomName   ;
                this.AtomType    = AtomType   ;     //this.AtomType    = PsfAtom.AtomType   ;
                this.AtomElem    = AtomElem   ;     //this.AtomElem    = PdbAtom.element    ;
                this.ResidueName = ResidueName;     //this.ResidueName = PsfAtom.ResidueName;
                this.Charge      = Charge     ;     //this.Charge      = PsfAtom.Charge     ;
                this.Mass        = Mass       ;     //this.Mass        = PsfAtom.Mass       ;
                this.epsilon     = epsilon    ;     //this.epsilon     = Nonbonded.epsilon  ;
                this.Rmin2       = Rmin2      ;     //this.Rmin2       = Nonbonded.Rmin2    ;
                this.eps_14      = eps_14     ;     //this.eps_14      = Nonbonded.eps_14   ;
                this.Rmin2_14    = Rmin2_14   ;     //this.Rmin2_14    = Nonbonded.Rmin2_14 ;
                this.sources     = sources    ;
            }
            public Atom(Namd.Psf.Atom PsfAtom, Namd.Prm.Nonbonded Nonbonded, Pdb.Atom PdbAtom)
			{
                //Debug.Assert(ID <= MaxID);
                //this._ID = ID;
                //this.PdbAtom = PdbAtom;
				//this.PsfAtom = PsfAtom;
				//this.PrmNonbonded = Nonbonded;

                this.AtomId      = PsfAtom.AtomId     ;
                this.AtomName    = PsfAtom.AtomName   ;
                this.AtomType    = PsfAtom.AtomType   ;
                this.AtomElem    = PdbAtom.element.Trim();
                this._ResidueId  = string.Format("({0},{1},{2})", PsfAtom.SegmentName, PsfAtom.ResidueId.Item1, PsfAtom.ResidueId.Item2);
                this.ResidueName = PsfAtom.ResidueName;
                this.Charge      = PsfAtom.Charge     ;
                this.Mass        = PsfAtom.Mass       ;
                this.epsilon     = Nonbonded.epsilon  ;
                this.Rmin2       = Nonbonded.Rmin2    ;
                this.eps_14      = Nonbonded.eps_14   ;
                this.Rmin2_14    = Nonbonded.Rmin2_14 ;
                this.sources     = new object[] { PdbAtom, PsfAtom, Nonbonded };
///             this.IsVirtual   = (PdbAtom.occupancy < 0) ? true : false;
			}
            public class CGromToCharmm { public double Rmin2, eps, Rmin2_14, eps_14; }
            public CGromToCharmm ConvertGromacsToCharmm
            {
                get
                {
                    /// http://gromacs.5086.x6.nabble.com/The-problem-of-converting-CGenff-parameters-to-those-of-Gromacs-td5002042.html
                    /// charmm forcefields specify epsilon and sigmas so you only need to convert them: 
                    ///   gromacs(sigma)   = (charmm(Rmin/2)/10) * (2/(2^(1/6))) 
                    ///   gromacs(epsilon) = charmm(eps) * 4.184 
                    /// For 1-4 pair interactions, 
                    ///   gromacs(sigma1,4 i,j) = (charmm(Rmin/2_1,4_i) + charmm(Rmin/2_1,4_j))/(2^1/6) 
                    ///   gromacs(eps1,4 i,j)   = sqrt(charmm(eps_1,4_i) * charmm(eps_1,4_j)) * 4.184 
                    ///   
                    /// charmm forcefields specify epsilon and sigmas so you only need to convert them: 
                    ///   charmm(Rmin/2) = gromacs(sigma) * 10 / (2/(2^(1/6)))
                    ///   charmm(eps)    = gromacs(epsilon) / 4.184 
                    /// For 1-4 pair interactions, 
                    ///   gromacs(sigma1,4 i,j) = (charmm(Rmin/2_1,4_i) + charmm(Rmin/2_1,4_j))/(2^1/6) 
                    ///   gromacs(eps1,4 i,j)   = sqrt(charmm(eps_1,4_i) * charmm(eps_1,4_j)) * 4.184 
                    double gromacs_sigma   = Rmin2;
                    double gromacs_epsilon = epsilon;
                    double gromacs_sigma_14   = Rmin2_14;
                    double gromacs_epsilon_14 = eps_14;
                    HDebug.Assert(double.IsNaN(gromacs_sigma_14));
                    HDebug.Assert(double.IsNaN(gromacs_epsilon_14));

                    double charmm_Rmin2    = gromacs_sigma   * 10 / (2.0/Math.Pow(2, 1.0/6));
                    double charmm_eps      = gromacs_epsilon / 4.184;
                    double charmm_Rmin2_14 = gromacs_sigma_14   * 10 / (2.0/Math.Pow(2, 1.0/6));
                    double charmm_eps_14   = gromacs_epsilon_14 / 4.184;
                    return new CGromToCharmm{ Rmin2 = charmm_Rmin2
                                            , eps   = charmm_eps
                                            , Rmin2_14 = charmm_Rmin2_14
                                            , eps_14   = charmm_eps_14
                                            };
                    
                }
            }
            public void AssignID(int ID)
            {
                this._ID = ID;
            }
			override public string ToString()
			{
				string str = "";
				str += ID + " ";
                str += ", " + AtomId    + ":" + AtomName;
				try { str += ", " + ResidueId + ":" + ResidueName; } catch { }
                str += ", chrg(" + Charge + ")";
				return str;
			}
            public void Isolate(Universe univ)
            {
                foreach(Bond        bond        in new List<Bond       >(Bonds       )) { foreach(Atom atom in bond       .atoms) if(atom != this) HDebug.Verify(atom.Bonds       .Remove(bond       )); HDebug.Verify(Bonds       .Remove(bond       )); HDebug.Verify(univ.bonds.Remove       (bond       )); }
                foreach(Angle       angle       in new List<Angle      >(Angles      )) { foreach(Atom atom in angle      .atoms) if(atom != this) HDebug.Verify(atom.Angles      .Remove(angle      )); HDebug.Verify(Angles      .Remove(angle      )); HDebug.Verify(univ.angles.Remove      (angle      )); }
                foreach(Dihedral    dihedral    in new List<Dihedral   >(Dihedrals   )) { foreach(Atom atom in dihedral   .atoms) if(atom != this) HDebug.Verify(atom.Dihedrals   .Remove(dihedral   )); HDebug.Verify(Dihedrals   .Remove(dihedral   )); HDebug.Verify(univ.dihedrals.Remove   (dihedral   )); }
                foreach(Improper    improper    in new List<Improper   >(Impropers   )) { foreach(Atom atom in improper   .atoms) if(atom != this) HDebug.Verify(atom.Impropers   .Remove(improper   )); HDebug.Verify(Impropers   .Remove(improper   )); HDebug.Verify(univ.impropers.Remove   (improper   )); }
                foreach(Nonbonded14 nonbonded14 in new List<Nonbonded14>(Nonbonded14s)) { foreach(Atom atom in nonbonded14.atoms) if(atom != this) HDebug.Verify(atom.Nonbonded14s.Remove(nonbonded14)); HDebug.Verify(Nonbonded14s.Remove(nonbonded14)); HDebug.Verify(univ.nonbonded14s.Remove(nonbonded14)); }
                //public List<Nonbonded> Nonbondeds = new List<Nonbonded>();
                foreach(Atom atom in new List<Atom>(Inter12)) { HDebug.Assert(atom != this); HDebug.Verify(atom.Inter12.Remove(this)); HDebug.Verify(Inter12.Remove(atom)); }
                foreach(Atom atom in new List<Atom>(Inter123)) { HDebug.Assert(atom != this); HDebug.Verify(atom.Inter123.Remove(this)); HDebug.Verify(Inter123.Remove(atom)); }
                foreach(Atom atom in new List<Atom>(Inter14 )) { HDebug.Assert(atom != this); HDebug.Verify(atom.Inter14 .Remove(this)); HDebug.Verify(Inter14 .Remove(atom)); }
            }

            public int GetValence()
            {
                HDebug.Assert(false);
                Pdb.Atom PdbAtom = null;

                switch(PdbAtom.element)
                {
                    case " H": return 1;
                    case " N": return 3;
                    case " C": return 4;
                    case " O": return 2;
                    //case " S": return 
                }
                HDebug.Assert(false);
                return -1;
            }

            public static int CompareById(Atom x, Atom y)
            {
                return (x.ID - y.ID);
            }
        }
        
        //public abstract class IGetAtoms
        //{
        //    public abstract IList<Atom> GetAtoms();
        //    public string ToString()
        //    {
        //        StringBuilder str = new StringBuilder();
        //        IList<Atom> atoms = GetAtoms();
        //        str.Append("{" + atoms[0].ToString());
        //        for(int i=1; i<atoms.Count; i++)
        //            str.Append("}-{" + atoms[i].ToString());
        //        str.Append("}");
        //        return str.ToString();
        //    }
        //}
    }
}
