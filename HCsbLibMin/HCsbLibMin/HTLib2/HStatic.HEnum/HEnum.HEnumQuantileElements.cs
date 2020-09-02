﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTLib2
{
    public static partial class HEnum
    {
        public static IEnumerable<double> HEnumQuantileElements(this IEnumerable<double> seq, double quantile_from, double quantile_to)
        {
            var elements = seq.ToList();
            elements.Sort();
            double realIndex_from = quantile_from*(elements.Count-1);
            double realIndex_to   = quantile_to  *(elements.Count-1);
            int index_from = (int)realIndex_from;
            int index_to   = (int)realIndex_to  ;
            for(int index=index_from; index<=index_to; index++)
                yield return elements[index];
        }
    }
}
