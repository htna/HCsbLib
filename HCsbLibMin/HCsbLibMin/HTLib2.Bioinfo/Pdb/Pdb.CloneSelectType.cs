using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        public Pdb CloneByRemoveHeader()
        {
            Pdb pdb = CloneSelectType
                //( "TITLE "
                //, "REMARK"
                //, "CRYST1"
                ( "MODEL "
                , "ATOM  "
                , "TER   "
                , "ENDMDL"
                , "ANISOU"
                , "HETATM"
                , "SIGUIJ"
                , "CONECT"
                );
            return pdb;
        }
        public Pdb CloneRemoveResName(bool inAtom, bool inHetatm, params string[] resName)
        {
            HashSet<IAtom>  iatomsInConect = ListIAtomInConect().HToHashSet();
            HashSet<string> setResName = resName.HTrim().HToHashSet();
            List<Element> nelements = new List<Element>();
            int numRemove         = 0;
            int numRemoveInConect = 0;
            foreach(Element element in elements)
            {
                string record = element.record.Trim();
                bool remove = false;
                if(inAtom   && record == "ATOM"  ) if(resName.Contains((element as Atom  ).resName.Trim())) remove=true;
                if(inHetatm && record == "HETATM") if(resName.Contains((element as Hetatm).resName.Trim())) remove=true;
                if(remove)
                {
                    if(iatomsInConect.Contains(element as IAtom))
                        numRemoveInConect++;
                    numRemove++;
                    continue;
                }
                nelements.Add(element);
            }
            HDebug.Assert(numRemoveInConect == 0);
            Pdb npdb = new Pdb(nelements);
            return npdb;
        }
        public Pdb CloneRemoveHetatm()
        {
            Pdb pdb = CloneSelectType
                ( null
                , "HEADER", "TITLE" , "COMPND", "SOURCE", "KEYWDS", "EXPDTA", "AUTHOR", "REVDAT", "JRNL"  , "REMARK"    // 
                , "DBREF" , "SEQRES", "SEQADV"                                                                          // 
                , "HET"   , "HETNAM", "FORMUL"                                                                          // 
                , "HELIX" , "SHEET"                                                                                     // 
                , "LINK"                                                                                                // 
                , "SITE"                                                                                                // 
                , "CRYST1", "ORIGX1", "ORIGX2", "ORIGX3", "SCALE1", "SCALE2", "SCALE3", "MTRIX1", "MTRIX2", "MTRIX3"    // 
                , "MODEL" , "ATOM"  , "ANISOU", "SIGUIJ", "TER"   , "ENDMDL"                                            // , "HETATM"
                , "CONECT"                                                                                              // 
                , "MASTER", "END"                                                                                       // 
                );
            return pdb;
        }
        public Pdb CloneSelectType(params string[] types)
        {
            //  ( null
            //  , "HEADER", "TITLE" , "COMPND", "SOURCE", "KEYWDS", "EXPDTA", "AUTHOR", "REVDAT", "JRNL"  , "REMARK"    // 
            //  , "DBREF" , "SEQRES"                                                                                    // 
            //  , "HET"   , "HETNAM", "FORMUL"                                                                          // 
            //  , "HELIX" , "SHEET"                                                                                     // 
            //  , "LINK"                                                                                                // 
            //  , "SITE"                                                                                                // 
            //  , "CRYST1", "ORIGX1", "ORIGX2", "ORIGX3", "SCALE1", "SCALE2", "SCALE3", "MTRIX1", "MTRIX2", "MTRIX3"    // 
            //  , "MODEL" , "ATOM"  , "ANISOU", "SIGUIJ", "TER"   , "HETATM", "ENDMDL"                                  // 
            //  , "CONECT"                                                                                              // 
            //  , "MASTER", "END"                                                                                       // 
            //  );

            HashSet<string> settypes = types.HRemoveAll(null).HTrim().HToUpper().HRemoveAll("").HToHashSet();

            List<Element> nelements = new List<Element>();
            foreach(Element element in elements)
            {
                switch(element.record.Trim())
                {
                    //case "______": if(settypes.Contains("______")) break; continue; // break->keep,     continue->remove
                                    /// Title Section
                    case "HEADER": if(settypes.Contains("HEADER")) break; continue; // break->keep,     continue->remove
                    case "TITLE" : if(settypes.Contains("TITLE" )) break; continue; // break->keep,     continue->remove
                    case "COMPND": if(settypes.Contains("COMPND")) break; continue; // break->keep,     continue->remove
                    case "SOURCE": if(settypes.Contains("SOURCE")) break; continue; // break->keep,     continue->remove
                    case "KEYWDS": if(settypes.Contains("KEYWDS")) break; continue; // break->keep,     continue->remove
                    case "EXPDTA": if(settypes.Contains("EXPDTA")) break; continue; // break->keep,     continue->remove
                    case "AUTHOR": if(settypes.Contains("AUTHOR")) break; continue; // break->keep,     continue->remove
                    case "REVDAT": if(settypes.Contains("REVDAT")) break; continue; // break->keep,     continue->remove
                    case "JRNL"  : if(settypes.Contains("JRNL"  )) break; continue; // break->keep,     continue->remove
                    case "REMARK": if(settypes.Contains("REMARK")) break; continue; // break->keep,     continue->remove
                                    /// Primary Structure Section
                    case "DBREF" : if(settypes.Contains("DBREF" )) break; continue; // break->keep,     continue->remove
                    case "SEQRES": if(settypes.Contains("SEQRES")) break; continue; // break->keep,     continue->remove
                    case "SEQADV": if(settypes.Contains("SEQADV")) break; continue; // break->keep,     continue->remove
                                    /// Heterogen Section (updated)
                    case "HET"   : if(settypes.Contains("HET"   )) break; continue; // break->keep,     continue->remove
                    case "HETNAM": if(settypes.Contains("HETNAM")) break; continue; // break->keep,     continue->remove
                    case "FORMUL": if(settypes.Contains("FORMUL")) break; continue; // break->keep,     continue->remove
                                    /// Secondary Structure Section
                    case "HELIX" : if(settypes.Contains("HELIX" )) break; continue; // break->keep,     continue->remove
                    case "SHEET" : if(settypes.Contains("SHEET" )) break; continue; // break->keep,     continue->remove
                                    /// Connectivity Annotation Section
                    case "LINK"  : if(settypes.Contains("LINK"  )) break; continue; // break->keep,     continue->remove
                                    /// Miscellaneous Features Section
                    case "SITE"  : if(settypes.Contains("SITE"  )) break; continue; // break->keep,     continue->remove
                                    /// Crystallographic and Coordinate Transformation Section
                    case "CRYST1": if(settypes.Contains("CRYST1")) break; continue; // break->keep,     continue->remove
                    case "ORIGX1": if(settypes.Contains("ORIGX1")) break; continue; // break->keep,     continue->remove
                    case "ORIGX2": if(settypes.Contains("ORIGX2")) break; continue; // break->keep,     continue->remove
                    case "ORIGX3": if(settypes.Contains("ORIGX3")) break; continue; // break->keep,     continue->remove
                    case "SCALE1": if(settypes.Contains("SCALE1")) break; continue; // break->keep,     continue->remove
                    case "SCALE2": if(settypes.Contains("SCALE2")) break; continue; // break->keep,     continue->remove
                    case "SCALE3": if(settypes.Contains("SCALE3")) break; continue; // break->keep,     continue->remove
                    case "MTRIX1": if(settypes.Contains("MTRIX1")) break; continue; // break->keep,     continue->remove
                    case "MTRIX2": if(settypes.Contains("MTRIX2")) break; continue; // break->keep,     continue->remove
                    case "MTRIX3": if(settypes.Contains("MTRIX3")) break; continue; // break->keep,     continue->remove
                                    /// Coordinate Section
                    case "MODEL" : if(settypes.Contains("MODEL" )) break; continue; // break->keep,     continue->remove
                    case "ATOM"  : if(settypes.Contains("ATOM"  )) break; continue; // break->keep,     continue->remove
                    case "ANISOU": if(settypes.Contains("ANISOU")) break; continue; // break->keep,     continue->remove
                    case "SIGUIJ": if(settypes.Contains("SIGUIJ")) break; continue; // break->keep,     continue->remove
                    case "TER"   : if(settypes.Contains("TER"   )) break; continue; // break->keep,     continue->remove
                    case "HETATM": if(settypes.Contains("HETATM")) break; continue; // break->keep,     continue->remove
                    case "ENDMDL": if(settypes.Contains("ENDMDL")) break; continue; // break->keep,     continue->remove
                                    /// Connectivity Section
                    case "CONECT": if(settypes.Contains("CONECT")) break; continue; // break->keep,     continue->remove
                                    /// Bookkeeping Section
                    case "MASTER": if(settypes.Contains("MASTER")) break; continue; // break->keep,     continue->remove
                    case "END"   : if(settypes.Contains("END"   )) break; continue; // break->keep,     continue->remove
                    /// not listed
                    default:
                        //HDebug.ToDo("donot list "+element.record+" in Pdb.CloneByRemoveHeader()");
                        HDebug.Assert();
                        break;
                }
                nelements.Add(element);
            }

            Pdb npdb = new Pdb(nelements);
            return npdb;
        }
    }
}
