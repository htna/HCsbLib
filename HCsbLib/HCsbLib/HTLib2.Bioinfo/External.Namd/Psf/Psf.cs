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
    public partial class Psf
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		//                                                                                                    //
		// http://www.ks.uiuc.edu/Training/Tutorials/namd/namd-tutorial-unix-html/node21.html                 //
		//                                                                                                    //
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		public string[] header   ;
		public string[] title    ;
		public Atom[]   atoms    ;
		public int[,]   bonds    ;
		public int[,]   angles   ;
		public int[,]   dihedrals;
		public int[,]   impropers;
		public int[,]   donors   ;
		public int[,]   acceptors;

        Psf()
        {
        }
		public static void SelfTest()
		{
			string filepath = @"C:\Users\htna\htnasvn_htna\VisualStudioSolutions\Library2\HTLib2.Bioinfo\Sample\1a6g_autopsf.psf";
			Psf psf = Psf.FromFile(filepath);
		}
		////////////////////////////////////////////////////////////////////////////////////
		// Serializable
        public Psf(SerializationInfo info, StreamingContext ctxt)
		{
            header    = (string[])info.GetValue("header"   , typeof(string[]));
            title     = (string[])info.GetValue("title"    , typeof(string[]));
            atoms     = (Atom[]  )info.GetValue("atoms"    , typeof(Atom[]  ));
            bonds     = (int[,]  )info.GetValue("bonds"    , typeof(int[,]  ));
            angles    = (int[,]  )info.GetValue("angles"   , typeof(int[,]  ));
            dihedrals = (int[,]  )info.GetValue("dihedrals", typeof(int[,]  ));
            impropers = (int[,]  )info.GetValue("impropers", typeof(int[,]  ));
            donors    = (int[,]  )info.GetValue("donors"   , typeof(int[,]  ));
            acceptors = (int[,]  )info.GetValue("acceptors", typeof(int[,]  ));
        }
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
            info.AddValue("header"   , header   );
            info.AddValue("title"    , title    );
            info.AddValue("atoms"    , atoms    );
            info.AddValue("bonds"    , bonds    );
            info.AddValue("angles"   , angles   );
            info.AddValue("dihedrals", dihedrals);
            info.AddValue("impropers", impropers);
            info.AddValue("donors"   , donors   );
            info.AddValue("acceptors", acceptors);
        }
	}
}
}
