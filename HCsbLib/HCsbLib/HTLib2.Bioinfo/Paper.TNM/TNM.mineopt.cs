using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Atom = Universe.Atom;
    using Bond = Universe.Bond;
    using RotableInfo = Universe.RotableInfo;
public static partial class Paper
{
    public partial class TNM
    {
    public partial class TNM_mineopt
    {
        public static void GetMaRaMaraITa(IList<Atom> atoms, Vector[] coords, ref double Ma, ref Vector Ra, ref Vector MaRa, ref MatrixByArr ITa)
        {
            ////////////////////////////////////////////////////////////////////////
            // http://en.wikipedia.org/wiki/Moment_of_inertia_tensor#Definition   //
            //                                                                    //
            // I = [Ixx Ixy Ixz]                                                  //
            //     [Iyx Iyy Iyz]                                                  //
            //     [Izx Izy Izz]                                                  //
            // Ixx = sum(i=1..n}{ mi (yi^2 + zi^2) }                              //
            // Iyy = sum{i=1..n}{ mi (xi^2 + zi^2) }                              //
            // Izz = sum{i=1..n}{ mi (xi^2 + yi^2) }                              //
            // Ixy = Iyx = sum{i=1..n}{ - mi xi yi }                              //
            // Ixz = Izx = sum{i=1..n}{ - mi xi zi }                              //
            // Iyz = Izy = sum{i=1..n}{ - mi yi zi }                              //
            ////////////////////////////////////////////////////////////////////////
            Ma = 0;
            HDebug.Assert(ITa.ColSize == 3, ITa.RowSize == 3); ITa[0, 0] = ITa[0, 1] = ITa[0, 2] = ITa[1, 0] = ITa[1, 1] = ITa[1, 2] = ITa[2, 0] = ITa[2, 1] = ITa[2, 2] = 0;
            HDebug.Assert(  Ra.Size == 3);   Ra[0] =   Ra[1] =   Ra[2] = 0;
            HDebug.Assert(MaRa.Size == 3); MaRa[0] = MaRa[1] = MaRa[2] = 0;
            foreach(Atom atom in atoms)
            {
                double m = atom.Mass;
                double x = coords[atom.ID][0];
                double y = coords[atom.ID][1];
                double z = coords[atom.ID][2];
                double xx = m*(y*y + z*z);
                double yy = m*(x*x + z*z);
                double zz = m*(x*x + y*y);
                double xy = -m*x*y;
                double xz = -m*x*z;
                double yz = -m*y*z;

                Ma  += m;
                  Ra[0] +=   x;   Ra[1] +=   y;   Ra[2] +=   z;
                MaRa[0] += m*x; MaRa[1] += m*y; MaRa[2] += m*z;
                ITa[0, 0] += xx; ITa[0, 1] += xy; ITa[0, 2] += xz;
                ITa[1, 0] += xy; ITa[1, 1] += yy; ITa[1, 2] += yz;
                ITa[2, 0] += xz; ITa[2, 1] += yz; ITa[2, 2] += zz;
            }
        }
        public static MatrixByArr GetSSMatrix(Vector v)
        {
            HDebug.Assert(v.Size == 3);
            MatrixByArr SS = new double[3, 3]{{     0, -v[2],  v[1] },
                                         {  v[2],     0, -v[0] },
                                         { -v[1],  v[0],     0 }};
            return SS;
        }

