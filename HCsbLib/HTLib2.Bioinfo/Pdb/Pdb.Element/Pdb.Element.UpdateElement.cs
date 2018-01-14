using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
    {
        public partial class Element
        {
            public Element UpdateElement()
            {
                return UpdateElement(this);
            }
            public static Element UpdateElement(Element element)
            {
                return UpdateElement(element.line);
            }
            public static Element UpdateElement(string line)
            {
                line = (line+"                                                                                ").Substring(0,80);
                if(Header.IsHeader(line)) return Header.FromString(line);   // Title Section
                if(Title .IsTitle (line)) return Title .FromString(line);   // Title Section
                if(Compnd.IsCompnd(line)) return Compnd.FromString(line);   // Title Section
                if(Source.IsSource(line)) return Source.FromString(line);   // Title Section
                if(Keywds.IsKeywds(line)) return Keywds.FromString(line);   // Title Section
                if(Expdta.IsExpdta(line)) return Expdta.FromString(line);   // Title Section
                if(Nummdl.IsNummdl(line)) return Nummdl.FromString(line);   // Title Section
                if(Author.IsAuthor(line)) return Author.FromString(line);   // Title Section
                if(Revdat.IsRevdat(line)) return Revdat.FromString(line);   // Title Section
                if(Jrnl  .IsJrnl  (line)) return Jrnl  .FromString(line);   // Title Section
                if(Remark.IsRemark(line)) return Remark.FromString(line);   // Title Section
                if(Seqres.IsSeqres(line)) return Seqres.FromString(line);   // Primary Structure Section
                if(Seqadv.IsSeqadv(line)) return Seqadv.FromString(line);   // Primary Structure Section
                if(Helix .IsHelix (line)) return Helix .FromString(line);   // Secondary Structure Section
                if(Sheet .IsSheet (line)) return Sheet .FromString(line);   // Secondary Structure Section
                if(Site  .IsSite  (line)) return Site  .FromString(line);   // Miscellaneous Features Section
                if(Cryst1.IsCryst1(line)) return Cryst1.FromString(line);   // Crystallographic and Coordinate Transformation Section
                if(Anisou.IsAnisou(line)) return Anisou.FromString(line);   // Coordinate Section
                if(Atom  .IsAtom  (line)) return Atom  .FromString(line);   // Coordinate Section
                if(Endmdl.IsEndmdl(line)) return Endmdl.FromString(line);   // Coordinate Section
                if(Hetatm.IsHetatm(line)) return Hetatm.FromString(line);   // Coordinate Section
                if(Model .IsModel (line)) return Model .FromString(line);   // Coordinate Section
                if(Siguij.IsSiguij(line)) return Siguij.FromString(line);   // Coordinate Section
                if(Ter   .IsTer   (line)) return Ter   .FromString(line);   // Coordinate Section
                if(Conect.IsConect(line)) return Conect.FromString(line);   // Connectivity Section
                if(Master.IsMaster(line)) return Master.FromString(line);   // Bookkeeping Section
                if(End   .IsEnd   (line)) return End   .FromString(line);   // Bookkeeping Section

                if(line.Substring(0, 6) == "DBREF ") return new Element(line);
                if(line.Substring(0, 6) == "SEQRES") return new Element(line);
                if(line.Substring(0, 6) == "MODRES") return new Element(line);
                if(line.Substring(0, 6) == "HET   ") return new Element(line);
                if(line.Substring(0, 6) == "HETNAM") return new Element(line);
                if(line.Substring(0, 6) == "FORMUL") return new Element(line);
                if(line.Substring(0, 6) == "SSBOND") return new Element(line);
                if(line.Substring(0, 6) == "LINK  ") return new Element(line);
                if(line.Substring(0, 6) == "ORIGX1") return new Element(line);
                if(line.Substring(0, 6) == "ORIGX2") return new Element(line);
                if(line.Substring(0, 6) == "ORIGX3") return new Element(line);
                if(line.Substring(0, 6) == "SCALE1") return new Element(line);
                if(line.Substring(0, 6) == "SCALE2") return new Element(line);
                if(line.Substring(0, 6) == "SCALE3") return new Element(line);
                if(line.Substring(0, 6) == "CISPEP") return new Element(line);  // Connectivity Annotation Section
                if(line.Substring(0, 6) == "HETSYN") return new Element(line);
                if(line.Substring(0, 6) == "SEQADV") return new Element(line);
                if(line.Substring(0, 6) == "SPRSDE") return new Element(line);
                if(line.Substring(0, 6) == "MTRIX1") return new Element(line);
                if(line.Substring(0, 6) == "MTRIX2") return new Element(line);
                if(line.Substring(0, 6) == "MTRIX3") return new Element(line);

                if(line.Substring(0, 6) == "MDLTYP") return new Element(line);
                if(line.Substring(0, 6) == "SPLIT ") return new Element(line);
                if(line.Substring(0, 6) == "CAVEAT") return new Element(line);
                if(line.Substring(0, 6) == "OBSLTE") return new Element(line);
                if(line.Substring(0, 6) == "HYDBND") return new Element(line);
                if(line.Substring(0, 6) == "SLTBRG") return new Element(line);
                if(line.Substring(0, 6) == "TVECT ") return new Element(line);
                if(line.Substring(0, 6) == "DBREF1") return new Element(line);
                if(line.Substring(0, 6) == "DBREF2") return new Element(line);
                if(line.Substring(0, 6) == "SIGATM") return new Element(line);

                HDebug.Assert(false);
                return new Element(line);
            }
        }
    }
}
