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
///     ##  cell.i  --  periodic boundaries using replicated cells  ##
///     ##                                                          ##
///     ##############################################################
///
///
///     xcell    length of the a-axis of the complete replicated cell
///     ycell    length of the b-axis of the complete replicated cell
///     zcell    length of the c-axis of the complete replicated cell
///     xcell2   half the length of the a-axis of the replicated cell
///     ycell2   half the length of the b-axis of the replicated cell
///     zcell2   half the length of the c-axis of the replicated cell
///     ncell    total number of cell replicates for periodic boundaries
///     icell    offset along axes for each replicate periodic cell
///
///
//      integer ncell,icell
//      real*8 xcell,ycell,zcell
//      real*8 xcell2,ycell2,zcell2
//      common /cell/ xcell,ycell,zcell,xcell2,ycell2,zcell2,ncell,
//     &              icell(3,maxcell)
    //int ncell;
    int[,] icell = new int[1+3, 1+maxcell];
    double xcell,ycell,zcell;
    double xcell2,ycell2,zcell2;
}
}
}
