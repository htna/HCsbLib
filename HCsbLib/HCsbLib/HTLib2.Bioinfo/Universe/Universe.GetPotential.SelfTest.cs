using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        static bool GetPotential_SelfTest_do = true;
        public static void GetPotential_SelfTest(string rootpath, string[] args)
        {
            if(GetPotential_SelfTest_do == false)
                return;

            GetPotential_SelfTest_do = false;
            Namd.Psf psf = Namd.Psf.FromFile(rootpath + @"\Sample\alanin.psf");
            Pdb pdb = Pdb.FromFile(rootpath + @"\Sample\alanin.pdb");
            Namd.Prm prm = Namd.Prm.FromFileXPlor(rootpath + @"\Sample\alanin.params", new TextLogger());

            Universe univ = Universe.Build(psf, prm, pdb, false);

            List<ForceField.IForceField> frcflds = ForceField.GetMindyForceFields();
            Vector[] forces = univ.GetVectorsZero();
            MatrixByArr hessian = null;
            Dictionary<string, object> cache = new Dictionary<string, object>();
            double energy = univ.GetPotential(frcflds, ref forces, ref hessian, cache);
            double toler = 0.000005;
            HDebug.AssertTolerance(toler, SelfTest_alanin_energy             - energy);
            HDebug.AssertTolerance(toler, SelfTest_alanin_energy_bonds        - (double)cache["energy_bonds     "]);
            HDebug.AssertTolerance(toler, SelfTest_alanin_energy_angles       - (double)cache["energy_angles    "]);
            HDebug.AssertTolerance(toler, SelfTest_alanin_energy_dihedrals    - (double)cache["energy_dihedrals "]);
            HDebug.AssertTolerance(toler, SelfTest_alanin_energy_impropers    - (double)cache["energy_impropers "]);
            HDebug.AssertTolerance(toler, SelfTest_alanin_energy_nonbondeds   - (double)cache["energy_nonbondeds"]);
            HDebug.AssertTolerance(toler, SelfTest_alanin_energy_unknowns     - (double)cache["energy_customs   "]);
            HDebug.AssertTolerance(toler, SelfTest_alanin_forces.GetLength(0) - forces.Length);
            for(int i=0; i<forces.Length; i++)
            {
                HDebug.Assert(forces[i].Size == 3);
                HDebug.AssertTolerance(toler, SelfTest_alanin_forces[i, 0] - forces[i][0]);
                HDebug.AssertTolerance(toler, SelfTest_alanin_forces[i, 1] - forces[i][1]);
                HDebug.AssertTolerance(toler, SelfTest_alanin_forces[i, 2] - forces[i][2]);
            }
        }

        static double    SelfTest_alanin_energy            = 4.5242582300;
        static double    SelfTest_alanin_energy_bonds      = 0.0050481628;
        static double    SelfTest_alanin_energy_angles     = 0.4191826514;
        static double    SelfTest_alanin_energy_dihedrals  = 0.0367989269;
        static double    SelfTest_alanin_energy_impropers  = 0.4590520051;
        static double    SelfTest_alanin_energy_vdw        = 0.5247388799;
        static double    SelfTest_alanin_energy_elec       = 3.0794376040;
        static double    SelfTest_alanin_energy_nonbondeds = SelfTest_alanin_energy_vdw + SelfTest_alanin_energy_elec;
        static double    SelfTest_alanin_energy_unknowns   = 0.0000000000;
        static double[,] SelfTest_alanin_forces = new double[,]{
                        {   1.0155925293,   0.5384059870,   0.0999537691 },
                        { -11.6400504560,  -7.1257119877,   5.0932371355 },
                        { -10.8424839461,   5.6826290218,   7.4480493268 },
                        {  -5.1777757355,  -1.1067095630,  -0.9914337925 },
                        {  -1.2022186549,  -0.7028165079,   3.3870148371 },
                        {   7.0382930735,  -4.3814638619,  -5.7305778394 },
                        {  -1.1370128131,  -3.1543032357,   0.3698093035 },
                        {  22.1098691222,  -5.4648428435, -11.0084835714 },
                        {   5.5587755240, -11.3193872498,   5.8405901149 },
                        {  -0.6871908946,   0.6689847004,  -4.7567821282 },
                        {   3.1838450285,  -1.5059211100,   1.3167526472 },
                        {  -2.3963392429,   7.9447266978,  -5.4718666670 },
                        {   0.6999108066,  -1.8865899320,  -2.6657056596 },
                        {  -1.8860147148,  22.5958922713,  -7.6487167212 },
                        {  10.1916806870,   2.8438546643, -10.1943274320 },
                        {  -4.5626195824,   2.8462750525,  -1.9779130129 },
                        {   3.2408413439,   0.7525129253,   0.8158296473 },
                        {  -7.9647474660,   0.0100418205,   6.5074438634 },
                        {  -1.9785439253,   1.6402311287,  -2.3714577021 },
                        { -12.2037702999,   1.3597066688,  20.2261046842 },
                        {  -6.6495433279,  13.6601506105,  -0.2620262114 },
                        {  -2.4434213632,  -1.0636191571,   3.5587859628 },
                        {  -1.2386909586,   0.1815517141,  -0.8038519571 },
                        {   2.5271854135,  -9.9548843525,   1.5144449039 },
                        {  -3.3379936664,  -0.8471924800,   0.9688832888 },
                        {  13.0763005451, -18.0560962469,   6.3131612136 },
                        {  -4.7353378322,  -4.4541351391,  13.7178946864 },
                        {   3.4648129943,  -3.7244336331,  -0.9831696298 },
                        {  -2.0529459732,  -0.9157795455,   0.6028924378 },
                        {   4.4843401728,   2.1061200625,  -7.9305104008 },
                        {  -0.1806516523,  -3.5628051319,  -0.9859353508 },
                        {  13.3357358835,   6.1554860959, -17.4296573525 },
                        {  12.4160800018,  -9.0914639998,  -0.9787795392 },
                        {   0.5724368277,   3.0404410470,  -4.4196782828 },
                        {   0.2819280697,  -2.4110900138,  -0.2818868012 },
                        {  -6.6246262098,   7.8279958823,  -0.2486684219 },
                        {   0.0485719768,  -0.3740874436,  -3.7278101812 },
                        { -12.6194746891,  17.5310404474,   2.6698726291 },
                        {   4.6775274040,  11.3331471100, -10.2392670864 },
                        {  -3.5519855211,   4.0265729504,   2.5542674100 },
                        {  -0.5006333023,  -1.0108239305,  -2.9260310912 },
                        {  -3.8343591441,  -5.3729619624,   6.6644594327 },
                        {  -3.3125022829,   1.0790377090,  -1.1581067068 },
                        {  -6.0833162302, -10.8938323717,  18.2155245727 },
                        { -11.1000878423,   9.2544496438,   8.5388540788 },
                        {  -0.0148212023,  -4.6905465563,   3.5587242289 },
                        {  -2.2899352504,   0.9262803666,  -1.0385413275 },
                        {   5.6295284377,  -6.7095349460,  -3.1094354105 },
                        {  -2.4854617850,  -2.5924860090,   0.8119710995 },
                        {  19.4345890132, -10.7890762151,  -3.9348378705 },
                        {  -1.0845940686, -15.3056553179,   8.3816433892 },
                        {   3.0508229346,  -2.3647339085,  -4.8046687109 },
                        {  -1.4790365935,  -1.5222267066,   1.3115921642 },
                        {   0.8583542096,   6.1034316191,  -7.8716221919 },
                        {   0.5464644301,  -3.0735344368,  -2.0382090964 },
                        {   4.6074030158,  16.9067748573, -10.5030568305 },
                        {  11.4690529191,  -2.6902098382, -14.2020649784 },
                        {  -2.0752911262,   6.1403210570,  -3.5272155538 },
                        {   0.5475345518,  -2.7978463558,  -1.1582688080 },
                        {  -7.5583656766,   2.8272195478,   3.5459582027 },
                        {  -1.4952375361,   0.9351647465,  -3.3794153493 },
                        {  -5.7050967247,   2.6970253610,  17.0544152261 },
                        {  -1.6553363326,   6.7941637723,   1.5374078734 },
                        {   0.4807068449,   0.8465184419,   9.2856158482 },
                        {   1.9570169774,   0.2720764711,  -2.4814885286 },
                        {  -0.7176867151,  -6.6114284623,   1.3303142178 },
                        };
    }
}
