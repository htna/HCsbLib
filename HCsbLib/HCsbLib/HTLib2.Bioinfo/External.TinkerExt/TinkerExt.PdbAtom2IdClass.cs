using System;
using System.Collections.Generic;
using System.Linq;

namespace HTLib2.Bioinfo
{
    using Prm = Tinker.Prm;
    public static partial class TinkerExt
    {
        public static class PdbAtom2IdClass
        {
            public static (int id, int cls)[] FromFile
                ( string pdbpath
                , string prmpath
                , string defaultHistidine = "Histidine (HE)"
                , double disulfideCutoff = 2.5
                )
            {
                Pdb pdb = Pdb.FromFile(pdbpath);
                Prm prm = Prm.FromFile(prmpath);

                return FromPdbPrm
                (
                    pdb.atoms,
                    prm,
                    defaultHistidine,
                    disulfideCutoff
                );
            }


            public static (int id, int cls)[] FromPdbPrm
                ( Pdb.Atom[] atoms
                , Prm prm
                , string defaultHistidine = "Histidine (HE)"
                , double disulfideCutoff = 2.5
                )
            {
                HDebug.Assert(atoms != null);
                HDebug.Assert(prm   != null);
                HDebug.Assert(disulfideCutoff > 0);

                ////////////////////////////////////////////////////////////
                // biotype:
                //     (atom name, Tinker residue name) -> Tinker atom id
                ////////////////////////////////////////////////////////////
                Dictionary<(string name, string resn), int> biotype_id = new Dictionary<(string name, string resn), int>();
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

                ////////////////////////////////////////////////////////////
                // Group atoms into residues
                ////////////////////////////////////////////////////////////
                Dictionary<(char chain, int resSeq, char iCode), Pdb.Atom[]> residues
                    = atoms
                    .GroupBy     (atom  => (atom.chainID, atom.resSeq, atom.iCode))
                    .ToDictionary(group => group.Key, group => group.ToArray()    );

                ////////////////////////////////////////////////////////////
                // Detect disulfide Cys residues
                ////////////////////////////////////////////////////////////
                HashSet<(char chain, int resSeq, char iCode)> disulfides = GetDisulfideResidues(residues, disulfideCutoff);

                ////////////////////////////////////////////////////////////
                // N/C terminal residue for each chain
                ////////////////////////////////////////////////////////////
                Dictionary<char, (int resSeq, char iCode)> nterminal = new Dictionary<char, (int resSeq, char iCode)>();
                Dictionary<char, (int resSeq, char iCode)> cterminal = new Dictionary<char, (int resSeq, char iCode)>();

                foreach(Pdb.Atom atom in atoms)
                {
                    char chain = atom.chainID;

                    var resid = ( resSeq: atom.resSeq
                                , iCode : atom.iCode
                                );

                    if(nterminal.ContainsKey(chain) == false)
                        nterminal.Add(chain, resid);

                    cterminal[chain] = resid;
                }

                ////////////////////////////////////////////////////////////
                // Assign Tinker id/class
                ////////////////////////////////////////////////////////////
                (int id, int cls)[] idclass = new (int id, int cls)[atoms.Length];

                for(int i=0; i<atoms.Length; i++)
                {
                    Pdb.Atom atom = atoms[i];

                    var resid =
                        (
                            chain : atom.chainID,
                            resSeq: atom.resSeq,
                            iCode : atom.iCode
                        );

                    Pdb.Atom[] resatoms = residues[resid];

                    string tinkerResn =
                        GetTinkerResidueName
                        ( atom.resName
                        , resatoms
                        , disulfides.Contains(resid)
                        , defaultHistidine
                        );

                    bool isNterminal =
                        (
                            atom.resSeq == nterminal[atom.chainID].resSeq &&
                            atom.iCode  == nterminal[atom.chainID].iCode
                        );

                    bool isCterminal =
                        (
                            atom.resSeq == cterminal[atom.chainID].resSeq &&
                            atom.iCode  == cterminal[atom.chainID].iCode
                        );

                    int? id = FindBiotypeId
                              ( atom
                              , tinkerResn
                              , isNterminal
                              , isCterminal
                              , biotype_id
                              );

                    if(id == null)
                    {
                        HDebug.Assert(false);
                        idclass[i] = (0,0);
                        // HDebug.Exception
                        // (
                        //     id != null,
                        //     string.Format
                        //     (
                        //         "Cannot find Tinker biotype: " +
                        //         "{0} {1} {2}{3} {4} [{5}]",
                        //         atom.chainID,
                        //         atom.resName.Trim(),
                        //         atom.resSeq,
                        //         atom.iCode,
                        //         atom.name.Trim(),
                        //         tinkerResn
                        //     )
                        // );
                    }
                    else
                    {
                        Prm.Atom prmatom = prm.IdToAtom(id.Value);

                        idclass[i] =
                        (
                            prmatom.Id,
                            prmatom.Class
                        );
                    }
                }

                return idclass;
            }

