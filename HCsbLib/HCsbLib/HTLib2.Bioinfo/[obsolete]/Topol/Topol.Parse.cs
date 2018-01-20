/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
	public partial class Topol
	{
		public static void Parse()
		{
			string[] lines = GetTopologyText();
			int index = 0;

			/////////////////////////////////////////////////////////////
			// header
			for(; index<lines.Length; index++)
			{
				string line = lines[index];
				if(line.StartsWith("*") == false)
					break;
			}
			/////////////////////////////////////////////////////////////
			// version
			version = lines[index++];
			/////////////////////////////////////////////////////////////
			// MASS
			MASSs = new Dictionary<string, Mass>();
			for(; index<lines.Length; index++)
			{
				string line = lines[index];
				line = line.Split('!')[0];
				line = line.Trim();
				if(line.Length == 0)
					continue;
				if(line.StartsWith("MASS ") == false)
					break;
				Mass MASS = Mass.FromLine(line);
				MASSs.Add(MASS.Type, MASS);
			}
			/////////////////////////////////////////////////////////////
			// DECL
			DECLs = new List<string>();
			for(; index<lines.Length; index++)
			{
				string line = lines[index];
				line = line.Split('!')[0];
				line = line.Trim();
				if(line.Length == 0)
					continue;
				if(line.StartsWith("DECL ") == false)
					break;
				DECLs.Add(line);
			}
			/////////////////////////////////////////////////////////////
			// DEFA
			DEFAs = new List<string>();
			for(; index<lines.Length; index++)
			{
				string line = lines[index];
				line = line.Split('!')[0];
				line = line.Trim();
				if(line.Length == 0)
					continue;
				if(line.StartsWith("DEFA ") == false)
					break;
				DEFAs.Add(line);
			}
			/////////////////////////////////////////////////////////////
			// AUTO
			AUTOs = new List<string>();
			for(; index<lines.Length; index++)
			{
				string line = lines[index];
				line = line.Split('!')[0];
				line = line.Trim();
				if(line.Length == 0)
					continue;
				if(line.StartsWith("AUTO ") == false)
					break;
				AUTOs.Add(line);
			}
			/////////////////////////////////////////////////////////////
			// RESI, PRES, END
			RESIs = new Dictionary<string,Resi>();
			PRESs = new Dictionary<string,Pres>();
			List<string> sublines = new List<string>();
			for(; index<lines.Length; index++)
			{
				string line = lines[index];
				line = line.Split('!')[0];
				line = line.Trim();
				if(line.Length == 0)
					continue;
				if(line.StartsWith("END"))
					break;
				if(sublines.Count == 0)
				{
					sublines.Add(line);
					continue;
				}
				if(line.StartsWith("RESI ") || line.StartsWith("PRES "))
				{
					if(sublines[0].StartsWith("RESI "))
					{
						Resi RESI = Resi.FromLines(sublines.ToArray());
						RESIs.Add(RESI.Name, RESI);
					}
					else if(sublines[0].StartsWith("PRES "))
					{
						Pres PRES = Pres.FromLines(sublines.ToArray());
						PRESs.Add(PRES.Name, PRES);
					}
					else HDebug.Assert(false);
					sublines.Clear();
					sublines.Add(line);
					continue;
				}
				sublines.Add(line);
			}
		}
	}
}
*/