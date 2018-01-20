using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HTLib2;
using HTLib2.Bioinfo;

namespace HTLib2.Bioinfo
{
    public partial class Align
    {
        public class MinIterAnisou : IMinAlign
        {
            public static void Align( List<Vector> coords1
                                    , ref List<Vector> coords2
                                    , int maxiteration = int.MaxValue
                                    , HPack<List<Trans3[]>> outTrajTrans = null
                                    )
            {
                List<Vector>[] ensemble = new List<Vector>[]
                {
                    coords1.HClone(),
                    coords2.HClone()
                };
                Align(ref ensemble, maxiteration: maxiteration, outTrajTrans: outTrajTrans);
                Trans3 trans = MinRMSD.GetTrans(coords1, ensemble[0]);
                Vector[] ensemble0 = trans.GetTransformed(ensemble[0]).ToArray();
                Vector[] ensemble1 = trans.GetTransformed(ensemble[1]).ToArray();
                coords2 = new List<Vector>(ensemble1);
            }

            public static void Align( ref List<Vector>[] ensemble
                                    , int maxiteration = int.MaxValue
                                    , HPack<List<Trans3[]>> outTrajTrans = null
                                    )
            {
                List<Vector>[] lensemble = ensemble.HClone<List<Vector>>();

                int size = lensemble[0].Count;
                for(int i=1; i<lensemble.Length; i++)
                    HDebug.Assert(size == lensemble[i].Count);

                // initial alignment using RMSD
                Trans3[] transs = new Trans3[lensemble.Length];
                transs[0] = Trans3.UnitTrans;

                Vector[] coords  = new Vector[size];
                Anisou[] anisous = new Anisou[size];
                double anisou_eigvalthres = 0.00001*0.00001;
                double[]         bfactors = new double[size];
                {
                    // set ensemble[0] as the initial mean structure
                    for(int i=0; i<size; i++)
                        coords[i] = lensemble[0][0].Clone();
                    // set anisou as sphere (with radii as one)
                    Anisou U0 = Anisou.FromMatrix(new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } });
                    for(int i=0; i<anisous.Length; i++)
                        anisous[i] = U0;
                    // set bfactor as one
                    for(int i=0; i<bfactors.Length; i++)
                        bfactors[i] = 1;
                }

                //List<double>[] bfactorss = new List<double>[ensemble.Length];
                //for(int i=0; i<bfactorss.Length; i++)
                //    bfactorss[i] = new List<double>(bfactors);
                //Pdb.ToFile("ensemble.000.pdb", null, lensemble, bfactorss);

                System.Console.WriteLine("begin iterative alignment");

                double move2 = double.PositiveInfinity;
                //double thres = 0.00001;// *size * lensemble.Length;
                double thres = 0.00001;// *size * lensemble.Length;
                //List<Pdb.Atom> atoms = null;
                //while(thres < move2)
                for(int iter=0; iter<maxiteration; iter++)
                {
                    transs = new Trans3[ensemble.Length];
                    move2 = Align(lensemble, coords, anisous, transs);

                    if(outTrajTrans != null)
                        outTrajTrans.value.Add(transs);

                    System.Console.WriteLine(iter+"th alignment done: (move "+move2+" Å)");
                    if(move2 < thres)
                        break;

                    DetermineMeanConf(lensemble, coords);
                    DetermineThrmlFluc(lensemble, coords, anisous, null, anisou_eigvalthres);
                }

                ensemble = lensemble;
            }

