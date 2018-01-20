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
///     ##  boxes.i  --  parameters for periodic boundary conditions  ##
///     ##                                                            ##
///     ################################################################
///
///
///     xbox        length of a-axis of periodic box in Angstroms
///     ybox        length of b-axis of periodic box in Angstroms
///     zbox        length of c-axis of periodic box in Angstroms
///     alpha       angle between b- and c-axes of box in degrees
///     beta        angle between a- and c-axes of box in degrees
///     gamma       angle between a- and b-axes of box in degrees
///     xbox2       half of the a-axis length of periodic box
///     ybox2       half of the b-axis length of periodic box
///     zbox2       half of the c-axis length of periodic box
///     box34       three-fourths axis length of truncated octahedron
///     lvec        real space lattice vectors as matrix rows
///     recip       reciprocal lattice vectors as matrix columns
///     volbox      volume in Ang**3 of the periodic box
///     beta_sin    sine of the beta periodic box angle
///     beta_cos    cosine of the beta periodic box angle
///     gamma_sin   sine of the gamma periodic box angle
///     gamma_cos   cosine of the gamma periodic box angle
///     beta_term   term used in generating triclinic box
///     gamma_term  term used in generating triclinic box
///     orthogonal  flag to mark periodic box as orthogonal
///     monoclinic  flag to mark periodic box as monoclinic
///     triclinic   flag to mark periodic box as triclinic
///     octahedron  flag to mark box as truncated octahedron
///     spacegrp    space group symbol for the unitcell type
///
///
//      real*8 xbox,ybox,zbox
//      real*8 alpha,beta,gamma
//      real*8 xbox2,ybox2,zbox2
//      real*8 box34,volbox
//      real*8 lvec,recip
//      real*8 beta_sin,beta_cos
//      real*8 gamma_sin,gamma_cos
//      real*8 beta_term,gamma_term
//      logical orthogonal,monoclinic
//      logical triclinic,octahedron
//      character*10 spacegrp
//      common /boxes/ xbox,ybox,zbox,alpha,beta,gamma,xbox2,ybox2,zbox2,
//     &               box34,lvec(3,3),recip(3,3),volbox,beta_sin,
//     &               beta_cos,gamma_sin,gamma_cos,beta_term,gamma_term,
//     &               orthogonal,monoclinic,triclinic,octahedron,spacegrp

    double xbox,ybox,zbox;
    //double alpha,beta,gamma;
    double xbox2,ybox2,zbox2;
    double box34;//,volbox;
    double[,] lvec  = new double[1+3, 1+3];
    double[,] recip = new double[1+3, 1+3];
    double beta_sin,beta_cos;
    double gamma_sin,gamma_cos;
    double beta_term,gamma_term;
    bool orthogonal,monoclinic;
    bool triclinic,octahedron;
    //string spacegrp;
}
}
}
