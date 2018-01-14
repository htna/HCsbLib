using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib2.Bioinfo
{
	public partial class Pdb
	{
        public static Pdb FromStream(Stream stream)
        {
            return FromStream(new StreamReader(stream));
        }
        public static Pdb FromStream(StreamReader streamreader)
		{
            List<string> lines = new List<string>();
            while(true)
            {
                string line = streamreader.ReadLine();
                if(line == null)
                    break;
                lines.Add(line);
            }

            return FromLines(lines);
		}
        public static Pdb FromLines(IList<string> lines)
        {
            //  Pdb FromLines(IEnumerable<string> lines)
            //  ...
            //  Element[] elements = new Element[lines.Count];
            //  for(int i=0; i<lines.Count; i++)
            //      elements[i] = new Element(lines[i]);
            //  elements = elements.UpdateElement();

            List<Element> elements = new List<Element>();
            foreach(string line in lines)
                elements.Add(new Element(line));

            elements = elements.UpdateElement();

            return new Pdb(elements);
        }
    }
}
