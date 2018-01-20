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
///     ##  usage.i  --  atoms active during energy computation  ##
///     ##                                                       ##
///     ###########################################################
///
///
///     nuse   total number of active atoms in energy calculation
///     iuse   numbers of the atoms active in energy calculation
///     use    true if an atom is active, false if inactive
///
///
//      integer nuse
//      integer, pointer :: iuse(:)
//      logical, pointer :: use(:)
//      common /usage/ nuse,iuse,use
    //int nuse;
    //int[] iuse;
    bool[] use;
}
}
}
