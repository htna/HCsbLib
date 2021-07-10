using System;
using System.Collections;
using System.Linq;
using System.Text;
namespace HTLib2
{
    public partial class HEnvironment
    {
        public static string CommandLine                                                                                              { get { return System.Environment.CommandLine            ;} }
        public static string CurrentDirectory                                                                                         { get { return System.Environment.CurrentDirectory       ;} set { System.Environment.CurrentDirectory = value; } }
        public static int ExitCode                                                                                                    { get { return System.Environment.ExitCode               ;} set { System.Environment.ExitCode         = value;} }
        public static bool HasShutdownStarted                                                                                         { get { return System.Environment.HasShutdownStarted     ;} }
        public static bool Is64BitOperatingSystem                                                                                     { get { return System.Environment.Is64BitOperatingSystem ;} }
        public static bool Is64BitProcess                                                                                             { get { return System.Environment.Is64BitProcess         ;} }
        public static string MachineName                                                                                              { get { return System.Environment.MachineName            ;} }
        public static string NewLine                                                                                                  { get { return System.Environment.NewLine                ;} }
        public static OperatingSystem OSVersion                                                                                       { get { return System.Environment.OSVersion              ;} }
        public static int ProcessorCount                                                                                              { get { return System.Environment.ProcessorCount         ;} }
        public static string StackTrace                                                                                               { get { return System.Environment.StackTrace             ;} }
        public static string SystemDirectory                                                                                          { get { return System.Environment.SystemDirectory        ;} }
        public static int SystemPageSize                                                                                              { get { return System.Environment.SystemPageSize         ;} }
        public static int TickCount                                                                                                   { get { return System.Environment.TickCount              ;} }
        public static string UserDomainName                                                                                           { get { return System.Environment.UserDomainName         ;} }
        public static bool UserInteractive                                                                                            { get { return System.Environment.UserInteractive        ;} }
        public static string UserName                                                                                                 { get { return System.Environment.UserName               ;} }
        public static Version Version                                                                                                 { get { return System.Environment.Version                ;} }
        public static long WorkingSet                                                                                                 { get { return System.Environment.WorkingSet             ;} }
        public static void Exit(int exitCode)                                                                                         {        System.Environment.Exit(exitCode)                                  ; }
        public static string ExpandEnvironmentVariables(string name)                                                                  { return System.Environment.ExpandEnvironmentVariables(name)                ; }
        public static void FailFast(string message)                                                                                   {        System.Environment.FailFast(message)                               ; }
        public static void FailFast(string message, Exception exception)                                                              {        System.Environment.FailFast(message, exception)                    ; }
        public static string[] GetCommandLineArgs()                                                                                   { return System.Environment.GetCommandLineArgs()                            ; }
        public static string GetEnvironmentVariable(string variable)                                                                  { return System.Environment.GetEnvironmentVariable(variable)                ; }
        public static string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)                                { return System.Environment.GetEnvironmentVariable(variable, target)        ; }
        public static IDictionary GetEnvironmentVariables()                                                                           { return System.Environment.GetEnvironmentVariables()                       ; }
        public static IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target)                                           { return System.Environment.GetEnvironmentVariables(target)                 ; }
        public static string GetFolderPath(System.Environment.SpecialFolder folder)                                                   { return System.Environment.GetFolderPath(folder)                           ; }
        public static string GetFolderPath(System.Environment.SpecialFolder folder, System.Environment.SpecialFolderOption option)    { return System.Environment.GetFolderPath(folder, option)                   ; }
        public static string[] GetLogicalDrives()                                                                                     { return System.Environment.GetLogicalDrives()                              ; }
        public static void SetEnvironmentVariable(string variable, string value)                                                      {        System.Environment.SetEnvironmentVariable(variable, value)         ; }
        public static void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target)                    {        System.Environment.SetEnvironmentVariable(variable, value, target) ; }
    }
}
