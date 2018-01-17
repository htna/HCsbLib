using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
	public partial class Matlab
	{
        private static bool _registrated = false;
        public static bool IsRegistered()
        {
            return _registrated;
        }
        public static void Register(string path_temporary /*=null*/)
        {
            if(_registrated == true)
            {
                //HDebug.Assert(false);
                return;
            }
            _registrated = true;
            _path_temporary = path_temporary;
            Matlab.NumericSolver.Register();
        }
        public static void Unregister()
        {
            if(_registrated == false)
            {
                HDebug.Assert(false);
                return;
            }
            _registrated = false;
        }
    }
}