            ////////////////////////////////////////////////////////////////
            // Detect cystines from SG-SG distance
            ////////////////////////////////////////////////////////////////
            private static HashSet<(char chain, int resSeq, char iCode)>
                GetDisulfideResidues
                ( Dictionary<(char chain, int resSeq, char iCode), Pdb.Atom[]> residues
                , double cutoff
                )
            {
                List<((char chain, int resSeq, char iCode) resid, Pdb.Atom sg)> cys_sg = new List<((char chain, int resSeq, char iCode) resid, Pdb.Atom sg)>();

                foreach(var residue in residues)
                {
                    string resname = residue.Value[0].resName.Trim().ToUpper();

                    if(resname != "CYS" &&
                       resname != "CYX" &&
                       resname != "CYM")
                        continue;

                    Pdb.Atom sg = null;
                    foreach(Pdb.Atom atom in residue.Value)
                    {
                        if(atom.name.Trim().ToUpper() == "SG")
                        {
                            sg = atom;
                            break;
                        }
                    }
                    if(sg != null)
                        cys_sg.Add((residue.Key, sg));
                }

                HashSet<(char chain, int resSeq, char iCode)> disulfides = new HashSet<(char chain, int resSeq, char iCode)>();

                double cutoff2 = cutoff * cutoff;
                for(int i=0; i<cys_sg.Count; i++)
                for(int j=i+1; j<cys_sg.Count; j++)
                {
                    var cys1 = cys_sg[i];
                    var cys2 = cys_sg[j];

                    double dx = cys1.sg.x - cys2.sg.x;
                    double dy = cys1.sg.y - cys2.sg.y;
                    double dz = cys1.sg.z - cys2.sg.z;

                    double dist2 = dx*dx + dy*dy + dz*dz;
                    if(dist2 <= cutoff2)
                    {
                        disulfides.Add(cys1.resid);
                        disulfides.Add(cys2.resid);
                    }
                }

                return disulfides;
            }

            ////////////////////////////////////////////////////////////////
            // Determine Tinker residue name
            ////////////////////////////////////////////////////////////////
            private static string GetTinkerResidueName
                ( string pdbResName
                , Pdb.Atom[] residueAtoms
                , bool isDisulfide
                , string defaultHistidine
                )
            {
                string resn = pdbResName.Trim().ToUpper();

                switch(resn)
                {
                    case "ALA": return "Alanine";
                    case "ARG": return "Arginine";
                    case "ASN": return "Asparagine";
                    case "ASP": return "Aspartic Acid";
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

                    ////////////////////////////////////////////////////////
                    // Cysteine
                    ////////////////////////////////////////////////////////

                    case "CYX": return "Cystine (SS)";
                    case "CYM": return "Cysteine (S-)";
                    case "CYS":
                        if(isDisulfide) return "Cystine (SS)";
                        else            return "Cysteine (SH)";

                    ////////////////////////////////////////////////////////
                    // Explicit histidine naming
                    ////////////////////////////////////////////////////////

                    case "HSD":
                    case "HID":
                        return "Histidine (HD)";

                    case "HSE":
                    case "HIE":
                        return "Histidine (HE)";

                    case "HSP":
                    case "HIP":
                        return "Histidine (+)";

                    ////////////////////////////////////////////////////////
                    // Generic HIS:
                    // infer state from explicit ring hydrogens.
                    ////////////////////////////////////////////////////////

                    case "HIS":
                    {
                        bool hasHD1 = HasAtom(residueAtoms, "HD1");
                        bool hasHE2 = HasAtom(residueAtoms, "HE2");

                        if(hasHD1 && hasHE2) return "Histidine (+)";
                        if(hasHD1          ) return "Histidine (HD)";
                        if(hasHE2          ) return "Histidine (HE)";

                        // A hydrogen-free PDB does not contain enough
                        // information to distinguish HID from HIE.
                        return defaultHistidine;
                    }
                }

                HDebug.Assert(false);
                return pdbResName.Trim();
            }

            private static bool HasAtom(Pdb.Atom[] atoms, string atomname)
            {
                atomname = atomname.Trim().ToUpper();

                foreach(Pdb.Atom atom in atoms)
                {
                    if(atom.name.Trim().ToUpper() == atomname)
                        return true;
                }
                return false;
            }

