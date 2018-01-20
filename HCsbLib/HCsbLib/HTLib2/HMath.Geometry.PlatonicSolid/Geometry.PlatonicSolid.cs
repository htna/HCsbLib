using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class GeometryStatic
    {
        public static Geometry.PlatonicSolid CloneByRadius(this Geometry.PlatonicSolid obj, double radius)
        {
            return Geometry.PlatonicSolid.CloneByRadius(obj, radius);
        }
    }
	public partial class Geometry
	{
        public partial class PlatonicSolid
        {
            public Vector[] verts;

            public static PlatonicSolid CloneByRadius(PlatonicSolid obj, double radius)
            {
                Vector[] verts = obj.verts.HCloneVectors().ToArray();
                double rad = verts[0].Dist;
                for(int i=0; i<verts.Length; i++)
                {
                    HDebug.AssertTolerance(0.00000001, rad-verts[i].Dist);
                    verts[i] = verts[i] * (radius / rad);
                }
                return new PlatonicSolid
                {
                    verts = verts
                };
            }

            public static PlatonicSolid GetTetrahedon(double radius)
            {
                /// Tetrahedon: 4 faces, 6 edges, 4 vertices
                ///             inscribed / circumscribed = 0.33333333333333337 = sqrt(1/24) / sqrt(3/8)
                /// http://en.wikipedia.org/wiki/Tetrahedron

                var obj = new PlatonicSolid
                {
                    verts = new Vector[]
                    {
                        new Vector( 1, 1, 1), new Vector( 1,-1,-1), 
                        new Vector(-1, 1,-1), new Vector(-1,-1, 1), 
                    }
                };
                return obj.CloneByRadius(radius);
            }
            public static PlatonicSolid GetCuve(double radius)
            {
                /// Cuve: 6 faces, 12 edges, 8 vertices
                ///             inscribed / circumscribed = 0.5773502691896258 = 1 / sqrt(3)
                /// http://en.wikipedia.org/wiki/Cube
                var obj = new PlatonicSolid
                {
                    verts = new Vector[]
                    {
                        new Vector( 1, 1, 1), new Vector( 1, 1,-1), new Vector( 1,-1, 1), new Vector( 1,-1,-1), 
                        new Vector(-1, 1, 1), new Vector(-1, 1,-1), new Vector(-1,-1, 1), new Vector(-1,-1,-1), 
                    }
                };
                return obj.CloneByRadius(radius);
            }
            public static PlatonicSolid GetOctahedron(double radius)
            {
                /// Octahedron: 8 faces, 12 edges, 6 vertices
                ///             inscribed / circumscribed = 0.5773502075429352 = 0.4082482 / 0.7071067
                /// http://en.wikipedia.org/wiki/Octahedron
                /// 
                /// Most efficient
                /// * less vertices
                /// * more faces
                var obj = new PlatonicSolid
                {
                    verts = new Vector[]
                    {
                        new Vector( 0, 0, 1), new Vector( 0, 0,-1), 
                        new Vector( 0, 1, 0), new Vector( 0,-1, 0), 
                        new Vector( 1, 0, 0), new Vector(-1, 0, 0), 
                    }
                };
                return obj.CloneByRadius(radius);
            }
            public static PlatonicSolid GetDodecahedron(double radius)
            {
                /// Dodecahedron: 12 faces, 30 edges, 20 vertices
                ///               inscribed / circumscribed = 0.7946544722498597 = 1.113516364 / 1.401258538
                /// http://en.wikipedia.org/wiki/Dodecahedron#Cartesian_coordinates
                ///   (±1, ±1, ±1)
                ///   (0, ±1/φ, ±φ)
                ///   (±1/φ, ±φ, 0)
                ///   (±φ, 0, ±1/φ)
                ///   where φ = (1 + √5) / 2
                double φ = (1.0 + Math.Sqrt(5)) / 2.0;
                var obj = new PlatonicSolid
                {
                    verts = new Vector[]
                    {
                        ///   (±1, ±1, ±1)
                        new Vector( 1, 1, 1), new Vector( 1, 1,-1), new Vector( 1,-1, 1), new Vector( 1,-1,-1), 
                        new Vector(-1, 1, 1), new Vector(-1, 1,-1), new Vector(-1,-1, 1), new Vector(-1,-1,-1), 
                        ///   (0, ±1/φ, ±φ)
                        new Vector( 0, 1/φ, φ), new Vector( 0, 1/φ,-φ), new Vector( 0,-1/φ, φ), new Vector( 0,-1/φ,-φ), 
                        ///   (±1/φ, ±φ, 0)
                        new Vector( 1/φ, φ, 0), new Vector( 1/φ,-φ, 0), new Vector(-1/φ, φ, 0), new Vector(-1/φ,-φ, 0), 
                        ///   (±φ, 0, ±1/φ)
                        new Vector( φ, 0, 1/φ), new Vector( φ, 0,-1/φ), new Vector(-φ, 0, 1/φ), new Vector(-φ, 0,-1/φ), 
                    }
                };
                return obj.CloneByRadius(radius);
            }
            public static PlatonicSolid Icosahedron(double radius)
            {
                /// Icosahedron: 20 faces, 30 edges, 12 vertices
                ///              inscribed / circumscribed = 0.7946544723127723 = 0.7557613141 / 0.9510565163
                /// http://en.wikipedia.org/wiki/Icosahedron#Cartesian_coordinates
                ///   (0, ±1, ±φ)
                ///   (±1, ±φ, 0)
                ///   (±φ, 0, ±1)
                ///   where φ = (1 + √5) / 2
                double φ = (1.0 + Math.Sqrt(5)) / 2.0;
                var obj = new PlatonicSolid
                {
                    verts = new Vector[]
                    {
                        ///   (0, ±1, ±φ)
                        new Vector( 0, 1, φ), new Vector( 0, 1,-φ), new Vector( 0,-1, φ), new Vector( 0,-1,-φ), 
                        ///   (±1, ±φ, 0)
                        new Vector( 1, φ, 0), new Vector( 1,-φ, 0), new Vector(-1, φ, 0), new Vector(-1,-φ, 0), 
                        ///   (±φ, 0, ±1)
                        new Vector( φ, 0, 1), new Vector( φ, 0,-1), new Vector(-φ, 0, 1), new Vector(-φ, 0,-1), 
                    }
                };
                return obj.CloneByRadius(radius);
            }
        }
	}
}
