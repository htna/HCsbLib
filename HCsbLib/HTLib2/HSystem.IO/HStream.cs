using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib2
{
    public static partial class HStaticStream
    {
        public static String[] HReadAllLines(this Stream stream)
        {
            //http://stackoverflow.com/questions/23989677/file-readalllines-or-stream-reader
            String line;
            List<String> lines = new List<String>();

            using(StreamReader sr = new StreamReader(stream))
                while ((line = sr.ReadLine()) != null)
                    lines.Add(line);

            return lines.ToArray();
        }
        public static String[] HReadAllLines(this Stream stream, Encoding encoding)
        {
            //http://stackoverflow.com/questions/23989677/file-readalllines-or-stream-reader
            String line;
            List<String> lines = new List<String>();

            using(StreamReader sr = new StreamReader(stream, encoding))
                while ((line = sr.ReadLine()) != null)
                    lines.Add(line);

            return lines.ToArray();
        }
    }
}
