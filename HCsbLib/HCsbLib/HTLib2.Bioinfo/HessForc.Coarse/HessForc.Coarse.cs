using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    using HessForcInfo = HessForc.HessForcInfo;
    public static partial class HessForcStatic
    {
        public static IEnumerable<object[]  > HEnumAtoms (this IEnumerable<HessForcInfo> hessforcs) { foreach(var hessforc in hessforcs) yield return hessforc.atoms ; }
        public static IEnumerable<Vector    > HEnumMass  (this IEnumerable<HessForcInfo> hessforcs) { foreach(var hessforc in hessforcs) yield return hessforc.mass  ; }
        public static IEnumerable<Vector[]  > HEnumCoords(this IEnumerable<HessForcInfo> hessforcs) { foreach(var hessforc in hessforcs) yield return hessforc.coords; }
        public static IEnumerable<HessMatrix> HEnumHess  (this IEnumerable<HessForcInfo> hessforcs) { foreach(var hessforc in hessforcs) yield return hessforc.hess  ; }
        public static IEnumerable<Vector[]  > HEnumForc  (this IEnumerable<HessForcInfo> hessforcs) { foreach(var hessforc in hessforcs) yield return hessforc.forc  ; }

        public static IEnumerable<object     > HEnumAtomsAt    (this IEnumerable<HessForcInfo> hessforcs, int idx       ) { foreach(var hessforc in hessforcs) yield return hessforc.atoms [idx] ; }
        public static IEnumerable<double     > HEnumMassAt     (this IEnumerable<HessForcInfo> hessforcs, int idx       ) { foreach(var hessforc in hessforcs) yield return hessforc.mass  [idx] ; }
        public static IEnumerable<Vector     > HEnumCoordsAt   (this IEnumerable<HessForcInfo> hessforcs, int idx       ) { foreach(var hessforc in hessforcs) yield return hessforc.coords[idx] ; }
        public static IEnumerable<MatrixByArr> HEnumHessBlockAt(this IEnumerable<HessForcInfo> hessforcs, int bc, int br) { foreach(var hessforc in hessforcs) yield return hessforc.hess.GetBlock(bc, br); }
        public static IEnumerable<Vector     > HEnumForcAt     (this IEnumerable<HessForcInfo> hessforcs, int idx       ) { foreach(var hessforc in hessforcs) yield return hessforc.forc  [idx] ; }
    }
    public partial class HessForc
    {
        public class HessForcInfo : IBinarySerializable
        {
            public const int    SerializeVersion = 2;
            public object[]     atoms   = null;
            public Vector       mass    = null;
            public Vector[]     coords  = null;
            public HessMatrix   hess    = null;
            public Vector[]     forc    = null;
            public double?      enrg    = null;

            public HessForcInfo
                ( object[]     atoms  = null
                , Vector       mass   = null
                , Vector[]     coords = null
                , HessMatrix   hess   = null
                , Vector[]     forc   = null
                , double?      enrg   = null
                )
            {
                this.atoms  = atoms ;
                this.mass   = mass  ;
                this.coords = coords;
                this.hess   = hess  ;
                this.forc   = forc  ;
                this.enrg   = enrg  ;
            }
            public static HessForcInfo From(Hess.HessInfo hessinfo)
            {
                return new HessForcInfo{
                    atoms  = hessinfo.atoms ,
                    mass   = hessinfo.mass  ,
                    coords = hessinfo.coords,
                    hess   = hessinfo.hess  ,
                };
            }
            public static HessForcInfo From(Hess.HessInfo hessinfo, Vector[] forc)
            {
                return new HessForcInfo{
                    atoms  = hessinfo.atoms ,
                    mass   = hessinfo.mass  ,
                    coords = hessinfo.coords,
                    hess   = hessinfo.hess  ,
                    forc   = forc           ,
                };
            }
            public static HessForcInfo From(Hess.HessInfo hessinfo, Vector[] forc, double enrg)
            {
                return new HessForcInfo{
                    atoms  = hessinfo.atoms ,
                    mass   = hessinfo.mass  ,
                    coords = hessinfo.coords,
                    hess   = hessinfo.hess  ,
                    forc   = forc           ,
                    enrg   = enrg           ,
                };
            }
            public Hess.HessInfo GetHessInfo()
            {
                return new Hess.HessInfo
                {
                    mass    = mass,
                    atoms   = atoms,
                    coords  = coords,
                    hess    = hess,
                    numZeroEigval = null,
                };
            }
            ///////////////////////////////////////////////////
            // IBinarySerializable
            public void BinarySerialize(HBinaryWriter writer)
            {
                writer.Write(SerializeVersion);
                //public object[]     atoms  
                {
                    writer.Write(atoms.Length);
                    for(int i=0; i<atoms.Length; i++)
                        WriteAtom(writer, atoms[i]);
                    //////////////////////////////////////////////////////////
                    static void WriteAtom(HBinaryWriter writer, object atom)
                    {
                        switch(atom)
                        {
                            case Tinker.Xyz.Atom xyzatom: writer.Write("Tinker.Xyz.Atom"); xyzatom.BinarySerialize(writer); return;
                            default: throw new Exception();
                        }
                    }
                }
                //public Vector       mass   
                //public Vector[]     coords 
                //public HessMatrix   hess   
                //public Vector[]     forc   
                //public double       enrg   
                {
                    writer.Write(mass  );
                    writer.Write(coords);
                    writer.Write(hess  );
                    writer.Write(forc  );
                    writer.Write(enrg  );
                }
            }
            public HessForcInfo(HBinaryReader reader)
            {
                int ver; //reader.Read(out ver); if(ver != SerializeVersion) throw new FormatException();
                //public object[]     atoms  
                {
                    int ver_length; reader.Read(out ver_length);
                    int     length;
                    if(ver_length < 100)
                    {
                        ver = ver_length;
                        reader.Read(out length);
                    }
                    else
                    {
                        length = ver_length;
                        ver = 0;
                    }

                    atoms = new object[length];
                    for(int i=0; i<atoms.Length; i++)
                        atoms[i] = ReadAtom(reader);
                    //////////////////////////////////////////////////////////
                    static object ReadAtom(HBinaryReader reader)
                    {
                        string type; reader.Read(out type);
                        switch(type)
                        {
                            case "Tinker.Xyz.Atom": return new Tinker.Xyz.Atom(reader);
                            default: throw new Exception();
                        }
                    }
                }
                //public Vector       mass   
                //public Vector[]     coords 
                //public HessMatrix   hess   
                //public Vector[]     forc   
                //public double       enrg   
                {
                    reader.Read(out mass  );
                    reader.Read(out coords);
                    reader.Read(out hess  );
                    reader.Read(out forc  );
        if(ver >= 2) reader.Read(out enrg  ); else enrg = null;
                }
            }
            // IBinarySerializable
            ///////////////////////////////////////////////////

            public bool Equals(HessForcInfo other)
            {
                return EqualsHessForcInfo(this, other);
            }
            public static bool EqualsHessForcInfo(object obj1, object obj2, double threshold=0)
            {
                HessForcInfo info1 = (HessForcInfo)obj1;
                HessForcInfo info2 = (HessForcInfo)obj2;
                if(info1 == null && info2 == null)
                {
                    HDebug.Assert(false);
                    return true;
                }
                if(info1 == null && info2 != null) return false;
                if(info1 != null && info2 == null) return false;
                //public object[]     atoms  
                if(info1.atoms.Length != info2.atoms.Length)
                    return false;
                for(int i=0; i<info1.atoms.Length; i++)
                {
                    Tinker.Xyz.Atom atom_comp = info1.atoms[i] as Tinker.Xyz.Atom;
                    Tinker.Xyz.Atom atom_load = info2.atoms[i] as Tinker.Xyz.Atom;
                    if(atom_comp.line != atom_load.line)
                        return false;
                }
                //public Vector[]     coords 
                if(info1.coords.Length != info2.coords.Length)
                    return false;
                for(int i=0; i<info1.coords.Length; i++)
                {
                    Vector coord_comp = info1.coords[i] as Vector;
                    Vector coord_load = info2.coords[i] as Vector;
                    if((coord_comp,coord_load).HAbsMaxDiffWith() > threshold)
                        return false;
                }
                //public double       enrg   
                {
                    static bool Equal(double? a, double? b, double threshold)
                    {
                        if(a == null                     && b == null                    ) return true;
                        double va = a.Value; double vb = b.Value;
                        if(double.IsNaN             (va) && double.IsNaN             (vb)) return true;
                        if(double.IsPositiveInfinity(va) && double.IsPositiveInfinity(vb)) return true;
                        if(double.IsNegativeInfinity(va) && double.IsNegativeInfinity(vb)) return true;
                        if(Math.Abs(va - vb) <= threshold                                ) return true;
                        return false;
                    }
                    if(Equal(info1.enrg, info2.enrg, threshold) == false)
                        return false;
                }
                //public Vector[]     forc   
                if(info1.forc.Length != info2.forc.Length)
                    return false;
                for(int i=0; i<info1.forc.Length; i++)
                {
                    Vector coord_comp = info1.forc[i] as Vector;
                    Vector coord_load = info2.forc[i] as Vector;
                    if((coord_comp, coord_load).HAbsMaxDiffWith() > threshold)
                        return false;
                }
                //public Vector       mass   
                {
                    if(info1.mass.Size != info2.mass.Size)
                        return false;
                    if((info1.mass, info2.mass).HAbsMaxDiffWith() > threshold)
                        return false;
                }
                //public HessMatrix   hess   
                {
                    bool hess_equal = HessMatrixStatic.HessMatrixEqual(info1.hess, info2.hess, threshold);
                    if(hess_equal == false)
                        return false;
                }
                return true;
            }
        }
    }
}
