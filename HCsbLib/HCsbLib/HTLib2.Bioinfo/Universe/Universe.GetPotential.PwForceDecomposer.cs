using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public class PwForceDecomposer
        {
            readonly Vector[,] forceij;
            public PwForceDecomposer(Vector[,] forceij) { this.forceij = forceij; }
            public static implicit operator PwForceDecomposer(Vector[,] forceij) { return new PwForceDecomposer(forceij); }
            public Vector this[int id0, int id1]
            {
                get
                {
                    HDebug.Assert(forceij != null);
                    if(forceij[id0, id1] == null)
                        forceij[id0, id1] = new double[3];
                    return forceij[id0, id1];
                }
                set
                {
                    HDebug.Assert(forceij != null);
                    forceij[id0, id1] = value;
                }
            }
            public void AddBond(int id0, int id1, Vector[] lcoords, Vector[] lforces)
            {
                Add2(id0, id1, lcoords, lforces);
            }
            public void AddAngle(int id0, int id1, int id2, Vector[] lcoords, Vector[] lforces)
            {
                Add3(id0, id1, id2, lcoords, lforces);
            }
            public void AddDihedral(int id0, int id1, int id2, int id3, Vector[] lcoords, Vector[] lforces)
            {
                Add3(id0, id1, id2, id3, lcoords, lforces);
            }
            public void AddImproper(int id0, int id1, int id2, int id3, Vector[] lcoords, Vector[] lforces)
            {
                Add3(id0, id1, id2, id3, lcoords, lforces);
            }
            public void AddNonbonded(int id0, int id1, Vector[] lcoords, Vector[] lforces)
            {
                Add2(id0, id1, lcoords, lforces);
            }
            public void AddNonbonded14(int id0, int id1, Vector[] lcoords, Vector[] lforces)
            {
                Add2(id0, id1, lcoords, lforces);
            }
            public void AddCustom(Vector[] lcoords, Vector[] lforces)
            {
                if(forceij == null)
                    return;
                HDebug.Assert(false);
            }
            ///////////////////////////
            public void Add2(int id0, int id1, Vector[] lcoords, Vector[] lforces)
            {
                if(forceij == null)
                    return;
                HDebug.Assert(lcoords.Length == 2);
                HDebug.Assert(lforces.Length == 2);
                if((lforces[0].Dist2 == 0) && (lforces[1].Dist2 == 0))
                    return;
                if(HDebug.IsDebuggerAttached)
                {
                    // check net force
                    HDebug.AssertTolerance(0.0001, lforces[0]+lforces[1]);

                    Vector diff = lcoords[1] - lcoords[0];
                    HDebug.AssertTolerance(0.0001, LinAlg.Angle(diff, lforces[0]));
                    HDebug.AssertTolerance(0.0001, LinAlg.Angle(diff, lforces[1]));
                    HDebug.AssertTolerance(0.0001, LinAlg.Angle(lforces[0], lforces[1]));
                }

                this[id0, id1] += lforces[0];
                this[id1, id0] += lforces[1];
            }
            public void Add3(int id0, int id1, int id2, Vector[] lcoords, Vector[] lforces)
            {
                if(forceij == null)
                    return;

                Vector ud01 = (lcoords[1] - lcoords[0]).UnitVector();
                Vector ud12 = (lcoords[2] - lcoords[1]).UnitVector();
                Vector ud20 = (lcoords[0] - lcoords[2]).UnitVector();
                // [ ud01_x,       0, -ud20_x]   [f01]   [lf0x]
                // [-ud01_x,  ud12_x,       0]   [f12]   [lf1x]
                // [      0, -ud12_x,  ud20_x] * [f20] = [lf2x]
                //                                             
                // [ ud01_y,       0, -ud20_y]           [lf0y]
                // [-ud01_y,  ud12_y,       0]           [lf1y]
                // [      0, -ud12_y,  ud20_y]           [lf2y]
                //                                             
                // [ ud01_z,       0, -ud20_z]           [lf0z]
                // [-ud01_z,  ud12_z,       0]           [lf1z]
                // [      0, -ud12_z,  ud20_z]           [lf2z]
                /////////////////////////////////////////////////
                //    A                        *   x   =  b
                MatrixByArr A = new double[9, 3];
                Vector b = new double[9];
                for(int i=0; i<3; i++)
                {
                    A[i+3*0, 0] =  ud01[i]; A[i+3*0, 1] =        0; A[i+3*0, 2] = -ud20[i]; b[i+3*0] = lforces[0][i];
                    A[i+3*1, 0] = -ud01[i]; A[i+3*1, 1] =  ud12[i]; A[i+3*1, 2] =        0; b[i+3*1] = lforces[1][i];
                    A[i+3*2, 0] =        0; A[i+3*2, 1] = -ud12[i]; A[i+3*2, 2] =  ud20[i]; b[i+3*2] = lforces[2][i];
                }
                InfoPack extra = HDebug.IsDebuggerAttached ? new InfoPack() : null;
                Matrix pinvA = NumericSolver.Pinv(A, extra);
                HDebug.Assert(extra.GetValueInt("rank") == 3);
                Vector x = LinAlg.MV(pinvA, b);
                Vector f01 = ud01 * x[0];
                Vector f12 = ud12 * x[1];
                Vector f20 = ud20 * x[2];
                if(HDebug.IsDebuggerAttached)
                {
                    // check net force
                    Vector sumforces = lforces[0];
                    for(int i=1; i<lforces.Length; i++)
                        sumforces += lforces[i];
                    HDebug.AssertTolerance(0.00000001, sumforces);

                    Vector f0 = f01 - f20;
                    Vector f1 = f12 - f01;
                    Vector f2 = -f12 + f20;
                    HDebug.AssertTolerance(0.00000001, f0-lforces[0]);
                    HDebug.AssertTolerance(0.00000001, f1-lforces[1]);
                    HDebug.AssertTolerance(0.00000001, f2-lforces[2]);
                }
                this[id0, id1] +=  f01; this[id1, id2] +=  f12; this[id2, id0] +=  f20;
                this[id1, id0] += -f01; this[id2, id1] += -f12; this[id0, id2] += -f20;
            }
            public void Add3(int id0, int id1, int id2, int id3, Vector[] lcoords, Vector[] lforces)
            {
                if(forceij == null)
                    return;

                Vector ud01 = (lcoords[1] - lcoords[0]).UnitVector();
                Vector ud02 = (lcoords[2] - lcoords[0]).UnitVector();
                Vector ud03 = (lcoords[3] - lcoords[0]).UnitVector();
                Vector ud12 = (lcoords[2] - lcoords[1]).UnitVector();
                Vector ud13 = (lcoords[3] - lcoords[1]).UnitVector();
                Vector ud23 = (lcoords[3] - lcoords[2]).UnitVector();
                //     f01     f02      f03      f12      f13      f23
                // [ ud01_x,  ud02_x,  ud03_x,       0,       0,       0]   [f01]   [lf0x]
                // [-ud01_x,       0,       0,  ud12_x,  ud13_x,       0]   [f02]   [lf1x]
                // [      0, -ud02_x,       0, -ud12_x,       0,  ud23_x] * [f03] = [lf2x]
                // [      0,       0, -ud03_x,       0, -ud13_x, -ud23_x] * [f03] = [lf3x]
                //                                                          [f12]         
                // [ ud01_y,  ud02_y,  ud03_y,       0,       0,       0]   [f13]   [lf0y]
                // [-ud01_y,       0,       0,  ud12_y,  ud13_y,       0]   [f23]   [lf1y]
                // [      0, -ud02_y,       0, -ud12_y,       0,  ud23_y]           [lf2y]
                // [      0,       0, -ud03_y,       0, -ud13_y, -ud23_y]           [lf3y]
                //                                                                        
                // [ ud01_z,  ud02_z,  ud03_z,       0,       0,       0]           [lf0z]
                // [-ud01_z,       0,       0,  ud12_z,  ud13_z,       0]           [lf1z]
                // [      0, -ud02_z,       0, -ud12_z,       0,  ud23_z]           [lf2z]
                // [      0,       0, -ud03_z,       0, -ud13_z, -ud23_z]           [lf3z]
                /////////////////////////////////////////////////
                //    A                                                   *   x   =  b
                MatrixByArr A = new double[12, 6];
                Vector b = new double[12];
                for(int i=0; i<3; i++)
                {
                    A[i+3*0,0] =  ud01[i]; A[i+3*0,1] =  ud02[i]; A[i+3*0,2] =  ud03[i]; A[i+3*0,3] =        0; A[i+3*0,4] =        0; A[i+3*0,5] =        0; b[i+3*0] = lforces[0][i];
                    A[i+3*1,0] = -ud01[i]; A[i+3*1,1] =        0; A[i+3*1,2] =        0; A[i+3*1,3] =  ud12[i]; A[i+3*1,4] =  ud13[i]; A[i+3*1,5] =        0; b[i+3*1] = lforces[1][i];
                    A[i+3*2,0] =        0; A[i+3*2,1] = -ud02[i]; A[i+3*2,2] =        0; A[i+3*2,3] = -ud12[i]; A[i+3*2,4] =        0; A[i+3*2,5] =  ud23[i]; b[i+3*2] = lforces[2][i];
                    A[i+3*3,0] =        0; A[i+3*3,1] =        0; A[i+3*3,2] = -ud03[i]; A[i+3*3,3] =        0; A[i+3*3,4] = -ud13[i]; A[i+3*3,5] = -ud23[i]; b[i+3*3] = lforces[3][i];
                }
                InfoPack extra = HDebug.IsDebuggerAttached ? new InfoPack() : null;
                Matrix   pinvA = NumericSolver.Pinv(A, extra);
                HDebug.Assert(extra.GetValueInt("rank") == 6);
                Vector x = LinAlg.MV(pinvA, b);
                Vector f01 = ud01 * x[0];
                Vector f02 = ud02 * x[1];
                Vector f03 = ud03 * x[2];
                Vector f12 = ud12 * x[3];
                Vector f13 = ud13 * x[4];
                Vector f23 = ud23 * x[5];
                if(HDebug.IsDebuggerAttached)
                {
                    // check net force
                    Vector sumforces = lforces[0];
                    for(int i=1; i<lforces.Length; i++)
                        sumforces += lforces[i];
                    HDebug.AssertTolerance(0.00000001, sumforces);

                    Vector f0 =  f01 + f02 + f03;
                    Vector f1 = -f01 + f12 + f13;
                    Vector f2 = -f02 - f12 + f23;
                    Vector f3 = -f03 - f13 - f23;
                    HDebug.AssertTolerance(0.00000001, f0-lforces[0]);
                    HDebug.AssertTolerance(0.00000001, f1-lforces[1]);
                    HDebug.AssertTolerance(0.00000001, f2-lforces[2]);
                    HDebug.AssertTolerance(0.00000001, f3-lforces[3]);
                }
                this[id0, id1] +=  f01;    this[id0, id2] +=  f02;    this[id0, id3] +=  f03;    this[id1, id2] +=  f12;    this[id1, id3] +=  f13;    this[id2, id3] +=  f23;
                this[id1, id0] += -f01;    this[id2, id0] += -f02;    this[id3, id0] += -f03;    this[id2, id1] += -f12;    this[id3, id1] += -f13;    this[id3, id2] += -f23;
            }
        }
    }
}
