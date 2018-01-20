using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
public partial class Namd
{
    [Serializable]
    public partial class Prm
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		//                                                                                                    //
		// http://                                                                                            //
		//                                                                                                    //
		////////////////////////////////////////////////////////////////////////////////////////////////////////

		public static void SelfTest(string rootpath, string[] args)
		{
			string filepath = rootpath + @"\Sample\par_all27_na.prm";
			Prm prm = Prm.FromFile(filepath, new TextLogger());
		}

		Bond[]      bonds     ;
		Angle[]     angles    ;
		Dihedral[]  dihedrals ;
		Improper[]  impropers ;
		Nonbonded[] nonbondeds;

        Prm()
        {
        }

		////////////////////////////////////////////////////////////////////////////////////
		// Serializable
        public Prm(SerializationInfo info, StreamingContext ctxt)
		{
            bonds      = (Bond[]     )info.GetValue("bonds"      , typeof(Bond[]     ));
            angles     = (Angle[]    )info.GetValue("angles"     , typeof(Angle[]    ));
            dihedrals  = (Dihedral[] )info.GetValue("dihedrals"  , typeof(Dihedral[] ));
            impropers  = (Improper[] )info.GetValue("impropers"  , typeof(Improper[] ));
            nonbondeds = (Nonbonded[])info.GetValue("nonbondeds" , typeof(Nonbonded[]));
        }
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            info.AddValue("bonds"      , bonds     );
            info.AddValue("angles"     , angles    );
            info.AddValue("dihedrals"  , dihedrals );
            info.AddValue("impropers"  , impropers );
            info.AddValue("nonbondeds" , nonbondeds);
        }
    }
}
}