        public unsafe static MatrixByArr GetJ(Universe univ, Vector[] coords, List<RotableInfo> rotInfos)
        {
            HDebug.Assert(rotInfos.ListMolecule().HUnion().Length == 1);
            Dictionary<Universe.Molecule,Dictionary<string,object>> moleMRI = new Dictionary<Universe.Molecule, Dictionary<string, object>>();

            int n = univ.atoms.Count;
            int m = rotInfos.Count;
            MatrixByArr   J  = new double[3*n, m];
            double[] _Ja = new double[3*n];
            for(int a=0; a<m; a++)
            {
                var mole = rotInfos[a].mole;
                if(moleMRI.ContainsKey(rotInfos[a].mole) == false)
                {
                    double _M  = 0;
                    Vector _R  = new double[3];
                    Vector _MR = new double[3];
                    MatrixByArr _I  = new double[3, 3];
                    GetMaRaMaraITa(mole.atoms.ToArray(), coords, ref _M, ref _R, ref _MR, ref _I);
                    MatrixByArr _invI = LinAlg.Inv3x3(_I);

                    MatrixByArr _MRx = GetSSMatrix(_MR);
                    MatrixByArr _MRxt = _MRx.Tr();
                    MatrixByArr _III = _I - (1.0/_M) * _MRx * _MRxt; // { MI - 1/M * [MR]x * [MR]x^t }
                    MatrixByArr _invIII = LinAlg.Inv3x3(_III);

                    moleMRI.Add(mole, new Dictionary<string, object>());
                    moleMRI[mole].Add("invI"  , _invI  );
                    moleMRI[mole].Add("invIII", _invIII);
                    moleMRI[mole].Add("MR"    , _MR    );
                    moleMRI[mole].Add("R"     , _R     );
                    moleMRI[mole].Add("M"     , _M     );
                }
                MatrixByArr invI   = moleMRI[mole]["invI"  ] as MatrixByArr;
                MatrixByArr invIII = moleMRI[mole]["invIII"] as MatrixByArr;
                Vector MR     = moleMRI[mole]["MR"    ] as Vector;
                Vector R      = moleMRI[mole]["R"     ] as Vector;
                double M      = (double)(moleMRI[mole]["M"     ]);



                RotableInfo rotInfo = rotInfos[a];
                double Ma   = 0;
                Vector Ra   = new double[3];
                Vector MaRa = new double[3];
                MatrixByArr Ia   = new double[3, 3];
                GetMaRaMaraITa(rotInfo.rotAtoms, coords, ref Ma, ref Ra, ref MaRa, ref Ia);

                HDebug.Assert(rotInfo.rotAtoms.Contains(rotInfo.bondedAtom));
                Vector sa = coords[rotInfo.bondedAtom.ID];
                Vector ea;
                {
                    Atom bondatom = null;
                    HDebug.Assert(rotInfo.bond.atoms.Length == 2);
                    if(rotInfo.bond.atoms[0] == rotInfo.bondedAtom) bondatom = rotInfo.bond.atoms[1];
                    else                                            bondatom = rotInfo.bond.atoms[0];
                    HDebug.Assert(rotInfo.rotAtoms.Contains(bondatom) == false);
                    ea = (sa - coords[bondatom.ID]).UnitVector();
                }

                Vector Aa;
                {
                    Vector ea_Rasa = LinAlg.CrossProd3(ea, Ra-sa);
                    Vector IAa = -LinAlg.MV(Ia, ea) + LinAlg.CrossProd3(Ma*R-MaRa, ea_Rasa);
                    Aa = LinAlg.MV(invI, IAa);
                }
                {
                    //Vector ea_MaRa_Masa = Vector.CrossProd3(ea, MaRa-Ma*sa);
                    //Vector ea_sa = Vector.CrossProd3(ea, sa);
                    Vector IAa = new double[3];
                    IAa += -LinAlg.MV(Ia, ea);
                    IAa +=  LinAlg.CrossProd3(MaRa ,LinAlg.CrossProd3(ea,sa));
                    IAa +=  LinAlg.CrossProd3(MR   ,LinAlg.CrossProd3(ea, MaRa-Ma*sa)) / M;
                    Aa = LinAlg.MV(invIII, IAa);
                }
                Vector ta = new double[3];
                {
                    //Aa = new double[3]; // check "translational Eckart condition" only
                    //ta += (-1/M) * Vector.CrossProd3(Aa, MMR);
                    ta += (-1/M) * LinAlg.CrossProd3(Aa, MR);
                    ta += (-1/M) * LinAlg.CrossProd3(ea, MaRa-Ma*sa);
                }

                fixed(double* Ja = _Ja)
                {
                    //for(int i=0; i<n; i++)
                    foreach(Atom iaa in mole.atoms)
                    {
                        int i = iaa.ID;
                        double mi = univ.atoms[i].Mass;
                        Vector ri = coords[univ.atoms[i].ID];
                        Vector Jia = new double[3];
                        //Jia += Vector.CrossProd3(Aa, ri*mi);
                        Jia += LinAlg.CrossProd3(Aa, ri);
                        Jia += ta;
                        //J[i, a] = Jia.ToColMatrix();
                        Ja[i*3+0] = Jia[0];
                        Ja[i*3+2] = Jia[2];
                        Ja[i*3+1] = Jia[1];
                    }
                    foreach(Atom ira in rotInfo.rotAtoms)
                    {
                        int i = ira.ID;
                        Vector ri = coords[univ.atoms[i].ID];
                        Vector Jia = LinAlg.CrossProd3(ea, ri-sa);
                        //J[i, a] += Jia.ToColMatrix();
                        Ja[i*3+0] += Jia[0];
                        Ja[i*3+1] += Jia[1];
                        Ja[i*3+2] += Jia[2];
                    }
                    //for(int i=0; i<n; i++)
                    foreach(Atom iaa in mole.atoms)
                    {
                        int i = iaa.ID;
                        J[i*3+0, a] = Ja[i*3+0];
                        J[i*3+1, a] = Ja[i*3+1];
                        J[i*3+2, a] = Ja[i*3+2];
                    }
                }

                if(HDebug.IsDebuggerAttached)
                {
                    // check Eckart condition
                    // http://en.wikipedia.org/wiki/Eckart_conditions
                    //////////////////////////////////////////////////////////////////////
                    // 1. translational Eckart condition
                    //    sum{i=1..n}{mi * Jia} = 0
                    Vector mJ = new double[3];
                    //for(int i=0; i<n; i++)
                    foreach(Atom iaa in mole.atoms)
                    {
                        int i = iaa.ID;
                        double mi = univ.atoms[i].Mass;
                        //Vector Jia = J[i, a].ToVector();
                        Vector Jia = new double[3] { J[i*3+0, a], J[i*3+1, a], J[i*3+2, a] };
                        mJ += mi * Jia;
                    }
                    HDebug.AssertTolerance(0.00000001, mJ);
                    //////////////////////////////////////////////////////////////////////
                    // 1. rotational Eckart condition
                    //    sum{i=1..n}{mi * r0i x Jia} = 0,
                    //    where 'x' is the cross product
                    Vector mrJ = new double[3];
                    //for(int i=0; i<n; i++)
                    foreach(Atom iaa in mole.atoms)
                    {
                        int i = iaa.ID;
                        double mi = univ.atoms[i].Mass;
                        Vector ri = coords[univ.atoms[i].ID];
                        //Vector Jia = J[i, a].ToVector();
                        Vector Jia = new double[3] { J[i*3+0, a], J[i*3+1, a], J[i*3+2, a] };
                        mrJ += mi * LinAlg.CrossProd3(ri, Jia);
                    }
                    HDebug.AssertTolerance(0.0000001, mrJ);
                }
            }
            return J;
        }
        public static bool CheckEckartConditions(Universe univ, List<RotableInfo> rotInfos, MatrixByArr[,] J)
        {
            int n = univ.atoms.Count;
            int m = rotInfos.Count;
            // 
            for(int a=0; a<m; a++)
            {
                MatrixByArr mJ = new double[3,1];
                for(int i=0; i<n; i++)
                    mJ += univ.atoms[i].Mass * J[i, a];
                HDebug.AssertTolerance(0.00001, mJ);
            }

            return true;
        }
    }
    }
}
}
