using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib3
{
	public static partial class HDirectory
	{
        public static DirectoryInfo CreateTempDirectory(string tempbase=null)
        {
            while(true)
            {
                string path = System.IO.Path.GetRandomFileName();
                if(System.IO.File.Exists(tempbase+path) == false)
                {
                    return System.IO.Directory.CreateDirectory(tempbase+path);
                }
            }
        }
	}
}
