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
///     ################################################################
///     ##                                                            ##
///     ##  bndpot.i  --  specifics of bond stretch functional forms  ##
///     ##                                                            ##
///     ################################################################
///
///
///     cbnd      cubic coefficient in bond stretch potential
///     qbnd      quartic coefficient in bond stretch potential
///     bndunit   convert bond stretch energy to kcal/mole
///     bndtyp    type of bond stretch potential energy function
///
///
//      real*8 cbnd,qbnd
//      real*8 bndunit
//      character*8 bndtyp
//      common /bndpot/ cbnd,qbnd,bndunit,bndtyp
    double cbnd,qbnd;
    double bndunit;
    string bndtyp;
}
}
}
