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
        public int FindType(string[] types, params string[] query)
        {
            int count_X = 0;
            foreach(string type in types)
                if(type == "X")
                    count_X++;

            int length = types.Length;
            HDebug.Assert(types.Length == query.Length);
            {
                int match = 0;
                for(int i=0; i<length; i++)
                    if(types[i] == "X" || types[i] == query[i])
                        match++;
                if(match == length)
                    return count_X;
            }
            {
                int match = 0;
                for(int i=0; i<length; i++)
                    if(types[i] == "X" || types[i] == query[length-i-1])
                        match++;
                if(match == length)
                    return count_X;
            }
            // not found
            return -1;
        }
    }
}
}
