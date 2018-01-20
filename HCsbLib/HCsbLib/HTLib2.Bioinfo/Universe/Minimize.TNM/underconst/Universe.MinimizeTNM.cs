using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Universe
	{
        public void __MinimizeTNM(List<ForceField.IForceField> frcflds)
        {
            HDebug.Assert(false);
            // do not use this, because not finished yet

            Graph<Universe.Atom[], Universe.Bond> univ_flexgraph = this.BuildFlexibilityGraph();
            List<Universe.RotableInfo> univ_rotinfos = this.GetRotableInfo(univ_flexgraph);
            Vector[] coords = this.GetCoords();
            double tor_normInf = double.PositiveInfinity;
            //double maxRotAngle = 0.1;
            Vector[] forces = null;
            MatrixByArr hessian = null;
            double forces_normsInf = 1;
            int iter = 0;
            double scale = 1;
            this._SaveCoordsToPdb(iter.ToString("0000")+".pdb", coords);

            while(true)
            {
                iter++;
                forces = this.GetVectorsZero();
                hessian = new double[size*3, size*3];
                Dictionary<string,object> cache = new Dictionary<string, object>();
                double energy = this.GetPotential(frcflds, coords, ref forces, ref hessian, cache);
                forces_normsInf = (new Vectors(forces)).NormsInf().ToArray().Max();
                //System.Console.WriteLine("iter {0:###}: frcnrminf {1}, energy {2}, scale {3}", iter, forces_normsInf, energy, scale);

                //if(forces_normsInf < 0.001)
                //{
                //    break;
                //}
                Vector torz = null;
                double maxcarz=1;
                Vector car = null;
                //double 
                using(new Matlab.NamedLock("TEST"))
                {
                    MatrixByArr H = hessian;
                    MatrixByArr J = Paper.TNM.GetJ(this, this.GetCoords(), univ_rotinfos);
                    Vector m = this.GetMasses(3);
                    Matlab.PutVector("TEST.F", Vector.FromBlockvector(forces));
                    Matlab.PutMatrix("TEST.J", J);
                    Matlab.PutMatrix("TEST.H", H);
                    Matlab.PutVector("TEST.M", m);
                    Matlab.Execute("TEST.M = diag(TEST.M);");
                    Matlab.Execute("TEST.JMJ = TEST.J' * TEST.M * TEST.J;");
                    Matlab.Execute("TEST.JHJ = TEST.J' * TEST.H * TEST.J;");
                    // (J' H J) tor = J' F
                    // (V' D V) tor = J' F  <= (V,D) are (eigvec,eigval) of generalized eigenvalue problem with (A = JHJ, B = JMJ)
                    // tor = inv(V' D V) J' F
                    Matlab.Execute("[TEST.V, TEST.D] = eig(TEST.JHJ, TEST.JMJ);");
                    //Matlab.Execute("TEST.zidx = 3:end;");

                    Matlab.Execute("TEST.invJHJ  = TEST.V * pinv(TEST.D ) * TEST.V';");
                    Matlab.Execute("TEST.tor  = TEST.invJHJ * TEST.J' * TEST.F;");
                    Matlab.Execute("TEST.car  = TEST.J * TEST.tor;");
                    car = Matlab.GetVector("TEST.car");

                    Matlab.Execute("[TEST.DS, TEST.DSI] = sort(abs(diag(TEST.D)));");
                    Matlab.Execute("TEST.zidx = TEST.DSI(6:end);");
                    Matlab.Execute("TEST.Dz = TEST.D;");
                    //Matlab.Execute("TEST.Dz(TEST.zidx,TEST.zidx) = 0;");
                    Matlab.Execute("TEST.invJHJz = TEST.V * pinv(TEST.Dz) * TEST.V';");
                    Matlab.Execute("TEST.torz = TEST.invJHJz * TEST.J' * TEST.F;");
                    Matlab.Execute("TEST.carz = TEST.J * TEST.torz;");
                    torz = Matlab.GetVector("TEST.torz");
                    maxcarz = Matlab.GetValue("max(max(abs(TEST.carz)))");
                    scale = 1;
                    if(maxcarz > 0.01) scale = scale * 0.01 / maxcarz;
                    Matlab.Clear("TEST");
                };
                tor_normInf = torz.NormInf();
                double frcnrinf = car.ToArray().HAbs().Max();
                if(maxcarz < 0.001)
                {
                    break;
                }
                System.Console.WriteLine("iter {0:###}: frcnrminf {1}, tor(frcnrinf) {2}, energy {3}, scale {4}", iter, forces_normsInf, frcnrinf, energy, scale);

                HDebug.Assert(univ_rotinfos.Count == torz.Size);
                for(int i=0; i<univ_rotinfos.Count; i++)
                {
                    Universe.RotableInfo rotinfo = univ_rotinfos[i];
                    Vector rotOrigin = coords[rotinfo.bondedAtom.ID];
                    double rotAngle  = torz[i]  * scale; // (maxRotAngle / tor_normInf);
                    if(rotAngle == 0)
                        continue;
                    Vector rotAxis   = coords[rotinfo.bond.atoms[1].ID] - coords[rotinfo.bond.atoms[0].ID];
                    Quaternion rot = new Quaternion(rotAxis, rotAngle);
                    MatrixByArr rotMat = rot.RotationMatrix;
                    foreach(Atom atom in rotinfo.rotAtoms)
                    {
                        int id = atom.ID;
                        Vector coord = rotMat * (coords[id] - rotOrigin) + rotOrigin;
                        coords[id] = coord;
                    }
                }
                this._SaveCoordsToPdb(iter.ToString("0000")+".pdb", coords);
            }
        }
    }
}
