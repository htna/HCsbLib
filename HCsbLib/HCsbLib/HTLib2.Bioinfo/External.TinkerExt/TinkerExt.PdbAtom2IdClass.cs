using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Prm = Tinker.Prm;
    public static partial class TinkerExt
    {
        public static class PdbIdClass
        {
            public static (int id, int cls)[] FromFile
                (string pdbpath, string prmpath)
            {
                Pdb pdb = Pdb.FromFile(pdbpath);
                Prm prm = Prm.FromFile(prmpath);

                return FromPdbPrm(pdb, prm);
            }


            public static (int id, int cls)[] FromPdbPrm
                (Pdb pdb, Prm prm)
            {
                HDebug.Assert(pdb != null);
                HDebug.Assert(prm != null);

                Pdb.Atom[] atoms = pdb.atoms;

                // --------------------------------------------------------
                // biotype:
                //
                //     (atom name, residue name) -> Tinker atom id
                //
                // --------------------------------------------------------

                Dictionary<(string name, string resn), int> biotype_id
                    = new Dictionary<(string name, string resn), int>();

                foreach(Prm.Biotype biotype in prm.biotypes)
                {
                    var key =
                    (
                        biotype.Name.Trim().ToUpper(),
                        biotype.Resn.Trim().ToUpper()
                    );

                    HDebug.Assert(biotype_id.ContainsKey(key) == false);

                    biotype_id.Add(key, biotype.Id);
                }


                // --------------------------------------------------------
                // Determine first and last residue of each chain.
                // --------------------------------------------------------

                Dictionary<char, (int resSeq, char iCode)> nterminal
                    = new Dictionary<char, (int resSeq, char iCode)>();

                Dictionary<char, (int resSeq, char iCode)> cterminal
                    = new Dictionary<char, (int resSeq, char iCode)>();

                foreach(Pdb.Atom atom in atoms)
                {
                    char chain = atom.chainID;
                    var res = (atom.resSeq, atom.iCode);

                    if(nterminal.ContainsKey(chain) == false)
                        nterminal.Add(chain, res);

                    cterminal[chain] = res;
                }


                // --------------------------------------------------------
                // Assign Tinker (id,class) to each PDB atom.
                // --------------------------------------------------------

                (int id, int cls)[] idclass
                    = new (int id, int cls)[atoms.Length];


                for(int i=0; i<atoms.Length; i++)
                {
                    Pdb.Atom atom = atoms[i];

                    bool isNterminal =
                        (atom.resSeq == nterminal[atom.chainID].resSeq &&
                         atom.iCode  == nterminal[atom.chainID].iCode);

                    bool isCterminal =
                        (atom.resSeq == cterminal[atom.chainID].resSeq &&
                         atom.iCode  == cterminal[atom.chainID].iCode);


                    string resn =
                        GetTinkerResidueName(atom.resName);


                    int? id =
                        FindBiotypeId
                        (
                            atom,
                            resn,
                            isNterminal,
                            isCterminal,
                            biotype_id
                        );


                    HDebug.Exception
                    (
                        id == null,
                        string.Format
                        (
                            "Cannot find Tinker biotype: {0} {1} {2} {3}",
                            atom.chainID,
                            atom.resName.Trim(),
                            atom.resSeq,
                            atom.name.Trim()
                        )
                    );


                    Prm.Atom prmatom =
                        prm.IdToAtom(id.Value);


                    HDebug.Assert(prmatom.Id == id.Value);


                    idclass[i] =
                    (
                        prmatom.Id,
                        prmatom.Class
                    );
                }


                return idclass;
            }


            private static int? FindBiotypeId
            (
                Pdb.Atom atom,
                string resn,
                bool isNterminal,
                bool isCterminal,
                Dictionary<(string name, string resn), int> biotype_id
            )
            {
                string atomname =
                    atom.name.Trim().ToUpper();


                // --------------------------------------------------------
                // Candidate atom names.
                //
                // Usually the PDB name and Tinker biotype name are
                // identical. H/H1/H2/H3 -> HN is useful for protein
                // backbone hydrogens in some Tinker parameter sets.
                // --------------------------------------------------------

                List<string> atomnames =
                    new List<string>();

                atomnames.Add(atomname);

                if(atomname == "H"  ||
                   atomname == "H1" ||
                   atomname == "H2" ||
                   atomname == "H3")
                {
                    atomnames.Add("HN");
                }


                // --------------------------------------------------------
                // Candidate residue names.
                //
                // Try terminal biotypes first, followed by the ordinary
                // residue biotype. This is important because side-chain
                // atoms of terminal residues may still use the ordinary
                // residue biotype.
                // --------------------------------------------------------

                List<string> resnames =
                    new List<string>();

                if(isNterminal)
                    resnames.Add("N-TERMINAL " + resn.ToUpper());

                if(isCterminal)
                    resnames.Add("C-TERMINAL " + resn.ToUpper());

                resnames.Add(resn.ToUpper());


                foreach(string resname in resnames)
                foreach(string name    in atomnames)
                {
                    var key = (name, resname);

                    if(biotype_id.ContainsKey(key))
                        return biotype_id[key];
                }


                return null;
            }


            private static string GetTinkerResidueName(string pdbresname)
            {
                pdbresname = pdbresname.Trim().ToUpper();

                switch(pdbresname)
                {
                    case "ALA": return "Alanine";
                    case "ARG": return "Arginine";
                    case "ASN": return "Asparagine";
                    case "ASP": return "Aspartic Acid";
                    case "CYS": return "Cysteine (SH)";
                    case "GLN": return "Glutamine";
                    case "GLU": return "Glutamic Acid";
                    case "GLY": return "Glycine";
                    case "ILE": return "Isoleucine";
                    case "LEU": return "Leucine";
                    case "LYS": return "Lysine";
                    case "MET": return "Methionine";
                    case "PHE": return "Phenylalanine";
                    case "PRO": return "Proline";
                    case "SER": return "Serine";
                    case "THR": return "Threonine";
                    case "TRP": return "Tryptophan";
                    case "TYR": return "Tyrosine";
                    case "VAL": return "Valine";


                    // Histidine protonation states
                    case "HSD":
                    case "HID":
                        return "Histidine (HD)";

                    case "HSE":
                    case "HIE":
                        return "Histidine (HE)";

                    case "HSP":
                    case "HIP":
                        return "Histidine (+)";


                    // Other common protonation/state names
                    case "CYX": return "Cystine (SS)";
                    case "ASH": return "Aspartic Acid (COOH)";
                    case "GLH": return "Glutamic Acid (COOH)";
                    case "LYN": return "Lysine (NH2)";

                    default:
                        return pdbresname;
                }
            }
        }
    }
}
