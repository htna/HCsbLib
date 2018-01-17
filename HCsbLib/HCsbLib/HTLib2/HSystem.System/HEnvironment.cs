using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace HTLib2
{
	public partial class HEnvironment
	{
		/**
		// Summary:
		//     Gets the command line for this process.
		//
		// Returns:
		//     A string containing command-line arguments.
		*/ public static string CommandLine                                                                                              { get { return System.Environment.CommandLine            ;} }
		/**
		// Summary:
		//     Gets or sets the fully qualified path of the current working directory.
		//
		// Returns:
		//     A string containing a directory path.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     Attempted to set to an empty string ("").
		//
		//   System.ArgumentNullException:
		//     Attempted to set to null.
		//
		//   System.IO.IOException:
		//     An I/O error occurred.
		//
		//   System.IO.DirectoryNotFoundException:
		//     Attempted to set a local path that cannot be found.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the appropriate permission.
		*/ public static string CurrentDirectory                                                                                         { get { return System.Environment.CurrentDirectory       ;} set { System.Environment.CurrentDirectory = value; } }
		/**
		//
		// Summary:
		//     Gets or sets the exit code of the process.
		//
		// Returns:
		//     A 32-bit signed integer containing the exit code. The default value is zero.
		*/ public static int ExitCode                                                                                                    { get { return System.Environment.ExitCode               ;} set { System.Environment.ExitCode         = value;} }
		/**
		// Summary:
		//     Gets a value indicating whether the common language runtime (CLR) is shutting
		//     down.
		//
		// Returns:
		//     true if the CLR is shutting down; otherwise, false.
		*/ public static bool HasShutdownStarted                                                                                         { get { return System.Environment.HasShutdownStarted     ;} }
		/**
		// Summary:
		//     Determines whether the current operating system is a 64-bit operating system.
		//
		// Returns:
		//     true if the operating system is 64-bit; otherwise, false.
		*/ public static bool Is64BitOperatingSystem                                                                                     { get { return System.Environment.Is64BitOperatingSystem ;} }
		/**
		// Summary:
		//     Determines whether the current process is a 64-bit process.
		//
		// Returns:
		//     true if the process is 64-bit; otherwise, false.
		*/ public static bool Is64BitProcess                                                                                             { get { return System.Environment.Is64BitProcess         ;} }
		/**
		// Summary:
		//     Gets the NetBIOS name of this local computer.
		//
		// Returns:
		//     A string containing the name of this computer.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     The name of this computer cannot be obtained.
		*/ public static string MachineName                                                                                              { get { return System.Environment.MachineName            ;} }
		/**
		// Summary:
		//     Gets the newline string defined for this environment.
		//
		// Returns:
		//     A string containing "\r\n" for non-Unix platforms, or a string containing
		//     "\n" for Unix platforms.
		*/ public static string NewLine                                                                                                  { get { return System.Environment.NewLine                ;} }
		/**
		// Summary:
		//     Gets an System.OperatingSystem object that contains the current platform
		//     identifier and version number.
		//
		// Returns:
		//     An object that contains the platform identifier and version number.
		//
		// Exceptions:
		//   System.InvalidOperationException:
		//     This property was unable to obtain the system version.-or- The obtained platform
		//     identifier is not a member of System.PlatformID
		*/ public static OperatingSystem OSVersion                                                                                       { get { return System.Environment.OSVersion              ;} }
		/**
		// Summary:
		//     Gets the number of processors on the current machine.
		//
		// Returns:
		//     The 32-bit signed integer that specifies the number of processors on the
		//     current machine. There is no default.
		*/ public static int ProcessorCount                                                                                              { get { return System.Environment.ProcessorCount         ;} }
		/**
		// Summary:
		//     Gets current stack trace information.
		//
		// Returns:
		//     A string containing stack trace information. This value can be System.String.Empty.
		//
		// Exceptions:
		//   System.ArgumentOutOfRangeException:
		//     The requested stack trace information is out of range.
		*/ public static string StackTrace                                                                                               { get { return System.Environment.StackTrace             ;} }
		/**
		// Summary:
		//     Gets the fully qualified path of the system directory.
		//
		// Returns:
		//     A string containing a directory path.
		*/ public static string SystemDirectory                                                                                          { get { return System.Environment.SystemDirectory        ;} }
		/**
		// Summary:
		//     Gets the amount of memory for an operating system's page file.
		//
		// Returns:
		//     The number of bytes in a system page file.
		*/ public static int SystemPageSize                                                                                              { get { return System.Environment.SystemPageSize         ;} }
		/**
		// Summary:
		//     Gets the number of milliseconds elapsed since the system started.
		//
		// Returns:
		//     A 32-bit signed integer containing the amount of time in milliseconds that
		//     has passed since the last time the computer was started.
		*/ public static int TickCount                                                                                                   { get { return System.Environment.TickCount              ;} }
		/**
		// Summary:
		//     Gets the network domain name associated with the current user.
		//
		// Returns:
		//     The network domain name associated with the current user.
		//
		// Exceptions:
		//   System.PlatformNotSupportedException:
		//     The operating system does not support retrieving the network domain name.
		//
		//   System.InvalidOperationException:
		//     The network domain name cannot be retrieved.
		*/ public static string UserDomainName                                                                                           { get { return System.Environment.UserDomainName         ;} }
		/**
		// Summary:
		//     Gets a value indicating whether the current process is running in user interactive
		//     mode.
		//
		// Returns:
		//     true if the current process is running in user interactive mode; otherwise,
		//     false.
		*/ public static bool UserInteractive                                                                                            { get { return System.Environment.UserInteractive        ;} }
		/**
		// Summary:
		//     Gets the user name of the person who is currently logged on to the Windows
		//     operating system.
		//
		// Returns:
		//     The user name of the person who is logged on to Windows.
		*/ public static string UserName                                                                                                 { get { return System.Environment.UserName               ;} }
		/**
		// Summary:
		//     Gets a System.Version object that describes the major, minor, build, and
		//     revision numbers of the common language runtime.
		//
		// Returns:
		//     An object that displays the version of the common language runtime.
		*/ public static Version Version                                                                                                 { get { return System.Environment.Version                ;} }
		/**
		// Summary:
		//     Gets the amount of physical memory mapped to the process context.
		//
		// Returns:
		//     A 64-bit signed integer containing the number of bytes of physical memory
		//     mapped to the process context.
		*/ public static long WorkingSet                                                                                                 { get { return System.Environment.WorkingSet             ;} }

		/**
		// Summary:
		//     Terminates this process and gives the underlying operating system the specified
		//     exit code.
		//
		// Parameters:
		//   exitCode:
		//     Exit code to be given to the operating system.
		//
		// Exceptions:
		//   System.Security.SecurityException:
		//     The caller does not have sufficient security permission to perform this function.
		*/ public static void Exit(int exitCode)                                                                                         {        System.Environment.Exit(exitCode)                                  ; }
		/**
		// Summary:
		//     Replaces the name of each environment variable embedded in the specified
		//     string with the string equivalent of the value of the variable, then returns
		//     the resulting string.
		//
		// Parameters:
		//   name:
		//     A string containing the names of zero or more environment variables. Each
		//     environment variable is quoted with the percent sign character (%).
		//
		// Returns:
		//     A string with each environment variable replaced by its value.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     name is null.
		*/ public static string ExpandEnvironmentVariables(string name)                                                                  { return System.Environment.ExpandEnvironmentVariables(name)                ; }
		/**
		// Summary:
		//     Immediately terminates a process after writing a message to the Windows Application
		//     event log, and then includes the message in error reporting to Microsoft.
		//
		// Parameters:
		//   message:
		//     A message that explains why the process was terminated, or null if no explanation
		//     is provided.
		*/ public static void FailFast(string message)                                                                                   {        System.Environment.FailFast(message)                               ; }
		/**
		// Summary:
		//     Immediately terminates a process after writing a message to the Windows Application
		//     event log, and then includes the message and exception information in error
		//     reporting to Microsoft.
		//
		// Parameters:
		//   message:
		//     A message that explains why the process was terminated, or null if no explanation
		//     is provided.
		//
		//   exception:
		//     An exception that represents the error that caused the termination. This
		//     is typically the exception in a catch block.
		*/ public static void FailFast(string message, Exception exception)                                                              {        System.Environment.FailFast(message, exception)                    ; }
		/**
		// Summary:
		//     Returns a string array containing the command-line arguments for the current
		//     process.
		//
		// Returns:
		//     An array of string where each element contains a command-line argument. The
		//     first element is the executable file name, and the following zero or more
		//     elements contain the remaining command-line arguments.
		//
		// Exceptions:
		//   System.NotSupportedException:
		//     The system does not support command-line arguments.
		*/ public static string[] GetCommandLineArgs()                                                                                   { return System.Environment.GetCommandLineArgs()                            ; }
		/**
		// Summary:
		//     Retrieves the value of an environment variable from the current process.
		//
		// Parameters:
		//   variable:
		//     The name of the environment variable.
		//
		// Returns:
		//     The value of the environment variable specified by variable, or null if the
		//     environment variable is not found.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     variable is null.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permission to perform this operation.
		*/ public static string GetEnvironmentVariable(string variable)                                                                  { return System.Environment.GetEnvironmentVariable(variable)                ; }
		/**
		// Summary:
		//     Retrieves the value of an environment variable from the current process or
		//     from the Windows operating system registry key for the current user or local
		//     machine.
		//
		// Parameters:
		//   variable:
		//     The name of an environment variable.
		//
		//   target:
		//     One of the System.EnvironmentVariableTarget values.
		//
		// Returns:
		//     The value of the environment variable specified by the variable and target
		//     parameters, or null if the environment variable is not found.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     variable is null.
		//
		//   System.NotSupportedException:
		//     target is System.EnvironmentVariableTarget.User or System.EnvironmentVariableTarget.Machine
		//     and the current operating system is Windows 95, Windows 98, or Windows Me.
		//
		//   System.ArgumentException:
		//     target is not a valid System.EnvironmentVariableTarget value.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permission to perform this operation.
		*/ public static string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)                                { return System.Environment.GetEnvironmentVariable(variable, target)        ; }
		/**
		// Summary:
		//     Retrieves all environment variable names and their values from the current
		//     process.
		//
		// Returns:
		//     A dictionary that contains all environment variable names and their values;
		//     otherwise, an empty dictionary if no environment variables are found.
		//
		// Exceptions:
		//   System.Security.SecurityException:
		//     The caller does not have the required permission to perform this operation.
		//
		//   System.OutOfMemoryException:
		//     The buffer is out of memory.
		*/ public static IDictionary GetEnvironmentVariables()                                                                           { return System.Environment.GetEnvironmentVariables()                       ; }
		/**
		// Summary:
		//     Retrieves all environment variable names and their values from the current
		//     process, or from the Windows operating system registry key for the current
		//     user or local machine.
		//
		// Parameters:
		//   target:
		//     One of the System.EnvironmentVariableTarget values.
		//
		// Returns:
		//     A dictionary that contains all environment variable names and their values
		//     from the source specified by the target parameter; otherwise, an empty dictionary
		//     if no environment variables are found.
		//
		// Exceptions:
		//   System.Security.SecurityException:
		//     The caller does not have the required permission to perform this operation
		//     for the specified value of target.
		//
		//   System.NotSupportedException:
		//     This method cannot be used on Windows 95 or Windows 98 platforms.
		//
		//   System.ArgumentException:
		//     target contains an illegal value.
		*/ public static IDictionary GetEnvironmentVariables(EnvironmentVariableTarget target)                                           { return System.Environment.GetEnvironmentVariables(target)                 ; }
		/**
		// Summary:
		//     Gets the path to the system special folder that is identified by the specified
		//     enumeration.
		//
		// Parameters:
		//   folder:
		//     An enumerated constant that identifies a system special folder.
		//
		// Returns:
		//     The path to the specified system special folder, if that folder physically
		//     exists on your computer; otherwise, an empty string ("").A folder will not
		//     physically exist if the operating system did not create it, the existing
		//     folder was deleted, or the folder is a virtual directory, such as My Computer,
		//     which does not correspond to a physical path.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     folder is not a member of System.Environment.SpecialFolder.
		//
		//   System.PlatformNotSupportedException:
		//     The current platform is not supported.
		*/ public static string GetFolderPath(System.Environment.SpecialFolder folder)                                                   { return System.Environment.GetFolderPath(folder)                           ; }
		/**
		// Summary:
		//     Gets the path to the system special folder that is identified by the specified
		//     enumeration, and uses a specified option for accessing special folders.
		//
		// Parameters:
		//   folder:
		//     An enumerated constant that identifies a system special folder.
		//
		//   option:
		//     Specifies options to use for accessing a special folder.
		//
		// Returns:
		//     The path to the specified system special folder, if that folder physically
		//     exists on your computer; otherwise, an empty string ("").A folder will not
		//     physically exist if the operating system did not create it, the existing
		//     folder was deleted, or the folder is a virtual directory, such as My Computer,
		//     which does not correspond to a physical path.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     folder is not a member of System.Environment.SpecialFolder
		//
		//   System.PlatformNotSupportedException:
		//     System.PlatformNotSupportedException
		*/ public static string GetFolderPath(System.Environment.SpecialFolder folder, System.Environment.SpecialFolderOption option)    { return System.Environment.GetFolderPath(folder, option)                   ; }
		/**
		// Summary:
		//     Returns an array of string containing the names of the logical drives on
		//     the current computer.
		//
		// Returns:
		//     An array of strings where each element contains the name of a logical drive.
		//     For example, if the computer's hard drive is the first logical drive, the
		//     first element returned is "C:\".
		//
		// Exceptions:
		//   System.IO.IOException:
		//     An I/O error occurs.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permissions.
		*/ public static string[] GetLogicalDrives()                                                                                     { return System.Environment.GetLogicalDrives()                              ; }
		/**
		// Summary:
		//     Creates, modifies, or deletes an environment variable stored in the current
		//     process.
		//
		// Parameters:
		//   variable:
		//     The name of an environment variable.
		//
		//   value:
		//     A value to assign to variable.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     variable is null.
		//
		//   System.ArgumentException:
		//     variable contains a zero-length string, an initial hexadecimal zero character
		//     (0x00), or an equal sign ("="). -or-The length of variable or value is greater
		//     than or equal to 32,767 characters.-or-An error occurred during the execution
		//     of this operation.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permission to perform this operation.
		*/ public static void SetEnvironmentVariable(string variable, string value)                                                      {        System.Environment.SetEnvironmentVariable(variable, value)         ; }
		/**
		// Summary:
		//     Creates, modifies, or deletes an environment variable stored in the current
		//     process or in the Windows operating system registry key reserved for the
		//     current user or local machine.
		//
		// Parameters:
		//   variable:
		//     The name of an environment variable.
		//
		//   value:
		//     A value to assign to variable.
		//
		//   target:
		//     One of the System.EnvironmentVariableTarget values.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     variable is null.
		//
		//   System.ArgumentException:
		//     variable contains a zero-length string, an initial hexadecimal zero character
		//     (0x00), or an equal sign ("="). -or-The length of variable is greater than
		//     or equal to 32,767 characters.-or-target is not a member of the System.EnvironmentVariableTarget
		//     enumeration. -or-target is System.EnvironmentVariableTarget.Machine or System.EnvironmentVariableTarget.User
		//     and the length of variable is greater than or equal to 255.-or-target is
		//     System.EnvironmentVariableTarget.Process and the length of value is greater
		//     than or equal to 32,767 characters. -or-An error occurred during the execution
		//     of this operation.
		//
		//   System.NotSupportedException:
		//     target is System.EnvironmentVariableTarget.User or System.EnvironmentVariableTarget.Machine
		//     and the current operating system is Windows 95, Windows 98, or Windows Me.
		//
		//   System.Security.SecurityException:
		//     The caller does not have the required permission to perform this operation.
		*/ public static void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target)                    {        System.Environment.SetEnvironmentVariable(variable, value, target) ; }
	}
}
