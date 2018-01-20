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
///     #############################################################
///     ##                                                         ##
///     ##  angle.i  --  bond angles within the current structure  ##
///     ##                                                         ##
///     #############################################################
///
///
///     ak       harmonic angle force constant (kcal/mole/rad**2)
///     anat     ideal bond angle or phase shift angle (degrees)
///     afld     periodicity for Fourier bond angle term
///     nangle   total number of bond angles in the system
///     iang     numbers of the atoms in each bond angle
///
///
//      integer nangle,iang
//      real*8 ak,anat,afld
//      common /angle/ ak(maxang),anat(maxang),afld(maxang),nangle,
//     &               iang(4,maxang)
    int      nangle;
    int[,]   iang = new int[4+1,maxang+1];
    double[] ak   = new double[maxang+1];
    double[] anat = new double[maxang+1];
    double[] afld = new double[maxang+1];
}
}
}
