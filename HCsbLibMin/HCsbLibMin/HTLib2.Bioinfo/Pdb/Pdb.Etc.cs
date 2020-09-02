using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
    public partial class Pdb
	{
        /// www.science20.com/princerain/blog/smallest_protein
        /// 
        /// Ever wonder what the smallest protein is? Apparently it's TRP-Cage, a protein
        /// with only 20 amino acids derived from the saliva of Gila monsters. 
        /// 
        /// Trp-cage - smallest protein
        /// 
        /// You can find the structure file and images in the PDB database(www.pdb.org)
        /// with PDB ID = 1L2Y. This highly stable mini-protein is important for studies
        /// of protein stability, protein folding, and 3D structure.
        /// 
        /// Even with this small size, it displays secondary structural elements, such as
        /// an alpha helix, found in many proteins.So far there are no known proteins with
        /// less than 20 residues, but we'll see what happens in the future.
        static public string   _smallest_protein_pdbid = "1L2Y";
        static public Vector[] _smallest_protein_cacoords
        {
            get
            {
                return new Vector[]
                {
                    new Vector(-8.60800,  3.13500, -1.61800),
                    new Vector(-4.92300,  4.00200, -2.45200),
                    new Vector(-3.69000,  2.73800,  0.98100),
                    new Vector(-5.85700, -0.44900,  0.61300),
                    new Vector(-4.12200, -1.16700, -2.74300),
                    new Vector(-0.71600, -0.63100, -0.99300),
                    new Vector(-1.64100, -2.93200,  1.96300),
                    new Vector(-3.02400, -5.79100, -0.26900),
                    new Vector( 0.46600, -6.01600, -1.90500),
                    new Vector( 2.06000, -6.61800,  1.59300),
                    new Vector( 2.62600, -2.96700,  2.72300),
                    new Vector( 6.33300, -2.53300,  3.80600),
                    new Vector( 7.04900, -6.17900,  2.70400),
                    new Vector( 6.38900, -5.31500, -1.01500),
                    new Vector( 9.45100, -3.11600, -1.87000),
                    new Vector( 7.28900,  0.08400, -2.05400),
                    new Vector( 6.78200,  3.08800,  0.34500),
                    new Vector( 3.28700,  4.03100,  1.68600),
                    new Vector( 1.18500,  6.54300, -0.35300),
                    new Vector( 0.85200, 10.02700,  1.28500),
                };
            }
        }
    }
}