            public static void WriteResult(string pdbid, List<Pdb.Atom>[] ensemble, List<Trans3[]> trajtrans, string pathroot)
            {
                List<Pdb.Atom> atoms = ensemble[0];

                int size = ensemble[0].Count;
                double bfactor0_min = double.PositiveInfinity;
                double bfactor0_max = double.NegativeInfinity;
                double anisou_eigvalthres = 0.00001*0.00001;
                {
                    Trans3[] trans = trajtrans.First();
                    List<Vector>[] lensemble = new List<Vector>[ensemble.Length];
                    for(int i=0; i<ensemble.Length; i++)
                        lensemble[i] = new List<Vector>(trans[i].GetTransformed(ensemble[i].ListCoord()));
                    Vector[] coords   = new Vector[size];
                    Anisou[] anisous  = new Anisou[size];
                    double[] bfactors = new double[size];
                    DetermineMeanConf(lensemble, coords);
                    DetermineThrmlFluc(lensemble, coords, anisous, bfactors, anisou_eigvalthres);
                    List<double>[] bfactorss = new List<double>[ensemble.Length];
                    for(int i=0; i<bfactorss.Length; i++)
                        bfactorss[i] = new List<double>(bfactors);

                    List<int> idxCa = atoms.IndexOfAtoms(atoms.SelectByName("CA"));
                    bfactor0_min = bfactors.HSelectByIndex(idxCa).HMinNth(idxCa.Count/10);
                    bfactor0_max = bfactors.HSelectByIndex(idxCa).HMaxNth(idxCa.Count/10);
                
                    Pdb.ToFile(pathroot + "ensemble.000.pdb", atoms, lensemble, bfactorss);
                    Pdb.ToFile(pathroot + "ensemble.000.anisou.1.pdb",     atoms, coords, anisous:anisous.GetUs(1    ), bfactors:bfactors, append: false);
                    Pdb.ToFile(pathroot + "ensemble.000.anisou.10.pdb",    atoms, coords, anisous:anisous.GetUs(10   ), bfactors:bfactors, append: false);
                    Pdb.ToFile(pathroot + "ensemble.000.anisou.100.pdb",   atoms, coords, anisous:anisous.GetUs(100  ), bfactors:bfactors, append: false);
                    Pdb.ToFile(pathroot + "ensemble.000.anisou.1000.pdb",  atoms, coords, anisous:anisous.GetUs(1000 ), bfactors:bfactors, append: false);
                    Pdb.ToFile(pathroot + "ensemble.000.anisou.10000.pdb", atoms, coords, anisous:anisous.GetUs(10000), bfactors:bfactors, append: false);
                }
                int iter = trajtrans.Count-1;
                double bfactorn_min = double.PositiveInfinity;
                double bfactorn_max = double.NegativeInfinity;
                {
                    Trans3[] trans = new Trans3[ensemble.Length];
                    for(int j=0; j<trans.Length; j++) trans[j] = Trans3.UnitTrans;
                    for(int i=0; i<trajtrans.Count; i++)
                    {
                        for(int j=0; j<trans.Length; j++)
                            trans[j] = Trans3.AppendTrans(trans[j], trajtrans[i][j]);
                    }

                    List<Vector>[] lensemble = new List<Vector>[ensemble.Length];
                    for(int i=0; i<ensemble.Length; i++)
                        lensemble[i] = new List<Vector>(trans[i].GetTransformed(ensemble[i].ListCoord()));
                    Vector[] coords   = new Vector[size];
                    Anisou[] anisous  = new Anisou[size];
                    double[] bfactors = new double[size];
                    DetermineMeanConf(lensemble, coords);
                    DetermineThrmlFluc(lensemble, coords, anisous, bfactors, anisou_eigvalthres);
                    List<double>[] bfactorss = new List<double>[ensemble.Length];
                    for(int i=0; i<bfactorss.Length; i++)
                        bfactorss[i] = new List<double>(bfactors);

                    List<int> idxCa = atoms.IndexOfAtoms(atoms.SelectByName("CA"));
                    bfactorn_min = bfactors.HSelectByIndex(idxCa).HMinNth(idxCa.Count/10);
                    bfactorn_max = bfactors.HSelectByIndex(idxCa).HMaxNth(idxCa.Count/10);
                    
                    Pdb.ToFile((pathroot + "ensemble.{finl}.pdb"             ).Replace("{finl}",iter.ToString("000")), atoms, lensemble, bfactorss);
                    Pdb.ToFile((pathroot + "ensemble.{finl}.anisou.1.pdb"    ).Replace("{finl}",iter.ToString("000")), atoms, coords, anisous:anisous.GetUs(1    ), bfactors:bfactors, append: false);
                    Pdb.ToFile((pathroot + "ensemble.{finl}.anisou.10.pdb"   ).Replace("{finl}",iter.ToString("000")), atoms, coords, anisous:anisous.GetUs(10   ), bfactors:bfactors, append: false);
                    Pdb.ToFile((pathroot + "ensemble.{finl}.anisou.100.pdb"  ).Replace("{finl}",iter.ToString("000")), atoms, coords, anisous:anisous.GetUs(100  ), bfactors:bfactors, append: false);
                    Pdb.ToFile((pathroot + "ensemble.{finl}.anisou.1000.pdb" ).Replace("{finl}",iter.ToString("000")), atoms, coords, anisous:anisous.GetUs(1000 ), bfactors:bfactors, append: false);
                    Pdb.ToFile((pathroot + "ensemble.{finl}.anisou.10000.pdb").Replace("{finl}",iter.ToString("000")), atoms, coords, anisous:anisous.GetUs(10000), bfactors:bfactors, append: false);
                }
                {
                    string[] lines = new string[]{   @"load ensemble.{init}.pdb,              {init}.align"
                                                    ,@"load ensemble.{init}.anisou.10.pdb,    {init}.anisou.10"
                                                    ,@"load ensemble.{init}.anisou.100.pdb,   {init}.anisou.100"
                                                    ,@"load ensemble.{init}.anisou.1000.pdb,  {init}.anisou.1000"
                                                    ,@"load ensemble.{init}.anisou.10000.pdb, {init}.anisou.10000"
                                                    ,@"load ensemble.{finl}.pdb,              {finl}.align"
                                                    ,@"load ensemble.{finl}.anisou.10.pdb,    {finl}.anisou.10"
                                                    ,@"load ensemble.{finl}.anisou.100.pdb,   {finl}.anisou.100"
                                                    ,@"load ensemble.{finl}.anisou.1000.pdb,  {finl}.anisou.1000"
                                                    ,@"load ensemble.{finl}.anisou.10000.pdb, {finl}.anisou.10000"
                                                    ,@"load ..\theseus\{pdbid}_theseus_sup.pdb, theseus"
                                                    ,@"align theseus, {finl}.align"
                                                    ,@"orient {finl}.align"
                                                    ,@"zoom {finl}.align"
                                                    ,@""
                                                    ,@"select sele, name ca and ({init}.anisou.* or {finl}.anisou.*)"
                                                    ,@"hide everything"
                                                    ,@"show ellipsoids, sele"
                                                    ,@"delete sele"
                                                    ,@""
                                                    ,@"cartoon loop"
                                                    ,@"set cartoon_loop_radius, 0.1"
                                                    ,@"set all_states"
                                                    ,@"show cartoon, {init}.align or {finl}.align"
                                                    ,@"show cartoon, theseus"
                                                    ,@"disable all"
                                                    ,@""
                                                    ,@"spectrum b, rainbow, minimum={bfactor-min}, maximum={bfactor-max}"
                                                    ,@"spectrum b, rainbow, minimum=0, maximum=50"
                                                    ,@"show line"
                                                    ,@"enable {init}.align; png ..\{pdbid}-{init}-line.png;  disable {init}.align;"
                                                    ,@"enable {finl}.align; png ..\{pdbid}-{finl}-line.png;  disable {finl}.align;"
                                                    ,@"hide line"
                                                    ,@"enable {init}.align; png ..\{pdbid}-{init}.png;  disable {init}.align;"
                                                    ,@"enable {finl}.align; png ..\{pdbid}-{finl}.png;  disable {finl}.align;"
                                                    ,@"enable theseus;      png ..\{pdbid}-theseus.png; disable theseus;"
                                                    ,@"enable {init}.align"
                                                    ,@"enable {finl}.align"
                                                    ,@"enable theseus"
                                                 };
                    lines = lines.HReplace("{init}", "000");
                    lines = lines.HReplace("{finl}", iter.ToString("000"));
                    lines = lines.HReplace("{pdbid}", pdbid);
                    lines = lines.HReplace("{bfactor-min}", bfactorn_min.ToString());
                    lines = lines.HReplace("{bfactor-max}", bfactorn_max.ToString());

                    string pmlpath = pathroot + "align.pml";
                    HFile.WriteAllLines(pmlpath, lines);

                    {
                        HFile.AppendAllLines(pmlpath, "quit");
                        string curdirectory = System.Environment.CurrentDirectory;
                        string pmldirectory = pmlpath.Substring(0,pmlpath.LastIndexOf('\\')+1);
                        System.Environment.CurrentDirectory = pmldirectory;
                        System.Diagnostics.ProcessStartInfo pymolStartInfo = new System.Diagnostics.ProcessStartInfo(@"C:\Program Files (x86)\PyMOL\PyMOL\PymolWin.exe ", "\""+pmlpath+"\"");
                        pymolStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                        System.Diagnostics.Process pymol = System.Diagnostics.Process.Start(pymolStartInfo);
                        //pymol.win
                        pymol.WaitForExit();
                        System.Environment.CurrentDirectory = curdirectory;
                        HFile.WriteAllLines(pmlpath, lines);
                    }
                    {
                        List<string> lines_notheseus = new List<string>();
                        for(int i=0; i<lines.Length; i++)
                            if(lines[i].Contains("theseus") == false)
                                lines_notheseus.Add(lines[i]);
                        HFile.WriteAllLines(pathroot + "align.notheseus.pml", lines);
                    }
                }
            }