            ////////////////////////////////////////////////////////////////
            // Find biotype
            ////////////////////////////////////////////////////////////////
            private static int? FindBiotypeId
                ( Pdb.Atom atom
                , string resn
                , bool isNterminal
                , bool isCterminal
                , Dictionary<(string name, string resn), int> biotype_id
                )
            {
                string atomname = atom.name.Trim().ToUpper();

                List<string> atomnames = GetTinkerAtomNameCandidates(atomname, isCterminal);

                ////////////////////////////////////////////////////////////
                // Normal residue name
                ////////////////////////////////////////////////////////////
                string normalresn = resn.ToUpper();

                ////////////////////////////////////////////////////////////
                // Terminal residue name
                ////////////////////////////////////////////////////////////
                string terminalresn = GetTinkerTerminalResidueName(atom.resName, resn);

                List<string> resnames = new List<string>();
                if(isNterminal) resnames.Add(("N-Terminal "+terminalresn).ToUpper());
                if(isCterminal) resnames.Add(("C-Terminal "+terminalresn).ToUpper());

                // Side-chain atoms generally continue to use
                // the ordinary residue biotype.
                resnames.Add(normalresn);

                ////////////////////////////////////////////////////////////
                // Exact atom name is always tried before progressively
                // simplified names.
                ////////////////////////////////////////////////////////////
                foreach(string residuename in resnames)
                foreach(string name        in atomnames)
                {
                    var key = (name, residuename);

                    if(biotype_id.ContainsKey(key))
                        return biotype_id[key];
                }

                HDebug.Assert(false);
                return null;
            }

            private static List<string> GetTinkerAtomNameCandidates(string atomname, bool isCterminal)
            {
                atomname = atomname.Trim().ToUpper();
                List<string> names = new List<string>();

                ////////////////////////////////////////////////////////////
                // Exact PDB atom name first.
                ////////////////////////////////////////////////////////////
                names.Add(atomname);

                ////////////////////////////////////////////////////////////
                // Peptide nitrogen hydrogen.
                //
                // PDB:
                //     H
                //     H1, H2, H3 at N-terminus
                //
                // Tinker biotype:
                //     HN
                ////////////////////////////////////////////////////////////
                if(atomname == "H"  ||
                   atomname == "H1" ||
                   atomname == "H2" ||
                   atomname == "H3")
                {
                    if(names.Contains("HN") == false)
                        names.Add("HN");
                }

                ////////////////////////////////////////////////////////////
                // C-terminal oxygen.
                //
                // pdbxyz accepts:
                //
                //     OXT
                //     OT2
                //
                // Tinker terminal biotype is OXT.
                ////////////////////////////////////////////////////////////
                if(isCterminal)
                {
                    if(atomname == "O"   ||
                       atomname == "OXT" ||
                       atomname == "OT2")
                    {
                        if(names.Contains("OXT") == false)
                            names.Add("OXT");
                    }
                }

                ////////////////////////////////////////////////////////////
                // Progressively remove trailing numbers.
                //
                // Examples:
                //
                // HB2  -> HB
                // HD11 -> HD1 -> HD
                // CD1  -> CD
                // NH1  -> NH
                //
                // This is attempted only AFTER the exact atom name.
                //
                // Therefore:
                //
                // TRP CD1 -> CD1        (exact match)
                // LEU CD1 -> CD1        (exact match)
                // ILE CD1 -> CD         (fallback)
                // PHE CD1 -> CD         (fallback)
                ////////////////////////////////////////////////////////////
                string name = atomname;
                while(name.Length > 1 && char.IsDigit(name[name.Length-1]))
                {
                    name = name.Substring(0, name.Length-1);
                    if(names.Contains(name) == false)
                        names.Add(name);
                }

                return names;
            }

            private static string GetTinkerTerminalResidueName
                ( string pdbresname
                , string tinkerResn
                )
            {
                string resn = pdbresname.Trim().ToUpper();

                switch(resn)
                {
                    case "ALA": return "ALA";
                    case "ARG": return "ARG";
                    case "ASN": return "ASN";
                    case "ASP": return "ASP";
                    case "GLN": return "GLN";
                    case "GLU": return "GLU";
                    case "GLY": return "GLY";
                    case "ILE": return "ILE";
                    case "LEU": return "LEU";
                    case "LYS": return "LYS";
                    case "MET": return "MET";
                    case "PHE": return "PHE";
                    case "PRO": return "PRO";
                    case "SER": return "SER";
                    case "THR": return "THR";
                    case "TRP": return "TRP";
                    case "TYR": return "TYR";
                    case "VAL": return "VAL";

                    // Cysteine states used by Tinker
                    case "CYS":
                        if(tinkerResn == "Cystine (SS)")
                            return "CYX (SS)";
                        if(tinkerResn == "Cysteine (S-)")
                            return "CYD (S-)";
                        return "CYS (SH)";

                    case "CYX":
                        return "CYX (SS)";

                    case "CYM":
                    case "CYD":
                        return "CYD (S-)";

                    // Histidine states
                    case "HIS":
                    case "HID":
                    case "HSD":
                    case "HIE":
                    case "HSE":
                    case "HIP":
                    case "HSP":
                        if(tinkerResn == "Histidine (+)")
                            return "HIS (+)";
                        if(tinkerResn == "Histidine (HD)")
                            return "HIS (HD)";
                        if(tinkerResn == "Histidine (HE)")
                            return "HIS (HE)";
                        break;
                }

                HDebug.Assert(false);
                return resn;
            }
        }
    }
}
