using System;
using System.Collections.Generic;
using System.Text;

namespace HTLib2
{
    /// http://www.mathworks.com/matlabcentral/answers/93609-how-can-i-create-dedicated-matlab-automation-servers-using-c-in-matlab-7-8-r2009a
    /// How can I create dedicated MATLAB Automation servers using C# in MATLAB 7.8 (R2009a)?
    /// 
    /// This is not possible using the MLApp class as we do not support the Single mode natively.
    /// 
    /// However, it is possible to use the following C# code to mimic the CreateObject behavior
    /// in VB.NET that would allow for dedicated MATLAB Automation Servers.
    /// 
    ///  Type matlab = Type.GetTypeFromProgID("Matlab.Application.Single");
    ///  object matlabObject = Activator.CreateInstance(matlab);
    ///  // Invoke method PutFullMatrix or any other method
    ///  // Here "parameter" should be Object array containing arguments to the method being invoked
    ///  matlab.InvokeMember("PutFullMatrix", System.Reflection.BindingFlags.InvokeMethod, null, matlabObject, parameter);
    /// Then follow the documentation for VB.NET for the dedicated server mode. See the attached example.cs for sample code.
    /// 
    /// http://www.mathworks.com/matlabcentral/answers/uploaded_files/1284/example.cs
    /// private void Foo()
    /// {
    ///     Type matlab = Type.GetTypeFromProgID("Matlab.Application.Single");
    /// 
    ///     object matlab1 = CreateMLApp(matlab);
    ///     object matlab2 = CreateMLApp(matlab);
    /// 
    ///     System.Array pr = new double[4];
    ///     pr.SetValue(11, 0);
    ///     pr.SetValue(12, 1);
    ///     pr.SetValue(13, 2);
    ///     pr.SetValue(14, 3);
    /// 
    ///     System.Array pi = new double[4];
    ///     pi.SetValue(1, 0);
    ///     pi.SetValue(2, 1);
    ///     pi.SetValue(3, 2);
    ///     pi.SetValue(4, 3);
    /// 
    ///     //Variable
    ///     object[] parameter = new object[4];
    ///     parameter[0] = "a";
    ///     parameter[1] = "base";
    ///     parameter[2] = pr;
    ///     parameter[3] = pi;
    /// 
    ///     matlab.InvokeMember("PutFullMatrix", System.Reflection.BindingFlags.InvokeMethod, null, matlab1, parameter);
    ///     parameter[0] = "b";
    ///     matlab.InvokeMember("PutFullMatrix", System.Reflection.BindingFlags.InvokeMethod, null, matlab2, parameter);
    /// }
    /// private object CreateMLApp(Type t)
    /// {
    ///     object matlabObject;
    ///     //Create instance of matlab
    ///     matlabObject = Activator.CreateInstance(t);
    /// 
    ///     return matlabObject;
    /// }
    /// http://stackoverflow.com/questions/23965073/using-matlab-from-c-sharp-update-variable-issue
    /// {
    ///     var acCtx = Type.GetTypeFromProgID("matlab.application.single");
    ///     var matlab = (MLApp.MLApp)Activator.CreateInstance(acCtx);
    ///     matlab.Visible = 0;
    ///     matlab.PutWorkspaceData("Pr", "base", 0);
    ///     onum = new OnlineOlcum();
    ///     try
    ///     {
    ///         onum.xpos = Convert.ToDouble(textBox1.Text);
    ///         onum.ypos = Convert.ToDouble(textBox2.Text);
    ///         label1.Text += " " + onum.xpos + "   " + onum.ypos + "\n";
    ///         
    ///         double t1, t2, t3, t4;
    ///         t1 = Math.Round(T1.Mesafe(onum), 8);
    ///         t2 = Math.Round(T2.Mesafe(onum), 8);
    ///         t3 = Math.Round(T3.Mesafe(onum), 8);
    ///         t4 = Math.Round(T4.Mesafe(onum), 8);
    ///         
    ///         string s1, s2, s3, s4;
    ///         s1 = t1.ToString(new CultureInfo("en-US"));
    ///         s2 = t2.ToString(new CultureInfo("en-US"));
    ///         s3 = t3.ToString(new CultureInfo("en-US"));
    ///         s4 = t4.ToString(new CultureInfo("en-US"));
    ///         
    ///         matlab.Execute("Pr=PowerRecLog(" + s1 + ",1,160,1);");
    ///         onum.lqia = Math.Round((double)matlab.GetVariable("Pr", "base"), 8);
    ///         matlab.Execute("Pr=PowerRecLog(" + s2 + ",1,160,1);");
    ///         onum.lqib = Math.Round((double)matlab.GetVariable("Pr", "base"), 8);
    ///         matlab.Execute("Pr=PowerRecLog(" + s3 + ",1,160,1);");
    ///         onum.lqic = Math.Round((double)matlab.GetVariable("Pr", "base"), 8);
    ///         matlab.Execute("Pr=PowerRecLog(" + s4 + ",1,160,1);");
    ///         onum.lqid = Math.Round((double)matlab.GetVariable("Pr", "base"), 8);
    ///     }
    ///     catch(Exception ex)
    ///     {
    ///         MessageBox.Show(ex.Message);
    ///     }
    ///     
    ///     matlab.Quit();
    /// }

