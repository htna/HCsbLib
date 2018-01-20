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
///     ##  COPYRIGHT (C)  1992  by  Jay William Ponder  ##
///     ##              All Rights Reserved              ##
///     ###################################################
///
///     ##############################################################
///     ##                                                          ##
///     ##  angpot.i  --  specifics of angle bend functional forms  ##
///     ##                                                          ##
///     ##############################################################
///
///
///     angunit    convert angle bending energy to kcal/mole
///     stbnunit   convert stretch-bend energy to kcal/mole
///     aaunit     convert angle-angle energy to kcal/mole
///     opbunit    convert out-of-plane bend energy to kcal/mole
///     opdunit    convert out-of-plane distance energy to kcal/mole
///     cang       cubic coefficient in angle bending potential
///     qang       quartic coefficient in angle bending potential
///     pang       quintic coefficient in angle bending potential
///     sang       sextic coefficient in angle bending potential
///     copb       cubic coefficient in out-of-plane bend potential
///     qopb       quartic coefficient in out-of-plane bend potential
///     popb       quintic coefficient in out-of-plane bend potential
///     sopb       sextic coefficient in out-of-plane bend potential
///     copd       cubic coefficient in out-of-plane distance potential
///     qopd       quartic coefficient in out-of-plane distance potential
///     popd       quintic coefficient in out-of-plane distance potential
///     sopd       sextic coefficient in out-of-plane distance potential
///     angtyp     type of angle bending function for each bond angle
///     opbtyp     type of out-of-plane bend potential energy function
///
///
//      real*8 angunit,stbnunit,aaunit
//      real*8 opbunit,opdunit
//      real*8 cang,qang,pang,sang
//      real*8 copb,qopb,popb,sopb
//      real*8 copd,qopd,popd,sopd
//      character*8 angtyp,opbtyp
//      common /angpot/ angunit,stbnunit,aaunit,opbunit,opdunit,cang,
//     &                qang,pang,sang,copb,qopb,popb,sopb,copd,qopd,
//     &                popd,sopd,angtyp(maxang),opbtyp

    double angunit;//,stbnunit,aaunit;
    //double opbunit,opdunit;
    double cang,qang,pang,sang;
    //double copb,qopb,popb,sopb;
    //double copd,qopd,popd,sopd;
    string[] angtyp = new string[maxang+1];
    //string   opbtyp;
}
}
}
