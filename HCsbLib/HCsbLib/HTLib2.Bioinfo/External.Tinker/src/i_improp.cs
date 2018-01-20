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
///     #################################################################
///     ##                                                             ##
///     ##  improp.i  --  improper dihedrals in the current structure  ##
///     ##                                                             ##
///     #################################################################
///
///
///     kprop    force constant values for improper dihedral angles
///     vprop    ideal improper dihedral angle value in degrees
///     niprop   total number of improper dihedral angles in the system
///     iiprop   numbers of the atoms in each improper dihedral angle
///
///
//      integer niprop,iiprop
//      real*8 kprop,vprop
//      common /improp/ kprop(maxtors),vprop(maxtors),niprop,
//     &                iiprop(4,maxtors)

    int niprop;
    int[,] iiprop = new int[1+4, 1+maxtors];
    double[] kprop = new double[1+maxtors];
    double[] vprop = new double[1+maxtors];
}
}
}
