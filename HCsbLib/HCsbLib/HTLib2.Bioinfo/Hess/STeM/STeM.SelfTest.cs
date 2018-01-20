using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Hess
{
    public partial class STeM
    {
        static bool _SelfTest = HDebug.IsDebuggerAttached;
        public static void SelfTest()
        {
            Matlab.NumericSolver.Register();
            //{
            //    Pdb pdb = Pdb.FromPdbid("1GIA");
            //    List<Pdb.Atom> atoms = pdb.atoms.SelectByChainID().SelectByAltLoc().SelectByName("CA");
            //    List<Vector> coords = atoms.ListCoord();
            //    Matlab.PutMatrix("coords", coords.ToMatrix(false));
            //    Matlab.PutMatrix("hv_mine", GetHessCa(coords));
            //    Matlab.Execute("[hv1,hv2,hv3,hv4] = STeM(coords);");
            //    Matlab.Execute("hv_tul = hv1 + hv2 + hv3 + hv4;");
            //    Matlab.Execute("hv_diff = hv_mine - hv_tul;");
            //    Matlab.Execute("max(max(abs(hv_diff)));");
            //    // ans = 2.4158e-13
            //}


            if(_SelfTest == false)
                return;
            _SelfTest = false;

            Random rand = new Random();

            Vector[] caArray = new Vector[100];
            for(int i=0; i<caArray.Length; i++)
                caArray[i] = new double[3] { rand.NextDouble()*100, rand.NextDouble()*100, rand.NextDouble()*100 };

            HessMatrix hess0;

            hess0 = GetHessCa_matlab(caArray);

            HessMatrix hess1 = GetHessCa(caArray);
            HessMatrix hess2 = GetHessCa_v2(caArray);

            HDebug.Assert(HDebug.CheckToleranceMatrix(0.00000001, hess1-hess2));
            HDebug.Assert(HDebug.CheckToleranceMatrix(0.00000001, hess0-hess1));
            HDebug.Assert(HDebug.CheckToleranceMatrix(0.00000001, hess0-hess2));


            HDebug.Assert(SelfTest_1AAC());
        }

        public static bool SelfTest_1AAC()
        {
            Pdb.Atom[] atoms_ca = new Pdb.Atom[]
                {
                #region 1AAC text of CA atoms
                    Pdb.Atom.FromString("ATOM      2  CA  ASP A   1      25.378 -18.141  21.012  1.00 23.28           C  "),
                    Pdb.Atom.FromString("ATOM     10  CA  LYS A   2      24.390 -18.799  17.407  1.00 12.32           C  "),
                    Pdb.Atom.FromString("ATOM     19  CA  ALA A   3      25.772 -15.617  15.840  1.00 10.31           C  "),
                    Pdb.Atom.FromString("ATOM     24  CA  THR A   4      28.837 -13.442  16.439  1.00  9.47           C  "),
                    Pdb.Atom.FromString("ATOM     31  CA  ILE A   5      28.913  -9.665  15.865  1.00  7.94           C  "),
                    Pdb.Atom.FromString("ATOM     39  CA  PRO A   6      31.906  -8.243  13.886  1.00  8.02           C  "),
                    Pdb.Atom.FromString("ATOM     46  CA  SER A   7      31.049  -4.632  14.810  1.00  7.70           C  "),
                    Pdb.Atom.FromString("ATOM     52  CA  GLU A   8      28.768  -3.589  17.658  1.00 10.01           C  "),
                    Pdb.Atom.FromString("ATOM     73  CA  PRO A  10      26.932   0.412  12.498  1.00  7.09           C  "),
                    Pdb.Atom.FromString("ATOM     80  CA  PHE A  11      29.488   1.217   9.844  1.00  6.81           C  "),
                    Pdb.Atom.FromString("ATOM     91  CA  ALA A  12      29.431   2.815   6.390  1.00  6.76           C  "),
                    Pdb.Atom.FromString("ATOM     96  CA  ALA A  13      28.012   1.036   3.320  1.00  8.60           C  "),
                    Pdb.Atom.FromString("ATOM    101  CA  ALA A  14      31.392   1.654   1.659  1.00 11.18           C  "),
                    Pdb.Atom.FromString("ATOM    106  CA  GLU A  15      33.092  -0.407   4.404  1.00  9.76           C  "),
                    Pdb.Atom.FromString("ATOM    115  CA  VAL A  16      31.115  -3.590   3.763  1.00 12.85           C  "),
                    Pdb.Atom.FromString("ATOM    122  CA  ALA A  17      33.429  -6.584   3.205  1.00 20.32           C  "),
                    Pdb.Atom.FromString("ATOM    127  CA  ASP A  18      34.080  -7.894  -0.306  1.00 27.64           C  "),
                    Pdb.Atom.FromString("ATOM    135  CA  GLY A  19      31.708 -10.634  -1.313  1.00 26.92           C  "),
                    Pdb.Atom.FromString("ATOM    139  CA  ALA A  20      29.393  -9.683   1.528  1.00 19.57           C  "),
                    Pdb.Atom.FromString("ATOM    144  CA  ILE A  21      25.881 -11.238   1.583  1.00 11.70           C  "),
                    Pdb.Atom.FromString("ATOM    152  CA  VAL A  22      23.847  -8.061   1.553  1.00  7.77           C  "),
                    Pdb.Atom.FromString("ATOM    159  CA  VAL A  23      20.196  -7.501   2.410  1.00  6.37           C  "),
                    Pdb.Atom.FromString("ATOM    166  CA  ASP A  24      19.093  -3.982   1.466  1.00  6.66           C  "),
                    Pdb.Atom.FromString("ATOM    174  CA  ILE A  25      16.500  -2.121   3.513  1.00  7.21           C  "),
                    Pdb.Atom.FromString("ATOM    182  CA  ALA A  26      14.398   0.375   1.556  1.00  8.41           C  "),
                    Pdb.Atom.FromString("ATOM    187  CA  LYS A  27      10.724   1.489   1.214  1.00  9.80           C  "),
                    Pdb.Atom.FromString("ATOM    201  CA  MET A  28      10.147   0.295   4.838  1.00  8.69           C  "),
                    Pdb.Atom.FromString("ATOM    209  CA  LYS A  29      10.882  -3.362   4.004  1.00  9.09           C  "),
                    Pdb.Atom.FromString("ATOM    223  CA  TYR A  30      13.753  -5.845   4.132  1.00  6.32           C  "),
                    Pdb.Atom.FromString("ATOM    235  CA  GLU A  31      14.275  -6.364   0.381  1.00  9.00           C  "),
                    Pdb.Atom.FromString("ATOM    244  CA  THR A  32      15.144 -10.026   0.769  1.00  7.30           C  "),
                    Pdb.Atom.FromString("ATOM    251  CA  PRO A  33      12.722 -11.107   3.546  1.00  8.68           C  "),
                    Pdb.Atom.FromString("ATOM    258  CA  GLU A  34      13.757 -14.780   3.406  1.00  9.28           C  "),
                    Pdb.Atom.FromString("ATOM    267  CA  LEU A  35      17.468 -15.255   2.972  1.00  7.34           C  "),
                    Pdb.Atom.FromString("ATOM    275  CA  HIS A  36      19.181 -18.672   2.822  1.00  6.97           C  "),
                    Pdb.Atom.FromString("ATOM    285  CA  VAL A  37      22.841 -18.828   3.840  1.00  7.29           C  "),
                    Pdb.Atom.FromString("ATOM    292  CA  LYS A  38      25.361 -21.456   5.044  1.00  9.55           C  "),
                    Pdb.Atom.FromString("ATOM    306  CA  VAL A  39      26.828 -21.965   8.529  1.00  9.09           C  "),
                    Pdb.Atom.FromString("ATOM    313  CA  GLY A  40      29.717 -19.533   8.887  1.00  8.81           C  "),
                    Pdb.Atom.FromString("ATOM    317  CA  ASP A  41      28.343 -16.891   6.520  1.00  8.19           C  "),
                    Pdb.Atom.FromString("ATOM    325  CA  THR A  42      28.196 -13.227   7.468  1.00  7.95           C  "),
                    Pdb.Atom.FromString("ATOM    332  CA  VAL A  43      24.989 -11.395   6.512  1.00  6.02           C  "),
                    Pdb.Atom.FromString("ATOM    339  CA  THR A  44      25.158  -7.607   6.224  1.00  5.34           C  "),
                    Pdb.Atom.FromString("ATOM    346  CA  TRP A  45      22.054  -5.421   6.244  1.00  4.51           C  "),
                    Pdb.Atom.FromString("ATOM    360  CA  ILE A  46      22.453  -1.974   4.623  1.00  5.55           C  "),
                    Pdb.Atom.FromString("ATOM    368  CA  ASN A  47      19.890   0.774   5.248  1.00  6.52           C  "),
                    Pdb.Atom.FromString("ATOM    376  CA  ARG A  48      19.350   2.559   1.939  1.00 10.21           C  "),
                    Pdb.Atom.FromString("ATOM    387  CA  GLU A  49      16.823   5.111   3.196  1.00 12.44           C  "),
                    Pdb.Atom.FromString("ATOM    396  CA  ALA A  50      16.764   8.058   5.564  1.00 13.39           C  "),
                    Pdb.Atom.FromString("ATOM    401  CA  MET A  51      14.386   6.508   8.133  1.00 12.52           C  "),
                    Pdb.Atom.FromString("ATOM    409  CA  PRO A  52      16.509   4.566  10.725  1.00  9.20           C  "),
                    Pdb.Atom.FromString("ATOM    416  CA  HIS A  53      16.032   0.778  10.889  1.00  6.43           C  "),
                    Pdb.Atom.FromString("ATOM    426  CA  ASN A  54      17.658  -2.107  12.766  1.00  5.06           C  "),
                    Pdb.Atom.FromString("ATOM    434  CA  VAL A  55      17.296  -5.891  13.052  1.00  5.44           C  "),
                    Pdb.Atom.FromString("ATOM    441  CA  HIS A  56      15.979  -7.379  16.316  1.00  4.53           C  "),
                    Pdb.Atom.FromString("ATOM    451  CA  PHE A  57      15.920 -11.061  17.243  1.00  5.94           C  "),
                    Pdb.Atom.FromString("ATOM    462  CA  VAL A  58      13.932 -11.794  20.435  1.00  7.60           C  "),
                    Pdb.Atom.FromString("ATOM    469  CA  ALA A  59      15.316 -13.765  23.405  1.00  7.37           C  "),
                    Pdb.Atom.FromString("ATOM    474  CA  GLY A  60      16.074 -17.393  22.787  1.00  9.00           C  "),
                    Pdb.Atom.FromString("ATOM    478  CA  VAL A  61      16.521 -17.029  18.997  1.00  8.51           C  "),
                    Pdb.Atom.FromString("ATOM    485  CA  LEU A  62      20.244 -16.290  18.543  1.00  9.65           C  "),
                    Pdb.Atom.FromString("ATOM    493  CA  GLY A  63      21.156 -16.644  22.197  1.00 10.26           C  "),
                    Pdb.Atom.FromString("ATOM    497  CA  GLU A  64      19.729 -16.413  25.651  1.00  9.18           C  "),
                    Pdb.Atom.FromString("ATOM    506  CA  ALA A  65      19.129 -12.671  25.468  1.00  8.86           C  "),
                    Pdb.Atom.FromString("ATOM    511  CA  ALA A  66      17.408 -10.687  22.723  1.00  7.22           C  "),
                    Pdb.Atom.FromString("ATOM    516  CA  LEU A  67      19.808  -9.283  20.128  1.00  9.01           C  "),
                    Pdb.Atom.FromString("ATOM    524  CA  LYS A  68      18.504  -5.777  19.473  1.00 10.06           C  "),
                    Pdb.Atom.FromString("ATOM    533  CA  GLY A  69      20.805  -4.416  16.788  1.00  8.06           C  "),
                    Pdb.Atom.FromString("ATOM    537  CA  PRO A  70      21.868  -0.715  16.656  1.00  8.23           C  "),
                    Pdb.Atom.FromString("ATOM    544  CA  MET A  71      19.807   1.769  14.640  1.00  8.56           C  "),
                    Pdb.Atom.FromString("ATOM    552  CA  MET A  72      21.337   2.189  11.156  1.00  8.26           C  "),
                    Pdb.Atom.FromString("ATOM    560  CA  LYS A  73      21.086   5.638   9.601  1.00  9.59           C  "),
                    Pdb.Atom.FromString("ATOM    574  CA  LYS A  74      21.097   6.084   5.777  1.00  9.46           C  "),
                    Pdb.Atom.FromString("ATOM    588  CA  GLU A  75      23.892   4.149   4.097  1.00  9.45           C  "),
                    Pdb.Atom.FromString("ATOM    602  CA  GLN A  76      24.997   2.451   7.267  1.00  6.02           C  "),
                    Pdb.Atom.FromString("ATOM    611  CA  ALA A  77      25.416  -1.310   7.631  1.00  5.75           C  "),
                    Pdb.Atom.FromString("ATOM    616  CA  TYR A  78      25.448  -3.978  10.318  1.00  4.90           C  "),
                    Pdb.Atom.FromString("ATOM    628  CA  SER A  79      26.700  -7.612  10.117  1.00  5.13           C  "),
                    Pdb.Atom.FromString("ATOM    634  CA  LEU A  80      26.024 -10.884  11.899  1.00  5.65           C  "),
                    Pdb.Atom.FromString("ATOM    642  CA  THR A  81      27.969 -14.123  11.320  1.00  6.53           C  "),
                    Pdb.Atom.FromString("ATOM    649  CA  PHE A  82      25.693 -17.171  11.780  1.00  5.24           C  "),
                    Pdb.Atom.FromString("ATOM    660  CA  THR A  83      27.363 -20.163  13.449  1.00  6.79           C  "),
                    Pdb.Atom.FromString("ATOM    667  CA  GLU A  84      24.599 -22.785  13.726  1.00  6.27           C  "),
                    Pdb.Atom.FromString("ATOM    676  CA  ALA A  85      21.975 -24.045  11.260  1.00  7.61           C  "),
                    Pdb.Atom.FromString("ATOM    681  CA  GLY A  86      18.324 -23.142  11.886  1.00  7.35           C  "),
                    Pdb.Atom.FromString("ATOM    685  CA  THR A  87      15.680 -20.519  11.055  1.00  8.50           C  "),
                    Pdb.Atom.FromString("ATOM    692  CA  TYR A  88      16.009 -17.145  12.731  1.00  7.35           C  "),
                    Pdb.Atom.FromString("ATOM    704  CA  ASP A  89      13.191 -14.620  12.557  1.00  7.28           C  "),
                    Pdb.Atom.FromString("ATOM    712  CA  TYR A  90      13.740 -10.922  13.122  1.00  5.26           C  "),
                    Pdb.Atom.FromString("ATOM    724  CA  HIS A  91      11.902  -7.619  12.935  1.00  4.80           C  "),
                    Pdb.Atom.FromString("ATOM    734  CA  CYS A  92      12.663  -3.894  13.049  1.00  5.44           C  "),
                    Pdb.Atom.FromString("ATOM    740  CA  THR A  93      12.228  -2.579  16.647  1.00  6.60           C  "),
                    Pdb.Atom.FromString("ATOM    747  CA  PRO A  94      10.460   0.815  15.821  1.00  7.51           C  "),
                    Pdb.Atom.FromString("ATOM    754  CA  HIS A  95       8.594  -0.728  12.857  1.00  7.46           C  "),
                    Pdb.Atom.FromString("ATOM    764  CA  PRO A  96       7.518  -4.286  13.932  1.00  7.59           C  "),
                    Pdb.Atom.FromString("ATOM    771  CA  PHE A  97       5.480  -4.778  10.739  1.00  7.30           C  "),
                    Pdb.Atom.FromString("ATOM    782  CA  MET A  98       8.923  -5.104   8.994  1.00  6.57           C  "),
                    Pdb.Atom.FromString("ATOM    790  CA  ARG A  99       9.707  -8.821   9.391  1.00  7.91           C  "),
                    Pdb.Atom.FromString("ATOM    808  CA  GLY A 100      12.343 -11.105   7.827  1.00  6.39           C  "),
                    Pdb.Atom.FromString("ATOM    812  CA  LYS A 101      14.085 -14.438   8.459  1.00  7.31           C  "),
                    Pdb.Atom.FromString("ATOM    826  CA  VAL A 102      17.523 -15.900   7.870  1.00  6.58           C  "),
                    Pdb.Atom.FromString("ATOM    833  CA  VAL A 103      17.552 -19.658   7.157  1.00  7.20           C  "),
                    Pdb.Atom.FromString("ATOM    840  CA  VAL A 104      21.061 -21.012   7.897  1.00  7.28           C  "),
                    Pdb.Atom.FromString("ATOM    847  CA  GLU A 105      21.699 -24.347   6.221  1.00 13.74           C  "),
            #endregion
                };
            Pdb.Atom[] atoms_r6 = new Pdb.Atom[]
                {
                    Pdb.Atom.FromString("ATOM     42  CB  PRO A   6      31.929  -8.271  12.386  1.00  7.98           C  "),
                    Pdb.Atom.FromString("ATOM     43  CG  PRO A   6      30.972  -9.378  12.022  1.00  9.48           C  "),
                    Pdb.Atom.FromString("ATOM     44  CD  PRO A   6      29.880  -9.313  13.099  1.00  8.27           C  "),
                };

            {
                HessMatrix hess = GetHessCa(atoms_ca.ListCoord());

                InfoPack extra = new InfoPack();
                //Vector bfactors = ENM.BFactorFromHessian(hess, null, 6, extra);
                Vector bfactors  = Hess.GetBFactor        (hess, null, 6, extra);
                Vector eigvals = (double[])extra["eigenvalues"];
                int[] idxsorted_eigvals = eigvals.ToArray().HAbs().HIdxSorted();
                for(int i = 0; i < 6; i++) eigvals[idxsorted_eigvals[i]] = 0;
                for(int i = 0; i < eigvals.Size; i++)
                    if(eigvals[i] < 0)
                    {
                        // return false, if there exists negative eigenvalue
                        HDebug.Assert(false);
                        return false;
                    }
            }
            {
                List<Pdb.Atom> atoms = new List<Pdb.Atom>();
                atoms.AddRange(atoms_ca);
                atoms.AddRange(atoms_r6);
                HDebug.Assert(atoms[5].resSeq == 6);
            
                List<int> idxs_ca = new List<int>(); for(int i=0; i<atoms_ca.Length; i++) idxs_ca.Add(i);
                List<int> idxs_r6 = new List<int>(); for(int i=atoms_ca.Length; i<atoms.Count; i++) idxs_r6.Add(i); idxs_r6.InsertRange(0, new int[]{3,4,5});
            
                List<Tuple<int, int>> list12 = new List<Tuple<int, int>>();
                for(int i=1; i<idxs_ca.Count; i++) list12.Add(new Tuple<int, int>(idxs_ca[i-1], idxs_ca[i]));
                for(int i=3; i<idxs_r6.Count; i++) list12.Add(new Tuple<int, int>(idxs_r6[i-1], idxs_r6[i]));
            
                List<Tuple<int, int, int>> list123 = new List<Tuple<int, int, int>>();
                for(int i=2; i<idxs_ca.Count; i++) list123.Add(new Tuple<int, int, int>(idxs_ca[i-2], idxs_ca[i-1], idxs_ca[i]));
                for(int i=3; i<idxs_r6.Count; i++) list123.Add(new Tuple<int, int, int>(idxs_r6[i-2], idxs_r6[i-1], idxs_r6[i]));
            
                List<Tuple<int, int, int, int>> list1234 = new List<Tuple<int, int, int, int>>();
                for(int i=3; i<idxs_ca.Count; i++) list1234.Add(new Tuple<int, int, int, int>(idxs_ca[i-3], idxs_ca[i-2], idxs_ca[i-1], idxs_ca[i]));
                for(int i=3; i<idxs_r6.Count; i++) list1234.Add(new Tuple<int, int, int, int>(idxs_r6[i-3], idxs_r6[i-2], idxs_r6[i-1], idxs_r6[i]));

                List<Tuple<int, int>> listNonbond = new List<Tuple<int, int>>();
                for(int i=0; i<idxs_ca.Count; i++) for(int j=i+4; j<idxs_ca.Count; j++) listNonbond.Add(new Tuple<int, int>(idxs_ca[i], idxs_ca[j]));
                for(int i=0; i<idxs_r6.Count; i++) for(int j=i+4; j<idxs_r6.Count; j++) listNonbond.Add(new Tuple<int, int>(idxs_r6[i], idxs_r6[j]));

                List<Vector> coords = atoms.ListCoord();
                HessMatrix hessian = HessMatrixSparse.ZerosSparse(atoms.Count*3, atoms.Count*3);
            
                double Epsilon=0.36;
                double K_r=100*Epsilon;
                double K_theta=20*Epsilon;
                double K_phi1=1*Epsilon;
                double K_phi3=0.5*Epsilon;
                for(int i=0; i<list12.Count; i++)
                    FirstTerm(coords, K_r, hessian, list12[i].Item1, list12[i].Item2);
                for(int i=0; i<list123.Count; i++)
                    SecondTerm(coords, K_theta, hessian, list123[i].Item1, list123[i].Item2, list123[i].Item3);
                for(int i=0; i<list1234.Count; i++)
                    ThirdTerm(coords, K_phi1, K_phi3, hessian, list1234[i].Item1, list1234[i].Item2, list1234[i].Item3, list1234[i].Item4);
                for(int i=0; i<listNonbond.Count; i++)
                    FourthTerm(coords, Epsilon, hessian, listNonbond[i].Item1, listNonbond[i].Item2);

                InfoPack extra = new InfoPack();
                //Vector bfactors = ENM.BFactorFromHessian(hessian, null, 6, extra);
                Vector bfactors = Hess.GetBFactor(hessian, null, 6, extra);
                Vector eigvals = (double[])extra["eigenvalues"];
                int[] idxsorted_eigvals = eigvals.ToArray().HAbs().HIdxSorted();
                for(int i = 0; i < 6; i++) eigvals[idxsorted_eigvals[i]] = 0;
                for(int i = 0; i < eigvals.Size; i++)
                    if(eigvals[i] < 0)
                    {
                        // return false, if there exists negative eigenvalue
                        HDebug.Assert(false);
                        return false;
                    }
            }
            //{
            //    Debug.Assert(atoms_ca[5].resSeq == 6);
            //    Pdb.Atom[] atoms_resi6 = new Pdb.Atom[]
            //        {
            //            atoms_ca[5],
            //            Pdb.Atom.FromString("ATOM     42  CB  PRO A   6      31.929  -8.271  12.386  1.00  7.98           C  "),
            //            Pdb.Atom.FromString("ATOM     43  CG  PRO A   6      30.972  -9.378  12.022  1.00  9.48           C  "),
            //            Pdb.Atom.FromString("ATOM     44  CD  PRO A   6      29.880  -9.313  13.099  1.00  8.27           C  "),
            //        };
            //    Matrix hess_ca = GetHessCa(atoms_ca.ListCoord());
            //    Matrix hess_resi6 = GetHessCa(atoms_resi6.ListCoord());
            //
            //    int size_ca = atoms_ca.Length;
            //    int size_r6 = atoms_resi6.Length - 1;
            //    Matrix hess = new double[(size_ca+size_r6)*3,(size_ca+size_r6)*3];
            //    for(int c = 0; c < hess.ColSize; c++)
            //        for(int r = 0; r < hess.RowSize; r++)
            //            hess[c, r] = double.NaN;
            //    for(int c = 0; c < hess_ca.ColSize; c++)
            //        for(int r = 0; r < hess_ca.RowSize; r++)
            //            hess[c, r] = hess_ca[c, r];
            //    for(int c = 0; c < hess_resi6.ColSize; c++)
            //        for(int r = 0; r < hess_resi6.RowSize; r++)
            //        {
            //                 if(c <  3 && r <  3) { int c0=5      *3+ c   ; int r0=5      *3+ r   ; Debug.Assert(double.IsNaN(hess[c0, r0]) ==false); hess[c0, r0] = hess_resi6[c, r]; }
            //            else if(c <  3 && r >= 3) { int c0=5      *3+ c   ; int r0=size_ca*3+(r-3); Debug.Assert(double.IsNaN(hess[c0, r0]) == true); hess[c0, r0] = hess_resi6[c, r]; }
            //            else if(c >= 3 && r <  3) { int c0=size_ca*3+(c-3); int r0=5      *3+ r   ; Debug.Assert(double.IsNaN(hess[c0, r0]) == true); hess[c0, r0] = hess_resi6[c, r]; }
            //            else if(c >= 3 && r >= 3) { int c0=size_ca*3+(c-3); int r0=size_ca*3+(r-3); Debug.Assert(double.IsNaN(hess[c0, r0]) == true); hess[c0, r0] = hess_resi6[c, r]; }
            //            else Debug.Assert(false);
            //        }
            //    for(int j = size_ca * 3; j < (size_ca + size_r6) * 3; j++)
            //    {
            //        for(int i = 0; i < 5*3; i++)
            //        {
            //            Debug.Assert(double.IsNaN(hess[i, j]) == true); hess[i, j] = 0;
            //            Debug.Assert(double.IsNaN(hess[j, i]) == true); hess[j, i] = 0;
            //        }
            //        for(int i = 6*3; i < size_ca * 3; i++)
            //        {
            //            Debug.Assert(double.IsNaN(hess[i, j]) == true); hess[i, j] = 0;
            //            Debug.Assert(double.IsNaN(hess[j, i]) == true); hess[j, i] = 0;
            //        }
            //    }
            //    for(int c = 0; c < hess.ColSize; c++)
            //        for(int r = 0; r < hess.RowSize; r++)
            //            Debug.Assert(double.IsNaN(hess[c, r]) == false);
            //
            //    InfoPack extra = new InfoPack();
            //    Vector bfactors = ENM.BFactorFromHessian(hess, null, 6, extra);
            //    Vector eigvals = (double[])extra["eigenvalues"];
            //    int[] idxsorted_eigvals = eigvals.ToArray().Abs().IdxSorted();
            //    for(int i = 0; i < 6; i++) eigvals[idxsorted_eigvals[i]] = 0;
            //    for(int i = 0; i < eigvals.Size; i++)
            //        if(eigvals[i] < 0)
            //        {
            //            // return false, if there exists negative eigenvalue
            //            Debug.Assert(false);
            //            return false;
            //        }
            //}

            return true;
        }
    }
}
}
