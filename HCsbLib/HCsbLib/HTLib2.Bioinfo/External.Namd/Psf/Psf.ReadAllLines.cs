using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
public partial class Namd
{
    public partial class Psf
	{
        public static string[] ReadAllLines(params string[] filepaths)
        {
            List<string> lines = new List<string>();
            foreach(string path in filepaths)
            {
                foreach(string line in HFile.HEnumAllLines(path))
                {
                    if(line.Trim().ToUpper() == "END")
                        continue;
                    lines.Add(line);
                }
            }
            lines.Add("");
            lines.Add("");
            lines.Add("");
            lines.Add("");
            lines.Add("END");
            lines.Add("");

            return lines.ToArray();
        }
	}
}
}
