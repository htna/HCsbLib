using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2.Bioinfo
{
    using Keywds = Pdb.Keywds;
    public static partial class PdbCollection
    {
        public static List<string> ListKeyword(this IList<Keywds> keywds)
        {
            string strKeywds = "";
            foreach(Keywds keywd in keywds)
            {
                string lstrKeywds = keywd.keywds.Trim();
                strKeywds += lstrKeywds;
            }
            strKeywds = strKeywds.Trim();
            while(true)
            {
                string _strKeywds = strKeywds.Replace("  ", " ");
                if(_strKeywds == strKeywds)
                    break;
                strKeywds = _strKeywds;
            }
            List<string> lstKeywd = new List<string>(strKeywds.Split(','));
            for(int i=0; i<lstKeywd.Count; i++)
                lstKeywd[i] = lstKeywd[i].Trim();
            while(lstKeywd.Remove(""));
            return lstKeywd;
        }
    }
}
