/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
	{
        public class Residue
        {
            public string resName  = "";
            public int    resSeq   = -1;
            public Atom[] resAtoms = new Atom[0];
            public override string ToString()
            {
                return string.Format("{0} {1} : {2} atoms", resSeq, resName, resAtoms.Length);
            }
        }
        public Residue[] CollectResidues()
        {
            List<Residue> residues = new List<Residue>();
            residues.Add(new Residue());
            residues.Last().resName = atoms[0].resName;
            residues.Last().resSeq  = atoms[0].resSeq ;
            foreach(Atom atom in atoms)
            {
                if(residues.Last().resSeq != atom.resSeq)
                {
                    residues.Add(new Residue());
                    residues.Last().resName = atom.resName;
                    residues.Last().resSeq  = atom.resSeq;
                }
                List<Atom> resAtoms = new List<Atom>(residues.Last().resAtoms);
                resAtoms.Add(atom);
                residues.Last().resAtoms = resAtoms.ToArray();
            }
            return residues.ToArray();
        }
    }
}
*/