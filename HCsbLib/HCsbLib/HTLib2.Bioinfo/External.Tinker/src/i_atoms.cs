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
///     ##  atoms.i  --  number, position and type of current atoms  ##
///     ##                                                           ##
///     ###############################################################
///
///
///     x       current x-coordinate for each atom in the system
///     y       current y-coordinate for each atom in the system
///     z       current z-coordinate for each atom in the system
///     n       total number of atoms in the current system
///     type    atom type number for each atom in the system
///
///
//      integer n,type
//      real*8 x,y,z
//      common /atoms/ x(maxatm),y(maxatm),z(maxatm),n,type(maxatm)
    int n;
    double[] x = new double[maxatm+1];
    double[] y = new double[maxatm+1];
    double[] z = new double[maxatm+1];
    int[] type = new int   [maxatm+1];
}
}
}
