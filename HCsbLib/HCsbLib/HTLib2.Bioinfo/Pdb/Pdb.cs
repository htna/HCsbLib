using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    [Serializable]
    public partial class Pdb
	{
		public readonly Element[] elements;
        public ELEM[] ListElemByType<ELEM>()
            where ELEM : Element
        {
            return elements.ListType<ELEM>().ToArray();
        }

		                                      public IAtom  [] iatoms   { get { return elements.HSelectByType <Element,IAtom >()           .ToArray(); } }
		public readonly int[] _idxatoms  ;    public Atom   [] atoms    { get { return elements.HSelectByIndex<Element,Atom  >(_idxatoms  ).ToArray(); } }
        public readonly int[] _idxhelixes;    public Helix  [] helixes  { get { return elements.HSelectByIndex<Element,Helix >(_idxhelixes).ToArray(); } }
		public readonly int[] _idxsheets ;    public Sheet  [] sheets   { get { return elements.HSelectByIndex<Element,Sheet >(_idxsheets ).ToArray(); } }
        public readonly int[] _idxhetatms;    public Hetatm [] hetatms  { get { return elements.HSelectByIndex<Element,Hetatm>(_idxhetatms).ToArray(); } }
        public readonly int[] _idxanisous;    public Anisou [] anisous  { get { return elements.HSelectByIndex<Element,Anisou>(_idxanisous).ToArray(); } }
        public readonly int[] _idxsiguijs;    public Siguij [] siguijs  { get { return elements.HSelectByIndex<Element,Siguij>(_idxsiguijs).ToArray(); } }
        public readonly int[] _idxmodels ;    public Model  [] models   { get { return elements.HSelectByIndex<Element,Model >(_idxmodels ).ToArray(); } }
        public readonly int[] _idxremarks;    public Remark [] remarks  { get { return elements.HSelectByIndex<Element,Remark>(_idxremarks).ToArray(); } }
                                              public Remark2[] remark2s { get { return ListElemByType<Pdb.Remark2>().ToArray(); } }
                                              public Seqres [] seqress  { get { return ListElemByType<Pdb.Seqres >().ToArray(); } }
                                              public Conect [] conects  { get { return ListElemByType<Pdb.Conect >().ToArray(); } }
                                              public Site   [] sites    { get { return ListElemByType<Pdb.Site   >().ToArray(); } }
                                              public Compnd [] compnds  { get { return ListElemByType<Pdb.Compnd >().ToArray(); } }
        public readonly int[] _idxendmdls;

        public Atom[] AtomsInModel(Model model)
        {
            return ELEMsInModel<Atom>(model);
        }
        public ELEM[] ELEMsInModel<ELEM>(Model model)
            where ELEM: Element
        {
            for(int im=0; im<_idxmodels.Length; im++)
            {
                int idxmodel = _idxmodels[im];
                if(elements[idxmodel] == model)
                    return ELEMsInModelByIndex<ELEM>(im);
            }
            HDebug.Assert(false);
            return null;
        }
        public Atom[] AtomsInModelBySerial(int serial)
        {
            return ELEMsInModelBySerial<Atom>(serial);
        }
        public ELEM[] ELEMsInModelBySerial<ELEM>(int serial)
            where ELEM : Element
        {
            foreach(var model in models)
            {
                if(model.serial == serial)
                    return ELEMsInModel<ELEM>(model);
            }
            return null;
        }
        public Atom[] AtomsInModelByIndex(int im)
        {
            return ELEMsInModelByIndex<Atom>(im);
        }
        public ELEM[] ELEMsInModelByIndex<ELEM>(int im)
            where ELEM: Element
        {
            int idxmodel  = _idxmodels[im];
            int idxendmdl = _idxendmdls[im];
            HDebug.Assert(idxmodel < idxendmdl);
            List<ELEM> atoms = new List<ELEM>();
            for(int ie=idxmodel; ie<=idxendmdl; ie++)
            {
                Element element = elements[ie];
                ELEM atom = element as ELEM;
                if(atom != null)
                    atoms.Add(atom);
            }
            return atoms.ToArray();
        }
        public Atom[] SelectAtoms(int? imodel=null, char? altLoc=null, char? chainID=null, char? iCode=null)
        {
            List<Atom> sel = atoms.ToList();
            if(imodel  != null) sel = AtomsInModelByIndex  (     imodel.Value).ToList();
            if(altLoc  != null) { sel = sel.SelectByAltLoc (' ', altLoc.Value); } else { sel = sel.SelectByAltLoc (); }
            if(chainID != null) { sel = sel.SelectByChainID(    chainID.Value); } else { sel = sel.SelectByChainID(); }
            if(iCode   != null) { sel = sel.SelectByICode  (      iCode.Value); } else { sel = sel.SelectByICode  (); }
            return sel.ToArray();
        }

		public Pdb(IList<Element> elements)
		{
            List<Element> lstElement = new List<Element>(elements.Count);
            for(int i=0; i<elements.Count; i++)
                if(elements[i] != null)
                    lstElement.Add(elements[i]);
            elements = lstElement;
            this.elements = elements.ToArray();
            List<int> idxatoms   = new List<int>(); for(int i=0; i<elements.Count; i++) if(typeof(Atom  ).IsInstanceOfType(elements[i])) idxatoms  .Add(i); this._idxatoms   = idxatoms  .ToArray();
            List<int> idxhelixes = new List<int>(); for(int i=0; i<elements.Count; i++) if(typeof(Helix ).IsInstanceOfType(elements[i])) idxhelixes.Add(i); this._idxhelixes = idxhelixes.ToArray();
            List<int> idxsheets  = new List<int>(); for(int i=0; i<elements.Count; i++) if(typeof(Sheet ).IsInstanceOfType(elements[i])) idxsheets .Add(i); this._idxsheets  = idxsheets .ToArray();
            List<int> idxhetatms = new List<int>(); for(int i=0; i<elements.Count; i++) if(typeof(Hetatm).IsInstanceOfType(elements[i])) idxhetatms.Add(i); this._idxhetatms = idxhetatms.ToArray();
            List<int> idxanisous = new List<int>(); for(int i=0; i<elements.Count; i++) if(typeof(Anisou).IsInstanceOfType(elements[i])) idxanisous.Add(i); this._idxanisous = idxanisous.ToArray();
            List<int> idxsiguijs = new List<int>(); for(int i=0; i<elements.Count; i++) if(typeof(Siguij).IsInstanceOfType(elements[i])) idxsiguijs.Add(i); this._idxsiguijs = idxsiguijs.ToArray();
            List<int> idxmodels  = new List<int>(); for(int i=0; i<elements.Count; i++) if(typeof(Model ).IsInstanceOfType(elements[i])) idxmodels .Add(i); this._idxmodels  = idxmodels .ToArray();
            List<int> idxendmdls = new List<int>(); for(int i=0; i<elements.Count; i++) if(typeof(Endmdl).IsInstanceOfType(elements[i])) idxendmdls.Add(i); this._idxendmdls = idxendmdls.ToArray();
            List<int> idxremarks = new List<int>(); for(int i=0; i<elements.Count; i++) if(typeof(Remark).IsInstanceOfType(elements[i])) idxremarks.Add(i); this._idxremarks = idxremarks.ToArray();
            HDebug.Assert(idxmodels.Count == idxendmdls.Count);
        }

        ////////////////////////////////////////////////////////////////////////////////////
        // Serializable
        public Pdb(SerializationInfo info, StreamingContext ctxt)
		{
            elements = (Element[])info.GetValue("elements", typeof(Element[]));
            //atoms    = (Atom   [])info.GetValue("atoms"   , typeof(Atom   []));
            //helixes  = (Helix  [])info.GetValue("helixes" , typeof(Helix  []));
            //sheets   = (Sheet  [])info.GetValue("sheets"  , typeof(Sheet  []));
            //hetatms  = (Hetatm [])info.GetValue("hetatms" , typeof(Hetatm []));
            //anisous  = (Anisou [])info.GetValue("anisous" , typeof(Anisou []));
            //siguijs  = (Siguij [])info.GetValue("siguijs" , typeof(Siguij []));
            List<int> idxatoms   = new List<int>(); for(int i=0; i<elements.Length; i++) if(typeof(Atom  ).IsInstanceOfType(elements[i])) idxatoms  .Add(i); this._idxatoms   = idxatoms  .ToArray();
            List<int> idxhelixes = new List<int>(); for(int i=0; i<elements.Length; i++) if(typeof(Helix ).IsInstanceOfType(elements[i])) idxhelixes.Add(i); this._idxhelixes = idxhelixes.ToArray();
            List<int> idxsheets  = new List<int>(); for(int i=0; i<elements.Length; i++) if(typeof(Sheet ).IsInstanceOfType(elements[i])) idxsheets .Add(i); this._idxsheets  = idxsheets .ToArray();
            List<int> idxhetatms = new List<int>(); for(int i=0; i<elements.Length; i++) if(typeof(Hetatm).IsInstanceOfType(elements[i])) idxhetatms.Add(i); this._idxhetatms = idxhetatms.ToArray();
            List<int> idxanisous = new List<int>(); for(int i=0; i<elements.Length; i++) if(typeof(Anisou).IsInstanceOfType(elements[i])) idxanisous.Add(i); this._idxanisous = idxanisous.ToArray();
            List<int> idxsiguijs = new List<int>(); for(int i=0; i<elements.Length; i++) if(typeof(Siguij).IsInstanceOfType(elements[i])) idxsiguijs.Add(i); this._idxsiguijs = idxsiguijs.ToArray();
            List<int> idxmodels  = new List<int>(); for(int i=0; i<elements.Length; i++) if(typeof(Model ).IsInstanceOfType(elements[i])) idxmodels .Add(i); this._idxmodels  = idxmodels .ToArray();
            List<int> idxendmdls = new List<int>(); for(int i=0; i<elements.Length; i++) if(typeof(Endmdl).IsInstanceOfType(elements[i])) idxendmdls.Add(i); this._idxendmdls = idxendmdls.ToArray();
            List<int> idxremarks = new List<int>(); for(int i=0; i<elements.Length; i++) if(typeof(Remark).IsInstanceOfType(elements[i])) idxremarks.Add(i); this._idxremarks = idxremarks.ToArray();
        }
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            info.AddValue("elements", elements);
            //info.AddValue("atoms"   , atoms   );
            //info.AddValue("helixes" , helixes );
            //info.AddValue("sheets"  , sheets  );
            //info.AddValue("hetatms" , hetatms );
            //info.AddValue("anisous" , anisous );
            //info.AddValue("siguijs" , siguijs );
        }
    }
}
