using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Acid = HBioinfo.Acid;
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

        public static IReadOnlyList<Acid> GerResAminoAcid(this Pdb.Atom atom)
        {
            string name3 = atom.resName;
            HDebug.Assert(name3.Length == 3);
            return Acid.From3Letter(name3);
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
        public class Acid
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
            protected Acid
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
            public static readonly Acid Alanine       = new Acid ( name:"Alanine"      , name3:"Ala", name1:'A', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 1.8, Weight:89  );
            public static readonly Acid Arginine      = new Acid ( name:"Arginine"     , name3:"Arg", name1:'R', sidechainPolarity:SideChainPolarity.BasicPolar , HydropathyIndex:-4.5, Weight:174 );
            public static readonly Acid Asparagine    = new Acid ( name:"Asparagine"   , name3:"Asn", name1:'N', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-3.5, Weight:132 );
            public static readonly Acid AsparticAcid  = new Acid ( name:"Aspartic acid", name3:"Asp", name1:'D', sidechainPolarity:SideChainPolarity.AcidicPolar, HydropathyIndex:-3.5, Weight:133 );
            public static readonly Acid Cysteine      = new Acid ( name:"Cysteine"     , name3:"Cys", name1:'C', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 2.5, Weight:121 );
            public static readonly Acid GlutamicAcid  = new Acid ( name:"Glutamic acid", name3:"Glu", name1:'E', sidechainPolarity:SideChainPolarity.AcidicPolar, HydropathyIndex:-3.5, Weight:147 );
            public static readonly Acid Glutamine     = new Acid ( name:"Glutamine"    , name3:"Gln", name1:'Q', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-3.5, Weight:146 );
            public static readonly Acid Glycine       = new Acid ( name:"Glycine"      , name3:"Gly", name1:'G', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex:-0.4, Weight:75  );
            public static readonly Acid Histidine     = new Acid ( name:"Histidine"    , name3:"His", name1:'H', sidechainPolarity:SideChainPolarity.BasicPolar , HydropathyIndex:-3.2, Weight:155 );
            public static readonly Acid Isoleucine    = new Acid ( name:"Isoleucine"   , name3:"Ile", name1:'I', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 4.5, Weight:131 );
            public static readonly Acid Leucine       = new Acid ( name:"Leucine"      , name3:"Leu", name1:'L', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 3.8, Weight:131 );
            public static readonly Acid Lysine        = new Acid ( name:"Lysine"       , name3:"Lys", name1:'K', sidechainPolarity:SideChainPolarity.BasicPolar , HydropathyIndex:-3.9, Weight:146 );
            public static readonly Acid Methionine    = new Acid ( name:"Methionine"   , name3:"Met", name1:'M', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 1.9, Weight:149 );
            public static readonly Acid Phenylalanine = new Acid ( name:"Phenylalanine", name3:"Phe", name1:'F', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 2.8, Weight:165 );
            public static readonly Acid Proline       = new Acid ( name:"Proline"      , name3:"Pro", name1:'P', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex:-1.6, Weight:115 );
            public static readonly Acid Serine        = new Acid ( name:"Serine"       , name3:"Ser", name1:'S', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-0.8, Weight:105 );
            public static readonly Acid Threonine     = new Acid ( name:"Threonine"    , name3:"Thr", name1:'T', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-0.7, Weight:119 );
            public static readonly Acid Tryptophan    = new Acid ( name:"Tryptophan"   , name3:"Trp", name1:'W', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex:-0.9, Weight:204 );
            public static readonly Acid Tyrosine      = new Acid ( name:"Tyrosine"     , name3:"Tyr", name1:'Y', sidechainPolarity:SideChainPolarity.Polar      , HydropathyIndex:-1.3, Weight:181 );
            public static readonly Acid Valine        = new Acid ( name:"Valine"       , name3:"Val", name1:'V', sidechainPolarity:SideChainPolarity.NonPolar   , HydropathyIndex: 4.2, Weight:117 );

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
            public static readonly Acid AcidicUnknown              = new Acid ( name:"Acidic unknown"             , name3:"ACD"               );
            public static readonly Acid Acetyl                     = new Acid ( name:"Acetyl"                     , name3:"ACE"               );
            public static readonly Acid Hydroxylysine              = new Acid ( name:"Hydroxylysine"              , name3:"HYL"               );
            public static readonly Acid BetaAlanine                = new Acid ( name:"beta-Alanine"               , name3:"ALB"               );
            public static readonly Acid AliphaticUnknown           = new Acid ( name:"Aliphatic unknown"          , name3:"ALI"               );
            public static readonly Acid GammaAminobutyricAcid      = new Acid ( name:"gamma-Aminobutyric acid"    , name3:"ABU"               );
            public static readonly Acid AromaticUnknown            = new Acid ( name:"Aromatic unknown"           , name3:"ARO"               );
            public static readonly Acid AspAsnAmbiguous            = new Acid ( name:"ASP/ASN ambiguous"          , name3:"ASX", name1:'B'    );
            public static readonly Acid BasicUnknown               = new Acid ( name:"Basic unknown"              , name3:"BAS"               );
            public static readonly Acid Betaine                    = new Acid ( name:"Betaine"                    , name3:"BET"               );
            public static readonly Acid Taurine                    = new Acid ( name:"Taurine"                    , name3:"TAU"               );
            public static readonly Acid Terminator                 = new Acid ( name:"Terminator"                 , name3:"TER"               );
            public static readonly Acid Formyl                     = new Acid ( name:"Formyl"                     , name3:"FOR"               );
            public static readonly Acid Thyroxine                  = new Acid ( name:"Thyroxine"                  , name3:"THY"               );
            public static readonly Acid GluGlnAmbiguous            = new Acid ( name:"GLU/GLN ambiguous"          , name3:"GLX", name1:'Z'    );
            public static readonly Acid Unknown                    = new Acid ( name:"Unknown"                    , name3:"UNK"               );
            public static readonly Acid Heterogen                  = new Acid ( name:"Heterogen"                  , name3:"HET"               );
            public static readonly Acid Water                      = new Acid ( name:"Water"                      , name3:"HOH"               );
            public static readonly Acid Homoserine                 = new Acid ( name:"Homoserine"                 , name3:"HSE"               );
            public static readonly Acid Hydroxyproline             = new Acid ( name:"Hydroxyproline"             , name3:"HYP"               );
            public static readonly Acid Ornithine                  = new Acid ( name:"Ornithine"                  , name3:"ORN"               );
            public static readonly Acid PyrollidoneCarboxylicAcid  = new Acid ( name:"Pyrollidone carboxylic acid", name3:"PCA"               );
            public static readonly Acid Sarcosine                  = new Acid ( name:"Sarcosine"                  , name3:"SAR"               );
                    
            // NAMD pdbalias
            public static readonly Acid NamdAliasHSD = new Acid ( name3:"HSD", name1:'H' ); // HIS -> HSD
            public static readonly Acid NamdAliasHSE = new Acid ( name3:"HSE", name1:'H' ); // HIS -> HSE
            public static readonly Acid NamdAliasHSP = new Acid ( name3:"HSP", name1:'H' ); // HIS -> HSP

            public static readonly IReadOnlyList<Acid> AminoAcids = new Acid[]
                { Alanine, Arginine, Asparagine, AsparticAcid, Cysteine, GlutamicAcid, Glutamine, Glycine, Histidine, Isoleucine, Leucine, Lysine, Methionine, Phenylalanine, Proline, Serine, Threonine, Tryptophan, Tyrosine, Valine
                // pdb
                , AcidicUnknown, Acetyl, Hydroxylysine, BetaAlanine, AliphaticUnknown, GammaAminobutyricAcid, AromaticUnknown, AspAsnAmbiguous, BasicUnknown, Betaine, Taurine, Terminator, Formyl, Thyroxine, GluGlnAmbiguous, Unknown                  
                , Heterogen, Water, Homoserine, Hydroxyproline, Ornithine, PyrollidoneCarboxylicAcid, Sarcosine
                // NAMD pdbalias
                , NamdAliasHSD, NamdAliasHSE, NamdAliasHSP
                };

            // https://www.sigmaaldrich.com/life-science/metabolomics/learning-center/amino-acid-reference-chart.html
            public static readonly IReadOnlyList<Acid> AminoAcidsWithHydrophobicSideChain_Aliphatic       = new Acid[] { Alanine, Isoleucine, Leucine, Methionine, Valine, };
            public static readonly IReadOnlyList<Acid> AminoAcidsWithHydrophobicSideChain_Aromatic        = new Acid[] { Phenylalanine, Tryptophan, Tyrosine, };
            public static readonly IReadOnlyList<Acid> AminoAcidsWithPolarNeutralSideChains               = new Acid[] { Asparagine, Cysteine, Glutamine, Serine, Threonine, };
            public static readonly IReadOnlyList<Acid> AminoAcidsWithElectricallyChargedSideChains_Acidic = new Acid[] { AsparticAcid,  AsparticAcid, GlutamicAcid, };
            public static readonly IReadOnlyList<Acid> AminoAcidsWithElectricallyChargedSideChains_Basic  = new Acid[] { Arginine, Histidine, Lysine, };
            public static readonly IReadOnlyList<Acid> AminoAcidsUnique                                   = new Acid[] { Glycine, Proline, };

            public static readonly IReadOnlyList<Acid> AminoAcidsHydrophobic = new Acid[] { Glycine,     Alanine, Valine, Phenylalanine, Proline, Leucine, Isoleucine, };
            public static readonly IReadOnlyList<Acid> AminoAcidsHydrophilic = new Acid[] { Arginine, AsparticAcid, GlutamicAcid, Serine, Cysteine, Asparagine, Glutamine, Histidine, };
            public static readonly IReadOnlyList<Acid> AminoAcidsAmphipathic = new Acid[] { Threonine, Lysine, Tyrosine, Methionine, Tryptophan, };

            public static IReadOnlyList<Acid> GetAminoAcidsByType(string type)
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

            static Dictionary<string, Acid[]> _distName3 = null;
            public static IReadOnlyList<Acid> From3Letter(string name3)
            {
                if(_distName3 == null)
                {
                    _distName3 = new Dictionary<string, Acid[]>();
                    foreach(var aa in AminoAcids)
                    {
                        string aa_name3 = aa.name3.ToUpper().Trim();
                        if(_distName3.ContainsKey(aa_name3) == false)
                            _distName3.Add(aa_name3, new Acid[] { aa });
                        else
                            _distName3[aa_name3] = _distName3[aa_name3].HAdd(aa);
                    }
                }
                if(_distName3.ContainsKey(name3))
                    return _distName3[name3];
                return null;
            }
            static Dictionary<char, Acid> _distName1 = null;
            public static Acid From1Letter(char name1)
            {
                if(_distName1 == null)
                {
                    _distName1 = new Dictionary<char, Acid>();
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