            private static double Align(List<Vector>[] ensemble, Vector[] meancoords, Anisou[] meananisous, Trans3[] outTranss)
            {
                double[] move2s = new double[ensemble.Length];
                System.Threading.Tasks.Parallel.For(0, ensemble.Length, delegate(int i)
                //for(int i=0; i<ensemble.Length; i++)
                {
                    HPack<List<Vector>> optMoveConf = new HPack<List<Vector>>();
                    HPack<Trans3> outTrans = new HPack<Trans3>();
                    MinAnisou.Align(meancoords, meananisous, ref ensemble[i], optMoveConf, outTrans:outTrans);
                    if(outTranss != null)
                        outTranss[i] = outTrans.value;
                    move2s[i] = optMoveConf.value.Dist2().Average();
                }
                );
                double move2 = move2s.Average();
                return move2;
            }

            public static void Align( ref List<Pdb.Atom>[] ensemble
                                    , int maxiteration=int.MaxValue
                                    , HPack<List<Trans3[]>> outTrajTrans = null
                                    )
            {
                List<Vector>[] coordss = new List<Vector>[ensemble.Length];
                for(int i=0; i<ensemble.Length; i++)
                    coordss[i] = ensemble[i].ListCoord();
                Align(ref coordss, maxiteration: maxiteration, outTrajTrans: outTrajTrans);
                ensemble = ensemble.HClone<List<Pdb.Atom>>();
                for(int i=0; i<ensemble.Length; i++)
                    ensemble[i] = ensemble[i].CloneByUpdateCoord(coordss[i]);
            }

