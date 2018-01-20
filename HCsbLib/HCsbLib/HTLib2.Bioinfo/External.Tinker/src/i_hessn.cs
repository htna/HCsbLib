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
///     ##  hessn.i  --  Cartesian Hessian elements for a single atom  ##
///     ##                                                             ##
///     #################################################################
///
///
///     hessx   Hessian elements for x-component of current atom
///     hessy   Hessian elements for y-component of current atom
///     hessz   Hessian elements for z-component of current atom
///
///
//      real*8 hessx,hessy,hessz
//      common /hessn/ hessx(3,maxatm),hessy(3,maxatm),hessz(3,maxatm)
    double[,] hessx = new double[3+1,maxatm+1];
    double[,] hessy = new double[3+1,maxatm+1];
    double[,] hessz = new double[3+1,maxatm+1];
}
}
}
