namespace HTLib2.Bioinfo
{
public partial class Tinker
{
public partial class Src
{
///
///
///     ###################################################
///     ##  COPYRIGHT (C)  1992  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##
///     ###################################################
///
///     ###############################################################
///     ##                                                           ##
///     ##  deriv.i  --  Cartesian coordinate derivative components  ##
///     ##                                                           ##
///     ###############################################################
///
///
///     desum   total energy Cartesian coordinate derivatives
///     deb     bond stretch Cartesian coordinate derivatives
///     dea     angle bend Cartesian coordinate derivatives
///     deba    stretch-bend Cartesian coordinate derivatives
///     deub    Urey-Bradley Cartesian coordinate derivatives
///     deaa    angle-angle Cartesian coordinate derivatives
///     deopb   out-of-plane bend Cartesian coordinate derivatives
///     deopd   out-of-plane distance Cartesian coordinate derivatives
///     deid    improper dihedral Cartesian coordinate derivatives
///     deit    improper torsion Cartesian coordinate derivatives
///     det     torsional Cartesian coordinate derivatives
///     dept    pi-orbital torsion Cartesian coordinate derivatives
///     debt    stretch-torsion Cartesian coordinate derivatives
///     dett    torsion-torsion Cartesian coordinate derivatives
///     dev     van der Waals Cartesian coordinate derivatives
///     dec     charge-charge Cartesian coordinate derivatives
///     decd    charge-dipole Cartesian coordinate derivatives
///     ded     dipole-dipole Cartesian coordinate derivatives
///     dem     multipole Cartesian coordinate derivatives
///     dep     polarization Cartesian coordinate derivatives
///     der     reaction field Cartesian coordinate derivatives
///     des     solvation Cartesian coordinate derivatives
///     delf    metal ligand field Cartesian coordinate derivatives
///     deg     geometric restraint Cartesian coordinate derivatives
///     dex     extra energy term Cartesian coordinate derivatives
///
///
//      real*8 desum,deb,dea,deba
//      real*8 deub,deaa,deopb,deopd
//      real*8 deid,det,dept,deit
//      real*8 debt,dett,dev,dec
//      real*8 decd,ded,dem,dep,der
//      real*8 des,delf,deg,dex
//      common /deriv/ desum(3,maxatm),deb(3,maxatm),dea(3,maxatm),
//     &               deba(3,maxatm),deub(3,maxatm),deaa(3,maxatm),
//     &               deopb(3,maxatm),deopd(3,maxatm),deid(3,maxatm),
//     &               deit(3,maxatm),det(3,maxatm),dept(3,maxatm),
//     &               debt(3,maxatm),dett(3,maxatm),dev(3,maxatm),
//     &               dec(3,maxatm),decd(3,maxatm),ded(3,maxatm),
//     &               dem(3,maxatm),dep(3,maxatm),der(3,maxatm),
//     &               des(3,maxatm),delf(3,maxatm),deg(3,maxatm),
//     &               dex(3,maxatm)
    double[,] desum = new double[3+1,maxatm+1];
    double[,] deb   = new double[3+1,maxatm+1];
    double[,] dea   = new double[3+1,maxatm+1];
    double[,] deba  = new double[3+1,maxatm+1];
    double[,] deub  = new double[3+1,maxatm+1];
    double[,] deaa  = new double[3+1,maxatm+1];
    double[,] deopb = new double[3+1,maxatm+1];
    double[,] deopd = new double[3+1,maxatm+1];
    double[,] deid  = new double[3+1,maxatm+1];
    double[,] deit  = new double[3+1,maxatm+1];
    double[,] det   = new double[3+1,maxatm+1];
    double[,] dept  = new double[3+1,maxatm+1];
    double[,] debt  = new double[3+1,maxatm+1];
    double[,] dett  = new double[3+1,maxatm+1];
    double[,] dev   = new double[3+1,maxatm+1];
    double[,] dec   = new double[3+1,maxatm+1];
    double[,] decd  = new double[3+1,maxatm+1];
    double[,] ded   = new double[3+1,maxatm+1];
    double[,] dem   = new double[3+1,maxatm+1];
    double[,] dep   = new double[3+1,maxatm+1];
    double[,] der   = new double[3+1,maxatm+1];
    double[,] des   = new double[3+1,maxatm+1];
    double[,] delf  = new double[3+1,maxatm+1];
    double[,] deg   = new double[3+1,maxatm+1];
    double[,] dex   = new double[3+1,maxatm+1];
}
}
}
