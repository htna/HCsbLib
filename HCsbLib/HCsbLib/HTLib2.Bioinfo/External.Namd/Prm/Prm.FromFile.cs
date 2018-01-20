using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
public partial class Namd
{
	public partial class Prm
	{
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		//                                                                                                    //
		// http://                                                                                            //
		//                                                                                                    //
		////////////////////////////////////////////////////////////////////////////////////////////////////////
		static readonly char[] separator = new char[] { ' ', '\t', '\n' };

		static readonly string[] keywards = new string[] { "BONDS", "ANGLES", "DIHEDRALS", "IMPROPER","CMAP",
															"NONBONDED", "NBFIX", "HBOND", "END",
														};

        public static Prm FromFile(string filepath)
        {
            string[] lines = System.IO.File.ReadAllLines(filepath);
            return FromLinesXXX(lines, new TextLogger());
        }
        public static Prm FromFile(string filepath, ITextLogger logger)
		{
            //return FromFileXPlor(filepath);

            string[] lines = System.IO.File.ReadAllLines(filepath);
            return FromLinesXXX(lines, logger);
		}
        public static Prm FromLines(IList<string> lines)
        {
            return FromLinesXXX(lines, new TextLogger());
        }
        public static Prm FromLines(IList<string> lines, ITextLogger logger)
        {
            return FromLinesXXX(lines, logger);
        }

        public static void RemoveComments(ref List<string> lines)
		{
			// remove comment that begins with '!'
			for(int i=0; i<lines.Count; i++)
			{
				string line = lines[i];
				int idx;
				// remove comment that begins with '!'
				idx = line.IndexOf("!");
				if(idx != -1)
					line = line.Substring(0, idx);
				// remove blanks at the end of line
				line = line.Trim();
				// update line
				lines[i] = line;
			}
			// remove empty lines
			for(int i=0; i<lines.Count; )
			{
				if(lines[i] == "")
					lines.RemoveAt(i);
				else
					i++;
			}
		}
	}
}
}
