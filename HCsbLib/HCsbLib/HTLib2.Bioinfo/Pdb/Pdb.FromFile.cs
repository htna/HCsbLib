using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        public static void FromFile(string filepath, out Pdb[] pdbs)
        {
            string[] lines = System.IO.File.ReadAllLines(filepath);
            Element[] elements = new Element[lines.Length];
            for(int i=0; i<lines.Length; i++)
                elements[i] = new Element(lines[i]);

            elements = elements.UpdateElement();

            List<List<Element>> ensembles;
            {
                ensembles = new List<List<Element>>();
                ensembles.Add(new List<Element>());
                for(int i=0; i<elements.Length; i++)
                {
                    //ensembles.Last().Add(elements[i]);
                    if(Atom.IsAtom(elements[i].line    )) ensembles.Last().Add(elements[i]);
                    if(Endmdl.IsEndmdl(elements[i].line)) ensembles.Add(new List<Element>());
                }
                if(ensembles.Last().Count == 0)
                    ensembles.Remove(ensembles.Last());
            }

            pdbs = new Pdb[ensembles.Count];
            for(int i=0; i<ensembles.Count; i++)
                pdbs[i] = new Pdb(ensembles[i].ToArray());
        }
        
        public static Pdb FromFile(string filepath)
		{
            System.IO.StreamReader reader = new System.IO.StreamReader(filepath);
            Pdb pdb = FromStream(reader);
            reader.Close();
            return pdb;
			//string[] lines = System.IO.File.ReadAllLines(filepath);
            //Element[] elements = new Element[lines.Length];
            //for(int i=0; i<lines.Length; i++)
            //    elements[i] = new Element(lines[i]);
            //
            //UpdateElements(elements);
            ////UpdateAtoms  (elements);
            ////UpdateHelixes(elements);
            ////UpdateSheets (elements);
            ////UpdateHetatms(elements);
            ////UpdateAnisous(elements);
            ////UpdateSiguijs(elements);
            //
			//return new Pdb(elements);
		}
        public static List<Tuple<Atom, int>> UpdateAtoms(Element[] elements)
		{
			int[] collection = Collect(elements, Atom.RecordName);
            List<Tuple<Atom ,int>> atoms = new List<Tuple<Atom, int>>();
			foreach(int idx in collection)
			{
                string line = elements[idx].line;
				Atom atom = Atom.FromString(line);
                elements[idx] = atom;
				atoms.Add(new Tuple<Atom,int>(atom,idx));
			}
			return atoms;
		}
        public static List<Tuple<Seqres, int>> UpdateSeqress(Element[] elements)
		{
            int[] collection = Collect(elements, Seqres.RecordName);
            List<Tuple<Seqres,int>> seqress = new List<Tuple<Seqres, int>>();
			foreach(int idx in collection)
			{
                string line = elements[idx].line;
                Seqres seqres = Seqres.FromString(line);
                elements[idx] = seqres;
                seqress.Add(new Tuple<Seqres, int>(seqres, idx));
			}
			return seqress;
		}
        public static List<Tuple<Seqadv, int>> UpdateSeqadvs(Element[] elements)
		{
            int[] collection = Collect(elements, Seqadv.RecordName);
            List<Tuple<Seqadv,int>> Seqadvs = new List<Tuple<Seqadv, int>>();
			foreach(int idx in collection)
			{
                string line = elements[idx].line;
                Seqadv seqadv = Seqadv.FromString(line);
                elements[idx] = seqadv;
                Seqadvs.Add(new Tuple<Seqadv, int>(seqadv, idx));
			}
			return Seqadvs;
		}
        public static List<Tuple<Helix, int>> UpdateHelixes(Element[] elements)
		{
			int[] collection = Collect(elements, Helix.RecordName);
            List<Tuple<Helix,int>> helixes = new List<Tuple<Helix, int>>();
			foreach(int idx in collection)
			{
                string line = elements[idx].line;
                Helix helix = Helix.FromString(line);
                elements[idx] = helix;
                helixes.Add(new Tuple<Helix,int>(helix,idx));
			}
			return helixes;
		}
        public static List<Tuple<Sheet, int>> UpdateSheets(Element[] elements)
		{
			int[] collection = Collect(elements, Sheet.RecordName);
            List<Tuple<Sheet,int>> sheets = new List<Tuple<Sheet, int>>();
			foreach(int idx in collection)
			{
                string line = elements[idx].line;
                Sheet sheet = Sheet.FromString(line);
                elements[idx] = sheet;
                sheets.Add(new Tuple<Sheet,int>(sheet,idx));
			}
			return sheets;
		}
        public static List<Tuple<Hetatm, int>> UpdateHetatms(Element[] elements)
		{
            int[] collection = Collect(elements, Hetatm.RecordName);
            List<Tuple<Hetatm ,int>> hetatms = new List<Tuple<Hetatm, int>>();
			foreach(int idx in collection)
			{
                string line = elements[idx].line;
                Hetatm hetatm = Hetatm.FromString(line);
                elements[idx] = hetatm;
                hetatms.Add(new Tuple<Hetatm, int>(hetatm, idx));
			}
			return hetatms;
		}
        public static List<Tuple<Anisou, int>> UpdateAnisous(Element[] elements)
        {
            int[] collection = Collect(elements, Anisou.RecordName);
            List<Tuple<Anisou ,int>> anisous = new List<Tuple<Anisou, int>>();
            foreach(int idx in collection)
            {
                string line = elements[idx].line;
                Anisou anisou = Anisou.FromString(line);
                elements[idx] = anisou;
                anisous.Add(new Tuple<Anisou, int>(anisou, idx));
            }
            return anisous;
        }
        public static List<Tuple<Siguij, int>> UpdateSiguijs(Element[] elements)
        {
            int[] collection = Collect(elements, Siguij.RecordName);
            List<Tuple<Siguij ,int>> siguijs = new List<Tuple<Siguij, int>>();
            foreach(int idx in collection)
            {
                string line = elements[idx].line;
                Siguij siguij = Siguij.FromString(line);
                elements[idx] = siguij;
                siguijs.Add(new Tuple<Siguij, int>(siguij, idx));
            }
            return siguijs;
        }
        //public static Residues BuildResidue(List<Atom> atoms)
		//{
		//    Residues residues = new Residues();
		//    List<Atom> atoms_ = new List<Atom>(atoms);
		//    while(atoms_.Count > 0)
		//    {
		//        int resSeq = atoms_[0].resSeq;
		//        Residue residue = Residue.FromAtoms(atoms_.TakeWhile(delegate(Atom atom) { return (atom.resSeq == resSeq); }));
		//        residues.Add(resSeq, residue);
		//        atoms_.RemoveAll(delegate(Atom atom) { return residue.atoms.Contains(atom); });
		//    }
		//    return residues;
		//}
	}
}
