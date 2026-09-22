using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;
using HTLib2.KDTree;

/*
 * Implementation of Surface Area Algorithm 
 * radius: 1.5 A (subject to change depending on atom type)
 */

namespace HTLib2.Bioinfo
{
    /// https://www.ncbi.nlm.nih.gov/pubmed/3718947
    /// Internal Cavities and Buried Waters in Globular Proteins
    /// A. A. Rashin, M. Iofin, and B. Honig
    /// Rashin AA, Iofin M, Honig B
    /// Biochemistry. 1986 Jun 17;25(12):3619-25.

    /// <Surfaces as Sets of Connected Points.> Two accessible points
    /// on the same sphere are considered as being connected by an
    /// edge if the distance between them is less than 1.5 times the
    /// smallest distance between any two points on the sphere. Two
    /// accessible points are regarded as connected if there exists a
    /// sequence of edges connecting them. A set of accessible points
    /// connected to one another constitutes a connected surface. For
    /// example, all the points on the surface of an isolated sphere
    /// are connected. However, only accessible points form surfaces.
    /// Therefore, if some of the points turn out to be buried due to
    /// overlaps with neighboring atoms, these points and the edges
    /// connected to them do not contribute to the connectivity. As
    /// a result, in some cases there may be accessible points on a
    /// single sphere with no sequence of edges connecting them. Thus
    /// there will be two or more connected surfaces on the sphere
    /// that are not connected to one another.
    /// 
    /// <Connectivity Algorithm.> In order to determine the number
    /// of connected surfaces on each sphere and to assign accessible
    /// points to each of these surfaces, the following algorithm is used.
    /// An accessible point is chosen and used to initiate a list. All
    /// accessible points on the sphere that are connected to it by an
    /// edge are then added to the list. This procedure is repeated
    /// for each of these newly added points. (Only points not yet
    /// in the list are added.) A description of a connected surface
    /// is complete when no new accessible points connected by an
    /// edge to accessible points already in the list can be found.
    /// Additional connected surfaces are defined by repeating the
    /// procedure, beginning with another point on the sphere not
    /// included in the previous lists. The process is completed when
    /// all accessible points are included. During the process every
    /// accessible point is assigned the index of the surface to which
    /// it belongs. This routine is applied to the surface of each
    /// accessible atom.
    /// 
    /// Now, two points belonging to accessible surfaces of two
    /// different atoms can be at a distance of less than the connectivity
    /// distance described above, and therefore these points are
    /// connected by an edge. If there exists one such edge, all the
    /// points of the two surfaces are connected and form a larger
    /// connected surface. Thus the problem of connecting two
    /// surfaces is reduced to the search for one connectivity distance
    /// (edge) between two points belonging to different surfaces. To
    /// accelerate the search, neighbor lists for each atom are created
    /// in the course of accessibility calculations. If each connected
    /// surface on an individual atom is considered as a generalized
    /// "point" and connectivity between neighboring surfaces as an
    /// "edge", then the algorithm for the connectivity of different
    /// atomic surfaces is essentially identical with that for the
    /// connectivity of the points on the surface of a single atom. The
    /// largest connected surface obtained is the outer surface of the
    /// protein, and all other connected surfaces belong to cavities.

    public partial class AccessSurfArea
    {
        public class SurfacePoint
        {
            public Vector coord;
            public int    atmidx;
        }
        public class PointsSurface
        {
            public AccessSurfArea              asa;    // parent object
            internal List<SurfacePoint> surfpoints; // list (surface point, atom-index)

            public IEnumerable<SurfacePoint> EnumSurfacePointAtomIndex()
            {
                foreach(var pt in surfpoints)
                    yield return pt;
            }
            public IEnumerable<Vector> EnumSurfacePoints()
            {
                foreach(var pt in surfpoints)
                    yield return pt.coord;
            }

