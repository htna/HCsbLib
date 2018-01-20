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
///     #############################################################
///     ##                                                         ##
///     ##  sizes.i  --  parameter values to set array dimensions  ##
///     ##                                                         ##
///     #############################################################
///
///
///     "sizes.i" sets values for critical array dimensions used
///     throughout the software; these parameters will fix the size
///     of the largest systems that can be handled; values too large
///     for the computer memory or swap space to accomodate will
///     result in poor performance or outright failure
///
///     parameter:      maximum allowed number of:
///
///     maxatm          atoms in the molecular system
///     maxval          atoms directly bonded to an atom
///     maxgrp          user-defined groups of atoms
///     maxref          stored reference molecular systems
///     maxtyp          force field atom type definitions
///     maxclass        force field atom class definitions
///     maxprm          lines in the parameter file
///     maxkey          lines in the keyword file
///     maxrot          bonds for torsional rotation
///     maxvar          optimization variables (vector storage)
///     maxopt          optimization variables (matrix storage)
///     maxhess         off-diagonal Hessian elements
///     maxlight        sites for method of lights neighbors
///     maxvlst         neighbors in van der Waals pair list
///     maxelst         neighbors in electrostatics pair list
///     maxulst         neighbors in dipole preconditioner list
///     maxfft          grid points in each FFT dimension
///     maxfix          geometric constraints and restraints
///     maxvib          vibrational frequencies
///     maxgeo          distance geometry points
///     maxcell         unit cells in replicated crystal
///     maxring         3-, 4-, or 5-membered rings
///     maxbio          biopolymer atom definitions
///     maxres          residues in the macromolecule
///     maxele          elements in periodic table
///     maxamino        amino acid residue types
///     maxnuc          nucleic acid residue types
///     maxbnd          covalent bonds in molecular system
///     maxang          bond angles in molecular system
///     maxtors         torsional angles in molecular system
///     maxbitor        bitorsions in molecular system
///
///
//      integer maxatm,maxval,maxgrp
//      integer maxref,maxtyp,maxclass
//      integer maxprm,maxkey,maxrot
//      integer maxvar,maxopt,maxhess
//      integer maxlight,maxvlst,maxelst
//      integer maxulst,maxfft,maxfix
//      integer maxvib,maxgeo,maxcell
//      integer maxring,maxbio,maxres
//      integer maxele,maxamino,maxnuc
//      integer maxbnd,maxang,maxtors
//      integer maxbitor
//      parameter (maxatm=100000)
//      parameter (maxval=8)
//      parameter (maxgrp=1000)
//      parameter (maxref=10)
//      parameter (maxtyp=5000)
//      parameter (maxclass=1000)
//      parameter (maxprm=25000)
//      parameter (maxkey=25000)
//      parameter (maxrot=1000)
//      parameter (maxvar=3*maxatm)
//      parameter (maxopt=1000)
//      parameter (maxhess=1000000)
//      parameter (maxlight=8*maxatm)
//      parameter (maxvlst=1800)
//      parameter (maxelst=1200)
//      parameter (maxulst=100)
//      parameter (maxfft=250)
//      parameter (maxfix=maxatm)
//      parameter (maxvib=1000)
//      parameter (maxgeo=2500)
//      parameter (maxcell=10000)
//      parameter (maxring=10000)
//      parameter (maxbio=10000)
//      parameter (maxres=10000)
//      parameter (maxele=112)
//      parameter (maxamino=38)
//      parameter (maxnuc=12)
//      parameter (maxbnd=2*maxatm)
//      parameter (maxang=4*maxatm)
//      parameter (maxtors=6*maxatm)
//      parameter (maxbitor=8*maxatm)

    static int maxatm=100000;
    static int maxval=8;
    static int maxgrp=1000;
    //static int maxref=10;
    //static int maxtyp=5000;
    //static int maxclass=1000;
    //static int maxprm=25000;
    //static int maxkey=25000;
    //static int maxrot=1000;
    static int maxvar=3*maxatm;
    //static int maxopt=1000;
    //static int maxhess=1000000;
    static int maxlight=8*maxatm;
    //static int maxvlst=1800;
    //static int maxelst=1200;
    //static int maxulst=100;
    //static int maxfft=250;
    static int maxfix=maxatm;
    //static int maxvib=1000;
    //static int maxgeo=2500;
    static int maxcell=10000;
    //static int maxring=10000;
    //static int maxbio=10000;
    //static int maxres=10000;
    //static int maxele=112;
    //static int maxamino=38;
    //static int maxnuc=12;
    static int maxbnd=2*maxatm;
    static int maxang=4*maxatm;
    static int maxtors=6*maxatm;
    static int maxbitor=8*maxatm;
}
}
}