            public static void DetermineMeanConf(List<Vector>[] ensemble, Vector[] meanconf)
            {
                int size = ensemble[0].Count;

                HDebug.AssertNotNull(ensemble);
                System.Threading.Tasks.Parallel.For(0, size, delegate(int ai)
                //for(int i=0; i<size; i++)
                {
                    Vector mean = new double[3];
                    {   // mean coord of atom i
                        for(int ei=0; ei<ensemble.Length; ei++)
                            mean += ensemble[ei][ai];
                        mean /= ensemble.Length;
                    }
                    meanconf[ai] = mean;
                }
                );
            }
            public static void DetermineThrmlFluc(List<Vector>[] ensemble, Vector[] meanconf, Anisou[] anisous, double[] bfactors, double eigvalthres)
            {
                #region old code
                //int size = ensemble[0].Count;
                //
                //Debug.AssertNotNull(ensemble);
                //Parallel.For(0, size, delegate(int ai)
                ////for(int i=0; i<size; i++)
                //{
                //    Vector mean = meanconf[ai];
                //    Matrix cov = new double[3, 3];
                //    {
                //        for(int ei=0; ei<ensemble.Length; ei++)
                //        {
                //            Vector vec = ensemble[ei][ai] - mean;
                //            cov += Matrix.VVt(vec, vec);
                //        }
                //    }
                //    anisous[ai] = Anisou.FromMatrix(cov);
                //    if(anisous[ai].eigvals[0] < eigvalthres) anisous[ai].eigvals[0] = 0;
                //    if(anisous[ai].eigvals[1] < eigvalthres) anisous[ai].eigvals[1] = 0;
                //    if(anisous[ai].eigvals[2] < eigvalthres) anisous[ai].eigvals[2] = 0;
                //    Debug.Assert((anisous[ai].eigvals[0] == 0 && anisous[ai].eigvals[1] == 0  && anisous[ai].eigvals[2] == 0) == false);
                //    if(bfactors != null) bfactors[ai] = (cov[0, 0] + cov[1, 1] + cov[2, 2]);
                //}
                //);
                #endregion

                List<Vector[]> lensemble = new List<Vector[]>();
                for(int i=0; i<ensemble.Length; i++)
                    lensemble.Add(ensemble[i].ToArray());

                int size = ensemble[0].Count;
                Anisou[] lanisous = Anisou.FromCoords(lensemble, meanconf: meanconf, eigvalthres: eigvalthres);

                for(int i=0; i<size; i++)
                {
                    anisous[i] = lanisous[i];
                    if(bfactors != null)
                        bfactors[i] = anisous[i].bfactor;
                }
            }

