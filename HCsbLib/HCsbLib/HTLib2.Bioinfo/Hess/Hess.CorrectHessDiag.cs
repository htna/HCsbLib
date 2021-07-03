using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
    {
        public static bool CorrectHessDiag_selftest = HDebug.IsDebuggerAttached;
        public static HessMatrix CorrectHessDiag(HessMatrix hessian)
        {
            if(CorrectHessDiag_selftest)
            {
                CorrectHessDiag_selftest = false;
                Vector[] tcoords = new Vector[]
                {
                    new Vector(0, 0, 0),
                    new Vector(1, 2, 3),
                    new Vector(1, 4, 7),
                };
                HessMatrix thess = Hess.GetHessAnm(tcoords);
                HDebug.Assert(Hess.CheckHessDiag(thess, 0.00000001) == true);
                for(int i=0; i<thess.ColSize; i++) thess[i, i] = 0;
                HDebug.Assert(Hess.CheckHessDiag(thess, 0.00000001) == false);
                thess = Hess.CorrectHessDiag(thess);
                HDebug.Assert(Hess.CheckHessDiag(thess, 0.00000001) == true);
            }

            HessMatrix nhess = hessian.CloneHessMatrix();
            HDebug.Assert(nhess.ColSize == nhess.RowSize);
            HDebug.Assert(nhess.ColSize % 3 == 0);
            int size = nhess.ColSize / 3;

            for(int i=0; i<size; i++)
                nhess.SetBlock(i, i, new double[3, 3]);
            foreach(var bc_br_bval in nhess.EnumBlocks())
            {
                int bc   = bc_br_bval.Item1;
                int br   = bc_br_bval.Item2;
                if(bc == br) continue;
                var bval = bc_br_bval.Item3;
                var diag = nhess.GetBlock(bc, bc);
                           nhess.SetBlock(bc, bc, diag - bval);
            }

            return nhess;
        }
    }
}
