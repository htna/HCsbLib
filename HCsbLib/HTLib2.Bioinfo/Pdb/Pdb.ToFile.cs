using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        public static void ToFile( string filepath
                                 , Element[] elements
                                 , bool append //= false
                                 , IList<string> additionalHeaders = null
                                 )
        {
            List<string> lines = new List<string>();
            if(additionalHeaders != null)
                lines.AddRange(additionalHeaders);
            lines.Add("MODEL        1                                                                  ");
            lines.AddRange(elements.ToLines());
            lines.Add("ENDMDL                                                                          ");
            if(append == false)
            {
                HFile.WriteAllLines(filepath, lines);
            }
            else
            {
                StringBuilder text = new StringBuilder();
                foreach(string line in lines)
                    text.AppendLine(line);
                HFile.AppendAllText(filepath, text.ToString());
            }
        }
        public static void ToFile( string filepath
                                 , IList<Atom> atoms
                                 , IList<Vector> coords = null
                                 , IList<MatrixByArr> anisous = null
                                 , double? anisouScale = null
                                 , IList<double> bfactors = null
                                 , bool append = false
                                 , IList<string> headers = null
                                 )
        {
            List<string> lines = new List<string>();
            int size = atoms.Count;
            if(coords      != null) HDebug.Assert(size == coords.Count);
            if(anisous     != null) HDebug.Assert(size == anisous.Count);
            //if(anisouScale != null) Debug.Assert(size == anisouScale.Count);
            if(headers     != null)
                lines.AddRange(headers);
            lines.Add("MODEL        1                                                                  ");
            for(int i=0; i<size; i++)
            {
                Atom atom = atoms[i];
                if(atom == null)
                    continue;
                {
                    if(coords != null)
                    {
                        double x = coords[i][0];
                        double y = coords[i][1];
                        double z = coords[i][2];
                        atom = Atom.FromString(atom.GetUpdatedLine(x, y, z));
                    }
                    if(bfactors != null)
                    {
                        atom = Atom.FromString(atom.GetUpdatedLineTempFactor(bfactors[i]));
                    }
                    lines.Add(atom.line);
                }
                {
                    if(anisous != null)
                    {
                        MatrixByArr U = anisous[i];
                        if(anisouScale != null)
                            U = U * anisouScale.Value;
                        Anisou anisou = Anisou.FromAtom(atom, U.ToArray().HToInt());
                        string line = anisou.line; //anisou.GetUpdatedU(anisous[i])
                        lines.Add(line);
                    }
                }
            }
            lines.Add("ENDMDL                                                                          ");
            //int idx = 0;
            //foreach(Element element in elements)
            //{
            //    string line = element.line;
            //    if(typeof(Atom).IsInstanceOfType(element))
            //    {
            //        double x = coords[idx][0];
            //        double y = coords[idx][1];
            //        double z = coords[idx][2];
            //        Atom atom = (Atom)element;
            //        line = atom.GetUpdatedLine(x, y, z);
            //        idx++;
            //    }
            //    lines.Add(line);
            //}
            if(append == false)
            {
                HFile.WriteAllLines(filepath, lines);
            }
            else
            {
                StringBuilder text = new StringBuilder();
                foreach(string line in lines)
                    text.AppendLine(line);
                HFile.AppendAllText(filepath, text.ToString());
            }
        }
        public static void ToFile( string filepath
                                 , List<Atom> atoms
                                 , List<Vector>[] coordss
                                 , List<double>[] bfactorss
                                 )
        {
            List<string> lines = new List<string>();
            int size = atoms.Count;
            for(int iensemble=0; iensemble<coordss.Length; iensemble++)
            {
                List<Vector> coords = coordss[iensemble];
                List<double> bfactors = bfactorss[iensemble];
                HDebug.Assert(size == coords.Count);
                //lines.Add("MODEL        1                                                                  ");
                lines.Add(Pdb.Model.FromModelSerial(iensemble+1).line);
                for(int i=0; i<size; i++)
                {
                    Atom atom = atoms[i];
                    {
                        if(coordss != null)
                        {
                            double x = coords[i][0];
                            double y = coords[i][1];
                            double z = coords[i][2];
                            atom = Atom.FromString(atom.GetUpdatedLine(x, y, z));
                        }
                        if(bfactors != null)
                        {
                            atom = Atom.FromString(atom.GetUpdatedLineTempFactor(bfactors[i]));
                        }
                        lines.Add(atom.line);
                    }
                }
                lines.Add(Pdb.Endmdl.From().line);
                //lines.Add("ENDMDL                                                                          ");
            }
            HFile.WriteAllLines(filepath, lines);
        }
        public static void ToFile( string filepath
                                 , Atom[] atoms
                                 , IList<Vector[]> ensemble
                                 )
        {
            ToFile(filepath, atoms, coords:ensemble[0], anisous:null, anisouScale:null, bfactors:null, append:false);
            for(int i=1; i<ensemble.Count; i++)
                ToFile(filepath, atoms, coords: ensemble[i], anisous: null, anisouScale: null, bfactors: null, append: true);
        }
        public static void ToFile(string filepath
                                 , List<Atom>[] ensemble
                                 )
        {
            ToFile(filepath, ensemble[0], coords:null, anisous:null, anisouScale:null, bfactors:null, append:false);
            for(int i=1; i<ensemble.Length; i++)
                ToFile(filepath, ensemble[i], coords:null, anisous:null, anisouScale:null, bfactors:null, append:true);
        }
        //public void ToFile(string filepath, IList<Vector> coords, bool append=false)
        //{
        //    List<string> lines = new List<string>();
        //    Debug.Assert(atoms.Length == coords.Count);
        //    int idx = 0;
        //    foreach(Element element in elements)
        //    {
        //        string line = element.line;
        //        if(typeof(Atom).IsInstanceOfType(element))
        //        {
        //            double x = coords[idx][0];
        //            double y = coords[idx][1];
        //            double z = coords[idx][2];
        //            Atom atom = (Atom)element;
        //            line = atom.GetUpdatedLine(x, y, z);
        //            idx++;
        //        }
        //        lines.Add(line);
        //    }
        //    if(append == false)
        //    {
        //        File.WriteAllLines(filepath, lines);
        //    }
        //    else
        //    {
        //        StringBuilder text = new StringBuilder();
        //        foreach(string line in lines)
        //            text.AppendLine(line);
        //        File.AppendAllText(filepath, text.ToString());
        //    }
        //}
        public string[] ToLines()
        {
            List<string> lines = new List<string>();
            foreach(Element element in elements)
                lines.Add(element.line);
            return lines.ToArray();
        }
        public void ToFile(string filepath)
        {
            string[] lines = ToLines();
            HFile.WriteAllLines(filepath, lines);
        }
        public void ToFile(string filepath, IList<Vector> coords=null, IList<double> bfactors=null, IList<MatrixByArr> anisous=null, double anisous_scale=1, bool append=false)
        {
            List<string> lines = new List<string>();
            if(coords == null)
                coords = this.atoms.ListCoord();
            //Debug.Assert(atoms.Length == coords.Count);
            int idx = 0;
            foreach(Element element in elements)
            {
                //string line = element.line;
                if(typeof(Atom).IsInstanceOfType(element))
                {
                    double x = coords[idx][0];
                    double y = coords[idx][1];
                    double z = coords[idx][2];

                    Atom atom = (Atom)element;
                    atom = Atom.FromString(atom.GetUpdatedLine(x, y, z));
                    
                    if(bfactors != null)
                        atom = Atom.FromString(atom.GetUpdatedLineTempFactor(bfactors[idx]));

                    lines.Add(atom.line);

                    if(anisous != null)
                    {
                        MatrixByArr anisoui = anisous[idx] * anisous_scale;
                        int[,] U = new int[3, 3]{ {(int)anisoui[0,0], (int)anisoui[0,1], (int)anisoui[0,2], },
                                                  {(int)anisoui[1,0], (int)anisoui[1,1], (int)anisoui[1,2], },
                                                  {(int)anisoui[2,0], (int)anisoui[2,1], (int)anisoui[2,2], },
                                                };
                        Anisou anisou = Anisou.FromAtom(atom, U);
                        lines.Add(anisou.line);
                    }

                    idx++;
                }
                else if(typeof(Anisou).IsInstanceOfType(element) && anisous != null)
                {
                    // skip because it is handled in the "Atom" part
                }
                else
                {
                    lines.Add(element.line);
                }
            }

            if(append == false)
            {
                HFile.WriteAllLines(filepath, lines);
            }
            else
            {
                StringBuilder text = new StringBuilder();
                foreach(string line in lines)
                    text.AppendLine(line);
                HFile.AppendAllText(filepath, text.ToString());
            }
        }
    }
}