            //public static void DetermineAnisous(Vector[] coords, out Vector mean, out Bioinfo.Anisou anisou)
            //{
            //    mean = coords.Mean();
            //    Matrix cov = new double[3, 3];
            //    for(int i=0; i<coords.Length; i++)
            //    {
            //        Vector vec = coords[i] - mean;
            //        cov += Matrix.VVt(vec, vec);
            //    }
            //    anisou = Bioinfo.GetAnisou(cov);
            //}
        }
    }
}

/*
namespace ConfAlign
{
    public partial class Program
    {
        public class MinIterAnisou : IMinAlign
        {
            public static void Align(ref List<Vector>[] ensemble, List<Pdb.Atom> atoms, string pdbid, string pathtraj, int maxiteration=int.MaxValue)
            {
                List<Vector>[] lensemble = ensemble.Clone<List<Vector>>();

                int size = lensemble[0].Count;
                for(int i=1; i<lensemble.Length; i++)
                    Debug.Assert(size == lensemble[i].Count);

                // initial alignment using RMSD
                for(int i=1; i<lensemble.Length; i++)
                    MinRMSD.Align(lensemble[0], ref lensemble[i]);
                Vector[]         coords  = new Vector[size];
                Bioinfo.Anisou[] anisous = new Bioinfo.Anisou[size];
                DetermineAnisous(lensemble, coords, anisous);
                {
                    Pdb.ToFile(pathtraj + ".000.pdb",              atoms, lensemble);
                    Pdb.ToFile(pathtraj + ".000.anisou.1.pdb",     atoms, coords, anisous: anisous.GetUs(1    ), append: false);
                    Pdb.ToFile(pathtraj + ".000.anisou.10.pdb",    atoms, coords, anisous: anisous.GetUs(10   ), append: false);
                    Pdb.ToFile(pathtraj + ".000.anisou.100.pdb",   atoms, coords, anisous: anisous.GetUs(100  ), append: false);
                    Pdb.ToFile(pathtraj + ".000.anisou.1000.pdb",  atoms, coords, anisous: anisous.GetUs(1000 ), append: false);
                    Pdb.ToFile(pathtraj + ".000.anisou.10000.pdb", atoms, coords, anisous: anisous.GetUs(10000), append: false);
                }

                System.Console.WriteLine("initial alignment done");

                double move2 = double.PositiveInfinity;
                double thres = 0.0001;// *size * lensemble.Length;
                int iter = 0;
                while(thres < move2)
                {
                    if(iter+1 > maxiteration)
                        break;
                    iter++;

                    move2 = Align(lensemble, coords, anisous);
                    DetermineAnisous(lensemble, coords, anisous);

                    System.Console.WriteLine(iter+"th alignment done");
                }

                {
                    Pdb.ToFile((pathtraj + ".{finl}.pdb"             ).Replace("{finl}",iter.ToString("000")), atoms, lensemble);
                    Pdb.ToFile((pathtraj + ".{finl}.anisou.1.pdb"    ).Replace("{finl}",iter.ToString("000")), atoms, coords, anisous: anisous.GetUs(1), append: false);
                    Pdb.ToFile((pathtraj + ".{finl}.anisou.10.pdb"   ).Replace("{finl}",iter.ToString("000")), atoms, coords, anisous: anisous.GetUs(10   ), append: false);
                    Pdb.ToFile((pathtraj + ".{finl}.anisou.100.pdb"  ).Replace("{finl}",iter.ToString("000")), atoms, coords, anisous: anisous.GetUs(100  ), append: false);
                    Pdb.ToFile((pathtraj + ".{finl}.anisou.1000.pdb" ).Replace("{finl}",iter.ToString("000")), atoms, coords, anisous: anisous.GetUs(1000 ), append: false);
                    Pdb.ToFile((pathtraj + ".{finl}.anisou.10000.pdb").Replace("{finl}",iter.ToString("000")), atoms, coords, anisous: anisous.GetUs(10000), append: false);
                }
                {
                    string[] lines = new string[]{   @"load ensemble.{init}.pdb,             {init}.align"
                                                    ,@"load ensemble.{init}.anisou.100.pdb,  {init}.anisou.100"
                                                    ,@"load ensemble.{init}.anisou.1000.pdb, {init}.anisou.1000"
                                                    ,@"load ensemble.{finl}.pdb,             {finl}.align"
                                                    ,@"load ensemble.{finl}.anisou.100.pdb,  {finl}.anisou.100"
                                                    ,@"load ensemble.{finl}.anisou.1000.pdb, {finl}.anisou.1000"
                                                    ,@"load ..\theseus\{pdbid}_theseus_sup.pdb, theseus"
                                                    ,@"align theseus, {finl}.align"
                                                    ,@"orient {finl}.align"
                                                    ,@"zoom {finl}.align"
                                                    ,@""
                                                    ,@"select sele, name ca and ({init}.anisou.* or {finl}.anisou.*)"
                                                    ,@"hide everything"
                                                    ,@"show ellipsoids, sele"
                                                    ,@"delete sele"
                                                    ,@""
                                                    ,@"cartoon loop"
                                                    ,@"set cartoon_loop_radius, 0.1"
                                                    ,@"set all_states"
                                                    ,@"show cartoon, {init}.align or {finl}.align or theseus"
                                                    ,@"disable all"
                                                    ,@""
                                                    ,@"enable {init}.align; png ..\{pdbid}.{init}.png;  disable {init}.align;"
                                                    ,@"enable {finl}.align; png ..\{pdbid}.{finl}.png;  disable {finl}.align;"
                                                    ,@"enable theseus;      png ..\{pdbid}.theseus.png; disable theseus;"
                                                    ,@"enable {init}.align"
                                                    ,@"enable {finl}.align"
                                                    ,@"enable theseus"
                                                 };
                    lines = lines.Replace("{init}", "000");
                    lines = lines.Replace("{finl}", iter.ToString("000"));
                    lines = lines.Replace("{pdbid}", pdbid);

                    string pmlpath = pathtraj + ".align.pml";
                    File.WriteAllLines(pmlpath, lines);

                    {
                        File.AppendAllLines(pmlpath, "quit");
                        string curdirectory = System.Environment.CurrentDirectory;
                        string pmldirectory = pmlpath.Substring(0,pmlpath.LastIndexOf('\\')+1);
                        System.Environment.CurrentDirectory = pmldirectory;
                        System.Diagnostics.Process pymol = System.Diagnostics.Process.Start(@"C:\Program Files (x86)\PyMOL\PyMOL\PymolWin.exe ", "\""+pmlpath+"\"");
                        pymol.WaitForExit();
                        System.Environment.CurrentDirectory = curdirectory;
                        File.WriteAllLines(pmlpath, lines);
                    }
                }
                ensemble = lensemble;
                //System.Diagnostics.Process.Start(@"C:\Program Files (x86)\PyMOL\PyMOL\PymolWin.exe "+pathtraj + ".align.pml");
            }

            private static double Align(List<Vector>[] ensemble, Vector[] meancoords, Bioinfo.Anisou[] meananisous)
            {
                double[] move2s = new double[ensemble.Length];
                Parallel.For(0, ensemble.Length, delegate(int i)
                //for(int i=0; i<ensemble.Length; i++)
                {
                    Pack<List<Vector>> optMoveConf = new Pack<List<Vector>>();
                    MinAnisou.Align(meancoords, meananisous, ref ensemble[i], optMoveConf);
                    move2s[i] = optMoveConf.value.Dist2().Average();
                }
                );
                double move2 = move2s.Average();
                return move2;
            }

            public static void Align(ref List<Pdb.Atom>[] ensemble, string pdbid, string pathtraj, int maxiteration=int.MaxValue)
            {
                List<Vector>[] coordss = new List<Vector>[ensemble.Length];
                for(int i=0; i<ensemble.Length; i++)
                    coordss[i] = ensemble[i].ListCoord();
                Align(ref coordss, ensemble[0], pdbid, pathtraj, maxiteration);
                ensemble = ensemble.Clone<List<Pdb.Atom>>();
                for(int i=0; i<ensemble.Length; i++)
                    ensemble[i] = ensemble[i].UpdateCoord(coordss[i]);
            }
            private static void DetermineAnisous(List<Vector>[] ensemble, Vector[] coords, Bioinfo.Anisou[] anisous)
            {
                int size = ensemble[0].Count;

                Parallel.For(0, size, delegate(int ai)
                //for(int i=0; i<size; i++)
                {
                    Vector mean = new double[3];
                    {   // mean coord of atom i
                        for(int ei=0; ei<ensemble.Length; ei++)
                            mean += ensemble[ei][ai];
                        mean /= ensemble.Length;
                    }
                    Matrix cov = new double[3, 3];
                    {
                        for(int ei=0; ei<ensemble.Length; ei++)
                        {
                            Vector vec = ensemble[ei][ai] - mean;
                            cov += Matrix.VVt(vec, vec);
                        }
                    }
                    coords[ai] = mean;
                    anisous[ai] = Bioinfo.GetAnisou(cov);
                }
                );
            }
            public static void DetermineAnisous(Vector[] coords, out Vector mean, out Bioinfo.Anisou anisou)
            {
                mean = coords.Mean();
                Matrix cov = new double[3, 3];
                for(int i=0; i<coords.Length; i++)
                {
                    Vector vec = coords[i] - mean;
                    cov += Matrix.VVt(vec, vec);
                }
                anisou = Bioinfo.GetAnisou(cov);
            }
        }
    }
}
*/