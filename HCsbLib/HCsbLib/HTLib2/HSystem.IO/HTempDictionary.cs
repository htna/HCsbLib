using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Linq;
using System.Text;
using System.IO;

namespace HTLib2
{
    /// 
    /// use(var temp = new HTempDirectory())
    /// {
    ///     temp.EnterTemp();
    ///     temp.QuitTemp();
    /// }
    /// 
	public class HTempDirectory : IDisposable
	{
        public readonly DirectoryInfo dirinfo;
        public readonly string        initpath;
        public readonly string        copyto;
        public HTempDirectory
            ( string tempbase
            , string copyto   // [=null]
            )
        {
            this.initpath = HEnvironment.CurrentDirectory;
            this.dirinfo  = HDirectory.CreateTempDirectory(tempbase);
            this.copyto   = copyto;
        }
        public void EnterTemp()
        {
            HEnvironment.CurrentDirectory = dirinfo.FullName;
        }
        public void QuitTemp()
        {
            HEnvironment.CurrentDirectory = initpath;
        }
        public void Dispose()
        {
            QuitTemp();
            try
            {
                if(copyto != null)
                    HDirectory.DirectoryCopy(dirinfo.FullName, copyto, true);
                dirinfo.Delete(true);
            }
            catch(Exception)
            {
            }
        }
        public override string ToString()
        {
            return dirinfo.FullName;
        }
	}
}
