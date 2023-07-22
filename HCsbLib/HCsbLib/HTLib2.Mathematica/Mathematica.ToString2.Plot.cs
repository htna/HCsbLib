using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Wolfram.NETLink;

// C:\Program Files\Wolfram Research\Mathematica\7.0\SystemFiles\Links\NETLink

namespace HTLib2
{
	public partial class Mathematica
	{
		public static List<string> ToString2_PlotHist2D
            ( IEnumerable<(double x, double y)> values
            , double x_gap
            , double y_gap
            )
	    {
            Dictionary<(int ix, int iy), int> ixy_cnt = new Dictionary<(int ix, int iy), int>();
            foreach((double x, double y) in values)
            {
                (int ix, int iy) key =
                    ( (int)Math.Round(x / x_gap)
                    , (int)Math.Round(y / y_gap)
                    );

                if(ixy_cnt.ContainsKey(key) == false)
                    ixy_cnt.Add(key, 0);
                ixy_cnt[key] ++;
            }

            List<string> lines = new List<string>();
            lines.Add("hist = "+Mathematica.ToString2(ixy_cnt) + ";"                                        );
            lines.Add("xgap = "+Mathematica.ToString2(x_gap  ) + ";"                                        );
            lines.Add("ygap = "+Mathematica.ToString2(y_gap  ) + ";"                                        );
            lines.Add(""                                                                                    );
            lines.Add(""                                                                                    );
            lines.Add("xmin = Min[Keys[ixycnt][[All,1]]]"                                                   );
            lines.Add("xmax = Max[Keys[ixycnt][[All,1]]]"                                                   );
            lines.Add("ymin = Min[Keys[ixycnt][[All,2]]]"                                                   );
            lines.Add("ymax = Max[Keys[ixycnt][[All,2]]]"                                                   );
            lines.Add(""                                                                                    );
            lines.Add(""                                                                                    );
            lines.Add("map1 = Table[ {ixy[[1]]*xgap  , ixy[[2]]*ygap  , ixycnt[ixy]}, {ixy, Keys[ixycnt]}];");
            lines.Add("map2 = Table[ {ixy[[1]]-xmin+1, ixy[[2]]-ymin+1, ixycnt[ixy]}, {ixy, Keys[ixycnt]}];");
            lines.Add("Dimensions[map2]"                                                                    );
            lines.Add(""                                                                                    );
            lines.Add("map3 = Table[0, {x,1,xmax-xmin+1},{y,1,ymax-ymin+1}];"                               );
            lines.Add("Dimensions[map3]"                                                                    );
            lines.Add("For[i=1,i<=Length[map2],i++,v"                                                       );
            lines.Add("  xyc = map2[[i]];"                                                                  );
            lines.Add("  map3[[ xyc[[1]], xyc[[2]] ]] = xyc[[3]];"                                          );
            lines.Add("  ];"                                                                                );
            lines.Add(""                                                                                    );
            lines.Add(""                                                                                    );
            lines.Add("ArrayPlot[map3]"                                                                     );
            lines.Add(""                                                                                    );
            lines.Add(""                                                                                    );
            lines.Add("ListPlot3D[map1, PlotRange->All]"                                                    );

            return lines;
        }
	}
}
