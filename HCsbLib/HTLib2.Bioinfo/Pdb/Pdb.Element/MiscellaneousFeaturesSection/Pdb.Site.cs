using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
    {
        [Serializable]
        public class Site : Element
        {
            /// http://www.wwpdb.org/documentation/format32/sect7.html#SITE
            ///
            /// Overview 
            ///     Site records specify residues comprising catalytic, co-factor, anti-codon, regulatory or
            ///     other essential sites or environments surrounding ligands present in the structure.
            /// 
            /// Record Format
            /// COLUMNS        DATA  TYPE    FIELD         DEFINITION
            /// ---------------------------------------------------------------------------------
            ///  1 -  6        Record name   "SITE  "
            ///  8 - 10        Integer       seqNum        Sequence number.
            /// 12 - 14        LString(3)    siteID        Site name.
            /// 16 - 17        Integer       numRes        Number of residues that compose the site.
            /// 19 - 21        Residue name  resName1      Residue name for first residue that creates the site.
            /// 23             Character     chainID1      Chain identifier for first residue of site.
            /// 24 - 27        Integer       seq1          Residue sequence number for first residue of the  site.
            /// 28             AChar         iCode1        Insertion code for first residue of the site.
            /// 30 - 32        Residue name  resName2      Residue name for second residue that creates the site.
            /// 34             Character     chainID2      Chain identifier for second residue of the site.
            /// 35 - 38        Integer       seq2          Residue sequence number for second residue of the site.
            /// 39             AChar         iCode2        Insertion code for second residue of the site.
            /// 41 - 43        Residue name  resName3      Residue name for third residue that creates the site.
            /// 45             Character     chainID3      Chain identifier for third residue of the site.
            /// 46 - 49        Integer       seq3          Residue sequence number for third residue of the site.
            /// 50             AChar         iCode3        Insertion code for third residue of the site.
            /// 52 - 54        Residue name  resName4      Residue name for fourth residue that creates the site.
            /// 56             Character     chainID4      Chain identifier for fourth residue of the site.
            /// 57 - 60        Integer       seq4          Residue sequence number for fourth residue of the site.
            /// 61             AChar         iCode4        Insertion code for fourth residue
            /// 
            /// Details 
            /// * The sequence number (columns 8 - 10) is reset to 1 for each new site.
            /// * SITE identifiers (columns 12 - 14) should be fully explained in remark 800.
            /// * If a site is composed of more than four residues, these may be specified on additional records
            ///   bearing the same site identifier.
            /// * SITE records can include HET groups. 
            /// 
            /// Verification/Validation/Value Authority Control 
            ///     Every SITE must have a corresponding description in remark 800. The numbering of
            ///     sequential SITE records and format of each one is verified, as well as the existence
            ///     of each residue in the ATOM records. 
            /// 
            /// Relationships to Other Record Types 
            ///     Each listed SITE needs a corresponding REMARK 800 that details its significance.
            /// 
            /// Example
            ///          1         2         3         4         5         6         7         8
            /// 12345678901234567890123456789012345678901234567890123456789012345678901234567890
            /// SITE     1 AC1  3 HIS A  94  HIS A  96  HIS A 119                               
            /// SITE     1 AC2  5 ASN A  62  GLY A  63  HIS A  64  HOH A 328                    
            /// SITE     2 AC2  5 HOH A 634                                                     
            /// SITE     1 AC3  5 GLN A 136  GLN A 137  PRO A 138  GLU A 205                    
            /// SITE     2 AC3  5 CYS A 206                                                     
            /// SITE     1 AC4 11 HIS A  64  HIS A  94  HIS A  96  HIS A 119                    
            /// SITE     2 AC4 11 LEU A 198  THR A 199  THR A 200  TRP A 209                    
            /// SITE     3 AC4 11 HOH A 572  HOH A 582  HOH A 635           

//          string Element.RecordName { get { return "SITE "; } }
            public static string RecordName { get { return "SITE "; } }
            Site(string line)
                : base(line)
            {
            }
            public static Site FromString(string line)
            {
                HDebug.Assert(IsSite(line));
                return new Site(line);
            }
            public static bool IsSite(string line) { return (line.Substring(0, 6) == "SITE  "); }
                                                                                // COLUMNS        DATA  TYPE     FIELD         DEFINITION
                                                                                // -----------------------------------------------------------------------------------
            public    int seqNum  { get { return Integer     ( 8,10).Value; } } //  8 - 10        Integer       seqNum        Sequence number.
            public string siteID  { get { return String      (12,14);       } } // 12 - 14        LString(3)    siteID        Site name.
            public    int numRes  { get { return Integer     (16,17).Value; } } // 16 - 17        Integer       numRes        Number of residues that compose the site.
            public string resName1{ get { return String      (19,21);       } } // 19 - 21        Residue name  resName1      Residue name for first residue that creates the site.
            public   char chainID1{ get { return Char        (23   );       } } // 23             Character     chainID1      Chain identifier for first residue of site.
            public    int seq1    { get { return Integer     (24,27).Value; } } // 24 - 27        Integer       seq1          Residue sequence number for first residue of the  site.
            public   char iCode1  { get { return Char        (28   );       } } // 28             AChar         iCode1        Insertion code for first residue of the site.
            public string resName2{ get { return String      (30,32);       } } // 30 - 32        Residue name  resName2      Residue name for second residue that creates the site.
            public   char chainID2{ get { return Char        (34   );       } } // 34             Character     chainID2      Chain identifier for second residue of the site.
            public    int seq2    { get { return Integer     (35,38).Value; } } // 35 - 38        Integer       seq2          Residue sequence number for second residue of the site.
            public   char iCode2  { get { return Char        (39   );       } } // 39             AChar         iCode2        Insertion code for second residue of the site.
            public string resName3{ get { return String      (41,43);       } } // 41 - 43        Residue name  resName3      Residue name for third residue that creates the site.
            public   char chainID3{ get { return Char        (45   );       } } // 45             Character     chainID3      Chain identifier for third residue of the site.
            public    int seq3    { get { return Integer     (46,49).Value; } } // 46 - 49        Integer       seq3          Residue sequence number for third residue of the site.
            public   char iCode3  { get { return Char        (50   );       } } // 50             AChar         iCode3        Insertion code for third residue of the site.
            public string resName4{ get { return String      (52,54);       } } // 52 - 54        Residue name  resName4      Residue name for fourth residue that creates the site.
            public   char chainID4{ get { return Char        (56   );       } } // 56             Character     chainID4      Chain identifier for fourth residue of the site.
            public    int seq4    { get { return Integer     (57,60).Value; } } // 57 - 60        Integer       seq4          Residue sequence number for fourth residue of the site.////////////////////////
            public   char iCode4  { get { return Char        (61   );       } } // 61             AChar         iCode4        Insertion code for fourth residue

            ////////////////////////////////////////////////////////////////////////////////////
            // Serializable
            public Site(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt) { }
            public override void GetObjectData(SerializationInfo info, StreamingContext context) { base.GetObjectData(info, context); }
        }
    }
}