    using Process = System.Diagnostics.Process;
    public partial class Matlab
    {
        public class matlab
        {
            // static MLApp.MLApp _matlab = new MLApp.MLApp();

            class matlabimpl : IDisposable
            {
                dynamic _matlab;    //MLApp.MLApp _matlab;
                public static dynamic CreateMatlabInstance()    //public static MLApp.MLApp CreateMatlabInstance()
                {
                    var acCtx = Type.GetTypeFromProgID("matlab.application.single");
                    //var acCtx = Type.GetTypeFromProgID("matlab.application");
                    dynamic _matlab = Activator.CreateInstance(acCtx); //MLApp.MLApp _matlab = (MLApp.MLApp)Activator.CreateInstance(acCtx);
                    return _matlab;
                }
                public matlabimpl()
                {
                    _matlab = CreateMatlabInstance();
                    //_matlab = new MLApp.MLApp();

                    {
                        /// http://stackoverflow.com/questions/5116429/get-window-instance-from-window-handle
                        //IntPtr handle = currprocess.MainWindowHandle;
                        //System.Windows.Interop.HwndSource hwndSource = System.Windows.Interop.HwndSource.FromHwnd(handle); /// PresentationCore, WindowsBase
                        //System.Windows.Media.Visual    window = hwndSource.RootVisual;// as System.Window;
                    }
                    Process.GetCurrentProcess().Exited               += QuitEventHandler;
                    System.Console.CancelKeyPress                    += QuitEventHandler;
                  //System.Windows.Forms.Application.ApplicationExit += QuitEventHandler;
                    AppDomain.CurrentDomain.ProcessExit              += QuitEventHandler;
                    AppDomain.CurrentDomain.DomainUnload             += QuitEventHandler;
                    AppDomain.CurrentDomain.UnhandledException       += QuitEventHandler;
                }
                public dynamic GetMatlabInterface() //public MLApp.MLApp GetMatlabInterface()
                {
                    return _matlab;
                }
                public void QuitEventHandler(object sender, EventArgs                   e) { Quit(); }
                public void QuitEventHandler(object sender, ConsoleCancelEventArgs      e) { Quit(); }
                public void QuitEventHandler(object sender, UnhandledExceptionEventArgs e) { Quit(); }
                public void Quit()
                {
                    if(_matlab != null)
                    {
                        _matlab.Quit();
                        _matlab = null;
                    }
                }
                void IDisposable.Dispose()
                {
                    _matlab.Execute("exit;");
                }
            }

