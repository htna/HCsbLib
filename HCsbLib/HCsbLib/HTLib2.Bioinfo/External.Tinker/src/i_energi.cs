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
///     ##  energi.i  --  individual potential energy components  ##
///     ##                                                        ##
///     ############################################################
///
///
///     esum   total potential energy of the system
///     eb     bond stretch potential energy of the system
///     ea     angle bend potential energy of the system
///     eba    stretch-bend potential energy of the system
///     eub    Urey-Bradley potential energy of the system
///     eaa    angle-angle potential energy of the system
///     eopb   out-of-plane bend potential energy of the system
///     eopd   out-of-plane distance potential energy of the system
///     eid    improper dihedral potential energy of the system
///     eit    improper torsion potential energy of the system
///     et     torsional potential energy of the system
///     ept    pi-orbital torsion potential energy of the system
///     ebt    stretch-torsion potential energy of the system
///     ett    torsion-torsion potential energy of the system
///     ev     van der Waals potential energy of the system
///     ec     charge-charge potential energy of the system
///     ecd    charge-dipole potential energy of the system
///     ed     dipole-dipole potential energy of the system
///     em     atomic multipole potential energy of the system
///     ep     polarization potential energy of the system
///     er     reaction field potential energy of the system
///     es     solvation potential energy of the system
///     elf    metal ligand field potential energy of the system
///     eg     geometric restraint potential energy of the system
///     ex     extra term potential energy of the system
///
///
//      real*8 esum,eb,ea,eba
//      real*8 eub,eaa,eopb,eopd
//      real*8 eid,eit,et,ept
//      real*8 ebt,ett,ev,ec
//      real*8 ecd,ed,em,ep,er
//      real*8 es,elf,eg,ex
//      common /energi/ esum,eb,ea,eba,eub,eaa,eopb,eopd,eid,eit,et,ept,
//     &                ebt,ett,ev,ec,ecd,ed,em,ep,er,es,elf,eg,ex
    double eb,ea;//,esum,eba;
    double eub;//,eaa,eopb,eopd;
    double eid;//,eit,et,ept;
    //double ebt,ett,ev,ec;
    //double ecd,ed,em,ep,er;
    //double es,elf,eg,ex;
}
}
}
