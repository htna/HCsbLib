using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using AminoAcid = HBioinfo.AminoAcids.AminoAcid;
    public static partial class HStaticBioinfo
    {
        public static Dictionary<string,AminoAcid> ToDictionaryBy3Letter(this IList<AminoAcid> acids, bool toupper)
        {
            Dictionary<string,AminoAcid> dict = new Dictionary<string,AminoAcid>();
            foreach(AminoAcid acid in acids)
            {
                string name3 = acid.name3;
                if(toupper) name3 = name3.ToUpper();
                dict.Add(name3, acid);
            }
            return dict;
        }
        public static Dictionary<char, AminoAcid> ToDictionaryBy1Letter(this IList<AminoAcid> acids)
        {
            Dictionary<char,AminoAcid> dict = new Dictionary<char, AminoAcid>();
            foreach(AminoAcid acid in acids)
                dict.Add(acid.name1, acid);
            return dict;
        }
        public static string[] HBioinfoConvertTo3Letter(              params char  [] resis1) { return HBioinfo.AminoAcids.Convert_1Letter_to_3Letter(         resis1); }
        public static char  [] HBioinfoConvertTo1Letter(bool toupper, params string[] resis3) { return HBioinfo.AminoAcids.Convert_3Letter_to_1Letter(toupper, resis3); }
    }
    public static partial class HBioinfo
    {
        public partial class AminoAcids
        {
            public class AminoAcid
            {
                public string name;     // full name
                public char   name1;    // 1 letter
                public string name3;    // 3 letters
                public override string ToString()
                {
                    return string.Format("{0} ({1}, {2})", name3, name1, name);
                }
            };

            static Dictionary<string, object> _dict = new Dictionary<string,object>();
            public static Dictionary<char,  AminoAcid> ToDictionaryBy1Letter()
            {
                if(_dict.ContainsKey("ToDictionaryBy1Letter()") == false)
                    _dict.Add("ToDictionaryBy1Letter()", GetAminoAcids().ToDictionaryBy1Letter());
                return _dict["ToDictionaryBy1Letter()"] as Dictionary<char, AminoAcid>;
            }
            public static Dictionary<string,AminoAcid> ToDictionaryBy3Letter(bool toupper)
            {
                if(toupper)
                {
                    if(_dict.ContainsKey("ToDictionaryBy3Letter(true)") == false)
                        _dict.Add("ToDictionaryBy3Letter(true)", GetAminoAcids().ToDictionaryBy3Letter(true));
                    return _dict["ToDictionaryBy3Letter(true)"] as Dictionary<string, AminoAcid>;
                }
                else
                {
                    if(_dict.ContainsKey("ToDictionaryBy3Letter(false)") == false)
                        _dict.Add("ToDictionaryBy3Letter(false)", GetAminoAcids().ToDictionaryBy3Letter(false));
                    return _dict["ToDictionaryBy3Letter(false)"] as Dictionary<string, AminoAcid>;
                }
            }


            public static string[] Convert_1Letter_to_3Letter(params char[] resis1)
            {
                var dict = GetAminoAcids().ToDictionaryBy1Letter();
                string[] resis3 = new string[resis1.Length];
                for(int i=0; i<resis1.Length; i++)
                    resis3[i] = dict[resis1[i]].name3;
                return resis3;
            }
            public static char[] Convert_3Letter_to_1Letter(bool toupper, params string[] resis3)
            {
                var dict = GetAminoAcids().ToDictionaryBy3Letter(toupper);
                char[] resis1 = new char[resis3.Length];
                for(int i=0; i<resis3.Length; i++)
                    resis1[i] = dict[resis3[i]].name1;
                return resis1;
            }
            public static AminoAcid[] GetAminoAcids()
            {
                // http://en.wikipedia.org/wiki/Amino_acid
                // | Amino Acid    | 3-Letter | 1-Letter | Side-chain    | Side-chain charge(pH 7.4)  | Hydropathy | Absorbance    | ε at λmax         | MW(Weight) |
                // |               | 3-Letter | 1-Letter | polarity      |                            | index      | λmax(nm)      | (mM-1 cm-1)       |            |
                // ==================================================================================================================================================
                // | Alanine       | Ala      | A        | nonpolar      | neutral                    |  1.8       |               |                   | 89         |
                // | Arginine      | Arg      | R        | Basic polar   | positive                   | -4.5       |               |                   | 174        |
                // | Asparagine    | Asn      | N        | polar         | neutral                    | -3.5       |               |                   | 132        |
                // | Aspartic acid | Asp      | D        | acidic polar  | negative                   | -3.5       |               |                   | 133        |
                // | Cysteine      | Cys      | C        | nonpolar      | neutral                    |  2.5       | 250           | 0.3               | 121        |
                // | Glutamic acid | Glu      | E        | acidic polar  | negative                   | -3.5       |               |                   | 147        |
                // | Glutamine     | Gln      | Q        | polar         | neutral                    | -3.5       |               |                   | 146        |
                // | Glycine       | Gly      | G        | nonpolar      | neutral                    | -0.4       |               |                   | 75         |
                // | Histidine     | His      | H        | Basic polar   | positive(10%),neutral(90%) | -3.2       | 211           | 5.9               | 155        |
                // | Isoleucine    | Ile      | I        | nonpolar      | neutral                    |  4.5       |               |                   | 131        |
                // | Leucine       | Leu      | L        | nonpolar      | neutral                    |  3.8       |               |                   | 131        |
                // | Lysine        | Lys      | K        | Basic polar   | positive                   | -3.9       |               |                   | 146        |
                // | Methionine    | Met      | M        | nonpolar      | neutral                    |  1.9       |               |                   | 149        |
                // | Phenylalanine | Phe      | F        | nonpolar      | neutral                    |  2.8       | 257, 206, 188 | 0.2, 9.3, 60.0    | 165        |
                // | Proline       | Pro      | P        | nonpolar      | neutral                    | -1.6       |               |                   | 115        |
                // | Serine        | Ser      | S        | polar         | neutral                    | -0.8       |               |                   | 105        |
                // | Threonine     | Thr      | T        | polar         | neutral                    | -0.7       |               |                   | 119        |
                // | Tryptophan    | Trp      | W        | nonpolar      | neutral                    | -0.9       | 280, 219      | 5.6, 47.0         | 204        |
                // | Tyrosine      | Tyr      | Y        | polar         | neutral                    | -1.3       | 274, 222, 193 | 1.4, 8.0, 48.0    | 181        |
                // | Valine        | Val      | V        | nonpolar      | neutral                    |  4.2       |               |                   | 117        |

                return new AminoAcid[]
                {
                    new AminoAcid { name="Alanine"      , name3="Ala", name1='A'},
                    new AminoAcid { name="Arginine"     , name3="Arg", name1='R'},
                    new AminoAcid { name="Asparagine"   , name3="Asn", name1='N'},
                    new AminoAcid { name="Aspartic acid", name3="Asp", name1='D'},
                    new AminoAcid { name="Cysteine"     , name3="Cys", name1='C'},
                    new AminoAcid { name="Glutamic acid", name3="Glu", name1='E'},
                    new AminoAcid { name="Glutamine"    , name3="Gln", name1='Q'},
                    new AminoAcid { name="Glycine"      , name3="Gly", name1='G'},
                    new AminoAcid { name="Histidine"    , name3="His", name1='H'},
                    new AminoAcid { name="Isoleucine"   , name3="Ile", name1='I'},
                    new AminoAcid { name="Leucine"      , name3="Leu", name1='L'},
                    new AminoAcid { name="Lysine"       , name3="Lys", name1='K'},
                    new AminoAcid { name="Methionine"   , name3="Met", name1='M'},
                    new AminoAcid { name="Phenylalanine", name3="Phe", name1='F'},
                    new AminoAcid { name="Proline"      , name3="Pro", name1='P'},
                    new AminoAcid { name="Serine"       , name3="Ser", name1='S'},
                    new AminoAcid { name="Threonine"    , name3="Thr", name1='T'},
                    new AminoAcid { name="Tryptophan"   , name3="Trp", name1='W'},
                    new AminoAcid { name="Tyrosine"     , name3="Tyr", name1='Y'},
                    new AminoAcid { name="Valine"       , name3="Val", name1='V'},
                };
            }
        }
    }
}