            static matlabimpl __matlab = null;
            static dynamic _matlab { get { return GetMatlabInterface(); } } //static MLApp.MLApp _matlab { get { return GetMatlabInterface(); } }
            public static dynamic GetMatlabInterface()  //public static MLApp.MLApp GetMatlabInterface()
            {
                int iter = 0;
                dynamic mlapp = (__matlab == null) ? null : __matlab.GetMatlabInterface(); //MLApp.MLApp mlapp = (__matlab == null) ? null : __matlab.GetMatlabInterface();
                while(mlapp == null)
                {
                    iter++;
                    if(iter > 10)
                        return null;
                    __matlab = new matlabimpl();
                    mlapp = __matlab.GetMatlabInterface();
                }
                return mlapp;
            }

            public static int    Visible                                                                                           { get { GetMatlabInterface(); { return _matlab.Visible;                                                                     }; }
                                                                                                                                     set { GetMatlabInterface(); {        _matlab.Visible = value;                                                             }; } }
            public static string Execute              (string Name                                                                     ) { GetMatlabInterface(); { return _matlab.Execute              (Name                                                ); }; }
            public static string GetCharArray         (string Name, string Workspace                                                   ) { GetMatlabInterface(); { return _matlab.GetCharArray         (Name, Workspace                                     ); }; }
            public static void   GetFullMatrix        (string Name, string Workspace, ref Array pr, ref Array pi                       ) { GetMatlabInterface(); {        _matlab.GetFullMatrix        (Name, Workspace, ref pr, ref pi                     ); }; }
            public static object GetVariable          (string Name, string Workspace                                                   ) { GetMatlabInterface(); { return _matlab.GetVariable          (Name, Workspace                                     ); }; }
            public static void   GetWorkspaceData     (string Name, string Workspace, out object pdata                                 ) { GetMatlabInterface(); {        _matlab.GetWorkspaceData     (Name, Workspace, out pdata                          ); }; }
            public static void   MaximizeCommandWindow(                                                                                ) { GetMatlabInterface(); {        _matlab.MaximizeCommandWindow(                                                    ); }; }
            public static void   MinimizeCommandWindow(                                                                                ) { GetMatlabInterface(); {        _matlab.MinimizeCommandWindow(                                                    ); }; }
            public static void   PutCharArray         (string Name, string Workspace, string charArray                                 ) { GetMatlabInterface(); {        _matlab.PutCharArray         (Name, Workspace, charArray                          ); }; }
            public static void   PutFullMatrix        (string Name, string Workspace, Array pr, Array pi                               ) { GetMatlabInterface(); {        _matlab.PutFullMatrix        (Name, Workspace, pr, pi                             ); }; }
            public static void   PutWorkspaceData     (string Name, string Workspace, object data                                      ) { GetMatlabInterface(); {        _matlab.PutWorkspaceData     (Name, Workspace, data                               ); }; }
            public static void   XLEval               (string bstrName, int nargout, ref object pvarArgOut, int nargin, object varArgIn) { GetMatlabInterface(); {        _matlab.XLEval               (bstrName, nargout, ref pvarArgOut, nargin, varArgIn ); }; }
            public static void   Quit()
            {
                if(__matlab != null)
                {
                    __matlab.Quit();
                    __matlab = null;
                }
            }

