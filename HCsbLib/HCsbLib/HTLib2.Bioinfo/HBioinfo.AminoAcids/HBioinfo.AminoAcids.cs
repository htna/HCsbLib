using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using AminoAcid = HBioinfo.AminoAcid;
    public static partial class HStaticBioinfo
    {
        //public static Dictionary<string,AminoAcid> ToDictionaryBy3Letter(this IList<AminoAcid> acids, bool toupper)
        //{
        //    Dictionary<string,AminoAcid> dict = new Dictionary<string,AminoAcid>();
        //    foreach(AminoAcid acid in acids)
        //    {
        //        string name3 = acid.name3;
        //        if(toupper) name3 = name3.ToUpper();
        //        dict.Add(name3, acid);
        //    }
        //    return dict;
        //}
        //public static Dictionary<char, AminoAcid> ToDictionaryBy1Letter(this IList<AminoAcid> acids)
        //{
        //    Dictionary<char,AminoAcid> dict = new Dictionary<char, AminoAcid>();
        //    foreach(AminoAcid acid in acids)
        //        dict.Add(acid.name1, acid);
        //    return dict;
        //}
        //public static string[] HBioinfoConvertTo3Letter(              params char  [] resis1) { return HBioinfo.AminoAcids.Convert_1Letter_to_3Letter(         resis1); }
        //public static char  [] HBioinfoConvertTo1Letter(bool toupper, params string[] resis3) { return HBioinfo.AminoAcids.Convert_3Letter_to_1Letter(toupper, resis3); }

        public static IReadOnlyList<AminoAcid> GerResAminoAcid(this Pdb.Atom atom)
        {
            string name3 = atom.resName;
            HDebug.Assert(name3.Length == 3);
            return AminoAcid.From3Letter(name3);
        }
        public static char? GetResNameSyn(this Pdb.Atom atom)
        {
            var aa = atom.GerResAminoAcid();
            if(aa == null)
                return null;
            if(aa.Count == 1)
                return aa[0].name1;
            throw new NotImplementedException();
        }
    }
    public static partial class HBioinfo
    {
        public class AminoAcid
        {
            public readonly string name;     // full name
            public readonly char?  name1;    // 1 letter
            public readonly string name3;    // 3 letters
            public readonly SideChainPolarity sidechainPolarity;        public enum SideChainPolarity { NA, NonPolar, BasicPolar, Polar, AcidicPolar };
            public readonly double HydropathyIndex;
            public readonly double Weight;

            public override string ToString()
            {
                return string.Format("{0} ({1}, {2})", name3, name1, name);
            }
            protected AminoAcid
                ( string name                         = null
                , char?  name1                        = null
                , string name3                        = null
                , SideChainPolarity sidechainPolarity = SideChainPolarity.NA
                , double HydropathyIndex              = double.NaN
                , double Weight                       = double.NaN
                )
            {
                this.name              = name             ;
                this.name1             = name1            ;
                this.name3             = name3            ;
                this.sidechainPolarity = sidechainPolarity;
                this.HydropathyIndex   = HydropathyIndex  ;
                this.Weight            = Weight           ;
            }

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
            public static readonly AminoAcid Alanine       = new AminoAcid ( name:"Alanine"      , name3:"Ala", name1:'A', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 1.8, Weight:89  );
            public static readonly AminoAcid Arginine      = new AminoAcid ( name:"Arginine"     , name3:"Arg", name1:'R', sidechainPolarity:SideChainPolarity.BasicPolar , HydropathyIndex:-4.5, Weight:174 );
            public static readonly AminoAcid Asparagine    = new AminoAcid ( name:"Asparagine"   , name3:"Asn", name1:'N', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-3.5, Weight:132 );
            public static readonly AminoAcid AsparticAcid  = new AminoAcid ( name:"Aspartic acid", name3:"Asp", name1:'D', sidechainPolarity:SideChainPolarity.AcidicPolar, HydropathyIndex:-3.5, Weight:133 );
            public static readonly AminoAcid Cysteine      = new AminoAcid ( name:"Cysteine"     , name3:"Cys", name1:'C', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 2.5, Weight:121 );
            public static readonly AminoAcid GlutamicAcid  = new AminoAcid ( name:"Glutamic acid", name3:"Glu", name1:'E', sidechainPolarity:SideChainPolarity.AcidicPolar, HydropathyIndex:-3.5, Weight:147 );
            public static readonly AminoAcid Glutamine     = new AminoAcid ( name:"Glutamine"    , name3:"Gln", name1:'Q', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-3.5, Weight:146 );
            public static readonly AminoAcid Glycine       = new AminoAcid ( name:"Glycine"      , name3:"Gly", name1:'G', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex:-0.4, Weight:75  );
            public static readonly AminoAcid Histidine     = new AminoAcid ( name:"Histidine"    , name3:"His", name1:'H', sidechainPolarity:SideChainPolarity.BasicPolar , HydropathyIndex:-3.2, Weight:155 );
            public static readonly AminoAcid Isoleucine    = new AminoAcid ( name:"Isoleucine"   , name3:"Ile", name1:'I', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 4.5, Weight:131 );
            public static readonly AminoAcid Leucine       = new AminoAcid ( name:"Leucine"      , name3:"Leu", name1:'L', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 3.8, Weight:131 );
            public static readonly AminoAcid Lysine        = new AminoAcid ( name:"Lysine"       , name3:"Lys", name1:'K', sidechainPolarity:SideChainPolarity.BasicPolar , HydropathyIndex:-3.9, Weight:146 );
            public static readonly AminoAcid Methionine    = new AminoAcid ( name:"Methionine"   , name3:"Met", name1:'M', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 1.9, Weight:149 );
            public static readonly AminoAcid Phenylalanine = new AminoAcid ( name:"Phenylalanine", name3:"Phe", name1:'F', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 2.8, Weight:165 );
            public static readonly AminoAcid Proline       = new AminoAcid ( name:"Proline"      , name3:"Pro", name1:'P', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex:-1.6, Weight:115 );
            public static readonly AminoAcid Serine        = new AminoAcid ( name:"Serine"       , name3:"Ser", name1:'S', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-0.8, Weight:105 );
            public static readonly AminoAcid Threonine     = new AminoAcid ( name:"Threonine"    , name3:"Thr", name1:'T', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-0.7, Weight:119 );
            public static readonly AminoAcid Tryptophan    = new AminoAcid ( name:"Tryptophan"   , name3:"Trp", name1:'W', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex:-0.9, Weight:204 );
            public static readonly AminoAcid Tyrosine      = new AminoAcid ( name:"Tyrosine"     , name3:"Tyr", name1:'Y', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-1.3, Weight:181 );
            public static readonly AminoAcid Valine        = new AminoAcid ( name:"Valine"       , name3:"Val", name1:'V', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 4.2, Weight:117 );

            // http://www.ccp4.ac.uk/html/pdbformat.html
            //  Acidic unknown              ACD         Homoserine                  HSE
            //  Acetyl                      ACE         Hydroxyproline              HYP
            //  Alanine                     ALA *       Hydroxylysine               HYL
            //  beta-Alanine                ALB         Isoleucine                  ILE *
            //  Aliphatic unknown           ALI         Leucine                     LEU *
            //  gamma-Aminobutyric acid     ABU         Lysine                      LYS *
            //  Arginine                    ARG *       Methionine                  MET *
            //  Aromatic unknown            ARO         Ornithine                   ORN
            //  Asparagine                  ASN *       Phenylalanine               PHE *
            //  Aspartic acid               ASP *       Proline                     PRO *
            //  ASP/ASN ambiguous           ASX         Pyrollidone carboxylic acid PCA
            //  Basic unknown               BAS         Sarcosine                   SAR
            //  Betaine                     BET         Serine                      SER *
            //  Cysteine                    CYS *       Taurine                     TAU
            //  Cystine                     CYS *       Terminator                  TER
            //  Formyl                      FOR         Threonine                   THR *
            //  Glutamic acid               GLU *       Thyroxine                   THY
            //  Glutamine                   GLN *       Tryptophan                  TRP *
            //  GLU/GLN ambiguous           GLX         Tyrosine                    TYR *
            //  Glycine                     GLY *       Unknown                     UNK
            //  Heterogen                   HET         Valine                      VAL *
            //  Histidine                   HIS *       Water                       HOH
            public static readonly AminoAcid AcidicUnknown              = new AminoAcid ( name:"Acidic unknown"             , name3:"ACD"               );
            public static readonly AminoAcid Acetyl                     = new AminoAcid ( name:"Acetyl"                     , name3:"ACE"               );
            public static readonly AminoAcid Hydroxylysine              = new AminoAcid ( name:"Hydroxylysine"              , name3:"HYL"               );
            public static readonly AminoAcid BetaAlanine                = new AminoAcid ( name:"beta-Alanine"               , name3:"ALB"               );
            public static readonly AminoAcid AliphaticUnknown           = new AminoAcid ( name:"Aliphatic unknown"          , name3:"ALI"               );
            public static readonly AminoAcid GammaAminobutyricAcid      = new AminoAcid ( name:"gamma-Aminobutyric acid"    , name3:"ABU"               );
            public static readonly AminoAcid AromaticUnknown            = new AminoAcid ( name:"Aromatic unknown"           , name3:"ARO"               );
            public static readonly AminoAcid AspAsnAmbiguous            = new AminoAcid ( name:"ASP/ASN ambiguous"          , name3:"ASX", name1:'B'    );
            public static readonly AminoAcid BasicUnknown               = new AminoAcid ( name:"Basic unknown"              , name3:"BAS"               );
            public static readonly AminoAcid Betaine                    = new AminoAcid ( name:"Betaine"                    , name3:"BET"               );
            public static readonly AminoAcid Taurine                    = new AminoAcid ( name:"Taurine"                    , name3:"TAU"               );
            public static readonly AminoAcid Terminator                 = new AminoAcid ( name:"Terminator"                 , name3:"TER"               );
            public static readonly AminoAcid Formyl                     = new AminoAcid ( name:"Formyl"                     , name3:"FOR"               );
            public static readonly AminoAcid Thyroxine                  = new AminoAcid ( name:"Thyroxine"                  , name3:"THY"               );
            public static readonly AminoAcid GluGlnAmbiguous            = new AminoAcid ( name:"GLU/GLN ambiguous"          , name3:"GLX", name1:'Z'    );
            public static readonly AminoAcid Unknown                    = new AminoAcid ( name:"Unknown"                    , name3:"UNK"               );
            public static readonly AminoAcid Heterogen                  = new AminoAcid ( name:"Heterogen"                  , name3:"HET"               );
            public static readonly AminoAcid Water                      = new AminoAcid ( name:"Water"                      , name3:"HOH"               );
            public static readonly AminoAcid Homoserine                 = new AminoAcid ( name:"Homoserine"                 , name3:"HSE"               );
            public static readonly AminoAcid Hydroxyproline             = new AminoAcid ( name:"Hydroxyproline"             , name3:"HYP"               );
            public static readonly AminoAcid Ornithine                  = new AminoAcid ( name:"Ornithine"                  , name3:"ORN"               );
            public static readonly AminoAcid PyrollidoneCarboxylicAcid  = new AminoAcid ( name:"Pyrollidone carboxylic acid", name3:"PCA"               );
            public static readonly AminoAcid Sarcosine                  = new AminoAcid ( name:"Sarcosine"                  , name3:"SAR"               );
                    
            // NAMD pdbalias
            public static readonly AminoAcid NamdAliasHSD = new AminoAcid ( name3:"HSD", name1:'H' ); // HIS -> HSD
            public static readonly AminoAcid NamdAliasHSE = new AminoAcid ( name3:"HSE", name1:'H' ); // HIS -> HSE
            public static readonly AminoAcid NamdAliasHSP = new AminoAcid ( name3:"HSP", name1:'H' ); // HIS -> HSP

            public static readonly IReadOnlyList<AminoAcid> AminoAcids = new AminoAcid[]
                { Alanine, Arginine, Asparagine, AsparticAcid, Cysteine, GlutamicAcid, Glutamine, Glycine, Histidine, Isoleucine, Leucine, Lysine, Methionine, Phenylalanine, Proline, Serine, Threonine, Tryptophan, Tyrosine, Valine
                // pdb
                , AcidicUnknown, Acetyl, Hydroxylysine, BetaAlanine, AliphaticUnknown, GammaAminobutyricAcid, AromaticUnknown, AspAsnAmbiguous, BasicUnknown, Betaine, Taurine, Terminator, Formyl, Thyroxine, GluGlnAmbiguous, Unknown                  
                , Heterogen, Water, Homoserine, Hydroxyproline, Ornithine, PyrollidoneCarboxylicAcid, Sarcosine
                // NAMD pdbalias
                , NamdAliasHSD, NamdAliasHSE, NamdAliasHSP
                };

            // https://www.sigmaaldrich.com/life-science/metabolomics/learning-center/amino-acid-reference-chart.html
            public static readonly IReadOnlyList<AminoAcid> AminoAcidsWithHydrophobicSideChain_Aliphatic       = new AminoAcid[] { Alanine, Isoleucine, Leucine, Methionine, Valine, };
            public static readonly IReadOnlyList<AminoAcid> AminoAcidsWithHydrophobicSideChain_Aromatic        = new AminoAcid[] { Phenylalanine, Tryptophan, Tyrosine, };
            public static readonly IReadOnlyList<AminoAcid> AminoAcidsWithPolarNeutralSideChains               = new AminoAcid[] { Asparagine, Cysteine, Glutamine, Serine, Threonine, };
            public static readonly IReadOnlyList<AminoAcid> AminoAcidsWithElectricallyChargedSideChains_Acidic = new AminoAcid[] { AsparticAcid,  AsparticAcid, GlutamicAcid, };
            public static readonly IReadOnlyList<AminoAcid> AminoAcidsWithElectricallyChargedSideChains_Basic  = new AminoAcid[] { Arginine, Histidine, Lysine, };
            public static readonly IReadOnlyList<AminoAcid> AminoAcidsUnique                                   = new AminoAcid[] { Glycine, Proline, };

            public static readonly IReadOnlyList<AminoAcid> AminoAcidsHydrophobic = new AminoAcid[] { Glycine,     Alanine, Valine, Phenylalanine, Proline, Leucine, Isoleucine, };
            public static readonly IReadOnlyList<AminoAcid> AminoAcidsHydrophilic = new AminoAcid[] { Arginine, AsparticAcid, GlutamicAcid, Serine, Cysteine, Asparagine, Glutamine, Histidine, };
            public static readonly IReadOnlyList<AminoAcid> AminoAcidsAmphipathic = new AminoAcid[] { Threonine, Lysine, Tyrosine, Methionine, Tryptophan, };

            public static IReadOnlyList<AminoAcid> GetAminoAcidsByType(string type)
            {
                switch(type)
                {
                    case "AminoAcidsWithHydrophobicSideChain_Aliphatic"       : return AminoAcidsWithHydrophobicSideChain_Aliphatic      ;
                    case "AminoAcidsWithHydrophobicSideChain_Aromatic"        : return AminoAcidsWithHydrophobicSideChain_Aromatic       ;
                    case "AminoAcidsWithPolarNeutralSideChains"               : return AminoAcidsWithPolarNeutralSideChains              ;
                    case "AminoAcidsWithElectricallyChargedSideChains_Acidic" : return AminoAcidsWithElectricallyChargedSideChains_Acidic;
                    case "AminoAcidsWithElectricallyChargedSideChains_Basic"  : return AminoAcidsWithElectricallyChargedSideChains_Basic ;
                    case "AminoAcidsUnique"                                   : return AminoAcidsUnique                                  ;
                    case "AminoAcidsHydrophobic"                              : return AminoAcidsHydrophobic                             ;
                    case "AminoAcidsHydrophilic"                              : return AminoAcidsHydrophilic                             ;
                    case "AminoAcidsAmphipathic"                              : return AminoAcidsAmphipathic                             ;
                    default:
                        return null;
                }
            }

            static Dictionary<string, AminoAcid[]> _distName3 = null;
            public static IReadOnlyList<AminoAcid> From3Letter(string name3)
            {
                if(_distName3 == null)
                {
                    _distName3 = new Dictionary<string, AminoAcid[]>();
                    foreach(var aa in AminoAcids)
                    {
                        string aa_name3 = aa.name3.ToUpper().Trim();
                        if(_distName3.ContainsKey(aa_name3) == false)
                            _distName3.Add(aa_name3, new AminoAcid[] { aa });
                        else
                            _distName3[aa_name3] = _distName3[aa_name3].HAdd(aa);
                    }
                }
                if(_distName3.ContainsKey(name3))
                    return _distName3[name3];
                return null;
            }
            static Dictionary<char, AminoAcid> _distName1 = null;
            public static AminoAcid From1Letter(char name1)
            {
                if(_distName1 == null)
                {
                    _distName1 = new Dictionary<char, AminoAcid>();
                    foreach(var aa in AminoAcids)
                        if(aa.name1 != null)
                            _distName1.Add(aa.name1.Value, aa);
                }
                if(_distName1.ContainsKey(name1))
                    return _distName1[name1];
                return null;
            }
        }
    }
}
