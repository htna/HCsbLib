using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Element = Tinker.TkFile.Element;
    public partial class Tinker
    {
        public partial class Xyz
        {
            public Xyz CloneByRemoveHeader()
            {
                List<Element> nelements = new List<Element>();
                foreach(Element element in elements)
                {
                    Element nelement = null;
                    switch(element.type)
                    {
                        case "Header":
                            nelement = Header.FromData((element as Header).NumAtoms);
                            break;
                        case "Atom":
                            nelement = element;
                            break;
                        default:
                            HDebug.ToDo("donot list "+element.type+" in Tinker.Xyz.CloneByRemoveHeader()");
                            break;
                    }
                    nelements.Add(nelement);
                }

                Xyz nxyz = new Xyz { elements = nelements.ToArray() };
                return nxyz;
            }
            public Xyz CloneByReindex(IList<Pdb.Atom> pdb0atoms, HPack<Tuple<int, int>[]> idxXyzFromTo=null)
            {
                IList<Xyz.Atom> xyz0atoms = atoms;
                return CloneByReindex(pdb0atoms, xyz0atoms, idxXyzFromTo);
            }
            public Xyz CloneByReindex(IList<Pdb.Atom> pdb0atoms, IList<Xyz.Atom> xyz0atoms, HPack<Tuple<int,int>[]> idxXyzFromTo=null)
            {
                {   // check if serials in pdbatoms are sequential
                    for(int i=0; i<pdb0atoms.Count; i++)
                        if(pdb0atoms[i].serial != i+1)
                        {
                            HDebug.Assert(false);
                            return null;
                        }
                }

                IList<Xyz.Atom> xyzatoms = atoms;
                int size = xyzatoms.Count;
                if(xyzatoms.Count != pdb0atoms.Count)
                {
                    HDebug.Assert(false);
                    return null;
                }
                if(xyzatoms.Count != xyz0atoms.Count)
                {
                    HDebug.Assert(false);
                    return null;
                }
                for(int i=0; i<size; i++)
                {
                    Xyz.Atom atm0 = xyz0atoms[i];
                    Xyz.Atom atm1 = xyzatoms[i];
                    bool test = true;

                    if(atm0.Id        != atm1.Id       ) test = false;
                    if(atm0.AtomType  != atm1.AtomType ) test = false;
                    //if(atm0.X         != atm1.X        ) test = false;
                    //if(atm0.Y         != atm1.Y        ) test = false;
                    //if(atm0.Z         != atm1.Z        ) test = false;
                    if(atm0.AtomId    != atm1.AtomId   ) test = false;
                    if(atm0.BondedId1 != atm1.BondedId1) test = false;
                    if(atm0.BondedId2 != atm1.BondedId2) test = false;
                    if(atm0.BondedId3 != atm1.BondedId3) test = false;
                    if(atm0.BondedId4 != atm1.BondedId4) test = false;
                    if(atm0.BondedId5 != atm1.BondedId5) test = false;
                    if(atm0.BondedId6 != atm1.BondedId6) test = false;
                    if(atm0.BondedId7 != atm1.BondedId7) test = false;
                    if(atm0.BondedId8 != atm1.BondedId8) test = false;
                    if(atm0.BondedId9 != atm1.BondedId9) test = false;

                    if(test == false)
                    {
                        HDebug.Assert(false);
                        return null;
                    }
                }

                Tuple<int, int>[] xyz2pdb = IdxXyzToPdb(xyz0atoms, pdb0atoms, tolDist2: 0.00001);
                if(idxXyzFromTo != null)
                    idxXyzFromTo.value = xyz2pdb;

                if(xyz2pdb.Length != size)
                {
                    HDebug.Assert(false);
                    return null;
                }

                Tuple<Xyz.Atom, int[], int[]>[] idxinfos = new Tuple<Xyz.Atom, int[], int[]>[size];
                for(int i=0; i<size; i++)
                {
                    Xyz.Atom atom = xyzatoms[i];
                    idxinfos[i] = new Tuple<Xyz.Atom, int[], int[]>(atom, new int[1]{atom.Id}, atom.BondedIds);
                }

                {
                    if(xyzatoms.Count != pdb0atoms.Count)
                    {
                        HDebug.Assert(false);
                        return null;
                    }
                    for(int i=0; i<xyz2pdb.Length; i++)
                    {
                        int xyzi = xyz2pdb[i].Item1;
                        int pdbi = xyz2pdb[i].Item2;
                        int xyzIdFrom = xyzatoms[xyzi].Id;
                        int xyzIdTo   = - pdb0atoms[pdbi].serial;

                        HDebug.Assert(idxinfos[xyzi].Item2.Length == 1, idxinfos[xyzi].Item2[0] == xyzIdFrom);
                        idxinfos[xyzi].Item2[0] = xyzIdTo;
                        foreach(Tuple<Xyz.Atom, int[], int[]> idxinfo in idxinfos)
                        {
                            for(int j=0; j<idxinfo.Item3.Length; j++)
                                if(idxinfo.Item3[j] == xyzIdFrom)
                                    idxinfo.Item3[j] = xyzIdTo;
                        }
                    }
                    foreach(Tuple<Xyz.Atom, int[], int[]> idxinfo in idxinfos)
                    {
                        HDebug.Assert(idxinfo.Item2.Length == 1, idxinfo.Item2[0] < 0);
                        for(int j=0; j<idxinfo.Item3.Length; j++)
                            HDebug.Assert(idxinfo.Item3[j] < 0);
                    }
                }

                Element[] nelements = new Element[elements.Length];
                {
                    // copy header
                    HDebug.Assert((elements[0] as Xyz.Header) != null);
                    nelements[0] = new Xyz.Header((elements[0] as Xyz.Header).format, elements[0].line);

                    foreach(var idxinfo in idxinfos)
                    {
                        Xyz.Atom atom    = idxinfo.Item1;
                        int nid          = -1 * idxinfo.Item2[0];          // convert negative to positive index
                        int[] nbondedids = idxinfo.Item3.HToVectorT() * -1; // convert negative to positive index
                        HDebug.Assert(nid > 0, nbondedids.HToVectorT() > 0); // check positive

                        Xyz.Atom natom = Xyz.Atom.FromData(atom.format, nid, atom.AtomType, atom.X, atom.Y, atom.Z, atom.AtomId, nbondedids);
                        nelements[nid] = natom;
                    }

                    // Verify
                    for(int i=1; i<nelements.Length; i++)
                        HDebug.Assert((nelements[i] as Xyz.Atom).Id == i);
                }

                return new Xyz { elements = nelements };
            }
            public Xyz CloneByReindex(int idStart=1)
            {
                List<Tuple<int, int>> idsFromTo = new List<Tuple<int, int>>();
                int nid = idStart;
                foreach(var atom in atoms)
                {
                    idsFromTo.Add(new Tuple<int, int>(atom.Id, nid));
                    nid++;
                }

                return CloneByReindex(idsFromTo);
            }
            public Xyz CloneBySelectIds(IList<int> idsSelect)
            {
                HashSet<int> lstAtomIdRemove = atoms.HListId().HToHashSet();
                Tuple<int,int>[] idsFromTo = new Tuple<int, int>[idsSelect.Count];
                for(int i=0; i<idsSelect.Count; i++)
                {
                    idsFromTo[i] = new Tuple<int, int>(idsSelect[i], i+1);
                    lstAtomIdRemove.Remove(idsSelect[i]);
                }

                Xyz newxyz;
                newxyz = this.CloneByRemoveIds(lstAtomIdRemove.ToList());
                newxyz = newxyz.CloneByReindex(idsFromTo);

                if(HDebug.IsDebuggerAttached)
                {
                    var id2atoms = this.atoms.ToIdDictionary();
                    var newatoms = newxyz.atoms;
                    for(int i=0; i<idsSelect.Count; i++)
                    {
                        int id = idsSelect[i];
                        var atom0 = id2atoms[id];
                        var atom1 = newatoms[i];
                        HDebug.Assert( atom0.AtomType         == atom1.AtomType);
                        HDebug.Assert((atom0.Coord            -  atom1.Coord).Dist < 0.00000001);
                        HDebug.Assert( atom0.BondedIds.Length == atom1.BondedIds.Length);
                    }
                }

                return newxyz;
            }
            public Xyz CloneByReindex(IList<Tuple<int,int>> idsFromTo, bool allowresize=false)
            {
                HashSet<int> setIdFrom = new HashSet<int>(idsFromTo.HListItem1());
                HashSet<int> setIdTo   = new HashSet<int>(idsFromTo.HListItem2());
                if(setIdFrom.Count != atoms.Length ) { HDebug.Assert(false); return null; }
                if(setIdFrom.Count != setIdTo.Count) { HDebug.Assert(false); return null; }

                Dictionary<int, Tuple<int, int>> idFrom2To = idsFromTo.HToDictionaryWithKeyItem1();

                List<Element>           nheaders = new List<Element>();
                Dictionary<int,Element> natoms   = new Dictionary<int, Element>();
                for(int ie=0; ie<elements.Length; ie++)
                {
                    switch(elements[ie].type)
                    {
                        case "Header":
                            nheaders.Add(elements[ie]);
                            break;
                        case "Atom":
                            {
                                Atom atom = elements[ie] as Atom;
                                HDebug.Assert(idFrom2To[atom.Id].Item1 == atom.Id);
                                int nid = idFrom2To[atom.Id].Item2;
                                int[] nbondedids = atom.BondedIds;
                                for(int i=0; i<nbondedids.Length; i++)
                                {
                                    int from = nbondedids[i];
                                    if(idFrom2To.ContainsKey(from))
                                    {
                                        HDebug.Assert(idFrom2To[from].Item1 == from);
                                        int to = idFrom2To[from].Item2;
                                        nbondedids[i] = to;
                                    }
                                }
                                Xyz.Atom natom = Xyz.Atom.FromData(atom.format, nid, atom.AtomType, atom.X, atom.Y, atom.Z, atom.AtomId, nbondedids);
                                natoms.Add(nid, natom);
                            }
                            break;
                        default:
                            HDebug.Assert(false);
                            return null;
                    }
                }

                List<Element> nelements = new List<Element>(nheaders);
                foreach(int nid in natoms.Keys.ToArray().HSort())
                    nelements.Add(natoms[nid]);

                {
                    int maxid = nelements.HSelectByType((Atom)null).HListId().Max();
                    int[] idxhdr = nelements.HIndexByType((Header)null).ToArray();
                    HDebug.Assert(idxhdr.Length == 1);
                    nelements[idxhdr[0]] = Header.FromData(maxid);
                }

                return new Xyz { elements = nelements.ToArray() };
            }
            public Xyz CloneByAlign(Xyz xyzToAlign)
            {
                Xyz.Atom[] atoms0 = atoms;
                Xyz.Atom[] atoms1 = xyzToAlign.atoms;
                if(atoms0.Length != atoms1.Length)
                {
                    HDebug.Assert(false);
                    return null;
                }

                for(int i=0; i<atoms0.Length; i++)
                {
                    Xyz.Atom atm0 = atoms0[i];
                    Xyz.Atom atm1 = atoms1[i];
                    bool test = true;

                    if(atm0.Id        != atm1.Id       ) test = false;
                    if(atm0.AtomType  != atm1.AtomType ) test = false;
                    //if(atm0.X         != atm1.X        ) test = false;
                    //if(atm0.Y         != atm1.Y        ) test = false;
                    //if(atm0.Z         != atm1.Z        ) test = false;
                    if(atm0.AtomId    != atm1.AtomId   ) test = false;
                    if(atm0.BondedId1 != atm1.BondedId1) test = false;
                    if(atm0.BondedId2 != atm1.BondedId2) test = false;
                    if(atm0.BondedId3 != atm1.BondedId3) test = false;
                    if(atm0.BondedId4 != atm1.BondedId4) test = false;
                    if(atm0.BondedId5 != atm1.BondedId5) test = false;
                    if(atm0.BondedId6 != atm1.BondedId6) test = false;
                    if(atm0.BondedId7 != atm1.BondedId7) test = false;
                    if(atm0.BondedId8 != atm1.BondedId8) test = false;
                    if(atm0.BondedId9 != atm1.BondedId9) test = false;

                    if(test == false)
                    {
                        HDebug.Assert(false);
                        return null;
                    }
                }

                Vector[] coords0 = atoms.HListCoords();
                Vector[] coords1 = xyzToAlign.atoms.HListCoords();
                Align.MinRMSD.Align(coords1, ref coords0);
                return CloneByCoords(coords0);
            }
            public Xyz CloneByCoords(IList<Vector> coords)
            {
                return CloneByCoords(coords, Atom.Format.defformat_digit06);
            }
            public Xyz CloneByCoords(IList<Vector> coords, Atom.Format format)
            {
                Element[] nelements = new Element[elements.Length];
                {
                    int icoords = 0;
                    for(int ie=0; ie<elements.Length; ie++)
                    {
                        Atom iatom = elements[ie] as Atom;
                        if(iatom == null)
                            nelements[ie] = elements[ie];
                        else
                        {
                            nelements[ie] = Atom.FromCoord(iatom, coords[icoords], format);
                            icoords++;
                        }
                    }
                    if(icoords != coords.Count)
                    {
                        HDebug.Assert(false);
                        return null;
                    }
                }

                return new Xyz { elements = nelements };
            }
            public Xyz CloneByRemoveIds(IList<int> lstAtomIdRemove)
            {
                Element[] nelements = elements.HClone();

                Dictionary<int,int> id2idx = new Dictionary<int, int>();
                for(int idx=0; idx<nelements.Length; idx++)
                {
                    if(nelements[idx] is Header) continue;
                    if(nelements[idx] is Atom  ) { id2idx.Add((nelements[idx] as Atom).Id, idx); continue; }
                    throw new Exception();
                }

                var atoms_format = this.atoms_format;
                // remove bonds
                foreach(var id in lstAtomIdRemove)
                {
                    int  idx  = id2idx[id];
                    Atom atom = nelements[idx] as Atom;
                    foreach(var bondid in atom.BondedIds)
                    {
                        int   bondidx = id2idx[bondid];
                        Atom  bond    = nelements[bondidx] as Atom;
                        int[] bond_bondids = bond.BondedIds;
                              bond_bondids = bond_bondids.HRemoveAll(id);
                              bond = Atom.FromData(atoms_format, bond.Id, bond.AtomType, bond.X, bond.Y, bond.Z, bond.AtomId, bond_bondids);
                        nelements[bondidx] = bond;
                    }
                    atom = Atom.FromData(atoms_format, atom.Id, atom.AtomType, atom.X, atom.Y, atom.Z, atom.AtomId, new int[] { });
                    nelements[idx] = atom;
                    nelements[idx] = null;
                }
                nelements = nelements.HRemoveAllNull(false).ToArray();

                int maxid = nelements.HSelectByType((Atom)null).HListId().Max();
                int[] idxhdr = nelements.HIndexByType((Header)null).ToArray();
                HDebug.Assert(idxhdr.Length == 1);
                nelements[idxhdr[0]] = Header.FromData(atoms_format, maxid);

                return new Xyz
                {
                    elements = nelements
                };
            }
        }
    }
}
