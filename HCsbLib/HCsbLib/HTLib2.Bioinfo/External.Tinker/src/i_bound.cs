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
///     ##  bound.i  --  control of periodic boundary conditions  ##
///     ##                                                        ##
///     ############################################################
///
///
///     polycut       cutoff distance for infinite polymer nonbonds
///     polycut2      square of infinite polymer nonbond cutoff
///     use_bounds    flag to use periodic boundary conditions
///     use_replica   flag to use replicates for periodic system
///     use_polymer   flag to mark presence of infinite polymer
///
///
//      real*8 polycut,polycut2
//      logical use_bounds
//      logical use_replica
//      logical use_polymer
//      common /bound/ polycut,polycut2,use_bounds,use_replica,use_polymer
    //double polycut,polycut2;
    //bool use_bounds;
    //bool use_replica;
    bool use_polymer;
}
}
}
