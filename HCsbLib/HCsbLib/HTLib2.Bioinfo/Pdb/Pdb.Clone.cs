using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
	{
        public Pdb Clone()
        {
            Element[] newelements = new Element[elements.Length];
            for(int i=0; i<elements.Length; i++)
                newelements[i] = elements[i].UpdateElement();
            return new Pdb(newelements);
        }

        public Pdb CloneUpdateCoord(IList<Vector> coords)
        {
            Atom[] latoms = this.atoms;
            if(latoms.Length != coords.Count)
            {
                throw new ArgumentException();
            }
            int leng = latoms.Length;

            Dictionary<IAtom, Vector> iatom2coord = new Dictionary<IAtom, Vector>();
            for(int i=0; i<leng; i++)
                iatom2coord.Add(latoms[i], coords[i]);

            return CloneUpdateCoord(iatom2coord);
        }
        public Pdb CloneUpdateCoord(Dictionary<IAtom,Vector> iatom2coord)
        {
            Element[] newelements = new Element[elements.Length];
            for(int i=0; i<elements.Length; i++)
            {
                Element elem = elements[i];
                if(elem is IAtom)
                {
                    if(iatom2coord.ContainsKey(elem as IAtom))
                    {
                        Vector coord = iatom2coord[elem as IAtom];
                        Element nelem = null;
                        if(elem is Atom)
                        {
                            if(nelem != null) throw new HException("(nelem != null)");
                            Atom atom = elem as Atom;
                            nelem = Atom.FromString(atom.GetUpdatedLine(coord));
                        }
                        if(elem is Hetatm)
                        {
                            if(nelem != null) throw new HException("(nelem != null)");
                            Hetatm hetatm = elem as Hetatm;
                            nelem = Hetatm.FromString(hetatm.GetUpdatedLine(coord));
                        }
                        if(nelem == null) throw new NotImplementedException();
                        elem = nelem;
                    }
                }
                newelements[i] = elem.UpdateElement();
            }
            return new Pdb(newelements);
        }

        public Pdb CloneUpdateTempFactor(IList<double> tempFactors)
        {
            Atom[] latoms = this.atoms;
            if(latoms.Length != tempFactors.Count)
            {
                throw new ArgumentException();
            }
            int leng = latoms.Length;

            Dictionary<IAtom, double> iatom2tempFactor = new Dictionary<IAtom, double>();
            for(int i=0; i<leng; i++)
                iatom2tempFactor.Add(latoms[i], tempFactors[i]);

            return CloneUpdateTempFactor(iatom2tempFactor);
        }
        public Pdb CloneUpdateTempFactor(Dictionary<IAtom,double> iatom2tempFactor)
        {
            Element[] newelements = new Element[elements.Length];
            for(int i=0; i<elements.Length; i++)
            {
                Element elem = elements[i];
                if(elem is IAtom)
                {
                    if(iatom2tempFactor.ContainsKey(elem as IAtom))
                    {
                        double tempFactor = iatom2tempFactor[elem as IAtom];
                        Element nelem = null;
                        if(elem is Atom)
                        {
                            if(nelem != null) throw new HException("(nelem != null)");
                            Atom atom = elem as Atom;
                            nelem = Atom.FromString(atom.GetUpdatedLineTempFactor(tempFactor));
                        }
                        if(elem is Hetatm)
                        {
                            if(nelem != null) throw new HException("(nelem != null)");
                            Hetatm hetatm = elem as Hetatm;
                            nelem = Hetatm.FromString(hetatm.GetUpdatedLineTempFactor(tempFactor));
                        }
                        if(nelem == null) throw new NotImplementedException();
                        elem = nelem;
                    }
                }
                newelements[i] = elem.UpdateElement();
            }
            return new Pdb(newelements);
        }

        public Dictionary<char, char> GetPairChainID(string IDfrom, string IDto)
        {
            /// chain ' ' in from/to will be ignored. Use this only for aligning domain of chains
            /// chain '_' in to will be a wild-card; multiple copy of '_' is OK.
            /// 
            /// example: "ABCDE"->"12345" => A1,B2,C3,D4,E5
            ///          "ABCDD"->"12345" => exception because multiple copy of chain in "ABCDD"
            ///          "ABCDE"->"12344" => exception because multiple copy of chain in "12344"
            ///          "AB DE"->"12 45" => A1,B2,D4,E5  ignoring ' '->' '
            ///          "A CDE"->"12345" => exception because ' '->'2'
            ///          "ABCDE"->"123 5" => exception because 'D'->' '
            ///          "ABCDE"->"12_4_" => A1,B2,C_,D4,E_ because '_' is a wild card
            ///          "ABC_E"->"12345" => A1,B2,C3,_4,E5
            ///          "AB_DE"->"12_45" => exception because '_'->'_'

            Dictionary<char, char> from2to = new Dictionary<char, char>();
            for(int i=0; i<IDfrom.Length; i++)
            {
                char fromi = IDfrom[i];
                char toi   = IDto[i];
                if((fromi == '_') || (toi == '_'))
                {
                    if((fromi == '_') && (toi == '_')) throw new ArgumentException("_->_ is not allowed");
                    from2to.Add(fromi, toi);
                    continue;
                }
                if((fromi == ' ') || (toi == ' '))
                {
                    if((fromi == ' ') && (toi != ' ')) throw new ArgumentException("only ' '->' ' is allowed");
                    if((fromi != ' ') && (toi == ' ')) throw new ArgumentException("only ' '->' ' is allowed");
                    if((fromi == ' ') && (toi == ' ')) continue;
                    throw new Exception("should not reach here");
                }
                if(from2to.ContainsKey(fromi)) throw new ArgumentException("duplicated chain ID in IDfrom");
                if(from2to.ContainsValue(toi)) throw new ArgumentException("duplicated chain ID in IDto");
                from2to.Add(fromi, toi);
            }
            return from2to;
        }
        public Pdb CloneReplaceChainID(string IDfrom, string IDto)
        {
            if(IDfrom == null) throw new ArgumentException("IDfrom is null");
            if(IDto   == null) throw new ArgumentException("IDto is null");
            if(IDfrom.Length != IDto.Length) throw new ArgumentException("IDFrom.leng != IDto.leng");

            Dictionary<char, char> from2to = GetPairChainID(IDfrom, IDto);
            if(from2to.ContainsKey('_'))
                throw new ArgumentException("wildkey('_') is not allowed in IDfrom");

            Element[] newelements = new Element[elements.Length];
            for(int i=0; i<elements.Length; i++)
            {
                Element elem = elements[i];
                char? ch = null;
                if(elem is Atom  ) { HDebug.Assert(ch == null); ch = (elem as Atom  ).chainID; }
                if(elem is Hetatm) { HDebug.Assert(ch == null); ch = (elem as Hetatm).chainID; }
                if(elem is Anisou) { HDebug.Assert(ch == null); ch = (elem as Anisou).chainID; }

                if(ch != null)
                {
                    if(from2to.ContainsKey(ch.Value))
                    {
                        if(from2to[ch.Value] == '_')
                            // if wildcard ('_') is in IDto, delete that atom
                            continue;
                        Element nelem = null;
                        if(elem is Atom)
                        {
                            if(nelem != null) throw new HException("(nelem != null)");
                            Atom atom = elem as Atom;
                            nelem = Atom.FromData(
                                serial     : atom.serial    ,
                                name       : atom.name      ,
                                resName    : atom.resName   ,
                                chainID    : from2to[atom.chainID],
                                resSeq     : atom.resSeq    ,
                                x          : atom.x         ,
                                y          : atom.y         ,
                                z          : atom.z         ,
                                altLoc     : atom.altLoc    ,
                                iCode      : atom.iCode     ,
                                occupancy  : atom.occupancy ,
                                tempFactor : atom.tempFactor,
                                element    : atom.element   ,
                                charge     : atom.charge    
                            );
                        }
                        if(elem is Hetatm)
                        {
                            if(nelem != null) throw new HException("(nelem != null)");
                            Hetatm hetatm = elem as Hetatm;
                            nelem = Hetatm.FromData(
                                serial     : hetatm.serial    ,
                                name       : hetatm.name      ,
                                resName    : hetatm.resName   ,
                                chainID    : from2to[hetatm.chainID],
                                resSeq     : hetatm.resSeq    ,
                                x          : hetatm.x         ,
                                y          : hetatm.y         ,
                                z          : hetatm.z         ,
                                altLoc     : hetatm.altLoc    ,
                                iCode      : hetatm.iCode     ,
                                occupancy  : hetatm.occupancy ,
                                tempFactor : hetatm.tempFactor,
                                element    : hetatm.element   ,
                                charge     : hetatm.charge    
                            );
                        }
                        if(elem is Anisou)
                        {
                            if(nelem != null) throw new HException("(nelem != null)");
                            Anisou anisou = elem as Anisou;
                            nelem = Anisou.FromData(anisou.line
                                                   , chainID: from2to[anisou.chainID]
                                                   );
                        }
                        if(nelem == null) throw new NotImplementedException();
                        elem = nelem;
                    }
                }
                newelements[i] = elem.UpdateElement();
            }
            newelements = newelements.HRemoveAll(null);

            return new Pdb(newelements);
        }
    }
}
