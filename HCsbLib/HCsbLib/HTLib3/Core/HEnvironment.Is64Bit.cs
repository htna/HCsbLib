using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace HTLib3
{
	public partial class HEnvironment
	{
        /// http://social.msdn.microsoft.com/Forums/en-US/csharpgeneral/thread/24792cdc-2d8e-454b-9c68-31a19892ca53
        /// 
        /// After a couple minutes digging around and finding the various posts, this simple section of code told me 
        /// if the current process is running on a 64 bit OS and is being emulated for 32 bit.
		[DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsWow64Process([In] IntPtr hProcess, [Out] out bool lpSystemInfo);

		public static bool IsWow64BitProcess
		{
			get
			{
				bool retVal;
				IsWow64Process(System.Diagnostics.Process.GetCurrentProcess().Handle, out retVal);
				return retVal;
			}
		}
	}
}