            public double CalcSurfaceArea()
            {
                return CalcSurfaceAreaOfAtoms().Sum();
            }
            public double[] CalcSurfaceAreaOfAtoms()
            {
                return CalcSurfaceAreaOfAtoms(asa.atoms, asa.radius);
            }
            public double[] CalcSurfaceAreaOfAtoms(Vector[] atoms, double[] radius)
            {
                double[] atom_surfarea = new double[atoms.Length];
                double single_pt_area = Math.PI * 4 / 500;
                for(int i = 0; i < surfpoints.Count; i++)
                {
                    int    atom_idx = surfpoints[i].atmidx;
                    double atom_rad = radius[atom_idx];
                    double area_i = single_pt_area * atom_rad * atom_rad;
                    atom_surfarea[atom_idx] += area_i;
                }
                return atom_surfarea;
            }
        }

        public Vector[] atoms;
        public double[] radius;
        public List<PointsSurface> surfaces;

        public static AccessSurfArea BuildAccessSurfArea(IList<Vector> atom_coords, IList<double> atom_radius)
        {
            Vector[] atoms  = atom_coords.HCloneVectors();
            double[] radius = atom_radius.ToArray().HClone();

            (PointsSurface surfacepoints, double smallestdist) = GetSurfacePoints(atoms, radius);
            //WriteToFile(@"K:\temp\test.pdb", new PointsSurface[] { surfacepoints });

            List<PointsSurface> lstSurfacePoints = GetSurfacePointsConnected(surfacepoints, smallestdist, atoms, radius);
            //for(int i=0; i<lstSurfacePoints.Count; i++)
            //    WriteToFile(@"K:\temp\test-asa-"+i+".pdb", new PointsSurface[] { lstSurfacePoints[i] });

            var asa = new AccessSurfArea
            {
                atoms   = atoms,
                radius   = radius,
                surfaces = lstSurfacePoints,
            };
            foreach(var surface in asa.surfaces)
                surface.asa = asa;

            //asa.WriteToFile(@"D:\temp\asa2.pdb");
            
            return asa;
        }

        public void WriteToFile(string pdbpath)
        {
            WriteToFile(pdbpath, surfaces);
        }
        public static void WriteToFile(string pdbpath, IList<PointsSurface> surfaces)
        {
            List<Pdb.Atom> atom_data = new List<Pdb.Atom>();
            int unicode = 'A';
            foreach (var cavity in surfaces)
            {
                foreach(var pt in cavity.surfpoints)
                {
                    var atom = Pdb.Atom.FromData(1, "H", "HOH", (char)unicode, 1, pt.coord[0], pt.coord[1], pt.coord[2]);
                    atom_data.Add(atom);
                }
                unicode++;
            }
            Pdb result_file = Pdb.FromAtoms(atom_data);
            result_file.ToFile(pdbpath);
        }

