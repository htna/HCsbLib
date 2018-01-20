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
///     ############################################################
///     ##                                                        ##
///     ##  couple.i  --  near-neighbor atom connectivity lists   ##
///     ##                                                        ##
///     ############################################################
///
///
///     maxn13   maximum number of atoms 1-3 connected to an atom
///     maxn14   maximum number of atoms 1-4 connected to an atom
///     maxn15   maximum number of atoms 1-5 connected to an atom
///
///     n12      number of atoms directly bonded to each atom
///     i12      atom numbers of atoms 1-2 connected to each atom
///     n13      number of atoms in a 1-3 relation to each atom
///     i13      atom numbers of atoms 1-3 connected to each atom
///     n14      number of atoms in a 1-4 relation to each atom
///     i14      atom numbers of atoms 1-4 connected to each atom
///     n15      number of atoms in a 1-5 relation to each atom
///     i15      atom numbers of atoms 1-5 connected to each atom
///
///
//      integer maxn13,maxn14,maxn15
//      parameter (maxn13=3*maxval)
//      parameter (maxn14=9*maxval)
//      parameter (maxn15=27*maxval)
//      integer n12,i12,n13,i13
//      integer n14,i14,n15,i15
//      common /couple/ n12(maxatm),i12(maxval,maxatm),n13(maxatm),
//     &                i13(maxn13,maxatm),n14(maxatm),i14(maxn14,maxatm),
//     &                n15(maxatm),i15(maxn15,maxatm)
    static int maxn13=3*maxval;
    static int maxn14=9*maxval;
    static int maxn15=27*maxval;
    int[]  n12 = new int[1+maxatm];
    int[,] i12 = new int[1+maxval, 1+maxatm];
    int[]  n13 = new int[1+maxatm];
    int[,] i13 = new int[1+maxn13, 1+maxatm];
    int[]  n14 = new int[1+maxatm];
    int[,] i14 = new int[1+maxn14, 1+maxatm];
    int[]  n15 = new int[1+maxatm];
    int[,] i15 = new int[1+maxn15, 1+maxatm];
}
}
}
