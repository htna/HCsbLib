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
///     ##  atmlst.i  --  local geometry terms involving each atom  ##
///     ##                                                          ##
///     ##############################################################
///
///
///     bndlist   list of the bond numbers involving each atom
///     anglist   list of the angle numbers centered on each atom
///
///
//      integer bndlist,anglist
//      common /atmlst/ bndlist(maxval,maxatm),
//     &                anglist(maxval*(maxval-1)/2,maxatm)

    int[,] bndlist = new int[1+maxval,              1+maxatm];
    int[,] anglist = new int[1+maxval*(maxval-1)/2, 1+maxatm];
}
}
}