        // receive surface points and its atom index, atoms, and radius 
        // return list of cavities
        static List<PointsSurface> GetSurfacePointsConnected(PointsSurface surfacepoints, double smallestdist, Vector[] atoms, double[] radius)
        {
            HGraph<SurfacePoint>.Node[] nodes = new HGraph<SurfacePoint>.Node[surfacepoints.surfpoints.Count];
            {
                for(int i=0; i<nodes.Length; i++)
                {
                    nodes[i] = new HGraph<SurfacePoint>.Node();
                    nodes[i].id        = i;
                    nodes[i].value     = surfacepoints.surfpoints[i];
                    nodes[i].neighbors = new HashSet<HGraph<SurfacePoint>.Node>();
                }
            }

            KDTree<HGraph<SurfacePoint>.Node> kdtree_isurfpoints = new KDTree<HGraph<SurfacePoint>.Node>(3);
            for(int i=0; i<nodes.Length; i++)
                kdtree_isurfpoints.insert(nodes[i].value.coord, nodes[i]);

            {
                double smallestdist_15 = smallestdist * 1.5;
                double smallestdist2_15 = smallestdist_15 * smallestdist_15;
                double[] lowk = new double[3];
                double[] uppk = new double[3];
                for(int i=0; i<nodes.Length; i++)
                {
                    SurfacePoint pt = nodes[i].value;
                    lowk[0] = pt.coord[0] - smallestdist_15;    uppk[0] = pt.coord[0] + smallestdist_15;
                    lowk[1] = pt.coord[1] - smallestdist_15;    uppk[1] = pt.coord[1] + smallestdist_15;
                    lowk[2] = pt.coord[2] - smallestdist_15;    uppk[2] = pt.coord[2] + smallestdist_15;
                    foreach(var rng in kdtree_isurfpoints.range(lowk, uppk))
                    {
                        SurfacePoint rngpt = rng.value;
                        if((pt.coord, rngpt.coord).Dist2() < smallestdist2_15)
                        {
                            nodes[i].neighbors.Add(rng);
                        }
                    }
                }
            }

            List<PointsSurface> surfaces = new List<PointsSurface>();
            {
                foreach(var group in HGraph.EnumConnecteds(nodes))
                {
                    // PointsSurface surface;
                    // surface.surfpoints
                    List<SurfacePoint> surfpoints = new List<SurfacePoint>();
                    foreach(HGraph<SurfacePoint>.Node node in group)
                        surfpoints.Add(node.value);

                    PointsSurface surface = new PointsSurface
                    {
                        surfpoints = surfpoints,
                    };
                    surfaces.Add(surface);
                }
            }

            return surfaces;
            //  Graph<Tuple<Vector, int>, int> surface = new Graph<Tuple<Vector, int>, int>();
            //  KDTree<(Vector coord, int atom)> allNode = new KDTree<Tuple<Vector, int>>(3);
            //  
            //  for(int i=0; i<surfacepoints.surfpoints.Count; i++)
            //  {
            //      allNode.insert(surfacepoints.surfpoints[i].Item1, surfacepoints.surfpoints[i]);
            //      surface.AddNode(surfacepoints.surfpoints[i]);
            //  }
            //  
            //  int index = 0;
            //  double mindist = GetMinDistEdge();
            //  for (int i=0; i<surfacepoints.surfpoints.Count; i++)
            //  {
            //      var pt = surfacepoints.surfpoints[i];
            //      Vector coord = pt.Item1;
            //  
            //      double[] lowk = new double[] { coord[0] - mindist, coord[1] - mindist, coord[2] - mindist };
            //      double[] uppk = new double[] { coord[0] + mindist, coord[1] + mindist, coord[2] + mindist };
            //  
            //      var nears = allNode.range(lowk, uppk);
            //      foreach (var near in nears)
            //      {
            //          double dist = (coord - near.Item1).Dist;
            //          // needs unique edge value, temporarily set it as index with increment
            //          if (surface.FindEdge(surface.GetNode(pt), surface.GetNode(near)) == null && dist < mindist)
            //          {
            //              surface.AddEdge(pt, near, index);
            //              index++;
            //          }
            //      }
            //  }
            //  
            //  List<List<Tuple<Vector, int>>> lst_surfacepoints;
            //  {
            //      lst_surfacepoints = surface.FindConnectedNodeValues();
            //  
            //      // sort the list in decreasing order of the entity size
            //      // the first one will be the surface points of the protein,
            //      // and others will be the cavity points
            //      int[] length_lst = lst_surfacepoints.HListCount();
            //      int[] idxsrt_rev = length_lst.HIdxSorted()
            //                                   .HReverse();
            //      lst_surfacepoints = lst_surfacepoints.HSelectByIndex(idxsrt_rev).ToList();
            //  }
            //  
            //  List<PointsSurface> lstSurfPoints = new List<PointsSurface>();
            //  foreach(var surfpoints in lst_surfacepoints)
            //      lstSurfPoints.Add(new PointsSurface
            //      {
            //          surfpoints = surfpoints,
            //      });
            //  
            //  return lstSurfPoints;
            //  
            //  // receive atoms and corresponding radius
            //  // find minimum distance edge of the points on the atom with max radius of all
            //  double GetMinDistEdge()
            //  {
            //      double max_rad = radius.Max();
            //  
            //      KDTree<object> tree = new KDTree<object>(3);
            //      Vector[] pts = Geometry.EqSpherePoints.GetEqSpherePoints(500);
            //      for (int i = 0; i < pts.Length; i++)
            //      {
            //          pts[i][0] *= max_rad;
            //          pts[i][1] *= max_rad;
            //          pts[i][2] *= max_rad;
            //          tree.insert(pts[i], i);
            //      }
            //  
            //      double min_dist = Double.MaxValue;
            //      for(int i=0; i<pts.Length; i++)
            //      {
            //          var nearby = tree.nearest(pts[i],2);
            //          var dist = (pts[i] - pts[(int)nearby[1]]).Dist;
            //          min_dist = dist < min_dist ? dist : min_dist;
            //      }
            //  
            //      return min_dist * 2;
            //  }
        }

