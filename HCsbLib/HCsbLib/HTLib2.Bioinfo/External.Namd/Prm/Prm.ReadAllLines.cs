using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HTLib2.Bioinfo
{
public partial class Namd
{
    public partial class Prm
	{
        public static string[] ReadAllLines(params string[] filepaths)
        {
            List<string> lines = new List<string>();
            foreach(string path in filepaths)
            {
                foreach(string line in HFile.HEnumAllLines(path))
                {
                    lines.Add(line);
                }
            }

            return lines.ToArray();
        }
    }
}
}