          //public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1 = Type.Missing, object arg2 = Type.Missing, object arg3 = Type.Missing, object arg4 = Type.Missing, object arg5 = Type.Missing, object arg6 = Type.Missing, object arg7 = Type.Missing, object arg8 = Type.Missing, object arg9 = Type.Missing, object arg10 = Type.Missing, object arg11 = Type.Missing, object arg12 = Type.Missing, object arg13 = Type.Missing, object arg14 = Type.Missing, object arg15 = Type.Missing, object arg16 = Type.Missing, object arg17 = Type.Missing, object arg18 = Type.Missing, object arg19 = Type.Missing, object arg20 = Type.Missing, object arg21 = Type.Missing, object arg22 = Type.Missing, object arg23 = Type.Missing, object arg24 = Type.Missing, object arg25 = Type.Missing, object arg26 = Type.Missing, object arg27 = Type.Missing, object arg28 = Type.Missing, object arg29 = Type.Missing, object arg30 = Type.Missing, object arg31 = Type.Missing, object arg32 = Type.Missing) { lock(_matlab) {        _matlab.Feval           (bstrName, nargout, out pvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30, arg31, arg32); } }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut)                                                                                                                                                                                                                                                                                                                                                                                                                                                               { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut);                                                                                                                                                                                                                         }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1)                                                                                                                                                                                                                                                                                                                                                                                                                                                  { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1);                                                                                                                                                                                                                   }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2)                                                                                                                                                                                                                                                                                                                                                                                                                                     { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2);                                                                                                                                                                                                             }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3)                                                                                                                                                                                                                                                                                                                                                                                                                        { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3);                                                                                                                                                                                                       }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4)                                                                                                                                                                                                                                                                                                                                                                                                           { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4);                                                                                                                                                                                                 }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5)                                                                                                                                                                                                                                                                                                                                                                                              { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5);                                                                                                                                                                                           }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6)                                                                                                                                                                                                                                                                                                                                                                                 { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6);                                                                                                                                                                                     }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7)                                                                                                                                                                                                                                                                                                                                                                    { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7);                                                                                                                                                                               }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8)                                                                                                                                                                                                                                                                                                                                                       { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);                                                                                                                                                                         }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9)                                                                                                                                                                                                                                                                                                                                          { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);                                                                                                                                                                   }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10)                                                                                                                                                                                                                                                                                                                            { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);                                                                                                                                                            }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11)                                                                                                                                                                                                                                                                                                              { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);                                                                                                                                                     }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12)                                                                                                                                                                                                                                                                                                { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);                                                                                                                                              }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13)                                                                                                                                                                                                                                                                                  { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);                                                                                                                                       }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14)                                                                                                                                                                                                                                                                    { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);                                                                                                                                }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15)                                                                                                                                                                                                                                                      { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);                                                                                                                         }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16)                                                                                                                                                                                                                                        { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);                                                                                                                  }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17)                                                                                                                                                                                                                          { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);                                                                                                           }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18)                                                                                                                                                                                                            { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);                                                                                                    }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19)                                                                                                                                                                                              { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);                                                                                             }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20)                                                                                                                                                                                { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);                                                                                      }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21)                                                                                                                                                                  { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21);                                                                               }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22)                                                                                                                                                    { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22);                                                                        }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23)                                                                                                                                      { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23);                                                                 }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24)                                                                                                                        { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24);                                                          }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25)                                                                                                          { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25);                                                   }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26)                                                                                            { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26);                                            }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27)                                                                              { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27);                                     }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28)                                                                { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28);                              }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28, object arg29)                                                  { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29);                       }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28, object arg29, object arg30)                                    { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30);                }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28, object arg29, object arg30, object arg31)                      { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30,  arg31);        }; pvarArgOut=lpvarArgOut; }
            public static void Feval(string bstrName, int nargout, out object pvarArgOut, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9, object arg10, object arg11, object arg12, object arg13, object arg14, object arg15, object arg16, object arg17, object arg18, object arg19, object arg20, object arg21, object arg22, object arg23, object arg24, object arg25, object arg26, object arg27, object arg28, object arg29, object arg30, object arg31, object arg32)        { object lpvarArgOut=null; { _matlab.Feval(bstrName, nargout, out lpvarArgOut, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20, arg21, arg22, arg23, arg24, arg25, arg26, arg27, arg28, arg29, arg30,  arg31, arg32); }; pvarArgOut=lpvarArgOut; }
        }
    }
}
