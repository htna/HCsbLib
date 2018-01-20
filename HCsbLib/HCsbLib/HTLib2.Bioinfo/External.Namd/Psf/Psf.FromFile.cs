using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Namd
{
	public partial class Psf
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		//                                                                                                    //
		// http://www.ks.uiuc.edu/Training/Tutorials/namd/namd-tutorial-unix-html/node21.html                 //
		//                                                                                                    //
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		static readonly char[] separator = new char[] { ' ', '\t', '\n' };

		public static Psf FromFile(string filepath)
		{
            List<string> lines = new List<string>(System.IO.File.ReadAllLines(filepath));
            Psf psf = FromLines(lines);
            return psf;
        }
        public static Psf FromLines(IEnumerable<string> lines)
		{
            List<string> llines = lines.ToList();

			Psf psf = new Psf();
			psf.header    = Collect_HEADER  (ref llines);
			psf.title     = Collect_NTITLE  (ref llines);
			psf.atoms     = CollectAtoms    (ref llines);
			psf.bonds     = CollectBonds    (ref llines);
			psf.angles    = CollectAngles   (ref llines);
			psf.dihedrals = CollectDihedrals(ref llines);
			psf.impropers = CollectImpropers(ref llines);
			psf.donors    = CollectDonors   (ref llines);
			psf.acceptors = CollectAcceptors(ref llines);

			return psf;
		}
		public static int ParseCount(ref List<string> lines, string header)
		{
			while(lines.First().Trim() == "")
				lines.RemoveAt(0);

			string line = lines[0];
			lines.RemoveAt(0);

			int idx = line.IndexOf('!');
			string strCount  = line.Substring(0, idx); strCount = strCount.Trim();
			string strHeader = line.Substring(idx+1); strHeader = strHeader.Trim();
			if(header.ToUpper() != strHeader.ToUpper())
				throw new FormatException("header in psf does not match : " + header);
			int count = int.Parse(strCount);
			return count;
		}
		public static int[,] ParseIndex(ref List<string> lines, int count, int dimension)
		{
			int[,] idxs = new int[count, dimension];
			int idx = 0;
            int iline = 0;
			while(idx < count*dimension)
			{
                string[] token = lines[iline].Split(separator, StringSplitOptions.RemoveEmptyEntries);
				for(int i=0; i<token.Length; i++)
				{
					int value = int.Parse(token[i]);
					idxs[idx/dimension, idx%dimension] = value;
					idx++;
				}
                lines[iline] = null;
                iline++;
			}
            lines.RemoveRange(0, iline);

			return idxs;
		}
		public static string[] Collect_HEADER(ref List<string> lines)
		{
			HDebug.Assert(lines[0].Substring(0,3) == "PSF");
			List<string> header = new List<string>();
			header.Add(lines[0]); lines.RemoveAt(0);
			return header.ToArray();
		}
		public static string[] Collect_NTITLE(ref List<string> lines)
		{
			int count = ParseCount(ref lines, "NTITLE");

			List<string> titles = lines.GetRange(0, count);
			lines.RemoveRange(0, count);

			return titles.ToArray();
		}
		public static Atom[] CollectAtoms(ref List<string> lines)
		{
			int count = ParseCount(ref lines, "NATOM");

			List<Atom> atoms = new List<Atom>();
			for(int i=0; i<count; i++)
			{
				Atom atom = Atom.FromLine(lines[i]);
				atoms.Add(atom);
			}
			lines.RemoveRange(0, count);
			return atoms.ToArray();
		}
		public static int[,] CollectBonds(ref List<string> lines)
		{
			int count = ParseCount(ref lines, "NBOND: bonds");
			return ParseIndex(ref lines, count, 2);
		}
		public static int[,] CollectAngles(ref List<string> lines)
		{
			int count = ParseCount(ref lines, "NTHETA: angles");
			return ParseIndex(ref lines, count, 3);
		}
		public static int[,] CollectDihedrals(ref List<string> lines)
		{
			int count = ParseCount(ref lines, "NPHI: dihedrals");
			return ParseIndex(ref lines, count, 4);
		}
		public static int[,] CollectImpropers(ref List<string> lines)
		{
			int count = ParseCount(ref lines, "NIMPHI: impropers");
			return ParseIndex(ref lines, count, 4);
		}
		public static int[,] CollectDonors(ref List<string> lines)
		{
			int count = ParseCount(ref lines, "NDON: donors");
			return ParseIndex(ref lines, count, 2);
		}
		public static int[,] CollectAcceptors(ref List<string> lines)
		{
			int count = ParseCount(ref lines, "NACC: acceptors");
			return ParseIndex(ref lines, count, 2);
		}
		
	}
}
}
