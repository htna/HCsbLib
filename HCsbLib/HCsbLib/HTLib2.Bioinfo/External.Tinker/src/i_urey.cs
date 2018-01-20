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
///     ##############################################################
///     ##                                                          ##
///     ##  urey.i  --  Urey-Bradley interactions in the structure  ##
///     ##                                                          ##
///     ##############################################################
///
///
///     uk      Urey-Bradley force constants (kcal/mole/Ang**2)
///     ul      ideal 1-3 distance values in Angstroms
///     nurey   total number of Urey-Bradley terms in the system
///     iury    numbers of the atoms in each Urey-Bradley interaction
///
///
//      integer nurey,iury
//      real*8 uk,ul
//      common /urey/ uk(maxang),ul(maxang),nurey,iury(3,maxang)

    int nurey;
    int[,] iury = new int[1+3, 1+maxang];
    double[] uk = new double[1+maxang];
    double[] ul = new double[1+maxang];
}
}
}
