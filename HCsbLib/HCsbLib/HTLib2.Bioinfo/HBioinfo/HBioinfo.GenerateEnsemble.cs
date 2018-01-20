using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    public static partial class HBioinfo
    {
        ////////////////////////////////////////////////////////////////////////////
        // Source: Gromacs, g_nmens.c
        //
        // #define BOLTZ            (RGAS/KILO)            /* (kJ/(mol K)) */
        // #define RGAS             (BOLTZMANN*AVOGADRO)   /* (J/(mol K))  */
        // #define BOLTZMANN	 (1.380658e-23)		/* (J/K)	*/
        // #define AVOGADRO	 (6.0221367e23)		/* ()		*/
        // #define KILO 		 (1e3)			/* Thousand	*/
        //
        //  for(s=0; s<nstruct; s++) {
        //    for(i=0; i<natoms; i++)
        //      copy_rvec(xav[i],xout1[i]);
        //	//////////////////////////////////////////////////
        //	randnorms_mag = 0;
        //	for(j=0; j<noutvec; j++)
        //	{
        //		randnorms[j] = RandNormal();
        //		randnorms_mag += randnorms[j]*randnorms[j];
        //	}
        //	randnorms_mag = sqrt(randnorms_mag);
        //	for(j=0; j<noutvec; j++)
        //	{
        //		randnorms[j] = randnorms[j] / randnorms_mag;
        //	}
        //	//////////////////////////////////////////////////
        //    for(j=0; j<noutvec; j++) {
        //      v = outvec[j];
        //      /* (r-0.5) n times:  var_n = n * var_1 = n/12
        //	 n=4:  var_n = 1/3, so multiply with 3 */
        //      
        //      rfac  = sqrt(3.0 * BOLTZ*temp/eigval[iout[j]]);
        //      //rhalf = 2.0*rfac; 
        //      //rfac  = rfac/(real)im;
        //	  //
        //      //jran = (jran*ia+ic) & im;
        //      //jr = (real)jran;
        //      //jran = (jran*ia+ic) & im;
        //      //jr += (real)jran;
        //      //jran = (jran*ia+ic) & im;
        //      //jr += (real)jran;
        //      //jran = (jran*ia+ic) & im;
        //      //jr += (real)jran;
        //      //disp = rfac * jr - rhalf;
        //	  disp = rfac*randnorms[j];
        //      
        //      for(i=0; i<natoms; i++)
        //          for(d=0; d<DIM; d++)
        //              xout1[i][d] += disp*eigvec[v][i][d]*invsqrtm[i];
        //    }
        //    for(i=0; i<natoms; i++)
        //        copy_rvec(xout1[i],xout2[index[i]]);
        //    t = s+1;
        //    write_trx(out,natoms,index,atoms,0,t,box,xout2,NULL,NULL);
        //    fprintf(stderr,"\rGenerated %d structures",s+1);
        //  }
        ////////////////////////////////////////////////////////////////////////////

        public static Vector[] GenerateEnsemble(IList<Vector> coords, Mode mode, IList<double> mass, double temperature=300, bool normalize=true, HPack<Vector> optOutRandNorms=null)
        {
            return GenerateEnsemble(coords, new Mode[] { mode }, mass, temperature: temperature, normalize: normalize, optOutRandNorms: optOutRandNorms);
        }
        public static Vector[] GenerateEnsemble(IList<Vector> coords, Mode[] modes, IList<double> mass, double temperature=300, bool normalize=true, HPack<Vector> optOutRandNorms=null)
        {
            double[] invsqrtm = new double[mass.Count];
            {
                for(int i=0; i<mass.Count; i++)
                    invsqrtm[i] = 1.0 / Math.Sqrt(mass[i]);
            }

            modes = modes.HRemoveAllNull().ToArray();

            Vector randnorms;
            {
                Random rand = new Random();
                randnorms = rand.NextNormalVector(modes.Length, 1);
                if(normalize)
                    randnorms = randnorms.UnitVector();
            }
            if(optOutRandNorms != null)
                optOutRandNorms.value = randnorms;

            int size = mass.Count;

            Vector[] ensemble = new Vector[size];
            for(int i=0; i<size; i++)
                ensemble[i] = coords[i].Clone();

            for(int m=0; m<modes.Length; m++)
            {
                double   eigval = modes[m].eigval;
                Vector[] eigvec = modes[m].GetEigvecsOfAtoms();
                HDebug.Assert(eigvec.Length == size);

                double rfac = Math.Sqrt(3.0 * BOLTZ*temperature/eigval);
                double disp = rfac*randnorms[m];
                //for(int i=0; i<size; i++)
                System.Threading.Tasks.Parallel.For(0, size, delegate(int i)
                {
                    ensemble[i] += disp * eigvec[i] * invsqrtm[i];
                });
            }

            return ensemble;
        }
    }
}

