using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
            public static Tuple<Xyz, Prm> FromNamd(Pdb pdb, Namd.Psf psf, Namd.Prm prm)
            {
                return BuilderFromNamd.BuildFromNamd(pdb, psf, prm);
            }
            
            public class BuilderFromNamd
            {
                public static void SelfTest()
                {
                    SelfTest("1UAO.psfgen.pdb", "1UAO.psfgen.psf", "1UAO.psfgen.prm", "1UAO.psfgen.xyz");
                    SelfTest("1L2Y.psfgen.pdb", "1L2Y.psfgen.psf", "1L2Y.psfgen.prm", "1L2Y.xyz");
                    SelfTest("1A6G.psfgen.pdb", "1A6G.psfgen.psf", "1A6G.psfgen.prm",              null);
                }
                public static void SelfTest(string pdbname, string psfname, string prmname, string xyzname)
                {
                    using(var temp = new HTempDirectory(@"C:\temp\", null))
                    {
                        temp.EnterTemp();
                        {
                            string resbase = "HTLib2.Bioinfo.HTLib2.Bioinfo.External.Tinker.Resources.selftest.";
                            HResource.CopyResourceTo<Tinker>(resbase+pdbname                   , pdbname);
                            HResource.CopyResourceTo<Tinker>(resbase+psfname                   , psfname);
                            HResource.CopyResourceTo<Tinker>(resbase+"par_all27_prot_lipid.inp", prmname);
                            if(xyzname != null) HResource.CopyResourceTo<Tinker>(resbase+xyzname, xyzname);
                            HResource.CopyResourceTo<Tinker>(resbase+"charmm22.prm"            , "charmm22.prm");
                            
                            {
                                var pdb = Pdb     .FromFile(pdbname);
                                var psf = Namd.Psf.FromFile(psfname);
                                var prm = Namd.Prm.FromFile(prmname);

                                var xyz_prm = BuildFromNamd(pdb, psf, prm);
                                xyz_prm.Item1.ToFile("TinkFromNamd.xyz", false);
                                xyz_prm.Item2.ToFile("TinkFromNamd.prm");
                            }

                            if(xyzname != null)
                            {
                                var xyz0 = Xyz.FromFile("TinkFromNamd.xyz", false);
                                var prm0 = Prm.FromFile("TinkFromNamd.prm");
                                var grad0 = Run.Testgrad(xyz0, prm0, @"C:\temp\"
                                    //, "VDWTERM     NONE"
                                    //, "CHARGETERM  NONE"
                                    //, "BONDTERM    NONE"
                                    //, "ANGLETERM   NONE"
                                    //, "UREYTERM    NONE"
                                    //, "IMPROPTERM  NONE"
                                    //, "TORSIONTERM NONE"
                                    );
                                var forc0 = grad0.anlyts.GetForces(xyz0.atoms);

                                var xyz1 = Xyz.FromFile(xyzname, false);
                                var prm1 = Prm.FromFile("charmm22.prm");
                                var grad1 = Run.Testgrad(xyz1, prm1, @"C:\temp\"
                                    //, "VDWTERM     NONE"
                                    //, "CHARGETERM  NONE"
                                    //, "BONDTERM    NONE"
                                    //, "ANGLETERM   NONE"
                                    //, "UREYTERM    NONE"
                                    //, "IMPROPTERM  NONE"
                                    //, "TORSIONTERM NONE"
                                    );
                                var forc1 = grad1.anlyts.GetForces(xyz1.atoms);
                                {
                                    KDTree.KDTree<object> kdtree = new KDTree.KDTree<object>(3);
                                    var atoms0 = xyz0.atoms;
                                    for(int i=0; i<atoms0.Length; i++)
                                        kdtree.insert(atoms0[i].Coord, i);
                                    var atoms1 = xyz1.atoms;
                                    int[] idx1to0 = new int[atoms1.Length];
                                    for(int i1=0; i1<atoms1.Length; i1++)
                                    {
                                        Vector coord1 = atoms1[i1].Coord;
                                        int i0 = (int)kdtree.nearest(coord1);
                                        Vector coord0 = atoms0[i0].Coord;
                                        kdtree.delete(coord0);
                                        idx1to0[i0] = i1;
                                    }
                                    atoms1 = atoms1.HSelectByIndex(idx1to0);
                                    forc1 = forc1.HSelectByIndex(idx1to0);
                                }

                                Vector[] dforc = VectorBlock.PwSub(forc0, forc1).ToArray();
                                double[] dforcl= dforc.Dist();
                                double max_dforc = dforc.Dist().Max();
                                HDebug.Exception(max_dforc < 1);   // 0.72682794387667848

                                {
                                    double EB   = Math.Abs(grad0.enrgCmpnt.EB    - grad1.enrgCmpnt.EB  );    HDebug.Exception(EB   <  0.1);
                                    double EA   = Math.Abs(grad0.enrgCmpnt.EA    - grad1.enrgCmpnt.EA  );    HDebug.Exception(EA   <  0.1);
                                    double EBA  = Math.Abs(grad0.enrgCmpnt.EBA   - grad1.enrgCmpnt.EBA );    HDebug.Exception(EBA  <  0.1);
                                    double EUB  = Math.Abs(grad0.enrgCmpnt.EUB   - grad1.enrgCmpnt.EUB );    HDebug.Exception(EUB  <  0.1);
                                    double EAA  = Math.Abs(grad0.enrgCmpnt.EAA   - grad1.enrgCmpnt.EAA );    HDebug.Exception(EAA  <  0.1);
                                    double EOPB = Math.Abs(grad0.enrgCmpnt.EOPB  - grad1.enrgCmpnt.EOPB);    HDebug.Exception(EOPB <  0.1);
                                    double EOPD = Math.Abs(grad0.enrgCmpnt.EOPD  - grad1.enrgCmpnt.EOPD);    HDebug.Exception(EOPD <  0.1);
                                    double EID  = Math.Abs(grad0.enrgCmpnt.EID   - grad1.enrgCmpnt.EID );    HDebug.Exception(EID  <  0.1); // 0.0019000000000000128 : N-terminus (and C-terminus) information is/are inconsistent betweeen namd-charmm and tink-charmm22
                                    double EIT  = Math.Abs(grad0.enrgCmpnt.EIT   - grad1.enrgCmpnt.EIT );    HDebug.Exception(EIT  <  0.1);
                                    double ET   = Math.Abs(grad0.enrgCmpnt.ET    - grad1.enrgCmpnt.ET  );    HDebug.Exception(ET   <  0.5); // 0.33029999999999404   : N-terminus (and C-terminus) information is/are inconsistent betweeen namd-charmm and tink-charmm22
                                    double EPT  = Math.Abs(grad0.enrgCmpnt.EPT   - grad1.enrgCmpnt.EPT );    HDebug.Exception(EPT  <  0.1);
                                    double EBT  = Math.Abs(grad0.enrgCmpnt.EBT   - grad1.enrgCmpnt.EBT );    HDebug.Exception(EBT  <  0.1);
                                    double ETT  = Math.Abs(grad0.enrgCmpnt.ETT   - grad1.enrgCmpnt.ETT );    HDebug.Exception(ETT  <  0.1);
                                    double EV   = Math.Abs(grad0.enrgCmpnt.EV    - grad1.enrgCmpnt.EV  );    HDebug.Exception(EV   <  0.1);
                                    double EC   = Math.Abs(grad0.enrgCmpnt.EC    - grad1.enrgCmpnt.EC  );    HDebug.Exception(EC   <  0.5); // 0.37830000000002428
                                    double ECD  = Math.Abs(grad0.enrgCmpnt.ECD   - grad1.enrgCmpnt.ECD );    HDebug.Exception(ECD  <  0.1);
                                    double ED   = Math.Abs(grad0.enrgCmpnt.ED    - grad1.enrgCmpnt.ED  );    HDebug.Exception(ED   <  0.1);
                                    double EM   = Math.Abs(grad0.enrgCmpnt.EM    - grad1.enrgCmpnt.EM  );    HDebug.Exception(EM   <  0.1);
                                    double EP   = Math.Abs(grad0.enrgCmpnt.EP    - grad1.enrgCmpnt.EP  );    HDebug.Exception(EP   <  0.1);
                                    double ER   = Math.Abs(grad0.enrgCmpnt.ER    - grad1.enrgCmpnt.ER  );    HDebug.Exception(ER   <  0.1);
                                    double ES   = Math.Abs(grad0.enrgCmpnt.ES    - grad1.enrgCmpnt.ES  );    HDebug.Exception(ES   <  0.1);
                                    double ELF  = Math.Abs(grad0.enrgCmpnt.ELF   - grad1.enrgCmpnt.ELF );    HDebug.Exception(ELF  <  0.1);
                                    double EG   = Math.Abs(grad0.enrgCmpnt.EG    - grad1.enrgCmpnt.EG  );    HDebug.Exception(EG   <  0.1);
                                    double EX   = Math.Abs(grad0.enrgCmpnt.EX    - grad1.enrgCmpnt.EX  );    HDebug.Exception(EX   <  0.1);
                                }
                            }
                        }
                        temp.QuitTemp();
                    }

                    //{
                    //    //string pathbase = @"C:\Users\htna.IASTATE\svn\htnasvn_htna\Research.bioinfo.prog.NAMD\1A6G\New folder\";
                    //    string pathbase = @"C:\Users\htna.IASTATE\svn\htnasvn_htna\Research.bioinfo.prog.NAMD\1A6G\";
                    //    string toplbase = @"C:\Users\htna.IASTATE\svn\htnasvn_htna\Research.bioinfo.prog.NAMD\top_all27_prot_lipid\";
                    //    var pdb = Pdb     .FromFile(pathbase+"1A6G.psfgen.pdb");
                    //    var psf = Namd.Psf.FromFile(pathbase+"1A6G.psfgen.psf");
                    //    var prm = Namd.Prm.FromFile(toplbase+"par_all27_prot_na.prm");
                    //
                    //        pdb = Pdb     .FromFile(@"K:\Tim-8TIM,1TPH,1M6J\Tim-1M6J\1M6J.psfgen.pdb");
                    //        psf = Namd.Psf.FromFile(@"K:\Tim-8TIM,1TPH,1M6J\Tim-1M6J\1M6J.psfgen.psf");
                    //        prm = Namd.Prm.FromFile(@"K:\Tim-8TIM,1TPH,1M6J\Tim-1M6J\par_all27_prot_lipid.inp");
                    //    var xyz_prm = BuildFromNamd(pdb, psf, prm);
                    //    xyz_prm.Item1.ToFile(@"C:\temp\TinkFromNamd.xyz", false);
                    //    xyz_prm.Item2.ToFile(@"C:\temp\TinkFromNamd.prm");
                    //}
                }
                static Tuple<string, string, double, double, double, int> UnivAtomToTinkKey(Universe.Atom uatom)
                {
                    Pdb.Atom           pdbatom = uatom.sources.HFirstByType(null as Pdb.Atom          ); HDebug.Assert(pdbatom != null);
                    Namd.Psf.Atom      psfatom = uatom.sources.HFirstByType(null as Namd.Psf.Atom     ); HDebug.Assert(psfatom != null);
                    Namd.Prm.Nonbonded prmnbnd = uatom.sources.HFirstByType(null as Namd.Prm.Nonbonded); HDebug.Assert(prmnbnd != null);
                    string name = psfatom.AtomName.Trim();
                    string resn = psfatom.ResidueName.Trim();
                    string type = psfatom.AtomType.Trim();
                    double rmin2= prmnbnd.Rmin2;
                    double eps  = prmnbnd.epsilon;
                    double chrg = psfatom.Charge;
                    int    valnc= uatom.Bonds.Count;

                    var key = new Tuple<string, string, double, double, double, int>(name+"-"+resn, type, rmin2, eps, chrg, valnc);
                    return key;
                }
                public static Tuple<Xyz, Prm> BuildFromNamd(Pdb pdb, Namd.Psf psf, Namd.Prm prm)
                {
                    var univ = Universe.BuilderNamd.Build(psf, prm, pdb, null, null);

                    /// Atom(Id,Class,Type,Desc,Mass)
                    /// * Id   : Vdw(Id,Rmin2,Epsilon)
                    ///          Charge(Id,pch)
                    ///          Biotype(BioId,Name,Resi,Id)
                    /// * Class: Vdw14(Class,Rmin2_14,Eps_14)
                    ///          Bond(Class1,Class2,Kb,b0)
                    ///          Angle(Class1,Class2,Class3,Ktheta,Theta0)
                    ///          Ureybrad(Class1,Class2,Class3,Kub,S0)
                    ///          Improper(Class1,Class2,Class3,Class4,Kpsi,psi0)
                    ///          Torsion(Class1,Class2,Class3,Class4,Kchi0,delta0,n0,Kchi1,delta1,n1,Kchi2,delta2,n2)
                    ///   Type : 
                    Dictionary<Tuple<string, string, double, double, double, int>, int> key_id = new Dictionary<Tuple<string, string, double, double, double, int>, int>();
                    Dictionary<string, int> type_cls = new Dictionary<string,int>();

                    Dictionary<int, Prm.Atom    > id_atom   = new Dictionary<int, Prm.Atom>();
                    Dictionary<int, Prm.Vdw     > cls_vdw   = new Dictionary<int, Prm.Vdw>();
                    Dictionary<int, Prm.Charge  > id_charge = new Dictionary<int, Prm.Charge>();
                    Dictionary<int, Prm.Biotype > id_biotype= new Dictionary<int, Prm.Biotype>();
                    Dictionary<int, Prm.Vdw14   > cls_vdw14 = new Dictionary<int, Prm.Vdw14>();

                    Dictionary<int, Tuple<string, Vector, int, List<int>>> xyzid_info = new Dictionary<int, Tuple<string, Vector, int, List<int>>>();

                    ///                      1                     NH3
                    /// ------------------------------------------------------------------------------------------------------------------
                    ///      pdb: "ATOM      1  N   GLY A   2      24.776  -0.687  28.652  1.00  0.00      A    N"
                    /// namd-psf:    "       1 A    2    GLY  N    NH3   -0.300000       14.0070           0"
                    /// namd-prm:                                 "NH3    0.000000  -0.200000     1.850000 ! ALLOW   POL"
                    /// tink-xyz:                         "     1  NH3   24.776000   -0.687000   28.652000    65     2     5     6     7"
                    /// tink-prm:         "atom         65   26    NH3   "Ammonium Nitrogen"            7    14.007    4"
                    foreach(var uatom in univ.atoms)
                    {
                        HDebug.Assert(uatom.sources.Length == 3);
                        Pdb.Atom           pdbatom = uatom.sources.HFirstByType(null as Pdb.Atom          ); HDebug.Assert(pdbatom != null);
                        Namd.Psf.Atom      psfatom = uatom.sources.HFirstByType(null as Namd.Psf.Atom     ); HDebug.Assert(psfatom != null);
                        Namd.Prm.Nonbonded prmnbnd = uatom.sources.HFirstByType(null as Namd.Prm.Nonbonded); HDebug.Assert(prmnbnd != null);
                        string name = psfatom.AtomName.Trim();
                        string resn = psfatom.ResidueName.Trim();
                        string type = psfatom.AtomType.Trim();
                        double rmin2= prmnbnd.Rmin2;
                        double eps  = prmnbnd.epsilon;
                        double chrg = psfatom.Charge;
                        int    valnc= uatom.Bonds.Count;
                        string desc = string.Format("{0}({1})-{2}", name, type, resn);

                        var key = UnivAtomToTinkKey(uatom);
                        if(key_id.ContainsKey(key) == false)
                            key_id.Add(key, key_id.Count+1);

                        if(type_cls.ContainsKey(type) == false)
                            type_cls.Add(type, type_cls.Count+1);

                        int    tink_id   = key_id[key];
                        int    tink_cls  = type_cls[type];
                        string tink_type = type;

                        if(id_atom.ContainsKey(tink_id) == false)
                        {
                            Prm.Atom tink_atom = Prm.Atom.FromData
                                ( Id          : tink_id
                                , Class       : tink_cls
                                , Type        : tink_type
                                , Description : desc
                                , AtomicNumber: null
                                , Mass        : psfatom.Mass
                                , Valence     : valnc
                                );
                            id_atom.Add(tink_id, tink_atom);
                        }
                        else
                        {
                            Prm.Atom tink_atom = id_atom[tink_id];
                            tink_id  = tink_atom.Id;
                            tink_cls = tink_atom.Class;
                            HDebug.Exception(         tink_atom.Type                 == tink_type);
                            HDebug.Exception(         tink_atom.Description          == desc     );
                            HDebug.Exception(Math.Abs(tink_atom.Mass - psfatom.Mass) < 0.001     );
                            HDebug.Exception(         tink_atom.Valence              == valnc    );
                        }

                        if(cls_vdw.ContainsKey(tink_cls) == false)
                        {
                            Prm.Vdw tink_vdw = Prm.Vdw.FromData
                                ( Class   : tink_cls
                                , Rmin2   : prmnbnd.Rmin2
                                , Epsilon : prmnbnd.epsilon
                                );
                            cls_vdw.Add(tink_cls, tink_vdw);
                        }
                        else
                        {
                            Prm.Vdw tink_vdw = cls_vdw[tink_cls];
                            HDebug.Exception(tink_vdw.Rmin2   == prmnbnd.Rmin2  );
                            HDebug.Exception(tink_vdw.Epsilon == prmnbnd.epsilon);
                        }

                        HDebug.AssertIf(double.IsNaN(prmnbnd.Rmin2_14) == true , double.IsNaN(prmnbnd.eps_14) == true );
                        HDebug.AssertIf(double.IsNaN(prmnbnd.Rmin2_14) == false, double.IsNaN(prmnbnd.eps_14) == false);
                        if(double.IsNaN(prmnbnd.Rmin2_14) == false && double.IsNaN(prmnbnd.eps_14) == false)
                        {
                            if(cls_vdw14.ContainsKey(tink_cls) == false)
                            {
                                Prm.Vdw14 tink_vdw14 = Prm.Vdw14.FromData
                                    ( Class    : tink_cls
                                    , Rmin2_14 : prmnbnd.Rmin2_14
                                    , Eps_14   : prmnbnd.eps_14
                                    );
                                cls_vdw14.Add(tink_cls, tink_vdw14);
                            }
                            else
                            {
                                Prm.Vdw14 tink_vdw14 = cls_vdw14[tink_cls];
                                HDebug.Exception(tink_vdw14.Rmin2_14 == prmnbnd.Rmin2_14);
                                HDebug.Exception(tink_vdw14.Eps_14   == prmnbnd.eps_14  );
                            }
                        }
                        else
                        {
                            HDebug.Exception(cls_vdw14.ContainsKey(tink_cls) == false);
                        }

                        if(id_charge.ContainsKey(tink_id) == false)
                        {
                            Prm.Charge tink_charge = Prm.Charge.FromData
                                ( Id  : tink_id
                                , pch : psfatom.Charge
                                );
                            id_charge.Add(tink_id, tink_charge);
                        }
                        else
                        {
                            Prm.Charge tink_charge = id_charge[tink_id];
                            HDebug.Exception(tink_charge.pch == psfatom.Charge);
                        }

                        if(id_biotype.ContainsKey(tink_id) == false)
                        {
                            Prm.Biotype tink_biotype = Prm.Biotype.FromData
                                ( BioId : id_biotype.Count+1
                                , Name  : name
                                , Resn  : string.Format("{0}({1})-{2}", name, type, resn)
                                , Id    : tink_id
                                );
                            id_biotype.Add(tink_id, tink_biotype);
                        }
                        else
                        {
                            Prm.Biotype tink_biotype = id_biotype[tink_id];
                            HDebug.Exception(tink_biotype.Name == name);
                            HDebug.Exception(tink_biotype.Resn == string.Format("{0}({1})-{2}", name, type, resn));
                            HDebug.Exception(tink_biotype.Id   == tink_id);
                        }

                        var xyzid   = uatom.AtomId;
                        var xyzinfo = new Tuple<string, Vector, int, List<int>>
                        ( tink_type     // AtomType
                        , uatom.Coord   // X, Y, Z
                        , tink_id       // AtomId  
                        , new List<int>()
                        );
                        xyzid_info.Add(xyzid, xyzinfo);
                    }

                    Dictionary<Tuple<int, int>, Prm.Bond> cls_bond = new Dictionary<Tuple<int, int>, Prm.Bond>();
                    foreach(var ubond in univ.bonds)
                    {
                        var key0 = UnivAtomToTinkKey(ubond.atoms[0]); string type0 = key0.Item2; int cls0 = type_cls[type0];
                        var key1 = UnivAtomToTinkKey(ubond.atoms[1]); string type1 = key1.Item2; int cls1 = type_cls[type1];

                        Tuple<int, int> cls01;
                        if(cls0 < cls1) cls01 = new Tuple<int, int>(cls0, cls1);
                        else            cls01 = new Tuple<int, int>(cls1, cls0);

                        if(cls_bond.ContainsKey(cls01) == false)
                        {
                            Prm.Bond tink_bond01 = Prm.Bond.FromData
                                ( Class1 : cls01.Item1
                                , Class2 : cls01.Item2
                                , Kb     : ubond.Kb
                                , b0     : ubond.b0
                                );
                            cls_bond.Add(cls01, tink_bond01);
                        }
                        else
                        {
                            Prm.Bond tink_bond01 = cls_bond[cls01];
                            HDebug.Exception(Math.Abs(tink_bond01.Kb -  ubond.Kb) < 0.01);
                            HDebug.Exception(         tink_bond01.b0 == ubond.b0);
                        }

                        int xyzid0 = ubond.atoms[0].AtomId;
                        int xyzid1 = ubond.atoms[1].AtomId;
                        xyzid_info[xyzid0].Item4.Add(xyzid1);
                        xyzid_info[xyzid1].Item4.Add(xyzid0);
                    }

                    Dictionary<Tuple<int, int, int>, Prm.Angle   > cls_angle    = new Dictionary<Tuple<int, int, int>, Prm.Angle   >();
                    Dictionary<Tuple<int, int, int>, Prm.Ureybrad> cls_ureybrad = new Dictionary<Tuple<int, int, int>, Prm.Ureybrad>();
                    foreach(var uangle in univ.angles)
                    {
                        var key0 = UnivAtomToTinkKey(uangle.atoms[0]); string type0 = key0.Item2; int cls0 = type_cls[type0];
                        var key1 = UnivAtomToTinkKey(uangle.atoms[1]); string type1 = key1.Item2; int cls1 = type_cls[type1];
                        var key2 = UnivAtomToTinkKey(uangle.atoms[2]); string type2 = key2.Item2; int cls2 = type_cls[type2];

                        Tuple<int, int, int> cls012;
                        if(cls0 < cls2) cls012 = new Tuple<int, int, int>(cls0, cls1, cls2);
                        else            cls012 = new Tuple<int, int, int>(cls2, cls1, cls0);

                        double uangle_Theta0 = 180.0 * uangle.Theta0 / Math.PI;

                        if(cls_angle.ContainsKey(cls012) == false)
                        {
                            Prm.Angle tink_angle012 = Prm.Angle.FromData
                                ( Class1 : cls012.Item1
                                , Class2 : cls012.Item2
                                , Class3 : cls012.Item3
                                , Ktheta : uangle.Ktheta
                                , Theta0 : uangle_Theta0
                                );
                            cls_angle.Add(cls012, tink_angle012);

                            HDebug.Exception(cls_ureybrad.ContainsKey(cls012) == false);
                            if(uangle.Kub != 0)
                            {
                                Prm.Ureybrad tink_ureybrad = Prm.Ureybrad.FromData
                                    ( Class1 : cls012.Item1
                                    , Class2 : cls012.Item2
                                    , Class3 : cls012.Item3
                                    , Kub    : uangle.Kub
                                    , S0     : uangle.S0
                                    );
                                cls_ureybrad.Add(cls012, tink_ureybrad);
                            }
                        }
                        else
                        {
                            Prm.Angle tink_angle012 = cls_angle[cls012];
                            HDebug.Exception(         tink_angle012.Ktheta == uangle.Ktheta);
                            HDebug.Exception(Math.Abs(tink_angle012.Theta0 -  uangle_Theta0) < 0.01);

                            if(uangle.Kub != 0)
                            {
                                Prm.Ureybrad tink_ureybrad = cls_ureybrad[cls012];
                                HDebug.Exception(tink_ureybrad.Kub == uangle.Kub);
                                HDebug.Exception(tink_ureybrad.S0  == uangle.S0);
                            }
                            else
                            {
                                HDebug.Exception(cls_ureybrad.ContainsKey(cls012) == false);
                            }
                        }
                    }

                    Dictionary<Tuple<int, int, int, int>, Prm.Improper> cls_improper = new Dictionary<Tuple<int, int, int, int>, Prm.Improper>();
                    foreach(var improper in univ.impropers)
                    {
                        var key0 = UnivAtomToTinkKey(improper.atoms[0]); string type0 = key0.Item2; int cls0 = type_cls[type0];
                        var key1 = UnivAtomToTinkKey(improper.atoms[1]); string type1 = key1.Item2; int cls1 = type_cls[type1];
                        var key2 = UnivAtomToTinkKey(improper.atoms[2]); string type2 = key2.Item2; int cls2 = type_cls[type2];
                        var key3 = UnivAtomToTinkKey(improper.atoms[3]); string type3 = key3.Item2; int cls3 = type_cls[type3];

                        Tuple<int, int, int, int> cls0123 = new Tuple<int, int, int, int>(cls0, cls1, cls2, cls3);
                        //if(cls0 < cls2) cls0123 = new Tuple<int, int, int, int>(cls0, cls1, cls2, cls3);
                        //else            cls0123 = new Tuple<int, int, int, int>(cls3, cls2, cls1, cls0);

                        double improper_psi0 = 180.0 * improper.psi0 / Math.PI;

                        if(cls_improper.ContainsKey(cls0123) == false)
                        {
                            Prm.Improper tink_improper = Prm.Improper.FromData
                                ( Class1 : cls0
                                , Class2 : cls1
                                , Class3 : cls2
                                , Class4 : cls3
                                , Kpsi   : improper.Kpsi
                                , psi0   : improper_psi0
                                );
                            cls_improper.Add(cls0123, tink_improper);
                        }
                        else
                        {
                            Prm.Improper tink_improper = cls_improper[cls0123];
                            HDebug.Exception(         tink_improper.Kpsi == improper.Kpsi);
                            HDebug.Exception(Math.Abs(tink_improper.psi0 -  improper_psi0) < 0.01);
                        }
                    }

                    Dictionary<Tuple<int, int, int, int>, List<Universe.Dihedral>> cls_dihedrals = new Dictionary<Tuple<int, int, int, int>, List<Universe.Dihedral>>();
                    foreach(var atom in univ.atoms)
                    {
                        var inter1234s = atom.ListInterAtom1234();
                        foreach(var inter1234 in inter1234s)
                        {
                            HDebug.Assert(inter1234.Count == 4);
                            var key0 = UnivAtomToTinkKey(inter1234[0]); string type0 = key0.Item2; int cls0 = type_cls[type0];
                            var key1 = UnivAtomToTinkKey(inter1234[1]); string type1 = key1.Item2; int cls1 = type_cls[type1];
                            var key2 = UnivAtomToTinkKey(inter1234[2]); string type2 = key2.Item2; int cls2 = type_cls[type2];
                            var key3 = UnivAtomToTinkKey(inter1234[3]); string type3 = key3.Item2; int cls3 = type_cls[type3];

                            Tuple<int, int, int, int> cls0123 = new Tuple<int, int, int, int>(cls0, cls1, cls2, cls3);
                            Tuple<int, int, int, int> cls3210 = new Tuple<int, int, int, int>(cls3, cls2, cls1, cls0);

                            if(cls_dihedrals.ContainsKey(cls0123) == false) cls_dihedrals.Add(cls0123, new List<Universe.Dihedral>());
                            if(cls_dihedrals.ContainsKey(cls3210) == false) cls_dihedrals.Add(cls3210, new List<Universe.Dihedral>());
                        }
                    }
                    foreach(var dihedral in univ.dihedrals)
                    {
                        var key0 = UnivAtomToTinkKey(dihedral.atoms[0]); string type0 = key0.Item2; int cls0 = type_cls[type0];
                        var key1 = UnivAtomToTinkKey(dihedral.atoms[1]); string type1 = key1.Item2; int cls1 = type_cls[type1];
                        var key2 = UnivAtomToTinkKey(dihedral.atoms[2]); string type2 = key2.Item2; int cls2 = type_cls[type2];
                        var key3 = UnivAtomToTinkKey(dihedral.atoms[3]); string type3 = key3.Item2; int cls3 = type_cls[type3];

                        Tuple<int, int, int, int> cls0123 = new Tuple<int, int, int, int>(cls0, cls1, cls2, cls3);
                        Tuple<int, int, int, int> cls3210 = new Tuple<int, int, int, int>(cls3, cls2, cls1, cls0);

                        cls_dihedrals[cls0123].Add(dihedral);
                        cls_dihedrals[cls3210].Add(dihedral);
                    }
                    Dictionary<Tuple<int, int, int, int>, Prm.Torsion> cls_torsion = new Dictionary<Tuple<int, int, int, int>, Prm.Torsion>();
                    foreach(var cls0123 in cls_dihedrals.Keys)
                    {
                        Universe.Dihedral[] dihedrals = cls_dihedrals[cls0123].ToArray();

                        Dictionary<Tuple<double, double>, List<double>> delta_n_Kchis = new Dictionary<Tuple<double, double>, List<double>>();
                        foreach(var dihedral in dihedrals)
                        {
                            double dihedral_delta = 180.0 * dihedral.delta / Math.PI;
                            var delta_n = new Tuple<double, double>(dihedral_delta, dihedral.n);
                            if(delta_n_Kchis.ContainsKey(delta_n) == false) delta_n_Kchis.Add(delta_n, new List<double>());
                            delta_n_Kchis[delta_n].Add(dihedral.Kchi);
                        }

                        Tuple<double, double>[] lst_delta_n = delta_n_Kchis.Keys.ToArray();
                        HDebug.Exception(lst_delta_n.Length <= 3);

                        double?  Kchi0 = null; if(lst_delta_n.Length >=1)  Kchi0 = delta_n_Kchis[lst_delta_n[0]].Mean();
                        double? delta0 = null; if(lst_delta_n.Length >=1) delta0 =               lst_delta_n[0].Item1;
                        double?     n0 = null; if(lst_delta_n.Length >=1)     n0 =               lst_delta_n[0].Item2;
                        double?  Kchi1 = null; if(lst_delta_n.Length >=2)  Kchi1 = delta_n_Kchis[lst_delta_n[1]].Mean();
                        double? delta1 = null; if(lst_delta_n.Length >=2) delta1 =               lst_delta_n[1].Item1;
                        double?     n1 = null; if(lst_delta_n.Length >=2)     n1 =               lst_delta_n[1].Item2;
                        double?  Kchi2 = null; if(lst_delta_n.Length >=3)  Kchi2 = delta_n_Kchis[lst_delta_n[2]].Mean();
                        double? delta2 = null; if(lst_delta_n.Length >=3) delta2 =               lst_delta_n[2].Item1;
                        double?     n2 = null; if(lst_delta_n.Length >=3)     n2 =               lst_delta_n[2].Item2;

                        Prm.Torsion tink_torsion = Prm.Torsion.FromData
                            ( Class1 : cls0123.Item1
                            , Class2 : cls0123.Item2
                            , Class3 : cls0123.Item3
                            , Class4 : cls0123.Item4
                            , Kchi0  : Kchi0
                            , delta0 : delta0
                            , n0     : n0
                            , Kchi1  : Kchi1
                            , delta1 : delta1
                            , n1     : n1
                            , Kchi2  : Kchi2
                            , delta2 : delta2
                            , n2     : n2
                            );
                        cls_torsion.Add(cls0123, tink_torsion);
                    }

                    List<string> prmlines;
                    #region build lines
                    {
                        List<string> lines = new List<string>();
                        lines.Add("");
                        lines.Add("      ##############################");
                        lines.Add("      ##                          ##");
                        lines.Add("      ##  Force Field Definition  ##");
                        lines.Add("      ##                          ##");
                        lines.Add("      ##############################");
                        lines.Add("");
                        lines.Add("");
                        lines.Add("forcefield              CHARMM22");
                        lines.Add("");
                        lines.Add("vdwtype                 LENNARD-JONES");
                        lines.Add("radiusrule              ARITHMETIC");
                        lines.Add("radiustype              R-MIN");
                        lines.Add("radiussize              RADIUS");
                        lines.Add("epsilonrule             GEOMETRIC");
                        lines.Add("vdw-14-scale            1.0");
                        lines.Add("chg-14-scale            1.0");
                        lines.Add("electric                332.0716");
                        lines.Add("dielectric              1.0");
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      #############################");
                        lines.Add("      ##                         ##");
                        lines.Add("      ##  Atom Type Definitions  ##");
                        lines.Add("      ##                         ##");
                        lines.Add("      #############################");
                        lines.Add("");
                        lines.Add("");
                        foreach(int id in id_atom.Keys.ToArray().HSort())
                            lines.Add(id_atom[id].line);
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      ################################");
                        lines.Add("      ##                            ##");
                        lines.Add("      ##  Van der Waals Parameters  ##");
                        lines.Add("      ##                            ##");
                        lines.Add("      ################################");
                        lines.Add("");
                        lines.Add("");
                        foreach(int cls in cls_vdw.Keys.ToArray().HSort())
                            lines.Add(cls_vdw[cls].line);
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      ####################################");
                        lines.Add("      ##                                ##");
                        lines.Add("      ##  1-4 Van der Waals Parameters  ##");
                        lines.Add("      ##                                ##");
                        lines.Add("      ####################################");
                        lines.Add("");
                        lines.Add("");
                        foreach(int cls in cls_vdw14.Keys.ToArray().HSort())
                            lines.Add(cls_vdw14[cls].line);
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      ##################################");
                        lines.Add("      ##                              ##");
                        lines.Add("      ##  Bond Stretching Parameters  ##");
                        lines.Add("      ##                              ##");
                        lines.Add("      ##################################");
                        lines.Add("");
                        lines.Add("");
                        foreach(var bond in cls_bond)
                            lines.Add(bond.Value.line);
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      ################################");
                        lines.Add("      ##                            ##");
                        lines.Add("      ##  Angle Bending Parameters  ##");
                        lines.Add("      ##                            ##");
                        lines.Add("      ################################");
                        lines.Add("");
                        lines.Add("");
                        foreach(var angle in cls_angle)
                            lines.Add(angle.Value.line);
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      ###############################");
                        lines.Add("      ##                           ##");
                        lines.Add("      ##  Urey-Bradley Parameters  ##");
                        lines.Add("      ##                           ##");
                        lines.Add("      ###############################");
                        lines.Add("");
                        lines.Add("");
                        foreach(var ureybrad in cls_ureybrad)
                            lines.Add(ureybrad.Value.line);
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      ####################################");
                        lines.Add("      ##                                ##");
                        lines.Add("      ##  Improper Dihedral Parameters  ##");
                        lines.Add("      ##                                ##");
                        lines.Add("      ####################################");
                        lines.Add("");
                        lines.Add("");
                        foreach(var improper in cls_improper)
                            lines.Add(improper.Value.line);
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      ############################");
                        lines.Add("      ##                        ##");
                        lines.Add("      ##  Torsional Parameters  ##");
                        lines.Add("      ##                        ##");
                        lines.Add("      ############################");
                        lines.Add("");
                        lines.Add("");
                        foreach(var torsion in cls_torsion)
                            lines.Add(torsion.Value.line);
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      ########################################");
                        lines.Add("      ##                                    ##");
                        lines.Add("      ##  Atomic Partial Charge Parameters  ##");
                        lines.Add("      ##                                    ##");
                        lines.Add("      ########################################");
                        lines.Add("");
                        lines.Add("");
                        foreach(int id in id_charge.Keys.ToArray().HSort())
                            lines.Add(id_charge[id].line);
                        lines.Add("");
                        lines.Add("");
                        lines.Add("      ########################################");
                        lines.Add("      ##                                    ##");
                        lines.Add("      ##  Biopolymer Atom Type Conversions  ##");
                        lines.Add("      ##                                    ##");
                        lines.Add("      ########################################");
                        lines.Add("");
                        lines.Add("");
                        foreach(int id in id_biotype.Keys.ToArray().HSort())
                            lines.Add(id_biotype[id].line);
                        lines.Add("");
                        prmlines = lines;
                    }
                    #endregion

                    Dictionary<string, string> atomtype_natomtype;
                    {
                        atomtype_natomtype = xyzid_info.Values.ToArray().HListItem1().HToHashSet().HToDictionaryAsKey("");
                        foreach(string atomtype in atomtype_natomtype.Keys.ToArray())
                        {
                            string natomtype = atomtype;
                            if(natomtype.Length > 3)
                            {
                                Dictionary<char, char> map = new Dictionary<char, char>
                                {
                                    {'0', '1'}, {'1', '2'}, {'2', '3'}, {'3', '4'}, {'4', '5'},
                                    {'5', '6'}, {'6', '7'}, {'7', '8'}, {'8', '9'}, {'9', 'A'},
                                    {'A', 'B'}, {'B', 'C'}, {'C', 'D'}, {'D', 'E'}, {'E', 'F'},
                                    {'F', 'G'}, {'G', 'H'}, {'H', 'I'}, {'I', 'J'}, {'J', 'K'},
                                    {'K', 'L'}, {'L', 'M'}, {'M', 'N'}, {'N', 'O'}, {'O', 'P'},
                                    {'P', 'Q'}, {'Q', 'R'}, {'R', 'S'}, {'S', 'T'}, {'T', 'U'},
                                    {'U', 'V'}, {'V', 'W'}, {'W', 'X'}, {'X', 'Y'}, {'Y', 'Z'},
                                    {'Z', '0'},
                                };
                                string temptype = atomtype.Substring(0, 3);
                                foreach(var key in map.Keys.ToArray())
                                    if(map[key] == temptype[2])
                                        map[key] = ' '; // marking ending point as ' '
                                while(true)
                                {
                                    if(temptype[2] == ' ') HDebug.Exception();

                                    if((atomtype_natomtype.ContainsKey(temptype) == false) && (atomtype_natomtype.ContainsValue(temptype) == false))
                                    {
                                        natomtype = temptype;
                                        break;
                                    }

                                    temptype = temptype.Substring(0,2) + map[temptype[2]];
                                }
                            }
                            atomtype_natomtype[atomtype] = natomtype;
                        }
                    }
                    List<string> xyzlines;
                    {
                        xyzlines = new List<string>();
                        xyzlines.Add(Xyz.Header.FromData(xyzid_info.Count, "", "", "").line);
                        foreach(var xyzid in xyzid_info.Keys.ToArray().HSort())
                        {
                            int    id        = xyzid;
                            string atomtype  = xyzid_info[xyzid].Item1;
                                   atomtype  = atomtype_natomtype[atomtype];
                            double x         = xyzid_info[xyzid].Item2[0];
                            double y         = xyzid_info[xyzid].Item2[1];
                            double z         = xyzid_info[xyzid].Item2[2];
                            int    atomid    = xyzid_info[xyzid].Item3;
                            int[]  bondedids = xyzid_info[xyzid].Item4.ToArray();
                            xyzlines.Add(Xyz.Atom.FromData(id, atomtype, x, y, z, atomid, bondedids).line);
                        }
                    }

                    {
                        for(int i=0; i<prmlines.Count; i++)
                        {
                            if(prmlines[i].StartsWith("atom "))
                            {
                                var  atom = Prm.Atom.FromLine(prmlines[i]);
                                var natom = Prm.Atom.FromData
                                    ( Id          : atom.Id 
                                    , Class       : atom.Class
                                    , Type        : atomtype_natomtype[atom.Type]
                                    , Description : atom.Description
                                    , AtomicNumber: atom.AtomicNumber
                                    , Mass        : atom.Mass
                                    , Valence     : atom.Valence
                                    );
                                prmlines[i] = natom.line;
                            }
                        }
                    }

                    Prm tink_prm = Prm.FromLines(prmlines);
                    Xyz tink_xyz = Xyz.FromLines(xyzlines);
                    return new Tuple<Xyz, Prm>(tink_xyz, tink_prm);
                }
            }
    }
}
