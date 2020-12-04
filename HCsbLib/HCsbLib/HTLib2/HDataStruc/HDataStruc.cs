using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;

namespace HTLib2
{
	public static partial class HDataStruc
	{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T HLast<T>(this List<T> list)
        {
			return list[list.Count-1];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void HRemoveLast<T>(this List<T> list)
        {
			list.RemoveAt(list.Count-1);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T HPopLast<T>(this List<T> list)
        {
			T last = list[list.Count-1];
			list.RemoveAt(list.Count-1);
			return last;
        }
	};
}
