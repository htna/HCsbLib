using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wolfram.NETLink;
//using System.Drawing;

namespace HTLib2
{
	public partial class Mathematica
	{
        public static void RegisterMathLinkFactory
            ( string dir_ml64_dll = @"C:\Program Files\Wolfram Research\Mathematica\11.3\SystemFiles\Links\MathLink\DeveloperKit\Windows-x86-64\SystemAdditions\"
            )
        {
            HMathLinkFactory.RegisterMathLinkFactory
                ( dir_ml64_dll
                );
        }

        public class HMathLinkFactory
        {
            static private bool _addpath = true;
            static public void RegisterMathLinkFactory
                ( string dir_ml64_dll = @"C:\Program Files\Wolfram Research\Mathematica\11.3\SystemFiles\Links\MathLink\DeveloperKit\Windows-x86-64\SystemAdditions\"
                , string dir_ml32_dll = @"C:\Program Files\Wolfram Research\Mathematica\11.3\SystemFiles\Links\MathLink\DeveloperKit\Windows\SystemAdditions\"
                )
            {
                if(_addpath == false)
                    return;
                _addpath = false;

                //  /// https://stackoverflow.com/questions/22093715/how-to-set-environment-variable-path-using-c-sharp
                //  const string name   = "PATH";
                //  string pathvar = System.Environment.GetEnvironmentVariable(name);
                //  var value  = pathvar + ";" + dir_ml64_dll;
                //  var target = EnvironmentVariableTarget.Process; //EnvironmentVariableTarget.Machine;
                //  System.Environment.SetEnvironmentVariable(name, value, target);

                //  /// https://stackoverflow.com/questions/1892492/set-custom-path-to-referenced-dlls
                //  AppDomain.CurrentDomain.AppendPrivatePath(dir_ml64_dll);

                Environment.SetEnvironmentVariable
                    ( "PATH"
                    , Environment.GetEnvironmentVariable("PATH") + ";" + dir_ml64_dll + ";" + dir_ml32_dll
                    );
            }

            Wolfram.NETLink.MathLinkFactory mathLinkFactory;
            public HMathLinkFactory() : base()
            {
                RegisterMathLinkFactory();
                mathLinkFactory = new MathLinkFactory();
            }

            public static Wolfram.NETLink.IKernelLink   CreateKernelLink()               { RegisterMathLinkFactory(); return Wolfram.NETLink.MathLinkFactory.CreateKernelLink();        }
            public static Wolfram.NETLink.IKernelLink   CreateKernelLink(string cmdLine) { RegisterMathLinkFactory(); return Wolfram.NETLink.MathLinkFactory.CreateKernelLink(cmdLine); }
            public static Wolfram.NETLink.IKernelLink   CreateKernelLink(string[] argv)  { RegisterMathLinkFactory(); return Wolfram.NETLink.MathLinkFactory.CreateKernelLink(argv);    }
            public static Wolfram.NETLink.IKernelLink   CreateKernelLink(IMathLink ml)   { RegisterMathLinkFactory(); return Wolfram.NETLink.MathLinkFactory.CreateKernelLink(ml);      }
            public static Wolfram.NETLink.ILoopbackLink CreateLoopbackLink()             { RegisterMathLinkFactory(); return Wolfram.NETLink.MathLinkFactory.CreateLoopbackLink();      }
            public static Wolfram.NETLink.IMathLink     CreateMathLink()                 { RegisterMathLinkFactory(); return Wolfram.NETLink.MathLinkFactory.CreateMathLink();          }
            public static Wolfram.NETLink.IMathLink     CreateMathLink(string cmdLine)   { RegisterMathLinkFactory(); return Wolfram.NETLink.MathLinkFactory.CreateMathLink(cmdLine);   }
            public static Wolfram.NETLink.IMathLink     CreateMathLink(string[] argv)    { RegisterMathLinkFactory(); return Wolfram.NETLink.MathLinkFactory.CreateMathLink(argv);      }
        }


        public static object   EvaluateObject     (string evaluate) { return Evaluate(evaluate, delegate(IKernelLink ml) { return ml.GetObject     (); }); }
        public static double   EvaluateDouble     (string evaluate) { return Evaluate(evaluate, delegate(IKernelLink ml) { return ml.GetDouble     (); }); }
        public static double[] EvaluateDoubleArray(string evaluate) { return Evaluate(evaluate, delegate(IKernelLink ml) { return ml.GetDoubleArray(); }); }

        public static T Evaluate<T>(string evaluate, Func<IKernelLink,T> GetT)
        {
            IKernelLink ml = HMathLinkFactory.CreateKernelLink();
            ml.WaitAndDiscardAnswer();
            ml.Evaluate(evaluate);
            ml.WaitForAnswer();

            T result;
            try
            {
                result = GetT(ml);
                //result = ml.GetDoubleArray();
                //result = ml.GetObject();
            }
            catch
            {
                result = default(T);
                //result = null;
            }
            ml.Close();
            return result;
        }

        public static void EvaluatePng(string evaluate, int width, int height, string pngpath)
        {
            throw new NotImplementedException();
        }
        public class Image
        {
            //  public System.Drawing.Image image;
            //  public Image(System.Drawing.Image image)
            //  {
            //      this.image = image;
            //  }
        };
        public static Image EvaluateImage(string evaluate, int width, int height)
        {
            throw new NotImplementedException();
            /// IKernelLink ml = HMathLinkFactory.CreateKernelLink();
            /// Image image;
            /// try
            /// {
            ///     ml.WaitAndDiscardAnswer();
            ///     image = ml.EvaluateToImage(evaluate, width, height);
            ///     //ml.WaitForAnswer();
            /// }
            /// catch
            /// {
            ///     image = null;
            /// }
            /// ml.Close();
            /// return image;
        }
        public static void EvaluatePng(string evaluate, int width, string pngpath)
        {
            throw new NotImplementedException();
            /// Image image = EvaluateImage(evaluate, width);
            /// image.Save(pngpath);
        }
        public static Image EvaluateImage(string evaluate, int width)
        {
            throw new NotImplementedException();
            /// IKernelLink ml = HMathLinkFactory.CreateKernelLink();
            /// Image image;
            /// try
            /// {
            ///     ml.WaitAndDiscardAnswer();
            ///     image = ml.EvaluateToTypeset(evaluate, width);
            ///     //ml.WaitForAnswer();
            /// }
            /// catch
            /// {
            ///     image = null;
            /// }
            /// ml.Close();
            /// return image;
        }
    }
}
