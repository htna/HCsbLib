using System;
using System.Collections.Generic;
using System.Text;

// References
// - http://www.geometryalgorithms.com
// - http://www.gamedev.net/community/forums/topic.asp?topic_id=444154


namespace HTLib2
{
    public partial class Geometry
    {
        public class Plane
        {
            public readonly Vector Base;
            public readonly Vector Normal;
            public Plane(Vector Base, Vector Normal) { this.Base = Base; this.Normal = Normal; HDebug.Assert(Verify()); }
            public bool Verify() { return (Math.Abs(Normal.Dist2-1) < 0.00000001); }
            // utilities
            public double DistanceFrom(Vector pt) { return DistancePointPlane(pt, this); }
            public double AngleFrom(Plane plane)  { return AngleBetween(Normal, plane.Normal); }
        }
        public class Line
        {
            public readonly Vector Base;
            public readonly Vector Normal;
            public Line(Vector Base, Vector Normal) { this.Base = Base; this.Normal = Normal; HDebug.Assert(Verify()); }
            public bool Verify() { return (Math.Abs(Normal.Dist2-1) < 0.00000001); }
            // utilities
            public double AngleFrom(Line line) { return AngleBetween(Normal, line.Normal); }
            public double DistanceFrom(Vector pt) { return DistancePointLine(pt, Base, Base+Normal); }
        }
        public class Segment
        {
            public readonly Vector Base;
            public readonly Vector Direct;
            public Vector PtFrom { get { return (Base); } }
            public Vector PtTo { get { return (Base + Direct); } }
            public Segment(Vector PtFrom, Vector PtTo) { this.Base = PtFrom; this.Direct = PtTo-PtFrom; HDebug.Assert(Verify()); }
            public Line ToLine() { return new Line(Base, Direct.UnitVector()); }
            public bool Verify() { return (Direct != new double[3]{0,0,0}); }
            public Vector this[double index] { get { return (Base + Direct*index); } }
            // utilities
            public double AngleFrom(Segment segment) { return AngleBetween(Direct, segment.Direct); }
            public double IndexClosestFrom(Vector point) { return IndexSegmentClosestPoint(this, point); }
        }
//        public class Circle
//        {
//            public readonly DoubleVector3 Center;
//            public readonly DoubleVector3 Normal;
//            public readonly double Radius;
//            readonly DoubleVector3 U, V; // temporarily
//            public Circle(DoubleVector3 Center, DoubleVector3 Normal, double Radius) { this.Center = Center; this.Normal = Normal; this.Radius = Radius; Pair<DoubleVector3,DoubleVector3> UV = Orthogonals(Normal); U=UV.first; V=UV.second; Debug.Assert(Verify()); }
//            public bool Verify() { return ((Math.Abs(Normal.Length2-1) < 0.00000001) && (Radius>=0)); }
//            public DoubleVector3 this[double angle] { get { return (Center + (Radius*Math.Cos(angle))*U + (Radius*Math.Sin(angle))*V); } }
//            // http://www.gamedev.net/community/forums/topic.asp?topic_id=427311
//        }
//        public class Cylinder
//        {
//            readonly Circle circle;
//            readonly double length; // length of circle.normal
//            public double Radius { get { return circle.Radius; } }
//            public DoubleVector3 Base { get { return circle.Center; } }
//            public DoubleVector3 Direction { get { return (circle.Normal * length); } }
//            public Cylinder(DoubleVector3 Base, DoubleVector3 Direction, double Radius) { circle = new Circle(Base, Direction.UnitVector, Radius); length = Direction.Length; Debug.Assert(Verify()); }
//            public bool Verify() { return (circle.Verify()); }
//            public DoubleVector3 this[double index, double angle] { get { return (circle[angle] + (circle.Normal * length) * index); } }
//        }
//        public class CylinderInf // infinite cylinder
//        {
//            readonly Circle circle;
//            public double Radius { get { return circle.Radius; } }
//            public DoubleVector3 Base { get { return circle.Center; } }
//            public DoubleVector3 Direction { get { return circle.Normal; } }
//            public CylinderInf(DoubleVector3 Base, DoubleVector3 Direction, double Radius) { circle = new Circle(Base, Direction.UnitVector, Radius); Debug.Assert(Verify()); }
//            public bool Verify() { return (circle.Verify()); }
//            public DoubleVector3 this[double index, double angle] { get { return (circle[angle] + (circle.Normal * index)); } }
//        }
    }
}
