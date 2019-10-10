using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wolfram.NETLink;
using System.Drawing;

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

        public static KernelLinkDelegate CreateKernelLinkDelegate()
        {
            IKernelLink ml = HMathLinkFactory.CreateKernelLink();
            ml.WaitAndDiscardAnswer();
            return new KernelLinkDelegate
            {
                ml = ml,
            };
        }
        public class KernelLinkDelegate
        {
            public Wolfram.NETLink.IKernelLink ml;

            public Exception LastError                                          { get { return ml.LastError          ; } }
            public bool TypesetStandardForm                                     { get { return ml.TypesetStandardForm; } set { value = ml.TypesetStandardForm; } }
            public string GraphicsFormat                                        { get { return ml.GraphicsFormat     ; } set { value = ml.GraphicsFormat     ; } }
            public bool UseFrontEnd                                             { get { return ml.UseFrontEnd        ; } set { value = ml.UseFrontEnd        ; } }
            public bool WasInterrupted                                          { get { return ml.WasInterrupted     ; } set { value = ml.WasInterrupted     ; } }
          //public event PacketHandler PacketArrived                            { get { return ml.PacketArrived; } }
            public void AbandonEvaluation()                                     {        ml.AbandonEvaluation()                         ; }
            public void AbortEvaluation()                                       {        ml.AbortEvaluation()                           ; }
            public void BeginManual()                                           {        ml.BeginManual()                               ; }
            public void EnableObjectReferences()                                {        ml.EnableObjectReferences()                    ; }
            public void Evaluate(string s)                                      {        ml.Evaluate(s)                                 ; }
            public void Evaluate(Expr e)                                        {        ml.Evaluate(e)                                 ; }
            public Image EvaluateToImage(Expr e, int width, int height)         { return new Image(ml.EvaluateToImage(e, width, height)); }
            public Image EvaluateToImage(string s, int width, int height)       { return new Image(ml.EvaluateToImage(s, width, height)); }
            public string EvaluateToInputForm(string s, int pageWidth)          { return ml.EvaluateToInputForm(s, pageWidth)           ; }
            public string EvaluateToInputForm(Expr e, int pageWidth)            { return ml.EvaluateToInputForm(e, pageWidth)           ; }
            public string EvaluateToOutputForm(string s, int pageWidth)         { return ml.EvaluateToOutputForm(s, pageWidth)          ; }
            public string EvaluateToOutputForm(Expr e, int pageWidth)           { return ml.EvaluateToOutputForm(e, pageWidth)          ; }
            public Image EvaluateToTypeset(Expr e, int width)                   { return new Image(ml.EvaluateToTypeset(e, width))      ; }
            public Image EvaluateToTypeset(string s, int width)                 { return new Image(ml.EvaluateToTypeset(s, width))      ; }
            public Array GetArray(Type leafType, int depth)                     { return ml.GetArray(leafType, depth)                   ; }
            public Array GetArray(Type leafType, int depth, out string[] heads) { return ml.GetArray(leafType, depth, out heads)        ; }
            public ExpressionType GetExpressionType()                           { return ml.GetExpressionType()                         ; }
            public ExpressionType GetNextExpressionType()                       { return ml.GetNextExpressionType()                     ; }
            public object GetObject()                                           { return ml.GetObject()                                 ; }
            public void HandlePacket(PacketType pkt)                            {        ml.HandlePacket(pkt)                           ; }
            public void InterruptEvaluation()                                   {        ml.InterruptEvaluation()                       ; }
            public void Message(string symtag, params string[] args)            {        ml.Message(symtag, args)                       ; }
            public bool OnPacketArrived(PacketType pkt)                         { return ml.OnPacketArrived(pkt)                        ; }
            public void Print(string s)                                         {        ml.Print(s)                                    ; }
            public void Put(object obj)                                         {        ml.Put(obj)                                    ; }
            public void PutReference(object obj, Type t)                        {        ml.PutReference(obj, t)                        ; }
            public void PutReference(object obj)                                {        ml.PutReference(obj)                           ; }
            public void TerminateKernel()                                       {        ml.TerminateKernel()                           ; }
            public void WaitAndDiscardAnswer()                                  {        ml.WaitAndDiscardAnswer()                      ; }
            public PacketType WaitForAnswer()                                   { return ml.WaitForAnswer()                             ; }
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
            public System.Drawing.Image image;
            public Image(System.Drawing.Image image)
            {
                this.image = image;
            }
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
