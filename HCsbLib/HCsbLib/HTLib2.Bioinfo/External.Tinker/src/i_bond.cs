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
///     ###########################################################
///     ##                                                       ##
///     ##  bond.i  --  covalent bonds in the current structure  ##
///     ##                                                       ##
///     ###########################################################
///
///
///     bk      bond stretch force constants (kcal/mole/Ang**2)
///     bl      ideal bond length values in Angstroms
///     nbond   total number of bond stretches in the system
///     ibnd    numbers of the atoms in each bond stretch
///
///
//      integer nbond,ibnd
//      real*8 bk,bl
//      common /bond/ bk(maxbnd),bl(maxbnd),nbond,ibnd(2,maxbnd)
    int nbond;
    int[,] ibnd = new int[2+1,maxbnd+1];
    double[] bk = new double[maxbnd+1];
    double[] bl = new double[maxbnd+1];
}
}
}
