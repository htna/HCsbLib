using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

/// https://programming.vip/docs/c-gets-cpu-and-memory-usage.html
namespace HTLib2
{
    public partial class HEnvironment
    {
        public partial class HGlobalMemoryStatus
        {
            // Get the current available physical memory size
            public static double GetAvailPhysMemByte() { return GetAvailPhysMem()                     ; }
            public static double GetAvailPhysMemMB  () { return GetAvailPhysMem() / (1024.0)          ; }
            public static double GetAvailPhysMemKB  () { return GetAvailPhysMem() / (1024.0*1024)     ; }
            public static double GetAvailPhysMemGB  () { return GetAvailPhysMem() / (1024.0*1024*1024); }

            // Get the current memory size used
            public static double GetUsedPhysMemByte() { return GetUsedPhysMem()                     ; }
            public static double GetUsedPhysMemMB  () { return GetUsedPhysMem() / (1024.0)          ; }
            public static double GetUsedPhysMemKB  () { return GetUsedPhysMem() / (1024.0*1024)     ; }
            public static double GetUsedPhysMemGB  () { return GetUsedPhysMem() / (1024.0*1024*1024); }

            // Get the current total physical memory size
            public static double GetTotalPhysMemByte() { return GetTotalPhysMem()                     ; }
            public static double GetTotalPhysMemMB  () { return GetTotalPhysMem() / (1024.0)          ; }
            public static double GetTotalPhysMemKB  () { return GetTotalPhysMem() / (1024.0*1024)     ; }
            public static double GetTotalPhysMemGB  () { return GetTotalPhysMem() / (1024.0*1024*1024); }

            //  static void Main(string[] args)
            //  {
            //      Console.WriteLine("Total memory:" + FormatSize(GetTotalPhys()));
            //      Console.WriteLine("Has been used:" + FormatSize(GetUsedPhys()));
            //      Console.WriteLine("It can be used:" + FormatSize(GetAvailPhys()));
            //      Console.ReadKey();
            //  }

            #region Obtain memory information API
            [DllImport("kernel32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO mi);

            //Define the information structure of memory
            [StructLayout(LayoutKind.Sequential)]
            public struct MEMORY_INFO
            {
                public uint dwLength; //Current structure size
                public uint dwMemoryLoad; //Current memory utilization
                public ulong ullTotalPhys; //Total physical memory size
                public ulong ullAvailPhys; //Available physical memory size
                public ulong ullTotalPageFile; //Total Exchange File Size
                public ulong ullAvailPageFile; //Total Exchange File Size
                public ulong ullTotalVirtual; //Total virtual memory size
                public ulong ullAvailVirtual; //Available virtual memory size
                public ulong ullAvailExtendedVirtual; //Keep this value always zero
            }
            #endregion

            #region Formatting capacity size
            /// <summary>
            /// Formatting capacity size
            /// </summary>
            /// <param name="size">Capacity ( B)</param>
            /// <returns>Formatted capacity</returns>
            private static string FormatSize(double size)
            {
                double d = (double)size;
                int i = 0;
                while ((d > 1024) && (i < 5))
                {
                    d /= 1024;
                    i++;
                }
                string[] unit = { "B", "KB", "MB", "GB", "TB" };
                return (string.Format("{0} {1}", Math.Round(d, 2), unit[i]));
            }
            #endregion

            #region Get the current memory usage
            /// <summary>
            /// Get the current memory usage
            /// </summary>
            /// <returns></returns>
            public static MEMORY_INFO GetMemoryStatus()
            {
                MEMORY_INFO mi = new MEMORY_INFO();
                mi.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(mi);
                GlobalMemoryStatusEx(ref mi);
                return mi;
            }
            #endregion

            #region Get the current available physical memory size
            /// <summary>
            /// Get the current available physical memory size
            /// </summary>
            /// <returns>Current available physical memory( B)</returns>
            public static ulong GetAvailPhysMem()
            {
                MEMORY_INFO mi = GetMemoryStatus();
                return mi.ullAvailPhys;
            }
            #endregion

            #region Get the current memory size used
            /// <summary>
            /// Get the current memory size used
            /// </summary>
            /// <returns>Memory size used( B)</returns>
            public static ulong GetUsedPhysMem()
            {
                MEMORY_INFO mi = GetMemoryStatus();
                return (mi.ullTotalPhys - mi.ullAvailPhys);
            }
            #endregion

            #region Get the current total physical memory size
            /// <summary>
            /// Get the current total physical memory size
            /// </summary>
            /// <returns>Total physical memory size( B)</returns>
            public static ulong GetTotalPhysMem()
            {
                MEMORY_INFO mi = GetMemoryStatus();
                return mi.ullTotalPhys;
            }
            #endregion
        }
        public class HCpuStatus
        {
            public static double GetCpuUsageRate()
            {
                // check only one NUMA node

                PerformanceCounter cpuCounter;
                cpuCounter = new PerformanceCounter();
                cpuCounter.CategoryName = "Processor";
                cpuCounter.CounterName = "% Processor Time";
                cpuCounter.InstanceName = "_Total";
                cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                float[] rates = new float[100];
                for(int i=0; i<rates.Length; i++)
                {
                    System.Threading.Thread.Sleep(10);
                    float rate = cpuCounter.NextValue();
                    rates[i] = rate;
                }
                return rates.Average();
            }

            //  static void Main(string[] args)
            //  {
            //      PerformanceCounter cpuCounter;
            //      PerformanceCounter ramCounter;
            //  
            //      cpuCounter = new PerformanceCounter();
            //      cpuCounter.CategoryName = "Processor";
            //      cpuCounter.CounterName = "% Processor Time";
            //      cpuCounter.InstanceName = "_Total";
            //      cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            //      ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            //  
            //  
            //  
            //      Console.WriteLine("Computer CPU Utilization rate:" + cpuCounter.NextValue() + "%");
            //      Console.WriteLine("The computer can use memory:" + ramCounter.NextValue() + "MB");
            //      Console.WriteLine();
            //  
            //      while (true)
            //      {
            //          System.Threading.Thread.Sleep(1000);
            //          Console.WriteLine("Computer CPU Utilization rate:" + cpuCounter.NextValue() + " %");
            //          Console.WriteLine("The computer can use memory:" + ramCounter.NextValue() + "MB");
            //          Console.WriteLine();
            //  
            //          if ((int)cpuCounter.NextValue() > 80)
            //          {
            //              System.Threading.Thread.Sleep(1000 * 60);
            //          }
            //      }
            //  }
        }
    }
}
