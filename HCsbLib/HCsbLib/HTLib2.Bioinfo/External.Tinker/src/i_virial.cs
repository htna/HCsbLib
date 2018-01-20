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
///     ##########################################################
///     ##                                                      ##
///     ##  virial.i  --  components of internal virial tensor  ##
///     ##                                                      ##
///     ##########################################################
///
///
///     vir    total internal virial Cartesian tensor components
///
///
//      real*8 vir
//      common /virial/ vir(3,3)
    double[,] vir = new double[3+1, 3+1];
}
}
}