        // receive Tuple of points on surface(Vector) and its atom index(int), and radius corresponds to atom index
        // calculate surface area and return area with corresponding surface points
        public List<Tuple<PointsSurface, double>> CalcSurfaceArea()
        {
            List<Tuple<PointsSurface, double>> surface_surfarea = new List<Tuple<PointsSurface, double>>();

            foreach (var surface in surfaces)
            {
                double[] atom_surfarea = surface.CalcSurfaceAreaOfAtoms(atoms, radius);
                surface_surfarea.Add(new Tuple<PointsSurface, double>(surface, atom_surfarea.Sum()));
            }
            return surface_surfarea;
        }
        public double[] CalcSurfaceAreaOfAtoms()
        {
            return surfaces[0].CalcSurfaceAreaOfAtoms(atoms, radius);
        }
        public double[] CalcSurfaceAreaOfAtoms(Vector[] atoms, double[] radius, double probe)
        {
            /// determine surface area of atom,
            /// when the surface points are determined using atom_radius+probe
            ///
            if(this.atoms.Length != atoms.Length)
                throw new ArgumentException();
            for(int ia=0; ia<atoms.Length; ia++)
            {
                if((this.atoms[ia] - atoms[ia]).Dist2 != 0)
                    throw new ArgumentException();
                if(this.radius[ia] != (radius[ia]+probe))
                    throw new ArgumentException();
            }
            return surfaces[0].CalcSurfaceAreaOfAtoms(atoms, radius);
        }

