using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Hess
	{
        public static HessInfo GetHessSbNMA
            ( Universe univ
            , IList<Vector> coords
            , double nbondMaxDist // =12
            , params string[] options
            )
        {
            return GetHessSbNMA
                ( univ
                , coords
                , nbondMaxDist
                , b_bonds       : true
                , b_angles      : true
                , b_impropers   : true
                , b_dihedrals   : true
                , b_nonbondeds  : true
                , b_nonbonded14s: true
                , maxAbsSpring  : null
                , sca_bonds       : null
                , sca_angles      : null
                , sca_impropers   : null
                , sca_dihedrals   : null
                , sca_nonbondeds  : null
                , sca_nonbonded14s: null
                , collectorBond        : null
                , collectorAngle       : null
                , collectorImproper    : null
                , collectorDihedral    : null
                , collectorNonbonded   : null
                , collectorNonbonded14 : null
                , GetCustomKij  : null
                , options       : options
                );
        }
        public static HessInfo GetHessSbNMA
            ( Universe univ
            , IList<Vector> coords
            , double nbondMaxDist // =12
            , Func<HessSpr.CustomKijInfo, double> GetCustomKij
            , params string[] options
            )
        {
            return GetHessSbNMA
                ( univ
                , coords
                , nbondMaxDist
                , b_bonds       : true
                , b_angles      : true
                , b_impropers   : true
                , b_dihedrals   : true
                , b_nonbondeds  : true
                , b_nonbonded14s: true
                , maxAbsSpring  : null
                , sca_bonds       : null
                , sca_angles      : null
                , sca_impropers   : null
                , sca_dihedrals   : null
                , sca_nonbondeds  : null
                , sca_nonbonded14s: null
                , collectorBond        : null
                , collectorAngle       : null
                , collectorImproper    : null
                , collectorDihedral    : null
                , collectorNonbonded   : null
                , collectorNonbonded14 : null
                , GetCustomKij  : GetCustomKij
                , options       : options
                );
        }
        public static HessInfo GetHessSbNMA
            ( Universe univ
            , IList<Vector> coords
            , double nbondMaxDist // =12
            , double? maxAbsSpring
            , params string[] options
            )
        {
            return GetHessSbNMA
                ( univ
                , coords
                , nbondMaxDist
                , b_bonds       : true
                , b_angles      : true
                , b_impropers   : true
                , b_dihedrals   : true
                , b_nonbondeds  : true
                , b_nonbonded14s: true
                , maxAbsSpring  : maxAbsSpring
                , sca_bonds       : null
                , sca_angles      : null
                , sca_impropers   : null
                , sca_dihedrals   : null
                , sca_nonbondeds  : null
                , sca_nonbonded14s: null
                , collectorBond        : null
                , collectorAngle       : null
                , collectorImproper    : null
                , collectorDihedral    : null
                , collectorNonbonded   : null
                , collectorNonbonded14 : null
                , GetCustomKij  : null
                , options       : options
                );
        }
        public static HessInfo GetHessSbNMA
            ( Universe univ
            , IList<Vector> coords
            , double nbondMaxDist // =12
            , double? maxAbsSpring
            , Func<HessSpr.CustomKijInfo, double> GetCustomKij
            , params string[] options
            )
        {
            return GetHessSbNMA
                ( univ
                , coords
                , nbondMaxDist
                , b_bonds       : true
                , b_angles      : true
                , b_impropers   : true
                , b_dihedrals   : true
                , b_nonbondeds  : true
                , b_nonbonded14s: true
                , maxAbsSpring  : maxAbsSpring
                , sca_bonds       : null
                , sca_angles      : null
                , sca_impropers   : null
                , sca_dihedrals   : null
                , sca_nonbondeds  : null
                , sca_nonbonded14s: null
                , collectorBond        : null
                , collectorAngle       : null
                , collectorImproper    : null
                , collectorDihedral    : null
                , collectorNonbonded   : null
                , collectorNonbonded14 : null
                , GetCustomKij  : GetCustomKij
                , options       : options
                );
        }
        public static HessInfo GetHessSbNMA
            ( Universe univ
            , IList<Vector> coords
            , double nbondMaxDist // =12
            , bool b_bonds
            , bool b_angles
            , bool b_impropers
            , bool b_dihedrals
            , bool b_nonbondeds
            , bool b_nonbonded14s
            , double? maxAbsSpring
            , Func<HessSpr.CustomKijInfo, double> GetCustomKij
            , params string[] options
            )
        {
            return GetHessSbNMA
            ( univ
            , coords
            , nbondMaxDist
            , b_bonds
            , b_angles
            , b_impropers
            , b_dihedrals
            , b_nonbondeds
            , b_nonbonded14s
            , maxAbsSpring
            , sca_bonds        : null
            , sca_angles       : null
            , sca_impropers    : null
            , sca_dihedrals    : null
            , sca_nonbondeds   : null
            , sca_nonbonded14s : null
            , collectorBond        : null
            , collectorAngle       : null
            , collectorImproper    : null
            , collectorDihedral    : null
            , collectorNonbonded   : null
            , collectorNonbonded14 : null
            , GetCustomKij     : GetCustomKij
            , options          : options
            );
        }
        public static HessInfo GetHessSbNMA
            ( Universe univ
            , IList<Vector> coords
            , double nbondMaxDist // =12
            , bool b_bonds
            , bool b_angles
            , bool b_impropers
            , bool b_dihedrals
            , bool b_nonbondeds
            , bool b_nonbonded14s
            , double? maxAbsSpring
            , double? sca_bonds
            , double? sca_angles
            , double? sca_impropers
            , double? sca_dihedrals
            , double? sca_nonbondeds
            , double? sca_nonbonded14s
            , Func<HessSpr.CustomKijInfo, double> GetCustomKij
            , params string[] options
            )
        {
            return GetHessSbNMA
            ( univ
            , coords
            , nbondMaxDist
            , b_bonds         : b_bonds         
            , b_angles        : b_angles        
            , b_impropers     : b_impropers     
            , b_dihedrals     : b_dihedrals     
            , b_nonbondeds    : b_nonbondeds    
            , b_nonbonded14s  : b_nonbonded14s  
            , maxAbsSpring    : maxAbsSpring    
            , sca_bonds       : sca_bonds       
            , sca_angles      : sca_angles      
            , sca_impropers   : sca_impropers   
            , sca_dihedrals   : sca_dihedrals   
            , sca_nonbondeds  : sca_nonbondeds  
            , sca_nonbonded14s: sca_nonbonded14s
            , collectorBond       : null
            , collectorAngle      : null
            , collectorImproper   : null
            , collectorDihedral   : null
            , collectorNonbonded  : null
            , collectorNonbonded14: null
            , GetCustomKij    : null
            , options         : options
            );
        }
        public static HessInfo GetHessSbNMA
            ( Universe univ
            , IList<Vector> coords
            , double nbondMaxDist // =12
            , bool b_bonds
            , bool b_angles
            , bool b_impropers
            , bool b_dihedrals
            , bool b_nonbondeds
            , bool b_nonbonded14s
            , double? maxAbsSpring
            , double? sca_bonds
            , double? sca_angles
            , double? sca_impropers
            , double? sca_dihedrals
            , double? sca_nonbondeds
            , double? sca_nonbonded14s
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, double>                                                       collectorBond
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double>                        collectorAngle
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collectorImproper
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collectorDihedral
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, double>                                                       collectorNonbonded
            , Action<Universe.Atom, Vector, Universe.Atom, Vector, double>                                                       collectorNonbonded14
            , Func<HessSpr.CustomKijInfo, double> GetCustomKij
            , params string[] options
            )
        {
            return HessSbNMA.GetHessSbNMA
            (univ, coords, nbondMaxDist
            , maxAbsSpring
            , b_bonds, b_angles, b_impropers, b_dihedrals, b_nonbondeds, b_nonbonded14s
            , sca_bonds, sca_angles, sca_impropers, sca_dihedrals, sca_nonbondeds, sca_nonbonded14s
            , collectorBond, collectorAngle, collectorImproper, collectorDihedral, collectorNonbonded, collectorNonbonded14
            , GetCustomKij
            , options
            );
        }
        public static partial class HessSbNMA
        {
            public static HessInfo GetHessSbNMA
                ( Universe univ
                , IList<Vector> coords
                , double nbondMaxDist // =12
                , double? maxAbsSpring
                , bool b_bonds
                , bool b_angles
                , bool b_impropers
                , bool b_dihedrals
                , bool b_nonbondeds
                , bool b_nonbonded14s
                , double? sca_bonds
                , double? sca_angles
                , double? sca_impropers
                , double? sca_dihedrals
                , double? sca_nonbondeds
                , double? sca_nonbonded14s
                , Action<Universe.Atom, Vector, Universe.Atom, Vector, double>                                                       collectorBond
                , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double>                        collectorAngle
                , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collectorImproper
                , Action<Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, Universe.Atom, Vector, double, double> collectorDihedral
                , Action<Universe.Atom, Vector, Universe.Atom, Vector, double>                                                       collectorNonbonded
                , Action<Universe.Atom, Vector, Universe.Atom, Vector, double>                                                       collectorNonbonded14
                , Func<HessSpr.CustomKijInfo, double> GetCustomKij = null
                , params string[] options
                )
            {
                IEnumerable<Universe.Nonbonded>   nonbondeds   = null;
                IEnumerable<Universe.Nonbonded14> nonbonded14s = univ.nonbonded14s.GetEnumerable();
                if(options.Contains("TIP3P: tetrahedral hydrogen bonds")) nonbondeds = EnumNonbondeds(univ.atoms, coords, univ.size, nbondMaxDist, ListTip3pTetraHBond, options);
                else if(options.Contains("TIP3P: near waters"          )) nonbondeds = EnumNonbondeds(univ.atoms, coords, univ.size, nbondMaxDist, ListTip3pNears     , options);
                else                                                      nonbondeds = EnumNonbondeds(univ.atoms, coords, univ.size, nbondMaxDist);

                bool vdW  = true;      // use vdW
                bool elec = false;     // ignore electrostatic
                double D  = double.PositiveInfinity; // dielectric constant for Tinker is "1"
                bool ignNegSpr = true; // ignore negative spring (do not add the spring into hessian matrix)
                string nbnd_opt = null;

                if(options.Contains("TIP3P: (vdW+elec) for OH,OO,HH")) nbnd_opt = "TIP3P: (vdW+elec) for OH,OO,HH";
                if(options.Contains("TIP3P: (vdW+elec) for OH"      )) nbnd_opt = "TIP3P: (vdW+elec) for OH"      ;
                if(options.Contains("vdW:L79"                       )) nbnd_opt = "L79";

                double? K_r       = null;  // null for sbNMA, and 340.00  for ssNMA
                double? K_theta   = null;  // null for sbNMA, and 45.00   for ssNMA
                double? K_ub      = null;  // null for sbNMA, and 10.00   for ssNMA
                double? K_psi     = null;  // null for sbNMA, and 70.00   for ssNMA
                double? K_chi     = null;  // null for sbNMA, and 1.00    for ssNMA
                double? n         = null;  // null for sbNMA, and 1       for ssNMA
                string k_vdW      = null;  // null for sbNMA, and "Unif"  for ssNMA

                HessMatrix hess = null;
                if(b_bonds       )  hess = STeM.GetHessBond       (coords, univ.bonds          , null      ,hessian: hess,                                                                         collector: collectorBond       );
                if(b_angles      )  hess = STeM.GetHessAngle      (coords, univ.angles   , true, null, null,hessian: hess,                                                                         collector: collectorAngle      );
                if(b_impropers   )  hess = STeM.GetHessImproper   (coords, univ.impropers      , null      ,hessian: hess,                  useArnaud96:true,                                      collector: collectorImproper   );
                if(b_dihedrals   )  hess = STeM.GetHessDihedral   (coords, univ.dihedrals      , null, null,hessian: hess, useAbsSpr: true, useArnaud96:true,                                      collector: collectorDihedral   );
                if(b_nonbondeds  )  hess = HessSpr.GetHessNonbond (coords, nonbondeds        ,D, nbnd_opt  ,hessian: hess, vdW: vdW, elec: elec, ignNegSpr: ignNegSpr, maxAbsSpring: maxAbsSpring, collector: collectorNonbonded  , GetCustomKij: GetCustomKij);
                if(b_nonbonded14s)  hess = HessSpr.GetHessNonbond (coords, nonbonded14s      ,D, nbnd_opt  ,hessian: hess, vdW: vdW, elec: elec, ignNegSpr: ignNegSpr, maxAbsSpring: maxAbsSpring, collector: collectorNonbonded14, GetCustomKij: GetCustomKij);

                if(sca_bonds        != null) if(sca_bonds       .Value != 1)  hess += (sca_bonds       .Value-1)*STeM.GetHessBond       (coords, univ.bonds          , null      ,hessian: null);
                if(sca_angles       != null) if(sca_angles      .Value != 1)  hess += (sca_angles      .Value-1)*STeM.GetHessAngle      (coords, univ.angles   , true, null, null,hessian: null);
                if(sca_impropers    != null) if(sca_impropers   .Value != 1)  hess += (sca_impropers   .Value-1)*STeM.GetHessImproper   (coords, univ.impropers      , null      ,hessian: null,                  useArnaud96:true);
                if(sca_dihedrals    != null) if(sca_dihedrals   .Value != 1)  hess += (sca_dihedrals   .Value-1)*STeM.GetHessDihedral   (coords, univ.dihedrals      , null, null,hessian: null, useAbsSpr: true, useArnaud96:true);
                if(sca_nonbondeds   != null) if(sca_nonbondeds  .Value != 1)  hess += (sca_nonbondeds  .Value-1)*HessSpr.GetHessNonbond (coords, nonbondeds        ,D, nbnd_opt  ,hessian: null, vdW: vdW, elec: elec, ignNegSpr: ignNegSpr, maxAbsSpring: maxAbsSpring);
                if(sca_nonbonded14s != null) if(sca_nonbonded14s.Value != 1)  hess += (sca_nonbonded14s.Value-1)*HessSpr.GetHessNonbond (coords, nonbonded14s      ,D, nbnd_opt  ,hessian: null, vdW: vdW, elec: elec, ignNegSpr: ignNegSpr, maxAbsSpring: maxAbsSpring);

                //Hess.UpdateHessNaN(hess, coords);
                {
                    foreach(var bc_br_bval in hess.EnumBlocks())
                    {
                        int bc   = bc_br_bval.Item1;
                        int br   = bc_br_bval.Item2;
                        var bval = bc_br_bval.Item3;

                        if(coords[bc] == null) throw new HException("have hess block for null-coord");
                        if(coords[br] == null) throw new HException("have hess block for null-coord");
                        if(bval == null) throw new HException();
                        for(int c=0; c<bval.ColSize; c++)
                            for(int r=0; r<bval.RowSize; r++)
                            {
                                double val = bval[c, r];
                                if(double.IsNaN             (val)) throw new HException("hess has nan element");
                                if(double.IsPositiveInfinity(val)) throw new HException("hess has pos-inf element");
                                if(double.IsNegativeInfinity(val)) throw new HException("hess has neg-inf element");
                            }
                    }
                }

                return new HessInfo
                {
                    hess   = hess,
                    mass   = univ.GetMasses(),
                    atoms  = univ.atoms.ToArray(),
                    coords = coords.HCloneVectors().ToArray(),
                    numZeroEigval = 6,
                };
            }
        }
        //public static SparseHessInfo GetSparseHessSbNMA(Universe univ, IList<Vector> coords, double nbondMaxDist=12)
        //{
        //    IEnumerable<Universe.Nonbonded> nonbondeds;
        //    IEnumerable<Universe.Nonbonded14> nonbonded14s;
        //    {
        //        Universe.Nonbondeds _nonbondeds = new Universe.Nonbondeds(univ.atoms, univ.size, nbondMaxDist);
        //        _nonbondeds.UpdateCoords(univ.GetCoords());
        //        nonbondeds = _nonbondeds.EnumNonbondeds();
        //        nonbonded14s = univ.nonbonded14s.GetEnumerable();
        //    }
        //    bool vdW  = true;      // use vdW
        //    bool elec = false;     // ignore electrostatic
        //    double D  = double.PositiveInfinity; // dielectric constant for Tinker is "1"
        //    bool ignNegSpr = true; // ignore negative spring (do not add the spring into hessian matrix)
        //
        //    Func<MatrixByArr> GetDefaultHessElement = delegate() { return new double[3, 3]; };
        //    MatrixSparse<MatrixByArr> hess = new MatrixSparse<MatrixByArr>(univ.size, univ.size, GetDefault: GetDefaultHessElement);
        //
        //    STeM   .GetHessBond    (coords ,univ.bonds        ,null       ,hess);
        //    STeM   .GetHessAngle   (coords ,univ.angles ,true ,null ,null ,hess);
        //    STeM   .GetHessImproper(coords ,univ.impropers    ,null       ,hess ,useArnaud96:true);
        //    STeM   .GetHessDihedral(coords ,univ.dihedrals    ,null ,null ,hess ,useAbsSpr:true, useArnaud96:true);
        //    HessSpr.GetHessNonbond (coords ,nonbondeds     ,D ,null       ,hess ,vdW:vdW ,elec:elec ,ignNegSpr:ignNegSpr);
        //    HessSpr.GetHessNonbond (coords ,nonbonded14s   ,D ,null       ,hess ,vdW:vdW ,elec:elec ,ignNegSpr:ignNegSpr);
        //
        //    var hessinfo = new SparseHessInfo
        //    {
        //        hess   = hess,
        //        mass   = univ.GetMasses(),
        //        atoms  = univ.atoms.ToArray(),
        //        coords = coords.HCloneVectors().ToArray(),
        //        numZeroEigval = 6,
        //    };
        //
        //    return hessinfo;
        //}
	}
}
