#pragma warning disable CS0649

namespace HTLib2.Bioinfo
{
public partial class Tinker
{
public partial class Src
{
///
///
///     ###################################################
///     ##  COPYRIGHT (C)  1997  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##
///     ###################################################
///
///     ############################################################
///     ##                                                        ##
///     ##  group.i  --  partitioning of system into atom groups  ##
///     ##                                                        ##
///     ############################################################
///
///
///     grpmass     total mass of all the atoms in each group
///     wgrp        weight for each set of group-group interactions
///     ngrp        total number of atom groups in the system
///     kgrp        contiguous list of the atoms in each group
///     igrp        first and last atom of each group in the list
///     grplist     number of the group to which each atom belongs
///     use_group   flag to use partitioning of system into groups
///     use_intra   flag to include only intragroup interactions
///     use_inter   flag to include only intergroup interactions
///
///
//      integer ngrp,kgrp
//      integer igrp,grplist
//      real*8 grpmass,wgrp
//      logical use_group
//      logical use_intra
//      logical use_inter
//      common /group/ grpmass(maxgrp),wgrp(0:maxgrp,0:maxgrp),ngrp,
//     &               kgrp(maxatm),igrp(2,0:maxgrp),grplist(maxatm),
//     &               use_group,use_intra,use_inter
	//int    ngrp;
    int[]  kgrp = new int[maxatm+1];
	int[,] igrp = new int[2+1,maxgrp+1];
    int[]  grplist = new int[maxatm+1];
	double[]  grpmass = new double[maxgrp];
    double[,] wgrp    = new double[maxgrp+1,maxgrp+1];
	bool use_group;
	//bool use_intra;
    //bool use_inter;
}
}
}