        // use the interface as Tuple<Vector,int>[] getSurfacePoints(IList<Vector> atoms, IList<double> radius, int start, int end)
        static (PointsSurface surfpoints, double smallestdist) GetSurfacePoints(Vector[] atoms, double[] radius)
        {
            (List<SurfacePoint> surfpoints, double smallestdist) = GetSurfacePoints(atoms, radius, 0, atoms.Length-1);

            var nsurfpoints = new PointsSurface
            {
                surfpoints = surfpoints,
            };

            return (nsurfpoints, smallestdist);
        }
        static Tuple<List<SurfacePoint>, double> GetSurfacePoints(Vector[] atoms, double[] radius, int start, int end)
        {
            KDTree<object> kdtree_atoms = new KDTree<object>(3);
            for(int ia=0; ia<atoms.Length; ia++)
                kdtree_atoms.insert(atoms[ia], ia);

            Vector[] pts500 = Geometry.EqSpherePoints.GetEqSpherePoints(500);

            double smallestdist;
            {
                smallestdist = (pts500[0], pts500[1]).Dist2();
                for(int i=0; i<pts500.Length; i++)
                    for(int j=i+1; j<pts500.Length; j++)
                    {
                        double dist2 = (pts500[i], pts500[j]).Dist2();
                        if(dist2 < smallestdist)
                            smallestdist = dist2;
                    }
                smallestdist = Math.Sqrt(smallestdist);
                smallestdist *= radius.Max();
            }

            double max_radius = radius.Max();
            Vector lowk = new double[3];
            Vector uppk = new double[3];

            List<SurfacePoint> surfpoints = new List<SurfacePoint>();
            for(int ia=0; ia<atoms.Length; ia++)
            {
                Vector ia_coord = atoms[ia];
                double ia_rad   = radius[ia];
                double ia_rad2  = ia_rad * ia_rad;
                Vector surfpt = new double[3];
                foreach(Vector pt in pts500)
                {
                    surfpt[0] = ia_coord[0] + ia_rad * pt[0];
                    surfpt[1] = ia_coord[1] + ia_rad * pt[1];
                    surfpt[2] = ia_coord[2] + ia_rad * pt[2];
                    
                    lowk[0] = surfpt[0] - max_radius;   uppk[0] = surfpt[0] + max_radius;
                    lowk[1] = surfpt[1] - max_radius;   uppk[1] = surfpt[1] + max_radius;
                    lowk[2] = surfpt[2] - max_radius;   uppk[2] = surfpt[2] + max_radius;

                    var range = kdtree_atoms.range(lowk, uppk);
                    bool bsurfpt = true;
                    foreach(int near in range)
                    {
                        Vector near_coord = atoms[near];
                        double near_rad   = radius[near];
                        double dist_near_surfpt = (near_coord, surfpt).Dist();
                        if(dist_near_surfpt < near_rad)
                        {
                            bsurfpt = false;
                            break;
                        }
                    }
                    if(bsurfpt)
                    {
                        var surfpoint = new SurfacePoint
                        {
                            coord  = surfpt.Clone(),
                            atmidx = ia,
                        };
                        surfpoints.Add(surfpoint);
                    }

                    //  int nearest = (int)(kdtree_atoms.nearest(surfpt));
                    //  if(nearest != ia)
                    //  {
                    //      double nearest_surfpt_dist = (surfpt, atoms[nearest]).Dist();
                    //      double nearest_rad         = radius[nearest];
                    //      if(nearest_surfpt_dist < nearest_rad)
                    //          continue;
                    //  }
                    //  else
                    //  {
                    //      HDebug.Assert(Math.Abs(ia_rad - (surfpt, atoms[nearest]).Dist()) < 0.00000001);
                    //  }
                    //  var surfpoint = new SurfacePoint
                    //  {
                    //      coord  = surfpt.Clone(),
                    //      atmidx = ia,
                    //  };
                    //  surfpoints.Add(surfpoint);
                }
            }

            if(HDebug.IsDebuggerAttached)
            {
                KDTree.KDTree<SurfacePoint> kdtree_surfpt = new KDTree<SurfacePoint>(3);
                foreach(var surfpt in surfpoints)
                    kdtree_surfpt.insert(surfpt.coord, surfpt);
                //foreach(var surfpoint in surfpoints)
                for(int ia=0; ia<atoms.Length; ia++)
                {
                    SurfacePoint nearest = kdtree_surfpt.nearest(atoms[ia]);
                    double dist_ia_nearest = (atoms[ia], nearest.coord).Dist();
                    HDebug.Assert(dist_ia_nearest + 0.00000001 >= radius[ia]);
                }
            }
            if(HDebug.False)
            {
                List<Pdb.Atom> atom_data = new List<Pdb.Atom>();
                foreach(var pt in surfpoints)
                {
                    var atom = Pdb.Atom.FromData(1, "H", "HOH", 'A', 1, pt.coord[0], pt.coord[1], pt.coord[2]);
                    atom_data.Add(atom);
                }
                Pdb result_file = Pdb.FromAtoms(atom_data);
                string pdbpath = @"D:\temp\surface2.pdb";
                result_file.ToFile(pdbpath);
            }

            //throw new Exception();
            return new Tuple<List<SurfacePoint>, double>(surfpoints, smallestdist);
        }
        //  static List<Tuple<Vector,int>> GetSurfacePoints(Vector[] atoms, double[] radius, int start, int end)
        //  {
        //      if (start == end)
        //      {
        //          var pts = Geometry.EqSpherePoints.GetEqSpherePoints(500);
        //          var  coord = atoms [start];
        //          double rad = radius[start];
        //          Tuple<Vector,int>[] ret = new Tuple<Vector,int>[pts.Length];
        //          for(int i = 0; i < pts.Length; i++)
        //          {
        //              pts[i][0] = rad * pts[i][0] + coord[0];
        //              pts[i][1] = rad * pts[i][1] + coord[1];
        //              pts[i][2] = rad * pts[i][2] + coord[2];
        //              ret[i] = new Tuple<Vector,int>(pts[i], start);
        //          }
        //          
        //          return ret.ToList();
        //      }
        //  
        //      int mid = (start + end) / 2;
        //      List<Tuple<Vector,int>> first  = GetSurfacePoints(atoms, radius, start  , mid);
        //      List<Tuple<Vector,int>> second = GetSurfacePoints(atoms, radius, mid + 1, end);
        //  
        //      List<Tuple<Vector,int>> ret_pts = new List<Tuple<Vector,int>>();
        //      foreach (var pt in findOverlap(first, mid+1, end))
        //          ret_pts.Add(pt);
        //      
        //      foreach(var pt in findOverlap(second, start, mid))
        //          ret_pts.Add(pt);
        //  
        //      return ret_pts;
        //  
        //      /// receives a IList<Vector> of points of node1 and starting/ending index of node2
        //      /// find overlapping points of node1 from node2, return new Hashset<Vector> of points of node1
        //      HashSet<Tuple<Vector,int>> findOverlap(IList<Tuple<Vector,int>> node1_pts, int lstart, int lend)
        //      {
        //          KDTree <Tuple<Vector,int>> neighbor = new KDTree <Tuple<Vector,int>>(3);
        //          HashSet<Tuple<Vector,int>> new_pts  = new HashSet<Tuple<Vector,int>>();
        //          foreach (var pt in node1_pts)
        //          {
        //              neighbor.insert(pt.Item1, pt);
        //              new_pts.Add(pt);
        //          }
        //  
        //          List<Tuple<Vector,int>> to_delete = new List<Tuple<Vector,int>>();
        //          for(int i=lstart; i<=lend; i++)
        //          {
        //              var coord = atoms [i];
        //              var rad   = radius[i];
        //  
        //              double[] lowk = new double[] { coord[0] - rad, coord[1] - rad, coord[2] - rad };
        //              double[] uppk = new double[] { coord[0] + rad, coord[1] + rad, coord[2] + rad };
        //  
        //              foreach(var overlap in neighbor.range(lowk, uppk))
        //              {
        //                  if((overlap.Item1 - coord).Dist < rad)
        //                      to_delete.Add(overlap);
        //              }
        //          }
        //  
        //          foreach(var pt in to_delete)
        //              new_pts.Remove(pt);
        //  
        //          return new_pts;
        //      }
        //  
        //      // Algorithm without KD-Tree
        //      // foreach(var pt in first.pts)
        //      // {
        //      //     bool overlap = false;
        //      //     foreach(var atom in second.atoms)
        //      //     {
        //      //         if ((atom.coord - pt).Dist <= second.radius)
        //      //         {
        //      //             overlap = true;
        //      //             break;
        //      //         }
        //      //     }
        //      //     if(!overlap)
        //      //         ret_pts.Add(pt);
        //      // }
        //      // 
        //      // foreach (var pt in second.pts)
        //      // {
        //      //     bool overlap = false;
        //      //     foreach(var atom in first.atoms)
        //      //     {
        //      //         if ((atom.coord - pt).Dist <= first.radius)
        //      //         {
        //      //             overlap = true;
        //      //             break;
        //      //         }
        //      //     }
        //      //     if(!overlap)
        //      //         ret_pts.Add(pt);
        //      // }            
        //  }
    }
}
