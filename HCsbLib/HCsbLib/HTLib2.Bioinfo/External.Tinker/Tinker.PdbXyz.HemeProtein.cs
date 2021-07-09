using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Tinker
    {
        public partial class PdbXyz
        {
            public partial class HemeProtein
            {
                static readonly string resbase = "HTLib2.Bioinfo.HTLib2.Bioinfo.External.Tinker.Resources.";
                static readonly string[] ress = HResource.GetResourceNames<HemeProtein>();

                public static void PdbXyzSample()
                {
                    // sample 1
                    {
                        using(var temp = new HTempDirectory(@"C:\temp\", null))
                        {
                            temp.EnterTemp();

                            string   pdbid            = "1A6G";
                            Pdb      pdb              = Pdb.FromPdbid(pdbid);
                            string   cachebase        = @".\";
                            double   rmsgrad          = 10.0;   // 0.0001; minimize grad
                            bool     rebuild          = true;   // false to skip if the xyz is already made. true to re-make in any case.
                            string[] psfgen_segmentp1 = null;   // psfgen segment p1 option
                            bool     fixheme          = true;   // true to fix heme posision when minimizing. false to allow bending heme
                            string[] keycontents      = null;   // contents for tinker key file

                            Dictionary<string, string> result = Tinker.PdbXyz.HemeProtein.PdbXyz
                            (
                                pdb, pdbid, cachebase, rmsgrad, rebuild,
                                psfgen_segmentp1: psfgen_segmentp1,
                                fixheme: fixheme,
                                keycontents: keycontents
                            );
                            // result["psfgen pdb"     ] == ".\1A6G.v1.psfgen.pdb"
                            // result["psfgen psf"     ] == ".\1A6G.v1.psfgen.psf"
                            // result["minimizable pdb"] == ".\1A6G.v7.prot-heme.from.v1.reorderedto.xyz.pdb"
                            // result["minimizable xyz"] == ".\1A6G.v7.prot-heme.copy.from.v6.xyz"
                            // result["minimize bat"   ] == ".\1A6G.va8.prot-heme.Min.Newton.FixHeme.bat"
                            // result["minimize key"   ] == ".\1A6G.va8.prot-heme.Min.Newton.FixHeme.key"
                            // result["minimized pdb"  ] == ".\1A6G.va9b2.prot-heme.AlignTo1A6G.pdb"
                            // result["minimized xyz"  ] == ".\1A6G.va9b2.prot-heme.AlignTo1A6G.xyz"
                            // result["prm"            ] == ".\charmm22-1A6G.prm"

                            temp.QuitTemp();
                        }
                    }
                    // sample 2: mutate resi 29 -> ALA
                    {
                        using(var temp = new HTempDirectory(@"C:\temp\", null))
                        {
                            temp.EnterTemp();

                            string   pdbid            = "1A6G";
                            Pdb      pdb              = Pdb.FromPdbid(pdbid);
                            string   cachebase        = @".\";
                            double   rmsgrad          = 10.0;   // 0.0001; minimize grad
                            bool     rebuild          = true;   // false to skip if the xyz is already made. true to re-make in any case.
                            string[] psfgen_segmentp1 = new string[] { "mutate 29 ALA" };
                            bool     fixheme          = true;   // true to fix heme posision when minimizing. false to allow bending heme
                            string[] keycontents      = null;   // contents for tinker key file

                            Dictionary<string, string> result = Tinker.PdbXyz.HemeProtein.PdbXyz
                            (
                                pdb, pdbid, cachebase, rmsgrad, rebuild,
                                psfgen_segmentp1: psfgen_segmentp1,
                                fixheme: fixheme,
                                keycontents: keycontents
                            );

                            temp.QuitTemp();
                        }
                    }
                    // sample 3: minimize with implicit-solvant
                    {
                        using(var temp = new HTempDirectory(@"C:\temp\", null))
                        {
                            temp.EnterTemp();

                            string   pdbid            = "1A6G";
                            Pdb      pdb              = Pdb.FromPdbid(pdbid);
                            string   cachebase        = @".\";
                            double   rmsgrad          = 10.0;   // 0.0001; minimize grad
                            bool     rebuild          = true;   // false to skip if the xyz is already made. true to re-make in any case.
                            string[] psfgen_segmentp1 = null;   // psfgen segment p1 option
                            bool     fixheme          = true;   // true to fix heme posision when minimizing. false to allow bending heme
                            string[] keycontents      = new string[] { "SOLVATE               GBSA" };

                            Dictionary<string, string> result = Tinker.PdbXyz.HemeProtein.PdbXyz
                            (
                                pdb, pdbid, cachebase, rmsgrad, rebuild,
                                psfgen_segmentp1: psfgen_segmentp1,
                                fixheme: fixheme,
                                keycontents: keycontents
                            );

                            temp.QuitTemp();
                        }
                    }
                }

                public static Dictionary<string, string> PdbXyz(Pdb pdb, string pdbid, string cachebase, double rmsgrad, bool rebuild)
                {
                    string[] psfgen_segmentp1 = new string[0];
                    string[] keycontents = new string[0];
                    //keycontents = new string[]
                    //{
                    //    "SOLVATE               GBSA",
                    //};
                    bool fixheme = true;
                    return PdbXyz
                        ( pdb, pdbid
                        , cachebase, rmsgrad, rebuild
                        , psfgen_segmentp1
                        , fixheme, keycontents
                        );
                }
                public static Dictionary<string, string> PdbXyz
                    ( Pdb pdb, string pdbid
                    , string cachebase
                    , double rmsgrad
                    , bool rebuild
                    , string[] psfgen_segmentp1
                    , bool fixheme, string[] keycontents
                    )
                {
                    if(HDirectory.Exists(cachebase) == false)
                        HDirectory.CreateDirectory(cachebase);
                    string workdir = HEnvironment.CurrentDirectory;

                    Dictionary<string, string> name_path;
                    {
                        name_path = new Dictionary<string,string>();
                        string pdb1_name  = cachebase+"$PDBID$.v1.psfgen.pdb"                           .Replace("$PDBID$", pdbid); name_path.Add("psfgen pdb"     , pdb1_name );
                        string psf1_name  = cachebase+"$PDBID$.v1.psfgen.psf"                           .Replace("$PDBID$", pdbid); name_path.Add("psfgen psf"     , psf1_name );
                        string pdb7_name  = cachebase+"$PDBID$.v7.prot-heme.from.v1.reorderedto.xyz.pdb".Replace("$PDBID$", pdbid); name_path.Add("minimizable pdb", pdb7_name );
                        string xyz7_name  = cachebase+"$PDBID$.v7.prot-heme.copy.from.v6.xyz"           .Replace("$PDBID$", pdbid); name_path.Add("minimizable xyz", xyz7_name );
                        string bat8_name  = cachebase+"$PDBID$.va8.prot-heme.Min.Newton.FixHeme.bat"    .Replace("$PDBID$", pdbid); name_path.Add("minimize bat"   , bat8_name );
                        string key8_name  = cachebase+"$PDBID$.va8.prot-heme.Min.Newton.FixHeme.key"    .Replace("$PDBID$", pdbid); name_path.Add("minimize key"   , key8_name );
                        string pdb9b_name = cachebase+"$PDBID$.va9b2.prot-heme.AlignTo1A6G.pdb"         .Replace("$PDBID$", pdbid); name_path.Add("minimized pdb"  , pdb9b_name);
                        string xyz9b_name = cachebase+"$PDBID$.va9b2.prot-heme.AlignTo1A6G.xyz"         .Replace("$PDBID$", pdbid); name_path.Add("minimized xyz"  , xyz9b_name);
                        string prm_name   = cachebase+"charmm22-$PDBID$.prm"                            .Replace("$PDBID$", pdbid); name_path.Add("prm"            , prm_name  );
                    }
                    if(rebuild==false && HFile.ExistsAll(name_path.Values.ToArray()))
                        return name_path;

                    HEnvironment.CurrentDirectory = cachebase;

                    using(var temp = new HTempDirectory(@"C:\temp\", null))
                    {
                        temp.EnterTemp();

                        HResource.CopyResourceTo<HemeProtein>(resbase+"psfgen.exe"              , "psfgen.exe"              );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"newton.exe"              , "newton.exe"              );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"libiomp5md.dll"          , "libiomp5md.dll"          );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"pdbxyz.exe"              , "pdbxyz.exe"              );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"grep.exe"                , "grep.exe"                );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"libiconv2.dll"           , "libiconv2.dll"           );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"libintl3.dll"            , "libintl3.dll"            );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"pcre3.dll"               , "pcre3.dll"               );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"regex2.dll"              , "regex2.dll"              );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"tcl85.dll"               , "tcl85.dll"               );
                        HResource.CopyResourceTo<HemeProtein>(resbase+"par_all27_prot_lipid.inp", "par_all27_prot_lipid.inp");
                        HResource.CopyResourceTo<HemeProtein>(resbase+"top_all27_prot_lipid.inp", "top_all27_prot_lipid.inp");
                        {
                            PdbXyz
                                ( pdb, pdbid, rmsgrad
                                , psfgen_segmentp1
                                , fixheme, keycontents
                                );
                        }
                        HFile.Delete("psfgen.exe"              );
                        HFile.Delete("newton.exe"              );
                        HFile.Delete("libiomp5md.dll"          );
                        HFile.Delete("pdbxyz.exe"              );
                        HFile.Delete("grep.exe"                );
                        HFile.Delete("libiconv2.dll"           );
                        HFile.Delete("libintl3.dll"            );
                        HFile.Delete("pcre3.dll"               );
                        HFile.Delete("regex2.dll"              );
                        HFile.Delete("tcl85.dll"               );
                        HFile.Delete("par_all27_prot_lipid.inp");
                        HFile.Delete("top_all27_prot_lipid.inp");

                        foreach(string filename in HDirectory.EnumerateFiles(cachebase))
                            HFile.Delete(filename);
                        HDirectory.DirectoryCopy(".", cachebase, false);

                        temp.QuitTemp();
                    }

                    HEnvironment.CurrentDirectory = workdir;

                    if(HFile.ExistsAll(name_path.Values.ToArray()))
                        return name_path;
                    return null;
                }
                public static void PdbXyz(Pdb pdb, string pdbid, double rmsgrad, string[] psfgen_segmentp1, bool fixheme, string[] keycontents)
                {
                    // remove altLoc
                    PdbXyz_01_RemoveAltloc(pdb, pdbid);
            
                    // psfgen
                    int conect_hse_resi; // resseq of hse
                    int conect_hem_resi; // resseq of heme
                    PdbXyz_02_FindConectResi(pdbid, out conect_hse_resi, out conect_hem_resi);
                    PdbXyz_03_Psfgen(pdbid, conect_hse_resi, conect_hem_resi, psfgen_segmentp1);

                    // convert tinker format
                    PdbXyz_04_SplitPdbToProtHeme(pdbid);
                    PdbXyz_05_Prm(pdbid);
                    PdbXyz_06_HemeXyz(pdbid);
                    PdbXyz_07_ProtXyz(pdbid);

                    string bind_xyz_name;
                    int    bind_ID;
                    PdbXyz_08_ProtXyzMarkBind(pdbid, out bind_xyz_name, out bind_ID);
                    PdbXyz_09_MergProtHemeXyz(pdbid, bind_xyz_name, bind_ID);
                    PdbXyz_10_XyzMinimizable(pdbid);

                    // minimize
                    PdbXyz_11_KeyFile(pdbid, fixheme, keycontents);
                    PdbXyz_12_Minimize(pdbid, rmsgrad);
                    PdbXyz_13_MinimizedPdb(pdbid);

                    // align to 1A6G
                    PdbXyz_14_ReferenceProtein_1A6G(pdbid);
                    PdbXyz_15_AlignToRef(pdbid);
                    PdbXyz_16_DeleteTemp(pdbid);
                }
                public static void PdbXyz_01_RemoveAltloc(Pdb pdb, string pdbid)
                {   /// $PDBID$.pdb
                    /// $PDBID$.v0.altlocA.pdb
                    //Pdb pdb = PdbDatabase.GetPdb(pdbid);
                    pdb.ToFile(pdbid+".pdb");

                    Pdb.Element[] pdb_elements = pdb.elements.HClone();
                    for(int i=0; i<pdb_elements.Length; i++)
                    {
                        char? altLoc = null;
                        if(pdb_elements[i] is Pdb.Atom  ) altLoc = (pdb_elements[i] as Pdb.Atom  ).altLoc;
                        if(pdb_elements[i] is Pdb.Hetatm) altLoc = (pdb_elements[i] as Pdb.Hetatm).altLoc;
                        if(altLoc != null)
                        {
                            if(altLoc.Value == ' ') continue;
                            if(altLoc.Value == 'A') continue;
                            if(altLoc.Value == 'a') continue;
                            pdb_elements[i] = null;
                        }
                    }
                    pdb_elements = pdb_elements.HRemoveAll(null);
                    Pdb pdb0 = Pdb.FromLines(pdb_elements.ListLine());
                    pdb0.ToFile(pdbid+".v0.altlocA.pdb");
                }
                public static void PdbXyz_02_FindConectResi
                    ( string pdbid
                    , out int conect_hse_resi
                    , out int conect_hem_resi
                    )
                {
                    Pdb pdb = Pdb.FromFile(pdbid+".v0.altlocA.pdb");
                    var atom_bondeds = pdb.conects.HDictConectAtoms();

                    Pdb.Hetatm[] hem_fe = pdb.hetatms
                                                .SelectByResName("HEM")
                                                .SelectByName("FE").ToArray();
                    HDebug.Assert(hem_fe.Length == 1);
                    int hem_fe_serial = hem_fe[0].serial;
                    conect_hem_resi  = hem_fe[0].resSeq;

                    HDebug.Assert(atom_bondeds.ContainsKey(hem_fe_serial));
                    Pdb.Atom[] atoms = pdb.atoms;
                    int[] serials = atoms.ListSerial().ToArray();
                    var hse_atom_serial = atom_bondeds[hem_fe_serial].Intersect(serials).ToArray();
                    HDebug.Assert(hse_atom_serial.Length == 1);

                    Pdb.Atom[] hse_atom = atoms.SelectBySerial(hse_atom_serial).ToArray();
                    HDebug.Assert(hse_atom.Length == 1);
                    conect_hse_resi = hse_atom[0].resSeq;
                }
                public static void PdbXyz_03_Psfgen
                    ( string pdbid
                    , int conect_hse_resi
                    , int conect_hem_resi
                    , params string[] psfgen_segmentp1
                    )
                {   /// $PDBID$.v1.psfgen.pdb
                    /// $PDBID$.v1.psfgen.psf
                    /// $PDBID$.v1.psfgen.txt
                    string[] psfgen_lines = HResource.GetResourceLines<HemeProtein>(resbase+"$PDBID$.v1.psfgen.txt");
                    psfgen_lines = psfgen_lines.HReplace("$PDBID$"   , pdbid                     );
                    psfgen_lines = psfgen_lines.HReplace("$PROTRESI$", conect_hse_resi.ToString());
                    psfgen_lines = psfgen_lines.HReplace("$HEMERESI$", conect_hem_resi.ToString());
                    {
                        //throw new NotImplementedException("check!!");
                        if(psfgen_segmentp1 != null)
                        foreach(string segment_p1 in psfgen_segmentp1)
                        {
                            int[] idx = psfgen_lines.HIndexAllContains("$SEGMENTP1$");
                            HDebug.Assert(idx.Length == 1);
                            psfgen_lines = psfgen_lines.HInsert(idx[0], "             "+segment_p1);
                        }
                        psfgen_lines = psfgen_lines.HRemoveAllContains("$SEGMENTP1$");
                    }
                    HFile.WriteAllLines("$PDBID$.v1.psfgen.txt".Replace("$PDBID$", pdbid), psfgen_lines);

                    psfgen_lines = psfgen_lines.HAdd("quit");
                    HFile.WriteAllLines("psfgen.txt", psfgen_lines);
                    HProcess.StartAsBatchInConsole("psfgen.bat", false
                                                    , "mkdir working                                                   "
                                                    , "grep -v \"^HETATM\" $PDBID$.v0.altlocA.pdb > working/$PDBID$_protein.pdb".Replace("$PDBID$", pdbid)
                                                    , "grep \"HEM\"        $PDBID$.v0.altlocA.pdb > working/$PDBID$_heme.pdb   ".Replace("$PDBID$", pdbid)
                                                    , "grep \"CMO\"        $PDBID$.v0.altlocA.pdb > working/$PDBID$_cmo.pdb    ".Replace("$PDBID$", pdbid)
                                                    , "psfgen.exe < psfgen.txt                                         "
                                                    );

                    string   psf_name = "$PDBID$.v1.psfgen.psf".Replace("$PDBID$", pdbid);
                    string   pdb_name = "$PDBID$.v1.psfgen.pdb".Replace("$PDBID$", pdbid);
                    if(HFile.Exists(psf_name) == false) throw new Exception("cannot create file: "+psf_name);
                    if(HFile.Exists(pdb_name) == false) throw new Exception("cannot create file: "+pdb_name);
                    HFile.Delete("psfgen.txt");
                    HFile.Delete("psfgen.txt");
                    HDirectory.Delete("working", true);
                }
                public static void PdbXyz_04_SplitPdbToProtHeme(string pdbid)
                {   /// $PDBID$.v2.prot.pdb
                    /// $PDBID$.v2.heme.pdb
                    Pdb pdb   = Pdb.FromFile("$PDBID$.v1.psfgen.pdb".Replace("$PDBID$", pdbid));
                    var atoms = pdb.atoms;
                    List<Pdb.Atom> prot = new List<Pdb.Atom>();
                    List<Pdb.Atom> heme = new List<Pdb.Atom>();
                    foreach(var atom in atoms)
                    {
                        string resname = atom.resName;
                        if(resname == "HEM") heme.Add(atom);
                        else                 prot.Add(atom);
                    }
                    HDebug.Assert(atoms.Length == prot.Count+heme.Count);
                    Pdb.FromLines(prot.ToArray().ListLine()).ToFile("$PDBID$.v2.prot.pdb".Replace("$PDBID$", pdbid));
                    Pdb.FromLines(heme.ToArray().ListLine()).ToFile("$PDBID$.v2.heme.pdb".Replace("$PDBID$", pdbid));
                }
                public static void PdbXyz_05_Prm(string pdbid)
                {   /// charmm22-PDBID.prm
                    string   prm_name  = "charmm22-$PDBID$.prm".Replace("$PDBID$", pdbid);
                    string[] prm_lines = HResource.GetResourceLines<HemeProtein>(resbase+"charmm22-$PDBID$.prm");
                    HFile.WriteAllLines(prm_name, prm_lines);
                }
                public static void PdbXyz_06_HemeXyz(string pdbid)
                {   /// $PDBID$.v3.heme.v0.from.v2-heme.pdb
                    /// $PDBID$.v3.heme.v1.xyz
                    /// $PDBID$.v3.heme.v3.from.v3-heme-v2b.xyz
                    string prm_name = "charmm22-$PDBID$.prm"                .Replace("$PDBID$", pdbid);
                    string pdb_name = "$PDBID$.v3.heme.v0.from.v2-heme.pdb" .Replace("$PDBID$", pdbid);
                    string xyz_name = "$PDBID$.v3.heme.v1.xyz"              .Replace("$PDBID$", pdbid);
                    HFile.Copy("$PDBID$.v2.heme.pdb"                        .Replace("$PDBID$", pdbid)
                                , pdb_name
                                );
                    HProcess.StartInConsole(false, "pdbxyz.exe {0} {1}", pdb_name, prm_name);
                    HFile.Move((pdb_name+"$").Replace(".pdb$", ".xyz"), xyz_name);
                    string nxyz_name = "$PDBID$.v3.heme.v3.from.v3-heme-v2b.xyz".Replace("$PDBID$", pdbid);
                    {
                        Pdb pdb0 = Pdb.FromFile(pdb_name);
                        Xyz xyz0 = Xyz.FromFile(xyz_name, false);
                        Pdb pdbx = Pdb.FromLines(HResource.GetResourceLines<HemeProtein>(resbase+"heme.pdb"));
                        Xyz xyzx = Xyz.FromLines(HResource.GetResourceLines<HemeProtein>(resbase+"heme.xyz"));
                        var pdb0_atoms = pdb0.atoms;
                        var xyz0_atoms = xyz0.atoms;
                        var pdbx_atoms = pdbx.atoms;
                        var xyzx_atoms = xyzx.atoms;
                        int length = pdbx_atoms.Length;
                        HDebug.Assert(length == pdbx_atoms.Length);
                        HDebug.Assert(length == xyzx_atoms.Length);
                        HDebug.Assert(length == pdb0_atoms.Length);
                        HDebug.Assert(length == xyz0_atoms.Length);
                        for(int i=0; i<length; i++)
                        {
                            HDebug.AssertTolerance(0.001, (xyz0_atoms[i].Coord-pdb0_atoms[i].coord).Dist);
                            HDebug.AssertTolerance(0.001, (xyzx_atoms[i].Coord-pdbx_atoms[i].coord).Dist);
                            HDebug.Assert(pdb0_atoms[i].name     == pdbx_atoms[i].name    );
                        }
                        Xyz nxyz0 = xyzx.CloneByCoords(xyz0_atoms.HListCoords());
                        nxyz0.ToFile(nxyz_name, false);
                    }
                }
                public static void PdbXyz_07_ProtXyz(string pdbid)
                {   /// $PDBID$.v3.prot.v0.from.v2-prot.pdb
                    /// $PDBID$.v3.prot.v1.seq
                    /// $PDBID$.v3.prot.v1.xyz
                    string prm_name = "charmm22-$PDBID$.prm"                .Replace("$PDBID$", pdbid);
                    string pdb_name = "$PDBID$.v3.prot.v0.from.v2-prot.pdb" .Replace("$PDBID$", pdbid);
                    string seq_name = "$PDBID$.v3.prot.v1.seq"              .Replace("$PDBID$", pdbid);
                    string xyz_name = "$PDBID$.v3.prot.v1.xyz"              .Replace("$PDBID$", pdbid);
                    HFile.Copy("$PDBID$.v2.prot.pdb"                        .Replace("$PDBID$", pdbid)
                                , pdb_name
                                );
                    HProcess.StartInConsole(false, "pdbxyz.exe {0} {1}", pdb_name, prm_name);
                    HFile.Move((pdb_name+"$").Replace(".pdb$", ".seq"), seq_name);
                    HFile.Move((pdb_name+"$").Replace(".pdb$", ".xyz"), xyz_name);
                }
                public static void PdbXyz_08_ProtXyzMarkBind
                    ( string pdbid
                    , out string bind_xyz_name
                    , out int    bind_ID
                    )
                {   /// $PDBID$.v3.prot.v2.mark.atom$BINDID$.that.binds.FE.xyz
                    /// $PDBID$                     $BINDID$
                    var pdb = Pdb.FromFile("$PDBID$.v3.prot.v0.from.v2-prot.pdb" .Replace("$PDBID$", pdbid));
                    var xyz = Xyz.FromFile("$PDBID$.v3.prot.v1.xyz"              .Replace("$PDBID$", pdbid), false);
                    KDTreeDLL.KDTree<Pdb.Atom> kdtree = new KDTreeDLL.KDTree<Pdb.Atom>(3);
                    foreach(var pdbatom in pdb.atoms)
                        kdtree.insert(pdbatom.coord, pdbatom);
                    double bind_dist = double.PositiveInfinity;
                    int    bind_idx  = -1;
                    for(int i=0; i<xyz.elements.Length; i++)
                    {
                        var elem = xyz.elements[i];
                        if(elem is Tinker.Xyz.Atom)
                        {
                            var xyzatom = elem as Tinker.Xyz.Atom;
                            var pdbatom = kdtree.nearest(xyzatom.Coord);
                            double dist = (xyzatom.Coord - pdbatom.coord).Dist;
                            if(dist > 0.1)
                            {
                                if(bind_idx != -1)
                                {
                                    throw new Exception();
                                }
                                bind_idx  = i;
                                bind_dist = dist;
                            }
                        }
                    }
                    var bind_atom = xyz.elements[bind_idx] as Tinker.Xyz.Atom;
                    bind_ID = bind_atom.Id;
                    xyz.elements[bind_idx] = Tinker.Xyz.Atom.FromCoord(bind_atom, bind_atom.Coord + new Vector(900, 900, 900));
                    bind_xyz_name = "$PDBID$.v3.prot.v2.mark.atom$BINDID$.that.binds.FE.xyz".Replace("$PDBID$" , pdbid)
                                                                                            .Replace("$BINDID$", bind_ID.ToString());
                    xyz.ToFile(bind_xyz_name, false);
                }
                public static void PdbXyz_09_MergProtHemeXyz
                    ( string pdbid
                    , string bind_xyz_name
                    , int    bind_ID
                    )
                {   /// $PDBID$.v4.prot-heme.v0.merge.v3-prot-v2.and.v3-heme-v3.xyz
                    /// $PDBID$.v4.prot-heme.v1.xyz
                    /// $PDBID$.v4.prot-heme.v2.xyz
                    /// $PDBID$.v4.prot-heme.v3.xyz
                    string prot40_name = "$PDBID$.v4.prot-heme.v0.merge.v3-prot-v2.and.v3-heme-v3.xyz".Replace("$PDBID$", pdbid);
                    string prot41_name = "$PDBID$.v4.prot-heme.v1.xyz"                                .Replace("$PDBID$", pdbid);
                    string prot42_name = "$PDBID$.v4.prot-heme.v2.xyz"                                .Replace("$PDBID$", pdbid);
                    string prot43_name = "$PDBID$.v4.prot-heme.v3.xyz"                                .Replace("$PDBID$", pdbid);

                    string hemev3__name = "$PDBID$.v3.heme.v3.from.v3-heme-v2b.xyz"                   .Replace("$PDBID$", pdbid);
                    string[] prot_lines = HFile.ReadAllLines(bind_xyz_name);
                    string[] heme_lines = HFile.ReadAllLines(hemev3__name);
                    string[] merg_lines = prot_lines.HAddRange(heme_lines);
                    HFile.WriteAllLines(prot40_name, merg_lines);

                    {
                        var xyz = Tinker.Xyz.FromFile(prot40_name, false);
                        var id2atom = xyz.atoms.HSelectCorrectAtomType().ToIdDictionary();
                        int HE2_ID   = bind_ID;
                        var HE2_atom = id2atom[HE2_ID];
                        if(HE2_atom.BondedIds.Length != 1)
                            throw new Exception("$PDBID$.v4.prot-heme.v1.xyz");
                        int NE2_ID = HE2_atom.BondedId1.Value;
                        int FE_ID   = 9902;

                        /// $PDBID$.v4.prot-heme.v1.xyz
                        string HE2_sid = string.Format("{0,6}",HE2_ID);
                        string NE2_sid = string.Format("{0,6}", NE2_ID);
                        string  FE_sid = string.Format("{0,6}",  FE_ID);
                        int HE2_idx = -1;
                        for(int i=0; i<xyz.elements.Length; i++)
                        {
                            if(xyz.elements[i].type != "Atom") continue;
                            var atom = xyz.elements[i] as Tinker.Xyz.Atom;
                            if(atom.AtomType.Trim().Length == 0) continue;
                            if(atom.Id == NE2_ID) { atom = new Tinker.Xyz.Atom(atom.line.Replace(HE2_sid ,"  xxxx")); xyz.elements[i] = atom;              continue; }
                            if(atom.Id == HE2_ID) { atom = new Tinker.Xyz.Atom(atom.line.Replace(HE2_sid ,"  xxxx")); xyz.elements[i] = atom; HE2_idx = i; continue; }
                            if(atom.Id ==  FE_ID) { atom = new Tinker.Xyz.Atom(atom.line.Replace("  9901","  yyyy")); xyz.elements[i] = atom;              continue; }
                        }
                        xyz.ToFile(prot41_name, false);

                        /// $PDBID$.v4.prot-heme.v2.xyz
                        xyz.elements = xyz.elements.HRemoveAt(HE2_idx).ToArray();
                        for(int i=0; i<xyz.elements.Length; i++)
                        {
                            if(xyz.elements[i].type != "Atom") continue;
                            var atom = xyz.elements[i] as Tinker.Xyz.Atom;
                            if(atom.AtomType.Trim().Length == 0) continue;
                            if(atom.Id == NE2_ID) { atom = new Tinker.Xyz.Atom(atom.line.Replace("  xxxx", FE_sid)); xyz.elements[i] = atom; continue; }
                            if(atom.Id ==  FE_ID) { atom = new Tinker.Xyz.Atom(atom.line.Replace("  yyyy",NE2_sid)); xyz.elements[i] = atom; continue; }
                        }
                        xyz.ToFile(prot42_name, false);

                        /// $PDBID$.v4.prot-heme.v3.xyz
                        string[] prot42_lines = HFile.ReadAllLines(prot42_name);
                        string[] prot43_lines = prot42_lines.HRemoveAll("    73        ");
                        HFile.WriteAllLines(prot43_name, prot43_lines);
                    }
                }
                public static void PdbXyz_10_XyzMinimizable(string pdbid)
                {   /// $PDBID$.v5.prot-heme.reindex.xyz
                    /// $PDBID$.v6.prot-heme.ready-to-minimize.xyz
                    /// $PDBID$.v7.prot-heme.copy.from.v6.xyz
                    /// $PDBID$.v7.prot-heme.from.v1.reorderedto.xyz.pdb
                    string  prm_name = "charmm22-$PDBID$.prm"                               .Replace("$PDBID$", pdbid);
                    string pdb1_name = "$PDBID$.v1.psfgen.pdb"                              .Replace("$PDBID$", pdbid);
                    string xyz4_name = "$PDBID$.v4.prot-heme.v3.xyz"                        .Replace("$PDBID$", pdbid);
                    string xyz5_name = "$PDBID$.v5.prot-heme.reindex.xyz"                   .Replace("$PDBID$", pdbid);
                    string xyz6_name = "$PDBID$.v6.prot-heme.ready-to-minimize.xyz"         .Replace("$PDBID$", pdbid);
                    string xyz7_name = "$PDBID$.v7.prot-heme.copy.from.v6.xyz"              .Replace("$PDBID$", pdbid);
                    string pdb7_name = "$PDBID$.v7.prot-heme.from.v1.reorderedto.xyz.pdb"   .Replace("$PDBID$", pdbid);
                    {
                        Tinker.Xyz xyz;
                        xyz = Tinker.Xyz.FromFile(xyz4_name, false);
                        xyz = xyz.CloneByReindex(1);
                        xyz.ToFile(xyz5_name, false);
                        xyz.ToFile(xyz6_name, false);
                    }
                    {
                        Tinker.Xyz.FromFile(xyz6_name, false)
                                    .ToFile  (xyz7_name, false);
                        var pdb = Pdb.FromFile(pdb1_name);
                        var xyz = Tinker.Xyz.FromFile(xyz7_name, false);
                        var pdbatoms = pdb.atoms.CloneByReindexByCoords(xyz.atoms.HListCoords(), 1);
                        Pdb.ToFile(pdb7_name, pdbatoms);
                    }
                }
                public static void PdbXyz_11_KeyFile(string pdbid, bool fixheme, params string[] keycontents)
                {   /// $PDBID$.va8.prot-heme.Min.Newton.FixHeme.key
                    string[] fixheme_pdbnames
                        = Pdb.FromLines(HResource.GetResourceLines<HemeProtein>(resbase+"heme-fix.pdb"))
                            .atoms.ListName()
                            .HTrim()
                            .HToHashSet()
                            .ToArray();
                    int fixheme_count = fixheme_pdbnames.Length;

                    Vector[] fixheme_coords;
                    {
                        fixheme_coords = new Vector[fixheme_count];
                        string pdb7_name = "$PDBID$.v7.prot-heme.from.v1.reorderedto.xyz.pdb".Replace("$PDBID$", pdbid);
                        Pdb pdb7 = Pdb.FromFile(pdb7_name);
                        Pdb.Atom[] hemeatoms = pdb7.atoms.SelectByResName("HEM").ToArray();
                        var hemedict = hemeatoms.HToDictionaryWithKey(hemeatoms.ListName().HTrim());
                        for(int i=0; i<fixheme_count; i++)
                        {
                            string   name = fixheme_pdbnames[i];
                            Pdb.Atom atom = hemedict[name];
                            Vector  coord = atom.coord;
                            fixheme_coords[i] = coord;
                        }
                    }

                    List<string> key_lines_fixheme;
                    {
                        key_lines_fixheme = new List<string>();
                        string xyz7_name = "$PDBID$.v7.prot-heme.copy.from.v6.xyz"              .Replace("$PDBID$", pdbid);
                        Xyz    xyz7 = Xyz.FromFile(xyz7_name, false);
                        KDTreeDLL.KDTree<Xyz.Atom> kdtree = new KDTreeDLL.KDTree<Xyz.Atom>(3);
                        foreach(Xyz.Atom xyzatom in xyz7.atoms)
                            kdtree.insert(xyzatom.Coord, xyzatom);
                        for(int i=0; i<fixheme_count; i++)
                        {
                            Xyz.Atom xyzatom = kdtree.nearest(fixheme_coords[i]);
                            HDebug.AssertTolerance(0.01, (xyzatom.Coord - fixheme_coords[i]).Dist);
                            key_lines_fixheme.Add(string.Format(
                                "RESTRAIN-POSITION     {0,4}     {1,7:0.000} {2,7:0.000} {3,7:0.000}       10000"
                                , xyzatom.Id, xyzatom.X, xyzatom.Y, xyzatom.Z
                                ));
                        }
                    }

                    string  prm_name = "charmm22-$PDBID$.prm"                               .Replace("$PDBID$", pdbid);
                    string  key_name = "$PDBID$.va8.prot-heme.Min.Newton.FixHeme.key"       .Replace("$PDBID$", pdbid);
                    List<string> key_lines = new List<string>();
                    key_lines.Add(string.Format("parameters            {0}",prm_name));
                    key_lines.Add("");
                    if((keycontents != null) && (keycontents.Length > 0))
                    {
                        key_lines.AddRange(keycontents);
                        key_lines.Add("");
                    }
                    if(fixheme)
                    {
                        key_lines.AddRange(key_lines_fixheme);
                        key_lines.Add("");
                    }
                    HFile.WriteAllLines(key_name, key_lines);
                }
                public static void PdbXyz_12_Minimize(string pdbid, double rmsgrad)
                {   /// $PDBID$.va8.prot-heme.Min.Newton.FixHeme.bat
                    /// $PDBID$.va8.prot-heme.Min.Newton.FixHeme.xyz
                    string   bat_name = "$PDBID$.va8.prot-heme.Min.Newton.FixHeme.bat"      .Replace("$PDBID$", pdbid);
                    string[] bat_lines = HResource.GetResourceLines<HemeProtein>(resbase+"$PDBID$.va8.prot-heme.Min.Newton.FixHeme.bat")
                                                                                            .HReplace("$PDBID$", pdbid);
                    HFile.WriteAllLines(bat_name, bat_lines);


                    // Enter RMS Gradient per Atom Criterion [0.01] : rmsgrad
                    bat_lines[0] = bat_lines[0] + " A "+rmsgrad.ToString(); // " A 0.0001";

                    HProcess.StartAsBatchInConsole("newtonbat.bat", false, bat_lines);
                }
                public static void PdbXyz_13_MinimizedPdb(string pdbid)
                {   /// $PDBID$.va8.prot-heme.Min.Newton.FixHeme.pdb
                    // 3. minimize using newton with grad 0.0001
                    //    XXXX.va8.prot-heme.Min.Newton.FixHeme.bat
                    //    save to XXXX.va8.prot-heme.Min.Newton.FixHeme.xyz
                    string prm_name  = "charmm22-$PDBID$.prm"                               .Replace("$PDBID$", pdbid);
                    string xyz7_name = "$PDBID$.v7.prot-heme.copy.from.v6.xyz"              .Replace("$PDBID$", pdbid);
                    string pdb7_name = "$PDBID$.v7.prot-heme.from.v1.reorderedto.xyz.pdb"   .Replace("$PDBID$", pdbid);
                    string xyz8_name = "$PDBID$.va8.prot-heme.Min.Newton.FixHeme.xyz"       .Replace("$PDBID$", pdbid);
                    string pdb8_name = "$PDBID$.va8.prot-heme.Min.Newton.FixHeme.pdb"       .Replace("$PDBID$", pdbid);
                    {   // copy from XXXX.va8.prot-heme.Min.Newton.FixHeme.xyz
                        //        to XXXX.va8.prot-heme.Min.Newton.FixHeme.pdb
                        var pdb =        Pdb.FromFile(pdb7_name);
                        var xyz = Tinker.Xyz.FromFile(xyz7_name, false);
                        var prm = Tinker.Prm.FromFile(prm_name);
                        Universe univ = Universe.BuilderTinker.Build(xyz, prm, pdb, 0.0001);

                        xyz = Tinker.Xyz.FromFile(xyz8_name, false);
                        univ.SetCoords(xyz.atoms.HListCoords());

                        univ.SaveCoordsToPdb(pdb8_name);
                    }
                }
                public static void PdbXyz_14_ReferenceProtein_1A6G(string pdbid)
                {   /// 1A6G-psfgen-AltlocA.pdb
                    HFile.WriteAllLines("1A6G-psfgen-AltlocA.pdb"
                                        , HResource.GetResourceLines<HemeProtein>(resbase+"1A6G-psfgen-AltlocA.pdb")
                                        );
                }
                public static void PdbXyz_15_AlignToRef(string pdbid)
                {   /// $PDBID$.va9b1.prot-heme.Align.v7to1A6G.pdb
                    {   // 1. align using PyMol
                        if(HDebug.False)
                        {   // initially align before running "pymol align"
                            // a. load pdb structures, CA atoms
                            Pdb prot = Pdb.FromFile("$PDBID$.va8.prot-heme.Min.Newton.FixHeme.pdb".Replace("$PDBID$", pdbid));
                            Pdb tagt = Pdb.FromFile("1A6G-psfgen-AltlocA.pdb");
                            Pdb.Atom[] protcas = prot.atoms.SelectByName("CA", "FE").ToArray();
                            Pdb.Atom[] tagtcas = tagt.atoms.SelectByName("CA", "FE").ToArray();
                            // b. collect common CA atoms
                            var prot_resn_resi = protcas.ListResidue().HListResNameSeq();
                            var tagt_resn_resi = tagtcas.ListResidue().HListResNameSeq();
                            HashSet<string> HIS = (new string[] {"HSD", "HSE", "HSP"}).HToHashSet();
                            for(int i=0; i<prot_resn_resi.Length; i++) if(HIS.Contains(prot_resn_resi[i].Item1)) prot_resn_resi[i] = new Tuple<string, int>("HIS", prot_resn_resi[i].Item2);
                            for(int i=0; i<tagt_resn_resi.Length; i++) if(HIS.Contains(tagt_resn_resi[i].Item1)) tagt_resn_resi[i] = new Tuple<string, int>("HIS", tagt_resn_resi[i].Item2);
                            var lcs = Bioinfo.HSequence.LongCommSubseq.GetLongCommSubseq(prot_resn_resi, tagt_resn_resi);
                            // c. align with common atoms.
                        }
                        HFile.WriteAllLines("pymol.pml",
                            new string[]
                            { "load  $PDBID$.va8.prot-heme.Min.Newton.FixHeme.pdb, prot"        .Replace("$PDBID$", pdbid)
                            , "load  1A6G-psfgen-AltlocA.pdb"
                            , "align prot, 1A6G-psfgen-AltlocA"
                            , "save  $PDBID$.va9b1.prot-heme.Align.v7to1A6G.pdb, prot"          .Replace("$PDBID$", pdbid)
                            , "quit"
                            });
                        Pymol.Run("pymol.pml");
                    }
                    {   // 2. PyMol change the order of atoms.
                        //    Therefore, realign original protein to the pymol-aligned pdb file.
                        /// $PDBID$.va8.prot-heme.Min.Newton.FixHeme.pdb
                        /// $PDBID$.va8.prot-heme.Min.Newton.FixHeme.xyz
                        /// $PDBID$.va9b1.prot-heme.Align.v7to1A6G.pdb
                        /// $PDBID$.va9b2.prot-heme.AlignTo1A6G.xyz
                        /// $PDBID$.va9b2.prot-heme.AlignTo1A6G.pdb
                        string pdb8_name  = "$PDBID$.va8.prot-heme.Min.Newton.FixHeme.pdb"      .Replace("$PDBID$", pdbid);
                        string xyz8_name  = "$PDBID$.va8.prot-heme.Min.Newton.FixHeme.xyz"      .Replace("$PDBID$", pdbid);
                        string pdb9a_name = "$PDBID$.va9b1.prot-heme.Align.v7to1A6G.pdb"        .Replace("$PDBID$", pdbid);
                        string xyz9b_name = "$PDBID$.va9b2.prot-heme.AlignTo1A6G.xyz"           .Replace("$PDBID$", pdbid);
                        string pdb9b_name = "$PDBID$.va9b2.prot-heme.AlignTo1A6G.pdb"           .Replace("$PDBID$", pdbid);
                        {   /// reorder atom sequence in 1BINA.v9.prot-heme.align.to.1A6G-psfgen-AltlocA.pdb
                            ///    to make mathched with 1BINA.v8.prot-heme.minimize.netwon.grad0.0001.pdb
                            ///              and save to 1BINA.v10.prot-heme.reorder.pdb.atom.sequences.pdb
                            ///                      and 1BINA.v10.prot-heme.reorder.pdb.atom.sequences.xyz
                            IList<Pdb.Atom> pdb8cas = Pdb.FromFile(pdb8_name).atoms.SelectByName("CA");
                            IList<Pdb.Atom> pdb9cas = Pdb.FromFile(pdb9a_name).atoms.SelectByName("CA");

                            var trans = Align.MinRMSD.GetTrans(pdb9cas.ListCoord(), pdb8cas.ListCoord());

                            var xyz = Tinker.Xyz.FromFile(xyz8_name, false);
                            var coords = xyz.atoms.HListCoords();
                            trans.DoTransform(coords);
                            xyz = xyz.CloneByCoords(coords);
                            xyz.ToFile(xyz9b_name, false);

                            var pdb = Pdb.FromFile(pdb8_name);
                            pdb.ToFile(pdb9b_name, coords);
                        }
                    }
                }
                public static void PdbXyz_16_DeleteTemp(string pdbid)
                {
                    HFile.Delete("newtonbat.bat"           );
                    HFile.Delete("psfgen.bat"              );
                    HFile.Delete("pymol.pml"               );
                }








                /*
                        public static string[] PdbXyz_delete(string pdbid)
                        {
                            {
                            //
                            //    /// align (minimized structure) to (order-corrected initial structure)
                            //    in_vc9a_pdb_AlignedToRefConf = v7_pdb_ReorderPdb1ToXyz7; // order-corrected initial structure
                            //       vc9a_xyz_AlignedToPdb9    = @"XXXX.va9a.prot-heme.AlignMinimizedToXry.xyz".Replace("XXXX", replaceXXXXto);
                            //       vc9a_pdb_AlignedToPdb9    = @"XXXX.va9a.prot-heme.AlignMinimizedToXry.pdb".Replace("XXXX", replaceXXXXto);
                            //    {   /// reorder atom sequence in 1BINA.v9.prot-heme.align.to.1A6G-psfgen-AltlocA.pdb
                            //        ///    to make mathched with 1BINA.v8.prot-heme.minimize.netwon.grad0.0001.pdb
                            //        ///              and save to 1BINA.v10.prot-heme.reorder.pdb.atom.sequences.pdb
                            //        ///                      and 1BINA.v10.prot-heme.reorder.pdb.atom.sequences.xyz
                            //        IList<Pdb.Atom> pdb8cas = Pdb.FromFile(pathbase+vc8_pdb_Minimized).atoms.SelectByName("CA");
                            //        IList<Pdb.Atom> pdb9cas = Pdb.FromFile(pathbase+in_vc9a_pdb_AlignedToRefConf).atoms.SelectByName("CA");
                            //
                            //        var trans = Align.MinRMSD.GetTrans(pdb9cas.ListCoord(), pdb8cas.ListCoord());
                            //
                            //        var xyz = Tinker.Xyz.FromFile(pathbase+in_vc8_xyz_Minimized, false);
                            //        var coords = xyz.atoms.HListCoords();
                            //        trans.DoTransform(coords);
                            //        xyz = xyz.CloneByCoords(coords);
                            //        xyz.ToFile(pathbase+vc9a_xyz_AlignedToPdb9, false);
                            //
                            //        var pdb = Pdb.FromFile(pathbase+vc8_pdb_Minimized);
                            //            pdb.ToFile(pathbase+vc9a_pdb_AlignedToPdb9,coords);
                            //    }
                            }
                            return null;
                            throw new NotImplementedException();
                            {
                                string cachebase = null;
                                string replaceXXXXto = null;
                                {
                                    string xyz4_name = null;
                                    {   /// XXXX.v5-v10.Program.cs
                                        /// XXXX.v5.prot-heme.reindex.xyz
                                        /// XXXX.v6.prot-heme.ready-to-minimize.xyz
                                        /// XXXX.v7.prot-heme.copy.from.v6.xyz
                                        /// XXXX.v7.prot-heme.from.v1.reorderedto.xyz.pdb
                                        /// XXXX.v7.prot-heme.copy.from.v6.xyz.extract-heme.pdb
                                        /// XXXX.va8.prot-heme.Min.Newton.FixHeme.key
                                        /// XXXX.va8.prot-heme.Min.Newton.FixHeme.bat
                                        /// XXXX.va8.prot-heme.Min.Newton.FixHeme.xyz
                                        /// XXXX.va8.prot-heme.Min.Newton.FixHeme.pdb
                                        /// XXXX.va9a.prot-heme.AlignMinimizedToXry.xyz
                                        /// XXXX.va9a.prot-heme.AlignMinimizedToXry.pdb
                                        string   prog_name  = "XXXX.v5-v10.Program.cs".Replace("XXXX",replaceXXXXto);
                                        string[] prog_lines = HResource.GetResourceLines<HemeProtein>(resbase+"XXXX.v5-v10.Program.cs")
                                                              .HReplace("XXXX", replaceXXXXto);
                                        HFile.WriteAllLines(prog_name, prog_lines);

                                        var prog = new XXXX_v5_v10_Program_run_cs(HEnvironment.CurrentDirectory+"\\", replaceXXXXto);
                                        {   // XXXX.va9b1.prot-heme.Align.v7to1A6G.pdb
                                            // XXXX.va9b2.prot-heme.AlignTo1A6G.pdb
                                            // XXXX.va9b2.prot-heme.AlignTo1A6G.xyz
                                            HFile.WriteAllLines("pymol.pml",
                                                new string[]
                                                { "load  XXXX.va8.prot-heme.Min.Newton.FixHeme.pdb, prot".Replace("XXXX", replaceXXXXto)
                                                , "load  1A6G-psfgen-AltlocA.pdb"
                                                , "align prot, 1A6G-psfgen-AltlocA"
                                                , "save  XXXX.va9b1.prot-heme.Align.v7to1A6G.pdb, prot".Replace("XXXX", replaceXXXXto)
                                                , "quit"
                                                });
                                            Pymol.Run("pymol.pml");

                                            prog.Main_step3();
                                        }
                                    }
                                    {   /// pdbxyz.bat
                                        /// pdbxyz.exe
                                        /// newtonbat.bat
                                        /// pymol.pml
                                        /// XXXX.v7.prot-heme.copy.from.v6.xyz_2
                                        HFile.Delete("pdbxyz.bat"   );
                                        HFile.Delete("newtonbat.bat");
                                        HFile.Delete("pymol.pml"    );
                                        HFile.Delete("XXXX.v7.prot-heme.copy.from.v6.xyz_2".Replace("XXXX", replaceXXXXto));
                                    }
                                }
                                return new string[]
                                {
                                    cachebase+"XXXX.va9b2.prot-heme.AlignTo1A6G.pdb".Replace("XXXX", replaceXXXXto),
                                    cachebase+"XXXX.va9b2.prot-heme.AlignTo1A6G.xyz".Replace("XXXX", replaceXXXXto),
                                    cachebase+"charmm22-XXXX.prm"                   .Replace("XXXX", replaceXXXXto),
                                };
                            }
                        }
                */
            }
        }
    }
}