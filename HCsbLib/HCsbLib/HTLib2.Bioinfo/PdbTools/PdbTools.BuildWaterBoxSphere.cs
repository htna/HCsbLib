using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class PdbTools
    {
        /// http://mc3cb.com/pdf_chemistry/What%20is%20the%20diameter%20of%20a%20water%20molecule.pdf
        /// 
        /// What is the diameter of a water molecule?
        /// 
        /// Chang chemistry solutionsCramster.com/Chang Chemistry 8th Edition Solutions.  
        ///  
        ///  
        /// The diameter is about 0.29 nm.  
        /// 
        /// The atomic diameter can be determined from interpolation of the effective ionic radii
        /// of the isoelectronic ions (from crystal data) of O2- (2.80 Å), OH- (2.74 Å) and H3O+
        /// (2.76 Å).  
        /// 
        /// Coincidentally, this diameter is similar to the length of a hydrogen bond. The water
        /// molecule (bond length 0.96 Å) is smaller than ammonia (bond length 1.01 Å) or methane
        /// (bond length 1.09 Å), with only H2 (bond length 0.74 Å) and HF (bond length 0.92 Å)
        /// being smaller molecules.  
        /// 
        /// Answer from Esteban Broitman, September 7 2008.
        /// 
        /////////////////////////////////////////////////////////////////////////////////////////////////
        /// From my experiment
        /// 
        /// average distance bw water is 2.7564
        /// 1. from minimized waterhuge.xyz
        /// 2. select coordinates of only oxygens
        /// 3. pdist in matlab
        /// 4. select distnacs of smaller than ≤4
        /// 5. show histogram (2 curves)
        /// 6. find bell-shaped curve
        /// 7. select distance of ≤3
        /// 8. show histogram (1 curve, OK, README-waterdist.png)
        /// 9. average distance is 2.7564
        /// 
        /////////////////////////////////////////////////////////////////////////////////////////////////

        public static IEnumerable<List<Pdb.Atom>> BuildWaterSphereExt
            ( Vector center
            , double radius
            , double dist2water = 2.9 //2.8 // 2.7564;
            , bool   randOrient = true
            , string opt = "water in uniform cube"
            )
        {
            Vector min = new double[]
            {
                center[0] - (radius + dist2water),
                center[1] - (radius + dist2water),
                center[2] - (radius + dist2water),
            };
            Vector max = new double[]
            {
                center[0] + (radius + dist2water),
                center[1] + (radius + dist2water),
                center[2] + (radius + dist2water),
            };

            double radius2 = radius * radius;
            bool insphere = false;
            foreach(List<Pdb.Atom> atoms in BuildWaterBoxExt(min, max, dist2water, randOrient, opt))
            {
                List<Pdb.Atom> atomsInSphere = new List<Pdb.Atom>();
                foreach(var atom in atoms)
                {
                    if(atom.name == "OH2 ")
                    {
                        double dx = atom.x - center[0];
                        double dy = atom.y - center[1];
                        double dz = atom.z - center[2];
                        double r2= (dx*dx + dy*dy + dz*dz);
                        insphere = (r2 <= radius2);
                    }
                    if(insphere)
                        atomsInSphere.Add(atom);
                }
                if(atomsInSphere.Count != 0)
                    yield return atomsInSphere;
            }
        }
        public static IEnumerable<List<Pdb.Atom>> BuildWaterBoxExt
            ( Vector min
            , Vector max
            , double dist2water = 2.9 //2.8 // 2.7564;
            , bool   randOrient = true
            , string opt = "water in uniform cube"
            )
        {
            double min_x = Math.Min(min[0], max[0]);
            double min_y = Math.Min(min[1], max[1]);
            double min_z = Math.Min(min[2], max[2]);
            double max_x = Math.Max(min[0], max[0]);
            double max_y = Math.Max(min[1], max[1]);
            double max_z = Math.Max(min[2], max[2]);
            double maxx = max_x - min_x;
            double maxy = max_y - min_y;
            double maxz = max_z - min_z;

            Dictionary<int, List<Pdb.Atom>> atoms = new Dictionary<int, List<Pdb.Atom>>();
            foreach(var file_atom in EnumAtomsInWaterBox(maxx, maxy, maxz, dist2water, randOrient, opt))
            {
                int file = file_atom.Item1;
                var atom = file_atom.Item2;
                double x = atom.x;
                double y = atom.y;
                double z = atom.z;
                Pdb.Atom uatom = Pdb.Atom.FromString(atom.GetUpdatedLine(min_x+x, min_y+y, min_z+z));

                if(atoms.ContainsKey(file) == false)
                {
                    if(atoms.Count == 1)
                    {
                        yield return atoms.First().Value;
                        atoms.Clear();
                    }
                    atoms.Add(file, new List<Pdb.Atom>());
                }

                atoms[file].Add(uatom);
            }

            if(atoms.Count == 1)
            {
                yield return atoms.First().Value;
                atoms.Clear();
            }
        }
        public static IEnumerable<ValueTuple<int, Pdb.Atom>> EnumAtomsInWaterBox
            ( double maxx
            , double maxy
            , double maxz
            , double dist2water = 2.9 //2.8 // 2.7564;
            , bool   randOrient = true
            , string opt = "water in uniform cube"
            )
        {
            System.Random rand = null;
            if(randOrient)
                rand = new System.Random();
            //List<Pdb.Atom> atoms = new List<Pdb.Atom>();
            int file  =1;
            int serial=1;
            int resSeq=1;

            IEnumerable<Vector> enumer = null;
            switch(opt)
            {
                case "water in uniform cube"       : enumer = Geometry.EnumPointsInVolume_UniformCube       (maxx/dist2water, maxy/dist2water, maxz/dist2water); break;
                case "water in uniform tetrahedron": enumer = Geometry.EnumPointsInVolume_UniformTetrahedron(maxx/dist2water, maxy/dist2water, maxz/dist2water); break;
            }

            foreach(Vector pt in enumer)
            {
                resSeq++;
                double x = pt[0]*dist2water;
                double y = pt[1]*dist2water;
                double z = pt[2]*dist2water;
                //var atom = Pdb.Atom.FromData(i+1, "H", "HOH", '1', i+1, pt[0], pt[1], pt[2]);

                /// ATOM   8221  OH2 TIP32 547      11.474  41.299  26.099  1.00  0.00           O
                /// ATOM   8222  H1  TIP32 547      12.474  41.299  26.099  0.00  0.00           H
                /// ATOM   8223  H2  TIP32 547      11.148  42.245  26.099  0.00  0.00           H
                Vector[] pthoh = new Vector[]
                {
                    new double[]{0.000000, 0.000000, 0.000000},
                    new double[]{0.000000, 0.000000, 0.957200},
                    new double[]{0.926627, 0.000000, 0.239987},
                };
                if(randOrient != null)
                {
                    Vector axis = new double[]
                    { (rand.NextDouble()*10)%5
                    , (rand.NextDouble()*10)%5
                    , (rand.NextDouble()*10)%5
                    };
                    Trans3 trans = Trans3.FromRotate(axis.UnitVector(), rand.NextDouble()*Math.PI*4);
                    trans.DoTransform(pthoh);
                }

                if((serial >= 99990) || (resSeq>=9990))
                {
                    file++;
                    serial = 1;
                    resSeq = 1;
                }

                serial++; Pdb.Atom OH2 = Pdb.Atom.FromData(serial, "OH2", "HOH", '1', resSeq+1, x+pthoh[0][0], y+pthoh[0][1], z+pthoh[0][2], element:"O"); yield return new ValueTuple<int, Pdb.Atom>(file, OH2); //atoms.Add(OH2);
                serial++; Pdb.Atom H1  = Pdb.Atom.FromData(serial, "H1" , "HOH", '1', resSeq+1, x+pthoh[1][0], y+pthoh[1][1], z+pthoh[1][2], element:"H"); yield return new ValueTuple<int, Pdb.Atom>(file, H1 ); //atoms.Add(H1 );
                serial++; Pdb.Atom H2  = Pdb.Atom.FromData(serial, "H2" , "HOH", '1', resSeq+1, x+pthoh[2][0], y+pthoh[2][1], z+pthoh[2][2], element:"H"); yield return new ValueTuple<int, Pdb.Atom>(file, H2 ); //atoms.Add(H2 );
            }
        }
	}
}
