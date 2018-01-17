using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.ConstrainedExecution;
using System.Linq;

namespace HTLib2
{
    public class HOptions
    {
        string[] opts;
        public HOptions(string opt)
        {
            if(opt == null)
                opt = "";
            opts = opt.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            opts = opts.HTrim();
            opts = opts.HRemoveAll("");
        }
        public HOptions(string[] opt)
        {
            opts = opt.HTrim();
            opts = opts.HRemoveAll("");
        }
        public bool Contains(string opt)
        {
            return opts.Contains(opt);
        }
        public string[] HSelectStartsWith(string startswith)
        {
            return opts.HSelectStartsWith(startswith);
        }

        public static implicit operator HOptions(string opt)
        {
            return new HOptions(opt);
        }
        public static implicit operator HOptions(string[] opt)
        {
            return new HOptions(opt);
        }
    }
}
