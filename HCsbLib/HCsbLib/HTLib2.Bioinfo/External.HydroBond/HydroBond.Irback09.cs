using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using PHydroBond = HydroBond.PHydroBond;
    using Atom = Universe.Atom;
    public static partial class HBioinfoStatic
    {
        public static IEnumerable<PHydroBond> EnumHydroBondIrback09(this Universe univ, IList<Vector> coords)
        {
            foreach(var NHOC in HydroBond.EnumHydroBondNHOC(univ.atoms))
            {
                HDebug.Assert(NHOC.Length == 4);
                int idN = NHOC[0].Item1.ID; var pdbN = NHOC[0].Item2; var Ni = new Tuple<Vector,string,int,string>(coords[idN], pdbN.name, pdbN.resSeq, pdbN.resName);
                int idH = NHOC[1].Item1.ID; var pdbH = NHOC[1].Item2; var Hi = new Tuple<Vector,string,int,string>(coords[idH], pdbH.name, pdbH.resSeq, pdbH.resName);
                int idO = NHOC[2].Item1.ID; var pdbO = NHOC[2].Item2; var Oj = new Tuple<Vector,string,int,string>(coords[idO], pdbO.name, pdbO.resSeq, pdbO.resName);
                int idC = NHOC[3].Item1.ID; var pdbC = NHOC[3].Item2; var Cj = new Tuple<Vector,string,int,string>(coords[idC], pdbC.name, pdbC.resSeq, pdbC.resName);

                PHydroBond hbond = HydroBond.Irback09(Ni, Hi, Oj, Cj);
                if(hbond == null)
                    continue;
                hbond.Ni = NHOC[0].Item1;
                hbond.Hi = NHOC[1].Item1;
                hbond.Oj = NHOC[2].Item1;
                hbond.Cj = NHOC[3].Item1;
                yield return hbond;
            }
        }
    }

    public partial class HydroBond
	{
        /// Anders Irbäck, Simon Mitternacht and Sandipan Mohanty,
        /// An effective all-atom potential for proteins
        /// PMC Biophysics 2009
        /// http://arxiv.org/pdf/0904.1365v1.pdf
        /// 
        ///   Hydrogen bonding. Our potential contains an explicit hydrogen-bond term,
        /// E_hb. All hydrogen bonds in the model are between NH and CO groups. They
        /// connect either two backbone groups or a charged side-chain (aspartic acid, glutamic
        /// acid, lysine, arginine) with a backbone group. Two neighboring peptide
        /// units, which interact through the local potential (see above), are not allowed to
        /// hydrogen bond with each other.
        ///   The form of the hydrogen-bond potential is
        ///
        /// E_hb =   e_hb^(1) \sum_(bb-bb) u(r_ij) v(alpha_ij, beta_ij)
        ///        + e_hb^(2) \sum_(sc_bb) u(r_ij) v(alpha_ij, beta_ij),        (9)
        ///
        /// where e_hb^(1)=3.0 eu and e_hb^(2) =2.3 eu set the strengths of backbone-backbone and
        /// sidechain-backbone bonds, respectively, r_ij is the HO distance, alpha_ij is the NHO
        /// angle, and beta_ij is the HOC angle. The functions u(r) and v(alpha,beta) are given by
        ///
        /// u(r)          = 5*(sigma_hb / r)^12 - 6*(sigma_hb/r)^10             (10)
        /// v(alpha,beta) = | (cos alpha cos beta)^0.5  if alpha,beta > 90 deg  (11)
        ///                 | 0                         otherwise
        /// where sigma_hb=2.0Å. A 4.5 Å cutoff is used for u(r).
        public static PHydroBond Irback09(Tuple<Vector, string, int, string> Ni, Tuple<Vector, string, int, string> Hi
                                         ,Tuple<Vector, string, int, string> Oj, Tuple<Vector, string, int, string> Cj
                                         ) // <coord, atom-name, resi-name>
        {
            // \           /
            //  N-H ... O=C
            // /           \
            //
            // * backbone-backbone (NH-OC)
            // * backbone-sidechain(NH-OC)
            //   N-NH ... ( OD1/OD2(-)-CG---CB-CA    : ASP )
            //   N-NH ... ( OE1/OE2(-)-CD---CG-CB-CA : GLU )
            // *sidechain(NH)-backbone(OC)
            //   ( LYS :                 CA-CB-CG-CD-CE-NZ-HZ1/HZ2/HZ3 ) ... O-C
            //   ( ARG :                             CA-CB-CG-CD-NE-HE ) ... O-C
            //   (       CA-CB-CG-CD-NE-CZ-NH1/NH2-NH11/NH12/NH21/NH22 ) ... O-C
            Vector Ni_Pos = Ni.Item1; string Ni_AtmName = Ni.Item2; int Ni_ResSeq = Ni.Item3; string Ni_ResName = Ni.Item4;
            Vector Hi_Pos = Hi.Item1; string Hi_AtmName = Hi.Item2; int Hi_ResSeq = Hi.Item3; string Hi_ResName = Hi.Item4;
            Vector Oj_Pos = Oj.Item1; string Oj_AtmName = Oj.Item2; int Oj_ResSeq = Oj.Item3; string Oj_ResName = Oj.Item4;
            Vector Cj_Pos = Cj.Item1; string Cj_AtmName = Cj.Item2; int Cj_ResSeq = Cj.Item3; string Cj_ResName = Cj.Item4;

            HDebug.Assert(Ni_ResSeq == Hi_ResSeq); HDebug.Assert(Ni_ResName == Hi_ResName);
            HDebug.Assert(Oj_ResSeq == Cj_ResSeq); HDebug.Assert(Oj_ResName == Cj_ResName);

            if(Math.Abs(Ni_ResSeq-Oj_ResSeq) <= 1)
                // 0: hydrogen bond within a residue
                // 1: two neighboring peptide units
                // are not allowed to hydrogen bond with each other.
                return null;

            double r = (Hi_Pos - Oj_Pos).Dist;
            if(r > 4.5)
                // A 4.5 Å cutoff is used for u(r).
                return null;

            bool BBi = (Ni_AtmName.Trim() == "N") && (Hi_AtmName.Trim() == "HN");
            bool BBj = (Oj_AtmName.Trim() == "O") && (Cj_AtmName.Trim() == "C" );
            bool SCi = (BBi == false) && (Hi_ResName == "LYS" || Hi_ResName == "ARG");
            bool SCj = (BBi == false) && (Oj_ResName == "ASP" || Oj_ResName == "GLU");

            double e_hb = double.NaN;
            if(BBi && BBj)
                // [eu] backbone-backbone
                e_hb = 3.0;
            else if((BBi && SCj) || (SCi && BBj))
                // [eu] sidechain-backbone
                e_hb = 2.3;
            else return null;

            double alpha = Geometry.AngleBetween(Ni_Pos, Hi_Pos, Oj_Pos);
            double beta  = Geometry.AngleBetween(Hi_Pos, Oj_Pos, Cj_Pos);
            if((alpha < Math.PI/2) || (beta < Math.PI/2))
                // v(alpha,beta) = | (cos alpha cos beta)^0.5  if alpha,beta > 90 deg  (11)
                //                 | 0                         otherwise
                return null;
            double v_alpha_beta = Math.Sqrt(Math.Cos(alpha) * Math.Cos(beta));

            double sigma_hb = 2.0;
            double sigmahb_r = sigma_hb / r;
            //     u(r)  = 5*(sigma_hb / r)^12 - 6*(sigma_hb/r)^10             (10)
            double u_r   = 5*        Math.Pow(sigmahb_r, 12) - 6*        Math.Pow(sigmahb_r, 10);
            double du_r  = 5*-12*    Math.Pow(sigmahb_r, 13) - 6*-10*    Math.Pow(sigmahb_r, 11);
            double ddu_r = 5*-12*-13*Math.Pow(sigmahb_r, 14) - 6*-10*-11*Math.Pow(sigmahb_r, 12);

            double energy = e_hb *   u_r * v_alpha_beta;
            double force  = e_hb *  du_r * v_alpha_beta;
            double spring = e_hb * ddu_r * v_alpha_beta;
            //HDebug.Assert(spring >= 0);

            return new PHydroBond
            {
                energy = energy,
                spring = spring,
            };
        }
	}
}
