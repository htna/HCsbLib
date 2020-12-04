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
		public static T HStackPeek<T>(this List<T> stack)
        {
			return stack[stack.Count-1];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T HStackPop<T>(this List<T> stack)
        {
			int idx = stack.Count-1;
			T top = stack[idx];
			stack.RemoveAt(idx);
			return top;
        }
	};
}
