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
    public partial class TNM_paper
    {

        public static MatrixByArr GetITa(IList<Atom> atoms, Vector[] coords, Vector Ra)
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
                Vector pt = coords[atom.ID] - Ra;
                double x = pt[0];
                double y = pt[1];
                double z = pt[2];
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
            // the mass-weighted center
            double Ma = 0;
            foreach(Atom atom in atoms)
                Ma += atom.Mass;
            return Ma;
        }
        public static Vector GetRa(IList<Atom> atoms, Vector[] coords, double Ma)
        {
            Vector MaRa = new double[3];
            foreach(Atom atom in atoms)
            {
                double mi = atom.Mass;
                Vector r0i = coords[atom.ID];
                MaRa += (mi * r0i);
            }
            Vector Ra = MaRa / Ma;
            return Ra;
        }
        //public static Vector GetAa(Vector ea, Vector R, Vector Ra, Vector sa, double Ma, Matrix invI, Matrix Ia)
        //{
        //    Vector ea_Rasa = Vector.CrossProd3(ea, Ra-sa);
        //    Vector IAa = -Vector.MV(Ia, ea) + Ma * Vector.CrossProd3(R-Ra, ea_Rasa);
        //    Vector Aa = Vector.MV(invI, IAa);
        //    return Aa;
        //}
        //public static Vector GetTa(Vector Aa, Vector ea, Vector R, double M, double Ma, Vector Ra, Vector sa)
        //{
        //    Vector ta = /*-Vector.CrossProd3(ea, R)*/ - (Ma/M) * Vector.CrossProd3(ea, Ra-sa);
        //    return ta;
        //}
        //public static Matrix GetSSMatrix(Vector v)
        //{
        //    Debug.Assert(v.Size == 3);
        //    Matrix SS = new double[3, 3]{{     0, -v[2],  v[1] },
        //                                 {  v[2],     0, -v[0] },
        //                                 { -v[1],  v[0],     0 }};
        //    return SS;
        //}

        public static MatrixByArr[,] GetJ(Universe univ, Vector[] coords, List<RotableInfo> rotInfos)
        {
            double M = GetMa(univ.atoms.ToArray());
            Vector R = GetRa(univ.atoms.ToArray(), coords, M);
            MatrixByArr I = GetITa(univ.atoms.ToArray(), coords, R);
            MatrixByArr invI = LinAlg.Inv3x3(I);

            int n = univ.atoms.Count;
            int m = rotInfos.Count;
            MatrixByArr[,] J = new MatrixByArr[n, m];
            for(int a=0; a<m; a++)
            {
                RotableInfo rotInfo = rotInfos[a];
                double Ma = GetMa(rotInfo.rotAtoms);
                Vector Ra = GetRa(rotInfo.rotAtoms, coords, Ma);
                MatrixByArr Ia = GetITa(rotInfo.rotAtoms, coords, Ra);

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
                    Vector IAa = -LinAlg.MV(Ia, ea) + Ma * LinAlg.CrossProd3(R-Ra, LinAlg.CrossProd3(ea, Ra-sa));
                    Aa = LinAlg.MV(invI, IAa);
                }
                Vector ta;
                {
                    ta = -LinAlg.CrossProd3(Aa, R) - (Ma/M) * LinAlg.CrossProd3(ea, Ra-sa);
                }

                for(int i=0; i<n; i++)
                {
                    double mi = univ.atoms[i].Mass;
                    Vector ri = coords[univ.atoms[i].ID];
                    Vector Jia  = new double[3];
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
                //    Vector mJ = new double[3];
                //    for(int i=0; i<n; i++)
                //    {
                //        double mi = univ.atoms[i].Mass;
                //        Vector Jia = J[i, a].ToVector();
                //        mJ += mi * Jia;
                //    }
                //    Debug.AssertTolerance(0.00000001, mJ);
                //    //////////////////////////////////////////////////////////////////////
                //    // 1. rotational Eckart condition
                //    //    sum{i=1..n}{mi * r0i x Jia} = 0,
                //    //    where 'x' is the cross product
                //    Vector mrJ = new double[3];
                //    for(int i=0; i<n; i++)
                //    {
                //        double mi = univ.atoms[i].Mass;
                //        Vector ri = coords[univ.atoms[i].ID];
                //        Vector Jia = J[i, a].ToVector();
                //        mrJ += mi * Vector.CrossProd3(ri, Jia);
                //    }
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
