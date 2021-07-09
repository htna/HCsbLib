using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class HessForc
    {
        public static partial class Coarse
        {
            public class HessForcInfo : IBinarySerializable
            {
                public object[]     atoms   = null;
                public Vector       mass    = null;
                public Vector[]     coords  = null;
                public HessMatrix   hess    = null;
                public Vector[]     forc    = null;

                public HessForcInfo
                    ( object[]     atoms  = null
                    , Vector       mass   = null
                    , Vector[]     coords = null
                    , HessMatrix   hess   = null
                    , Vector[]     forc   = null
                    )
                {
                    this.atoms  = atoms ;
                    this.mass   = mass  ;
                    this.coords = coords;
                    this.hess   = hess  ;
                    this.forc   = forc  ;
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
                    {
                        writer.Write(mass  );
                        writer.Write(coords);
                        writer.Write(hess  );
                        writer.Write(forc  );
                    }
                }
                public HessForcInfo(HBinaryReader reader)
                {
                    //public object[]     atoms  
                    {
                        int length; reader.Read(out length);
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
                    {
                        reader.Read(out mass  );
                        reader.Read(out coords);
                        reader.Read(out hess  );
                        reader.Read(out forc  );
                    }
                }
            }
        }
    }
}
