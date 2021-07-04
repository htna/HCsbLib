using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Hess
{
    public partial class STeM
    {
        public class DataSet
        {
            public string pdbid;
            public double RÅ;
            public double ANM;
            public double GNM;
            public double STeM;

            public Pdb GetPdb()
            {
                Pdb pdb = PdbDatabase.GetPdb(pdbid);
                pdb = pdb.GetNoAltloc();
                return pdb;
            }
        }
        public static DataSet[] TuLiangDataSet = new DataSet[]
        {
            new DataSet{ pdbid="1AAC",  RÅ=1.31,  ANM=0.70,  GNM= 0.71,  STeM=0.76 },
            new DataSet{ pdbid="1ADS",  RÅ=1.65,  ANM=0.77,  GNM= 0.74,  STeM=0.71 },
            new DataSet{ pdbid="1AHC",  RÅ=2.00,  ANM=0.79,  GNM= 0.68,  STeM=0.61 },
            #region ...
            new DataSet{ pdbid="1AKY",  RÅ=1.63,  ANM=0.56,  GNM= 0.72,  STeM=0.60 },
            new DataSet{ pdbid="1AMM",  RÅ=1.20,  ANM=0.56,  GNM= 0.72,  STeM=0.55 },
            new DataSet{ pdbid="1AMP",  RÅ=1.80,  ANM=0.62,  GNM= 0.59,  STeM=0.68 },
            new DataSet{ pdbid="1ARB",  RÅ=1.20,  ANM=0.78,  GNM= 0.76,  STeM=0.83 },
            new DataSet{ pdbid="1ARS",  RÅ=1.80,  ANM=0.14,  GNM= 0.43,  STeM=0.41 },
            new DataSet{ pdbid="1ARU",  RÅ=1.60,  ANM=0.70,  GNM= 0.78,  STeM=0.79 },
            new DataSet{ pdbid="1BKF",  RÅ=1.60,  ANM=0.52,  GNM= 0.43,  STeM=0.50 },
            new DataSet{ pdbid="1BPI",  RÅ=1.09,  ANM=0.43,  GNM= 0.56,  STeM=0.57 },
            new DataSet{ pdbid="1CDG",  RÅ=2.00,  ANM=0.65,  GNM= 0.62,  STeM=0.71 },
            new DataSet{ pdbid="1CEM",  RÅ=1.65,  ANM=0.51,  GNM= 0.63,  STeM=0.76 },
            new DataSet{ pdbid="1CNR",  RÅ=1.05,  ANM=0.34,  GNM= 0.64,  STeM=0.42 },
            new DataSet{ pdbid="1CNV",  RÅ=1.65,  ANM=0.69,  GNM= 0.62,  STeM=0.68 },
            new DataSet{ pdbid="1CPN",  RÅ=1.80,  ANM=0.51,  GNM= 0.54,  STeM=0.56 },
            new DataSet{ pdbid="1CSH",  RÅ=1.65,  ANM=0.44,  GNM= 0.41,  STeM=0.57 },
            new DataSet{ pdbid="1CTJ",  RÅ=1.10,  ANM=0.47,  GNM= 0.39,  STeM=0.62 },
            new DataSet{ pdbid="1CUS",  RÅ=1.25,  ANM=0.74,  GNM= 0.66,  STeM=0.76 },
            new DataSet{ pdbid="1DAD",  RÅ=1.60,  ANM=0.28,  GNM= 0.50,  STeM=0.42 },
            new DataSet{ pdbid="1DDT",  RÅ=2.00,  ANM=0.21,  GNM=-0.01,  STeM=0.49 },
            new DataSet{ pdbid="1EDE",  RÅ=1.90,  ANM=0.67,  GNM= 0.63,  STeM=0.75 },
            new DataSet{ pdbid="1EZM",  RÅ=1.50,  ANM=0.56,  GNM= 0.60,  STeM=0.58 },
            new DataSet{ pdbid="1FNC",  RÅ=2.00,  ANM=0.29,  GNM= 0.59,  STeM=0.61 },
            new DataSet{ pdbid="1FRD",  RÅ=1.70,  ANM=0.54,  GNM= 0.83,  STeM=0.77 },
            new DataSet{ pdbid="1FUS",  RÅ=1.30,  ANM=0.40,  GNM= 0.63,  STeM=0.61 },
            new DataSet{ pdbid="1FXD",  RÅ=1.70,  ANM=0.58,  GNM= 0.56,  STeM=0.70 },
            new DataSet{ pdbid="1GIA",  RÅ=2.00,  ANM=0.68,  GNM= 0.67,  STeM=0.69 },
            new DataSet{ pdbid="1GKY",  RÅ=2.00,  ANM=0.36,  GNM= 0.55,  STeM=0.44 },
            new DataSet{ pdbid="1GOF",  RÅ=1.70,  ANM=0.75,  GNM= 0.76,  STeM=0.78 },
            new DataSet{ pdbid="1GPR",  RÅ=1.90,  ANM=0.65,  GNM= 0.62,  STeM=0.66 },
            new DataSet{ pdbid="1HFC",  RÅ=1.50,  ANM=0.63,  GNM= 0.38,  STeM=0.35 },
            new DataSet{ pdbid="1IAB",  RÅ=1.79,  ANM=0.36,  GNM= 0.42,  STeM=0.53 },
            new DataSet{ pdbid="1IAG",  RÅ=2.00,  ANM=0.34,  GNM= 0.52,  STeM=0.44 },
            new DataSet{ pdbid="1IFC",  RÅ=1.19,  ANM=0.61,  GNM= 0.67,  STeM=0.53 },
            new DataSet{ pdbid="1IGD",  RÅ=1.10,  ANM=0.18,  GNM= 0.44,  STeM=0.27 },
            new DataSet{ pdbid="1IRO",  RÅ=1.10,  ANM=0.82,  GNM= 0.51,  STeM=0.85 },
            new DataSet{ pdbid="1JBC",  RÅ=1.15,  ANM=0.72,  GNM= 0.70,  STeM=0.73 },
            new DataSet{ pdbid="1KNB",  RÅ=1.70,  ANM=0.63,  GNM= 0.66,  STeM=0.54 },
            new DataSet{ pdbid="1LAM",  RÅ=1.60,  ANM=0.53,  GNM= 0.63,  STeM=0.71 },
            new DataSet{ pdbid="1LCT",  RÅ=2.00,  ANM=0.52,  GNM= 0.57,  STeM=0.61 },
            new DataSet{ pdbid="1LIS",  RÅ=1.90,  ANM=0.16,  GNM= 0.43,  STeM=0.30 },
            new DataSet{ pdbid="1LIT",  RÅ=1.55,  ANM=0.65,  GNM= 0.62,  STeM=0.76 },
            new DataSet{ pdbid="1LST",  RÅ=1.80,  ANM=0.39,  GNM= 0.72,  STeM=0.73 },
            new DataSet{ pdbid="1MJC",  RÅ=2.00,  ANM=0.67,  GNM= 0.67,  STeM=0.61 },
            new DataSet{ pdbid="1MLA",  RÅ=1.50,  ANM=0.59,  GNM= 0.57,  STeM=0.54 },
            new DataSet{ pdbid="1MRJ",  RÅ=1.60,  ANM=0.66,  GNM= 0.49,  STeM=0.50 },
            new DataSet{ pdbid="1NAR",  RÅ=1.80,  ANM=0.62,  GNM= 0.76,  STeM=0.74 },
            new DataSet{ pdbid="1NFP",  RÅ=1.60,  ANM=0.23,  GNM= 0.48,  STeM=0.41 },
            new DataSet{ pdbid="1NIF",  RÅ=1.70,  ANM=0.42,  GNM= 0.58,  STeM=0.61 },
            new DataSet{ pdbid="1NPK",  RÅ=1.80,  ANM=0.53,  GNM= 0.55,  STeM=0.64 },
            new DataSet{ pdbid="1OMP",  RÅ=1.80,  ANM=0.61,  GNM= 0.63,  STeM=0.65 },
            new DataSet{ pdbid="1ONC",  RÅ=1.70,  ANM=0.55,  GNM= 0.70,  STeM=0.58 },
            new DataSet{ pdbid="1OSA",  RÅ=1.68,  ANM=0.36,  GNM= 0.42,  STeM=0.55 },
            new DataSet{ pdbid="1OYC",  RÅ=2.00,  ANM=0.78,  GNM= 0.73,  STeM=0.77 },
            new DataSet{ pdbid="1PBE",  RÅ=1.90,  ANM=0.53,  GNM= 0.61,  STeM=0.63 },
            new DataSet{ pdbid="1PDA",  RÅ=1.76,  ANM=0.60,  GNM= 0.76,  STeM=0.58 },
            new DataSet{ pdbid="1PHB",  RÅ=1.60,  ANM=0.56,  GNM= 0.52,  STeM=0.59 },
            new DataSet{ pdbid="1PHP",  RÅ=1.65,  ANM=0.59,  GNM= 0.63,  STeM=0.65 },
            new DataSet{ pdbid="1PII",  RÅ=2.00,  ANM=0.19,  GNM= 0.44,  STeM=0.28 },
            new DataSet{ pdbid="1PLC",  RÅ=1.33,  ANM=0.41,  GNM= 0.47,  STeM=0.42 },
            new DataSet{ pdbid="1POA",  RÅ=1.50,  ANM=0.54,  GNM= 0.66,  STeM=0.42 },
            new DataSet{ pdbid="1POC",  RÅ=2.00,  ANM=0.46,  GNM= 0.52,  STeM=0.39 },
            new DataSet{ pdbid="1PPN",  RÅ=1.60,  ANM=0.61,  GNM= 0.64,  STeM=0.67 },
            new DataSet{ pdbid="1PTF",  RÅ=1.60,  ANM=0.47,  GNM= 0.60,  STeM=0.54 },
            new DataSet{ pdbid="1PTX",  RÅ=1.30,  ANM=0.65,  GNM= 0.51,  STeM=0.62 },
            new DataSet{ pdbid="1RA9",  RÅ=2.00,  ANM=0.48,  GNM= 0.61,  STeM=0.53 },
            new DataSet{ pdbid="1RCF",  RÅ=1.40,  ANM=0.59,  GNM= 0.63,  STeM=0.58 },
            new DataSet{ pdbid="1REC",  RÅ=1.90,  ANM=0.34,  GNM= 0.50,  STeM=0.49 },
            new DataSet{ pdbid="1RIE",  RÅ=1.50,  ANM=0.71,  GNM= 0.25,  STeM=0.52 },
            new DataSet{ pdbid="1RIS",  RÅ=2.00,  ANM=0.25,  GNM= 0.24,  STeM=0.47 },
            new DataSet{ pdbid="1RRO",  RÅ=1.30,  ANM=0.08,  GNM= 0.31,  STeM=0.36 },
            new DataSet{ pdbid="1SBP",  RÅ=1.70,  ANM=0.69,  GNM= 0.72,  STeM=0.67 },
            new DataSet{ pdbid="1SMD",  RÅ=1.60,  ANM=0.50,  GNM= 0.62,  STeM=0.67 },
            new DataSet{ pdbid="1SNC",  RÅ=1.65,  ANM=0.68,  GNM= 0.71,  STeM=0.72 },
            new DataSet{ pdbid="1THG",  RÅ=1.80,  ANM=0.50,  GNM= 0.53,  STeM=0.50 },
            new DataSet{ pdbid="1TML",  RÅ=1.80,  ANM=0.64,  GNM= 0.64,  STeM=0.58 },
            new DataSet{ pdbid="1UBI",  RÅ=1.80,  ANM=0.56,  GNM= 0.69,  STeM=0.61 },
            new DataSet{ pdbid="1WHI",  RÅ=1.50,  ANM=0.12,  GNM= 0.33,  STeM=0.38 },
            new DataSet{ pdbid="1XIC",  RÅ=1.60,  ANM=0.29,  GNM= 0.40,  STeM=0.47 },
            new DataSet{ pdbid="2AYH",  RÅ=1.60,  ANM=0.63,  GNM= 0.73,  STeM=0.82 },
            new DataSet{ pdbid="2CBA",  RÅ=1.54,  ANM=0.67,  GNM= 0.75,  STeM=0.80 },
            new DataSet{ pdbid="2CMD",  RÅ=1.87,  ANM=0.68,  GNM= 0.60,  STeM=0.62 },
            new DataSet{ pdbid="2CPL",  RÅ=1.63,  ANM=0.61,  GNM= 0.60,  STeM=0.72 },
            new DataSet{ pdbid="2CTC",  RÅ=1.40,  ANM=0.63,  GNM= 0.67,  STeM=0.75 },
            new DataSet{ pdbid="2CY3",  RÅ=1.70,  ANM=0.51,  GNM= 0.50,  STeM=0.67 },
            new DataSet{ pdbid="2END",  RÅ=1.45,  ANM=0.63,  GNM= 0.71,  STeM=0.68 },
            new DataSet{ pdbid="2ERL",  RÅ=1.00,  ANM=0.74,  GNM= 0.73,  STeM=0.85 },
            new DataSet{ pdbid="2HFT",  RÅ=1.69,  ANM=0.63,  GNM= 0.79,  STeM=0.72 },
            new DataSet{ pdbid="2IHL",  RÅ=1.40,  ANM=0.62,  GNM= 0.69,  STeM=0.72 },
            new DataSet{ pdbid="2MCM",  RÅ=1.50,  ANM=0.78,  GNM= 0.83,  STeM=0.79 },
            new DataSet{ pdbid="2MHR",  RÅ=1.30,  ANM=0.65,  GNM= 0.52,  STeM=0.64 },
            new DataSet{ pdbid="2MNR",  RÅ=1.90,  ANM=0.46,  GNM= 0.50,  STeM=0.47 },
            new DataSet{ pdbid="2PHY",  RÅ=1.40,  ANM=0.54,  GNM= 0.55,  STeM=0.68 },
            new DataSet{ pdbid="2RAN",  RÅ=1.89,  ANM=0.43,  GNM= 0.40,  STeM=0.31 },
            new DataSet{ pdbid="2RHE",  RÅ=1.60,  ANM=0.28,  GNM= 0.38,  STeM=0.33 },
            new DataSet{ pdbid="2RN2",  RÅ=1.48,  ANM=0.68,  GNM= 0.71,  STeM=0.75 },
            new DataSet{ pdbid="2SIL",  RÅ=1.60,  ANM=0.43,  GNM= 0.50,  STeM=0.51 },
            new DataSet{ pdbid="2TGI",  RÅ=1.80,  ANM=0.69,  GNM= 0.71,  STeM=0.73 },
            new DataSet{ pdbid="3CHY",  RÅ=1.66,  ANM=0.61,  GNM= 0.75,  STeM=0.68 },
            new DataSet{ pdbid="3COX",  RÅ=1.80,  ANM=0.71,  GNM= 0.71,  STeM=0.72 },
            new DataSet{ pdbid="3EBX",  RÅ=1.40,  ANM=0.22,  GNM= 0.58,  STeM=0.40 },
            new DataSet{ pdbid="3GRS",  RÅ=1.54,  ANM=0.44,  GNM= 0.57,  STeM=0.59 },
            new DataSet{ pdbid="3LZM",  RÅ=1.70,  ANM=0.60,  GNM= 0.52,  STeM=0.66 },
            new DataSet{ pdbid="3PTE",  RÅ=1.60,  ANM=0.68,  GNM= 0.83,  STeM=0.77 },
            new DataSet{ pdbid="4FGF",  RÅ=1.60,  ANM=0.41,  GNM= 0.27,  STeM=0.43 },
            new DataSet{ pdbid="4GCR",  RÅ=1.47,  ANM=0.73,  GNM= 0.81,  STeM=0.75 },
            new DataSet{ pdbid="4MT2",  RÅ=2.00,  ANM=0.42,  GNM= 0.37,  STeM=0.46 },
            new DataSet{ pdbid="5P21",  RÅ=1.35,  ANM=0.40,  GNM= 0.51,  STeM=0.45 },
            new DataSet{ pdbid="7RSA",  RÅ=1.26,  ANM=0.42,  GNM= 0.63,  STeM=0.59 },
            new DataSet{ pdbid="8ABP",  RÅ=1.49,  ANM=0.61,  GNM= 0.82,  STeM=0.62 },
            #endregion
        };
        public static void TestTuLiangDataSet()
        {
            System.Console.WriteLine("pdbid : ANM(TuLiang -> mine), STeM(TuLiang -> mine)");
            foreach(STeM.DataSet data in STeM.TuLiangDataSet)
            {
                Pdb pdb = PdbDatabase.GetPdb(data.pdbid);
                int numChain = pdb.atoms.ListChainID().HListCommonT().Count;
                HDebug.Assert(numChain == 1);
                //List<Pdb.Atom> atoms = pdb.atoms.SelectByName("CA");
                List<Pdb.Atom> atoms = pdb.atoms.SelectByAltLoc().SelectByName("CA");
                //List<Pdb.Atom> atoms = pdb.atoms.SelectByDefault().SelectByName("CA");
                List<Vector> coords = atoms.ListCoord();
                Vector bfactors = atoms.ListTempFactor().ToArray();

                HessMatrix hessANM = Hess.GetHessAnm(coords, 12);
                Mode[] modeANM = Hess.GetModesFromHess(hessANM, null);
                modeANM = modeANM.SelectExceptSmallSix();
                Vector bfactorANM = modeANM.GetBFactor().ToArray();
                double corrANM = HBioinfo.BFactor.Corr(bfactors, bfactorANM);

                //Matrix hessGNM = ENM.GnmHessian(coords, 12);
                //Mode[] modeGNM = Hess.GetModes(hessGNM);
                //Vector bfactorGNM = modeGNM.GetBFactor().ToArray();
                //double corrGNM = BFactor.Corr(bfactors, bfactorGNM);

                HessMatrix hessSTeM = STeM.GetHessCa_matlab(coords);
                //Matrix hessSTeM = STeM.GetHessCa(coords);
                Mode[] modeSTeM = Hess.GetModesFromHess(hessSTeM, null);
                modeSTeM = modeSTeM.SelectExceptSmallSix();
                Vector bfactorSTeM = modeSTeM.GetBFactor().ToArray();
                double corrSTeM = HBioinfo.BFactor.Corr(bfactors, bfactorSTeM);

                System.Console.Write(data.pdbid);
                System.Console.Write(" : ANM({0:0.00}) -> {1:0.0000})", data.ANM, corrANM);
                System.Console.Write(" : STeM({0:0.00}) -> {1:0.0000})", data.STeM, corrSTeM);
                System.Console.WriteLine();
            }
            /// Capture of result...
            /// 
            /// pdbid : ANM(TuLiang -> mine), STeM(TuLiang -> mine)
            /// 1AAC : ANM(0.70) -> 0.6918) : STeM(0.76) -> 0.7538)
            /// 1ADS : ANM(0.77) -> 0.7807) : STeM(0.71) -> 0.7142)
            /// 1AHC : ANM(0.79) -> 0.7953) : STeM(0.61) -> 0.6117)
            /// 1AKY : ANM(0.56) -> 0.5675) : STeM(0.60) -> 0.6023)
            /// 1AMM : ANM(0.56) -> 0.5951) : STeM(0.55) -> 0.5304)
            #region ...
            /// 1AMP : ANM(0.62) -> 0.5643) : STeM(0.68) -> 0.6739)
            /// 1ARB : ANM(0.78) -> 0.7526) : STeM(0.83) -> 0.8253)
            /// 1ARS : ANM(0.14) -> 0.1343) : STeM(0.41) -> 0.3646)
            /// 1ARU : ANM(0.70) -> 0.7358) : STeM(0.79) -> 0.7921)
            /// 1BKF : ANM(0.52) -> 0.4695) : STeM(0.50) -> 0.4940)
            /// 1BPI : ANM(0.43) -> 0.4225) : STeM(0.57) -> 0.5588)
            /// 1CDG : ANM(0.65) -> 0.6586) : STeM(0.71) -> 0.7068)
            /// 1CEM : ANM(0.51) -> 0.5232) : STeM(0.76) -> 0.6735)
            /// 1CNR : ANM(0.34) -> 0.3445) : STeM(0.42) -> 0.4190)
            /// 1CNV : ANM(0.69) -> 0.6748) : STeM(0.68) -> 0.6775)
            /// 1CPN : ANM(0.51) -> 0.5607) : STeM(0.56) -> 0.5535)
            /// 1CSH : ANM(0.44) -> 0.4257) : STeM(0.57) -> 0.5755)
            /// 1CTJ : ANM(0.47) -> 0.4889) : STeM(0.62) -> 0.6256)
            /// 1CUS : ANM(0.74) -> 0.7416) : STeM(0.76) -> 0.7992)
            /// 1DAD : ANM(0.28) -> 0.3461) : STeM(0.42) -> 0.4155)
            /// 1DDT : ANM(0.21) -> 0.1899) : STeM(0.49) -> 0.4869)
            /// 1EDE : ANM(0.67) -> 0.7044) : STeM(0.75) -> 0.7439)
            /// 1EZM : ANM(0.56) -> 0.5609) : STeM(0.58) -> 0.5842)
            /// 1FNC : ANM(0.29) -> 0.2663) : STeM(0.61) -> 0.6109)
            /// 1FRD : ANM(0.54) -> 0.5933) : STeM(0.77) -> 0.7579)
            /// 1FUS : ANM(0.40) -> 0.3935) : STeM(0.61) -> 0.6084)
            /// 1FXD : ANM(0.58) -> 0.6291) : STeM(0.70) -> 0.6793)
            /// 1GIA : ANM(0.68) -> 0.6655) : STeM(0.69) -> 0.6856)
            /// 1GKY : ANM(0.36) -> 0.3833) : STeM(0.44) -> 0.4257)
            /// 1GOF : ANM(0.75) -> 0.7614) : STeM(0.78) -> 0.7736)
            /// 1GPR : ANM(0.65) -> 0.5689) : STeM(0.66) -> 0.6534)
            /// 1HFC : ANM(0.63) -> 0.6313) : STeM(0.35) -> 0.7303)
            /// 1IAB : ANM(0.36) -> 0.3262) : STeM(0.53) -> 0.5232)
            /// 1IAG : ANM(0.34) -> 0.3464) : STeM(0.44) -> 0.4344)
            /// 1IFC : ANM(0.61) -> 0.5054) : STeM(0.53) -> 0.5395)
            /// 1IGD : ANM(0.18) -> 0.1874) : STeM(0.27) -> 0.2660)
            /// 1IRO : ANM(0.82) -> 0.7949) : STeM(0.85) -> 0.8461)
            /// 1JBC : ANM(0.72) -> 0.7380) : STeM(0.73) -> 0.7326)
            /// 1KNB : ANM(0.63) -> 0.6615) : STeM(0.54) -> 0.5389)
            /// 1LAM : ANM(0.53) -> 0.5324) : STeM(0.71) -> 0.7102)
            /// 1LCT : ANM(0.52) -> 0.5488) : STeM(0.61) -> 0.6115)
            /// 1LIS : ANM(0.16) -> 0.1674) : STeM(0.30) -> 0.2959)
            /// 1LIT : ANM(0.65) -> 0.5715) : STeM(0.76) -> 0.7575)
            /// 1LST : ANM(0.39) -> 0.3860) : STeM(0.73) -> 0.7283)
            /// 1MJC : ANM(0.67) -> 0.6470) : STeM(0.61) -> 0.6164)
            /// 1MLA : ANM(0.59) -> 0.5686) : STeM(0.54) -> 0.5404)
            /// 1MRJ : ANM(0.66) -> 0.6708) : STeM(0.50) -> 0.4923)
            /// 1NAR : ANM(0.62) -> 0.6257) : STeM(0.74) -> 0.7332)
            /// 1NFP : ANM(0.23) -> 0.2561) : STeM(0.41) -> 0.4053)
            /// 1NIF : ANM(0.42) -> 0.4139) : STeM(0.61) -> 0.6112)
            /// 1NPK : ANM(0.53) -> 0.5654) : STeM(0.64) -> 0.5983)
            /// 1OMP : ANM(0.61) -> 0.5857) : STeM(0.65) -> 0.6499)
            /// 1ONC : ANM(0.55) -> 0.5789) : STeM(0.58) -> 0.5736)
            /// 1OSA : ANM(0.36) -> 0.3596) : STeM(0.55) -> 0.5465)
            /// 1OYC : ANM(0.78) -> 0.7708) : STeM(0.77) -> 0.7757)
            /// 1PBE : ANM(0.53) -> 0.5341) : STeM(0.63) -> 0.6290)
            /// 1PDA : ANM(0.60) -> 0.6240) : STeM(0.58) -> 0.5697)
            /// 1PHB : ANM(0.56) -> 0.5919) : STeM(0.59) -> 0.5866)
            /// 1PHP : ANM(0.59) -> 0.5852) : STeM(0.65) -> 0.6470)
            /// 1PII : ANM(0.19) -> 0.2429) : STeM(0.28) -> 0.2871)
            /// 1PLC : ANM(0.41) -> 0.3411) : STeM(0.42) -> 0.4239)
            /// 1POA : ANM(0.54) -> 0.5944) : STeM(0.42) -> 0.4290)
            /// 1POC : ANM(0.46) -> 0.4341) : STeM(0.39) -> 0.3878)
            /// 1PPN : ANM(0.61) -> 0.5655) : STeM(0.67) -> 0.6757)
            /// 1PTF : ANM(0.47) -> 0.4669) : STeM(0.54) -> 0.5349)
            /// 1PTX : ANM(0.65) -> 0.5949) : STeM(0.62) -> 0.6167)
            /// 1RA9 : ANM(0.48) -> 0.5029) : STeM(0.53) -> 0.5267)
            /// 1RCF : ANM(0.59) -> 0.5629) : STeM(0.58) -> 0.5937)
            /// 1REC : ANM(0.34) -> 0.3352) : STeM(0.49) -> 0.4884)
            /// 1RIE : ANM(0.71) -> 0.7440) : STeM(0.52) -> 0.6806)
            /// 1RIS : ANM(0.25) -> 0.2199) : STeM(0.47) -> 0.4779)
            /// 1RRO : ANM(0.08) -> 0.1192) : STeM(0.36) -> 0.3266)
            /// 1SBP : ANM(0.69) -> 0.6955) : STeM(0.67) -> 0.6668)
            /// 1SMD : ANM(0.50) -> 0.5193) : STeM(0.67) -> 0.6713)
            /// 1SNC : ANM(0.68) -> 0.6860) : STeM(0.72) -> 0.7275)
            /// 1THG : ANM(0.50) -> 0.4982) : STeM(0.50) -> 0.4934)
            /// 1TML : ANM(0.64) -> 0.6266) : STeM(0.58) -> 0.5728)
            /// 1UBI : ANM(0.56) -> 0.5610) : STeM(0.61) -> 0.6235)
            /// 1WHI : ANM(0.12) -> 0.1223) : STeM(0.38) -> 0.3713)
            /// 1XIC : ANM(0.29) -> 0.2942) : STeM(0.47) -> 0.4624)
            /// 2AYH : ANM(0.63) -> 0.6453) : STeM(0.82) -> 0.8157)
            /// 2CBA : ANM(0.67) -> 0.6562) : STeM(0.80) -> 0.8054)
            /// 2CMD : ANM(0.68) -> 0.6630) : STeM(0.62) -> 0.6106)
            /// 2CPL : ANM(0.61) -> 0.6379) : STeM(0.72) -> 0.7131)
            /// 2CTC : ANM(0.63) -> 0.6220) : STeM(0.75) -> 0.7495)
            /// 2CY3 : ANM(0.51) -> 0.5150) : STeM(0.67) -> 0.6614)
            /// 2END : ANM(0.63) -> 0.6307) : STeM(0.68) -> 0.6841)
            /// 2ERL : ANM(0.74) -> 0.7400) : STeM(0.85) -> 0.8445)
            /// 2HFT : ANM(0.63) -> 0.5503) : STeM(0.72) -> 0.7228)
            /// 2IHL : ANM(0.62) -> 0.6632) : STeM(0.72) -> 0.7083)
            /// 2MCM : ANM(0.78) -> 0.7774) : STeM(0.79) -> 0.7886)
            /// 2MHR : ANM(0.65) -> 0.6117) : STeM(0.64) -> 0.6341)
            /// 2MNR : ANM(0.46) -> 0.4762) : STeM(0.47) -> 0.4688)
            /// 2PHY : ANM(0.54) -> 0.5160) : STeM(0.68) -> 0.6831)
            /// 2RAN : ANM(0.43) -> 0.4072) : STeM(0.31) -> 0.3138)
            /// 2RHE : ANM(0.28) -> 0.2074) : STeM(0.33) -> 0.3317)
            /// 2RN2 : ANM(0.68) -> 0.6555) : STeM(0.75) -> 0.7478)
            /// 2SIL : ANM(0.43) -> 0.4203) : STeM(0.51) -> 0.5127)
            /// 2TGI : ANM(0.69) -> 0.6787) : STeM(0.73) -> 0.7391)
            /// 3CHY : ANM(0.61) -> 0.5572) : STeM(0.68) -> 0.6885)
            /// 3COX : ANM(0.71) -> 0.6925) : STeM(0.72) -> 0.7179)
            /// 3EBX : ANM(0.22) -> 0.1913) : STeM(0.40) -> 0.3871)
            /// 3GRS : ANM(0.44) -> 0.4431) : STeM(0.59) -> 0.5910)
            /// 3LZM : ANM(0.60) -> 0.5867) : STeM(0.66) -> 0.6567)
            /// 3PTE : ANM(0.68) -> 0.6788) : STeM(0.77) -> 0.7688)
            /// 4FGF : ANM(0.41) -> 0.3695) : STeM(0.43) -> 0.4166)
            /// 4GCR : ANM(0.73) -> 0.7077) : STeM(0.75) -> 0.7258)
            /// 4MT2 : ANM(0.42) -> 0.3117) : STeM(0.46) -> 0.4547)
            /// 5P21 : ANM(0.40) -> 0.3540) : STeM(0.45) -> 0.4521)
            /// 7RSA : ANM(0.42) -> 0.4663) : STeM(0.59) -> 0.5938)
            /// 8ABP : ANM(0.61) -> 0.6265) : STeM(0.62) -> 0.6182)
            #endregion
        }
    }
}
}
