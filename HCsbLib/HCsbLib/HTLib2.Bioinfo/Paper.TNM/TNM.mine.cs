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
    public partial class TNM_mine
    {

        public static MatrixByArr GetITa(IList<Atom> atoms, Vector[] coords)
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
            MatrixByArr ITa = new double[3,3];
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
                MatrixByArr ITi = new double[3,3] { {xx, xy, xz},
                                               {xy, yy, yz},
                                               {xz, yz, zz} };
                ITa += ITi;
            }
            return ITa;
        }
        public static double GetMa(IList<Atom> atoms)
        {
            double Ma = 0;
            foreach(Atom atom in atoms)
                Ma += atom.Mass;
            return Ma;
        }
        public static Vector GetRa(IList<Atom> atoms, Vector[] coords)
        {
            Vector Ra = new double[3];
            foreach(Atom atom in atoms)
            {
                Vector r0i = coords[atom.ID];
                Ra        += coords[atom.ID];
            }
            return Ra;
        }
        public static Vector GetMaRa(IList<Atom> atoms, Vector[] coords)
        {
            Vector MaRa = new double[3];
            foreach(Atom atom in atoms)
            {
                double mi = atom.Mass;
                Vector r0i = coords[atom.ID];
                MaRa += (mi * r0i);
            }
            return MaRa;
        }
        public static Vector GetMaMaRa(IList<Atom> atoms, Vector[] coords)
        {
            Vector MaMaRa = new double[3];
            foreach(Atom atom in atoms)
            {
                double mi = atom.Mass;
                Vector r0i = coords[atom.ID];
                MaMaRa += (mi * mi * r0i);
            }
            return MaMaRa;
        }
        public static Vector GetAa(Vector ea, Vector R, Vector Ra, Vector sa, double Ma, MatrixByArr invI, MatrixByArr Ia)
        {
            Vector ea_Rasa = LinAlg.CrossProd3(ea, Ra-sa);
            Vector IAa = -LinAlg.MV(Ia, ea) + Ma * LinAlg.CrossProd3(R-Ra, ea_Rasa);
            Vector Aa = LinAlg.MV(invI, IAa);
            return Aa;
        }
        public static Vector GetTa(Vector Aa, Vector ea, Vector R, double M, double Ma, Vector Ra, Vector sa)
        {
            Vector ta = /*-Vector.CrossProd3(ea, R)*/ - (Ma/M) * LinAlg.CrossProd3(ea, Ra-sa);
            return ta;
        }
        public static MatrixByArr GetSSMatrix(Vector v)
        {
            HDebug.Assert(v.Size == 3);
            MatrixByArr SS = new double[3, 3]{{     0, -v[2],  v[1] },
                                         {  v[2],     0, -v[0] },
                                         { -v[1],  v[0],     0 }};
            return SS;
        }

        public static MatrixByArr[,] GetJ(Universe univ, Vector[] coords, List<RotableInfo> rotInfos, Func<MatrixByArr,MatrixByArr> fnInv3x3=null)
        {
            if(fnInv3x3 == null)
                fnInv3x3 = delegate(MatrixByArr A)
                {
                    using(new Matlab.NamedLock("GetJ"))
                    {
                        return LinAlg.Inv3x3(A);
                    }
                };

            MatrixByArr I = GetITa(univ.atoms.ToArray(), coords);
            MatrixByArr invI = fnInv3x3(I);

            Vector R = GetRa(univ.atoms.ToArray(), coords);
            double M = GetMa(univ.atoms.ToArray());
            Vector MR = GetMaRa(univ.atoms.ToArray(), coords);
            MatrixByArr MRx = GetSSMatrix(MR);
            MatrixByArr MRxt = MRx.Tr();
            MatrixByArr III = I - (1.0/M) * MRx * MRxt; // { MI - 1/M * [MR]x * [MR]x^t }
            MatrixByArr invIII = fnInv3x3(III);
            //Vector MMR = GetMaMaRa(univ.atoms.ToArray());

            int n = univ.atoms.Count;
            int m = rotInfos.Count;
            MatrixByArr[,] J = new MatrixByArr[n, m];
            for(int a=0; a<m; a++)
            {
                RotableInfo rotInfo = rotInfos[a];
                Vector Ra = GetRa(rotInfo.rotAtoms, coords);
                double Ma = GetMa(rotInfo.rotAtoms);
                Vector MaRa = GetMaRa(rotInfo.rotAtoms, coords);
                MatrixByArr Ia = GetITa(rotInfo.rotAtoms, coords);

                HDebug.Assert(rotInfo.rotAtoms.Contains(rotInfo.bondedAtom));
                Vector sa = coords[rotInfo.bondedAtom.ID];
                //Vector Sa = sa * rotInfos[a].rotAtoms.Length;
                Vector ea;
                {
                    HashSet<Atom> bondatoms = new HashSet<Atom>(rotInfo.bond.atoms);
                    bondatoms.Remove(rotInfo.bondedAtom);
                    HDebug.Assert(bondatoms.Count == 1);
                    HDebug.Assert(rotInfo.rotAtoms.Contains(bondatoms.First()) == false);
                    ea = (sa - coords[bondatoms.First().ID]).UnitVector();
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

                for(int i=0; i<n; i++)
                {
                    double mi = univ.atoms[i].Mass;
                    Vector ri = coords[univ.atoms[i].ID];
                    Vector Jia = new double[3];
                    //Jia += Vector.CrossProd3(Aa, ri*mi);
                    Jia += LinAlg.CrossProd3(Aa, ri);
                    Jia += ta;
                    J[i, a] = Jia.ToColMatrix();
                }
                foreach(Atom ira in rotInfo.rotAtoms)
                {
                    int i = ira.ID;
                    Vector ri = coords[univ.atoms[i].ID];
                    Vector Jia = LinAlg.CrossProd3(ea, ri-sa);
                    J[i, a] += Jia.ToColMatrix();
                }

                //if(Debug.IsDebuggerAttached)
                //{
                //    // check Eckart condition
                //    // http://en.wikipedia.org/wiki/Eckart_conditions
                //    //////////////////////////////////////////////////////////////////////
                //    // 1. translational Eckart condition
                //    //    sum{i=1..n}{mi * Jia} = 0
                //    Vector mJ  = EckartConditionTrans(univ, J, a);
                //    Debug.AssertTolerance(0.00000001, mJ);
                //    // 2. rotational Eckart condition
                //    //    sum{i=1..n}{mi * r0i x Jia} = 0,
                //    //    where 'x' is the cross product
                //    Vector mrJ = EckartConditionRotat(univ, coords, J, a);
                //    Debug.AssertTolerance(0.00000001, mrJ);
                //}
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
