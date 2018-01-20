using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Gromacs
    {
        public partial class Top
        {
            /// http://manual.gromacs.org/online/top.html
            ///
            #region description
            /// ;
            /// ;    Example topology file
            /// ;
            /// [ defaults ]
            /// ; nbfunc        comb-rule       gen-pairs       fudgeLJ fudgeQQ
            ///   1             1               no              1.0     1.0
            /// 
            /// ; The force field files to be included
            /// #include "rt41c5.itp"
            /// 
            /// [ moleculetype ]
            /// ; name  nrexcl
            /// Urea         3
            /// 
            /// [ atoms ]
            /// ;   nr    type   resnr  residu    atom    cgnr  charge
            ///      1       C       1    UREA      C1       1   0.683
            ///      2       O       1    UREA      O2       1  -0.683
            ///      3      NT       1    UREA      N3       2  -0.622
            ///      4       H       1    UREA      H4       2   0.346
            ///      5       H       1    UREA      H5       2   0.276
            ///      6      NT       1    UREA      N6       3  -0.622
            ///      7       H       1    UREA      H7       3   0.346
            ///      8       H       1    UREA      H8       3   0.276
            /// 
            /// [ bonds ]
            /// ;  ai    aj funct           c0           c1
            ///     3     4     1 1.000000e-01 3.744680e+05 
            ///     3     5     1 1.000000e-01 3.744680e+05 
            ///     6     7     1 1.000000e-01 3.744680e+05 
            ///     6     8     1 1.000000e-01 3.744680e+05 
            ///     1     2     1 1.230000e-01 5.020800e+05 
            ///     1     3     1 1.330000e-01 3.765600e+05 
            ///     1     6     1 1.330000e-01 3.765600e+05 
            /// 
            /// [ pairs ]
            /// ;  ai    aj funct           c0           c1
            ///     2     4     1 0.000000e+00 0.000000e+00 
            ///     2     5     1 0.000000e+00 0.000000e+00 
            ///     2     7     1 0.000000e+00 0.000000e+00 
            ///     2     8     1 0.000000e+00 0.000000e+00 
            ///     3     7     1 0.000000e+00 0.000000e+00 
            ///     3     8     1 0.000000e+00 0.000000e+00 
            ///     4     6     1 0.000000e+00 0.000000e+00 
            ///     5     6     1 0.000000e+00 0.000000e+00 
            /// 
            /// [ angles ]
            /// ;  ai    aj    ak funct           c0           c1
            ///     1     3     4     1 1.200000e+02 2.928800e+02 
            ///     1     3     5     1 1.200000e+02 2.928800e+02 
            ///     4     3     5     1 1.200000e+02 3.347200e+02 
            ///     1     6     7     1 1.200000e+02 2.928800e+02 
            ///     1     6     8     1 1.200000e+02 2.928800e+02 
            ///     7     6     8     1 1.200000e+02 3.347200e+02 
            ///     2     1     3     1 1.215000e+02 5.020800e+02 
            ///     2     1     6     1 1.215000e+02 5.020800e+02 
            ///     3     1     6     1 1.170000e+02 5.020800e+02 
            /// 
            /// [ dihedrals ]
            /// ;  ai    aj    ak    al funct           c0           c1           c2
            ///     2     1     3     4     1 1.800000e+02 3.347200e+01 2.000000e+00 
            ///     6     1     3     4     1 1.800000e+02 3.347200e+01 2.000000e+00 
            ///     2     1     3     5     1 1.800000e+02 3.347200e+01 2.000000e+00 
            ///     6     1     3     5     1 1.800000e+02 3.347200e+01 2.000000e+00 
            ///     2     1     6     7     1 1.800000e+02 3.347200e+01 2.000000e+00 
            ///     3     1     6     7     1 1.800000e+02 3.347200e+01 2.000000e+00 
            ///     2     1     6     8     1 1.800000e+02 3.347200e+01 2.000000e+00 
            ///     3     1     6     8     1 1.800000e+02 3.347200e+01 2.000000e+00 
            /// 
            /// [ dihedrals ]
            /// ;  ai    aj    ak    al funct           c0           c1
            ///     3     4     5     1     2 0.000000e+00 1.673600e+02 
            ///     6     7     8     1     2 0.000000e+00 1.673600e+02 
            ///     1     3     6     2     2 0.000000e+00 1.673600e+02 
            /// 
            /// ; Include SPC water topology
            /// #include "spc.itp"
            /// 
            /// [ system ]
            /// Urea in Water
            /// 
            /// [ molecules ]
            /// Urea    1
            /// SOL     1000
            #endregion
            public class LineElement
            {
                public char[] defsep { get { return new char[] { ' ', '\t', ',', '\\' , '\n' }; } }
                public readonly string line;
                public readonly object source;
                public LineElement(string line, object source) { this.line = line; this.source = source; }
                public override string ToString() { return line; }

                public string GetString (int idx, char[] sep=null) { if(sep == null) sep = defsep; return line.Split(sep, StringSplitOptions.RemoveEmptyEntries).ElementAt(idx); }
                public int    GetInteger(int idx, char[] sep=null) { if(sep == null) sep = defsep; return int   .Parse(GetString(idx,sep)); }
                public double GetDouble (int idx, char[] sep=null) { if(sep == null) sep = defsep; return double.Parse(GetString(idx,sep)); }
                public bool   GetBoolean(int idx, char[] sep=null) { if(sep == null) sep = defsep; return bool  .Parse(GetString(idx,sep)); }

                public static string GetType(string line)
                {
                    string type = line;
                    if(type.Length == 0)
                        return null;
                    int idx_comt = type.IndexOf(';');
                    if(idx_comt != -1)
                        type = type.Substring(0, idx_comt);
                    int idx_begin = type.IndexOf('[');
                    int idx_end   = type.IndexOf(']');
                    if(idx_begin == -1 || idx_end == -1)
                        return null;
                    type = type.Substring(idx_begin+1, idx_end-idx_begin-1).Trim();
                    return type;
                }
                //public static string GetStringKey(params string[] values)
                //{
                //    if(string.Compare(values.First(), values.Last()) > 0)
                //        values = values.Reverse();
                //    string key = values[0];
                //    for(int i=1; i<values.Length; i++)
                //        key += "-" + values[i];
                //    return key;
                //}
            }
            //public interface IStringKey
            //{
            //    string GetStringKey();
            //}
            public class Defaults : LineElement
            {
                public Defaults(string line, object source) : base(line, source) { }
                // [ defaults ]
                // ; nbfunc        comb-rule       gen-pairs       fudgeLJ fudgeQQ
                //   1             1               no              1.0     1.0
                public int    nbfunc    { get { return GetInteger(0); } }
                public int    comb_rule { get { return GetInteger(1); } }
                public bool   gen_pairs { get { return GetBoolean(2); } }
                public double fudgeLJ   { get { return GetDouble (3); } }
                public double fudgeQQ   { get { return GetDouble (4); } }
                public override string ToString() { return base.ToString(); }
            }
            public class Moleculetype : LineElement
            {
                public Moleculetype(string line, object source) : base(line, source) { }
                // [ moleculetype ]
                // ; name  nrexcl
                // Urea         3
                public string name   { get { return GetString (0); } }
                public int    nrexcl { get { return GetInteger(1); } }
                public override string ToString() { return base.ToString(); }
            }
            public class Atom : LineElement
            {
                public Atom(string line, object source) : base(line, source) { }
                // [ atoms ]
                // ;   nr    type   resnr  residu    atom    cgnr  charge
                //      1       C       1    UREA      C1       1   0.683
                // ;   nr       type  resnr residue  atom   cgnr     charge       mass  typeB    chargeB      massB
                //      1        NH3      1    MET      N      1       -0.3     14.007   ; qtot -0.3
                public int    nr     { get { return GetInteger( 0); } }
                public string type   { get { return GetString ( 1); } }
                public int    resnr  { get { return GetInteger( 2); } }
                public string residu { get { return GetString ( 3); } }
                public string atom   { get { return GetString ( 4); } }
                public int    cgnr   { get { return GetInteger( 5); } }
                public double charge { get { return GetDouble ( 6); } }
                public double mass   { get { return GetDouble ( 7); } }
                public string typeB  { get { return GetString ( 8); } }
                public double chargeB{ get { return GetDouble ( 9); } }
                public double massB  { get { return GetDouble (10); } }
                public override string ToString()
                {
                    string str = "";
                    str += nr;
                    str += ", " + atom + "(" + type +")";
                    str += ", " + residu + resnr;
                    str += ", mass(" + mass + ")";
                    str += ", chrg(" + charge + ")";
                    return str;
                }
            }
            public class Atomtypes : LineElement, Universe.IHKeyStrings//, IStringKey
            {
                public Atomtypes(string line, object source) : base(line, source) { }
                // [ atomtypes ]
                // ;name    at.num  mass        charge  ptype   sigma           epsilon
                // C        6       12.01100    0.51    A       0.356359487256  0.46024
                // CD       6       12.01100    0.000   A       0.356359487256  0.29288     ; partial charge def not found
                public string name   { get { return GetString ( 0); } }
                public int    at_num { get { return GetInteger( 1); } }
                public double mass   { get { return GetDouble ( 2); } }
                public double charge { get { return GetDouble ( 3); } }
                public string ptype  { get { return GetString ( 4); } }
                public double sigma  { get { return GetDouble ( 5); } }
                public double epsilon{ get { return GetDouble ( 6); } }
                //public string GetStringKey() { return name; }
                public string[] GetKeyStrings() { return new string[] { name }; }
                public override string ToString()
                {
                    string str ="";
                    str += name;
                    str += ", mass("+mass+")";
                    str += ", chrg("+charge+")";
                    str += ", epsi("+epsilon+")";
                    str += ", sigm("+sigma+")";
                    return str;
                }
            }
            public class Pairtypes : LineElement
            {
                public Pairtypes(string line, object source) : base(line, source) { }
                // [ pairtypes ]
                // ; i  j       func    sigma1-4        epsilon1-4  ; THESE ARE 1-4 INTERACTIONS
                // CP1  CP1	    1       0.338541512893  0.04184
                public string i         { get { return GetString ( 0); } }
                public string j         { get { return GetString ( 1); } }
                public int    func      { get { return GetInteger( 2); } }
                public double sigma14   { get { return GetDouble ( 3); } }
                public double epsilon14 { get { return GetDouble ( 4); } }
                public override string ToString() { return base.ToString(); }
            }
            public class Bondtypes : LineElement, Universe.IHKeyStrings//IStringKey, IEquatable<Bondtypes>
            {
                public Bondtypes(string line, object source) : base(line, source) { }
                // [ bondtypes ]
                // ; i     j       func    b0      kb
                // CST     OST     1       0.116   784884.928
                public string i         { get { return GetString ( 0); } }
                public string j         { get { return GetString ( 1); } }
                public int    func      { get { return GetInteger( 2); } }
                public double b0        { get { return GetDouble ( 3); } }
                public double kb        { get { return GetDouble ( 4); } }
                //public string GetStringKey() { return GetStringKey(i, j); }
                //public bool Equals(Bondtypes other) { return (Equals1(other) || Equals2(other)); }
                //public bool Equals1(Bondtypes other) { return ((i    == other.i   )
                //                                            && (j    == other.j   )
                //                                            && (func == other.func)
                //                                            && (b0   == other.b0  )
                //                                            && (kb   == other.kb  )
                //                                            );}
                //public bool Equals2(Bondtypes other) { return ((i    == other.j   )
                //                                            && (j    == other.i   )
                //                                            && (func == other.func)
                //                                            && (b0   == other.b0  )
                //                                            && (kb   == other.kb  )
                //                                            );}
                public string[] GetKeyStrings() { return new string[] { i, j }; }
                public override string ToString() { return base.ToString(); }
            }
            public class Constrainttypes : LineElement
            {
                public Constrainttypes(string line, object source) : base(line, source) { }
                public override string ToString() { return base.ToString(); }
                //[ constrainttypes ]
                //; this section is copied from OPLS. In theory we could recalculate
                //; optimal geometries from charmm values, but since pratical equilibrium
                //; geometries do not correspond exactly to these values anyway it is not
                //; worth the effort...
                //MNH3     CT1   2    0.191406
                //MNH3     CT2   2    0.191406
            }
            public class Angletypes : LineElement, Universe.IHKeyStrings//IStringKey, IEquatable<Angletypes>
            {
                public Angletypes(string line, object source) : base(line, source) { }
                // [ angletypes ]
                // ; i      j       k       func    th0         cth         ub0     cub
                // OST      CST     OST	    5       180.0000    25104.0     0.0     0.0
                public string i         { get { return GetString ( 0); } }
                public string j         { get { return GetString ( 1); } }
                public string k         { get { return GetString ( 2); } }
                public int    func      { get { return GetInteger( 3); } }
                public double th0       { get { return GetDouble ( 4); } }
                public double cth       { get { return GetDouble ( 5); } }
                public double ub0       { get { return GetDouble ( 6); } }
                public double cub       { get { return GetDouble ( 7); } }
                //public string GetStringKey() { return GetStringKey(i, j, k); }
                //public bool Equals(Angletypes other) { return (Equals1(other) || Equals2(other)); }
                //public bool Equals1(Angletypes other) { return ((i    == other.i   )
                //                                             && (j    == other.j   )
                //                                             && (k    == other.k   )
                //                                             && (func == other.func)
                //                                             && (th0  == other.th0 )
                //                                             && (cth  == other.cth )
                //                                             && (ub0  == other.ub0 )
                //                                             && (cub  == other.cub )
                //                                             );}
                //public bool Equals2(Angletypes other) { return ((i    == other.k   )
                //                                             && (j    == other.j   )
                //                                             && (k    == other.i   )
                //                                             && (func == other.func)
                //                                             && (th0  == other.th0 )
                //                                             && (cth  == other.cth )
                //                                             && (ub0  == other.ub0 )
                //                                             && (cub  == other.cub )
                //                                             );}
                public string[] GetKeyStrings() { return new string[] { i, j, k }; }
                public override string ToString() { return base.ToString(); }
            }
            public class Dihedraltypes : LineElement, Universe.IHKeyStrings//IStringKey, IEquatable<Dihedraltypes>
            {
                public Dihedraltypes(string line, object source) : base(line, source) { }
                // [ dihedraltypes ]
                // ; i  j   k   l   func    phi0    cp       mult
                // CA   CA  CA  CA  9       180.00  12.9704  2
                public string i         { get { return GetString ( 0); } }
                public string j         { get { return GetString ( 1); } }
                public string k         { get { return GetString ( 2); } }
                public string l         { get { return GetString ( 3); } }
                public int    func      { get { return GetInteger( 4); } }
                public double phi0      { get { return GetDouble ( 5); } }
                public double cp        { get { return GetDouble ( 6); } }
                public double mult      { get { return GetDouble ( 7); } }
                //public string GetStringKey() { return GetStringKey(i, j, k, l); }
                //public bool Equals(Dihedraltypes other) { return (Equals1(other) || Equals2(other)); }
                //public bool Equals1(Dihedraltypes other) { return ((i    == other.i   )
                //                                                && (j    == other.j   )
                //                                                && (k    == other.k   )
                //                                                && (l    == other.l   )
                //                                                && (func == other.func)
                //                                                && (phi0 == other.phi0)
                //                                                && (cp   == other.cp  )
                //                                                && (mult == other.mult)
                //                                                );}
                //public bool Equals2(Dihedraltypes other) { return ((i    == other.l   )
                //                                                && (j    == other.k   )
                //                                                && (k    == other.j   )
                //                                                && (l    == other.i   )
                //                                                && (func == other.func)
                //                                                && (phi0 == other.phi0)
                //                                                && (cp   == other.cp  )
                //                                                && (mult == other.mult)
                //                                                );}
                public string[] GetKeyStrings() { return new string[] { i, j, k, l }; }
                public override string ToString() { return base.ToString(); }
            }
            public class Implicit_genborn_params : LineElement
            {
                public Implicit_genborn_params(string line, object source) : base(line, source) { }
                // [ implicit_genborn_params ]
                // ; Atom type      sar     st      pi      gbr      hct
                //  NH1             0.155   1       1.028   0.17063  0.79 ; N
                public string Atom_type { get { return GetString ( 0); } }
                public double sar       { get { return GetDouble ( 0); } }
                public int    st        { get { return GetInteger( 0); } }
                public double pi        { get { return GetDouble ( 0); } }
                public double gbr       { get { return GetDouble ( 0); } }
                public double hct       { get { return GetDouble ( 0); } }
                public override string ToString() { return base.ToString(); }
            }
            public class Cmaptypes : LineElement
            {
                public Cmaptypes(string line, object source) : base(line, source) { }
                // [ cmaptypes ]
                // C NH1 CT1 C NH1 1 24 24      0.53048936 3.2162408 4.06375184 5.23405848 8.87430584 11.27767912 8.63761696 7.38388136 ...
                public override string ToString() { return base.ToString(); }
            }
            public class Settles : LineElement
            {
                public Settles(string line, object source) : base(line, source) { }
                // [ settles ]
                // ; i  j   funct       length
                //   1  1   0.09572     0.15139
                public int    i         { get { return GetInteger( 0); } }
                public int    j         { get { return GetInteger( 1); } }
                public string funct     { get { return GetString ( 2); } }
                public double length    { get { return GetDouble ( 3); } }
                public override string ToString() { return base.ToString(); }
            }
            public class Exclusions : LineElement
            {
                public Exclusions(string line, object source) : base(line, source) { }
                // [ exclusions ]
                // 1 2 3
                // 2 1 3
                // 3 1 2
                public override string ToString() { return base.ToString(); }
            }
            public class FunctElement : LineElement
            {
                public readonly int anum;
                public FunctElement(string line, int anum, object source) : base(line, source) { this.anum = anum; }
                // ;  ai    aj funct           c0           c1
                //     3     4     1 1.000000e-01 3.744680e+05 
                // ;  ai    aj funct           c0           c1
                //     2     4     1 0.000000e+00 0.000000e+00 
                // ;  ai    aj    ak funct           c0           c1
                //     1     3     4     1 1.200000e+02 2.928800e+02 
                // ;  ai    aj    ak    al funct           c0           c1           c2
                //     2     1     3     4     1 1.800000e+02 3.347200e+01 2.000000e+00 
                public int    ai    { get { int idx=0; HDebug.Assert(idx<anum); if(idx>=anum) return int.MinValue; return GetInteger(idx); } }
                public int    aj    { get { int idx=1; HDebug.Assert(idx<anum); if(idx>=anum) return int.MinValue; return GetInteger(idx); } }
                public int    ak    { get { int idx=2; HDebug.Assert(idx<anum); if(idx>=anum) return int.MinValue; return GetInteger(idx); } }
                public int    al    { get { int idx=3; HDebug.Assert(idx<anum); if(idx>=anum) return int.MinValue; return GetInteger(idx); } }
                public int    am    { get { int idx=4; HDebug.Assert(idx<anum); if(idx>=anum) return int.MinValue; return GetInteger(idx); } }
                public int    funct { get { int idx=anum;                      return GetInteger(idx); } }
                public double c0    { get { int idx=anum+1+0;                  return GetDouble (idx); } }
                public double c1    { get { int idx=anum+1+1;                  return GetDouble (idx); } }
                public double c2    { get { int idx=anum+1+2;                  return GetDouble (idx); } }
                public double c3    { get { int idx=anum+1+3;                  return GetDouble (idx); } }
                public override string ToString() { return base.ToString(); }
            }
            public class Bond : LineElement
            {
                public Bond(string line, object source) : base(line, source) { }
                // [ bonds ]
                // ;  ai    aj funct           c0           c1
                //     3     4     1 1.000000e-01 3.744680e+05 
                public int    ai    { get { return GetInteger( 0); } }
                public int    aj    { get { return GetInteger( 1); } }
                public int    funct { get { return GetInteger( 2); } }
                public double c0    { get { return GetDouble ( 3); } }
                public double c1    { get { return GetDouble ( 4); } }
                public override string ToString() { return base.ToString(); }
            }
            public class Pair : FunctElement
            {
                public Pair(string line, object source) : base(line, 2, source) { }
                // [ pairs ]
                // ;  ai    aj funct           c0           c1
                //     2     4     1 0.000000e+00 0.000000e+00 
                public override string ToString() { return base.ToString(); }
            }
            public class Angle : FunctElement
            {
                public Angle(string line, object source) : base(line, 3, source) { }
                // [ angles ]
                // ;  ai    aj    ak funct           c0           c1
                //     1     3     4     1 1.200000e+02 2.928800e+02 
                public override string ToString() { return base.ToString(); }
            }
            public class Dihedral : FunctElement
            {
                public Dihedral(string line, object source) : base(line, 4, source) { }
                // [ dihedrals ]
                // ;  ai    aj    ak    al funct           c0           c1           c2
                //     2     1     3     4     1 1.800000e+02 3.347200e+01 2.000000e+00 
            }
            public class Cmap : FunctElement
            {
                public Cmap(string line, object source) : base(line, 5, source) { }
                // [ dihedrals ]
                // ;  ai    aj    ak    al funct           c0           c1           c2
                //     2     1     3     4     1 1.800000e+02 3.347200e+01 2.000000e+00 
                public override string ToString() { return base.ToString(); }
            }
            //public class Include : LineElement
            //{
            //    public Include(string line) : base(line) { }
            //    // ; The force field files to be included
            //    // #include "rt41c5.itp"
            //    // ; Include SPC water topology
            //    // #include "spc.itp"
            //    public string path { get { return GetString(1, new char[] { ' ', ',', '#', '\"', '\'' }); } }
            //}
            public class Position_restraints : LineElement
            {
                public Position_restraints(string line, object source) : base(line, source) { }
                // [ position_restraints ]
                // ;  i funct       fcx        fcy        fcz
                //    1    1       1000       1000       1000
                public int    i         { get { return GetInteger( 0); } }
                public string funct     { get { return GetString ( 1); } }
                public int    fcx       { get { return GetInteger( 2); } }
                public int    fcy       { get { return GetInteger( 3); } }
                public int    fcz       { get { return GetInteger( 4); } }
                public override string ToString() { return base.ToString(); }
            }
            public class System : LineElement
            {
                public System(string line, object source) : base(line, source) { }
                // [ system ]
                // Urea in Water
                public override string ToString() { return base.ToString(); }
            }
            public class Molecules : LineElement
            {
                public Molecules(string line, object source) : base(line, source) { }
                // Urea    1
                // SOL     1000
                public override string ToString() { return base.ToString(); }
            }
        }
    }
}
