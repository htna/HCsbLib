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
                    //public Vector       mass   
                    //public Vector[]     coords 
                    //public HessMatrix   hess   
                    //public Vector[]     forc   
                    throw new NotImplementedException();
                    //writer.HWrite(value);
                }
                public HessForcInfo(HBinaryReader reader)
                {
                    throw new NotImplementedException();
                    //reader.HRead(out value);
                }
            }
        }
    }
}
