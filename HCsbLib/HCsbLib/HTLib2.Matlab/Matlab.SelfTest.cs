using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public partial class Matlab
    {
        static void Main(string[] args)
		{
            double x=0;
            for(int i=0; i<100000; i++)
            {
                Matlab.PutValue("x", x);
                Matlab.Execute("x=x+1;");
                x = Matlab.GetValue("x");
                //matlab.Execute("clear x");
                System.Console.WriteLine(x);
            }
		}
	}
}
